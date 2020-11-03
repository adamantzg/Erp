
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;
using company.Common;
using Dapper;

namespace erp.Model.Dal.New
{
	public class InvoicesDAL : GenericDal<Invoices>, IInvoicesDAL
	{
		private IInvoiceLinesDAL invoiceLinesDal;
		private ICreditNoteLinesDAL creditNoteLinesDal;
		private ICompanyDAL companyDAL;
		private IReturnsDAL returnsDal;

		public InvoicesDAL(IDbConnection conn, IInvoiceLinesDAL invoiceLinesDal, ICreditNoteLinesDAL creditNoteLinesDal,
				ICompanyDAL companyDAL, IReturnsDAL returnsDal) : base(conn)
		{
			this.returnsDal = returnsDal;
			this.invoiceLinesDal = invoiceLinesDal;
			this.creditNoteLinesDal = creditNoteLinesDal;
			this.companyDAL = companyDAL;
		}
		
		public override Invoices GetById(object id)
		{
			Invoices inv = null;
			conn.Query<Payment_details, Invoices, Company, Company, Invoice_lines, Invoices>(
				@"SELECT payment_details.*, invoices.*, client.*, companyFrom.*, invoice_lines.*  
					FROM invoices INNER JOIN users client ON invoices.userid1 = client.user_id 
					LEFT OUTER JOIN users companyFrom ON invoices.invoice_from = companyFrom.user_id
					LEFT OUTER JOIN payment_details ON invoices.payment_details_id = payment_details.payment_details_id
					LEFT OUTER JOIN invoice_lines ON invoices.invoice = invoice_lines.invoice_id
					WHERE invoices.invoice = @id",
					(pd, i, c, from, il) =>
					{
						if(inv == null)
						{
							inv = i;
							inv.Client = c;
							inv.From = from;
							inv.Payment = pd;
							inv.Lines = new List<Invoice_lines>();
						}						
						inv.Lines.Add(il);
						return inv;
					}, new {id}, splitOn: "invoice, user_id, user_id, linenum"
				).FirstOrDefault();
			if(inv != null && inv.From?.invoice_sequence != null)
			{
				inv.eb_invoice = conn.ExecuteScalar<int?>("SELECT sequence FROM order_invoice_sequence WHERE invoiceid = @id", new {id });
			}				
			return inv;
		}

		
		public void Create(Invoices o, bool createEBinvoice = true, int? invoice_sequence_type = null)
		{
			string insertsql = @"INSERT INTO invoices (invoice,orderid,invdate,cidate,brsinvdate,userid1,locid,exch_rate,status,delivery_address1,delivery_address2,delivery_address3,
						delivery_address4,delivery_address5,invoice_address1,invoice_address2,invoice_address3,invoice_address4,invoice_address5,currency,notes,inv_amount,inv_amount2,
					inv_amount3,inv_payment,int_payment_tmp,duedate1,duedate2,sea_freight,duty,local_charge,invoice_number,invoice_type_id,invoice_from,reference_number,eta,trading_term,
					payment_details_id,invoice_no,eb_invoice,confirmed,cprod_user,vat_applicable,rebate_type,dealer_id) 
					VALUES(@invoice,@orderid,@invdate,@cidate,@brsinvdate,@userid1,@locid,@exch_rate,@status,@delivery_address1,@delivery_address2,@delivery_address3,@delivery_address4,
							@delivery_address5,@invoice_address1,@invoice_address2,@invoice_address3,@invoice_address4,@invoice_address5,@currency,@notes,@inv_amount,@inv_amount2,@inv_amount3,
					@inv_payment,@int_payment_tmp,@duedate1,@duedate2,@sea_freight,@duty,@local_charge,@invoice_number,@invoice_type_id,@invoice_from,@reference_number,@eta,@trading_term,
					@payment_details_id,@invoice_no,@eb_invoice,@confirmed,@cprod_user,@vat_applicable,@rebate_type,@dealer_id)";

			if(conn.State != ConnectionState.Open)
				conn.Open();
			var tr = conn.BeginTransaction();
			try
			{	
				o.invoice = conn.ExecuteScalar<int>("SELECT MAX(invoice)+1 FROM invoices");
			    if (createEBinvoice)
                {
                    if (invoice_sequence_type == null)
                    {
                        o.eb_invoice = conn.ExecuteScalar<int>(
							@"SELECT MAX(eb_invoice)+1 FROM (SELECT eb_invoice FROM order_header 
							UNION SELECT eb_invoice FROM invoices) AS header_invoices");
                    }                   				    
                }
				conn.Execute(insertsql, o, tr);

                if(invoice_sequence_type != null) {

					conn.Execute(
						@"INSERT INTO order_invoice_sequence(invoiceid,sequence, type) 
						SELECT @invoice, (SELECT COALESCE(MAX(sequence),0)+1 FROM order_invoice_sequence WHERE type = @invoice_sequence_type), @invoice_sequence_type",
						new {o.invoice, invoice_sequence_type }, tr);                    
                }

				if (o.Lines != null)
				{
					foreach (var line in o.Lines)
					{
						line.invoice_id = o.invoice;
						invoiceLinesDal.Create(line, tr);
					}
				}
				if (o.CreditnoteLines != null)
				{
					foreach (var cnline in o.CreditnoteLines)
					{
						cnline.invoice_id = o.invoice;
						creditNoteLinesDal.Create(cnline, tr);
					}
				}
				tr.Commit();
			}
			catch
			{
				tr.Rollback();
				throw;
			}			
			
		}

	    public void ActivateCreditNote(int id)
	    {

			conn.Execute(@"UPDATE invoices SET status = NULL, 
			eb_invoice = (SELECT MAX(eb_invoice)+1 FROM (SELECT eb_invoice FROM order_header 
						UNION SELECT eb_invoice FROM invoices) AS invoiceunion) WHERE invoice = @id", new {id});	        
	    }

        public int GetLastOrderId()
        {
			return conn.ExecuteScalar<int>("SELECT MAX(orderid)+1 FROM invoices");            
        }
		
				
		public void Update(Invoices o, List<Invoice_lines> deletedLines )
		{
			string updatesql = @"UPDATE invoices SET invoice_type_id = @invoice_type_id,orderid = @orderid,invdate = @invdate,cidate = @cidate,brsinvdate = @brsinvdate,userid1 = @userid1,
						locid = @locid,exch_rate = @exch_rate,status = @status,delivery_address1 = @delivery_address1,delivery_address2 = @delivery_address2,delivery_address3 = @delivery_address3,
								delivery_address4 = @delivery_address4,delivery_address5 = @delivery_address5,invoice_address1 = @invoice_address1,invoice_address2 = @invoice_address2,invoice_address3 = @invoice_address3,invoice_address4 = @invoice_address4,invoice_address5 = @invoice_address5,currency = @currency,notes = @notes,inv_amount = @inv_amount,inv_amount2 = @inv_amount2,inv_amount3 = @inv_amount3,inv_payment = @inv_payment,int_payment_tmp = @int_payment_tmp,duedate1 = @duedate1,duedate2 = @duedate2,sea_freight = @sea_freight,duty = @duty,local_charge = @local_charge,invoice_number = @invoice_number,invoice_type_id = @invoice_type_id,invoice_from = @invoice_from,reference_number = @reference_number,eta = @eta,trading_term = @trading_term,payment_details_id = @payment_details_id,invoice_no = @invoice_no,eb_invoice = @eb_invoice,
								confirmed = @confirmed,cprod_user = @cprod_user,vat_applicable = @vat_applicable, rebate_type=@rebate_type, dealer_id=@dealer_id  WHERE invoice = @invoice";

			if(conn.State != ConnectionState.Open)
				conn.Open();
			var tr = conn.BeginTransaction();
			try
			{
				conn.Execute(updatesql, o, tr);

				if (o.Lines != null)
				{
					foreach (var line in o.Lines)
					{
						if (line.linenum <= 0)
							invoiceLinesDal.Create(line, tr);
						else
						{
							invoiceLinesDal.Update(line, tr);
						}
					}
				}
				foreach (var d in deletedLines)
				{
					invoiceLinesDal.Delete(d.linenum, tr);
				}

				//Update lines
				tr.Commit();
			}
			catch
			{
				tr.Rollback();
				throw;
			}
			
		}

		protected override string GetAllSql()
		{
			throw new NotImplementedException();
		}

		protected override string GetByIdSql()
		{
			throw new NotImplementedException();
		}

		protected override string GetCreateSql()
		{
			throw new NotImplementedException();
		}

		protected override string GetUpdateSql()
		{
			throw new NotImplementedException();
		}

		protected override string GetDeleteSql()
		{
			return "DELETE FROM invoices WHERE invoice = @id";
		}

		
		public List<Invoices> GetByCriteria(List<int> custids, DateTime? from, DateTime? to,bool? brands = null, bool filterbyStatus = true, bool excludeOrders = true, int? invoice_sequence = null)
		{
			 return conn.Query<Invoices, Company, Company, Invoices>(
				$@"SELECT invoices.*, {(invoice_sequence != null ? "order_invoice_sequence.sequence," : "")} 
						(SELECT SUM(CASE qty_type WHEN 1 THEN orderqty*unitprice ELSE orderqty/100*unitprice END) 
							FROM invoice_lines WHERE invoice_id = invoices.invoice) AS Amount,
						(SELECT SUM(unitprice*quantity) FROM creditnote_line WHERE invoice_id = invoices.invoice) AS AmountCN,
						client.*, from_user.*,
                        from_user.user_name as from_user_name, from_user.customer_code as from_customer_code
                        FROM invoices INNER JOIN users client ON invoices.userid1 = client.user_id
						LEFT OUTER JOIN users from_user ON invoices.invoice_from = from_user.user_id
                        {(invoice_sequence != null ? " INNER JOIN order_invoice_sequence  ON invoices.invoice = order_invoice_sequence.invoiceid " : "")}
						WHERE (invdate >= @from OR @from IS NULL) AND (invdate <= @to OR @TO IS NULL) 
						{(brands == true ? " AND client.distributor > 0" : brands == false ? " AND client.distributor <= 0 " : "")}
						{(custids != null ? " AND invoices.userid1 IN @custids" : "")} 
						{(excludeOrders ? " AND invoices.orderid IS NULL " : "")} 
						{(filterbyStatus ? " AND COALESCE(status,'') <> 'T'" : "")} 
						{(invoice_sequence != null ? " AND order_invoice_sequence.type = @invoice_sequence" : "")}",
					(i, c, f) =>
					{
						i.Client = c;
						i.From = f;
						if(i.sequence != null && i.eb_invoice == null)
							i.eb_invoice = i.sequence;
						return i;
					}, 	new {from, to, invoice_sequence}, splitOn: "user_id, user_id").ToList();				
                
		}

		public Invoices GetCreditNoteByCriteria(int client_id,int? brand_user, DateTime invoiceDate)
		{
			Invoices inv = null;
			conn.Query<Invoices, Company, Company, Creditnote_line, Invoices >(
				$@"SELECT invoices.*,order_invoice_sequence.sequence, client.*, companyFrom.*, Creditnote_line.* 
					FROM invoices INNER JOIN users client ON invoices.userid1 = client.user_id
					LEFT OUTER JOIN users companyFrom ON invoices.invoices_from = companyfrom.user_id
					LEFT OUTER order_invoice_sequence ON invoices.invoice = order_invoice_sequence.invoiceid
					LEFT OUTER creditnote_line ON creditnote_line.invoice_id = invoices.invoice
					WHERE invoices.userid1 = @client_id AND invoice_type_id = {Invoice_type.CreditNoteReturn}  
                    AND COALESCE(status,'') <> 'T' 
                    AND (invoices.cprod_user = @brand_user OR @brand_user IS NULL) AND invoices.invdate = @invoiceDate",
				(i, c, f, cnl) =>
				{
					if(inv == null)
					{
						inv = i;
						inv.Client = c;
						inv.From = f;
						inv.CreditnoteLines = new List<Creditnote_line>();
					}
					inv.CreditnoteLines.Add(cnl);
					return inv;
				}, new {client_id, brand_user, invoiceDate }, splitOn: "user_id, user_id, line_id"
				).ToList();
			return inv;
		}

		public void CreateReturnCreditNotes(DateTime? from, DateTime? to, IList<int> excludedClientIds = null, IList<int> includedClientsIds = null, DateTime? invoiceDate = null,
            Dictionary<int, int> companyMappings = null, IList<int> excludedClientsForEBInvoice = null, int? invoice_from = null, int? invoice_from_nonUK = null, IList<int> ukDistributors_Exceptions = null, IList<int> brands = null)
		{
			var returns = returnsDal.GetInPeriod(from, to).Where(r=>r.claim_value > 0 || r.credit_value > 0).ToList();


            //If needed, merge companies e.g. if Froy has associated company, it should show as Froys
		    if (companyMappings != null && companyMappings.Count > 0)
		    {
		        foreach (var r in returns)
		        {
		            if (r.client_id != null)
		            {
                        if (companyMappings.ContainsKey(r.client_id.Value))
                        {
                            r.client_id = companyMappings[r.client_id.Value];
                        }    
		            }
		        }
		    }

		    Company invoiceFrom = null;
		    Dictionary<int, Company> fromCompanies = new Dictionary<int, Company>();
		    if (invoice_from != null)
		        fromCompanies[invoice_from.Value] = companyDAL.GetById(invoice_from.Value);
            if (invoice_from_nonUK != null)
                fromCompanies[invoice_from_nonUK.Value] = companyDAL.GetById(invoice_from_nonUK.Value);


            if (invoiceDate == null)
		        invoiceDate = DateTime.Today;
			foreach (var returnsByClient in returns.GroupBy(r=>r.client_id))
			{
				if (returnsByClient.Key != null && (excludedClientIds == null || !excludedClientIds.Contains(returnsByClient.Key.Value)) && (includedClientsIds == null || includedClientsIds.Contains(returnsByClient.Key.Value)))
				{
					var client = companyDAL.GetById(returnsByClient.Key.Value);
				    var createEBInvoice = excludedClientsForEBInvoice == null ||
				                          !excludedClientsForEBInvoice.Contains(client.user_id);
					if (client.distributor > 0)
					{
						foreach (var returnByBrand in returnsByClient.GroupBy(r=>r.Product.cprod_user))
						{
							if (returnByBrand.Key != null && (brands == null || brands.Contains(returnByBrand.Key.Value )))
							{
								var brand_company = companyDAL.GetById(returnByBrand.Key.Value);
								var inv = new Invoices
									{
										invdate = invoiceDate,
										userid1 = client.user_id,
										invoice_address1 = client.user_address1,
										invoice_address2 = client.user_address2,
										invoice_address3 = client.user_address3,
										invoice_address4 = client.user_address4,
										invoice_address5 = client.user_address5,
										invoice_type_id = Invoice_type.CreditNoteReturn,
										currency =  client.user_curr,
										vat_applicable = client.user_country.In("UK","GB"),
										cprod_user = brand_company.user_id,
										CreditnoteLines = new List<Creditnote_line>(),
                                        invoice_from = client.user_country.In("UK","GB","IE") && ukDistributors_Exceptions?.Contains(client.user_id) != true ? invoice_from : invoice_from_nonUK
									};
							    invoiceFrom = invoice_from != null && fromCompanies.ContainsKey(inv.invoice_from.Value)
							        ? fromCompanies[inv.invoice_from.Value]
							        : null;
                                foreach (var r in returnByBrand.OrderBy(re=>re.returnsid))
								{
									inv.CreditnoteLines.Add(new Creditnote_line{client_ref = r.reference,cprod_code = r.Product.cprod_code1,
											cprod_name = r.claim_type == Returns.ClaimType_Refit ? "installation/other" : r.Product.cprod_name,quantity = r.claim_type == Returns.ClaimType_Refit ? 1 : r.return_qty,
											unitprice = r.claim_type == Returns.ClaimType_Refit ? r.claim_value : r.credit_value_override > 0 ? r.credit_value_override : r.credit_value,return_no = r.return_no,overridden = r.ebuk == 1});
								}
								Create(inv,createEBInvoice,invoiceFrom?.invoice_sequence );

							}
						}
					}
					else
					{
						var inv = new Invoices
						{
							invdate = invoiceDate,
							userid1 = client.user_id,
							invoice_address1 = client.user_address1,
							invoice_address2 = client.user_address2,
							invoice_address3 = client.user_address3,
							invoice_address4 = client.user_address4,
							invoice_address5 = client.user_address5,
							invoice_type_id = 4,
							currency = client.user_curr,
							vat_applicable = false,
							CreditnoteLines = new List<Creditnote_line>(),
                            invoice_from = client.user_country.In("UK", "GB", "IE") && ukDistributors_Exceptions?.Contains(client.user_id) != true ? invoice_from : invoice_from_nonUK
                        };
                        invoiceFrom = invoice_from != null && fromCompanies.ContainsKey(inv.invoice_from.Value)
                                    ? fromCompanies[inv.invoice_from.Value]
                                    : null;
                        foreach (var r in returnsByClient.OrderBy(re=>re.returnsid) )
						{
							inv.CreditnoteLines.Add(new Creditnote_line { client_ref = r.reference, cprod_code = r.Product.cprod_code1,
									cprod_name = r.claim_type == 2 ? "installation/other" : r.Product.cprod_name,
									quantity = r.claim_type == 2 ? 1 : r.return_qty, 
									unitprice = r.claim_type == 2 ? r.claim_value : r.credit_value_override > 0 ? r.credit_value_override : r.credit_value, return_no = r.return_no,overridden = r.ebuk == 1});

						}
						Create(inv,createEBInvoice, invoiceFrom?.invoice_sequence);
					}
				}
			}
		}

		public Invoices GetByOrder(int? orderid, IDbConnection conn = null)
		{
			return conn.QueryFirstOrDefault<Invoices>(
				
				@"SELECT Invoices.*,order_invoice_sequence.sequence,order_invoice_sequence.type as invoice_sequence_type 
				FROM Invoices LEFT OUTER JOIN order_header ON order_header.orderid = invoices.orderid 
				LEFT OUTER JOIN order_invoice_sequence ON order_header.orderid = order_invoice_sequence.orderid 
				WHERE Invoices.orderid = @orderid  AND COALESCE(Invoices.status,'') <> 'T'",
				new {orderid });			
		}

        public List<Invoices> GetForDealer(int id)
        {
			return conn.Query<Invoices>(
				"SELECT * FROM invoices WHERE dealer_id=@id", new {id }).ToList();            
        }

		
	}
}
			
			
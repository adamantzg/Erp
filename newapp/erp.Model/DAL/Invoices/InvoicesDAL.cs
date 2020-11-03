
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace asaq2.Model
{
    public class InvoicesDAL
	{
	
		public static List<Invoices> GetAll()
		{
			var result = new List<Invoices>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand("SELECT * FROM invoices", conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
		}
		
		
		public static Invoices GetById(int id)
		{
			Invoices result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand("SELECT invoices.*, users.* FROM invoices INNER JOIN users ON invoices.userid1 = users.user_id WHERE invoice = @id", conn);
				cmd.Parameters.AddWithValue("@id", id);
                var dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    result = GetFromDataReader(dr);
                    result.Client = CompanyDAL.GetFromDataReader(dr);
                    if(result.invoice_from != null)
                        result.From = CompanyDAL.GetById(result.invoice_from.Value);
                    if (result.payment_details_id != null)
                        result.Payment = Payment_detailsDAL.GetById(result.payment_details_id.Value);
                    result.Lines = Invoice_linesDAL.GetByInvoice(id);
                }
                dr.Close();
            }
			return result;
		}
		
	
		private static Invoices GetFromDataReader(MySqlDataReader dr)
		{
			Invoices o = new Invoices();
		
			o.invoice =  (int) dr["invoice"];
			o.orderid =  Utilities.FromDbValue<int>(dr["orderid"]);
			o.invdate = Utilities.FromDbValue<DateTime>(Utilities.GetReaderField(dr,"invdate"));
			o.cidate = Utilities.FromDbValue<DateTime>(Utilities.GetReaderField(dr,"cidate"));
			o.brsinvdate = Utilities.FromDbValue<DateTime>(Utilities.GetReaderField(dr,"brsinvdate"));
			o.userid1 = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"userid1"));
			o.locid = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"locid"));
			o.exch_rate = Utilities.FromDbValue<double>(Utilities.GetReaderField(dr,"exch_rate"));
			o.status = string.Empty + Utilities.GetReaderField(dr,"status");
			o.delivery_address1 = string.Empty + Utilities.GetReaderField(dr,"delivery_address1");
			o.delivery_address2 = string.Empty + Utilities.GetReaderField(dr,"delivery_address2");
			o.delivery_address3 = string.Empty + Utilities.GetReaderField(dr,"delivery_address3");
			o.delivery_address4 = string.Empty + Utilities.GetReaderField(dr,"delivery_address4");
			o.delivery_address5 = string.Empty + Utilities.GetReaderField(dr,"delivery_address5");
            o.invoice_address1 = string.Empty + Utilities.GetReaderField(dr, "invoice_address1");
            o.invoice_address2 = string.Empty + Utilities.GetReaderField(dr, "invoice_address2");
            o.invoice_address3 = string.Empty + Utilities.GetReaderField(dr, "invoice_address3");
            o.invoice_address4 = string.Empty + Utilities.GetReaderField(dr, "invoice_address4");
            o.invoice_address5 = string.Empty + Utilities.GetReaderField(dr, "invoice_address5");
			o.currency = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"currency"));
			o.notes = string.Empty + Utilities.GetReaderField(dr,"notes");
			o.inv_amount = Utilities.FromDbValue<double>(Utilities.GetReaderField(dr,"inv_amount"));
			o.inv_amount2 = Utilities.FromDbValue<double>(Utilities.GetReaderField(dr,"inv_amount2"));
			o.inv_amount3 = Utilities.FromDbValue<double>(Utilities.GetReaderField(dr,"inv_amount3"));
			o.inv_payment = Utilities.FromDbValue<double>(Utilities.GetReaderField(dr,"inv_payment"));
			o.int_payment_tmp = Utilities.FromDbValue<double>(Utilities.GetReaderField(dr,"int_payment_tmp"));
			o.duedate1 = Utilities.FromDbValue<DateTime>(Utilities.GetReaderField(dr,"duedate1"));
			o.duedate2 = Utilities.FromDbValue<DateTime>(Utilities.GetReaderField(dr,"duedate2"));
			o.sea_freight = Utilities.FromDbValue<double>(Utilities.GetReaderField(dr,"sea_freight"));
			o.duty = Utilities.FromDbValue<double>(Utilities.GetReaderField(dr,"duty"));
			o.local_charge = Utilities.FromDbValue<double>(Utilities.GetReaderField(dr,"local_charge"));
			o.invoice_number = string.Empty + Utilities.GetReaderField(dr,"invoice_number");
			o.invoice_type_id = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"invoice_type_id"));
			o.invoice_from = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"invoice_from"));
			o.reference_number = string.Empty + Utilities.GetReaderField(dr,"reference_number");
			o.eta = Utilities.FromDbValue<DateTime>(Utilities.GetReaderField(dr,"eta"));
			o.trading_term = string.Empty + Utilities.GetReaderField(dr,"trading_term");
			o.payment_details_id = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"payment_details_id"));
		    o.invoice_no = string.Empty + dr["invoice_no"];
		    o.eb_invoice = Utilities.FromDbValue<int>(dr["eb_invoice"]);
		    o.confirmed = Convert.ToBoolean(dr["confirmed"]);
		    o.cprod_user = Utilities.FromDbValue<int>(dr["cprod_user"]);
		    o.vat_applicable = Utilities.BoolFromLong(dr["vat_applicable"]);
		    if (Utilities.ColumnExists(dr, "amount"))
		        o.Amount = Utilities.FromDbValue<double>(dr["amount"]);
            if (Utilities.ColumnExists(dr, "amountCN"))
                o.AmountCN = Utilities.FromDbValue<double>(dr["amountCN"]);
			
			return o;

		}
		
		
		public static void Create(Invoices o)
        {
            string insertsql = @"INSERT INTO invoices (invoice,orderid,invdate,cidate,brsinvdate,userid1,locid,exch_rate,status,delivery_address1,delivery_address2,delivery_address3,
                        delivery_address4,delivery_address5,invoice_address1,invoice_address2,invoice_address3,invoice_address4,invoice_address5,currency,notes,inv_amount,inv_amount2,
                    inv_amount3,inv_payment,int_payment_tmp,duedate1,duedate2,sea_freight,duty,local_charge,invoice_number,invoice_type_id,invoice_from,reference_number,eta,trading_term,
                    payment_details_id,invoice_no,eb_invoice,confirmed,cprod_user,vat_applicable) 
                    VALUES(@invoice,@orderid,@invdate,@cidate,@brsinvdate,@userid1,@locid,@exch_rate,@status,@delivery_address1,@delivery_address2,@delivery_address3,@delivery_address4,
                            @delivery_address5,@invoice_address1,@invoice_address2,@invoice_address3,@invoice_address4,@invoice_address5,@currency,@notes,@inv_amount,@inv_amount2,@inv_amount3,
                    @inv_payment,@int_payment_tmp,@duedate1,@duedate2,@sea_freight,@duty,@local_charge,@invoice_number,@invoice_type_id,@invoice_from,@reference_number,@eta,@trading_term,
                    @payment_details_id,@invoice_no,@eb_invoice,@confirmed,@cprod_user,@vat_applicable)";

		    var conn = new MySqlConnection(Properties.Settings.Default.ConnString);
            conn.Open();
		    var tr = conn.BeginTransaction();
		    try
		    {
		        MySqlCommand cmd = new MySqlCommand("SELECT MAX(invoice)+1 FROM invoices", conn, tr);
		        o.invoice = Convert.ToInt32(cmd.ExecuteScalar());
		        cmd.CommandText = insertsql;

		        cmd.CommandText =
		            "SELECT MAX(eb_invoice)+1 FROM (SELECT eb_invoice FROM order_header UNION SELECT eb_invoice FROM invoices) AS header_invoices";
		        o.eb_invoice = Convert.ToInt32(cmd.ExecuteScalar());

		        cmd.CommandText = insertsql;

		        BuildSqlParameters(cmd, o);
		        cmd.ExecuteNonQuery();

                if (o.Lines != null)
                {
                    foreach (var line in o.Lines)
                    {
                        line.invoice_id = o.invoice;
                        Invoice_linesDAL.Create(line, tr);
                    }
                }
                if (o.CreditnoteLines != null)
                {
                    foreach (var cnline in o.CreditnoteLines)
                    {
                        cnline.invoice_id = o.invoice;
                        Creditnote_lineDAL.Create(cnline, tr);
                    }
                }
		        tr.Commit();
		    }
		    catch
		    {
		        tr.Rollback();
		        throw;
		    }
		    finally
		    {
                conn.Close();
		        conn = null;
		    }
		    
        }
		
		private static void BuildSqlParameters(MySqlCommand cmd, Invoices o, bool forInsert = true)
        {
			
			cmd.Parameters.AddWithValue("@invoice", o.invoice);
			cmd.Parameters.AddWithValue("@orderid", o.orderid);
			cmd.Parameters.AddWithValue("@invdate", o.invdate);
			cmd.Parameters.AddWithValue("@cidate", o.cidate);
			cmd.Parameters.AddWithValue("@brsinvdate", o.brsinvdate);
			cmd.Parameters.AddWithValue("@userid1", o.userid1);
			cmd.Parameters.AddWithValue("@locid", o.locid);
			cmd.Parameters.AddWithValue("@exch_rate", o.exch_rate);
			cmd.Parameters.AddWithValue("@status", o.status);
			cmd.Parameters.AddWithValue("@delivery_address1", o.delivery_address1);
			cmd.Parameters.AddWithValue("@delivery_address2", o.delivery_address2);
			cmd.Parameters.AddWithValue("@delivery_address3", o.delivery_address3);
			cmd.Parameters.AddWithValue("@delivery_address4", o.delivery_address4);
			cmd.Parameters.AddWithValue("@delivery_address5", o.delivery_address5);
            cmd.Parameters.AddWithValue("@invoice_address1", o.invoice_address1);
            cmd.Parameters.AddWithValue("@invoice_address2", o.invoice_address2);
            cmd.Parameters.AddWithValue("@invoice_address3", o.invoice_address3);
            cmd.Parameters.AddWithValue("@invoice_address4", o.invoice_address4);
            cmd.Parameters.AddWithValue("@invoice_address5", o.invoice_address5);
			cmd.Parameters.AddWithValue("@currency", o.currency);
			cmd.Parameters.AddWithValue("@notes", o.notes);
			cmd.Parameters.AddWithValue("@inv_amount", o.inv_amount);
			cmd.Parameters.AddWithValue("@inv_amount2", o.inv_amount2);
			cmd.Parameters.AddWithValue("@inv_amount3", o.inv_amount3);
			cmd.Parameters.AddWithValue("@inv_payment", o.inv_payment);
			cmd.Parameters.AddWithValue("@int_payment_tmp", o.int_payment_tmp);
			cmd.Parameters.AddWithValue("@duedate1", o.duedate1);
			cmd.Parameters.AddWithValue("@duedate2", o.duedate2);
			cmd.Parameters.AddWithValue("@sea_freight", o.sea_freight);
			cmd.Parameters.AddWithValue("@duty", o.duty);
			cmd.Parameters.AddWithValue("@local_charge", o.local_charge);
			cmd.Parameters.AddWithValue("@invoice_number", o.invoice_number);
			cmd.Parameters.AddWithValue("@invoice_type_id", o.invoice_type_id);
			cmd.Parameters.AddWithValue("@invoice_from", o.invoice_from);
			cmd.Parameters.AddWithValue("@reference_number", o.reference_number);
			cmd.Parameters.AddWithValue("@eta", o.eta);
			cmd.Parameters.AddWithValue("@trading_term", o.trading_term);
			cmd.Parameters.AddWithValue("@payment_details_id", o.payment_details_id);
		    cmd.Parameters.AddWithValue("@invoice_no", o.invoice_no);
		    cmd.Parameters.AddWithValue("@eb_invoice", o.eb_invoice);
		    cmd.Parameters.AddWithValue("@confirmed", o.confirmed);
		    cmd.Parameters.AddWithValue("@cprod_user", o.cprod_user);
		    cmd.Parameters.AddWithValue("@vat_applicable", o.vat_applicable);

        }
		
		public static void Update(Invoices o, List<Invoice_lines> deletedLines )
		{
            string updatesql = @"UPDATE invoices SET invoice_type_id = @invoice_type_id,orderid = @orderid,invdate = @invdate,cidate = @cidate,brsinvdate = @brsinvdate,userid1 = @userid1,
                        locid = @locid,exch_rate = @exch_rate,status = @status,delivery_address1 = @delivery_address1,delivery_address2 = @delivery_address2,delivery_address3 = @delivery_address3,
                                delivery_address4 = @delivery_address4,delivery_address5 = @delivery_address5,invoice_address1 = @invoice_address1,invoice_address2 = @invoice_address2,invoice_address3 = @invoice_address3,invoice_address4 = @invoice_address4,invoice_address5 = @invoice_address5,currency = @currency,notes = @notes,inv_amount = @inv_amount,inv_amount2 = @inv_amount2,inv_amount3 = @inv_amount3,inv_payment = @inv_payment,int_payment_tmp = @int_payment_tmp,duedate1 = @duedate1,duedate2 = @duedate2,sea_freight = @sea_freight,duty = @duty,local_charge = @local_charge,invoice_number = @invoice_number,invoice_type_id = @invoice_type_id,invoice_from = @invoice_from,reference_number = @reference_number,eta = @eta,trading_term = @trading_term,payment_details_id = @payment_details_id,invoice_no = @invoice_no,eb_invoice = @eb_invoice,
                                confirmed = @confirmed,cprod_user = @cprod_user,vat_applicable = @vat_applicable  WHERE invoice = @invoice";

		    var conn = new MySqlConnection(Properties.Settings.Default.ConnString);
            conn.Open();
            var tr = conn.BeginTransaction();
		    try
		    {
		        var cmd = new MySqlCommand(updatesql, conn);
		        BuildSqlParameters(cmd, o, false);
		        cmd.ExecuteNonQuery();

                if (o.Lines != null)
                {
                    foreach (var line in o.Lines)
                    {
                        if (line.linenum <= 0)
                            Invoice_linesDAL.Create(line, tr);
                        else
                        {
                            Invoice_linesDAL.Update(line, tr);
                        }
                    }
                }
                foreach (var d in deletedLines)
                {
                    Invoice_linesDAL.Delete(d.linenum, tr);
                }

                //Update lines
                tr.Commit();
		    }
		    catch
		    {
		        tr.Rollback();
		        throw;
		    }
		    finally
		    {
                conn.Close();
		    }
		}
		
		public static void Delete(int invoice)
		{
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				var cmd = new MySqlCommand("DELETE FROM invoices WHERE invoice = @id" , conn);
                cmd.Parameters.AddWithValue("@id", invoice);
                cmd.ExecuteNonQuery();
            }
		}


        public static List<Invoices> GetByCriteria(List<int> custids, DateTime? from, DateTime? to)
        {
            var result = new List<Invoices>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd =
                    new MySqlCommand("", conn);
                  cmd.CommandText = string.Format(@"SELECT invoices.*, users.*, (SELECT SUM(CASE qty_type WHEN 1 THEN orderqty*unitprice ELSE orderqty/100*unitprice END) FROM invoice_lines WHERE invoice_id = invoices.invoice) AS Amount,
                                                  (SELECT SUM(unitprice*quantity) FROM creditnote_line WHERE invoice_id = invoices.invoice) AS AmountCN FROM invoices INNER JOIN users ON invoices.userid1 = users.user_id 
                                            WHERE (invdate >= @from OR @from IS NULL) AND (invdate <= @to OR @TO IS NULL) 
                                            {0} AND invoices.orderid IS NULL ", custids != null ? string.Format("AND invoices.userid1 IN ({0})", Utilities.CreateParametersFromIdList(cmd,custids)) : "");
                        
                cmd.Parameters.AddWithValue("@from", from != null ? (object) from : DBNull.Value);
                cmd.Parameters.AddWithValue("@to", to != null ? (object) to : DBNull.Value);
                
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    var i = GetFromDataReader(dr);
                    i.Client = CompanyDAL.GetFromDataReader(dr);
                    result.Add(i);
                }
                dr.Close();
            }
            return result;
        }

        public static Invoices GetCreditNoteByCriteria(int client_id,int? brand_user, DateTime invoiceDate)
        {
            Invoices result = null;
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd =
                    new MySqlCommand(
                        @"SELECT invoices.*,users.* FROM invoices INNER JOIN users ON invoices.userid1 = users.user_id 
                                            WHERE invoices.userid1 = @client_id AND invoice_type_id = 4 AND (invoices.cprod_user = @brand_user OR @brand_user IS NULL) AND invoices.invdate = @date",
                        conn);
                cmd.Parameters.AddWithValue("@client_id", client_id);
                cmd.Parameters.AddWithValue("@brand_user", brand_user);
                cmd.Parameters.AddWithValue("@date", invoiceDate);
                var dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    result = GetFromDataReader(dr);
                    result.Client = CompanyDAL.GetFromDataReader(dr);
                    result.CreditnoteLines = Creditnote_lineDAL.GetForInvoice(result.invoice);
                }
            }
            return result;
        }

        public static void CreateReturnCreditNotes(DateTime? from, DateTime? to)
        {
            var returns = ReturnsDAL.GetInPeriod(from, to);
            
            foreach (var returnsByClient in returns.GroupBy(r=>r.client_id))
            {
                if (returnsByClient.Key != null)
                {
                    var client = CompanyDAL.GetById(returnsByClient.Key.Value);
                    if (client.distributor > 0)
                    {
                        foreach (var returnByBrand in returnsByClient.GroupBy(r=>r.Product.cprod_user))
                        {
                            if (returnByBrand.Key != null)
                            {
                                var brand_company = CompanyDAL.GetById(returnByBrand.Key.Value);
                                var inv = new Invoices
                                    {
                                        invdate = DateTime.Today,
                                        userid1 = client.user_id,
                                        invoice_address1 = client.user_address1,
                                        invoice_address2 = client.user_address2,
                                        invoice_address3 = client.user_address3,
                                        invoice_address4 = client.user_address4,
                                        invoice_address5 = client.user_address5,
                                        invoice_type_id = 4,
                                        currency =  1,
                                        cprod_user = brand_company.user_id,
                                        CreditnoteLines = new List<Creditnote_line>()
                                    };
                                foreach (var r in returnByBrand.OrderBy(re=>re.returnsid))
                                {
                                    inv.CreditnoteLines.Add(new Creditnote_line{client_ref = r.reference,cprod_code = r.Product.cprod_code1,
                                            cprod_name = r.claim_type == 2 ? "installation/other" : r.Product.cprod_name,quantity = r.claim_type == 2 ? 1 : r.return_qty,
                                            unitprice = r.claim_type == 2 ? r.claim_value : r.credit_value_override > 0 ? r.credit_value_override : r.credit_value,return_no = r.return_no,overridden = r.ebuk == 1});
                                }
                                Create(inv);

                            }
                        }
                    }
                    else
                    {
                        var inv = new Invoices
                        {
                            invdate = DateTime.Today,
                            userid1 = client.user_id,
                            invoice_address1 = client.user_address1,
                            invoice_address2 = client.user_address2,
                            invoice_address3 = client.user_address3,
                            invoice_address4 = client.user_address4,
                            invoice_address5 = client.user_address5,
                            invoice_type_id = 4,
                            currency = 1,
                            CreditnoteLines = new List<Creditnote_line>()
                        };
                        foreach (var r in returnsByClient.OrderBy(re=>re.returnsid) )
                        {
                            inv.CreditnoteLines.Add(new Creditnote_line { client_ref = r.reference, cprod_code = r.Product.cprod_code1,
                                    cprod_name = r.claim_type == 2 ? "installation/other" : r.Product.cprod_name,
                                    quantity = r.claim_type == 2 ? 1 : r.return_qty, 
                                    unitprice = r.claim_type == 2 ? r.claim_value : r.credit_value_override > 0 ? r.credit_value_override : r.credit_value, return_no = r.return_no,overridden = r.ebuk == 1});

                        }
                        Create(inv);
                    }
                }
            }
        }
	}
}
			
			
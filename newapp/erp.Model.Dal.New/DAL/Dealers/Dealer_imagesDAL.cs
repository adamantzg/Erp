
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using Dapper;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace erp.Model.Dal.New
{
    public class DealerImagesDal : GenericDal<Dealer_images>, IDealerImagesDal
	{
		public DealerImagesDal(IDbConnection conn) : base(conn)
		{
		}

		public List<Dealer_images> GetByDealer(int dealer_id)
		{
			var data = conn.Query<Dealer_images>("SELECT dealer_images.* FROM dealer_images WHERE dealer_id = @dealer_id",
				new {dealer_id}).ToList();
			
			foreach (var image in data)
			{
				image.Brands = conn.Query<Brand>(
					@"SELECT brands.* FROM brands INNER JOIN dealer_image_brand ON brands.brand_id = dealer_image_brand.brand_id 
					WHERE dealer_image_brand.dealer_image_id = @image_id", new {image_id = image.image_unique}
				).ToList();
			}

			return data;
		}
		
		public override Dealer_images GetById(object id)
		{
			var di = conn.QueryFirstOrDefault<Dealer_images>("SELECT dealer_images.* FROM dealer_images WHERE image_unique = @id",
				new {id});
			
			if(di != null)
			{
				di.Brands = conn.Query<Brand>(
					@"SELECT brands.* FROM brands INNER JOIN dealer_image_brand ON brands.brand_id = dealer_image_brand.brand_id 
					WHERE dealer_image_brand.dealer_image_id = @image_id", new {image_id = di.image_unique}
				).ToList();
			}

			return di;
		}

        public List<Dealer_images> GetUnallocatedImages()
        {
	        return conn.Query<Dealer, Dealer_images, Dealer_images>(
		        @"SELECT dealers.*, dealer_images.* FROM dealer_images INNER JOIN dealers ON dealer_images.dealer_id = dealers.user_id 
				WHERE COALESCE(store_page,0) = 0 AND NOT EXISTS (SELECT * FROM dealer_image_brand WHERE dealer_image_id = dealer_images.image_unique)",
		        (d, di) =>
		        {
			        di.Dealer = d;
			        return di;
		        }, splitOn: "image_unique").ToList();
			
        }

		public List<Dealer_images> GetForSlaveHost(int host_id)
		{
			return conn.Query<Dealer_images>(
				@"SELECT dealer_images.* FROM dealer_images INNER JOIN dealer_images_file_transfer 
					ON dealer_images.image_unique = dealer_images_file_transfer.image_id
					  WHERE dealer_images_file_transfer.host_id = @host_id",  new {host_id}).ToList();
		}

		public void DeleteTransferData(int host_id, int image_id)
		{
			conn.Execute("DELETE FROM dealer_images_file_transfer WHERE host_id = @host_id AND image_id = @image_id", new { host_id, image_id});
		}

		public override void Create(Dealer_images o, IDbTransaction tr)
		{
			var shouldRollback = tr == null;
			if (tr == null)
				tr = conn.BeginTransaction();
			try
			{
				conn.Execute(GetCreateSql(), o, tr);

				if (o.Brands != null)
				{
					foreach (var brand in o.Brands)
					{
						conn.Execute("INSERT INTO dealer_image_brand(dealer_image_id, brand_id) VALUES(@dealer_image_id, @brand_id)",
							brand, tr);
					}
				}
				tr.Commit();
			}
			catch (Exception)
			{
				if(shouldRollback)
					tr.Rollback();
				throw;
			}

        }
		
		
		public override void Update(Dealer_images o, IDbTransaction tr = null)
		{
			var shouldRollback = tr == null;
			if (tr == null)
				tr = conn.BeginTransaction();
			try
			{
				conn.Execute(GetUpdateSql(), o, tr);

				conn.Execute("DELETE FROM dealer_image_brand WHERE dealer_image_id = @dealer_image_id",
					new { dealer_image_id = o.image_unique });

				if (o.Brands != null && o.Brands.Count > 0)
				{
					foreach (var brand in o.Brands)
					{
						conn.Execute(
							"INSERT INTO dealer_image_brand(dealer_image_id, brand_id) VALUES(@dealer_image_id, @brand_id)", new
							{
								brand.brand_id,
								dealer_image_id = o.image_unique
							}, tr);
					}
				}

				conn.Execute(@"DELETE FROM dealer_brandstatus WHERE dealer_id = @dealer_id 
                                AND NOT EXISTS (SELECT * FROM dealer_images INNER JOIN dealer_image_brand ON dealer_images.image_unique = dealer_image_brand.dealer_image_id
                                WHERE dealer_id = @dealer_id AND brand_id = dealer_brandstatus.brand_id)",
					new { o.dealer_id }, tr);
				tr.Commit();
			}
			catch (Exception)
			{
				if(shouldRollback)
					tr.Rollback();
				throw;
			}
            
            
		}
		
		public override void Delete(int image_unique, IDbTransaction tr)
		{
			if (tr == null)
				tr = conn.BeginTransaction();
            
			var dealer_id =
				conn.ExecuteScalar<int>("SELECT dealer_id FROM dealer_images WHERE image_unique = @id", new {image_unique});

			try
			{
				conn.Execute(GetDeleteSql(), new { id = image_unique }, tr);

				conn.Execute(@"DELETE FROM dealer_brandstatus WHERE dealer_id = @dealer_id 
                                AND NOT EXISTS (SELECT * FROM dealer_images INNER JOIN dealer_image_brand ON dealer_images.image_unique = dealer_image_brand.dealer_image_id
                                WHERE dealer_id = @dealer_id AND brand_id = dealer_brandstatus.brand_id)",
					new { dealer_id }, tr);
				tr.Commit();
			}
			catch (Exception)
			{
				tr.Rollback();
			}

           
		}

		protected override string GetAllSql()
		{
			return "SELECT * FROM dealer_images";
		}

		protected override string GetByIdSql()
		{
			throw new NotImplementedException();
		}

		protected override string GetCreateSql()
		{
			return @"INSERT INTO dealer_images (dealer_id,dealer_image,seq,hide,reviewed,store_page,dateModified,dateCreated) 
				VALUES(@dealer_id,@dealer_image,@seq,@hide,@reviewed,@store_page,@dateModified,@dateCreated)";
		}

		protected override string GetUpdateSql()
		{
			return
				@"UPDATE dealer_images SET dealer_id = @dealer_id,dealer_image = @dealer_image,seq = @seq,hide = @hide, reviewed = @reviewed,store_page = @store_page, 
                                dateModified = @dateModified, dateCreated = @dateCreated  WHERE image_unique = @image_unique";
		}

		protected override string GetDeleteSql()
		{
			return "DELETE FROM dealer_images WHERE image_unique = @id";
		}

		protected override string IdField => "image_unique";
	}
}
			
			
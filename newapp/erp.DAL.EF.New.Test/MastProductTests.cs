using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using asaq2.Model;
using Dapper;

namespace asaq2.DAL.EF.New.Test
{
	[TestClass]
	[Table("mast_products")]
	public class MastProductTests : asaq2.Model.Dal.New.Test.MastProductTests
	{

		UnitOfWork unitOfWork = new UnitOfWork(new Model("name=connString"));
		

		public MastProductTests()
		{

		}

		public MastProductTests(IDbConnection conn) : base(conn)
		{

		}

		[TestMethod]
		public void FilesTests()
		{
			var data = new List<Mast_products>
			{
				new Mast_products
				{
					factory_ref = "ref"
				}
			};
			GenerateTestData(data);
			var files = new List<File>
			{
				new File { name = "file1"},
				new File { name = "file2"}
			};
			fileTests.GenerateTestData(files);

			var mp = data[0];
			mp.OtherFiles = new List<File>();
			mp.OtherFiles.Add(files[0]);

			unitOfWork.MastProductRepository.Update(mp);
			unitOfWork.Save();

			var dbData = conn.Query("SELECT * FROM mast_product_file WHERE mast_id = @mast_id", new { mast_id = mp.mast_id}).ToList();

			Assert.IsNotNull(dbData);
			Assert.AreEqual(1, dbData.Count);

		}

		[TestCleanup]
		public override void Cleanup()
		{
			base.Cleanup();			
		}
	}
}

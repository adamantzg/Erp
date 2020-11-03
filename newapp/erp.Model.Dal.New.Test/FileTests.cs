using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace asaq2.Model.Dal.New.Test
{
	[TestClass]
	[Table("file")]
	public class FileTests : DatabaseTestBase
	{
		public FileTests()
		{

		}

		public FileTests(IDbConnection conn) : base(conn)
		{

		}

		protected override string GetCreateSql()
		{
			return @"INSERT INTO file(name, type_id) VALUES(@name, @type_id)";
		}
	}
}

﻿using System.Data.Common;
using System.Data.SqlClient;
using SqlFu;
using SqlFu.Executors;
using SqlFu.Providers.SqlServer;
using Tests.Providers;

namespace Tests.Usage
{
    public class SqlServerSProcTests : AStoredProcsTests
    {
        public SqlServerSProcTests()
        {
        }

        protected override DbConnection GetConnection()
        {
            return new SqlFuConnection(new SqlServer2012Provider(SqlClientFactory.Instance.CreateConnection),Setup.SqlServerConnection,new SqlFuConfig());
        }

        protected override void CreateSproc()
        {
            _db.AddDbObjectOrIgnore(@"CREATE PROCEDURE spTest
	@id int,
	@pout varchar(50) out
AS
BEGIN
		SET NOCOUNT ON;
   set @pout='bla';
	SELECT 1 as id,'bla' as name, @id as [input];
	return 100;
END");

        }
    }
}
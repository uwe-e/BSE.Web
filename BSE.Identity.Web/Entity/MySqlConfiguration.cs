using System.Data.Entity;
using BSE.Identity.Web.Entity.Migrations.History;

namespace BSE.Identity.Web.Entity
{
	public class MySqlConfiguration : DbConfiguration
	{
		public MySqlConfiguration()
		{
			SetHistoryContext("MySql.Data.MySqlClient", (conn, schema) => new MySqlHistoryContext(conn, schema));
		}
	}
}
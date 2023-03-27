using System.Data.SqlClient;

namespace Services
{
    internal class SqlService
    {
        private string dataBaseName;

        public SqlService(string dataBaseName)
        {
            this.dataBaseName = dataBaseName;
        }
    }
}

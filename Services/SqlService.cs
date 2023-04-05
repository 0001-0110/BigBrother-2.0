using Microsoft.EntityFrameworkCore;

namespace Services
{
    internal partial class DatabaseContext : DbContext
    {
        public static readonly string CONNECTIONSTRING = File.ReadAllText(BigBrother.Instance.GetPath("ConnectionString.txt"));

        public DatabaseContext() : base(new DbContextOptionsBuilder<DatabaseContext>().UseSqlServer(CONNECTIONSTRING).Options) { }

        public DatabaseContext(DbContextOptions options) : base(options) { }

        public virtual DbSet<Test> Tests { get; set; }
    }

    [Obsolete]
    internal class SqlService
    {
        private string dataBaseName;

        public SqlService(string dataBaseName)
        {
            this.dataBaseName = dataBaseName;
        }
    }
}

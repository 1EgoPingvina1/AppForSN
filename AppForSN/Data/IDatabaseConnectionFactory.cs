using System.Data;

namespace AppForSN.Data
{
    public interface IDatabaseConnectionFactory
    {
        IDbConnection CreateConnection();
    }
}

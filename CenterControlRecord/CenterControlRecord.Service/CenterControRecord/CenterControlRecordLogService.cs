using CenterControlRecord.Infrastructrue.Configuration;
using SqlServerDataAdapter;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenterControlRecord.Service.CenterControRecord
{
    public class CenterControlRecordLogService
    {
        private static string _connString = ConnectionStringFactory.NXJCConnectionString;
        private static ISqlServerDataFactory _dataFactory = new SqlServerDataFactory(_connString);
        public static DataTable GetCenterControlRecordLogData(string organizationId)
        {
            string mySql = @"SELECT ProductionPrcessName,RecordName,CreateDate
                             FROM shift_CenterControlRecord
                             where OrganizationID=@organizationId
                             order by DisplayIndex,RecordType,CreateDate";
            SqlParameter sqlParameter = new SqlParameter("@organizationId", organizationId);
            DataTable RecordLogTable = _dataFactory.Query(mySql, sqlParameter);
            return RecordLogTable;
        }
    }
}

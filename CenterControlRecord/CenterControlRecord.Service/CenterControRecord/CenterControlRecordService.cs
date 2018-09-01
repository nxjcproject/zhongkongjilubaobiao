using CenterControlRecord.Infrastructrue.Configuration;
using SqlServerDataAdapter;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenterControlRecord.Service.CenterControl
{
    public class CenterControlRecordService
    {
        private static string _connString = ConnectionStringFactory.NXJCConnectionString;
        private static ISqlServerDataFactory _dataFactory = new SqlServerDataFactory(_connString);
        public static DataTable GetProcessTypeInfo(string organizationId)
        {
            string mySql = @"SELECT ProductionPrcessId as id
                                  ,ProductionPrcessName as text
                             from [dbo].[shift_CenterControlRecord] 
                             where OrganizationID=@organizationId
                             group by ProductionPrcessId,ProductionPrcessName,DisplayIndex
                             order by DisplayIndex";
            SqlParameter sqlParameter = new SqlParameter("@organizationId", organizationId);
            DataTable table = _dataFactory.Query(mySql, sqlParameter);
            return table;
        }

        public static DataTable GetReportNameInfo(string organizationId, string productionprocessId)
        {
            string mySql = @"SELECT RecordType as id,
                                    RecordName as text
                               from [dbo].[shift_CenterControlRecord] 
                              where ProductionPrcessId=@ProductionPrcessId
                                and OrganizationId=@organizationId
                              group by RecordType,RecordName
                              order by RecordType";
            SqlParameter[] myParameter = { new SqlParameter("@organizationId", organizationId), 
                                           new SqlParameter("@ProductionPrcessId", productionprocessId) };
            DataTable table = _dataFactory.Query(mySql, myParameter);
            return table;
        }

        public static int GetSumCountNum(string keyId)
        {
            string mySql = @"select DisplayIndex from [dbo].[shift_CenterControlRecordItems] 
                              where KeyId=@KeyId
                                and Enabled=1
                                and DataType=1  
                              order by DisplayIndex desc";
            SqlParameter myParameter = new SqlParameter("@KeyId", keyId);
            DataTable table = _dataFactory.Query(mySql, myParameter);
            int CountNum = Convert.ToInt16(table.Rows[0][0]);
            return CountNum;
        }

        /// <summary>
        /// 获取中控记录表中需要查询的表的数目
        /// </summary>
        /// <param name="keyId"></param>
        /// <returns></returns>
        public static DataTable GetTableNum(string keyId)
        {
            string mySql = @"select DCSTableName,row_number() over (order by DCSTableName) as id
                                from [dbo].[shift_CenterControlRecordItems] 
                                where KeyId=@KeyId and Enabled=1 and DataType=1 and DCSTableName is not null
                                group by DCSTableName";
            SqlParameter myParameter = new SqlParameter("@KeyId", keyId);
            DataTable table = _dataFactory.Query(mySql, myParameter);
            return table;
        }

        public static DataTable GetCountList(string keyId)
        {
            string mySql = @"select ContrastID,DisplayIndex from [dbo].[shift_CenterControlRecordItems] 
                                where KeyId=@KeyId
                                and Enabled=1
                                and DataType=1
	                            order by DisplayIndex asc";
            SqlParameter myParameter = new SqlParameter("@KeyId", keyId);
            DataTable table = _dataFactory.Query(mySql, myParameter);
            return table;
        }

        public static DataTable GetCountList(string keyId, string DCStableName)
        {
            string mySql = @"select ContrastID,DisplayIndex,DCSTableName from [dbo].[shift_CenterControlRecordItems] 
                                where KeyId=@KeyId
                                and Enabled=1
                                and DataType=1
                                and DCSTableName=@DCSTableName
	                            order by DisplayIndex asc";
            SqlParameter[] myParameter = { new SqlParameter("@KeyId", keyId),
                                         new SqlParameter("@DCSTableName", DCStableName)};
            DataTable table = _dataFactory.Query(mySql, myParameter);
            return table;
        }

        public static DataTable GetAllTableData(string OrganizationId, string ProductionPrcessId, string mRecordType, string time, string countType, out int mSumCount, out string mTemplateUrl)
        {
            //筛选出符合条件的引领表信息
            string mSql = @"SELECT TOP 1 
                                   [KeyID]
                                  ,[ProductionPrcessId]
                                  ,[ProductionPrcessName]
                                  ,[RecordType]
                                  ,[RecordName]
                                  ,[CreateDate]
                                  ,[DatabaseID]
                                  ,[TemplateUrl]
                              FROM [NXJC].[dbo].[shift_CenterControlRecord]
                             WHERE OrganizationID = @mOrganizationId
                               AND ProductionPrcessId = @mProductionPrcessId
                               AND RecordType = @mRecordType
                               AND CreateDate <= @mCreateDate
                             ORDER BY CreateDate DESC";
            SqlParameter[] parameters = { new SqlParameter("@mOrganizationId",OrganizationId),
                                          new SqlParameter("@mProductionPrcessId",ProductionPrcessId),
                                          new SqlParameter("@mCreateDate",time),
                                          new SqlParameter("@mRecordType",mRecordType) };
            DataTable mtable = _dataFactory.Query(mSql, parameters);
            string mKeyID = null;
            string mDatabaseID = null;
            mTemplateUrl = "";                   //传回前台
            mSumCount = 0;
            if (mtable.Rows.Count > 0)
            {
                mKeyID = mtable.Rows[0]["KeyID"].ToString().Trim();
                mDatabaseID = mtable.Rows[0]["DatabaseID"].ToString().Trim();
                mTemplateUrl = mtable.Rows[0]["TemplateUrl"].ToString().Trim();
                mSumCount = GetSumCountNum(mKeyID);  //传回前台
            }

            DataTable m_CenterControlTableStructrue = GetCenterControlTableStructrue();
            string m_Startime = time + " 00:00:00";
            string m_Endtime = time + " 23:59:59";

            if (mKeyID != "" && mKeyID != null)
            {
                //创建空DataTable 然后再往里面填数
                DataTable c_AList = GetCountList(mKeyID);
                int cACount = c_AList.Rows.Count;
                for (int cA = 0; cA < cACount; cA++)
                {
                    string s_Count = "Sum" + c_AList.Rows[cA]["DisplayIndex"].ToString().Trim();
                    m_CenterControlTableStructrue.Columns.Add(s_Count, typeof(float));
                }

                //数据处理主程序
                DataTable t_List = GetTableNum(mKeyID);
                int tCount = t_List.Rows.Count;   //获取表数目
                for (int t = 0; t < tCount; t++)
                {
                    string mTableName = t_List.Rows[t]["DCSTableName"].ToString().Trim();
                    DataTable c_List = GetCountList(mKeyID, mTableName);     //获取该表下的列  拼SQL 语句
                    int cCount = c_List.Rows.Count;
                    string mySql = "";
                    string s_sum = "";
                    if (countType == "avgValue")
                    {
                        for (int c = 0; c < cCount; c++)
                        {
                            s_sum = s_sum + ", AVG([" + c_List.Rows[c]["ContrastID"].ToString().Trim() + "])" + " as Sum" + c_List.Rows[c]["DisplayIndex"].ToString().Trim();
                        }
                        string mSelect = "select convert(VARCHAR(2),vDate,108) as hour";
                        string mFrom = " from " + mDatabaseID + ".dbo.History_" + mTableName;
                        string mWhere = " where vDate>'" + m_Startime + "' and vDate<'" + m_Endtime + "'";
                        string mGroupby = " group by convert(VARCHAR(2),vDate,108)";
                        string mOrderby = " order by convert(VARCHAR(2),vDate,108)";
                        mySql = mSelect + s_sum + mFrom + mWhere + mGroupby + mOrderby;
                    }
                    else if (countType == "oclockValue")
                    {
                        for (int c = 0; c < cCount; c++)
                        {
                            s_sum = s_sum + ", ([" + c_List.Rows[c]["ContrastID"].ToString().Trim() + "])" + " as Sum" + c_List.Rows[c]["DisplayIndex"].ToString().Trim();
                        }
                        string selectA = "select A.* from (";
                        string row = ",(row_number() over (partition by convert(VARCHAR(2),vDate,108) order by convert(VARCHAR(2),vDate,108))) as group_idx";
                        string last = ")as A where group_idx=1";
                        string mSelect = "select convert(VARCHAR(2),vDate,108) as hour";
                        string mFrom = " from " + mDatabaseID + ".dbo.History_" + mTableName;
                        string mWhere = " where vDate>'" + m_Startime + "' and vDate<'" + m_Endtime + "'";
                        mySql = selectA + mSelect + s_sum + row + mFrom + mWhere + last;
                    }
                    else if (countType == "MaxMin")
                    {
                        for (int c = 0; c < cCount; c++)
                        {
                            s_sum = s_sum + ",(max(" + c_List.Rows[c]["ContrastID"].ToString().Trim() + ")-min(" + c_List.Rows[c]["ContrastID"].ToString().Trim() + "))" + " as Sum" + c_List.Rows[c]["DisplayIndex"].ToString().Trim();
                        }
                        string mSelect = "select convert(VARCHAR(2),vDate,108) as hour";
                        string mFrom = " from " + mDatabaseID + ".dbo.History_" + mTableName;
                        string mWhere = " where vDate>'" + m_Startime + "' and vDate<'" + m_Endtime + "'";
                        string mGroupby = " group by convert(VARCHAR(2),vDate,108)";
                        string mOrderby = " order by convert(VARCHAR(2),vDate,108)";
                        mySql = mSelect + s_sum + mFrom + mWhere + mGroupby + mOrderby;
                    }
                    DataTable table = _dataFactory.Query(mySql);
                    int tbCount = table.Rows.Count;

                    //再次获取列数  列扫描填写
                    for (int c = 0; c < cCount; c++)
                    {
                        string cName = "Sum" + c_List.Rows[c]["DisplayIndex"].ToString().Trim();
                        if (tbCount == 24)
                        {
                            for (int tb = 0; tb < 24; tb++)
                            {
                                m_CenterControlTableStructrue.Rows[tb][cName] = table.Rows[tb][cName];
                            }
                        }
                        else
                        {           //如果不等于24行需要进行判断
                            for (int n = 0; n < 10; n++)
                            {
                                string rName = "0" + n.ToString();
                                for (int nOne = 0; nOne < tbCount; nOne++)
                                {
                                    if (table.Rows[nOne]["hour"].ToString() == rName)
                                    {
                                        m_CenterControlTableStructrue.Rows[nOne][cName] = table.Rows[nOne][cName];
                                    }
                                }
                            }

                            for (int m = 10; m < 24; m++)
                            {
                                string rName = m.ToString();
                                for (int mTwo = 0; mTwo < tbCount; mTwo++)
                                {
                                    if (table.Rows[mTwo]["hour"].ToString() == rName)
                                    {
                                        m_CenterControlTableStructrue.Rows[mTwo][cName] = table.Rows[mTwo][cName];
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return m_CenterControlTableStructrue;
        }
        private static DataTable GetCenterControlTableStructrue()
        {
            DataTable m_CenterControlTableStructrue = new DataTable();
            m_CenterControlTableStructrue.Columns.Add("hour", typeof(string));
            string mhour = "";
            for (int n = 0; n < 10; n++)
            {
                mhour = "0" + n.ToString();
                m_CenterControlTableStructrue.Rows.Add(mhour);
            }
            for (int m = 10; m < 24; m++)
            {
                mhour = m.ToString();
                m_CenterControlTableStructrue.Rows.Add(mhour);
            }
            return m_CenterControlTableStructrue;
        }

        public static void ExportExcelFile(string myFileType, string myFileName, string myData)
        {

            if (myFileType == "xls")
            {
                UpDownLoadFiles.DownloadFile.ExportExcelFile(myFileName, myData);
            }
        }
    }
}
//思路   1.获取表字段个数 2 获取表格式 3 查询各个表的数据 4 拼成DataTable 5 return

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
       public static DataTable GetProcessTypeInfo(string organizationId)
       {
           string connectionString = ConnectionStringFactory.NXJCConnectionString;
           ISqlServerDataFactory dataFactory = new SqlServerDataFactory(connectionString);
           string mySql = @"select DisplayIndex as id ,ProductionPrcessName as text, ProductionPrcessId as value
                                       from [dbo].[shift_CenterControlRecord] 
                                       where OrganizationID=@organizationId
                                       group by DisplayIndex,ProductionPrcessName,ProductionPrcessId
                                       order by DisplayIndex";
           SqlParameter sqlParameter = new SqlParameter("@organizationId",organizationId);
           DataTable table = dataFactory.Query(mySql, sqlParameter);
           return table;
        }
       public static DataTable GetReportNameInfo(string organizationId,string productionprocessId) 
       {
           string connectionString = ConnectionStringFactory.NXJCConnectionString;
           ISqlServerDataFactory dataFactory = new SqlServerDataFactory(connectionString);
           string mySql = @"select RecordType as id,RecordName as text,KeyID,DatabaseID 
                                       from [dbo].[shift_CenterControlRecord] 
                                       where ProductionPrcessId=@ProductionPrcessId
                                         and OrganizationId=@organizationId
                                         order by RecordType";
           SqlParameter[] myParameter = { new SqlParameter("@organizationId", organizationId), 
                                                 new SqlParameter("@ProductionPrcessId", productionprocessId) };
           DataTable table = dataFactory.Query(mySql,myParameter);
           return table;
       }
       public static DataTable GetTableFieldInfo(string keyId,string databaseId,string time,int id,string tableName)
       {    
           string connectionString = ConnectionStringFactory.NXJCConnectionString;
           ISqlServerDataFactory dataFactory = new SqlServerDataFactory(connectionString);
           string mySql = @"select A.ContrastID,A.DisplayIndex,B.VariableName,B.TableName,B.FieldName from [dbo].[shift_CenterControlRecordItems] A,
                            {0}.[dbo].[DCSContrast] B
                            where A.KeyId=@KeyId
                            and A.ContrastID=B.VariableName
                            and A.DCSTableName=B.TableName
                            and A.Enabled=1
                            and A.DataType=1
                            group by B.TableName,A.ContrastID,B.VariableName,B.FieldName,A.DisplayIndex
                            order by B.TableName,A.DisplayIndex";
           mySql = string.Format(mySql,databaseId);
           SqlParameter myParameter = new SqlParameter("@KeyId", keyId);
           DataTable table = dataFactory.Query(mySql, myParameter);
           int t_count=table.Rows.Count;
           string m_from = "";
           List<string> m_convert = new List<string>();
           string[] m_sum=new string[t_count];
           int t = 0;
           string m_select = "select ";
           //"convert(VARCHAR(2), t0.vDate, 108) as hour0" 
           m_from = " from " + databaseId + ".[dbo].[History_" + tableName.Trim() + "] t" + (id-1).ToString();
           m_convert.Insert(0, "convert(VARCHAR(2), t0.vDate, 108) as hour");
            for (int m = 0; m < (table.Rows.Count - 1); m++) 
            {
                //Convert.ToInt16(table.Rows[0]["DisplayIndex"]) - 1             
                m_sum[Convert.ToInt16(table.Rows[0]["DisplayIndex"]) - 1] = "sum(t0." + table.Rows[0]["VariableName"].ToString().Trim() + ")";
                if (table.Rows[m]["TableName"].ToString() == table.Rows[m + 1]["TableName"].ToString())
                {
                    m_sum[Convert.ToInt16(table.Rows[m + 1]["DisplayIndex"]) - 1] = "sum(t" + t + "." + table.Rows[m + 1]["VariableName"].ToString().Trim() + ")";
                }
                else
                {
                    t = t + 1;
                    m_convert.Add("convert(VARCHAR(2),t" + t + ".vDate,108) as hour" + t); 
                    //m_select = ",convert(VARCHAR(2),t" + t + ".vDate,108) as hour" + t;
                    m_sum[Convert.ToInt16(table.Rows[m + 1]["DisplayIndex"]) - 1] = "sum(t" + t + "." + table.Rows[m + 1]["VariableName"].ToString().Trim() + ")";
                }
            }
            string m_where = " where ";
            string m_groupby = " group by ";
            string m_orderby = " order by ";
            string m_Startime = time + " 00:00:00";
            string m_Endtime = time + " 23:59:59";
            //for (int n = 0; n < m_convert.Count(); n++)
            //{
            //    m_select = m_select + m_convert[n] + ',';
            m_where = m_where + "t" + (id - 1).ToString() + ".vDate>'" + m_Startime + "'and " + "t" + (id - 1).ToString() + ".vDate<'" + m_Endtime+"'";
            //    if (n > 0)
            //    {
            //        strConvert = strConvert + m_convert[n - 1].Substring(0, m_convert[n - 1].Length - 8) + "=" + m_convert[n].Substring(0, m_convert[n].Length - 8) + " and ";
            //    }
            //    else
            //    {
            m_orderby = m_orderby + m_convert[id-1].Substring(0, m_convert[id-1].Length - 8);
            //    }
            m_groupby = m_groupby + m_convert[id-1].Substring(0, m_convert[id-1].Length - 8);
            //}
            m_select = m_select + m_convert[id-1] + ',';
      
            for (int n = 0; n < m_sum.Count();n++ )
            {
                if (m_sum[n].Contains("sum(t"+(id-1).ToString()))
                {
                    m_select = m_select + m_sum[n] + "as Sum" + (n+1) + ",";
                }             
            }
            m_select = m_select.Substring(0, m_select.Length - 1);
             //m_from = m_from;
             //t = t;
            string mysql = m_select + m_from + m_where + m_groupby + m_orderby;
            DataTable RecordTable= GetRecordDataTable(mysql);
            return RecordTable;
       }

       public static DataTable GetRecordDataTable(string mySql)
       {
           string m_mySql = @mySql;
           string connectionString = ConnectionStringFactory.NXJCConnectionString;
           ISqlServerDataFactory dataFactory = new SqlServerDataFactory(connectionString);
           DataTable table = dataFactory.Query(m_mySql);
           return table;      
       }
       public static DataTable GetHtmlTempleteTable(string keyId) 
       {
           string connectionString = ConnectionStringFactory.NXJCConnectionString;
           ISqlServerDataFactory dataFactory = new SqlServerDataFactory(connectionString);
           string mySql = @"select TemplateUrl from [dbo].[shift_CenterControlRecord]
                            where keyID=@KeyId";
           SqlParameter myParameter = new SqlParameter("@KeyId", keyId);
           DataTable table = dataFactory.Query(mySql, myParameter);
           return table;
       }
       public static int GetSumCountNum(string keyId)
       {
           string connectionString = ConnectionStringFactory.NXJCConnectionString;
           ISqlServerDataFactory dataFactory = new SqlServerDataFactory(connectionString);
           string mySql = @"select DisplayIndex from [dbo].[shift_CenterControlRecordItems] 
                            where KeyId=@KeyId
                            and Enabled=1
                            and DataType=1  order by DisplayIndex desc";
           SqlParameter myParameter = new SqlParameter("@KeyId", keyId);
           DataTable table = dataFactory.Query(mySql, myParameter);
           int CountNum=Convert.ToInt16(table.Rows[0][0]);
           return CountNum;
       }
       /// <summary>
       /// 获取中控记录表中需要查询的表的数目
       /// </summary>
       /// <param name="keyId"></param>
       /// <returns></returns>
       public static DataTable GetTableNum(string keyId) 
       {
           string connectionString = ConnectionStringFactory.NXJCConnectionString;
           ISqlServerDataFactory dataFactory = new SqlServerDataFactory(connectionString);
           string mySql = @"select DCSTableName,row_number() over (order by DCSTableName) as id
                                from [dbo].[shift_CenterControlRecordItems] 
                                where KeyId=@KeyId and Enabled=1and DataType=1
                                group by DCSTableName";
           SqlParameter myParameter = new SqlParameter("@KeyId", keyId);
           DataTable table = dataFactory.Query(mySql, myParameter);
           return table;
       }
       public static DataTable GetCountList(string keyId)
       {
           string connectionString = ConnectionStringFactory.NXJCConnectionString;
           ISqlServerDataFactory dataFactory = new SqlServerDataFactory(connectionString);
           string mySql = @"select ContrastID,DisplayIndex from [dbo].[shift_CenterControlRecordItems] 
                                where KeyId=@KeyId
                                and Enabled=1
                                and DataType=1
	                            order by DisplayIndex asc";
           SqlParameter myParameter = new SqlParameter("@KeyId", keyId);
           DataTable table = dataFactory.Query(mySql, myParameter);
           return table;      
       }
       public static DataTable GetCountList(string keyId, string DCStableName)
       {
           string connectionString = ConnectionStringFactory.NXJCConnectionString;
           ISqlServerDataFactory dataFactory = new SqlServerDataFactory(connectionString);
           string mySql = @"select ContrastID,DisplayIndex,DCSTableName from [dbo].[shift_CenterControlRecordItems] 
                                where KeyId=@KeyId
                                and Enabled=1
                                and DataType=1
                                and DCSTableName=@DCSTableName
	                            order by DisplayIndex asc";
           SqlParameter[] myParameter = { new SqlParameter("@KeyId", keyId),
                                         new SqlParameter("@DCSTableName", DCStableName)};
           DataTable table = dataFactory.Query(mySql, myParameter);
           return table;           
       }
       public static DataTable GetAllTableData(string keyId, string databaseId, string time) 
       {
           DataTable m_CenterControlTableStructrue= GetCenterControlTableStructrue();
           //keyId = "04BBFF6C-6915-443C-97F5-ECB4379A8987";
           //databaseId = "zc_nxjc_byc_byf_dcs01";
           //time = "2016-03-14";
           string m_Startime = time + " 00:00:00";
           string m_Endtime = time + " 23:59:59";

           //创建空DataTable 然后再往里面填数
           DataTable c_AList = GetCountList(keyId);    //获取标签及显示顺序
           int cACount = c_AList.Rows.Count;
           for (int cA = 0; cA < cACount; cA++)
           {
               string s_Count = "Sum" + c_AList.Rows[cA]["DisplayIndex"].ToString().Trim();
               m_CenterControlTableStructrue.Columns.Add(s_Count,typeof(float));
           }
            ////////创建模板表完成

           //数据处理主程序
           DataTable t_List = GetTableNum(keyId);
           int tCount=t_List.Rows.Count;   //获取表数目
           for (int t = 0; t < tCount; t++) 
           {
               string mTableName=t_List.Rows[t]["DCSTableName"].ToString().Trim(); //所要查询的历史表名称
               DataTable c_List=GetCountList(keyId, mTableName);     //获取该表下的列  拼SQL 语句
               int cCount = c_List.Rows.Count;
               string s_sum="";
               for (int c = 0; c < cCount;c++ ) 
               {
                   s_sum = s_sum + ", ([" + c_List.Rows[c]["ContrastID"].ToString().Trim() + "])" + " as Sum" + c_List.Rows[c]["DisplayIndex"].ToString().Trim();
               }
               string selectA = "select A.* from (";
               string row=",(row_number() over (partition by convert(VARCHAR(2),vDate,108) order by convert(VARCHAR(2),vDate,108))) as group_idx";
               string last = ")as A where group_idx=1";
               string mSelect = "select convert(VARCHAR(2),vDate,108) as hour";
               string mFrom = " from " + databaseId + ".dbo.History_" + mTableName;
               string mWhere = " where vDate>'" + m_Startime + "' and vDate<'" + m_Endtime + "'";
               //string mGroupby = " group by convert(VARCHAR(2),vDate,108)";
               //string mOrderby = " order by convert(VARCHAR(2),vDate,108)";
               string msql = selectA + mSelect + s_sum + row + mFrom + mWhere + last;
               string connectionString = ConnectionStringFactory.NXJCConnectionString;
               ISqlServerDataFactory dataFactory = new SqlServerDataFactory(connectionString);
               string mySql = @msql;
               DataTable table = dataFactory.Query(mySql);
               int tbCount=table.Rows.Count;

               //再次获取列数  列扫描填写
               for (int c = 0; c < cCount; c++)
               {
                   string cName="Sum" + c_List.Rows[c]["DisplayIndex"].ToString().Trim();
                   if(tbCount==24)
                   {
                       for(int tb=0;tb<24;tb++)
                       {
                        m_CenterControlTableStructrue.Rows[tb][cName]= table.Rows[tb][cName];
                       }
                      DataTable test1= m_CenterControlTableStructrue;
                   }else{           //如果不等于24行需要进行判断
                            for(int n=0;n<10;n++)
                            {
                                  string rName="0"+n.ToString();
                                  for (int nOne = 0; nOne < tbCount;nOne++ )
                                  {
                                      if (table.Rows[nOne]["hour"].ToString() == rName)
                                      {
                                          m_CenterControlTableStructrue.Rows[n][cName] = table.Rows[nOne][cName];
                                      }
                                  }
          
                             }

                            for (int m = 10; m < 24; m++) 
                            {

                                string rName =m.ToString();
                                for (int mTwo = 0; mTwo < tbCount; mTwo++)
                                {
                                    if (table.Rows[mTwo]["hour"].ToString() == rName)
                                    {
                                        m_CenterControlTableStructrue.Rows[m][cName] = table.Rows[mTwo][cName];
                                    }
                                }

                             }                  
                       }
                  }
               DataTable test2 = m_CenterControlTableStructrue;
           }
           return m_CenterControlTableStructrue;

       }
       private static DataTable GetCenterControlTableStructrue()
       {
           DataTable m_CenterControlTableStructrue = new DataTable();
           m_CenterControlTableStructrue.Columns.Add("hour", typeof(string));
           string mhour = "";
           for (int n = 0; n < 10;n++ )
           {
              mhour= "0"+ n.ToString();
              m_CenterControlTableStructrue.Rows.Add(mhour);
           }
           for (int m = 10; m < 24;m++ )
           {
               mhour = m.ToString();
               m_CenterControlTableStructrue.Rows.Add(mhour);
           }
           return m_CenterControlTableStructrue;
       }
       public static DataTable GetTagTable(string KeyID, string DatabaseID, string OrganizationId)
       {
           string viewName=GetDCSContrastViewTableName(OrganizationId);
           string connectionString = ConnectionStringFactory.NXJCConnectionString;
           ISqlServerDataFactory dataFactory = new SqlServerDataFactory(connectionString);
           string mySql = @"select  A.DisplayIndex, B.VariableDescription,A.ContrastID,C.DatabaseID,A.DCSTableName,A.Enabled 
                            from NXJC.dbo.shift_CenterControlRecordItems A ,{0} B,
                            NXJC.dbo.shift_CenterControlRecord C
                            where A.KeyId=@KeyId and A.ContrastID=B.VariableName 
                            and C.KeyID=@KeyId order by DisplayIndex";
           mySql= mySql.Replace("{0}", viewName);
           SqlParameter myParameter = new SqlParameter("@KeyId", KeyID);
           DataTable table = dataFactory.Query(mySql, myParameter);
           return table;      
       
       }
       private static string GetDCSContrastViewTableName(string OrganizationId)
       {
           string connectionString = ConnectionStringFactory.NXJCConnectionString;
           ISqlServerDataFactory dataFactory = new SqlServerDataFactory(connectionString);
           string mySql = @"select C.MeterDatabase+'.[dbo].[View_DCSContrast]' from 
			                    (select MeterDataBase from NXJC.dbo.system_Database A,
			                    (select DatabaseID from NXJC.dbo.system_Organization 
			                    where OrganizationID=@OrganizationId)B
			                     where A.DatabaseID =B.DatabaseID) C ";
           SqlParameter myParameter = new SqlParameter("@OrganizationId", OrganizationId);

           DataTable table = dataFactory.Query(mySql, myParameter);
           string tableName = table.Rows[0][0].ToString().Trim();
           return tableName;          
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

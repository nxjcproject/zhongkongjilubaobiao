using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using CenterControlRecord.Service.CenterControl;
using System.Data;
using StatisticalReport.Service.StatisticalReportServices;
using System.Text;
namespace CenterControlRecord.Web.UI_CenterControl.CenterControl
{
    public partial class CenterControlRecord : WebStyleBaseForEnergy.webStyleBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //base.InitComponts();
            if (!IsPostBack)
            {
                ////////////////////调试用,自定义的数据授权
#if DEBUG       
                List<string> m_DataValidIdItems = new List<string>() { "zc_nxjc_byc_byf", "zc_nxjc_qtx", "zc_nxjc_tsc_tsf", "zc_nxjc_ychc", "zc_nxjc_znc_znf", "zc_nxjc_klqc_klqf", "zc_nxjc_qtx_efc" };
                AddDataValidIdGroup("ProductionOrganization", m_DataValidIdItems);
                mPageOpPermission = "0000";
#elif RELEASE
#endif
                this.OrganisationTree_ProductionLine.Organizations = GetDataValidIdGroup("ProductionOrganization");                 //向web用户控件传递数据授权参数
                this.OrganisationTree_ProductionLine.PageName = "CenterControlRecord.aspx";                                     //向web用户控件传递当前调用的页面名称
                this.OrganisationTree_ProductionLine.LeveDepth = 5;



                string m_FunctionName = Request.Form["myFunctionName"] == null ? "" : Request.Form["myFunctionName"].ToString();             //方法名称,调用后台不同的方法
                string m_Parameter1 = Request.Form["myParameter1"] == null ? "" : Request.Form["myParameter1"].ToString();                   //方法的参数名称1
                string m_Parameter2 = Request.Form["myParameter2"] == null ? "" : Request.Form["myParameter2"].ToString();                   //方法的参数名称2
                string nowBrowser = Request.Form["nowBrowser"] == null ? "" : Request.Form["nowBrowser"].ToString();
                if (m_FunctionName == "ExcelStream")
                {
                    //ExportFile("xls", "导出报表1.xls");
                    string m_ExportTable = m_Parameter1.Replace("&lt;", "<");
                    m_ExportTable = m_ExportTable.Replace("&gt;", ">");
                    m_ExportTable = m_ExportTable.Replace("&nbsp", "  ");
                    if (nowBrowser == "ie" || nowBrowser == "chrome")
                    {
                        string filename = HttpUtility.UrlEncode(m_Parameter2 + "中控记录报表.xls", Encoding.UTF8).ToString();
                        CenterControlRecordService.ExportExcelFile("xls", filename, m_ExportTable);
                    }
                    if (nowBrowser == "firefox")
                    {
                        CenterControlRecordService.ExportExcelFile("xls", m_Parameter2 + "中控记录报表.xls", m_ExportTable);
                    }
                    else
                    {
                        CenterControlRecordService.ExportExcelFile("xls", m_Parameter2 + "中控记录报表.xls", m_ExportTable);
                    }
                }
            }
        }
        [WebMethod]
        public static char[] AuthorityControl()
        {
            return mPageOpPermission.ToArray();
        }
        [WebMethod]
        public static string GetPrcessTypeItem(string myOrganizationId)
        {
            DataTable table = CenterControlRecordService.GetProcessTypeInfo(myOrganizationId);
            string json=EasyUIJsonParser.DataGridJsonParser.DataTableToJson(table);
            return json;       
        }
        [WebMethod]
        public static string GetRecordNameItem(string myOrganizationId, string ProductionprocessId)
        {
            DataTable table = CenterControlRecordService.GetReportNameInfo(myOrganizationId, ProductionprocessId);
            string json = EasyUIJsonParser.DataGridJsonParser.DataTableToJson(table);
            return json;
        }
        [WebMethod]
        public static string GetRecordDataJson(string KeyID, string DatabaseID, string Time, string countType)
        {
            //DataTable table = CenterControlRecordService.GetTableFieldInfo(KeyID, DatabaseID, Time, m_id, m_DCSTableName);

            DataTable table = CenterControlRecordService.GetAllTableData(KeyID, DatabaseID, Time, countType);
            string json = EasyUIJsonParser.DataGridJsonParser.DataTableToJson(table);
            return json;        
        }
        [WebMethod]
        public static string GetHtmlTemplete(string KeyID) {
            DataTable table = CenterControlRecordService.GetHtmlTempleteTable(KeyID);
            string json = EasyUIJsonParser.DataGridJsonParser.DataTableToJson(table);
            return json;  
        
        }
        [WebMethod]
        public static string GetSumCount(string KeyID)
        {
            int num = CenterControlRecordService.GetSumCountNum(KeyID);
            string json = num.ToString();
            return json;   
        }
        [WebMethod]
        public static string GetTableList(string KeyID) 
        {
            DataTable table = CenterControlRecordService.GetTableNum(KeyID);
            string json = EasyUIJsonParser.DataGridJsonParser.DataTableToJson(table);
            return json;   
        }
        [WebMethod]
        public static string GetTagDataJson(string KeyID, string DatabaseID, string OrganizationId)
       {
           DataTable table = CenterControlRecordService.GetTagTable(KeyID, DatabaseID, OrganizationId);
           string json = EasyUIJsonParser.DataGridJsonParser.DataTableToJson(table);
           return json;   
       }
        //[WebMethod]
        //public static string GetRecordData(string mySql)
        //{
        //    DataTable table = CenterControlRecordService.GetRecordDataTable(mySql);
        //    string json = EasyUIJsonParser.DataGridJsonParser.DataTableToJson(table);
        //    return json;        
        //}
    }
}
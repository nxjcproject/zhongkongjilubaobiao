using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using CenterControlRecord.Service.CenterControl;
using System.Data;

namespace CenterControlRecord.Web.UI_CenterControl.CenterControl
{
    public partial class CenterControlRecord : WebStyleBaseForEnergy.webStyleBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            base.InitComponts();
            if (!IsPostBack)
            {
                ////////////////////调试用,自定义的数据授权
#if DEBUG       
                List<string> m_DataValidIdItems = new List<string>() { "zc_nxjc_byc_byf", "zc_nxjc_qtx", "zc_nxjc_tsc_tsf", "zc_nxjc_ychc","zc_nxjc_znc_znf" };
                AddDataValidIdGroup("ProductionOrganization", m_DataValidIdItems);
                mPageOpPermission = "0000";
#elif RELEASE
#endif
                this.OrganisationTree_ProductionLine.Organizations = GetDataValidIdGroup("ProductionOrganization");                 //向web用户控件传递数据授权参数
                this.OrganisationTree_ProductionLine.PageName = "CenterControlRecord.aspx";                                     //向web用户控件传递当前调用的页面名称
                this.OrganisationTree_ProductionLine.LeveDepth = 5;
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
        public static string GetRecordDataJson(string KeyID, string DatabaseID, string Time)
        {
            //DataTable table = CenterControlRecordService.GetTableFieldInfo(KeyID, DatabaseID, Time, m_id, m_DCSTableName);

            DataTable table = CenterControlRecordService.GetAllTableData(KeyID, DatabaseID, Time);
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
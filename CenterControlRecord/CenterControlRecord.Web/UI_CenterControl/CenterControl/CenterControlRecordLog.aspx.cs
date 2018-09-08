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
using CenterControlRecord.Service.CenterControRecord;

namespace CenterControlRecord.Web.UI_CenterControl.CenterControl
{
    public partial class CenterControlRecordLog : WebStyleBaseForEnergy.webStyleBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            base.InitComponts();
            ////////////////////调试用,自定义的数据授权
#if DEBUG
            List<string> m_DataValidIdItems = new List<string>() { "zc_nxjc_byc_byf", "zc_nxjc_qtx", "zc_nxjc_tsc_tsf", "zc_nxjc_ychc" };
            AddDataValidIdGroup("ProductionOrganization", m_DataValidIdItems);
            mPageOpPermission = "1111";
#elif RELEASE
#endif
            this.OrganisationTree_ProductionLine.Organizations = GetDataValidIdGroup("ProductionOrganization");                 //向web用户控件传递数据授权参数
            this.OrganisationTree_ProductionLine.PageName = "CenterControlRecordLog.aspx";                                     //向web用户控件传递当前调用的页面名称
            this.OrganisationTree_ProductionLine.LeveDepth = 5;
            if (!IsPostBack)
            {

            }
            //以下是接收js脚本中post过来的参数
            ///以下是接收js脚本中post过来的参数
            string m_FunctionName = Request.Form["myFunctionName"] == null ? "" : Request.Form["myFunctionName"].ToString();             //方法名称,调用后台不同的方法
            string m_Parameter1 = Request.Form["myParameter1"] == null ? "" : Request.Form["myParameter1"].ToString();                   //方法的参数名称1
            string m_Parameter2 = Request.Form["myParameter2"] == null ? "" : Request.Form["myParameter2"].ToString();                   //方法的参数名称2
            if (m_FunctionName == "ExcelStream")
            {
                //ExportFile("xls", "导出报表1.xls");
                string m_ExportTable = m_Parameter1.Replace("&lt;", "<");
                m_ExportTable = m_ExportTable.Replace("&gt;", ">");
                StatisticalReportHelper.ExportExcelFile("xls", m_Parameter2 + "中控记录版本日志.xls", m_ExportTable);
            }
        }
        [WebMethod]
        public static string GetCenterControlRecordLogDataJson(string organizationId)
        {
            DataTable table = CenterControlRecordLogService.GetCenterControlRecordLogData(organizationId);
            string json = EasyUIJsonParser.DataGridJsonParser.DataTableToJson(table);
            return json;
        }
    }
}



 
            
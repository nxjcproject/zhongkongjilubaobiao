<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CenterControlRecordLog.aspx.cs" Inherits="CenterControlRecord.Web.UI_CenterControl.CenterControl.CenterControlRecordLog" %>
<%@ Register Src="../../UI_WebUserControls/OrganizationSelector/OrganisationTree.ascx" TagName="OrganisationTree" TagPrefix="uc1" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>中控记录版本日志</title>
    <link rel="stylesheet" type="text/css" href="/lib/ealib/themes/gray/easyui.css" />
    <link rel="stylesheet" type="text/css" href="/lib/ealib/themes/icon.css" />
    <link rel="stylesheet" type="text/css" href="/lib/extlib/themes/syExtIcon.css" />
    <link rel="stylesheet" type="text/css" href="/lib/extlib/themes/syExtCss.css" />

    <script type="text/javascript" src="/lib/ealib/jquery.min.js" charset="utf-8"></script>
    <script type="text/javascript" src="/lib/ealib/jquery.easyui.min.js" charset="utf-8"></script>
    <script type="text/javascript" src="/lib/ealib/easyui-lang-zh_CN.js" charset="utf-8"></script>
    <script type="text/javascript" src="/lib/ealib/extend/editCell.js" charset="utf-8"></script>
    <script type="text/javascript" src="../js/common/PrintFile.js" charset="utf-8"></script>
    <script type="text/javascript" src="../js/common/MergeCell.js" charset="utf-8"></script>
    <script type="text/javascript" src="../js/page/CenterControl/CenterControlRecordLog.js" charset="utf-8"></script>
</head>
<body>
    <div class="easyui-layout" data-options="fit:true,border:false">
        <div data-options="region:'west',border:false " style="width: 150px;">
            <uc1:OrganisationTree ID="OrganisationTree_ProductionLine" runat="server" />
        </div>
         <div data-options="region:'center',border:false">
            <div class="easyui-layout" data-options="fit:true,border:false">
            <div id="toolbar_CenterControlRecordLogInfo" style="height: 30px">
            <table>
                <tr>
                    <td style="width:50px; text-align: right;">组织机构</td>
                    <td>
                        <input id="TextBox_OrganizationText" class="easyui-textbox" data-options="editable:false, readonly:true" style="width: 100px;" />
                    </td>
                    <td>
                        <a href="#" class="easyui-linkbutton" data-options="iconCls:'ext-icon-page_white_excel',plain:true" onclick="ExportFileFun();">导出</a>
                    </td>            
                </tr>            
            </table>
        </div>   
             <div data-options="region:'center',border:false">
                    <table id="table_CenterControlRecordLogData" ></table>
            </div>
         </div>
    </div>
    </div>
    <form id="form1" runat="server">
    </form>
</body>
</html>

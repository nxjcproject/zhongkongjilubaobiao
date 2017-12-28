 <%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CenterControlRecord.aspx.cs" Inherits="CenterControlRecord.Web.UI_CenterControl.CenterControl.CenterControlRecord" %>

<%@ Register Src="../../UI_WebUserControls/OrganizationSelector/OrganisationTree.ascx" TagName="OrganisationTree" TagPrefix="uc1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>中控记录</title>
    <link rel="stylesheet" type="text/css" href="/lib/ealib/themes/gray/easyui.css" />
    <link rel="stylesheet" type="text/css" href="/lib/ealib/themes/icon.css" />
    <link rel="stylesheet" type="text/css" href="/lib/extlib/themes/syExtIcon.css" />
    <link rel="stylesheet" type="text/css" href="/lib/extlib/themes/syExtCss.css" />

    <script type="text/javascript" src="/lib/ealib/jquery.min.js" charset="utf-8"></script>
    <script type="text/javascript" src="/lib/ealib/jquery.easyui.min.js" charset="utf-8"></script>
    <script type="text/javascript" src="/lib/ealib/easyui-lang-zh_CN.js" charset="utf-8"></script>
    <script type="text/javascript" src="/lib/ealib/extend/editCell.js" charset="utf-8"></script>
    <script type="text/javascript"  src="../js/common/PrintFile.js" charset="utf-8"></script>

    <script type="text/javascript" src="../js/page/CenterControl/CenterControlRecord.js" charset="utf-8"></script>
     <style type="text/css">
        .table {
            border-collapse: collapse;
        }
        .table .td {
            border: 1px solid black;
        }
    </style>
</head>
<body>
  <div class="easyui-layout" data-options="fit:true,border:false" style="padding: 5px;">
        <div data-options="region:'west',border:false " style="width: 150px;">
            <uc1:OrganisationTree ID="OrganisationTree_ProductionLine" runat="server" />
        </div>
         <div data-options="region:'center',border:false">
            <div class="easyui-layout" data-options="fit:true,border:false">
            <div id="toolbar_EnergyConsumptionPlanInfo" data-options="region:'north'" style="height: 80px">
            <table>
                <tr>
                    <td>
                    <table>
                            <tr>
                                <td>产线类别</td>
                                <td style="width: 100px;">
                                    <input id="TextBox_OrganizationText" class="easyui-textbox" data-options="editable:false, readonly:true" style="width: 100px;" />
                                </td>
                                  <td>工序选择</td>
                                <td style="width: 100px;">
                                    <input id="comb_ProcessType" class="easyui-combobox" style="width: 100px;"data-options="panelHeight:'auto'" />
                                </td>                           
                                <td>操作记录</td>
                                <td style="width: 100px;">
                                    <input id="comb_LineType" class="easyui-combobox" style="width: 100px;"data-options="panelHeight:'auto'" />
                                </td>  
                                  <td>查询日期</td>
                                <td style="width: 100px;">
                                    <input id="dbox_QueryDate" type="text" class="easyui-datebox" style="width:100px;" required="required"/>
                                 </td>                            
                                <%--<td>
                                    <a href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search',plain:true"
                                        onclick="GetCenterControlReportTagInfoFun();">标签查询</a>
                                </td>--%>
                                <td>
                                    <input id="TextBox_OrganizationId" style="width: 10px; visibility: hidden;" />
                                </td>
                            </tr>                           
                    </table>
                    </td>
                </tr>
                <tr>
                    <td>
                        <table>
                            <tr>
                                <td>统计类型</td>
                                <td style="width: 100px;">
                                    <select id="countType" class="easyui-combobox" style="width: 100px;"data-options="panelHeight:'auto'">
                                        <option value="avgValue" selected="true">平均值</option>
                                        <option value="oclockValue">整点值</option>
                                        <option value="MaxMin">最大差值</option>
                                    </select>
                                </td>
                                <td>
                                    <a href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search',plain:true"
                                onclick="QueryCenterControlReportInfoFun();">查询</a>
                                </td>
                                <td style="width: 230px;"></td>                                          
                                 <td><a href="#" class="easyui-linkbutton" data-options="iconCls:'ext-icon-page_white_excel',plain:true" onclick="ExportFileFun();">导出</a>
                                </td>
                                <td><a href="#" class="easyui-linkbutton" data-options="iconCls:'ext-icon-printer',plain:true" onclick="PrintFileFun();">打印</a> 
                                </td>
                                <td>
                                    <div class="datagrid-btn-separator"></div>
                                </td>
                                <td>
                                    <a href="#" class="easyui-linkbutton" data-options="iconCls:'icon-reload',plain:true" onclick="RefreshRecordDataFun();">刷新</a>
                                 </td>
                            </tr>
                        </table>                           
                      </td>
                </tr>
            </table>
        </div>   
            <div data-options="region:'center',border:'false'" >
               <div id="contain" data-options="region:'left'" style="height:auto; overflow: auto;padding:30px 40px 35px 40px "></div>               
             </div>

            </div>
         </div>
    </div>
    <div id="dialog_Tag" class="easyui-dialog" title="My Dialog" style="width:800px;height:600px" data-options="closed:true,resizable:true,modal:true">
        <table id="datagrid_Tag"></table>
    </div>



    <form id="form1" runat="server">
    <div>    
    </div>
    </form>
</body>
</html>


var m_KeyID = "";
var m_DatabaseID = "";
var m_Count = 0;
var m_TableList = [];
var m_organizationId = "";
$(document).ready(function () {
    //LoadProductionType('first');
    //loadOrganisationTree('first');

    //$('#TextBox_OrganizationId').textbox('hide');
    InitDate();
    //LoadEnergyConsumptionData('first');
    initPageAuthority();
    //LoadHtml(g_templateURL);

    ///// 测试
    //t_url = "";
    //g_templateURL = "/UI_CenterControl/ReportTemplete/" + t_url;
    //$("#contain").load(g_templateURL);
    LoadTagDatagrid();
});

function InitDate() {
    var myDate = new Date();
    var DateString = myDate.getFullYear() + '-' + (myDate.getMonth() + 1) + '-' + (myDate.getDate()-1);
    $('#dbox_QueryDate').datebox('setValue', DateString);
}
//初始化页面的增删改查权限
function initPageAuthority() {
    $.ajax({
        type: "POST",
        url: "CenterControlRecord.aspx/AuthorityControl",
        data: "",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        async: false,//同步执行
        success: function (msg) {
            var authArray = msg.d;
            //增加
            //if (authArray[1] == '0') {
            //    $("#add").linkbutton('disable');
            //}
            //修改
            if (authArray[2] == '0') {
                $("#id_save").linkbutton('disable');
            }
            //删除
            //if (authArray[3] == '0') {
            //    $("#delete").linkbutton('disable');
            //}
        }
    });
}
function onOrganisationTreeClick(myNode) {
    //alert(myNode.text);
    m_organizationId = myNode.OrganizationId;
    $('#TextBox_OrganizationId').attr('value', m_organizationId);  //textbox('setText', myNode.OrganizationId);
    $('#TextBox_OrganizationText').textbox('setText', myNode.text);
    //$('#TextBox_OrganizationType').textbox('setText', myNode.OrganizationType);
    PrcessTypeItem(m_organizationId);
}
function PrcessTypeItem(m_OrganizationId)
{
    $.ajax({
        type: "POST",
        url: "CenterControlRecord.aspx/GetPrcessTypeItem",
        data: "{myOrganizationId:'" + m_OrganizationId + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            var m_MsgData = jQuery.parseJSON(msg.d);
            if (m_MsgData.total == 0) {
                $.messager.alert('提示','未查询到工序','info');
            }
            //InitializeEnergyConsumptionGrid(m_GridCommonName, m_MsgData);
            $('#comb_ProcessType').combobox({
                data:m_MsgData.rows,
                valueField: 'id',
                textField: 'text',
                onSelect: function (param) {
                    var m_productionprocessId = param.value;
                    RecordNameItem(m_OrganizationId,m_productionprocessId);
                }
            });
        },
        error: function () {
            $.messager.alert('提示','工序加载失败！');
        }
    });
}
function RecordNameItem(OrganizationId, ProductionprocessId) {
    var m_OrganizationID = OrganizationId;
    $.ajax({
        type: "POST",
        url: "CenterControlRecord.aspx/GetRecordNameItem",
        data: "{myOrganizationId:'" + OrganizationId + "',ProductionprocessId:'" + ProductionprocessId + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            var m_MsgData = jQuery.parseJSON(msg.d);
            if (m_MsgData.total == 0 ) {
                $.messager.alert('提示','未查询到该类别下的产线！')
            }
            $('#comb_LineType').combobox({
                data: m_MsgData.rows,
                valueField: 'id',
                textField: 'text',
                onSelect: function (param) {
                    m_KeyID = param.KeyID;
                    m_DatabaseID = param.DatabaseID;
                    QueryTableCount(m_KeyID);
                    LoadHtml(m_KeyID);
                  
                }
            });
        },
        error: function () {
            $.messager.alert('提示', '操作记录类型加载失败！');
        }
    });
}
//加载报表模板
function LoadHtml(KeyId) {
    $.ajax({
        type: "POST",
        url: "CenterControlRecord.aspx/GetHtmlTemplete",
        data: "{KeyID:'" + KeyId + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            var m_MsgData = jQuery.parseJSON(msg.d);
            if (m_MsgData.rows[0]["TemplateUrl"]=="") {
                $.messager.alert('提示','未查询到该记录的模板！');
            }
            t_url = m_MsgData.rows[0]["TemplateUrl"];
            g_templateURL = "/UI_CenterControl/ReportTemplete/" + t_url;
            //$("#contain").empty();
            $("#contain").load(g_templateURL);
        },
        error:function(){
            $.messager.alert('提示', '记录模板加载失败！');
        }
    })
  
}

function QueryTableCount(KeyId) {
    $.ajax({
        type: "POST",
        url: "CenterControlRecord.aspx/GetSumCount",
        data: "{KeyID:'" + KeyId + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            var m_MsgData = jQuery.parseJSON(msg.d);
            m_Count = m_MsgData;
        },
        error: function () {
            $.messager.alert('提示','未能加载字段个数！');
        }
    })
}
function QueryCenterControlReportInfoFun() {
    var KeyID = m_KeyID;
    var DatabaseID = m_DatabaseID;
    var m_Time = $('#dbox_QueryDate').datebox('getValue');
    var m_SumCount = m_Count;      

    $("#contain").load(g_templateURL);
        $.ajax({
            type: "POST",
            url: "CenterControlRecord.aspx/GetRecordDataJson",
            data: "{KeyID:'" + KeyID + "',DatabaseID:'" + DatabaseID + "',Time:'" + m_Time  + "'}",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (msg) {
                var m_MsgData = jQuery.parseJSON(msg.d);

                var m_table = document.getElementById("RecordTable");
                for (var m = 0; m < m_MsgData.rows.length; m++)    // 数据表行数
                {
                    var m_hour = parseInt(m_MsgData.rows[m]["hour"], 10);     //根据hour这列获取“时”
                    var i = $("#" + m_hour).parent().index();
                    var j = $("#" + m_hour).index();
                    for (var n = 1; n < m_SumCount + 1; n++)
                    {
                        if (m_MsgData.rows[m]["Sum" + n] != undefined) {
                            var m_Cell = m_table.rows[i].cells[j + n];
                            var myData = m_MsgData.rows[m]["Sum" + n];
                            var value = Number(myData).toFixed(2) == "NaN" ? myData : Number(myData).toFixed(2);
                            m_Cell.innerText = value;
                        }
                    }
                }
            },
            error: function () {
                $.messager.alert('提示', '数据加载错误！');
            }
        })
}
function RefreshRecordDataFun()
{ QueryCenterControlReportInfoFun(); }

function LoadTagDatagrid()
{
    $('#datagrid_Tag').datagrid({
        columns:[[
            { field: 'DisplayIndex', title: '标签号', width: 50 },
            { field: 'VariableDescription', title: '名称', width: 150 },
            { field: 'ContrastID', title: '标签名', width: 100, align: 'right' },
            { field: 'Enabled', title: '是否可见', width: 80, align: 'right' },
            { field: 'DatabaseID', title: 'DCS数据库', width: 140 },
            { field: 'DCSTableName', title: 'DCS表名', width: 120 }
        ]]
    });

}

function GetCenterControlReportTagInfoFun()
{

    var OrganizationId= m_organizationId;
    var KeyID = m_KeyID;
    var DatabaseID = m_DatabaseID;
    var m_Time = $('#dbox_QueryDate').datebox('getValue');
    var m_SumCount = m_Count;

    $.ajax({
        type: "POST",
        url: "CenterControlRecord.aspx/GetTagDataJson",
        data: "{KeyID:'" + KeyID + "',DatabaseID:'" + DatabaseID + "',OrganizationId:'" + OrganizationId + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            var m_MsgData = jQuery.parseJSON(msg.d);
            $('#datagrid_Tag').datagrid('loadData', m_MsgData);
        },
        error: function () {
            $.messager.alert('提示', '数据加载错误！');
        }
    })

    $('#dialog_Tag').dialog('open');
}



var m_KeyID = "";
var m_DatabaseID = "";
var m_Count = 0;
var m_organizationId = "";
var mProductionPrcessId = "";
var mRecordType = "";

$(document).ready(function () {
    InitDate();
    getbrowser();
});

function InitDate() {
    var myDate = new Date();
    var DateString = myDate.getFullYear() + '-' + (myDate.getMonth() + 1) + '-' + (myDate.getDate() - 1);
    $('#dbox_QueryDate').datebox('setValue', DateString);
}

function onOrganisationTreeClick(myNode) {
    m_organizationId = myNode.OrganizationId;
    $('#TextBox_OrganizationId').attr('value', m_organizationId);
    $('#TextBox_OrganizationText').textbox('setText', myNode.text);
    PrcessTypeItem(m_organizationId);
}

function PrcessTypeItem(m_OrganizationId) {
    $.ajax({
        type: "POST",
        url: "CenterControlRecord.aspx/GetPrcessTypeItem",
        data: "{myOrganizationId:'" + m_OrganizationId + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            var m_MsgData = jQuery.parseJSON(msg.d);
            if (m_MsgData.total == 0) {
                $.messager.alert('提示', '未查询到工序', 'info');
            }
            $('#comb_ProcessType').combobox({
                data: m_MsgData.rows,
                valueField: 'id',
                textField: 'text',
                onSelect: function (param) {
                    mProductionPrcessId = param.id;
                    RecordNameItem(m_OrganizationId, mProductionPrcessId);
                }
            });
        },
        error: function () {
            $.messager.alert('提示', '工序加载失败！');
        }
    });
}
function RecordNameItem(OrganizationId, ProductionPrcessId) {
    var m_OrganizationID = OrganizationId;
    $.ajax({
        type: "POST",
        url: "CenterControlRecord.aspx/GetRecordNameItem",
        data: "{myOrganizationId:'" + m_OrganizationID + "',ProductionprocessId:'" + ProductionPrcessId + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            var m_MsgData = jQuery.parseJSON(msg.d);
            if (m_MsgData.total == 0) {
                $.messager.alert('提示', '未查询到该类别下的产线！')
            }
            $('#comb_LineType').combobox({
                data: m_MsgData.rows,
                valueField: 'id',
                textField: 'text',
                onSelect: function (param) {
                    mRecordType = param.id;
                }
            });
        },
        error: function () {
            $.messager.alert('提示', '操作记录类型加载失败！');
        }
    });
}

//加载报表模板
function LoadHtml(m_SumCount) {
    var oBox = document.getElementById("contain");
    oBox.style.border = "30px";      //设置实线宽度
    oBox.style.borderStyle = "solid"   //设置边界为实线
    oBox.style.borderColor = "lightgray";  //设置实线颜色为灰色   
    var st = m_SumCount * 60 + 180;//根据模板列数获取母版宽度 
    $("#contain").css("width", st);//设置模板所在母版的宽度
}

function QueryCenterControlReportInfoFun() {
    var m_Time = $('#dbox_QueryDate').datebox('getValue');
    var m_countType = $('#countType').combobox('getValue');

    var win = $.messager.progress({
        title: '请稍后',
        msg: '数据载入中...'
    });
    $.ajax({
        type: "POST",
        url: "CenterControlRecord.aspx/GetRecordDataJson",
        data: "{OrganizationId:'" + m_organizationId + "',ProductionPrcessId:'" + mProductionPrcessId + "',mRecordType:'" + mRecordType + "',Time:'" + m_Time + "',countType:'" + m_countType + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            $.messager.progress('close');
            var m_MsgData = jQuery.parseJSON(msg.d);
            m_SumCount = m_MsgData.SumCount;
            m_TemplateUrl = m_MsgData.TemplateUrl;
            if (m_TemplateUrl == undefined || m_TemplateUrl == null || m_TemplateUrl == "") {
                $.messager.alert('提示', '该时间无中控记录');
                return;
            }
            var g_templateURL = "/UI_CenterControl/ReportTemplete/" + m_TemplateUrl;
            LoadHtml(m_SumCount);
            
            $("#contain").load(g_templateURL, function () {
                var m_table = document.getElementById("RecordTable");   //所有黑白表的id
                for (var m = 0; m < m_MsgData.rows.length; m++)        // 数据表行数
                {
                    var m_hour = parseInt(m_MsgData.rows[m]["hour"], 10);     //根据hour这列获取“时”
                    var i = $("#" + m_hour).parent().index();
                    var j = $("#" + m_hour).index();
                    for (var n = 1; n < m_SumCount + 1; n++) {
                        if (m_MsgData.rows[m]["Sum" + n] != undefined) {
                            var m_Cell = m_table.rows[i].cells[j + n];
                            var myData = m_MsgData.rows[m]["Sum" + n];
                            var value = myData == "" ? myData : Number(myData).toFixed(2);
                            m_Cell.innerText = value;
                        }
                    }
                }
            });
        },
        beforeSend: function () {
            win;
        },
        error: function () {
            $.messager.alert('提示', '数据加载错误！');
            $.messager.progress('close');
        }
    })
}

function RefreshRecordDataFun() {
    QueryCenterControlReportInfoFun();
}

//获得浏览器名称
function getbrowser() {
    var brow = $.browser;
    if (brow.msie) {
        BrowserName = "IE";
        var m_Version = (brow.version).substring(0, brow.version.indexOf('.'));
        BrowserVersion = parseInt(m_Version, 0);
    }
    if (brow.mozilla) {
        BrowserName = "FF";
        var m_Version = (brow.version).substring(0, brow.version.indexOf('.'));
        BrowserVersion = parseInt(m_Version, 0);
    }
    if (brow.safari) {
        BrowserName = "Safari";
        var m_Version = (brow.version).substring(0, brow.version.indexOf('.'));
        BrowserVersion = parseInt(m_Version, 0);
    }
    if (brow.opera) {
        BrowserName = "Opera";
        var m_Version = (brow.version).substring(0, brow.version.indexOf('.'));
        BrowserVersion = parseInt(m_Version, 0);
    }
    if (brow.chrome) {
        BrowserName = "Chrome";
        var m_Version = (brow.version).substring(0, brow.version.indexOf('.'));
        BrowserVersion = parseInt(m_Version, 0);
    }
    //判断是否IE浏览器
}

var nowBrowser = '';
function ExportFileFun() {
    if (BrowserName == "IE") {
        nowBrowser = "ie";
    }
    if (BrowserName == "FF") {
        nowBrowser = "firefox";
    }
    if (BrowserName == "Chrome") {
        nowBrowser = "chrome";
    }

    var m_FunctionName = "ExcelStream";
    var m_Parameter1 = $("#contain").html();
    var m_Parameter2 = "";

    var m_ReplaceAlllt = new RegExp("<", "g");
    var m_ReplaceAllgt = new RegExp(">", "g");
    m_Parameter1 = m_Parameter1.replace(m_ReplaceAlllt, "&lt;");
    m_Parameter1 = m_Parameter1.replace(m_ReplaceAllgt, "&gt;");

    var form = $("<form id = 'ExportFile'>");   //定义一个form表单
    form.attr('style', 'display:none');   //在form表单中添加查询参数
    form.attr('target', '');
    form.attr('method', 'post');
    form.attr('action', "CenterControlRecord.aspx");

    var input_Method = $('<input>');
    input_Method.attr('type', 'hidden');
    input_Method.attr('name', 'myFunctionName');
    input_Method.attr('value', m_FunctionName);
    var input_Data1 = $('<input>');
    input_Data1.attr('type', 'hidden');
    input_Data1.attr('name', 'myParameter1');
    input_Data1.attr('value', m_Parameter1);
    var input_Data2 = $('<input>');
    input_Data2.attr('type', 'hidden');
    input_Data2.attr('name', 'myParameter2');
    input_Data2.attr('value', m_Parameter2);
    var input_Data3 = $('<input>');
    input_Data3.attr('type', 'hidden');
    input_Data3.attr('name', 'nowBrowser');
    input_Data3.attr('value', nowBrowser);

    $('body').append(form);  //将表单放置在web中 
    form.append(input_Method);   //将查询参数控件提交到表单上
    form.append(input_Data1);   //将查询参数控件提交到表单上
    form.append(input_Data2);   //将查询参数控件提交到表单上
    form.append(input_Data3);
    form.submit();
    //释放生成的资源
    form.remove();
}
function PrintFileFun() {
    var m_ReportTableHtml = GetTreeTableHtml("grid_Main", "能耗日报", "Name", SelectOrganizationName, SelectDatetime);
    PrintHtml(m_ReportTableHtml);
}



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
    getbrowser();
});

function InitDate() {
    var myDate = new Date();
    var DateString = myDate.getFullYear() + '-' + (myDate.getMonth() + 1) + '-' + (myDate.getDate() - 1);
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
            //InitializeEnergyConsumptionGrid(m_GridCommonName, m_MsgData);
            $('#comb_ProcessType').combobox({
                data: m_MsgData.rows,
                valueField: 'id',
                textField: 'text',
                onSelect: function (param) {
                    var m_productionprocessId = param.value;
                    RecordNameItem(m_OrganizationId, m_productionprocessId);
                }
            });
        },
        error: function () {
            $.messager.alert('提示', '工序加载失败！');
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
            if (m_MsgData.total == 0) {
                $.messager.alert('提示', '未查询到该类别下的产线！')
            }
            $('#comb_LineType').combobox({
                data: m_MsgData.rows,
                valueField: 'id',
                textField: 'text',
                onSelect: function (param) {
                    m_KeyID = param.KeyID;
                    m_DatabaseID = param.DatabaseID;
                    QueryTableCount(m_KeyID);

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
    //var winH = $(window).height();




    var oBox = document.getElementById("contain");
    oBox.style.border = "30px";      //设置实线宽度
    oBox.style.borderStyle="solid"   //设置边界为实线
    oBox.style.borderColor = "lightgray";  //设置实线颜色为灰色
    $("#contain").css("width", 'auto');//设置模板宽度为自动



    
    $.ajax({
        type: "POST",
        url: "CenterControlRecord.aspx/GetHtmlTemplete",
        data: "{KeyID:'" + KeyId + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            var m_MsgData = jQuery.parseJSON(msg.d);
            if (m_MsgData.rows[0]["TemplateUrl"] == "") {
                $.messager.alert('提示', '未查询到该记录的模板！');
            }
            t_url = m_MsgData.rows[0]["TemplateUrl"];
            g_templateURL = "/UI_CenterControl/ReportTemplete/" + t_url;
            //$("#contain").empty();
            $("#contain").load(g_templateURL);
            var mwidth = $("#RecordTable").css('width');
        },
        error: function () {
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
            $.messager.alert('提示', '未能加载字段个数！');
        }
    })
}
function QueryCenterControlReportInfoFun() {



    var KeyID = m_KeyID;
    var DatabaseID = m_DatabaseID;
    var m_Time = $('#dbox_QueryDate').datebox('getValue');
    var m_SumCount = m_Count;
    var st=m_SumCount*51;//根据模板列数获取母版宽度
    if (KeyID == "" || DatabaseID == "" || m_Time == "" || m_SumCount == "") {
        $.messager.alert('提示', '请选择对应选项！');
    }
    else {
            LoadHtml(m_KeyID);


    $("#contain").css("width", st);//设置模板所在母版的宽度
    $("#contain").load(g_templateURL);
    
    var win = $.messager.progress({
        title: '请稍后',
        msg: '数据载入中...'
    });
        $.ajax({
            type: "POST",
            url: "CenterControlRecord.aspx/GetRecordDataJson",
            data: "{KeyID:'" + KeyID + "',DatabaseID:'" + DatabaseID + "',Time:'" + m_Time + "'}",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (msg) {
                $.messager.progress('close');
                var m_MsgData = jQuery.parseJSON(msg.d);

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

   
}
function RefreshRecordDataFun()
{ QueryCenterControlReportInfoFun(); }

function LoadTagDatagrid() {
    $('#datagrid_Tag').datagrid({
        columns: [[
            { field: 'DisplayIndex', title: '标签号', width: 50 },
            { field: 'VariableDescription', title: '名称', width: 150 },
            { field: 'ContrastID', title: '标签名', width: 100, align: 'right' },
            { field: 'Enabled', title: '是否可见', width: 80, align: 'right' },
            { field: 'DatabaseID', title: 'DCS数据库', width: 140 },
            { field: 'DCSTableName', title: 'DCS表名', width: 120 }
        ]]
    });

}

function GetCenterControlReportTagInfoFun() {

    var OrganizationId = m_organizationId;
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
var nowBrowser = '';
//function InitializeIframe() {
//    if (BrowserName == "IE") {
//        nowBrowser = "ie";
//    }
//    if (BrowserName == "FF") {
//        nowBrowser = "firefox";
//    }
//}
//获得浏览器名称
function getbrowser() {
    //var userAgent = navigator.userAgent; //取得浏览器的userAgent字符串
    //var isOpera = userAgent.indexOf("Opera") > -1;
    //if (isOpera) { return "Opera" }; //判断是否Opera浏览器
    //if (userAgent.indexOf("Firefox") > -1) { return "FF"; } //判断是否Firefox浏览器
    //if (userAgent.indexOf("Safari") > -1) { return "Safari"; } //判断是否Safari浏览器
    //if (userAgent.indexOf("compatible") > -1 && userAgent.indexOf("MSIE") > -1 && !isOpera) { return "IE"; };var brow = $.browser;
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



var SelectDatetime = "";
$(function () {
    loadDataGrid("first");
});

var organizationID = "";
function onOrganisationTreeClick(node) {
    organizationID = node.OrganizationId;
    if (node.OrganizationType == '分公司') {
        $.messager.alert("提示", "请选择分厂级别！");
    } else {
        $('#TextBox_OrganizationText').textbox('setText', node.text);
        $('#TextBox_OrganizationId').val(organizationID);
        QueryData(organizationID);
    }
}


function loadDataGrid(type, myData) {
    if (type == "first") {
        $('#table_CenterControlRecordLogData').datagrid({
            columns: [[
                    { field: 'ProductionPrcessName', title: '工序名称', width: 150 },
                    { field: 'RecordName', title: '记录名称', width: 150 },
                    { field: 'CreateDate', title: '版本时间', width: 150 }
            ]],
            fit: true,
            toolbar: "#toolbar_CenterControlRecordLogInfo",
            rownumbers: true,
            singleSelect: true,
            striped: true,
            data: []
        });
    }
}

function QueryData(organizationID) {
    var win = $.messager.progress({
        title: '请稍后',
        msg: '数据载入中...'
    });
    var g_organizationId = organizationID;
    $.ajax({
        type: "POST",
        url: "CenterControlRecordLog.aspx/GetCenterControlRecordLogDataJson",
        data: '{organizationId: "' + g_organizationId + '"}',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            $.messager.progress('close');
            var myData = jQuery.parseJSON(msg.d);
            $('#table_CenterControlRecordLogData').datagrid('loadData', myData);
            myMergeCell('table_CenterControlRecordLogData', "ProductionPrcessName");
        },
        beforeSend: function (XMLHttpRequest) {
            win;
        },
        error: function () {
            $.messager.progress('close');
        }
    });


}

function ExportFileFun() {
    var m_FunctionName = "ExcelStream";
    var m_Parameter1 = GetDataGridTableHtml("table_CenterControlRecordLogData", "中控记录版本日志",SelectDatetime);
    var m_Parameter2 = SelectDatetime;

    var m_ReplaceAlllt = new RegExp("<", "g");
    var m_ReplaceAllgt = new RegExp(">", "g");
    m_Parameter1 = m_Parameter1.replace(m_ReplaceAlllt, "&lt;");
    m_Parameter1 = m_Parameter1.replace(m_ReplaceAllgt, "&gt;");

    var form = $("<form id = 'ExportFile'>");   //定义一个form表单
    form.attr('style', 'display:none');   //在form表单中添加查询参数
    form.attr('target', '');
    form.attr('method', 'post');
    form.attr('action', "CenterControlRecordLog.aspx");

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

    $('body').append(form);  //将表单放置在web中 
    form.append(input_Method);   //将查询参数控件提交到表单上
    form.append(input_Data1);   //将查询参数控件提交到表单上
    form.append(input_Data2);   //将查询参数控件提交到表单上
    form.submit();
    //释放生成的资源
    form.remove();
}
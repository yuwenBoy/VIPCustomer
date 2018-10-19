// 初始化页面
$(function () {
    initPage();
});
var URL = "/IServer/OrderManage/OrderManage.ashx?action=";
var initPage = function () {
    $('#table_list').jqGrid({
        url: URL + 'GetOrderSaleManagerPager',
        mtype: 'post',
        datatype: 'json',
        viewrecords: true,  //  是否显示总记录数
        styleUI: 'Bootstrap',
        colModel: [
              { label: '主键ID', name: 'PKID', hidden: true, key: true, },
              { label: 'CustomerID', name: 'CustomerID', hidden: true },
              { label: 'ToExamineState', name: 'ToExamineState', hidden: true },
              { label: 'IsEdit', name: 'IsEdit', hidden: true },
            {
                label: '销售单状态', name: '', width: 120, formatter: function (cellvalue, options, rowObject) {
                    var value = rowObject.ToExamineState;
                    switch (true) {
                        case value == 0:
                            return '<span style="color:red;">未提交</span>';
                        case value == -1000:
                            return '作废销售单';
                        case value == 1100:
                            return '返款申请驳回';
                        case value == 400:
                            return '待审批返款';
                        case value < 400:
                            return '<span style="color:red;">未返款申请</span>';
                        default:
                            return '返款初审通过';
                    }
                }
            },
            { label: '生效日期', name: 'SubmitTime', width: 120, formatter: "date", formatoptions: { newformat: 'Y-m-d' } },
            { label: '记录编号', name: 'Code', width: 130 },
            { label: '客户名称', name: 'CustomerName', width: 160 },
            { label: '购买方式', name: 'BuyWay', width: 120 },
            { label: '采购类型', name: 'PurchaseType', width: 120 },
            { label: '创建日期', name: 'CreateTime', width: 120, formatter: "date", formatoptions: { newformat: 'Y-m-d' } },
            {
                label: '经销店', name: 'DealerName', width: 100
            },
            { label: '记录状态', name: 'OrderState', width: 120 },
            { label: '记录名称', name: 'RecordName', width: 120 },
            { label: '制表人', name: 'Watchmaker', width: 120 },
            { label: '车辆用途', name: 'CarUse', width: 120 },
            {
                label: '需要异地交车', name: 'DifferentPlace', width: 100, align: 'center', formatter: function (cellvalue, options, rowObject) {
                    var temp = "";
                    if (cellvalue == true) {
                        temp = '<input type="checkbox" value="1" checked="true" disabled="disabled"/>';
                        return temp;
                    }
                    else {
                        temp = '<input type="checkbox" value="0" disabled="disabled"/>';
                        return temp;
                    }
                }
            },
            { label: '客户建议', name: 'CustomerSuggestion', width: 120 },
            { label: '记录备注', name: 'BaseRemark', width: 120 },
            { label: '返款审核人', name: 'Replyer3', width: 120 },
            { label: '返款意见', name: 'ReComment3', width: 120 },
            { label: '审核时间', name: 'ReDate3', width: 120, formatter: "date", formatoptions: { newformat: 'Y-m-d' } },
            { label: '返款备注', name: 'ReRemark3', width: 120 }
        ],
        autowidth: true,
        height: 568,
        shrinkToFit: false,
        rownumbers: true,   // 显示行号
        rowNum: 50,
        rowList: [50, 100, 200, 500],
        pager: "#pager_list",
        pagerpos: 'left',
        jsonReader: {
            records: "totalCount", root: "dataList", total: "totalpages", page: "currPage",
            repeatitems: false
        }
    });

    // 查询
    $('#btnSearch').click(function () {
        $('#table_list').jqGrid('setGridParam', {
            url: URL + 'GetOrderSaleManagerPager',
            mtype: 'post',
            postData: {
                OrdersearchContext: function () {
                    var code = $('input[name=Code]').val();  // 订单编号
                    var StringToJson = JSON.stringify({ "Code": code });
                    return StringToJson;
                }
            },
            page: 1
        }).trigger('reloadGrid');  // 重新载入
    });
}

// 编辑
var EditDetail = function () {
    var row = getRows('#table_list');
    if (row != null) {
        $('#myModal').modal('show');
    }
    else {
        layer.msg('请至少选择一个项目！');
        return;
    }
}

// 作废销售单
var DeleteData = function () {
    var row = getRows('#table_list');
    if (row != null) {
        layer.confirm('您确定要选定作废的销售单吗？<br>（没有编辑权限的销售单不会被作废。）', { icon: 3, title: '提示信息' }, function (index) {
            var index = layer.msg('正在处理作废销售单，请稍候', { icon: 16, time: false, shade: 0.8 });
            setTimeout(function () {
                $.ajax({
                    url: URL + 'DeleteOrders',
                    type: 'post',
                    async: false,
                    data: { orderId: row },
                    success: function (data) {
                        var index = parent.layer.getFrameIndex(window.name); //先得到当前iframe层的索引
                        $('#table_list').jqGrid({}).trigger('reloadGrid');  // 重新载入
                        layer.close(index);
                        layer.msg("操作完成<br>（没有编辑权限的销售单不会被作废。）。");
                    }, error: function (data) {
                        top.layer.close(index);
                    }
                });
                return false;
            }, 2000);
        });
    }
    else {
        layer.msg('请至少选择一个项目！');
        return;
    }
}

// 申请返款
var SubmitRebatesData = function () {
    var row = getRows('#table_list');
    if (row != null) {
        var rowData = $("#table_list").jqGrid('getRowData', row);
        var submitData = new Array();
        if (rowData.ToExamineState >= 0 && rowData.ToExamineState < 400) {
            submitData.push(rowData.PKID);
        }
        layer.confirm('确定进行返款申请吗？', { icon: 3, title: '提示信息' }, function (index) {
            var index = layer.msg('正在处理返款申请销售单，请稍候', { icon: 16, time: false, shade: 0.8 });
            setTimeout(function () {
                $.ajax({
                    url: URL + 'SubmitRebates',
                    type: 'post',
                    dataType: 'json',
                    data: { orderId: submitData.toString() },
                    success: function (res) {
                        if (res.msg != '') {
                            layer.alert(res.msg);
                        }
                        else {
                            layer.alert("返款申请提交成功。");
                            var index = parent.layer.getFrameIndex(window.name); //先得到当前iframe层的索引
                            $('#table_list').jqGrid({}).trigger('reloadGrid');  // 重新载入
                            layer.close(index);
                        }
                    }, error: function (data) {
                        top.layer.close(index);
                    }
                });
                return false;
            }, 2000);
        });
    }
    else {
        layer.msg('请至少选择一个项目！');
        return;
    }
}
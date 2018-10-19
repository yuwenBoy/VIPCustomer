$(function () {
    initPage();
    GetcboCars();
})
var $ElemTable = $('#table_list');
var URL = "/IServer/OrderManage/OrderManage.ashx?action=";
var initPage = function () {
    $ElemTable.jqGrid({
        url: URL + 'GetOrdersByToExamineState',
        mtype: 'post',
        datatype: 'json',
        viewrecords: true,  //  是否显示总记录数
        styleUI: 'Bootstrap',
        colModel: [
              { label: '主键ID', name: 'PKID', hidden: true, key: true, },
              {
                  label: '审核状态', name: 'ToExamineState', width: 120, formatter: function (cellvalue, options, rowObject) {
                      switch (rowObject.ToExamineState) {
                          case 200:
                              return '<span style="color:red;">未审核</span>';
                          case -200:
                              return '<span style="color:red;">已驳回</span>';
                          default:
                              return '已审核';
                      }
                  }
              },
              {
                  label: '审核状态', name: 'ToExamineState', hidden: true
              },
            { label: '订单编号', name: 'Code', width: 130 },
            { label: '经销店', name: 'DealerName', width: 120 },
            { label: '客户名称', name: 'CustomerName', width: 160 },
            { label: '客户性质', name: '客户性质1名称', width: 120 },
            { label: '客户性质2', name: '客户性质2名称', width: 120 },
            {
                label: '申请大客户资源', name: 'IsApplyMaxCustomerResources', align: 'center', width: 120, formatter: function (cellvalue, options, rowObject) {
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
            {
                label: '非本地上牌', name: 'OffAddressOnCardReport', align: 'center', width: 120, formatter: function (cellvalue, options, rowObject) {
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
            {
                label: '发票不一致', name: 'InvoiceAndCustomerAtypism', align: 'center', width: 100, formatter: function (cellvalue, options, rowObject) {
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
            {
                label: '发票客户信息', name: 'InvoiceCustomerInfo', align: 'center', width: 100
            },

            { label: '提交日期', name: 'CreateTime', width: 120, formatter: "date", formatoptions: { newformat: 'Y-m-d' } },
            { label: 'FTMS回复人', name: 'Watchmaker', width: 120 },
            { label: 'FTMS回复意见', name: 'ReComment3', width: 120 },
            { label: 'FTMS回复日期', name: 'CreateTime', width: 120, formatter: "date", formatoptions: { newformat: 'Y-m-d' } },
            { label: 'FTMS回复备注', name: 'ReRemark3', width: 120 },
            { label: '电子邮件', name: 'ReRemark3', width: 120 },
            { label: '记录状态', name: 'OrderState', width: 120 },
            { label: '记录名称', name: 'RecordName', width: 120 },
            {
                label: '需要异地交车', name: 'DifferentPlace', align: 'center', width: 120, formatter: function (cellvalue, options, rowObject) {
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
            }
        ],
        multiselect: true, // 多选框
        multiboxonly: true,
        autowidth: true,
        height: 560,
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
}

// 获取车辆名称
var GetcboCars = function () {
    $.ajax({
        url: '/IServer/Cars/CarsManage.ashx?action=cboCarName',
        type: 'post',
        async: false,
        dataType: 'json',
        success: function (data) {
            for (var i = 0; i < data.length; i++) {
                $('select[name=cboCarName]').append("<option value='" + data[i].CarName + "'>" + data[i].CarName + "</option>");
            }
        }
    });
}

// 批量审核
var CheckData = function () {
    var rowsId = $ElemTable.jqGrid("getGridParam", "selarrrow");
    if (rowsId == '') {
        layer.msg('请至少选择一个项目。');
        return;
    }
    else {
        $('#WindowBatchAudit').modal('show');
        // 获取当前用户所属经销店
        $.post('/IServer/Dealer/Dealer.ashx?action=GetUserDealerByPKID', function (data) {
            var json = $.parseJSON(data);
            // $('input[name=DealerId]').val(json.DealerId);
            $('#Batch_FN_RebatesUser').val(json.DealerName);
            $('#Batch_FN_RebatesDate').val(new Date().Format('yyyy-MM-dd'));
        });
    }
}

// 保存数据
function batchAudit() {
    if ($('#Batch_cboAudit').val() == -1) {
        layer.msg('请选择审核意见。');
        return;
    }
    var rowsId = $ElemTable.jqGrid("getGridParam", "selarrrow");

    var submitDate = new Array();
    for (var i = 0; i < rowsId.length; i++) {
        var row = $ElemTable.jqGrid('getRowData', rowsId[i]);
        if (row.ToExamineState == 200) {
            submitDate.push(rowsId[i]);
        }
    }
    var SaveLoading = layer.msg('正在处理订单，请稍候', { icon: 16, time: false, shade: 0.8 });
    $.ajax({
        url: URL + 'BatchUpdateAudit',
        type: 'post',
        data: { orderId: submitDate.toString(), Batch_cboAudit: $('#Batch_cboAudit').val(), Batch_FN_RebatesRemark: $('#Batch_FN_RebatesRemark').val() },
        dataType: 'json',
        success: function (result) {
            if (result.success == true) {
                setTimeout(function () {
                    top.layer.close(SaveLoading);
                    layer.msg(result.msg);
                    $('#WindowBatchAudit').modal('hide');
                    window.location.reload();
                }, 2000);
                return false;
            }
            else {
                layer.msg(result.msg);
            }
        },
        errer: function () { }
    });
    return false;
}


var getSelectOrdersID = function () {
    var selectedData = new Array();
    var rowsId = $ElemTable.jqGrid("getGridParam", "selarrrow");
    var submitDate = new Array();
    for (var i = 0; i < rowsId.length; i++) {
        selectedData.push(rowsId[i]);
    }
    return selectedData;
}

// 拆单
function SplitData() {
    var rowIDs = getSelectOrdersID();
    if (rowIDs.length < 1) {
        layer.msg('请至少选择一个项目。');
        return;
    }
    else {
        layer.confirm('您确定要拆分选定的订单吗(不可恢复)?<br>', { icon: 3, title: '提示信息' }, function (index) {
            var index = layer.msg('正在处理，请稍候', { icon: 16, time: false, shade: 0.8 });
            setTimeout(function () {
                $.ajax({
                    url: URL + 'SplitOrders',
                    type: 'post',
                    async: false,
                    data: { orderId: rowIDs.toString() },
                    success: function (data) {
                        if (data) {
                            var index = parent.layer.getFrameIndex(window.name); //先得到当前iframe层的索引
                            $ElemTable.jqGrid({}).trigger('reloadGrid');  // 重新载入
                            layer.msg("拆分完成。");
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
}

// 作废订单
function CancelOrders() {
    var rowIDs = getSelectOrdersID();
    if (rowIDs.length < 1) {
        layer.msg('请至少选择一个项目。');
        return;
    }
    else {
        layer.confirm('确定要作废订单吗?', { icon: 3, title: '提示信息' }, function (index) {
            var index = layer.msg('正在处理，请稍候', { icon: 16, time: false, shade: 0.8 });
            setTimeout(function () {
                $.ajax({
                    url: URL + 'CancelOrders',
                    type: 'post',
                    async: false,
                    dataType: 'json',
                    data: { orderIds: rowIDs.toString() },
                    success: function (result) {
                        if (result.success == true) {
                            var index = parent.layer.getFrameIndex(window.name); //先得到当前iframe层的索引
                            $ElemTable.jqGrid({}).trigger('reloadGrid');  // 重新载入
                            layer.msg(result.msg);
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
}

// 导出
function getExcelData() {
    $('#btnExcel').attr('disabled', true);
    console.debug();
    var sendData = GetGridDataAll($ElemTable);
    $.post(URL + 'getExcelData', sendData, sendData, function (result) {

    },
    function (result) { layer.msg('操作失败，请刷新重新重试！') });
    layer.alert('正在生成下载文件，请稍等...');
    $('#btnExcel').attr('disabled', false);
}
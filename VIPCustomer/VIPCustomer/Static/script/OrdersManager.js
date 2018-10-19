// 初始化页面
$(function () {
    initPage();
    $.ajax({
        url: '/IServer/Cars/CarsManage.ashx?action=cboCarName',
        type: 'post',
        async: false,
        dataType: 'json',
        success: function (data) {
            for (var i = 0; i < data.length; i++) {
                $('#CarName').append("<option value='" + data[i].CarName + "'>" + data[i].CarName + "</option>");
            }
        }
    });
});

var URL = "/IServer/OrderManage/OrderManage.ashx?action=";

var showOrderType = function (cellvalue, options, rowObject) {
    switch (rowObject.OrderType) {
        case 1:
            return '订单';
        case 2:
            return '销售单';
        default:
            return rowObject.OrderType;
    }
}

var changeStatCN = function (cellvalue, options, rowObject) {
    switch (rowObject.ToExamineState) {
        case 0:
            return '<span style="color:red;">未提交</span>';
        case 100:
            return '等待大区经理审核';
        case -100:
            return '大区经理驳回';
        case 200:
            return '等待大客户室审核';
        case -200:
            return '大客户室驳回';
        case 300:
            return '大客户审过';
        case 310:
            return '配车队列';
        case 400:
            return '等待返款申请审核';
        case 410:
            return '返款通过';
        case 1000:
            return '全部完成';
        case -1000:
            return '作废订单';
        case 1100:
            return '返款驳回订单';
        default:
            return '配车返款';
    }
}

var initPage = function () {
    $('#table_list').jqGrid({
        url: URL + 'GetOrdersAllPager',
        mtype: 'post',
        datatype: 'json',
        viewrecords: true,  //  是否显示总记录数
        styleUI: 'Bootstrap',
        colModel: [
                { label: '主键ID', name: 'PKID', hidden: true, key: true, },
                { label: 'CustomerID', name: 'CustomerID', hidden: true },
                { label: 'ToExamineState', name: 'ToExamineState', hidden: true },
                { label: 'IsEdit', name: 'IsEdit', hidden: true },
                { label: '记录类别', name: '', width: 80, formatter: showOrderType },
                { label: '订单状态', name: '', width: 120, formatter: changeStatCN },
                { label: '状态值', name: 'ToExamineState', width: 80 },
                { label: '经销店', name: 'DealerName', width: 100 },
                { label: '记录编号', name: 'Code', width: 130 },
                { label: '客户名称', name: 'CustomerName', width: 160 },
                { label: '客户性质', name: '客户性质1名称', width: 120, },
                { label: '客户性质2', name: '客户性质2名称', width: 120, },
                { label: '购买方式', name: 'BuyWay', width: 120 },
                { label: '采购类型', name: 'PurchaseType', width: 120 },
                { label: '创建日期', name: 'CreateTime', width: 120, formatter: "date", formatoptions: { newformat: 'Y-m-d' } },
                { label: '生效日期', name: 'ReDate3', width: 120, formatter: "date", formatoptions: { newformat: 'Y-m-d' } },
                { label: '记录状态', name: 'OrderState', width: 120 },
                { label: '记录名称', name: 'RecordName', width: 120 },
                { label: '制表人', name: 'Watchmaker', width: 120 },
                {
                    label: '需要大客户资源', name: 'IsApplyMaxCustomerResources', align: 'center', width: 120, formatter: function (cellvalue, options, rowObject) {
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
                },
                { label: '客户建议', name: 'CustomerSuggestion', width: 120 },
                { label: '记录备注', name: 'BaseRemark', width: 120 },
                { label: 'FTMS回复人', name: 'Replyer3', width: 120 },
                { label: 'FTMS回复意见', name: 'ReComment3', width: 120 },
                { label: 'FTMS回复日期', name: 'ReDate3', width: 120, formatter: "date", formatoptions: { newformat: 'Y-m-d' } },
                { label: 'FTMS回复备注', name: 'ReRemark3', width: 120 }
        ],
        autowidth: true,
        height: 500,
        shrinkToFit: false,
        rownumbers: true,   // 显示行号
        rowNum: 10,
        rowList: [50, 100,200,500],
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
            url: URL + 'GetOrdersAllPager',
            mtype: 'post',
            postData: {
                OrdersearchContext: function () {
                    var type = $('select[name=type]').val();  // 订单类别
                    var CarName = $('#CarName').val(); // 车辆名称
                    var stateBegin = $('#stateBegin').val();
                    var stateEnd = $('#stateEnd').val();
                    var OrderNo = $('#OrderNo').val();
                    var carNo = $('#carNo').val();
                    var StringToJson = JSON.stringify({
                        OrderType: type, CarName: CarName, "ToExamineState": stateBegin, "ToExamineState": stateEnd,
                        Code: OrderNo, carNo: carNo
                    });
                    return StringToJson;
                }
            },
            page: 1
        }).trigger('reloadGrid');  // 重新载入
    });
}

// 编辑
showWindow = function () {
    var rowId = getRows("#table_list");
    if (rowId != null) {
        $('#myModal').modal('show');
    }
    else {
        layer.msg("请至少选择一个项目");
        $('#myModal').modal('hide');
        return;
    }
}

// 设置单据类别
var setOrdersType = function () {
    var rowIDs = getSelectOrdersID();
    if (rowIDs.length < 1) {
        return;
    }
    var SaveLoading = layer.msg('数据提交中，请稍候', { icon: 16, time: false, shade: 0.8 });
    $.ajax({
        type: 'post',
        url: URL + 'ChangeType',
        data: { "rowIDs": rowIDs, "cmboxOrderType": $('#cmboxOrderType').val() },
        dataType: 'json',
        success: function (res) {
            if (res.success) {
              layer.close(SaveLoading);
                layer.msg("操作完成。");
                $('#myModal').modal('hide');
                $('#table_list').jqGrid('setGridParam', {
                }).trigger('reloadGrid');  // 重新载入
            }
            else {
              layer.close(SaveLoading);
                layer.alert("操作失败。")
            }
        },
        error: function () {
            layer.alert("操作失败。");
        }
    })
}

// 设置订单状态
var setOrdersEditable = function () {
    var rowIDs = getSelectOrdersID();
    if (rowIDs.length < 0) {
        return;
    }
    var SaveLoading =layer.msg('数据提交中，请稍候', { icon: 16, time: false, shade: 0.8 });
    $.ajax({
        type: 'post',
        url: URL + 'SetOrdersStatus',
        data: { "rowIDs": rowIDs, "status": $('#cmboxStatus').val() },
        dataType: 'json',
        success: function (res) {
            if (res.success) {
               layer.close(SaveLoading);
                layer.msg("操作完成。");
                $('#myModal').modal('hide');
                $('#table_list').jqGrid('setGridParam', {
                }).trigger('reloadGrid');  // 重新载入
            }
            else {
              layer.close(SaveLoading);
                layer.alert("操作失败。")
            }
        },
        error: function () {
            layer.alert("操作失败。");
        }
    })
}

var DeleteData = function () {
    var rowIDs = getSelectOrdersID();
    if (rowIDs == null) {
        layer.msg('请至少选择一个项目');
        return;
    }
    else {
        layer.confirm('您确定要删除选定的订单吗（不可恢复）？', { icon: 3, title: '提示信息' }, function (index) {
            var index = layer.msg('正在处理，请稍候', { icon: 16, time: false, shade: 0.8 });
            setTimeout(function () {
                $.ajax({
                    url: URL + 'Delete',
                    type: 'post',
                    async: false,
                    dataType: 'json',
                    data: { orderId: rowIDs },
                    success: function (data) {
                        if (data.success) {
                            var index = layer.getFrameIndex(window.name); //先得到当前iframe层的索引
                            $('#table_list').jqGrid({}).trigger('reloadGrid');  // 重新载入
                            layer.msg("删除成功");
                            layer.close(index);
                        }
                    }, error: function (data) {
                       layer.close(index);
                    }
                });
                return false;
            }, 2000);
        });
    }
}

var SplitData = function () {
    var rowIDs = getSelectOrdersID();
    if (rowIDs == null) {
        layer.msg('请至少选择一个项目');
        return;
    }
    else {
        layer.confirm('您确定要拆分选定的订单吗（不可恢复）？', { icon: 3, title: '提示信息' }, function (index) {
            var index = layer.msg('正在处理，请稍候', { icon: 16, time: false, shade: 0.8 });
            setTimeout(function () {
                $.ajax({
                    url: URL + 'SplitOrders',
                    type: 'post',
                    async: false,
                    data: { orderId: rowIDs },
                    success: function (data) {
                        if (data) {
                            var index = layer.getFrameIndex(window.name); //先得到当前iframe层的索引
                            $('#table_list').jqGrid({}).trigger('reloadGrid');  // 重新载入
                            layer.msg("拆分完成。");
                            layer.close(index);
                        }
                    }, error: function (data) {
                      layer.close(index);
                    }
                });
                return false;
            }, 2000);
        });
    }
}

var Distribute = function (type) {
    var rowIDs = getSelectOrdersID();
    if (rowIDs == null) {
        layer.msg('请至少选择一个项目');
        return;
    }
    var msgType = (0 == type) ? "放弃" : "恢复";
    layer.confirm('您确定选定的订单要{0}继续配车吗？'.format(msgType), { icon: 3, title: '提示信息' }, function (index) {
        var index = layer.msg('正在处理，请稍候', { icon: 16, time: false, shade: 0.8 });
        setTimeout(function () {
            $.ajax({
                url: URL + 'DistributeNeed',
                type: 'post',
                async: false,
                data: { orderId: rowIDs, needType: type },
                success: function (data) {
                    var index = layer.getFrameIndex(window.name); //先得到当前iframe层的索引
                    $('#table_list').jqGrid({}).trigger('reloadGrid');  // 重新载入
                    layer.msg("{0}继续配车完成。".format(msgType));
                    layer.close(index);
                }, error: function (data) {
                   layer.close(index);
                }
            });
            return false;
        }, 2000);
    });

}

var FinishData = function () {
    var rowIDs = getSelectOrdersID();
    if (rowIDs == null) {
        layer.msg('请至少选择一个项目');
        return;
    }
    layer.confirm('您确定要结束选定的订单吗？', { icon: 3, title: '提示信息' }, function (index) {
        var index = layer.msg('正在处理，请稍候', { icon: 16, time: false, shade: 0.8 });
        setTimeout(function () {
            $.ajax({
                url: URL + 'FinishOrders',
                type: 'post',
                async: false,
                data: { orderId: rowIDs },
                success: function (data) {
                    var index = layer.getFrameIndex(window.name); //先得到当前iframe层的索引
                    $('#table_list').jqGrid({}).trigger('reloadGrid');  // 重新载入
                    layer.msg("订单结束完成。");
                    layer.close(index);
                }, error: function (data) {
                    layer.close(index);
                }
            });
            return false;
        }, 2000);
    });
}

var ShowOrdersDetail = function () {
    var rowId = getSelectOrdersID();
    if (rowId == null) {
        layer.msg('请至少选择一个项目！');
        return;
    }
    else {
        $('#myDetailWin').modal('show');
        GetInfo(rowId);
        GetgridPayCar(rowId);

    }
}

var tabClick = function (obj, rowId) {
    if (obj == 1)
    { GetgridPayCar(rowId); }
};

var vm = null;
var GetInfo = function (orderId) {
    // 用到深拷贝知识，判断如果vm实例已经创建就不在重复创建，否则创建vm实例
    if (!vm) {
        vm = new Vue({
            el: '#tab-1',
            data: {
                OrderList: {}
            }
        });
        $.ajax({
            url: URL + 'GetData',
            type: 'post',
            async: false,
            dataType: 'json',
            data: { orderId: orderId },
            success: function (data) {
                vm.OrderList = Object.assign({}, vm.OrderList, data);//深拷贝
            }, error: function (data) {
                layer.alert('获取数据失败。');
            }
        });
    }
    else {
        $.ajax({
            url: URL + 'GetData',
            type: 'post',
            async: false,
            dataType: 'json',
            data: { orderId: orderId },
            success: function (data) {
                vm.OrderList = Object.assign({}, vm.OrderList, data);
            }, error: function (data) {
                layer.alert('获取数据失败。');
            }
        });
    }
}

$("#myDetailWin").on('hidden.bs.modal', '.modal', function (e) {
    console.log($("#myDetailWin").html())
    $(this).removeData("bs.modal");
});

var GetgridPayCar = function (orderId) {
    $('#gridPanelPayCar').jqGrid({
        url: URL + 'GetPayCarDetail',
        mtype: 'post',
        styleUI: 'Bootstrap',
        datatype: 'json',
        postData: { orderId: orderId },
        colModel: [
            { label: 'PKID', name: 'PKID', key: true, hidden: true },
            { label: '车名', name: 'CarName', sortable: false, width: 100 },
            { label: '单车申请优惠幅度', name: 'Model', sortable: false },
            { label: '返款金额1', name: 'CarModel', sortable: false },
            { label: '返款人1', name: 'SFX', sortable: false },
            { label: '返款日期1', name: 'CarColorCode', formatter: "date", formatoptions: { newformat: 'Y-m-d' }, sortable: false },
            { label: '返款金额2', name: 'RequirementNumber', sortable: false },
            { label: '返款人2', name: 'WantSumbitCarDate', sortable: false },
            { label: '返款日期2', name: 'OldNo', formatter: "date", formatoptions: { newformat: 'Y-m-d' }, sortable: false },
            { label: 'SFX', name: 'RuckSack', sortable: false },
            { label: '颜色', name: 'WithNoCurtains', sortable: false },
            { label: '发票日期', name: 'NameplateSeats', formatter: "date", formatoptions: { newformat: 'Y-m-d' }, sortable: false },
            { label: '发动机号', name: 'TableChang', sortable: false },
            { label: '车架号', name: 'Other', sortable: false },
            { label: '车型', name: 'WantFTMSCarDateTime', sortable: false, formatter: "date", formatoptions: { newformat: 'Y-m-d' } },
            { label: '规格', name: 'SubmitDealerID', sortable: false },
            { label: '销售规格', name: 'SubmitDealerID', sortable: false },
            { label: '客户优惠幅度', name: 'SubmitDealerID', sortable: false },
            { label: '制表日期', name: 'SubmitDealerID', formatter: "date", formatoptions: { newformat: 'Y-m-d' }, sortable: false },
            { label: '备注', name: 'SubmitDealerID', sortable: false },
        ],
        autoScroll: true,
        height: 445,
        autowidth: true,
        shrinkToFit: false,
        sortable: false,
        rowNum: 'all', viewrecords: true,  //  是否显示总记录数
        jsonReader: {
            root: "dataList", repeatitems: false
        }
    })

}

var getSelectOrdersID = function () {
    var row = getRows("#table_list");
    if (row == null) {
        return;
    }
    return row;
}
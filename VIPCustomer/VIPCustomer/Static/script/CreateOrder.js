// 订单一致：制表人
var fixFields = new Array('Watchmaker');
// 初始化页面
$(function () {
    initPage();
});
var commitInto = "请确定已经填写完购车信息，客户联系人以及返款经销店联系人方便联系。<BR>您确定要提交选定的订单吗?";
var URL = " /IServer/OrderManage/OrderManage.ashx?action=";

var initPage = function () {
    $('#table_list').jqGrid({
        url: URL + 'GetOrderManagerPager',
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
                label: '流程', name: '', width: 60, formatter: function (cellvalue, options, rowObject) {
                    return "<a onclick='showOrdersFlow(" + rowObject.PKID + ",&quot;" + rowObject.Code + "&quot;)' title='查看'><i class='fa fa-file-o'></i></a>";
                }
            },
            {
                label: '订单状态', name: '', width: 120, formatter: function (cellvalue, options, rowObject) {
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
                        case 1000:
                            return '全部完成';
                        case -1000:
                            return '作废订单';
                        case 410:
                            return '返款初审通过';
                        default:
                            return '配车返款';
                    }
                }
            },
            { label: '生效日期', name: 'ReDate3', width: 120, formatter: "date", formatoptions: { newformat: 'Y-m-d' } },
            { label: '记录编号', name: 'Code', width: 130 },
            { label: '客户名称', name: 'CustomerName', width: 160 },
            { label: '购买方式', name: 'BuyWay', width: 120 },
            { label: '采购类型', name: 'PurchaseType', width: 120 },
            { label: '创建日期', name: 'CreateTime', width: 120, formatter: "date", formatoptions: { newformat: 'Y-m-d' } },
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
            { label: '记录状态', name: 'OrderState', width: 120 },
            { label: '记录名称', name: 'RecordName', width: 120 },
            { label: '经销店', name: 'DealerName', width: 120 },
            { label: '制表人', name: 'Watchmaker', width: 120 },
            { label: '客户建议', name: 'CustomerSuggestion', width: 120 },
            { label: '记录备注', name: 'BaseRemark', width: 120 },
            { label: 'FTMS回复人', name: 'FTMSBackAuditor', width: 120 },
            { label: 'FTMS回复意见', name: 'FTMSBackSuggestion', width: 120 },
            { label: 'FTMS回复日期', name: 'FTMSBackToExamineDate', width: 120, formatter: "date", formatoptions: { newformat: 'Y-m-d' } },
            { label: 'FTMS回复备注', name: 'FTMSBackRemark', width: 120 }
        ],
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

    // 查询
    $('#btnSearch').click(function () {
        $('#table_list').jqGrid('setGridParam', {
            url: URL + 'GetOrderManagerPager',
            mtype: 'post',
            postData: {
                OrdersearchContext: function () {
                    var code = $('input[name=Code]').val();  // 订单编号
                    var ToExamineState = $('select[name=cboOrderState]').val();
                    var StringToJson = JSON.stringify({ "Code": code, "ToExamineState": ToExamineState });
                    return StringToJson;
                }
            },
            page: 1
        }).trigger('reloadGrid');  // 重新载入
    });

    // 添加
    $(window).one("resize", function () {
        $("#btnAdd").click(function () {
            var index = layer.open({
                title: "添加",
                type: 2,
                skin: 'layui-layer-molv',
                content: '../../Page/MajorAccount/CreateOrderEdit.html',
                success: function (layero, index) {
                    setTimeout(function () {
                        layer.tips('点击此处返回订单列表', '.layui-layer-setwin .layui-layer-close', {
                            tips: 3
                        });
                    }, 500)
                }
            });
            layer.full(index);
            $('#opType').val("add");
        })
    }).resize();

    // 编辑
    $(window).one("resize", function () {
        $("#btnEdit").click(function () {
            var rowId = $("#table_list").jqGrid("getGridParam", "selrow");
            if (rowId != null) {
                var rowData = $("#table_list").jqGrid('getRowData', rowId);
                var customerId = rowData.CustomerID;
                var index = layer.open({
                    title: "编辑",
                    type: 2,
                    skin: 'layui-layer-molv',
                    content: '../../Page/MajorAccount/CreateOrderEdit.html?orderId=' + rowId + '&customerId=' + customerId,
                    success: function (layero, index) {
                        setTimeout(function () {
                            layer.tips('点击此处返回订单列表', '.layui-layer-setwin .layui-layer-close', {
                                tips: 3
                            });
                        }, 500)
                    }
                });
                layer.full(index);
                $('#opType').val("update");
            }
            else {
                layer.msg("请至少选择一个项目");
            }
        })
    }).resize();

    // 判断点击是添加还是修改按钮
    var opType = $('#opType', window.parent.document).val();
    if (opType == 'add') {
        AddDetail();
        $('#opType').val(opType);
    }
    else if (opType == 'update') {
        EditDetail();
        $('#opType').val(opType);
    };

    // 保存并提交
    $('#btnSaveCommit').on('click', function () {
        layer.confirm(commitInto, { icon: 3, title: '提示信息' }, function (index) {
            SaveAllData(true);
        })
        return false;
    });

    // 取消
    $("#cancle").click(function () {
        var index = parent.layer.getFrameIndex(window.name); //先得到当前iframe层的索引
        parent.layer.close(index); //再执行关闭
    });
}

// 选择客户
var SelCustomerAll = function () {
    layer.open({
        title: '<i class="fa fa-user"></i>&nbsp;选择客户',
        type: 2,
        scrollbar: false,   //浏览器滚动条已锁
        fix: true, //不固定
        maxmin: false,  //禁止最小化
        area: ['900px', '685px'],
        btn: ['选择'], btnAlign: 'c',
        shadeClose: false, //点击遮罩关闭
        content: '../CommonPage/CustomerList.html', //iframe的url，no代表不显示滚动条
        yes: function (index, layero) {
            $('input[name=hiddenCustomerId]').val(global.PKID)
            $('input[name=CustomerName]').val(global.Name);
            $('input[name=CustomerType]').val(global.CustomerNatureName);
            $('input[name=Code]').val(global.EnterpriseCode);
            $('input[name=Phone]').val(global.Phone);
            $('input[name=Address]').val(global.Address);
            $('input[name=Eamil]').val(global.Eamil);
            $('input[name=Fax]').val(global.Fax);
            layer.close(index); //关掉指定层
        },
    });
}

//// 新增客户
//var AddCustomer = function () {
//    layer.open({
//        title: "添加",
//        type: 2,
//        skin: 'layui-layer-molv',
//        area: ['1000px', '780px'],
//        content: '../../Page/MajorAccount/CustomerEdit.html',
//    });
//    $('#input_Div').hide();
//}

// 显示订单进度流程
let showOrdersFlow = function (ordersID, ordersNo) {
    let templateTitle = "订单流程查看 - 【{0}】";
    var resultTitle = templateTitle.format(ordersNo);
    $.ajax({
        url: URL + 'GetOrderProcessByOrderID',
        type: 'post',
        data: { orderId: ordersID },
        dataType: 'json',
        success: function (data) {
            let strHtml = "";
            strHtml += '<h3 style="text-align:center;">订单流程信息</h3>';
            strHtml += '<table id="Tab" class="table table-striped table-bordered">';
            strHtml += '<thead><tr class="active"><th width="90">操作名称</th><th>操作人</th><th>操作时间</th><th>下步操作候选人</th></tr></thead>';
            strHtml += '<tbody>';
            for (var i = 0; i < data.length; i++) {
                strHtml += '<tr><td>' + data[i].OperationName + '</td>';
                strHtml += '<td>' + data[i].Operator + '</td>';
                strHtml += '<td>' + new Date(data[i].OperationDate).Format("yyyy-MM-dd") + '</td>';
                strHtml += '<td>' + data[i].NextOperator + '</td></tr>';
            }
            strHtml += '</tbody>'
            strHtml += '</table>';
            setTimeout(function () {
                layer.open({
                    title: resultTitle,
                    type: 1,
                    skin: 'layui-layer-molv', //样式类名
                    area: ['600px', '400px'],
                    btn: ['关闭'],
                    btnAlign: 'c',
                    content: strHtml,
                    end: function (layero, index) {
                        layer.close(index); //关掉指定层
                    }
                });
            }, 100);
        }
    });

}

var AddDetail = function () {
    $.ajax({
        type: 'post',
        url: URL + 'GetOrderNoandDate',
        data: {},
        dataType: 'json',
        success: function (result) {
            if (result) {
                $('input[name=OrderNo]').val(result.OrderNo);
                $('input[name=CreateTime]').val(result.DateNow);
            }
            else {
                alert("获取订单编号及日期信息失败");
            }
        }
    });

    // 购买方式下拉列表
    $.ajax({
        url: URL + 'GetBuyWay',
        type: 'post',
        async: false,
        dataType: 'json',
        success: function (data) {
            for (var i = 0; i < data.length; i++) {
                $('select[name=BuyWay]').append("<option value='" + data[i].ListName + "'>" + data[i].ListName + "</option>");
            }
        }
    });

    // 采购类型下拉列表
    $.ajax({
        url: URL + 'GetPurchaseType',
        type: 'post',
        async: false,
        dataType: 'json',
        success: function (data) {
            for (var i = 0; i < data.length; i++) {
                $('select[name=PurchaseType]').append("<option value='" + data[i].ListName + "'>" + data[i].ListName + "</option>");
            }
        }
    });

    // 记录状态下拉列表
    $.ajax({
        url: URL + 'GetOrderState',
        type: 'post',
        async: false,
        dataType: 'json',
        success: function (data) {
            for (var i = 0; i < data.length; i++) {
                $('select[name=OrderState]').append("<option value='" + data[i].ListName + "'>" + data[i].ListName + "</option>");
            }
        }
    });

    // 记录名称下拉列表
    $.ajax({
        url: URL + 'GetRecordName',
        type: 'post',
        async: false,
        dataType: 'json',
        success: function (data) {
            for (var i = 0; i < data.length; i++) {
                $('select[name=RecordName]').append("<option value='" + data[i].ListName + "'>" + data[i].ListName + "</option>");
            }
        }
    });
    addSetData();
}

var addSetData = function () {
    $('select[name=OrderState]').val("受注");
    $.post(URL + 'GetWatchmaker', function (res) {
        var obj = JSON.parse(res); //由JSON字符串转换为JSON对象
        $('input[name=Watchmaker]').val(obj.strWatchmacker);
        $('input[name=hiddenDealerId]').val(obj.dealerId);
    });
    getOrderState();
}

//订单编辑状态
var orderState = {
    orderID: '',
    editState: '1'
}

// 获取当前订单编辑状态
var getOrderState = function (orderId, IsEdit, OrderState, ToExamineState) {
    var opType = $('#opType', window.parent.document).val();
    if (opType == 'update') {
        orderState.orderID = orderId;
    }
    orderState.editState = '1';
    if (('协商' == OrderState) && (-1000 != ToExamineState)) {
        orderState.editState = '1';
    }
    if (IsEdit == 0) {
        orderState.editState = '0';
    }
    if (IsEdit == 1) {
        orderState.editState = '1';
    }
    var valid = '1' != orderState.editState;
    if (!isNaN(valid) && valid == true) {
        $("#btnSave").attr({ "disabled": "disabled" });
        $("#btnSaveCommit").attr({ "disabled": "disabled" });
        $("#btnAddCar").attr({ "disabled": "disabled" });
        $("#btnDelCar").attr({ "disabled": "disabled" });
    }
    else {
        $("#btnSave").removeAttr("disabled")
        $("#btnSaveCommit").removeAttr("disabled")
        $("#btnAddCar").removeAttr("disabled")
        $("#btnDelCar").removeAttr("disabled")
    }
}

// 编辑信息
var EditDetail = function () {
    var orderId = getUrlParam('orderId');
    var valid = '';
    if (orderId == null)
        return;
    $.ajax({
        type: 'post',
        url: URL + 'GetOrderByPKID',
        data: { orderId: orderId },
        dataType: 'json',
        success: function (result) {
            if (result) {
                $('input[name=OrderNo]').val(result.Code);
                $('input[name=CreateTime]').val(result.CreateTime.slice(0, 10));// 格式化日期
                $('select[name=BuyWay]').append("<option value=" + result.BuyWay + ">" + result.BuyWay + "</option>");
                $('select[name=OrderState]').append("<option value=" + result.OrderState + ">" + result.OrderState + "</option>");
                $('select[name=PurchaseType]').append("<option value=" + result.PurchaseType + ">" + result.PurchaseType + "</option>");
                $('select[name=CarUse]').append("<option value=" + result.CarUse + ">" + result.CarUse + "</option>");
                $('select[name=RecordName]').append("<option value=" + result.RecordName + ">" + result.RecordName + "</option>");
                $('input[name=Watchmaker]').val(result.Watchmaker);
                $('input[name=Replyer3]').val(result.Replyer3);
                $('input[name=ReComment3]').val(result.ReComment3);
                result.InvoiceAndCustomerAtypism == 1 ? $("[name = InvoiceDiffer]:checkbox").attr("checked", true) : $("[name = InvoiceDiffer]:checkbox").attr("checked", false);
                result.OffAddressOnCardReport == 1 ? $("[name = OffAddressOnCardReport]:checkbox").attr("checked", true) : $("[name = OffAddressOnCardReport]:checkbox").attr("checked", false);
                result.DifferentPlace == 1 ? $("[name = DifferentPlace]:checkbox").attr("checked", true) : $("[name = DifferentPlace]:checkbox").attr("checked", false);
                $('input[name=OffAddressOnCardReport]').val(result.OffAddressOnCardReport);
                $('input[name=ReDate3]').val(result.ReDate3);
                $('input[name=ReRemark3]').val(result.ReRemark3);
                $('input[name=hiddenCustomerId]').val(result.CustomerID);

                $('#hiddenDealerId').val(result.DealerID);
                getOrderState(result.PKID, result.IsEdit, result.OrderState, result.ToExamineState);
            }
            else {
                alert("获取订单编号及日期信息失败");
            }
        }
    });

    var customerId = getUrlParam('customerId');
    if (customerId == null) return;
    $.ajax({
        type: 'post',
        url: '/IServer/Customer/CusManage.ashx',
        data: { customerId: customerId, action: 'GetCustomerByPKID' },
        dataType: 'json',
        success: function (result) {
            if (result) {
                $('input[name=CustomerName]').val(result.Name);
                $('input[name=CustomerType]').val(result.CustomerNatureName);
                $('input[name=Code]').val(result.EnterpriseCode);
                $('input[name=Phone]').val(result.Phone);
                $('input[name=Address]').val(result.Address);
                $('input[name=Eamil]').val(result.Eamil);
                $('input[name=Fax]').val(result.Fax);
            }
            else {
                alert("获取客户基本信息失败。");
            }
        }
    });
}

// 保存
let SaveAllData = function (isConfirmData) {
    let opType = $('#opType', window.parent.document).val();
    var rowId = $("#table_list", window.parent.document).jqGrid("getGridParam", "selrow");
    var rowData = $("#table_list", window.parent.document).jqGrid('getRowData', rowId);
    if (opType == 'update') {
        if (('协商' != rowData.OrderState) && (1 != '协商' != rowData.IsEdit)) {
            layer.msg("订单目前不可编辑");
            return;
        }
    }

    if ($('#hiddenCustomerId').val() == "") {
        layer.msg("请选择一个客户。");
        return;
    }
    var param = {
        BuyWay: $('select[name=BuyWay]').val(), CreateTime: $('input[name=CreateTime]').val(), OrderNo: $('input[name=OrderNo]').val(), PurchaseType: $('select[name=PurchaseType]').val(),
        OrderState: $('select[name=OrderState]').val(), CarUse: $('select[name=CarUse]').val(), OffAddressOnCardReport: $('input[name=OffAddressOnCardReport]').val(),
        InvoiceDiffer: $('input[name=InvoiceDiffer]').val(), IsApplyMaxCustomerResources: $('input[name=IsApplyMaxCustomerResources]').val()
    };

    if (typeof (param) != "object") {
        return;
    }
    else {
        if (param.BuyWay < 0 || param.CreateTime.trim() == "" || param.OrderNo.trim() == "" || param.PurchaseType < 0 ||
            param.OrderState < 0 || param.CarUse < 0) {
            layer.msg("请填写完整信息。");
            return;
        }
    }
    var needCheckCarData = false;
    if (rowData.OrderState != '协商') {
        needCheckCarData = true;
    }

    // 订车数据
    var arrCar = new Array();
    var record = $('#CarDemandTable').jqGrid('getRowData');
    for (var i = 0; record != null && i < record.length; i++) {
        if (needCheckCarData) {
            if (!checkCarData(record[i])) return;
            arrCar.push(record[i]);
        }
    }
    $('#HidOrderID').val(getUrlParam('orderId'));
    var sendData = $('#commentForm').serializeObject();
    SaveData(isConfirmData, sendData, arrCar);
}

// 保存订单
var SaveData = function (isConfirmData, sendData, arrCar) {
    var SaveLoading = layer.msg('正在处理订单，请稍候', { icon: 16, time: false, shade: 0.8 });
    $.ajax({
        type: 'post',
        url: URL + 'SaveData',
        data: { isConfirmData: isConfirmData, info: sendData, arrCar: JSON.stringify(arrCar) },
        dataType: 'json',
        success: function (result) {
            var index = parent.layer.getFrameIndex(window.name);
            if (result.state == 'true') {
                setTimeout(function () {
                    layer.close(SaveLoading);
                    layer.msg("保存成功。");
                    parent.layer.close(index); //再执行关闭
                    parent.location.reload();
                }, 2000);
                return false;
            }
        },
        error: function (data) {
            layer.msg("程序在执行过程出现错误。");
        }
    });
    return false;
}

var checkCarData = function (data) {
    if ("COASTER" == data.CarName.trim().toUpperCase()) {
        if ((data.OldNo == "") || (data.RuckSack == "")) {
            layer.alert("非协商单，COASTER车型必须填写传真号以及原始订单号（TACT协议号）。");
            return false;
        }
    }
    return true;
}

// 切换选项卡
var tabClick = function (obj) {
    switch (obj) {
        case 1:
            showCarDemand();
            break;
        case 2:
            getDC();
            break;
        case 3:
            getCC();
            break;
    }
}

// 车辆需求列表
var showCarDemand = function () {
    var orderId = getUrlParam('orderId');
    var carURL = URL + 'showCarDemand';
    if (orderId == null) {
        $('#CarDemandTable').jqGrid({
            caption: "车辆需求列表",
            url: carURL,
            mtype: 'post',
            styleUI: 'Bootstrap',
            datatype: 'json',
            postData: { orderId: orderId },
            colModel: [
                { label: 'PKID', name: 'PKID', key: true, hidden: true },
                { label: '车辆颜色ID', name: 'CarColorID', hidden: true },
                { label: '车辆ID', name: 'CarID', hidden: true },
                { label: '车辆用途', name: 'CarUsing', hidden: true },
                { label: '使用者', name: 'Users', hidden: true },
                { label: '备注', name: 'Remake', hidden: true },
                { label: '座位变更', name: 'TableChang', hidden: true },
                { label: '车名', name: 'CarName', sortable: false },
                { label: '车型', name: 'Model', sortable: false },
                { label: 'E/G', name: 'CarModel', sortable: false },
                { label: 'SFX', name: 'SFX', sortable: false },
                { label: '颜色', name: 'CarColorCode', sortable: false },
                { label: '数量', name: 'RequirementNumber', sortable: false },
                { label: '希望交车日期', name: 'WantSumbitCarDate', formatter: "date", formatoptions: { newformat: 'Y-m-d' }, sortable: false },
                { label: '成都分室传真订单编号', name: 'OldNo', sortable: false },
                { label: 'TACT订单协议号', name: 'RuckSack', sortable: false },
                { label: 'TA行李架', name: 'WithNoCurtains', sortable: false },
                { label: '窗帘加否', name: 'NameplateSeats', sortable: false },
                { label: '铭牌座位数', name: 'TableChang', sortable: false },
                { label: '其他要求', name: 'Other', sortable: false },
                { label: '期望FTMS发车时间', name: 'WantFTMSCarDateTime', sortable: false, formatter: "date", formatoptions: { newformat: 'Y-m-d' } },
                { label: '交车经销店', name: 'SubmitDealerID', sortable: false },
            ],
            height: 500,
            width: window.screen.availWidth - 240,
            autowidth: false,
            shrinkToFit: false,
            sortable: false,
            rowNum: 'all',
            viewrecords: true,  //  是否显示总记录数
            jsonReader: {
                root: "dataList", repeatitems: false
            }
        })
    }
    else {
        $('#CarDemandTable').jqGrid({
            caption: "车辆需求列表",
            url: carURL,
            mtype: 'post',
            styleUI: 'Bootstrap',
            datatype: 'json',
            postData: { orderId: orderId },
            colModel: [
             { label: 'PKID', name: 'PKID', key: true, hidden: true },
             { label: '车辆颜色ID', name: 'CarColorID', hidden: true },
             { label: '车辆ID', name: 'CarID', hidden: true },
             { label: '车辆用途', name: 'CarUsing', hidden: true },
             { label: '使用者', name: 'Users', hidden: true },
             { label: '备注', name: 'Remake', hidden: true },
             { label: '座位变更', name: 'TableChang', hidden: true },
             { label: '车名', name: 'CarName', sortable: false },
             { label: '车型', name: 'Model', sortable: false },
             { label: 'E/G', name: 'CarModel', sortable: false },
             { label: 'SFX', name: 'SFX', sortable: false },
             { label: '颜色', name: 'CarColorCode', sortable: false },
             { label: '数量', name: 'RequirementNumber', sortable: false },
             { label: '希望交车日期', name: 'WantSumbitCarDate', formatter: "date", formatoptions: { newformat: 'Y-m-d' }, sortable: false },
             { label: '成都分室传真订单编号', name: 'OldNo', sortable: false },
             { label: 'TACT订单协议号', name: 'RuckSack', sortable: false },
             { label: 'TA行李架', name: 'WithNoCurtains', sortable: false },
             { label: '窗帘加否', name: 'NameplateSeats', sortable: false },
             { label: '铭牌座位数', name: 'TableChang', sortable: false },
             { label: '其他要求', name: 'Other', sortable: false },
             { label: '期望FTMS发车时间', name: 'WantFTMSCarDateTime', formatter: "date", formatoptions: { newformat: 'Y-m-d' }, sortable: false },
             { label: '交车经销店', name: 'SubmitDealerID', sortable: false },
            ],
            height: 500,
            width: window.screen.availWidth - 240,
            autowidth: false,
            shrinkToFit: false,
            rowNum: 'all',
            viewrecords: true,  //  是否显示总记录数
            jsonReader: {
                root: "dataList", repeatitems: false
            }
        })
    }
}

// 经销店联系人列表
var getDC = function () {
    if ($('#hiddenDealerId').val() != null) {
        $('#dealerContactTable').jqGrid({
            caption: "经销店联系人列表",
            url: URL + 'getDC',
            mtype: 'post',
            styleUI: 'Bootstrap',
            datatype: 'json',
            postData: { DealerId: $('#hiddenDealerId').val(), orderId: orderState.orderID, IsEdit: orderState.editState },
            colModel: [
                { label: '主键', name: 'PKID', key: true, hidden: true },
                { label: '姓名', name: 'Name' },
                { label: '职务', name: 'JobName' },
                { label: '电话', name: 'Phone' },
                { label: '移动电话', name: 'MobileTel' },
                { label: '传真', name: 'Fax' },
                { label: '电子信箱', name: 'Email' },
                { label: '其他联系方式', name: 'OtherContactInfo' },
            ],
            height: 600,
            width: window.screen.availWidth - 240,
            autowidth: false,
            shrinkToFit: false,
            multiselect: true,
            rowNum: 'all',
            viewrecords: true,  //  是否显示总记录数
            jsonReader: {
                root: "dataList", repeatitems: false
            }
        })
    }
}

// 客户联系人列表
var getCC = function (table) {
    var customerId = $('#hiddenCustomerId').val();
    if (customerId == null) {
        $('#CContactTable').jqGrid({
            caption: "客户联系人列表",
            url: URL + 'getCC',
            mtype: 'post',
            styleUI: 'Bootstrap',
            datatype: 'json',
            postData: { customerId: customerId },
            colModel: [
                { label: '主键', name: 'PKID', key: true, width: 60, hidden: true },
                { label: '姓名', name: 'Name' },
                { label: '职务', name: 'Job' },
                { label: '电话', name: 'Phone' },
                { label: '移动电话', name: 'MobileTel' },
                { label: '传真', name: 'Fax' },
                { label: '电子信箱', name: 'Email' },
                { label: '其他联系方式', name: 'OtherContactInfo' },
                { label: '生日', name: 'Birthday', formatter: "date", formatoptions: { newformat: 'Y-m-d' } },
                { label: '兴趣爱好', name: 'Hobbies' }
            ],
            height: 600,
            width: window.screen.availWidth - 240,
            autowidth: false,
            shrinkToFit: false,
            multiselect: true,
            rowNum: 'all',
            viewrecords: true,  //  是否显示总记录数
            jsonReader: {
                root: "dataList", repeatitems: false
            }
        })
    }
    else {
        $('#CContactTable').jqGrid({
            caption: "客户联系人列表",
            url: URL + 'getCC',
            mtype: 'post',
            styleUI: 'Bootstrap',
            datatype: 'json',
            postData: { customerId: customerId, orderId: orderState.orderID, IsEdit: orderState.editState },
            colModel: [
               { label: '主键', name: 'PKID', key: true, width: 60, hidden: true },
                { label: '姓名', name: 'Name' },
                { label: '职务', name: 'Job' },
                { label: '电话', name: 'Phone' },
                { label: '移动电话', name: 'MobileTel' },
                { label: '传真', name: 'Fax' },
                { label: '电子信箱', name: 'Email' },
                { label: '其他联系方式', name: 'OtherContactInfo' },
                { label: '生日', name: 'Birthday', formatter: "date", formatoptions: { newformat: 'Y-m-d' } },
                { label: '兴趣爱好', name: 'Hobbies' }
            ],
            height: 600,
            width: window.screen.availWidth - 240,
            autowidth: false,
            shrinkToFit: false,
            multiselect: true,
            rowNum: 'all',
            viewrecords: true,  //  是否显示总记录数
            jsonReader: {
                root: "dataList", repeatitems: false
            }
        })
    }
}

// 作废订单
var DeleteData = function () {
    var rowId = $("#table_list").jqGrid("getGridParam", "selrow");
    if (rowId != null) {
        layer.confirm('您确定要选定作废的订单吗？<br>（没有编辑权限的订单不会被作废。）', { icon: 3, title: '提示信息' }, function (index) {
            var index = layer.msg('正在处理作废订单，请稍候', { icon: 16, time: false, shade: 0.8 });
            setTimeout(function () {
                $.ajax({
                    url: URL + 'DeleteOrders',
                    type: 'post',
                    async: false,
                    data: { orderId: rowId },
                    success: function (data) {
                        var index =layer.getFrameIndex(window.name); //先得到当前iframe层的索引
                        $('#table_list').jqGrid({}).trigger('reloadGrid');  // 重新载入
                        layer.close(index);
                        layer.msg("操作完成<br>（没有编辑权限的订单不会被作废。）。");
                    }, error: function (data) {
                       layer.close(index);
                    }
                });
                return false;
            }, 2000);
        });
    }
    else {

        layer.msg("请至少选择一个项目");
    }
}

// 提交订单
var SubmitData = function () {
    var rowId = $("#table_list").jqGrid("getGridParam", "selrow");
    if (rowId != null) {
        var submitData = "";
        var customerData = "";
        var rowData = $("#table_list").jqGrid('getRowData', rowId);
        layer.confirm(commitInto, { icon: 3, title: '提示信息' }, function (index) {
            if ('1' == rowData.IsEdit) {
                submitData = rowData.PKID;
                customerData = rowData.CustomerID;
            }
            var index = layer.msg('正在处理订单，请稍候', { icon: 16, time: false, shade: 0.8 });
            setTimeout(function () {
                $.ajax({
                    type: 'post',
                    url: URL + 'SubmitOrders',
                    data: { submitData: submitData, customerData: customerData },
                    dataType: 'json',
                    success: function (result) {
                        if (result.success) {
                            var index = parent.layer.getFrameIndex(window.name);
                            $('#table_list').jqGrid({}).trigger('reloadGrid');  // 重新载入
                            layer.close(index);
                            layer.msg('提交成功!', { icon: 6, time: 2000 });
                        }
                        else {
                            layer.msg(result.msg);
                        }
                    },
                    error: function (errMsg) {
                       layer.close(index);
                    }
                })
                return false;
            }, 2000);
        });
    }
    else {
        layer.msg("请至少选择一个项目");
    }
}

// 打印需求确认单
var ConfirmPrint = function (reportType, ordersID) {
    var rowId = $("#table_list").jqGrid("getGridParam", "selrow");
    if (rowId != null) {
        var rowData = $("#table_list").jqGrid('getRowData', rowId);
        if (rowData.ToExamineState == 0) {
            layer.msg("订单未提交，不能打印需求确认单。");
            return;
        }
        ordersID = rowId;
        if (reportType == 'confirm') {
            openWindow();
        }
        else { }
    }
    else {
        layer.msg("请至少选择一个项目");
    }
}

var opCar = 0;

// 添加车辆
var AddCarDetail = function () {
    $('#windows1').modal('show');
    opCar = 'carAdd';
    $('.modal-title1').text('添加');
}

// 编辑车辆
var EditCarDetail = function () {
    var rowId = $("#CarDemandTable").jqGrid("getGridParam", "selrow");
    if (rowId != null) {
        opCar = 'carUpdate';
        var rowData = $("#CarDemandTable").jqGrid('getRowData', rowId);
        $('input[name=carName]').val(rowData.CarName);
        $('select[name=color]').val(rowData.CarID);
        addCarColor(rowData.CarID);
        $('input[name=carModel]').val(rowData.CarModel);
        $('input[name=EG]').val(rowData.Model);
        $('input[name=SFX]').val(rowData.SFX);
        $('input[name=Number]').val(rowData.RequirementNumber);
        $('input[name=WantSubmitDate]').val(rowData.WantSubmitDate);
        $('input[name=carYongTu]').val(rowData.CarUsing);
        $('input[name=OtherXuQiu]').val(rowData.OtheRequirements);
        $('input[name=Uses]').val(rowData.Users);
        $('input[name=CatchCarDealerName]').val(rowData.SubmitDealerID);
        $('textarea[name=Remark]').val(rowData.Remark);
        $('input[name=FaxOrdersNo]').val(rowData.OldNo);
        $('textarea[name=TactNo]').val(rowData.RuckSack);
        $('input[name=FN_Rack]').val(rowData.WithNoCurtains);
        $('input[name=FN_HasCurtain]').val(rowData.NameplateSeats);
        $('input[name=FN_SeatNumber]').val(rowData.NameplateSeats);
        $('input[name=FN_FTMSSendDate]').val(rowData.WantSumbitCarDate);
        $('textarea[name=FN_ChangeChair]').val(rowData.TableChang);
        $('textarea[name=FN_OtherRequire]').val(rowData.Other);
        $('#windows1').modal('show');
    }
    else {
        layer.msg("请至少选择一个项目");
    }
}

// 删除车辆
var DelCarDetail = function () {
    var rowId = $("#CarDemandTable").jqGrid("getGridParam", "selrow");
    if (rowId != null) {
        layer.confirm('您确定要删除选定的数据吗？', { icon: 3, title: '提示信息' }, function (index) {
            var index = layer.msg('正在删除，请稍候', { icon: 16, time: false, shade: 0.8 });
            $("#CarDemandTable").jqGrid('delRowData', rowId);
            layer.close(index);
            layer.msg("删除成功");
        })
    }
    else {
        layer.msg("请至少选择一个项目");
    }
}

let addSortNo = 0;
var carID = null;

// 选择车辆
var SelectCarShow = function () {
    layer.open({
        title: '<i class="fa fa-truck"></i>&nbsp;选择车辆',
        type: 2,
        scrollbar: false,   //浏览器滚动条已锁
        fix: true, //不固定
        maxmin: false,  //禁止最小化
        area: ['700px', '550px'],
        btn: ['选择'], btnAlign: 'c',
        shadeClose: false, //点击遮罩关闭
        content: '../CommonPage/CarList.html', //iframe的url，no代表不显示滚动条
        yes: function (index, layero) {
            $('input[name=carName]').val(global.Name)
            $('input[name=SFX]').val(global.SFX);
            $('input[name=carModel]').val(global.Model);
            $('input[name=EG]').val(global.CarModel);
            addCarColor(global.PKID);
            carID = global.PKID;
            layer.close(index); //关掉指定层
        },
    });
}

//填充车辆颜色下拉框
var addCarColor = function (carId) {
    if (carId > 0) {
        $.ajax({
            type: 'post',
            url: URL + 'GetCarColorByCarId',
            data: { carId: carId },
            dataType: 'json',
            success: function (data) {
                for (var i = 0; i < data.length; i++) {
                    $("select[name=color]").append("<option value='" + data[i].PKID + "'>" + data[i].Code + "</option>");
                }
            },
        })
    }
}

// 选择经销店
var PayCarDealerSelect = function (index) {
    if (index == 0) {
        $('input[name=CatchCarDealerName]').val("");
    }
    else {
        layer.open({
            title: '<i class="fa fa-user"></i>&nbsp;选择经销店',
            type: 2,
            scrollbar: false,   //浏览器滚动条已锁
            fix: true, //不固定
            maxmin: false,  //禁止最小化
            area: ['900px', '685px'],
            btn: ['选择'], btnAlign: 'c',
            shadeClose: false, //点击遮罩关闭
            content: '../CommonPage/DealerList.html', //iframe的url，no代表不显示滚动条
            yes: function (index, layero) {
                $('input[name=CatchCarDealerName]').val(global.Name);
                layer.close(index); //关掉指定层
            },
        });
    }
}

// 保存车辆需求信息
var SaveCarDetail = function () {
    var $from = $("#CarDemandForm");
    var flag;
    // 非空验证
    if (('协商' != $('input[select=OrderState]').find("option:selected").text(), window.parent.document) && 'COASTER' == $('input[name=carName]').val().toUpperCase()) {
        flag = $from.validate({
            rules: {
                "FaxOrdersNo": {
                    required: true
                },
                "TactNo": {
                    required: true
                }
            }
        });
    }
    else {
        flag = $from.validate({
            rules: {
                "FaxOrdersNo": {
                    required: false
                },
                "TactNo": {
                    required: false
                }
            }
        })
    }
    if ('COASTER' == $('input[name=carName]').val().toUpperCase() && ('T' == $('input[name=SFX]').val().trim().toUpperCase().substring(0, 1))) {
        flag = $from.validate({
            rules: {
                "FN_Rack": {
                    required: true
                },
                "FN_HasCurtain": {
                    required: true
                },
                "FN_SeatNumber": {
                    required: true
                },
                "FN_FTMSSendDate": {
                    required: true
                }
            }
        });
    }
    else {
        flag = $from.validate({
            rules: {
                "FN_Rack": {
                    required: false
                },
                "FN_HasCurtain": {
                    required: false
                },
                "FN_SeatNumber": {
                    required: false
                },
                "FN_FTMSSendDate": {
                    required: false
                }
            }
        });
    }
    flag = $from.valid();
    if (!flag) {
        return;
    }
    var rowId = $("#CarDemandTable").jqGrid("getGridParam", "selrow");
    var rowData = $("#CarDemandTable").jqGrid('getRowData', rowId);
    var recordParts = {
        PKID: 0,
        CarColorID: $('select[name=color]').val(),
        CarID: carID,
        CarUsing: $('input[name=carYongTu]').val(),
        Users: $('input[name=Uses]').val(),
        CarName: $('input[name=carName]').val(),
        CarModel: $('input[name=carModel]').val(),
        Model: $('input[name=EG]').val(),
        SFX: $('input[name=SFX]').val(),
        CarColorCode: $("select[name=color]").find("option:selected").text(),
        RequirementNumber: $('input[name=Number]').val(),
        WantSumbitCarDate: $('input[name=WantSubmitDate]').val(),
        OldNo: $('input[name=FaxOrdersNo]').val(),
        RuckSack: $('textarea[name=TactNo]').val(),
        WithNoCurtains: $('input[name=FN_Rack]').val(),
        NameplateSeats: $('input[name=FN_HasCurtain]').val(),
        TableChang: $('textarea[name=FN_ChangeChair]').val(),
        Other: $('textarea[name=FN_OtherRequire]').val(),
        WantFTMSCarDateTime: $('input[name=FN_FTMSSendDate]').val(),
        SubmitDealerID: $('input[name=CatchCarDealerName]').val(),
    };
    if (opCar == 'carAdd') {
        addSortNo++;
        $("#CarDemandTable").jqGrid("addRowData", addSortNo, recordParts, "last");
    }
    else if (opCar == 'carUpdate') {
        if (rowId == 0) {
            $("#CarDemandTable").jqGrid("setRowData", 0, recordParts, "after", rowId);
        }
        else {
            $("#CarDemandTable").jqGrid('setRowData', rowData.PKID, recordParts);
        }
    };
    layer.msg('保存成功。', { icon: 6, time: 2000 });
    //flag.validate().resetForm();
    $('#windows1').modal('hide');
}


var btnUpload = function () {
    $('#uploadWindow').modal('show');
}

// 上传附件
var uploadFileAttachment = function (obj) {
    var projectfileoptions = {
        showUpload: false,
        showRemove: false,
        language: 'zh',
        allowedPreviewTypes: ['image'],
        allowedFileExtensions: ['jpg', 'png', 'gif'],
        maxFileSize: 2000,
    };
    // 文件上传框
    $('input[class=projectfile]').each(function () {
        var imageurl = $(this).attr("value");

        if (imageurl) {
            var op = $.extend({
                initialPreview: [ // 预览图片的设置
                "<img src='" + imageurl + "' class='file-preview-image'>", ]
            }, projectfileoptions);

            $(this).fileinput(op);
        } else {
            $(this).fileinput(projectfileoptions);
        }
    });
}

// 查看附件
let viewFileAttachment = function (obj) {
    alert(obj);
}

function setCheckStatA() {
    if ($('input[name=OffAddressOnCardReport]').is(':checked')) {
        $('input[name=OffAddressOnCardReport]').val("1");
        $('input[name=InvoiceDiffer]').removeAttr('disabled');
    }
    else {
        $('input[name=InvoiceDiffer]').attr('disabled', true);
        $('input[name=OffAddressOnCardReport]').val("0");
    }
}

function setCheckStatB() {
    if ($('input[name=InvoiceDiffer]').is(':checked')) {
        $('input[name=InvoiceDiffer]').val("1");
        $('input[name=FN_InvoiceName]').removeAttr('readonly');
    }
    else {
        $('input[name=InvoiceDiffer]').val("0");
        $('input[name=FN_InvoiceName]').attr('readonly', true);
    }
}

function setCheckStatC() {
    if ($('input[name=DifferentPlace]').is(':checked')) {
        $('input[name=DifferentPlace]').val("1");
    }
    else {
        $('input[name=DifferentPlace]').val("0");
    }
}

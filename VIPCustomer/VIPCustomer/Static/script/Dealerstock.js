$(function () {
    InitPage();
});

// 初始化页面
function InitPage() {
    var URL = '/IServer/Dealer/Dealer.ashx?action=GetDealerStockPagerList';
    $('#table_list').jqGrid({
        url: URL,
        mtype: 'post',
        datatype: 'json',
        viewrecords: true,  //  是否显示总记录数
        styleUI: 'Bootstrap',
        colModel: [
              { label: '主键ID', name: 'PKID', hidden: true, key: true, },
              { label: '经销店ID', name: 'DealerID', hidden: true },
              { label: '客户ID', name: 'CustomerID', hidden: true },
              { label: '企业代码', name: 'EnterpriseCode', width: 120 },
              { label: '客户名称', name: 'Name', width: 240 },
              { label: '车辆名称', name: 'CarName', width: 120 },
              { label: '库存月份', name: 'StockMonth', width: 120, formatter: "date", formatoptions: { newformat: 'Y-m' } },
              { label: '库存数量', name: 'StockCount', width: 120 },
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

    // 获取月份
    var date = new Date;
    var year = date.getFullYear();
    var params = [];
    for (var i = year; i > year - 5; i--) {
        params.push({ "Year": i });
    }
    for (var i = 0; i < params.length; i++) {
        $('select[name=cbYear]').append("<option value='" + params[i].Year + "'>" + params[i].Year + "</option>");
    }

    // 查询
    $('#btnSearch').click(function () {
        $('#table_list').jqGrid('setGridParam', {
            url: URL,
            mtype: 'post',
            postData: {
                filterContext: function () {
                    var tfCondition = $('input[name=tfCondition]').val();  // 企业代码 、客户名称、车辆名称
                    var year = $('select[name=cbYear]').val();
                    var month = $('select[name=cbMonth]').val();
                    var StringToJson = JSON.stringify({ "EnterpriseCode": tfCondition, "Year": year, "Month": month });
                    return StringToJson;
                }
            },
            page: 1
        }).trigger('reloadGrid');  // 重新载入
    });
}
$.ajax({
    url: '/IServer/Cars/CarsManage.ashx?action=cboCarName',
    type: 'post',
    async: false,
    dataType: 'json',
    success: function (data) {
        for (var i = 0; i < data.length; i++) {
            $('select[name=cboCarname]').append("<option value='" + data[i].CarName + "'>" + data[i].CarName + "</option>");
        }
    }
});
var AddDetail = function () {
    $('#myModal').modal('show');
    $('.modal-title').text('添加');
    $('input[name=CustomerName]').val("").attr('disabled', true);
    $('input[name=StockNumber]').val("");
    $('input[name=opType]').val('add');
}

var EditDetail = function () {
    var rowId = getRows("#table_list");
    if (rowId != null) {
        var rowData = $("#table_list").jqGrid('getRowData', rowId);
        $('input[name=opType]').val('edit');
        $('input[name=HidID]').val(rowId);
        $('input[name=hidCustomerID]').val(rowData.CustomerID);
        $('input[name=CustomerName]').val(rowData.Name);
        $('select[name=cboCarname]').val(rowData.CarName);
        $('input[name=StockDate]').val(rowData.StockMonth);
        $('input[name=StockNumber]').val(rowData.StockCount);
        $('.modal-title').text('编辑');
        $('#myModal').modal('show');
    }
    else {
        layer.msg("请至少选择一个项目");
    }
}

// 选择客户
var SelectCus = function () {
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
            $('input[name=CustomerName]').val(global.Name);
            $('input[name=hidCustomerID]').val(global.PKID);
            $('input[name=hidCode]').val(global.EnterpriseCode);
            layer.close(index); //关掉指定层
        },
    });
}

var SaveData = function () {
    var flag = $('#from2').valid();
    if (!flag) {
        return;
    }
    else {
        $('input[name=CustomerName]').attr('disabled', false); // 将disabled属性设置false，防止后台获取不到值
        var params = $("#from2").serialize();
        var SaveLoading = layer.msg('数据提交中，请稍候', { icon: 16, time: false, shade: 0.8 });
        $.ajax({
            url: '/IServer/Dealer/Dealer.ashx?action=SaveData',
            type: 'post',
            data: params,
            dataType: 'json',
            success: function (result) {
                var index =layer.getFrameIndex(window.name); //先得到当前iframe层的索引
                if (result.success) {
                   layer.close(SaveLoading);
                    $('#myModal').modal('hide');
                    layer.msg(result.msg);
                    $('#table_list').jqGrid('setGridParam').trigger('reloadGrid');  // 重新载入
                }
                else if (result.state = 1) {
                  layer.close(SaveLoading);
                    layer.msg(result.msg)
                }
                else {
                   layer.close(SaveLoading);
                    layer.msg(data.msg);
                }
            }, error: function (data) {
               layer.close(index);
            }
        });
        return false;
    }
}

var DeleteData = function () {
    var rowId = getRows("#table_list");
    if (rowId != null) {
        var rowData = $("#table_list").jqGrid('getRowData', rowId);
        layer.confirm('确定要删除信息吗？', { icon: 3, title: '提示信息' }, function (index) {
            var index = layer.msg('正在删除，请稍候', { icon: 16, time: false, shade: 0.8 });
            setTimeout(function () {
                $.ajax({
                    url: '/IServer/Dealer/Dealer.ashx?action=DeleteDealerStockByPKID',
                    type: 'post',
                    async: false,
                    data: { 'StockId': rowId },
                    success: function (data) {
                        var index =layer.getFrameIndex(window.name); //先得到当前iframe层的索引
                        $('#table_list').jqGrid({}).trigger('reloadGrid');  // 重新载入
                        layer.close(index);
                        layer.msg("删除成功。");
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
};
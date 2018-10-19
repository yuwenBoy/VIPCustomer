var URL = '/IServer/Cars/CarsManage.ashx?action=';
var GetBigCustomer = function () {
    var jqGird = $('#table_list').jqGrid({
        url: URL + 'GetMonthlyCarsNumberPagerList',
        mtype: 'post',
        datatype: 'json',
        viewrecords: true,  //  是否显示总记录数
        styleUI: 'Bootstrap',
        colModel: [
              { label: '主键ID', name: 'PKID', hidden: true, key: true, },
              { label: '车辆ID', name: 'CarID', hidden: true },
              { label: '车辆颜色ID', name: 'CarColorID', hidden: true },
              { label: '车名', name: 'Name', width: 120 },
              { label: 'SFX', name: 'SFX', width: 120 },
              { label: '颜色', name: 'Code', width: 120 },
              { label: '年', name: 'Year', width: 120 },
              { label: '月', name: 'Month', width: 120 },
              { label: '总资源', name: 'Count', width: 120 },
              { label: '剩余资源', name: 'ResidualQuantity', width: 120 },
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
            url: URL + 'GetMonthlyCarsNumberPagerList',
            mtype: 'post',
            postData: {
                filterContext: function () {
                    var cboCarName = $('select[name=cboCarName]').val();
                    var year = $('select[name=cbofilterYear]').val();
                    var month = $('select[name=cbMonth]').val();
                    var StringToJson = JSON.stringify({ "Name": cboCarName, "Year": year, "Month": month });
                    return StringToJson;
                }
            },
            page: 1
        }).trigger('reloadGrid');  // 重新载入
    });

    GetcboYear();
    GetcboCars();

}

// 获取月份
var GetcboYear = function () {
    var date = new Date;
    var year = date.getFullYear();
    var params = [];
    for (var i = 2005; i <= year + 1; i++) {
        params.push({ "Year": i });
    }
    for (var i = 0; i < params.length; i++) {
        $('select[name=cbofilterYear]').append("<option value='" + params[i].Year + "'>" + params[i].Year + "</option>");
        $('select[name=cboYear]').append("<option value='" + params[i].Year + "'>" + params[i].Year + "</option>");
    }
}

// 获取车辆名称
var GetcboCars = function () {
    $.ajax({
        url: URL + 'cboCarName',
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

var AddDetail = function () {
    ClearDetail();
    $('#myModal').modal('show');
    $('.modal-title').text('添加');
}

//清空窗体信息
var ClearDetail = function () {
    $('input[name=Name]').val("");
    $('input[name=SFX]').val("");
    $('select[name=cboColor]').val("");
    $("select[name=cboMonth]").val(new Date().getMonth() + 1);
    $("#cboYear").prepend("<option value='" + new Date().getYear() + "'>" + new Date().getYear() + "</option>");
    $('input[name=Count]').val("1");
    $('input[name=opType]').val('add');
}

var EditDetail = function () {
    var rowId = getRows("#table_list");
    if (rowId != null) {
        var rowData = $("#table_list").jqGrid('getRowData', rowId);
        $('input[name=opType]').val('update');
        $('input[name=HidID]').val(rowId);
        $('input[name=hidCarID]').val(rowData.CarID);
        $('input[name=SFX]').val(rowData.SFX);
        $('input[name=Name]').val(rowData.Name);
        $('select[name=cboColor]').append("<option value='" + rowData.CarColorID + "'>" + rowData.Code + "</option>");
        $('select[name=cboMonth]').val(rowData.Month);
        $('select[name=cboYear]').append("<option value='" + rowData.Year + "'>" + rowData.Year + "</option>");
        $('input[name=ResidualQuantity]').val(rowData.ResidualQuantity);
        $('input[name=Count]').val(rowData.Count);
        $('.modal-title').text('编辑');
        $('#myModal').modal('show');
    }
    else {
        layer.msg("请至少选择一个项目");
    }
}

// 选择车辆
var selectCars = function () {
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
            $('input[name=Name]').val(global.Name);
            $('input[name=SFX]').val(global.SFX);
            $('input[name=hidCarID]').val(global.PKID);
            addCarColor(global.PKID);
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
                    $("select[name=cboColor]").append("<option value='" + data[i].PKID + "'>" + data[i].Code + "</option>");
                }
            },
        })
    }
}

// 保存数据
var SaveData = function () {
    var flag = $('#from2').valid();
    if (!flag) {
        return;
    }
    else {
        $('input[name=Name]').attr('disabled', false); // 将disabled属性设置false，防止后台获取不到值
        $('input[name=SFX]').attr('disabled', false);
        $('input[name=ResidualQuantity]').attr('disabled', false);
        var params = $("#from2").serialize();
        var SaveLoading = layer.msg('数据提交中，请稍候', { icon: 16, time: false, shade: 0.8 });
        $.ajax({
            url: URL + 'SaveData',
            type: 'post',
            data: params,
            dataType: 'json',
            success: function (result) {
                var index =  layer.getFrameIndex(window.name); //先得到当前iframe层的索引
                if (result.success) {
                  layer.close(SaveLoading);
                    $('#myModal').modal('hide');
                    layer.msg(result.msg);
                    $('#table_list').jqGrid('setGridParam').trigger('reloadGrid');  // 重新载入
                }
                else if (result.state = 1) {
                   layer.close(SaveLoading);
                    return;
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

// 删除数据
var DeleteData = function () {
    var rowId = getRows("#table_list");
    if (rowId != null) {
        var rowData = $("#table_list").jqGrid('getRowData', rowId);
        layer.confirm('确定要删除信息吗？', { icon: 3, title: '提示信息' }, function (index) {
            var index = layer.msg('正在删除，请稍候', { icon: 16, time: false, shade: 0.8 });
            setTimeout(function () {
                $.ajax({
                    url: URL + 'DeleteData',
                    type: 'post',
                    async: false,
                    data: { 'Id': rowId },
                    success: function (data) {
                        var index = layer.getFrameIndex(window.name); //先得到当前iframe层的索引
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

var initPage = function () {
    GetBigCustomer();
}


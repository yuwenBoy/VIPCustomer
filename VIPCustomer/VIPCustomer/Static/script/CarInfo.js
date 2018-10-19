$(function () {
    InitMainTable();
    SaveData();
    East.saveData();
});
var URL = '/IServer/SysManage/CarsHandler.ashx?action=';
var $table;
function InitMainTable() {
    var queryUrl = URL + 'GetCarsManagePager';
    $table = $('#tbCars').bootstrapTable({
        url: queryUrl,                      //请求后台的URL（*）
        method: 'post',                      //请求方式（*）
        contentType: "application/x-www-form-urlencoded",//一种编码。好像在post请求的时候需要用到。这里用的get请求，注释掉这句话也能拿到数据
        striped: true,                      //是否显示行间隔色
        cache: false,                       //是否使用缓存，默认为true，所以一般情况下需要设置一下这个属性（*）
        pagination: true,                   //是否显示分页（*）
        sortable: true,                     //是否启用排序
        sortOrder: "asc",                   //排序方式
        sidePagination: "server",           //分页方式：client客户端分页，server服务端分页（*）
        pageNumber: 1,                      //初始化加载第一页，默认第一页,并记录
        pageSize: 50,                     //每页的记录行数（*）
        pageList: [50, 100, 200, 500],        //可供选择的每页的行数（*）
        search: false,                      //是否显示表格搜索
        strictSearch: true,
        showColumns: false,                  //是否显示所有的列（选择显示的列）
        showRefresh: false,                  //是否显示刷新按钮
        minimumCountColumns: 2,             //最少允许的列数
        clickToSelect: true,                 //是否启用点击选中行
        height: 580,//$(window).height - 173,                      //行高，如果没有设置height属性，表格自动根据记录条数觉得表格高度
        uniqueId: "PKID",                     //每一行的唯一标识，一般为主键列
        showToggle: false,                   //是否显示详细视图和列表视图的切换按钮
        cardView: false,                    //是否显示详细视图
        detailView: false,                  //是否显示父子表
        undefinedText: '',                  //清除列里面有“-”
        singleSelect: true,                 //是否可以多选
        paginationFirstText: "第一页",
        paginationPreText: "上一页",
        paginationNextText: "下一页",
        paginationLastText: "最后一页",
        //得到查询的参数
        queryParams: function (params) {
            //这里的键的名字和控制器的变量名必须一直，这边改动，控制器也需要改成一样的
            var temp = $('#searchForm').serializeObject();
            temp['rows'] = params.limit,// 总数据
            temp['page'] = (params.offset / params.limit) + 1,   //页码
            temp['sort'] = params.sort,//排序列名
            temp['sortOrder'] = params.order//排位命令（desc，asc）
            return temp;
        },
        columns: [{
            checkbox: true,
            visible: true                  //是否显示复选框
        },
         { title: '序号', width: 50, align: "center", formatter: function (value, row, index) { return index + 1; } },
         {
             field: 'Name',
             title: '车名',
             sortable: true,
             width: 150,
         }, {
             field: 'SFX',
             title: '车型缩写(SFX)',
             sortable: true,
             width: 100
         }, {
             field: 'CarLogoName',
             title: '车辆标识',
             sortable: true,
             width: 150,
         }, {
             field: 'CarModel',
             title: '车型(E/G)',
             sortable: true,
             width: 150
         }, {
             field: 'GearboxVersion',
             title: '变速箱型号(T/M)',
             sortable: true,
             width: 150
         }, {
             field: 'Spec',
             title: '规格',
             sortable: true,
             width: 180
         }, {
             field: 'Model',
             title: '型号',
             sortable: true,
             width: 150
         }, {
             field: 'Other',
             title: '数据用途',
             sortable: true,
             width: 150
         }, {
             field: 'BrandName',
             title: '品牌',
             sortable: true,
             width: 150
         }],
        onLoadError: function (res) {
            alert(res);
            layer.msg("加载数据失败", { time: 1500, icon: 2 });
        },
        onDblClickRow: function (row, $element) {
            $('#HidId').val(row.PKID);
            $('#myModal').modal('show');
            $('.modal-title').text('修改');
            initControl(row.PKID);
        },
    });
};

// 获取车型名称
$.post(URL + 'GetNameList', function (data) {
    var json = $.parseJSON(data);
    for (var i = 0; i < json.length; i++) {
        $('select[name=Name]').append("<option value='" + json[i].CarName + "'>" + json[i].CarName + "</option>");
    }
});

// 刷新信息
function RefreshData() {
    $('#tbCars').bootstrapTable('destroy');//先要将table销毁，否则会保留上次加载的内容
    InitMainTable();//重新初使化表格。
}

// 重置
function reset() {
    $('input[name=SFX]').val('');
    $('select[name=Name]').val('');
}

// 查询
function btn_search() {
    RefreshData();
}

// 获取车型名称
$.post(URL + 'GetNameList', function (data) {
    var json = $.parseJSON(data);
    for (var i = 0; i < json.length; i++) {
        $('select[name=fr_Name]').append("<option value='" + json[i].CarName + "'>" + json[i].CarName + "</option>");
    }
});

// 获取类型
$.post(URL + 'GetTypeList', function (data) {
    var json = $.parseJSON(data);
    for (var i = 0; i < json.length; i++) {
        $('select[name=TypeID]').append("<option value='" + json[i].TypeID + "'>" + json[i].TypeName + "</option>");
    }
});

// 获取品牌
$.post(URL + 'GetBrandList', function (data) {
    var json = $.parseJSON(data);
    for (var i = 0; i < json.length; i++) {
        $('select[name=BrandID]').append("<option value='" + json[i].BrandID + "'>" + json[i].BrandName + "</option>");
    }
});

// 获取配车标识
$.post(URL + 'GetCarLogoList', function (data) {
    var json = $.parseJSON(data);
    for (var i = 0; i < json.length; i++) {
        $('select[name=CarLogoID]').append("<option value='" + json[i].CarLogoID + "'>" + json[i].CarLogoName + "</option>");
    }
});

// 新增
function AddDetail() {
    $('#myModal').modal('show');
    $('#lblAddTitle').text('添加');
}

// 编辑
function EditDetail() {
    var rows = $table.bootstrapTable('getSelections');
    if (rows <= 0) {
        layer.msg('请至少选择一条数据。');
        return;
    }
    else {
        // 编辑状态下把主键ID赋值给隐藏域，方便后台接收处理
        $('#HidId').val(rows[0].PKID);
        $('#myModal').modal('show');
        $('#lblAddTitle').text('修改');
        initControl(rows[0].PKID);
    }
}

// 编辑将对象绑定到控件
function initControl(Id) {
    $.getJSON(URL + 'GetCarInfoByPKID&keyword=' + Id, function (res) {
        $('select[name=fr_Name]').val(res.Name);
        $('input[name=CarModel]').val(res.CarModel);
        $('input[name=GearboxVersion]').val(res.GearboxVersion);
        $('input[name=Spec]').val(res.Spec);
        $('input[name=Model]').val(res.Model);
        $('input[name=fr_SFX]').val(res.SFX);
        $('input[name=Code]').val(res.Code);
        $('select[name=BrandID]').val(res.BrandID),
        $('input[name=BuiltInColor]').val(res.BuiltInColor);
        $('input[name=Drive]').val(res.Drive);
        $('input[name=SuggestPrice]').val(res.SuggestPrice);
        $('select[name=CarLogoID]').val(res.CarLogoID);
        $('input[name=Remark]').val(res.Remark);
        $('select[name=TypeID]').val(res.TypeID);
    });
}

// 保存数据
function SaveData() {
    $('#Form1').bootstrapValidator({
        message: 'This value is not valid',
        feedbackIcons: {/*input状态样式图片*/
            valid: 'glyphicon glyphicon-ok',
            invalid: 'glyphicon glyphicon-remove',
            validating: 'glyphicon glyphicon-refresh'
        },
        fields: {/*验证：规则*/
            fr_Name: {//验证input项：验证规则
                message: '车型名称不能为空',
                validators: {
                    notEmpty: {//非空验证：提示消息
                        message: '车型名称不能为空'
                    }
                }
            },
            fr_SFX: {
                message: '车型缩写（SFX）不能为空',
                validators: {
                    notEmpty: {
                        message: '车型缩写（SFX）不能为空'
                    },
                }
            }
        }
    }).on('success.form.bv', function (e) {//点击提交之后
        e.preventDefault();    // Prevent form submission
        var $form = $(e.target);      // Get the form instance
        // Use Ajax to submit form data 提交至form标签中的action，result自定义
        $.post($form.attr('action'), $form.serialize(), function (result) {
            var SaveLoading =layer.msg('数据提交中，请稍候', { icon: 16, time: true, shade: 0.8 });
            var json = JSON.parse(result);  // 由json字符串转换为json对象
            if (json.success) {
              layer.close(SaveLoading);
                layer.msg("保存成功", { time: 1500, icon: 1 });
                RefreshData();
                $('#myModal').modal('hide');
                $('#Form1').bootstrapValidator('resetForm');
                clearForm($($form));
            }
            else if (json.state == "1") {
                var arr = json.msg;
                var spStr = arr.split(',');
                var Name = spStr[0];
                var SFX = spStr[1];
                layer.alert("车辆【" + Name + "】SFX【" + SFX + "】信息已经存在，请修改。");
            }
            else {
              layer.close(SaveLoading);
                layer.msg("保存失败，请重试。", { time: 3000, icon: 2 });
            }
        });
    });
    // 关闭
    $('#btnClose').on('click', function () {
        $('#Form1').bootstrapValidator('resetForm');
        $('#myModal').modal('hide');
    });
}

// 删除
function DeleteDetail() {
    var rows = $table.bootstrapTable('getSelections');
    if (rows <= 0) {
        layer.msg('请至少选择一条数据。');
        return;
    }
    else {
        layer.confirm('您确定要删除车型信息主键为【' + rows[0].PKID + '】的数据吗？ ', { icon: 3, title: '提示信息' }, function (index) {
            var index = layer.msg('正在删除，请稍候', { icon: 16, time: false, shade: 0.8 });
            setTimeout(function () {
                $.ajax({
                    url: URL + 'DeleteCarInfoByPKID',
                    type: 'post',
                    async: false,
                    data: { PKID: rows[0].PKID },
                    dataType: 'json',
                    success: function (data) {
                        var index =layer.getFrameIndex(window.name); //先得到当前iframe层的索引
                        if (data.success) {
                            layer.close(index);
                            layer.msg("删除成功");
                            RefreshData();
                        }
                        else {
                           layer.close(index);
                            layer.msg("删除失败，请重试。", { time: 3000, icon: 2 });
                        }
                    }, error: function (data) {
                      layer.close(index);
                    }
                });
                return false;
            }, 2000);
        })
    }
}

// 颜色设定
function SetColor() {
    var rows = $table.bootstrapTable('getSelections');
    if (rows <= 0) {
        layer.msg('请至少选择一条数据。');
        return;
    }
    else {
        $('#myModal2').modal('show');
        East.showList(rows[0].PKID);
        $('#hidCarId').val(rows[0].PKID);
    }
}

// 关闭设定颜色窗口
function Hide() {
    $('#myModal2').modal('hide');
}

var $tb2;
// 设定颜色列表：增删改查操作
var East = {
    showList: function (carId) {
        $tb2 = $('#TB2').bootstrapTable({
            columns: [
                {
                    checkbox: true,
                    visible: true                  //是否显示复选框
                }, { title: '序号', width: 30, align: "center", formatter: function (value, row, index) { return index + 1; } },
                 {
                     field: 'PKID',
                     title: 'PKID',
                     visible: false
                 },
                  {
                      field: 'Code',
                      title: '颜色编码',
                      sortable: true,
                      width: 150,
                  },
             {
                 field: 'Name',
                 title: '颜色',
                 sortable: true,
                 width: 150,
             }],
            toolbar: '#Tool',
            url: URL + 'GetCarColorInfo&PKID=' + carId,
            striped: true,//是否显示行间隔色
            height: 420,
            undefinedText: '',  //清除列里面有“-”
            singleSelect: true,                 //是否可以多选               
            onLoadError: function () {
                layer.msg("加载数据失败", { time: 1500, icon: 2 });
            },
        });
    },
    RefreshData: function () {
        $tb2.bootstrapTable('refresh');
    },
    showDetail: function (type) {
        if (type == 1) {
            $('#showCarColor').modal('show');
        }
        else {
            var rows = $tb2.bootstrapTable('getSelections');
            if (rows <= 0) {
                layer.msg('请至少选择一条数据。');
                return;
            }
            else {
                $('#showCarColor').modal('show');
                $('#showCarColorTitle').text('修改');
                $('#HidCarColorId').val(rows[0].PKID);
                $('input[name=colorCode]').val(rows[0].Code);
                $('input[name=colorName]').val(rows[0].Name);
            }
        }
    },
    saveData: function () {
        $('#Form3').bootstrapValidator({
            message: 'This value is not valid',
            feedbackIcons: {/*input状态样式图片*/
                valid: 'glyphicon glyphicon-ok',
                invalid: 'glyphicon glyphicon-remove',
                validating: 'glyphicon glyphicon-refresh'
            },
            fields: {/*验证：规则*/
                colorCode: {//验证input项：验证规则
                    message: '颜色编码不能为空',
                    validators: {
                        notEmpty: {//非空验证：提示消息
                            message: '颜色编码不能为空'
                        }
                    }
                },
            }
        }).on('success.form.bv', function (e) {//点击提交之后
            e.preventDefault();    // Prevent form submission
            var $form = $(e.target);      // Get the form instance
            // Use Ajax to submit form data 提交至form标签中的action，result自定义
            $.post($form.attr('action'), $form.serialize(), function (result) {
                var SaveLoading =layer.msg('数据提交中，请稍候', { icon: 16, time: true, shade: 0.8 });
                var json = JSON.parse(result);  // 由json字符串转换为json对象
                if (json.success) {
                 layer.close(SaveLoading);
                    layer.msg("保存成功", { time: 1500, icon: 1 });
                    East.RefreshData();
                    $('#showCarColor').modal('hide');
                    clearForm($($form));
                }
                else if (json.state == "1") {
                    layer.alert("颜色信息已经存在，请修改。");
                }
                else {
                 layer.close(SaveLoading);
                    layer.msg("保存失败，请重试。", { time: 3000, icon: 2 });
                }
            });
        });
    },
    close: function () {
        $('#showCarColor').modal('hide');
        $('input[name=colorCode]').val('');
        $('input[name=colorName]').val('');
    },
    deleteData: function () {
        var rows = $tb2.bootstrapTable('getSelections');
        if (rows <= 0) {
            layer.msg('请至少选择一条数据。');
            return;
        }
        else {
            layer.confirm('您确定要删除选中的数据吗？ ', { icon: 3, title: '提示信息' }, function (index) {
                var index = layer.msg('正在删除，请稍候', { icon: 16, time: false, shade: 0.8 });
                setTimeout(function () {
                    $.ajax({
                        url: URL + 'EastDeleteData',
                        type: 'post',
                        async: false,
                        data: { PKID: rows[0].PKID },
                        dataType: 'json',
                        success: function (data) {
                            var index = layer.getFrameIndex(window.name); //先得到当前iframe层的索引
                            if (data.success) {
                                layer.close(index);
                                layer.msg("删除成功");
                                East.RefreshData();
                            }
                            else {
                                layer.close(index);
                                layer.msg("删除失败，请重试。", { time: 3000, icon: 2 });
                            }
                        }, error: function (data) {
                            layer.close(index);
                        }
                    });
                    return false;
                }, 2000);
            })
        }
    }
}



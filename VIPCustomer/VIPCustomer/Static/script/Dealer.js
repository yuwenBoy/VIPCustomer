$(function () {
    InitMainTable();
    SaveData();
    $("select[name=cboBigArea]").on('change', function () {
        var val = $(this).val();
        if (val != null) {
            bindProvince(val);
        }
    });
    $("select[name=cboProvince]").on('change', function () {
        var val = $(this).val();
        if (val != null) {
            bindCity(val);
        }
    });
})
var $table;
var URL = '/IServer/Dealer/Dealer.ashx?action=';
function InitMainTable() {
    $table = $('#TbDC').bootstrapTable({
        url: URL + 'GetDealerManagePager',                      //请求后台的URL（*）
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
        pageList: [50, 100, 200, 500],
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
        singleSelect: false,                 //是否可以多选
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
             field: 'Code',
             title: '经销商编号',
             sortable: true,
             width: 150
         }, {
             field: 'Name',
             title: '经销商名称',
             sortable: true,
             width: 300
         }, {
             field: 'OldName',
             title: '曾用名',
             sortable: true,
             width: 300
         }, {
             field: 'BearUserId',
             title: '地担',
             sortable: true, width: 120,
             formatter: function (value, row, index) {
                 if (value == 0) {
                     return '';
                 }
             }

         }, {
             field: 'RegionManagerUserId',
             title: '大区经理',
             sortable: true, width: 120, formatter: function (value, row, index) {
                 if (value == 0) {
                     return '';
                 }
             }
         },
         {
             field: 'Country',
             title: '大区',
             sortable: true, width: 120
         }, {
             field: 'Province',
             title: '省份',
             sortable: true, width: 120
         },
         {
             field: 'City',
             title: '城市',
             sortable: true, width: 120
         },
         {
             field: 'Address',
             title: '地址',
             sortable: true, width: 520
         }, {
             field: 'ZipCode',
             title: '邮政编码',
             sortable: true, width: 120
         }, {
             field: 'Phone',
             title: '电话',
             sortable: true, width: 120
         }, {
             field: 'fax',
             title: '传真',
             sortable: true, width: 120
         },
         {
             field: 'Email',
             title: '邮箱',
             sortable: true, width: 120
         }, {
             field: 'SalesTel',
             title: '销售电话',
             sortable: true, width: 120
         }, {
             field: 'ServerTel',
             title: '服务电话',
             sortable: true, width: 120
         }, {
             field: 'SystemEmail',
             title: '系统邮箱',
             sortable: true, width: 120

         }, {
             field: 'SalesDepartment',
             title: '销售部长',
             sortable: true, width: 120
         }, {
             field: 'SalesDepartmentTel',
             title: '销售部长电话',
             sortable: true, width: 120
         }
         , {
             field: 'MaxCommissioner',
             title: '大区专员',
             sortable: true, width: 120
         }, {
             field: 'MaxCommissionerTel',
             title: '大区专员电话',
             sortable: true, width: 120
         }],
        onLoadSuccess: function () {
            //  alert("加载成功");
        },
        onLoadError: function () {
            layer.msg("加载数据失败", { time: 1500, icon: 2 });
        },
        onDblClickRow: function (row, $element) {
            var id = row.PKID;
            $('#HidId').val(id);// 编辑状态下把主键ID赋值给隐藏域，方便后台接收处理
            $('#myModal').modal('show');
            $('.modal-title').text('修改');
            bindController(id);
        },
    });
};

// 刷新信息
function RefreshData() {
    $('#TbDC').bootstrapTable('destroy');//先要将table销毁，否则会保留上次加载的内容
    InitMainTable();//重新初使化表格。
}

// 重置
function reset() {
    $('input[name=Code]').val('');
    $('input[name=Name]').val('');
    $('select[name=cboBigArea]').val(-1);
    $('select[name=cboProvince]').val(-1);
    $('select[name=cboCity]').val(-1);
}

// 查询
function btn_search() {
    RefreshData();
}

// 获取大区
$.post('/IServer/SysManage/SysManage.ashx?action=GetList', function (data) {
    var json = $.parseJSON(data);
    for (var i = 0; i < json.length; i++) {
        $('select[name=cboBigArea]').append("<option value='" + json[i].cityId + "'>" + json[i].City + "</option>");
    }
});

// 获取省
function bindProvince(cityId) {
    $.post('/IServer/SysManage/SysManage.ashx?action=GetProvince&cityId=' + cityId, function (result) {
        if (result != null) {
            var cboProvince = $("select[name=cboProvince]");
            //清空第二级里面的省
            cboProvince.empty();
            for (var i = 0; i < result.length; i++) {
                $('select[name=cboProvince]').append("<option value='" + result[i].PKID + "'>" + result[i].City + "</option>");
            }
        }
    }, 'json');
}

// 获取城市
function bindCity(cityId) {
    $.post('/IServer/SysManage/SysManage.ashx?action=GetCity&cityId=' + cityId, function (result) {
        if (result != null) {
            var cboProvince = $("select[name=cboCity]");
            //清空第二级里面的省
            cboProvince.empty();
            for (var i = 0; i < result.length; i++) {
                $('select[name=cboCity]').append("<option value='" + result[i].cityId + "'>" + result[i].CityName + "</option>");
            }
        }
    }, 'json');
}

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
        bindController(rows[0].PKID);
    }
}

// 编辑将对象绑定到控件
function bindController(Id) {
    $.getJSON(URL + 'GetDealerByPKID&dealerId=' + Id, function (data) {
        $('input[name=fr_Code]').val(data.Code);
        $('input[name=fr_Name]').val(data.Name);
        $('input[name=OldName]').val(data.OldName);
        $('select[name=Code]').val(data.Code);
        $('select[name=Code]').val(data.Code);
        $('select[name=cboBigArea]').val(data.CountryID);


        if ($('select[name=cboProvince]').val() == null) {
            $('select[name=cboProvince]').append("<option value='" + data.ProvinceID + "'>" + data.Province + "</option>");
        }
        else {
            $('select[name=cboProvince]').empty();
            $('select[name=cboProvince]').append("<option value='" + data.ProvinceID + "'>" + data.Province + "</option>");
        }

        if ($('select[name=cboCity]').val() == null) {
            $('select[name=cboCity]').append("<option value='" + data.CityId + "'>" + data.City + "</option>");
        }
        else {
            $('select[name=cboCity]').empty();
            $('select[name=cboCity]').append("<option value='" + data.CityId + "'>" + data.City + "</option>");
        }
        $('input[name=Address]').val(data.Address);
        $('input[name=ZipCode]').val(data.ZipCode);
        $('input[name=Phone]').val(data.Phone);
        $('input[name=fax]').val(data.fax);
        $('input[name=Email]').val(data.Email);
        $('input[name=SalesTel]').val(data.SalesTel);
        $('input[name=ServerTel]').val(data.ServerTel);
        $('input[name=SystemEmail]').val(data.SystemEmail);
        $('input[name=SalesDepartment]').val(data.SalesDepartment);
        $('input[name=SalesDepartmentTel]').val(data.SalesDepartmentTel);
        $('input[name=MaxCommissioner]').val(data.MaxCommissioner);
        $('input[name=MaxCommissionerTel]').val(data.MaxCommissionerTel);
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
            fr_Code: {//验证input项：验证规则
                message: '经销商编号不能为空',
                validators: {
                    notEmpty: {//非空验证：提示消息
                        message: '经销商编号不能为空'
                    }
                }
            },
            fr_Name: {
                message: '经销商名称不能为空',
                validators: {
                    notEmpty: {
                        message: '经销商名称不能为空'
                    },
                }
            },
            cboCity: {
                message: '城市不能为空',
                validators: {
                    notEmpty: {
                        message: '城市不能为空'
                    },
                }
            }
        }
    }).on('success.form.bv', function (e) {
        e.preventDefault();
        var $form = $(e.target);
        $.post($form.attr('action'), $form.serialize(), function (result) {
            var SaveLoading =layer.msg('数据提交中，请稍候', { icon: 16, time: true, shade: 0.8 });
            var json = JSON.parse(result);  // 由json字符串转换为json对象
            if (json.success) {
            layer.close(SaveLoading);
                layer.msg("保存成功。", { time: 1500, icon: 1 });
                RefreshData();
                $('select[name=cboBigArea]').val(-1);
                $('select[name=cboProvince]').val(-1);
                $('select[name=cboCity]').val(-1);
                $('#myModal').modal('hide');
                $form.bootstrapValidator('resetForm');
                clearForm($($form));
            }
            else if (json.state == "T") {
                layer.alert(json.msg);
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
        var delData = "";
        for (var i = 0; i < rows.length; i++) {
            delData += rows[i].PKID + ",";
        }
        var delDatas = delData.substring(0, delData.length - 1);
        layer.confirm('您确定要删除选择的【' + rows.length + '条】经销商吗？ ', { icon: 3, title: '提示信息' }, function (index) {
            var index = layer.msg('正在删除，请稍候', { icon: 16, time: false, shade: 0.8 });
            setTimeout(function () {
                $.ajax({
                    url: URL + 'DeleteData',
                    type: 'post',
                    async: false,
                    data: { dealerIds: delDatas },
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
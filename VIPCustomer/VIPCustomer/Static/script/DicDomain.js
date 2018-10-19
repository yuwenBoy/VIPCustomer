$(function () {
    InitDTDicType();
    SaveData();
    Item.submitListData();
    GetPagerListByCode($('#HidCode2').val());
});

var $table;
var $table2;
var URL = '/IServer/SysManage/SysManage.ashx?action=';
function InitDTDicType() {
    $table = $('#tbDicType').bootstrapTable({
        toolbar: '#Tool',
        url: URL + 'PagerTypeList',                      //请求后台的URL（*）
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
        height: 680,                        //行高，如果没有设置height属性，表格自动根据记录条数觉得表格高度
        uniqueId: "Code",                     //每一行的唯一标识，一般为主键列
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
             field: 'Code',
             title: '预编码',
             sortable: true,
         }, {
             field: 'Name',
             title: '域名称',
             sortable: true,
         }],
        onLoadError: function () {
            layer.msg("加载数据失败", { time: 1500, icon: 2 });
        },
        onDblClickRow: function (row, $element) {
            $('#tbDicList').bootstrapTable('destroy');
            GetPagerListByCode(row.Code);
        },
        onClickRow: function (row, $element) {
            $('#tbDicList').bootstrapTable('destroy');
            GetPagerListByCode(row.Code);
        },
    });
};

// 根据数据分类域编码查询数据列表
function GetPagerListByCode(code) {
    $table2 = $('#tbDicList').bootstrapTable({
        url: URL + 'PagerList&keyword=' + code,                      //请求后台的URL（*）
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
        pageList: [50, 100, 200, 500],           //可供选择的每页的行数（*）
        search: false,                      //是否显示表格搜索
        strictSearch: true,
        showColumns: false,                  //是否显示所有的列（选择显示的列）
        showRefresh: false,                  //是否显示刷新按钮
        minimumCountColumns: 2,             //最少允许的列数
        clickToSelect: true,                 //是否启用点击选中行
        height: 680,//$(window).height - 173,                      //行高，如果没有设置height属性，表格自动根据记录条数觉得表格高度
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
             field: 'ListName',
             title: '列表显示名称',
             sortable: true,
         }, {
             field: 'Sort',
             title: '排序',
             sortable: true,
         }, {
             field: 'Remark',
             title: '说明',
         }],
        onLoadError: function () {
            layer.msg("加载数据失败", { time: 1500, icon: 2 });
        },
        onDblClickRow: function (row, $element) {
            $('#myModal2').modal('show');
            $('.lblAddTitle2').text('修改');
            $('#HidID').val(row.PKID);
            $('#HidCode2').val(row.Code);
            Item.initBindData(row.PKID);
        },
    });
}

// 刷新分类表格
function RefreshData() {
    $('#tbDicType').bootstrapTable('destroy');//先要将table销毁，否则会保留上次加载的内容
    InitDTDicType();//重新初使化表格。
}

function reset() {
    $('input[name=Name]').val('');
    $('input[name=Code]').val('');
    $('input[name=ListName]').val('');
    $('textarea[name=Remark]').val('');
    $('input[name=Sort]').val('');
}

// 新增
function AddDetail() {
    $('#hidDiv').show();
    $('#hidDiv2').show();
    $('#hidDiv3').show();
    $('#hidDiv4').show();
    reset();
    $('#myModal').modal('show');
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
        $('#HidCode').val(rows[0].Code);
        $('#hidDiv').hide();
        $('#hidDiv2').hide();
        $('#hidDiv3').hide();
        $('#hidDiv4').hide();
        $('#myModal').modal('show');
        $('#lblAddTitle').text('修改');
        bindController(rows[0].Code);
    }
}

// 编辑将对象绑定到控件
function bindController(code) {
    $.getJSON(URL + 'GetDicDomainByCode&keyword=' + code, function (res) {
        $('input[name=Name]').val(res.Name);
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
            Code: {//验证input项：验证规则
                message: '域编码不能为空',
                validators: {
                    notEmpty: {//非空验证：提示消息
                        message: '域编码不能为空'
                    }
                }
            },
            Name: {
                message: '域名称不能为空',
                validators: {
                    notEmpty: {
                        message: '域名称不能为空'
                    },

                }
            },
            ListName: {
                message: '列表显示名称不能为空',
                validators: {
                    notEmpty: {
                        message: '列表显示名称不能为空'
                    },
                }
            }
        }
    }).on('success.form.bv', function (e) {//点击提交之后
        e.preventDefault();
        var $form = $(e.target);
        $.post($form.attr('action'), $form.serialize(), function (result) {
            var SaveLoading =layer.msg('数据提交中，请稍候', { icon: 16, time: true, shade: 0.8 });
            var json = JSON.parse(result);  // 由json字符串转换为json对象
            if (json.success) {
              layer.close(SaveLoading);
                layer.msg("保存成功", { time: 1500, icon: 1 });
                RefreshData();
                $('#myModal').modal('hide');
                reset();
            }
            else if (json.state == "1") {
                var arr = json.msg;
                var spStr = arr.split(',');
                var code = spStr[0];
                var name = spStr[1];
                layer.alert("域编码【" + code + "】或域名称【" + name + "】信息已经存在，请修改。");
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
        layer.confirm('您确定要删除选择的数据吗？ ', { icon: 3, title: '提示信息' }, function (index) {
            var index = layer.msg('正在删除，请稍候', { icon: 16, time: false, shade: 0.8 });
            setTimeout(function () {
                $.ajax({
                    url: URL + 'DeleteDicDomainByCode',
                    type: 'post',
                    async: false,
                    data: { keyword: rows[0].Code },
                    dataType: 'json',
                    success: function (data) {
                        var index =layer.getFrameIndex(window.name); //先得到当前iframe层的索引
                        if (data.success) {
                            layer.close(index);
                            layer.msg("删除成功");
                            RefreshData();
                            $('#tbDicList').bootstrapTable('destroy');//先要将table销毁，否则会保留上次加载的内容
                            GetPagerListByCode(rows[0].Code);//重新初使化表格。
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

// 数据列表增删改
var Item = {
    Add: function () {
        var rows = $table.bootstrapTable('getSelections');
        if (rows <= 0) {
            layer.msg('请选择一个域类别。');
            return;
        }
        else {
            reset();
            $('#HidID').val(0);
            $('#HidName').val(rows[0].Name);
            $('#HidCode2').val(rows[0].Code);
            $('#myModal2').modal('show');
        }
    },
    Edit: function () {
        var rows = $table2.bootstrapTable('getSelections');
        if (rows <= 0) {
            layer.msg('请至少选择一条数据。');
            return;
        }
        else {
            // 编辑状态下把主键ID赋值给隐藏域，方便后台接收处理
            $('#myModal2').modal('show');
            $('.lblAddTitle2').text('修改');
            $('#HidID').val(rows[0].PKID);
            $('#HidCode2').val(rows[0].Code);
            Item.initBindData(rows[0].PKID);
        }
    },
    initBindData: function (keyword) {
        $.getJSON(URL + 'GetDicDomainByPKID&keyword=' + keyword, function (res) {
            $('textarea[name=Remark]').val(res.Remark);
            $('input[name=Sort]').val(res.Sort);
            $('input[name=ListName]').val(res.ListName);
        });
    },
    submitListData: function () {
        $('#Form2').bootstrapValidator({
            message: 'This value is not valid',
            feedbackIcons: {/*input状态样式图片*/
                valid: 'glyphicon glyphicon-ok',
                invalid: 'glyphicon glyphicon-remove',
                validating: 'glyphicon glyphicon-refresh'
            },
            fields: {/*验证：规则*/
                ListName: {
                    message: '列表显示名称不能为空',
                    validators: {
                        notEmpty: {
                            message: '列表显示名称不能为空'
                        },
                    }
                }
            }
        }).on('success.form.bv', function (e) {//点击提交之后
            e.preventDefault();
            var $form = $(e.target);
            $.post($form.attr('action'), $form.serialize(), function (result) {
                var SaveLoading =layer.msg('数据提交中，请稍候', { icon: 16, time: true, shade: 0.8 });
                var json = JSON.parse(result);  // 由json字符串转换为json对象
                if (json.success) {
                 layer.close(SaveLoading);
                    layer.msg("保存成功", { time: 1500, icon: 1 });
                    $('#myModal2').modal('hide');
                    $('#tbDicList').bootstrapTable('destroy');//先要将table销毁，否则会保留上次加载的内容
                    GetPagerListByCode($('#HidCode2').val());//重新初使化表格。
                    reset();
                }
                else if (json.state == 11) {
                    layer.alert("列表显示名称【" + json.msg + "】已经存在，请修改。");
                }
                else {
                  layer.close(SaveLoading);
                    layer.msg("保存失败，请重试。", { time: 3000, icon: 2 });
                }
            });
        });

    },
    Delete: function () {
        var rows = $table2.bootstrapTable('getSelections');
        if (rows <= 0) {
            layer.msg('请至少选择一条数据。');
            return;
        }
        else {
            layer.confirm('您确定要删除选择的车辆吗？ ', { icon: 3, title: '提示信息' }, function (index) {
                var index = layer.msg('正在删除，请稍候', { icon: 16, time: false, shade: 0.8 });
                setTimeout(function () {
                    $.ajax({
                        url: URL + 'DeleteItemsByPKID',
                        type: 'post',
                        async: false,
                        data: { keyword: rows[0].PKID },
                        dataType: 'json',
                        success: function (data) {
                            var index =layer.getFrameIndex(window.name); //先得到当前iframe层的索引
                            if (data.success) {
                                layer.close(index);
                                layer.msg("删除成功");
                                $('#tbDicList').bootstrapTable('destroy');//先要将table销毁，否则会保留上次加载的内容
                                GetPagerListByCode(rows[0].Code);//重新初使化表格。
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
    },
    Close: function () {
        $('#Form2').bootstrapValidator('resetForm');
        $('#myModal2').modal('hide');
    }
}
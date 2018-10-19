$(function () {
    InitMainTable();
    SaveData();
})
var $table;
function InitMainTable() {
    //记录页面bootstrap-table全局变量$table，方便应用
    var queryUrl = '/IServer/Dealer/Dealer.ashx?action=GetDealerContactManagerPager';
    $table = $('#TbDC').bootstrapTable({
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
        singleSelect: true,                 //是否可以多选
        paginationFirstText: "第一页",
        paginationPreText: "上一页",
        paginationNextText: "下一页",
        paginationLastText: "最后一页",
        //得到查询的参数
        queryParams: function (params) {
            ////这里的键的名字和控制器的变量名必须一直，这边改动，控制器也需要改成一样的
            var temp = $('#searchForm').serializeObject();
            temp['rows'] = params.limit,// 总数据
            temp['page'] = (params.offset / params.limit) + 1,   //页码
            temp['sort'] = params.sort,//排序列名
            temp['sortOrder'] = params.order//排位命令（desc，asc）
            return temp;
        },
        columns: [{
            checkbox: true,
            visible: true //是否显示复选框
        },
            { title: '序号', width: 50, align: "center", formatter: function (value, row, index) { return index + 1; } },
            {
                field: 'DealerName',
                title: '所属经销店',
                sortable: true
            }, {
                field: 'Name',
                title: '姓名',
                sortable: true
            }, {
                field: 'JobName',
                title: '职务',
                sortable: true,
            }, {
                field: 'Phone',
                title: '电话',
                sortable: true,
            }, {
                field: 'MobileTel',
                title: '移动电话',
                sortable: true,
            }, {
                field: 'Fax',
                title: '传真',
                sortable: true
            }, {
                field: 'Email',
                title: '电子信箱',
                sortable: true
            }, {
                field: 'OtherContactInfo',
                title: '其他联系方式',
                sortable: true
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
            //  $($element).addClass("backcolor").siblings("tr").removeClass("backcolor");
            $('#myModal').modal('show');
            $('.modal-title').text('修改');
            bindController(id);
        },
    });

    // 获取职务列表
    $.post('/IServer/Dealer/Dealer.ashx?action=GetJobList', function (data) {
        var json = $.parseJSON(data);
        for (var i = 0; i < json.length; i++) {
            $('select[name=JobID]').append("<option value='" + json[i].JobID + "'>" + json[i].JobName + "</option>");
        }
    });
};

// 刷新信息
function RefreshData() {
    $('#TbDC').bootstrapTable('destroy');//先要将table销毁，否则会保留上次加载的内容
    InitMainTable();//重新初使化表格。
}

// 获取职务列表
$.post('/IServer/Dealer/Dealer.ashx?action=GetJobList', function (data) {
    var json = $.parseJSON(data);
    for (var i = 0; i < json.length; i++) {
        $('select[name=fr_JobID]').append("<option value='" + json[i].JobID + "'>" + json[i].JobName + "</option>");
    }
});

function bindController(Id) {
    $.getJSON('/IServer/Dealer/Dealer.ashx?action=GetDealerContactByPKID&keyword=' + Id, function (res) {
        $('input[name=fr_Name]').val(res.Name);
        $('input[name=DealerName]').val(res.DealerName);
        $('input[name=DealerId]').val(res.DealerID);
        $('select[name=fr_JobID]').val(res.JobID);
        $('input[name=MobileTel]').val(res.MobileTel);
        $('input[name=OtherContactInfo]').val(res.OtherContactInfo);
        $('input[name=Email]').val(res.Email);
        $('input[name=Fax]').val(res.Fax);
        $('input[name=Phone]').val(res.Phone);
        $('textarea[name=Remark]').val(res.Remark);
    })
}
var DealerID = null;

// 新增
function AddDetail() {
    $('#myModal').modal('show');
    $('input[name=DealerId]').val(DealerID);
    $('#Form1').bootstrapValidator('resetForm');
    $('#lblAddTitle').text('添加');
}


// 获取当前用户所属经销店
$.post('/IServer/Dealer/Dealer.ashx?action=GetUserDealerByPKID', function (data) {
    var json = $.parseJSON(data);
    DealerID = json.DealerId;
    $('input[name=DealerId]').val(json.DealerId);
    $('input[name=DealerName]').val(json.DealerName);
});

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

// 查询
function SearchInfo() {
    $table.bootstrapTable('refresh');
}

// 重置
function reset() {
    $('input[name=Name]').val('');
    $('select[name=JobID]').val(-1);
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
                message: 'The username is not valid',

                validators: {
                    notEmpty: {//非空验证：提示消息
                        message: '姓名不能为空'
                    }
                }
            },
            Phone: {
                message: '电话不能为空',
                validators: {
                    notEmpty: {
                        message: '电话不能为空'
                    },

                }
            },
            MobileTel: {
                validators: {
                    notEmpty: {
                        message: '移动电话不能为空'
                    }
                }
            },
            Email: {
                message: '',
                validators: {
                    notEmpty: {
                        message: '电子信箱不能为空'
                    },
                }
            }
        }
    }).on('success.form.bv', function (e) {//点击提交之后
        e.preventDefault();    // Prevent form submission
        var $form = $(e.target);      // Get the form instance
        // Use Ajax to submit form data 提交至form标签中的action，result自定义
        $.post($form.attr('action'), $form.serializeObject(), function (result) {
            var SaveLoading = top.layer.msg('数据提交中，请稍候', { icon: 16, time: true, shade: 0.8 });
            var json = JSON.parse(result);  // 由json字符串转换为json对象
            if (json.success) {
                top.layer.close(SaveLoading);
                layer.msg("保存成功。", { time: 1500, icon: 1 });
                RefreshData();
                $('#myModal').modal('hide');
                clearForm($($form));
                $('input[name=DealerId]').val(DealerID);
            }
            else if (json.state == 'T') {
                layer.alert(json.msg);
            }
            else {
                top.layer.close(SaveLoading);
                layer.msg("保存失败，请重试。", { time: 3000, icon: 2 });
            }
        });
    });
    // 关闭
    $('#btnClose').on('click', function () {
        $('#Form1').bootstrapValidator('resetForm');
        $('#myModal').modal('hide');
    })
}

// 删除
function DeleteDetail() {
    var rows = $table.bootstrapTable('getSelections');
    if (rows <= 0) {
        layer.msg('请至少选择一条数据。');
        return;
    }
    else {
        layer.confirm('您确定要删除选择的经销店联系人吗？', { icon: 3, title: '提示信息' }, function (index) {
            var index = layer.msg('正在删除，请稍候', { icon: 16, time: false, shade: 0.8 });
            setTimeout(function () {
                $.ajax({
                    url: '/IServer/Dealer/Dealer.ashx?action=DeleteDealerContact',
                    type: 'post',
                    async: false,
                    data: { PKID: rows[0].PKID },
                    dataType: 'json',
                    success: function (data) {
                        var index = parent.layer.getFrameIndex(window.name); //先得到当前iframe层的索引
                        if (data.success) {
                            layer.close(index);
                            layer.msg("删除成功");
                            $table.bootstrapTable('refresh');
                        }
                        else {
                            top.layer.close(index);
                            layer.msg("删除失败，请重试。", { time: 3000, icon: 2 });
                        }
                    }, error: function (data) {
                        top.layer.close(index);
                    }
                });
                return false;
            }, 2000);
        })
    }
}
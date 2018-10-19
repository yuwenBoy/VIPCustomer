var $table;
var URL = '/IServer/Info/InfoHandler.ashx?action=';
$(function () {
    InitMainTable();
    addRecord();
    $('#fr_Title').focus();
    $("#TypeId").bindSelect({
        url: URL + 'GetTypeListJson',
        id: "PKID",
        text: "ListName"
    });
});

function InitMainTable() {
    $table = $('#TbDC').bootstrapTable({
        url: URL + 'GetInfoListPager',                      //请求后台的URL（*）
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
             field: 'EmergencyDegreeName',
             title: '紧急程度',
             width: 100,
             sortable: true
         }, {
             field: 'Title',
             title: '标题',
             width: 300,
             sortable: true
         }, {
             field: 'TypeName',
             title: '类别',
             width: 100,
             sortable: true,
         }, {
             field: 'Author',
             title: '作者',
             width: 100,
             sortable: true

         }, {
             field: 'InDate',
             title: '生效日期',
             width: 100,
             sortable: true,
             formatter: dateFormatter
         }, {
             field: 'OutDate',
             title: '失效日期',
             width: 100,
             sortable: true,
             formatter: dateFormatter
         }, {
             field: 'ModifyDate',
             title: '修改日期',
             width: 100,
             sortable: true,
             formatter: dateupdateFormatter
         }],
        onLoadSuccess: function () {
        },
        onLoadError: function () {
            layer.msg("加载数据失败", { time: 1500, icon: 2 });
        },
        onDblClickRow: function (row, $element) {

        },
    });
};

// 格式化时间
function dateFormatter(value, row, index) {
    if (value.length > 10) {
        return value.substr(0, 10);
    }
}
function dateupdateFormatter(value, row, index) {
    if (value.length > 10) {
        var date = value.substr(0, 10);
        var dateString = date + ' ' + value.substr(11);
        var dateResult = dateString.substr(0, 19);
        return dateResult;
    }
}
// 刷新信息
function RefreshData() {
    $('#TbDC').bootstrapTable('destroy');//先要将table销毁，否则会保留上次加载的内容
    InitMainTable();//重新初使化表格。
}

// 添加
function AddDetail() {
    $('#myModal').modal('show');
    $('#Oper2').hide();
    resetForm();
}

$('#fr_Title').on('blur', function () {
    var $Title = $('#fr_Title').val();
    if ($Title.trim().length > 0) {
        $('#btn_submit').attr('disabled', false);
    }
    else {
        $('#btn_submit').attr('disabled', true);
        $('#fr_Title').focus();
    }
});

// 编辑
function EditDetail() {
    var rows = $table.bootstrapTable('getSelections');
    if (rows <= 0) {
        layer.msg('请至少选择一条数据。');
        return;
    }
    else {
        $('#Oper1').hide();
        $('#Oper2').show();
        var InfoID = rows[0].PKID
        $('#HidId').val(InfoID);
        $('#myModal').modal('show');
        resetForm();
        $.getJSON(URL + 'GetModel&InfoID=' + InfoID, function (res) {
            setInfoDetails(res);
        })
    }
}

// 将对象绑定到控件
function setInfoDetails(result) {
    $('#fr_Title').val(result.Title);
    $('#fr_TypeId').val(result.TypeID).trigger("change");
    if ($('#fr_InDate').length > 0) {
        $('#fr_InDate').val(result.InDate.substr(0, 10));
    }
    if ($('#fr_OutDate').length > 0) {
        $('#fr_OutDate').val(result.OutDate.substr(0, 10));
    }
    $('#Form1').formSerialize(result);
}

// 重置表单
function resetForm() {
    var date = new Date();
    $('#fr_InDate').val(date.Format('yyyy-MM-dd'));
    var month = (date.getMonth() > 9 ? (date.getMonth() + 2) : '0' + (date.getMonth() + 02));
    $('#fr_OutDate').val(date.Format('yyyy') + '-' + month + '-' + date.Format('dd'));
    $("#fr_TypeId").bindSelect({
        url: URL + 'GetTypeListJson',
        id: "PKID",
        text: "ListName"
    });

    $("#EmergencyDegreeID").bindSelect({
        url: URL + 'GetEmergencyDegreeListJson',
        id: "PKID",
        text: "ListName"
    });
    $('#fr_Title').val('');
    $('#Contents').val('');
};

// 添加数据
function addRecord() {
    $('#Form1').bootstrapValidator({
        message: 'This value is not valid',
        feedbackIcons: {/*input状态样式图片*/
            valid: 'glyphicon glyphicon-ok',
            invalid: 'glyphicon glyphicon-remove',
            validating: 'glyphicon glyphicon-refresh'
        },
        fields: {/*验证：规则*/
            fr_Title: {//验证input项：验证规则
                message: '标题不能为空',
                validators: {
                    notEmpty: {//非空验证：提示消息
                        message: '标题不能为空'
                    }
                }
            }
        }
    }).on('success.form.bv', function (e) {//点击提交之后
        e.preventDefault();    // Prevent form submission
        var $form = $(e.target);      // Get the form instance
        var beginTime = $('#fr_InDate').val();
        var endTime = $('#fr_OutDate').val();
        var json = "{";
        json = json + String.format("TypeID:\"{0}\",EmergencyDegreeID:\"{1}\",Title:\"{2}\",InDate:\"{3}\",OutDate:\"{4}\",Contents:\"{5}\"",
                                       $("#fr_TypeId").val(), $("#EmergencyDegreeID").val(), $("#fr_Title").val(), beginTime, endTime, $("#Contents").val());
        json = json + "}";
        var SaveLoading = layer.msg('数据提交中，请稍候', { icon: 16, time: true, shade: 0.8 });
        $.ajax({
            url: $form.attr('action'),
            type: 'post',
            data: { param: json },
            dataType: 'json',
            success: function (result) {
                if (result.success) {
                    layer.close(SaveLoading);
                    layer.msg("添加成功", { time: 1500, icon: 1 });
                    $('#myModal').modal('hide');
                    RefreshData();
                    $form.bootstrapValidator('resetForm');
                    resetForm();
                }
                else {
                    layer.close(SaveLoading);
                    layer.msg("添加失败，请重试。", { time: 3000, icon: 2 });
                }
            },
            error: function () { }
        })
    });
    // 关闭
    $('#btnClose').on('click', function () {
        $('#Form1').bootstrapValidator('resetForm');
        $('#myModal').modal('hide');
    });
}

// 更新数据
function updateRecord() {
    var beginTime = $('#fr_InDate').val();
    var endTime = $('#fr_OutDate').val();
    var json = "{";
    json = json + String.format("TypeID:\"{0}\",EmergencyDegreeID:\"{1}\",Title:\"{2}\",InDate:\"{3}\",OutDate:\"{4}\",Contents:\"{5}\",PKID:\"{6}\"",
                                   $("#fr_TypeId").val(), $("#EmergencyDegreeID").val(), $("#fr_Title").val(), beginTime, endTime, $("#Contents").val(), $('#HidId').val());
    json = json + "}";
    var SaveLoading = layer.msg('数据提交中，请稍候', { icon: 16, time: true, shade: 0.8 });
    $.ajax({
        url: URL + 'UpdateRecord',
        type: 'post',
        data: { param: json },
        dataType: 'json',
        success: function (result) {
            if (result.success) {
                layer.close(SaveLoading);
                layer.msg("更新成功", { time: 1500, icon: 1 });
                $('#myModal').modal('hide');
                RefreshData();
                $form.bootstrapValidator('resetForm');
                resetForm();
            }
            else {
                layer.close(SaveLoading);
                layer.msg("添加失败，请重试。", { time: 3000, icon: 2 });
            }
        },
        error: function () { }
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
        layer.confirm('是否要删除选中行数据？ ', { icon: 3, title: '提示信息' }, function (index) {
            var index = layer.msg('正在删除，请稍候', { icon: 16, time: false, shade: 0.8 });
            setTimeout(function () {
                $.ajax({
                    url: URL + 'DELETEDATE',
                    type: 'post',
                    async: false,
                    data: { PKID: rows[0].PKID },
                    dataType: 'json',
                    success: function (data) {
                        var index = layer.getFrameIndex(window.name); //先得到当前iframe层的索引
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
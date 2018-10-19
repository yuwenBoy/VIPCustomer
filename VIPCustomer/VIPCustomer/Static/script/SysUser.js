$(function () {
    InitMainTable();
    SaveData();

})
var $table;
var URL = '/IServer/SysManage/SysManage.ashx?action=';

function InitMainTable() {
    $table = $('#TbDC').bootstrapTable({
        url: URL + 'GetUserManagePager',                      //请求后台的URL（*）
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
             field: 'LoginName',
             title: '用户名称',
             sortable: true
         }, {
             field: 'Name',
             title: '用户姓名',
             sortable: true
         }, {
             field: 'DealerName',
             title: '所属经销店',
             sortable: true,
         }, {
             field: 'UserTypeName',
             title: '用户类别',
             sortable: true

         }, {
             field: 'Email',
             title: 'Email',
             sortable: true
         }, {
             field: 'Phone',
             title: '联系电话',
             sortable: true
         }, {
             field: 'IsActivate',
             title: '是否启用',
             sortable: true,
             formatter: IsActivateFormatter
         }, {
             field: 'Remark',
             title: '说明',
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
            $('#myModal').modal('show');
            $('#lblAddTitle').text('修改');
            bindController(id);
        },
    });

    $.getJSON(URL + 'GetUserType', function (json) {
        for (var i = 0; i < json.length; i++) {
            $('select[name=UserTypeId]').append("<option value='" + json[i].UserTypeId + "'>" + json[i].UserTypeName + "</option>");
        }
    });

    $('input[name=IsActivate]').click(function () {
        if ($(this).is(':checked')) {
            $(this).val("1");
        }
        else {
            $(this).val("0");
        }
    });
};

// 格式化用户启用状态列
function IsActivateFormatter(value, row, index) {
    if (value == 1) {
        return '<input type="checkbox" value="1" checked="true" disabled="disabled"/>';
    } else {
        return '<input type="checkbox" value="0" disabled="disabled"/>';
    }
}

// 刷新信息
function RefreshData() {
    $('#TbDC').bootstrapTable('destroy');//先要将table销毁，否则会保留上次加载的内容
    InitMainTable();//重新初使化表格。
}

// 重置
function reset() {
    $('input[name=LoginName]').val('');
    $('input[name=Name]').val('');
    $('select[name=UserTypeId]').val(-1);
    $('select[name=IsActivate]').val(-1);
}

// 查询
function btn_search() {
    RefreshData();
}

// 新增
function AddDetail() {
    $('#myModal').modal('show');
    $('#lblAddTitle').text('添加');
    $('#Form1').bootstrapValidator('resetForm');
}

// 获取经销店名称
$.getJSON(URL + 'GetDealerAll', function (result) {
    for (var i = 0; i < result.length; i++) {
        $('select[name=DealerId]').append("<option value='" + result[i].PKID + "'>" + result[i].Name + "</option>");
    }
});

// 获取用户类别
$.post(URL + 'GetUserType', function (data) {
    var json = $.parseJSON(data);
    for (var i = 0; i < json.length; i++) {
        $('select[name=fr_UserTypeId]').append("<option value='" + json[i].UserTypeId + "'>" + json[i].UserTypeName + "</option>");
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
        // 编辑状态下把主键ID赋值给隐藏域，方便后台接收处理
        $('#HidId').val(rows[0].PKID);
        $('#myModal').modal('show');
        $('#lblAddTitle').text('修改');
        bindController(rows[0].PKID);
    }
}

// 编辑将对象绑定到控件
function bindController(Id) {
    $.getJSON(URL + 'GetSysUserByPKID&keyword=' + Id, function (res) {
        $('input[name=fr_LoginName]').val(res.LoginName).attr({ readonly: 'true' });
        $('input[name=fr_Name]').val(res.Name);
        $('select[name=fr_UserTypeId]').val(res.UserTypeId);
        $('input[name=Phone]').val(res.Phone);
        $('input[name=Email]').val(res.Email);
        $('select[name=DealerId]').val(res.DealerId);
        $('input[name=LoginPwd]').val(res.LoginPwd);
        $('textarea[name=Remark]').val(res.Remark);
        $('input[name=IsActivate]').val(res.IsActivate);
        res.IsActivate == 0 ? $('input[name=IsActivate]').attr('checked', false) : $('input[name=IsActivate]').attr('checked', true);
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
            fr_LoginName: {//验证input项：验证规则
                message: '用户名称不能为空',
                validators: {
                    notEmpty: {//非空验证：提示消息
                        message: '用户名称不能为空'
                    }
                }
            },
            fr_Name: {
                message: '用户姓名不能为空',
                validators: {
                    notEmpty: {
                        message: '用户姓名不能为空'
                    },

                }
            }
        }
    }).on('success.form.bv', function (e) {//点击提交之后
        e.preventDefault();    // Prevent form submission
        var $form = $(e.target);      // Get the form instance
        $.post($form.attr('action'), $form.serializeObject(), function (result) {
            var SaveLoading =  layer.msg('数据提交中，请稍候', { icon: 16, time: true, shade: 0.8 });
            var json = JSON.parse(result);  // 由json字符串转换为json对象
            if (json.success) {
                layer.close(SaveLoading);
                layer.msg("保存成功，默认密码为123", { time: 1500, icon: 1 });
                RefreshData();
                $('#myModal').modal('hide');
                $form.bootstrapValidator('resetForm');
                clearForm($($form));
            }
            else if (json.status == "1") {
                layer.alert("用户名称【" + json.msg + "】已存在，请修改");
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
        layer.confirm('您确定要删除用户【' + rows[0].LoginName + '】吗？ ', { icon: 3, title: '提示信息' }, function (index) {
            if (rows[0].IsSystemUser == 1) {
                layer.alert('抱歉，系统用户【' + rows[0].LoginName + '】不能删除,请重新选择数据。');
                return;
            }
            else {
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
            }
        })
    }
}

var $tb2 = '';
var index = '';
var $tb2_td_PKID = '';
var $tb2_td_LeftName = '';


// 设置角色
function setRole() {
    var rows = $table.bootstrapTable('getSelections');
    if (rows <= 0) {
        layer.msg('请至少选择一条数据。');
        return;
    }
    else {
        $('#myModal2').modal('show');
        $('#HidId').val(rows[0].PKID);// 用于保存角色
        TB2();
        bindUserRoleDataTable(rows[0].PKID);
    }
}

// 绑定角色列表
function TB2() {
    var queryUrl = URL + 'GetRoleList';
    $tb2 = $('#TB2').bootstrapTable({
        columns: [
             {
                 field: 'PKID',
                 title: 'PKID',
                 visible: false
             },
         {
             field: 'Name',
             title: '名称',
             sortable: true
         }],
        url: queryUrl,
        striped: true,//是否显示行间隔色
        height: 420,
        onLoadError: function () {
            layer.msg("加载数据失败", { time: 1500, icon: 2 });
        },
        onDblClickRow: function (row, $element) {
            $($element).addClass("backcolor").siblings("tr").removeClass("backcolor");
            index = $("#TB3 tbody").find('tr').length;
            tr = $("#TB3 tr td");
            let isExist = false;// 声明变量，append数据时用于判断右侧表格是否存在左侧表格里的数据
            if (tr.length == 0) {
                isExist = true;
            }
            else {
                for (var i = 0; i < tr.length; i++) {
                    let rightName = tr.eq(i).text();
                    if (row.PKID == rightName) {
                        isExist = false;
                        break;
                    }
                    else {
                        isExist = true;
                    }
                };
            }
            if (isExist) {
                $("#TB3 tbody").append('<tr data-index="' + index++ + '"><td style="display:none;">' + row.PKID + '</td><td style="width: 320px;;height: 38px;">' + $element.text() + '</td></tr>');
            }
        },
        onClickRow: function (row, $element) {
            $($element).addClass("backcolor").siblings("tr").removeClass("backcolor");
            $tb2_td_PKID = row.PKID;
            $tb2_td_LeftName = $element.text();
        }
    });
}

// 单个右移
function MoveRigth() {
    var len = $('#TB2 tbody tr.backcolor').length;
    if (len == 0) {
        return;
    }
    else {
        index = $("#TB3 tbody").find('tr').length;
        tr = $("#TB3 tr td");
        let isExist = false;// 声明变量，append数据时用于判断右侧表格是否存在左侧表格里的数据
        if (tr.length == 0) {
            isExist = true;
        }
        else {
            for (var i = 0; i < tr.length; i++) {
                let rightName = tr.eq(i).text();
                if ($tb2_td_PKID == rightName) {
                    isExist = false;
                    break;
                }
                else {
                    isExist = true;
                }
            };
        }
        if (isExist) {
            $("#TB3 tbody").append('<tr data-index="' + index++ + '"><td style="display:none;">' + $tb2_td_PKID + '</td><td style="width: 320px;;height: 38px;">' + $tb2_td_LeftName + '</td></tr>');
        }
    }
}

// 全部右移
function AllMoveRigth() {

}

// 单个移除
function MoveLeft() {
    var len = $('#TB3 tbody tr.backcolor').length;
    if (len == 0) {
        return;
    }
    else {
        $('#TB3 tbody tr.backcolor').remove();
    }
}

// 全部移除
function AllMoveLeft() {
    $('#TB3 tbody tr').remove();
}

// 绑定用户角色列表
function bindUserRoleDataTable(keyword) {
    $.getJSON(URL + 'GetRoleByUserId&keyword=' + keyword, function (res) {
        let strHtml = "";
        if (res != null) {
            for (var i = 0; i < res.length; i++) {
                strHtml += '<tr><td style="display:none;">' + res[i].PKID + '</td><td style="width: 320px;;height: 38px;">' + res[i].Name + '</td></tr>';
                $("#TB3 tbody").html(strHtml);
            }
        }
        return;
    });
    $('#TB3 tbody').on('click dblclick', 'tr', function () {
        $(this).addClass("backcolor").siblings("tr").removeClass("backcolor");
    });
    $('#TB3 tbody').on('dblclick', 'tr', function () {
        $('#TB3 tbody tr.backcolor').remove();
    });
}

// 保存数据
function SaveUserRole() {
    var roleid = "";
    $("#TB3 tr :nth-child(1)").each(function (i, dom) {
        roleid += "," + $(this).html()
    });
    var roleIds = roleid.substring(4);
    var keyword = {
        userId: $('#HidId').val(),
        roleIds: roleIds
    };
    $.post(URL + 'SubmitUserRoleData', keyword, function (result) {
        var SaveLoading =  layer.msg('数据提交中，请稍候', { icon: 16, time: true, shade: 0.8 });
        var json = JSON.parse(result);  // 由json字符串转换为json对象
        if (json.success) {
           layer.close(SaveLoading);
            layer.msg("保存成功", { time: 1500, icon: 1 });
            RefreshData();
            $('#myModal2').modal('hide');
        }
        else {
           layer.close(SaveLoading);
            layer.msg("保存失败，请重试。", { time: 3000, icon: 2 });
        }
    });
}

// 关闭设置角色弹层
function setRoleHide() {
    $('#myModal2').modal('hide');
}

































//var table;
//// opType操作变量说明:3=编辑 5=设置角色,1=添加
//function initPage() {
//    table = $('#example').DataTable({
//        "oLanguage": {
//            "sLengthMenu": "每页显示 _MENU_ 条记录",
//            "sZeroRecords": "抱歉， 没有找到",
//            "sInfo": "从 _START_ 到 _END_ /共 _TOTAL_ 条数据",
//            "sInfoEmpty": "没有数据",
//            "sInfoFiltered": "(从 _MAX_ 条数据中检索)",
//            "oPaginate": {
//                "sFirst": "首页",
//                "sPrevious": "上一页",
//                "sNext": "下一页",
//                "sLast": "尾页"
//            },
//            "sZeroRecords": "没有检索到数据",
//            "sProcessing": "<img src='../images/loading-0.gif' /> 努力加载数据中",
//        },
//        "aaSorting": [[1, "desc"]],
//        "bProcessing": true, //当datatable获取数据时候是否显示正在处理提示信息。
//        "bLengthChange": false, //改变每页显示数据数量
//        "bAutoWidth": true,//自动宽度
//        "bFilter": false,    //过滤禁用
//        "iDisplayLength": 50,  // 每页显示多少条数据
//        "serverSide": true,
//        'sClass': "text-center",
//        //"scrollX": false,
//        //"scrollY": false,
//        "ajax": {
//            "url": "../Tools/handlerDate.ashx?action=GetSysUserInfo",
//            "type": "post",
//            "data": function (d) {
//                d.search = GetSearchString();
//            }
//        },
//        "columns": [
//       { "data": "PKID" },
//       { "data": "LoginName" },
//       { "data": "Name" },
//       { "data": "DealerName" },
//       { "data": "UserTypeName" },
//       { "data": "Email" },
//       { "data": "Phone" },
//       { "data": "IsActivate" },
//       { "data": "Remark" },
//        ],
//        "columnDefs": [
//             {
//                 targets: 0,
//                 render: function (data, type, row, meta) {
//                     return '<input type="checkbox" name="checklist" class="Selected" value="' + row.id + '" />'
//                 }
//             }, { "orderable": false, "targets": 0 },
//             {
//                 "bVisible": false, "targets": 0
//             },
//             {
//                 "targets": 2,
//                 "render": function (data, type, row, meta) {
//                     return data.length > 30 ? data.substr(0, 30) + "..." : data;
//                 }
//             },

//        ],
//        "fnRowCallback": function (nRow, aData, iDisplayIndex) {
//            var strHtml = "";
//            if (aData["IsActivate"] == "1") {
//                $('td:eq(6)', nRow).html('<input type="checkbox" checked="checked" disabled="disabled">');
//            }
//            else {
//                $('td:eq(6)', nRow).html('<input type="checkbox"  disabled="disabled">');
//            }
//            return nRow;
//        }
//    });
//    // 获取用户类别
//    $.post('../Tools/handlerDate.ashx?action=GetUserType', function (data) {
//        var json = $.parseJSON(data);
//        for (var i = 0; i < json.length; i++) {
//            $('#UserTypeId').append("<option value='" + json[i].UserTypeId + "'>" + json[i].UserTypeName + "</option>");
//        }
//    });

//    $("#editable-search").click(function () {
//        ajaxRedrawDataTables();
//    });

//    $('#example tbody').on('click', 'tr', function () {
//        if ($(this).hasClass('selected')) {
//            $(this).removeClass('selected');
//            $(this).addClass('selected');
//        }
//        else {
//            table.$('tr.selected').removeClass('selected');
//            $(this).addClass('selected');
//        }
//    });
//}

//function GetSearchString() {
//    var loginName = $('#LoginName').val().trim();
//    var name = $('#Name').val().trim();
//    var userTypeId = $('#UserTypeId').val();
//    var isActivate = $('#IsActivate').val();
//    var strwhere = $('#strWhere').val();
//    var jsonString = JSON.stringify({ "LoginName": loginName, "Name": name, "UserTypeId": userTypeId, "IsActivate": isActivate, "strWhere": strwhere });
//    return jsonString;
//}

//function ajaxRedrawDataTables() {
//    table.rows().draw(false);
//}

//// 接收地址栏参数
//function getUrlParam(name) {
//    var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)"); //构造一个含有目标参数的正则表达式对象
//    var r = window.location.search.substr(1).match(reg);  //匹配目标参数
//    if (r != null) return unescape(r[2]); return null; //返回参数值
//}

//// 加载保存数据页面
//function InitPageSaveDate() {
//    // 获取经销店名称
//    $.post('../Tools/handlerDate.ashx?action=GetDealerAll', function (data) {
//        var json = $.parseJSON(data);
//        for (var i = 0; i < json.length; i++) {
//            $('#DealerId').append("<option value='" + json[i].PKID + "'>" + json[i].Name + "</option>");
//        }
//    });

//    // 获取用户类别
//    $.post('../Tools/handlerDate.ashx?action=GetUserType', function (data) {
//        var json = $.parseJSON(data);
//        for (var i = 0; i < json.length; i++) {
//            $('#UserTypeId').append("<option value='" + json[i].UserTypeId + "'>" + json[i].UserTypeName + "</option>");
//        }
//    });

//    var opType = $('#opType', window.parent.document).val();
//    if (3 == opType) {
//        var PKID = getUrlParam("PKID");
//        var param = { action: 'GetSysUserByPKID', pkid: PKID };
//        $.ajax({
//            url: '../Tools/handlerDate.ashx',
//            type: 'post',
//            data: param,
//            dataType: 'json',
//            success: function (data) {
//                $('#LoginName').val(data.LoginName);
//                $("#LoginName").attr({ readonly: 'true' });
//                $('#Name').val(data.Name);
//                $('#LoginPwd').val(data.LoginPwd);
//                $('#Email').val(data.Email);
//                $('#LoginPwd1').val(data.LoginPwd);
//                $('#Phone').val(data.Phone);
//               $('#DealerId').val(data.DealerId);// 经销店
//                //$('#DealerId').append("<option value='" + data.DealerId + "'>" + data.Name + "</option>");
//                //$('select[name=DealerId]').append("<option value='" + data.ProvinceID + "'>" + data.Province + "</option>");
//                $('#UserTypeId').val(data.UserTypeId);// 用户类型
//                if (data.IsActivate == 1) {
//                    $('#IsActivate').attr('checked', true);
//                    $('#IsActivate').val("1");
//                }
//                else {
//                    $('#IsActivate').attr('checked', false);
//                    $('#IsActivate').val("0");
//                }
//                $('#Remark').val(data.Remark);
//            }
//        });
//    }
//    else {
//    }

//    $('#Form1').bootstrapValidator({
//        message: 'This value is not valid',
//        feedbackIcons: {
//            valid: 'glyphicon glyphicon-ok',
//            invalid: 'glyphicon glyphicon-remove',
//            validating: 'glyphicon glyphicon-refresh'
//        },
//        fields: {
//            LoginName: {
//                message: '用户名称验证失败',
//                validators: {
//                    notEmpty: {
//                        message: '用户名称不能为空'
//                    }
//                }
//            },
//            Name: {
//                message: '用户姓名验证失败',
//                validators: {
//                    notEmpty: {
//                        message: '用户姓名不能为空'
//                    }
//                }
//            }
//        }
//    }).
//        on('success.form.bv', function (e) {//点击提交之后
//            e.preventDefault();
//            var $form = $(e.target);
//            var bv = $form.data('bootstrapValidator');
//            var PKID = getUrlParam("PKID");
//            var param = {
//                opType: opType, PKID: PKID, LoginName: $('#LoginName').val(),
//                Name: $('#Name').val(), LoginPwd1: $('#LoginPwd1').val(),
//                Email: $('#Email').val(), Phone: $('#Phone').val(),
//                DealerId: $('#DealerId').val(),
//                UserTypeId: $('#UserTypeId').val(),
//                IsActivate: $('#IsActivate').val(), Remark: $('#Remark').val()
//            };
//            $.ajax({
//                url: $form.attr('action'),
//                type: 'post',
//                data: param,
//                dataType: 'json',
//                success: function (result) {
//                    if (result.status == "1") {
//                        layer.alert("用户名称【" + result.msg + "】已存在，请修改");
//                    }
//                    else if (result.success) {
//                        layer.alert(result.msg);
//                        setTimeout(function () {
//                            parent.layer.closeAll();
//                            parent.location.reload();
//                        }, 1000);
//                    }
//                    else {
//                        layer.alert(result.msg);
//                    }
//                },
//                error: function (jqXHR, textStatus, errorThrown) {
//                    layer.alert("程序执行过程中发生如下错误:" + errorThrown)
//                }
//            })
//        });

//    $('#IsActivate').click(function () {
//        if ($(this).is(':checked')) {
//            $(this).val("1");
//        }
//        else {
//            $(this).val("0");
//        }
//    });
//}

//// 点击添加按钮
//$("#editable-add").click(function () {
//    layer.open({
//        title: '添加用户信息',
//        scrollbar: false,
//        type: 2,
//        maxmin: true,
//        area: ['1000px', '500px'],
//        content: '../Page/SysUserEdit.html',
//        end: function () {
//            // parent.location.reload();
//        }
//    });
//    $('#opType').val("1");
//});

//// 编辑用户
//function Edit_Click() {
//    var rows = table.rows('.selected').data().length;
//    if (rows < 1) {
//        layer.alert("请至少选择一条记录进行操作。");
//        return;
//    }
//    else {
//        var PKID = table.row('.selected').data().PKID;
//        $('#opType').val("3");
//        layer.open({
//            title: '编辑用户信息',
//            type: 2,
//            scrollbar: false,
//            maxmin: true,
//            area: ['1000px', '500px'],
//            content: '../Page/SysUserEdit.html?PKID=' + PKID,
//            end: function () {
//                // parent.location.reload();
//            }
//        });
//    }
//}

//// 删除数据
//function Delete_Click() {
//    var rows = table.rows('.selected').data().length;
//    if (rows < 1) {
//        layer.alert("请至少选择一条记录进行操作。");
//        return;
//    }
//    else {
//        layer.confirm('您确定要删除用户【' + table.row('.selected').data().LoginName + '】吗？ ', function () {
//            var PKID = table.row('.selected').data().PKID;
//            var param = { action: 'DELETEDATE', PKID: PKID };
//            $.ajax({
//                url: '../Tools/handlerDate.ashx',
//                type: 'post',
//                data: param,
//                dataType: 'json',
//                success: function (d) {
//                    if (d.success) {
//                        setTimeout(function () {
//                            layer.alert(d.msg);
//                            ajaxRedrawDataTables();
//                        }, 1000)
//                    }
//                    else {
//                        layer.alert(d.msg);
//                    }
//                },
//                error: function (jqXHR, textStatus, errorThrown) {
//                    layer.alert("程序执行过程中发生如下错误:" + errorThrown)
//                }
//            })
//        })
//    }
//}

//var RoleTable;// 角色列表

//// 保存用户角色
//function SaveUserRoleDealer_Click() {
//    var rows = table.rows('.selected').data().length;
//    if (rows < 1) {
//        layer.alert("请至少选择一条记录进行操作。");
//        return;
//    }
//    else {
//        var userID = table.row('.selected').data().PKID;
//        $('#opType').val("5");
//        layer.open({
//            type: 2,
//            title: '选择用户角色',
//            scrollbar: false, // 父页面 滚动条 禁止  
//            maxmin: true,
//            area: ['800px', '680px'],
//            content: '../Page/SetRoleList.html?userId=' + userID,
//            end: function () {
//                // parent.location.reload();
//            }
//        });
//    }
//}

//function LoadPageSetRoleList() {
//    RoleTable = $('#RoleTable').DataTable({
//        "oLanguage": {
//            "sLengthMenu": "每页显示 _MENU_ 条记录",
//            "sZeroRecords": "抱歉， 没有找到",
//            "sInfo": "从 _START_ 到 _END_ /共 _TOTAL_ 条数据",
//            "sInfoEmpty": "没有数据",
//            "sInfoFiltered": "(从 _MAX_ 条数据中检索)",
//            "oPaginate": {
//                "sFirst": "首页",
//                "sPrevious": "上一页",
//                "sNext": "下一页",
//                "sLast": "尾页"
//            },
//            "sZeroRecords": "没有检索到数据",
//            "sProcessing": "<img src='../images/loading-2.gif' /> 努力加载数据中",
//        },
//        "ordering": false,// 禁用排序
//        "bProcessing": true, //当datatable获取数据时候是否显示正在处理提示信息。
//        "bLengthChange": false, //改变每页显示数据数量
//        "bAutoWidth": false,//自动宽度
//        "bFilter": false,    //过滤禁用
//        "iDisplayLength": 10,  // 每页显示多少条数据
//        "serverSide": true,
//        "scrollCollapse": true,
//        "sScrollY": "40%", "sScrollX": "1000px",
//        "ajax": {
//            "url": "../Tools/handlerDate.ashx?action=GetRoleList",
//            "type": "post",
//            "data": function (d) {
//            }
//        },
//        //要與這裡匹配
//        "columns": [
//            { "data": "PKID" },
//            { "data": "Name" },
//        ],
//        "columnDefs": [
//            {
//                "bVisible": false, "targets": 0,
//            }
//        ],
//    });

//    $('#RoleTable tbody').on('click dblclick', 'tr', function () {
//        if ($(this).hasClass('selected')) {
//            $(this).removeClass('selected');
//        }
//        else {
//            RoleTable.$('tr.selected').removeClass('selected');
//            $(this).addClass('selected');
//        }
//    });
//    var param = { action: 'GetRoleByUserId', userId: getUrlParam("userId") };
//    var UserRoleTable = $.ajax({
//        url: '../Tools/handlerDate.ashx',
//        type: 'post',
//        data: param,
//        dataType: 'json',
//        success: function (data) {
//            let strHtml = "";
//            if (data != null) {
//                for (var i = 0; i < data.length; i++) {
//                    strHtml += '<tr><td style="display:none;">' + data[i].PKID + '</td><td style="width: 1%;height: 38px;">' + data[i].Name + '</td></tr>';
//                    $("#UserRoleTable tbody").html(strHtml);
//                }
//            }
//            return;
//        }
//    });

//    $('#UserRoleTable tbody').on('click dblclick', 'tr', function () {
//        if ($(this).hasClass('selected')) {
//            $(this).removeClass('selected');
//        }
//        else {
//            $('#UserRoleTable tbody tr.selected').removeClass('selected');
//            $(this).addClass('selected');
//        }
//    });

//    // 单个右移双击行事件
//    $('#RoleTable tbody').on('dblclick', 'tr', function () {
//        let roleName = $("#RoleTable .selected td").html();
//        let roleId = RoleTable.row('.selected').data().PKID;
//        if (roleName == undefined) {
//            return;
//        }
//        let userRoleList = $("#UserRoleTable tr td");
//        let isExist = false;
//        if (userRoleList.length == 0) {
//            isExist = true;
//        } else {
//            for (var i = 0; i < userRoleList.length; i++) {
//                let userRole = userRoleList.eq(i).html();
//                if (roleName == userRole) {
//                    isExist = false;
//                    break;
//                }
//                else {
//                    isExist = true;
//                }
//            }
//        }
//        if (isExist) {
//            let strRole = "<tr><td style='display:none;'>" + roleId + "</td><td style='width: 1%;height: 38px;'>" + roleName + "</td></tr>";
//            $("#UserRoleTable tbody").append(strRole);
//        }
//    });

//    // 单个右移
//    $('#SelRightMove').on('click', function () {
//        let roleName = $("#RoleTable .selected td").html();
//        let roleId = RoleTable.row('.selected').data().PKID;
//        if (roleName == undefined) {
//            return;
//        }
//        let userRoleList = $("#UserRoleTable tr td");
//        let isExist = false;
//        if (userRoleList.length == 0) {
//            isExist = true;
//        } else {
//            for (var i = 0; i < userRoleList.length; i++) {
//                let userRole = userRoleList.eq(i).html();
//                if (roleName == userRole) {
//                    isExist = false;
//                    break;
//                }
//                else {
//                    isExist = true;
//                }
//            }
//        }
//        if (isExist) {
//            let strRole = "<tr><td style='display:none;'>" + roleId + "</td><td style='width: 1%;height: 38px;'>" + roleName + "</td></tr>";
//            $("#UserRoleTable tbody").append(strRole);
//        }
//    });

//    // 全部右移
//    $('#AllRightMove').on('click', function () {
//        var rows = RoleTable.rows('.selected').data().length;
//        if (rows < 1) {
//            return;
//        }
//        else {

//        }
//    });

//    // 双击行事件【左移】
//    $('#UserRoleTable tbody').on('dblclick', 'tr', function () {
//        let userRole = $('#UserRoleTable .selected td').html();
//        if (userRole == undefined) {
//            return;
//        }
//        else {
//            $('#UserRoleTable .selected td').remove();
//        }
//    });

//    // 单个左移
//    $('#SelLeftMove').on('click', function () {
//        let userRole = $('#UserRoleTable .selected td').html();
//        if (userRole == undefined) {
//            return;
//        }
//        else {
//            $('#UserRoleTable .selected td').remove();
//        }
//    });

//    // 全部左移
//    $('#AllLeftMove').on('click', function () {
//        let userRole = $('#UserRoleTable .selected td').html();
//        if (userRole == undefined) {
//            return;
//        }
//        else {
//            $('#UserRoleTable tr td').remove();
//        }
//    });

//    // 选择保存用户角色
//    $('#btn_Save').on('click', function () {
//        var roleId = "";
//        $("#UserRoleTable tr :nth-child(1)").each(function (i, dom) {
//            roleId += "," + $(this).html()
//        });
//        var strRoleId = roleId.substring(6);
//        var opType = $('#opType', window.parent.document).val();
//        if (opType == 5) {
//            var param = { opType: opType, action: 'SaveData', userId: getUrlParam('userId'), RoleIds: strRoleId };
//            $.ajax({
//                url: '../Tools/handlerDate.ashx',
//                type: 'post',
//                data: param,
//                dataType: 'json',
//                success: function (res) {
//                    if (res.success) {
//                        setTimeout(function () {
//                            layer.alert(res.msg);
//                        }, 1000);
//                        parent.layer.closeAll();
//                        parent.location.reload();
//                    }
//                    else {
//                        layer.alert(res.msg);
//                    }
//                },
//                error: function (jqXHR, textStatus, errorThrown) {
//                    layer.alert("程序执行过程中发生如下错误:" + errorThrown)
//                }
//            })
//        }
//    })
//}
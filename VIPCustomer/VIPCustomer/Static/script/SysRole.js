$(function () {
    $('.col-lg-2').hide();
    $('.col-lg-10').css('width', '100%');
    InitMainTable();
    SaveData();
});

var $table;
var zTree;
var RoleId = null;
var moduleId = null;
function InitMainTable() {
    //记录页面bootstrap-table全局变量$table，方便应用
    var queryUrl = '/IServer/SysManage/SysManage.ashx?action=GetRoleManagePager';
    $table = $('#tbRole').bootstrapTable({
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
             title: '角色名称',
             sortable: true,
             width: 300,
         }, {
             field: 'RoleGrade',
             title: '角色等级',
             sortable: true,
             width: 100
         }, {
             field: 'IsActivate',
             title: '是否启用',
             sortable: true,
             width: 100,
             formatter: IsActivateFormatter
         }, {
             field: 'Remark',
             title: '说明',
             sortable: true,
         }],
        onLoadError: function () {
            layer.msg("加载数据失败", { time: 1500, icon: 2 });
        },
        onDblClickRow: function (row, $element) {
            RoleId = row.PKID;
            $('.col-lg-10').css('width', 83.33333333 + '%');
            $('.panel-title').find('a').eq(2).css('margin-left', 718 + 'px');
            $('.col-lg-2').show();
            Authority.RoleModuleTree(RoleId);
        },
        onClickRow: function (row, $element) {
            $('.col-lg-2').hide();
            $('.col-lg-10').css('width', '100%');
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

// 格式化启用状态列
function IsActivateFormatter(value, row, index) {
    if (value == 1) {
        return '<input type="checkbox" value="1" checked="true" disabled="disabled"/>';
    } else {
        return '<input type="checkbox" value="0" disabled="disabled"/>';
    }
}

// 刷新信息
function RefreshData() {
    $('#tbRole').bootstrapTable('destroy');//先要将table销毁，否则会保留上次加载的内容
    InitMainTable();//重新初使化表格。
}

// 重置
function reset() {
    $('input[name=Name]').val('');
}

// 查询
function btn_search() {
    RefreshData();
}

// 新增
function AddDetail() {
    $('#myModal').modal('show');
    $('#Form1').bootstrapValidator('resetForm');
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
        $('.modal-title').text('修改');
        bindController(rows[0].PKID);
    }
}

// 编辑将对象绑定到控件
function bindController(Id) {
    $.getJSON('/IServer/SysManage/SysManage.ashx?action=GetRoleByPKID&keyword=' + Id, function (res) {
        $('input[name=fr_Name]').val(res.Name);
        $('input[name=RoleGrade]').val(res.RoleGrade);
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
            fr_Name: {//验证input项：验证规则
                message: '角色名称不能为空',
                validators: {
                    notEmpty: {//非空验证：提示消息
                        message: '角色名称不能为空'
                    }
                }
            },
            RoleGrade: {
                message: '角色等级不能为空',
                validators: {
                    notEmpty: {
                        message: '角色等级不能为空'
                    },

                }
            }
        }
    }).on('success.form.bv', function (e) {//点击提交之后
        e.preventDefault();    // Prevent form submission
        var $form = $(e.target);      // Get the form instance
        // Use Ajax to submit form data 提交至form标签中的action，result自定义
        $.post($form.attr('action'), $form.serializeObject(), function (result) {
            var SaveLoading =  layer.msg('数据提交中，请稍候', { icon: 16, time: true, shade: 0.8 });
            var json = JSON.parse(result);  // 由json字符串转换为json对象
            if (json.success) {
                layer.close(SaveLoading);
                layer.msg("保存成功", { time: 1500, icon: 1 });
                RefreshData();
                $('#myModal').modal('hide');
                $('#Form1').bootstrapValidator('resetForm');
                clearForm($($form));
            }
            else if (json.status == "1") {
                layer.alert("角色名称【" + json.msg + "】已存在，请修改");
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
        layer.confirm('您确定要删除角色【' + rows[0].Name + '】吗？ ', { icon: 3, title: '提示信息' }, function (index) {
            var index = layer.msg('正在删除，请稍候', { icon: 16, time: false, shade: 0.8 });
            setTimeout(function () {
                $.ajax({
                    url: '/IServer/SysManage/SysManage.ashx?action=DeleteRole',
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

//=====================设置权限Begin=========================
// 设置权限
function SetAuthority() {
    var rows = $table.bootstrapTable('getSelections');
    if (rows <= 0) {
        layer.msg('请至少选择一条数据。');
        return;
    }
    else {
        RoleId = rows[0].PKID;
        $('.col-lg-10').css('width', 83.33333333 + '%');
        $('.panel-title').find('a').eq(2).css('margin-left', 718 + 'px');
        $('.col-lg-2').show();
        Authority.RoleModuleTree(RoleId);
    }
}

// 设置权限
var Authority = {
    // 加载角色模块树
    RoleModuleTree: function (roleId) {
        var setting = {
            check: {
                enable: true,
                chkStyle: "checkbox",
            },
            data: {
                key: {
                    name: "name", //如果命名就是"name"，此处可以不写  
                    checked: "checked",
                },
                simpleData: {
                    enable: true,
                }
            },
            callback: {
                onCheck: zTreeOnCheck,
                onClick: zTreeOnClick,
            },
        };
        var param = { action: 'GetRoleModuleByRoleIdTree', RoleId: roleId };
        $.ajax({
            type: 'Get',
            url: "/IServer/SysManage/SysManage.ashx",
            data: param,
            success: function (data) {
                var nodesjson = eval(data);
                setTimeout(function () {
                    if (zTree != undefined) {
                        zTree.destroy();
                    }
                    zTree = $.fn.zTree.init($("#RoleModuleTree"), setting, nodesjson);
                    zTree.expandAll(true);
                }, 100);
            },
            error: function (msg) {
                layer.alert(" 数据加载失败！" + msg);
            }
        });
    },
    // 检测保存的节点
    SaveRoleModule: function () {
        selectedModuleIDs = GetCheckedNodes();
        if (selectedModuleIDs.length == 0) {
            layer.confirm('没有模块被选择，确认是要清除所有权限吗？', function () {
                saveCheckedNodes(selectedModuleIDs, RoleId);
            });
        }
        else
            saveCheckedNodes(selectedModuleIDs, RoleId);
    },
    // 设定控制项
    ShowControlWindow: function (wtype) {
        var treeObj = $.fn.zTree.getZTreeObj("RoleModuleTree");
        var nodes = treeObj.getSelectedNodes();
        if (nodes.length <= 0) {
            return;
        }
        if (wtype == 'control') {
            $('#TBControl').modal('show');
            moduleId = nodes[0].id;
            TB3ControlLeft(moduleId); // 加载不同系统页面对应的按钮列表
            TB3ControlRight(RoleId, moduleId);
        }
    }
}

// 获取选中的checkbox
var GetCheckedNodes = function () {
    var treeObj = $.fn.zTree.getZTreeObj("RoleModuleTree");
    var nodes = treeObj.getCheckedNodes();
    let selectedModuleIDs = "";
    nodes.forEach(function (element) {
        selectedModuleIDs += element.id + ",";
    }, this);
    if (selectedModuleIDs.length > 0) {
        selectedModuleIDs = selectedModuleIDs.substring(0, selectedModuleIDs.length - 1);
    }
    return selectedModuleIDs;
}

// 控制功能按钮
var zTreeOnCheck = function (event, treeId, treeNode) {
    var checked = treeNode.checked;
    if (checked ? "true" : "false") {
        $('#btnSaveRight').attr("disabled", false);
    }
}

var zTreeOnClick = function (event, treeId, treeNode) {
    var zTree = $.fn.zTree.getZTreeObj(treeId);
    var nodes = zTree.getSelectedNodes();
    if (nodes[0].checked == true) {
        $('#btnSetControl').attr('disabled', false);
    }
    else {
        $('#btnSetControl').attr('disabled', 'disabled');
    }
}

// 保存选择的节点信息
function saveCheckedNodes(modules, roleid) {
    var param = { action: 'SaveRoleModule', moules: modules, roleid, roleid};
    $.ajax({
            type: 'post',
            url: '/IServer/SysManage/SysManage.ashx?',
            data: param,
            success: function () {
                layer.msg("保存成功", { time: 1500, icon: 1 });
    }, error: function () {
            layer.msg("保存失败，请重试。", { time: 3000, icon: 2 });
    }
    });
    }

//=====================设置权限End=========================

//=====================设置车型Begin=========================
var $tb2 = '';
var index = '';
var $tb2_td_PKID = '';
var $tb2_td_LeftName = '';

// 设置车型
function ShowControlWindow(wtype) {
    var rows = $table.bootstrapTable('getSelections');
    if (rows <= 0) {
        layer.msg('请至少选择一条数据。');
        return;
    }
    else {
        if ('car' == wtype)
    {
            $('#myModal2').modal('show');
            $('#HidId').val(rows[0].PKID);// 用于保存角色车型
            TB2();
            bindRoleCarsDataTable(rows[0].PKID);
    }
    }
    }

// 设置车型：查询角色车型列表
function TB2() {
    var queryUrl = '/IServer/SysManage/SysManage.ashx?action=GetBrandDictionaryByPageList';
    $tb2 = $('#TB2').bootstrapTable({
        columns: [
    {
        field: 'CarName',
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
            var carName = $element.text();
            tr = $("#TB3 tr td");
            let isExist = false;// 声明变量，append数据时用于判断右侧表格是否存在左侧表格里的数据
            if (tr.length == 0) {
                isExist = true;
            }
            else {
                for (var i = 1; i < tr.length; i++) {
                    let rightName = tr.eq(i).text();
                    if (row.CarName == rightName.trim()) {
                        isExist = false;
                        break;
                    }
                    else {
                        isExist = true;
                    }
                };
            }
            if (isExist) {
                $("#TB3 tbody").append('<tr data-index="' + index++ + '"><td style="display:none;">' + $('#HidId').val() + '</td><td style="width: 320px;height: 38px;">' + carName.trim() + '</td></tr>');
            }
        },
        onClickRow: function (row, $element) {
            $($element).addClass("backcolor").siblings("tr").removeClass("backcolor");
            $tb2_td_PKID = $('#HidId').val();
            $tb2_td_LeftName = $element.text();
        }
    });
}

// 设置车型：根据角色主键查询角色车型列表
function bindRoleCarsDataTable(keyword) {
    $.getJSON('/IServer/SysManage/SysManage.ashx?action=GetRoleBrandByRoleID&RoleId=' + keyword, function (res) {
        let strHtml = "";
        if (res != null) {
            for (var i = 0; i < res.length; i++) {
                strHtml += '<tr><td style="display:none;">' + res[i].RoleID + '</td><td style="width: 320px;height: 38px;">' + res[i].CarName + '</td></tr>';
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
            for (var i = 1; i < tr.length; i++) {
                let rightName = tr.eq(i).text();
                if ($tb2_td_LeftName.trim() == rightName.trim()) {
                    isExist = false;
                    break;
                }
                else {
                    isExist = true;
                }
            };
        }
        if (isExist) {
            $("#TB3 tbody").append('<tr data-index="' + index++ + '"><td style="display:none;">' + $tb2_td_PKID + '</td><td style="width: 320px;height: 38px;">' + $tb2_td_LeftName.trim() + '</td></tr>');
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

// 保存设置车型数据
function SaveCars() {
    var CarName = "";
    $("#TB3 tr :nth-child(2)").each(function (i, dom) {
        CarName += "," + $(this).html()
    });
    var strCarName = CarName.substring(2);
    var param = { action: 'SaveCars', roleId: $('#HidId').val(), carNames: strCarName.trim() };
    $.ajax({
        url: '/IServer/SysManage/SysManage.ashx',
        type: 'post',
        data: param,
        dataType: 'json',
        success: function (result) {
            var SaveLoading =  layer.msg('数据提交中，请稍候', { icon: 16, time: true, shade: 0.8 });
            if (result.success) {
                layer.close(SaveLoading);
                layer.msg("保存成功", { time: 1500, icon: 1 });
                RefreshData();
                $('#myModal2').modal('hide');
            }
            else {
               layer.close(SaveLoading);
                layer.msg("保存失败，请重试。", { time: 3000, icon: 2 });
            }
        }
    })
}
// 关闭设置角色弹层
function setRoleHide() {
    $('#myModal2').modal('hide');
}
//=====================设置车型End=========================

//=====================设定角色模块Begin=========================
// 设定控制项：根据模块ID加载不同系统页面对应的按钮列表
var controlID = null;
var controlName = null;
function TB3ControlLeft(moduleId) {
    var queryUrl = '/IServer/SysManage/SysManage.ashx?action=GetControlByModule&ModuleId=' + moduleId + '&rmd=' + new Date().getTime();
    $('#TBControl1').bootstrapTable({
        columns: [{
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
            index = $("#TBControl2 tbody").find('tr').length;
            tr = $("#TBControl2 tr td");
            let isExist = false;// 声明变量，append数据时用于判断右侧表格是否存在左侧表格里的数据
            if (tr.length == 0) {
                isExist = true;
            }
            else {
                for (var i = 1; i < tr.length; i++) {
                    let rightName = tr.eq(i).text();
                    if (row.Name == rightName.trim()) {
                        isExist = false;
                        break;
                    }
                    else {
                        isExist = true;
                    }
                };
            }
            if (isExist) {
                $("#TBControl2 tbody").append('<tr data-index="' + index++ + '"><td style="display:none;">' + row.PKID + '</td><td style="width: 320px;height: 38px;">' + $element.text() + '</td></tr>');
            }
        },
        onClickRow: function (row, $element) {
            $($element).addClass("backcolor").siblings("tr").removeClass("backcolor");
            controlID = row.PKID; // 角色模块控制ID
            controlName = $element.text();// 角色模块名称
        }
    });
}

// 设定控制项：根据角色主键查询角色车型列表
function TB3ControlRight(roleId, moduleId) {
    $.getJSON('/IServer/SysManage/SysManage.ashx?action=GetControlInfo&RoleId=' + roleId + '&ModuleId=' + moduleId, function (res) {
        let strHtml = "";
        if (res != null) {
            for (var i = 0; i < res.length; i++) {
                strHtml += '<tr><td style="display:none;">' + res[i].PKID + '</td><td style="width: 320px;height: 38px;">' + res[i].Name + '</td></tr>';
                $("#TBControl2 tbody").html(strHtml);
            }
        }
        return;
    });
    $('#TBControl2 tbody').on('click dblclick', 'tr', function () {
        $(this).addClass("backcolor").siblings("tr").removeClass("backcolor");
    });
    $('#TBControl2 tbody').on('dblclick', 'tr', function () {
        $('#TBControl2 tbody tr.backcolor').remove();
    });
}

// 单个右移
function MoveRigth2() {
    var len = $('#TBControl1 tbody tr.backcolor').length;
    if (len == 0) {
        return;
    }
    else {
        index = $("#TBControl2 tbody").find('tr').length;
        tr = $("#TBControl2 tr td");
        let isExist = false;// 声明变量，append数据时用于判断右侧表格是否存在左侧表格里的数据
        if (tr.length == 0) {
            isExist = true;
        }
        else {
            for (var i = 1; i < tr.length; i++) {
                let rightName = tr.eq(i).text();
                if (controlName.trim() == rightName.trim()) {
                    isExist = false;
                    break;
                }
                else {
                    isExist = true;
                }
            };
        }
        if (isExist) {
            $("#TBControl2 tbody").append('<tr data-index="' + index++ + '"><td style="display:none;">' + controlID + '</td><td style="width: 320px;height: 38px;">' + controlName + '</td></tr>');
        }
    }
}

// 全部右移
function AllMoveRigth2() {

}

// 单个移除
function MoveLeft2() {
    var len = $('#TBControl2 tbody tr.backcolor').length;
    if (len == 0) {
        return;
    }
    else {
        $('#TBControl2 tbody tr.backcolor').remove();
    }
}

// 全部移除
function AllMoveLeft2() {
    $('#TBControl2 tbody tr').remove();
}

// 保存设定控制项数据
function SaveControl() {
    var controlIDs = "";
    $("#TBControl2 tr :nth-child(1)").each(function (i, dom) {
        controlIDs += "," + $(this).html()
    });
    var strcontrolIDs = controlIDs.substring(4);
    var param = { action: 'SaveControl', moduleId: moduleId, roleId: RoleId, ContortIds: strcontrolIDs };
    $.ajax({
        url: '/IServer/SysManage/SysManage.ashx',
        type: 'post',
        data: param,
        success: function () {
            layer.msg("保存成功", { time: 1500, icon: 1 });
            RefreshData();
            $('#TBControl').modal('hide');
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            layer.alert(textStatus);
        },
    })
}

// 关闭设定控制项弹层
function controlHide() {
    $('#TBControl').modal('hide');
}
//=====================设定角色模块End=========================
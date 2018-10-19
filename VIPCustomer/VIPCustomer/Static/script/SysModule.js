// 初始化树
function InitTree() {
    var zTree;
    var setting = {
        data: {
            key: {
                name: "name" //如果命名就是"name"，此处可以不写  
            },             // 数据源格式
            simpleData: {
                enable: true,
            }
        },
        callback: {
            onDblClick: dblClick,
            onClick: zTreeOnClick,
        },
    };
    $.ajax({
        type: 'Get',
        url: "/IServer/SysManage/SysManage.ashx?action=LoadTree",
        //dataType: "json",
        success: function (data) {
            var nodesjson = eval(data);
            zTree = $.fn.zTree.init($("#FlatTree"), setting, nodesjson);
            zTree.expandAll(true);
        },
        error: function (msg) {
            layer.alert(" 数据加载失败！" + msg);
        }
    });

    $('#tableModuleCommand tbody').on('click', 'tr', function () {
        if ($(this).hasClass('selected')) {
            $(this).removeClass('selected');
            $(this).addClass('selected');
        }
        else {
            $('#tableModuleCommand tbody tr.selected').removeClass('selected');
            $(this).addClass('selected');
        }
    });
}
var zTreeOnClick = function () {
    $('.col-lg-3').hide();
    $('.col-lg-9').css('width', 100 + '%');
}
// 展开
function expandAllNodes() {
    $.fn.zTree.getZTreeObj("FlatTree").expandAll(true);
}

// 收起
function closeAllNodes() {
    $.fn.zTree.getZTreeObj("FlatTree").expandAll(false);
}

var nodeId = "";
var parentId = "";

// 接收地址栏参数
var getUrlParam = function (name) {
    var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)");
    var r = window.location.search.substr(1).match(reg);
    if (r != null)
        return unescape(r[2]);
    return null;
}
// 显示操作类型窗口
function ShowDetail(opType) {
    $('#opType').val(opType);
    var treeObj = $.fn.zTree.getZTreeObj("FlatTree");
    var nodes = treeObj.getSelectedNodes();
    if (nodes.length <= 0) {
        layer.alert("未选择节点，请先选择一个节点。");
        return;
    }
    else {
        nodeId = nodes[0].id;
        parentId = nodes[0].pId;
        switch (opType) {
            case 1:
                OpenAddType1();
                break;
            case 2:
                OpenAddType2();
                break;
            case 3:
                OpenEditWindow();
                break;
            default:
        }
    }
}

// 打开添加窗口
var OpenAddType1 = function () {
    layer.open({
        title: '添加同级模块信息',
        type: 2,
        scrollbar: false,
        maxmin: true,
        area: ['500px', '500px'],
        content: '/Page/SysManage/SysModuleEdit.html?parentId=' + parentId + '&id=' + nodeId,
        end: function () {
        }
    });
}

var OpenAddType2 = function () {
    layer.open({
        title: '添加子级模块信息',
        type: 2,
        scrollbar: false,
        maxmin: true,
        area: ['500px', '500px'],
        content: '/Page/SysManage/SysModuleEdit.html?parentId=' + parentId + '&id=' + nodeId,
        end: function () {
        }
    });
}

// 打开编辑窗口
var OpenEditWindow = function () {
    layer.open({
        title: '编辑模块信息',
        type: 2,
        scrollbar: false,
        maxmin: true,
        area: ['500px', '500px'],
        content: '/Page/SysManage/SysModuleEdit.html?nodeId=' + nodeId,
        end: function () {
        }
    });
}

// 保存数据
function PageInitModule() {
    var opType = $('#opType', window.parent.document).val();
    if (3 == opType) {
        var param = { PKID: getUrlParam("nodeId"), action: 'GetModuleByPKID' };
        $.ajax({
            type: 'post',
            url: '/IServer/SysManage/SysManage.ashx',
            data: param,
            dataType: 'json',
            success: function (data) {
                $('#Name').val(data.Name);
                $('#PageAddress').val(data.PageAddress);
                if (data.IsActivate == 1) {
                    $('#IsActivate').attr('checked', true);
                    $('#IsActivate').val("1");
                }
                else {
                    $('#IsActivate').attr('checked', false);
                    $('#IsActivate').val("0");
                }
                if (data.IsSysModule == 1) {
                    $('#IsSysModule').attr('checked', true);
                    $('#IsSysModule').val("1");
                }
                else {
                    $('#IsSysModule').attr('checked', false);
                    $('#IsSysModule').val("0");
                }
                $('#Remark').val(data.Remark);
            },
            error: function () {
                alert("获取数据失败。");
            }
        })
    }
    else {

    }
    $('#Form1').bootstrapValidator({
        message: 'This value is not valid',
        feedbackIcons: {
            valid: 'glyphicon glyphicon-ok',
            invalid: 'glyphicon glyphicon-remove',
            validating: 'glyphicon glyphicon-refresh'
        },
        fields: {
            Name: {
                message: '模块名称验证失败',
                validators: {
                    notEmpty: {
                        message: '模块名称不能为空'
                    }
                }
            },
            RoleGrade: {
                message: '模块等级验证失败',
                validators: {
                    notEmpty: {
                        message: '模块等级不能为空'
                    }
                }
            }
        }
    }).
     on('success.form.bv', function (e) {//点击提交之后
         e.preventDefault();
         var $form = $(e.target);
         var bv = $form.data('bootstrapValidator');
         var param = {
             opType: opType,
             PKID: getUrlParam("nodeId"),
             id: getUrlParam('id'),
             Name: $('#Name').val(),
             ParentID: getUrlParam('parentId'),
             IsActivate: $('#IsActivate').val(),
             IsSysModule: $('#IsSysModule').val(),
             PageAddress: $('#PageAddress').val(),
             Remark: $('#Remark').val()
         };
         $.ajax({
             url: $form.attr('action'),
             type: 'post',
             data: param,
             dataType: 'json',
             success: function (result) {
                 if (result.success) {
                     parent.location.reload();
                     setTimeout(function () {
                         layer.alert(result.msg);
                     }, 2000);
                     parent.layer.closeAll();
                 }
                 else {
                     layer.alert(result.msg);
                 }
             },
             error: function (jqXHR, textStatus, errorThrown) {
                 layer.alert("程序执行过程中发生如下错误:" + errorThrown)
             }
         })
     });

    $('#IsActivate').click(function () {
        if ($(this).is(':checked')) {
            $(this).val("1");
        }
        else {
            $(this).val("0");
        }
    });
    $('#IsSysModule').click(function () {
        if ($(this).is(':checked')) {
            $(this).val("1");
        }
        else {
            $(this).val("0");
        }
    });
}

// 删除节点
var DeleteTree = function () {
    var treeObj = $.fn.zTree.getZTreeObj("FlatTree");
    var nodes = treeObj.getSelectedNodes();
    if (nodes.length <= 0) {
        layer.alert("未选择节点，请先选择一个节点。");
        return;
    }
    else {
        layer.confirm('您确定要删除模块【' + nodes[0].name + '】及其子模块吗？ ', function () {
            var nodeIds = getChildNodes(nodes[0]);
            var param = { action: 'DeleteTree', PKID: nodeIds };
            $.ajax({
                url: '/IServer/SysManage/SysManage.ashx',
                type: 'post',
                data: param,
                dataType: 'json',
                success: function (d) {
                    if (d.success) {
                        parent.location.reload();
                        layer.alert(d.msg);
                    }
                    else {
                        layer.alert(d.msg);
                    }
                }
            })
        })
    }
}

// 获取选中节点的所有子节点
function getChildNodes(treeNode) {
    var ztree = $.fn.zTree.getZTreeObj("FlatTree");
    var childNodes = ztree.transformToArray(treeNode);
    var nodes = new Array();
    for (i = 0; i < childNodes.length; i++) {
        nodes[i] = childNodes[i].id;
    }
    return nodes.join(",");
}

// 设定模块控制项
var ShowEast = function () {
    var treeObj = $.fn.zTree.getZTreeObj("FlatTree");
    var nodes = treeObj.getSelectedNodes();
    if (nodes.length <= 0) {
        layer.alert("未选择节点，请先选择一个节点。");
        return;
    }
    else {
        $('.col-lg-3').show();
        $('.col-lg-9').css('width', 75 + '%');
        $('#moduleID').val(nodes[0].id);
        nodeId = nodes[0].id;
        MC.showList(nodes[0].id);
    }
}

// 双击设定模块控制项
var dblClick = function () {
    var treeObj = $.fn.zTree.getZTreeObj("FlatTree");
    var nodes = treeObj.getSelectedNodes();
    if (nodes.length <= 0) {
        layer.alert("未选择节点，请先选择一个节点。");
        return;
    }
    else {
        $('.col-lg-3').show();
        $('.col-lg-9').css('width', 75 + '%');
        $('#moduleID').val(nodes[0].id);
        nodeId = nodes[0].id;
        MC.showList(nodes[0].id);

    }
}

// Module控制操作
var MC = {
    showList: function (nodeID) {
        var param = { action: 'GetModuleContorl', ModuleID: nodeID };
        $.ajax({
            url: '/IServer/SysManage/SysManage.ashx',
            type: 'post',
            data: param,
            dataType: 'json',
            success: function (data) {
                var strHtml = "";
                if (data != null) {
                    for (var i = 0; i < data.length; i++) {
                        strHtml += '<tr><td style="display:none;">' + data[i].PKID + '</td><td>' + data[i].Code + '</td><td>' + data[i].Name + '</td><td>' + data[i].Remark + '</td></tr>';
                    }
                }
                $("#tableModuleCommand tbody").html(strHtml);
            }
        });
    },
    showDetail: function (type) {
        $('#opTypeMC').val(type);
        if (type == 3) {
            var rows = $('#tableModuleCommand .selected').length;
            if (rows <= 0) {
                layer.alert("请至少选择一条记录进行操作。");
                return;
            }
            else {
                var PKID = $('#tableModuleCommand .selected').find('td:eq(0)').html();
                layer.open({
                    title: '编辑模块控制',
                    type: 2,
                    scrollbar: false,
                    maxmin: true,
                    area: ['500px', '450px'],
                    content: '/Page/SysManage/ModuleContorlEdit.html?PKID=' + PKID,
                });
            }
        }
        else {
            layer.open({
                title: '添加模块控制',
                type: 2,
                scrollbar: false,
                maxmin: true,
                area: ['500px', '450px'],
                content: '/Page/SysManage/ModuleContorlEdit.html?moduleID=' + nodeId,
                end: function () {

                }
            });
        }
    },
    deleteControl: function () {
        var rows = $('#tableModuleCommand .selected').length;
        if (rows <= 0) {
            layer.alert("请至少选择一条记录进行操作。");
            return;
        }
        else {
            layer.confirm('您确定要删除模块控制【' + $('#tableModuleCommand .selected').find('td:eq(2)').html() + '】吗？ ', function () {
                var PKID = $('#tableModuleCommand .selected').find('td:eq(0)').html();
                var param = { action: 'DeleteModuleControl', PKID: PKID };
                $.ajax({
                    url: '/IServer/SysManage/SysManage.ashx',
                    type: 'post',
                    data: param,
                    dataType: 'json',
                    success: function (d) {
                        if (d.success) {
                            setTimeout(function () {
                                layer.alert(d.msg);
                            }, 1000);
                            MC.showList(nodeId);
                        }
                        else {
                            layer.alert(d.msg);
                        }
                    },
                    error: function (jqXHR, textStatus, errorThrown) {
                        layer.alert("程序执行过程中发生如下错误:" + errorThrown)
                    }
                })
            })
        }
    }
}

// 保存数据
function PageMCInitModule() {
    var opTypeMC = $('#opTypeMC', window.parent.document).val();
    var PKID = getUrlParam("PKID");
    if (3 == opTypeMC) {
        var param = { action: 'ModuleCommandByPKID', pkid: PKID };
        $.ajax({
            url: '/IServer/SysManage/SysManage.ashx',
            type: 'post',
            data: param,
            dataType: 'json',
            success: function (data) {
                $('#Name').val(data.Name);
                $('#Code').val(data.Code);
                $('#Remark').val(data.Remark);
            }
        });
    }
    else { }
    $('#Form1').bootstrapValidator({
        message: 'This value is not valid',
        feedbackIcons: {
            valid: 'glyphicon glyphicon-ok',
            invalid: 'glyphicon glyphicon-remove',
            validating: 'glyphicon glyphicon-refresh'
        },
        fields: {
            Code: {
                message: '功能标识验证失败',
                validators: {
                    notEmpty: {
                        message: '功能标识不能为空'
                    }
                }
            },
            Name: {
                message: '功能名称验证失败',
                validators: {
                    notEmpty: {
                        message: '功能名称不能为空'
                    }
                }
            }
        }
    }).
     on('success.form.bv', function (e) {//点击提交之后
         e.preventDefault();
         var $form = $(e.target);
         var bv = $form.data('bootstrapValidator');
         var param = {
             opTypeMC: opTypeMC,
             moduleID: getUrlParam('moduleID'),
             PKID: PKID,
             Code: $('#Code').val(),
             Name: $('#Name').val(),
             Remark: $('#Remark').val()
         };
         $.ajax({
             url: $form.attr('action'),
             type: 'post',
             data: param,
             dataType: 'json',
             success: function (result) {
                 if (result.success) {
                     layer.alert(result.msg);
                     setTimeout(function () {
                         parent.layer.closeAll();
                         MC.showList(parent.$('#moduleID').val());
                     }, 1000);
                 }
                 else {
                     layer.alert(result.msg);
                 }
             },
             error: function (jqXHR, textStatus, errorThrown) {
                 layer.alert("程序执行过程中发生如下错误:" + errorThrown)
             }
         })
     });
}

//显示移动目标树
var ShowMoveTree = function () {
    var treeObj = $.fn.zTree.getZTreeObj("FlatTree");
    var nodes = treeObj.getSelectedNodes();
    if (nodes.length <= 0) {
        layer.alert("未选择节点，请先选择一个节点。");
        return;
    }
    else {
        layer.open({
            title: '添加',
            type: 2,
            scrollbar: false,
            maxmin: false,
            area: ['500px', '450px'],
            content: '/Page/SysManage/ModuleTree.html',
        });
    }
}

var MoveTreeLoad = function () {
    var zTree;
    var setting = {
        data: {
            key: {
                name: "name" //如果命名就是"name"，此处可以不写  
            },             // 数据源格式
            simpleData: {
                enable: true,
            }
        },
        callback: {
            onDblClick: dblClick,
        }
    };
    $.ajax({
        type: 'Get',
        url: "/IServer/SysManage/SysManage.ashx?action=LoadTree",
        //dataType: "json",
        success: function (data) {
            var nodesjson = eval(data);
            zTree = $.fn.zTree.init($("#MoveTree"), setting, nodesjson);
            zTree.expandAll(true);
        },
        error: function (msg) {
            layer.alert(" 数据加载失败！" + msg);
        }
    });
}

// 移动节点
var MoveTree = function (moveType) {
    var MoveTree = $.fn.zTree.getZTreeObj("MoveTree");
    var targetObj = MoveTree.getSelectedNodes();
    if (targetObj.length <= 0) {
        layer.alert("未选择目标节点，请先选择一个目标节点。");
        return;
    }
    var FlatTree = parent.$.fn.zTree.getZTreeObj("FlatTree");
    var sourceObj = FlatTree.getSelectedNodes();
    if (targetObj[0].id == sourceObj[0].id) {
        layer.alert("移动的节点与目标节点相同，请重新选择目标节点。");
        return;
    }
    var typeStr = (1 == moveType) ? "后" : "下";
    layer.confirm("您确定要移动模块【" + sourceObj[0].name + "】至模块【" + targetObj[0].name + "】" + typeStr + "吗？", function () {

        var param = { action: 'MoveModules', moveType: moveType, targetObj: targetObj[0].id, sourceObj: sourceObj[0].id };
        $.ajax({
            url: '/IServer/SysManage/SysManage.ashx',
            type: 'post',
            data: param,
            success: function () {
                layer.alert("保存成功。");
                parent.location.reload();
            },
            error: function (jqXHR, textStatus, errorThrown) {
                layer.alert("程序执行过程中发生如下错误:" + errorThrown)
            }
        })
    })
}


$(document).ready(function () {
    // 计算iframe高度
    $(".wrapper").find('.mainContent').height($(window).height() - 100);
    $(window).resize(function (e) {
        $(".wrapper").find('.mainContent').height($(window).height() - 100);
    });

    var userInfo = top.clients.loginInfo;
    if (userInfo != undefined) {
        sessionStorage.PKID = userInfo.PKID;
        sessionStorage.Name = userInfo.Name;
        sessionStorage.DealerName = userInfo.DealerName;
        $('.username').html(userInfo.Name);
        var dealerName = userInfo.DealerName == null ? '' : userInfo.DealerName;
        var name = userInfo.Name;
        $('#labUser').html(dealerName + '&nbsp;' + name);
    }
    else {
        setTimeout(function () {
            window.location.href = "Login.html";
        }, 1000);
    }
    GetLoadNav();
});

// 个人信息
function btn_PersonDeail() {
    $('#PersonInfo').modal('show');
    var PKID = sessionStorage.PKID;//接收PKID的值
    $.getJSON('../IServer/SysManage/SysManage.ashx?action=GetSysUserByPKID&keyword=' + PKID, function (result) {
        $('input[name=LoginPwd]').val(result.LoginPwd);
        $('input[name=Name]').val(result.Name);
        $('input[name=Phone]').val(result.Phone);
        $('input[name=Email]').val(result.Email);
        $('#HidId').val(PKID);
    });
}

// 保存个人信息
function SavePersonInfo() {
    $.ajax({
        type: 'post',
        url: '../IServer/SysManage/SysManage.ashx?action=SaveParsonInfo',
        dataType: 'json',
        data: $('#Form1').serializeObject(),
        success: function (result) {
            if (result.success) {
                layer.msg("保存成功", { time: 1500, icon: 1 });
                $('#PersonInfo').modal('hide');
                clearForm($('#Form1'));
            }
        },
    });
}

function btn_SelfPassword() {
    $('#SelfPassword').modal('show');
    var PKID = sessionStorage.PKID;//接收PKID的值
    $.getJSON('../IServer/SysManage/SysManage.ashx?action=GetSysUserByPKID&keyword=' + PKID, function (result) {
        $('input[name=LoginName]').val(result.LoginName);
        $('#Pwd_PKID').val(PKID);
    });
}

// 修改密码
function SelfPassword() {
    $('#Form2').bootstrapValidator({
        message: 'This value is not valid',
        feedbackIcons: {/*input状态样式图片*/
            valid: 'glyphicon glyphicon-ok',
            invalid: 'glyphicon glyphicon-remove',
            validating: 'glyphicon glyphicon-refresh'
        },
        fields: {/*验证：规则*/
            LoginPwd: {
                validators: {
                    notEmpty: {
                        message: '旧密码不能为空'
                    },
                }
            },
            newPassword: {
                message: '密码无效',
                validators: {
                    notEmpty: {
                        message: '新密码不能为空'
                    },
                }
            },
            newPassword2: {
                message: '密码无效',
                validators: {
                    notEmpty: {
                        message: '确认新密码不能为空'
                    },
                    identical: {//相同
                        field: 'newPassword',
                        message: '两次密码不一致'
                    }
                }
            }
        }
    }).on('success.form.bv', function (e) {//点击提交之后
        e.preventDefault();    // Prevent form submission
        var $form = $(e.target);      // Get the form instance
        $.post($form.attr('action'), $form.serializeObject(), function (result) {
            var SaveLoading = top.layer.msg('数据提交中，请稍候', { icon: 16, time: true, shade: 0.8 });
            var json = JSON.parse(result);  // 由json字符串转换为json对象
            if (json.success) {
                top.layer.close(SaveLoading);
                layer.msg("修改成功。", { time: 1500, icon: 1 });
                $('#SelfPassword').modal('hide');
                $form.bootstrapValidator('resetForm');
                clearForm($($form));
            }
            else {
                top.layer.close(SaveLoading);
                layer.msg(json.msg, { time: 3000, icon: 2 });
            }
        });
    });
}

// 动态获取系统左侧权限菜单
function GetLoadNav() {
    var data = top.clients.authorizeMenu;
    if (data != "") {
        if (data[0].ParentID == 0) {
            var menuHtml = "";
            for (var i = 0; i < data.length; i++) {
                menuHtml += '<li class="sub-menu dcjq-parent-li"><a href="javascript:;" class="dcjq-parent">';
                menuHtml += '<i class="' + data[i].ImageAddress + '" data-icon="' + data[i].ImageAddress + '"></i><span>' + data[i].MenuName + '</span>';
                menuHtml += '<span class="label label-danger span-sidebar">' + data[i].MenuId + '</span>';
                menuHtml += '<span class="dcjq-icon"></span></a>';
                if (data[i].MenuId == data[i].Children[0].ParentID) {
                    menuHtml += '<ul class="sub" style="overflow: hidden; display: none;">';
                    for (var j = 0; j < data[i].Children.length; j++) {
                        menuHtml += '<li class="sub-menu dcjq-parent-li"><a href="javascript:;" class="dcjq-parent">';
                        menuHtml += '<i class="' + data[i].Children[j].ImageAddress + '" data-icon="' + data[i].Children[j].ImageAddress + '"></i>';
                        menuHtml += data[i].Children[j].MenuName + '';
                        menuHtml += '<span class="dcjq-icon"></span></a>';
                        if (data[i].Children[j].MenuId == data[i].Children[j].Children[0].ParentID) {
                            menuHtml += '<ul class="sub" style="overflow: hidden; display: none;">';
                            for (var k = 0; k < data[i].Children[j].Children.length; k++) {
                                menuHtml += '<li>';
                                menuHtml += '<a class="menuItem" data-id="' + data[i].Children[j].Children[k].MenuId + '"  href="' + data[i].Children[j].Children[k].MenuUrl + '" >';
                                menuHtml += '' + data[i].Children[j].Children[k].MenuName + '</a>';
                            }
                            menuHtml += '</ul>';
                        }
                    }
                    menuHtml += '</ul>';
                }
                menuHtml += '</li>';
            }
            $('.sidebar-menu').html(menuHtml);
        }
    }
    else {
        $('.sidebar-menu').empty();
    }
}

function LoginOut() {
    $.getJSON('IServer/Handler1.ashx?action=LoginOut', function (result) {
        if (result.success) {
            location.href = 'Login.html';
        }
    });
}
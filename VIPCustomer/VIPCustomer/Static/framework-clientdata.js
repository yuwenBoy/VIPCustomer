var clients = [];
$(function () {
    clients = $.clientsInit();
});
$.clientsInit = function () {
    var dataJson = {
        loginInfo: [],// 登录信息
        authorizeMenu: [],// 权限菜单
    };
    var init = function () {
        $.ajax({
            url: 'IServer/Handler1.ashx',
            type: 'post',
            async: false,
            data: { action: 'getUser' },
            dataType: 'json',
            success: function (result) {
                if (result.success == false) {
                    window.location.href = 'Login.html';
                }
                else {
                    dataJson.authorizeMenu = result.authorizeMenu; // 获取客户端权限菜单
                    dataJson.loginInfo = result.userLoginInfo;// 获取客户端在线登录信息
                }
            }
        });
    }
    init();
    return dataJson;
}
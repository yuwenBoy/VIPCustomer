$.load = function () {
    location.reload();
}

// 对Date的扩展，将 Date 转化为指定格式的String
// 月(M)、日(d)、小时(h)、分(m)、秒(s)、季度(q) 可以用 1-2 个占位符， 
// 年(y)可以用 1-4 个占位符，毫秒(S)只能用 1 个占位符(是 1-3 位的数字) 
// 例子： 
// (new Date()).Format("yyyy-MM-dd hh:mm:ss.S") ==> 2006-07-02 08:09:04.423 
// (new Date()).Format("yyyy-M-d h:m:s.S")      ==> 2006-7-2 8:9:4.18 
Date.prototype.Format = function (fmt) {
    var o = {
        "M+": this.getMonth() + 1, //月份 
        "d+": this.getDate(), //日 
        "h+": this.getHours(), //小时 
        "m+": this.getMinutes(), //分 
        "s+": this.getSeconds(), //秒 
        "q+": Math.floor((this.getMonth() + 3) / 3), //季度 
        "S": this.getMilliseconds() //毫秒 
    };
    if (/(y+)/.test(fmt)) fmt = fmt.replace(RegExp.$1, (this.getFullYear() + "").substr(4 - RegExp.$1.length));
    for (var k in o)
        if (new RegExp("(" + k + ")").test(fmt)) fmt = fmt.replace(RegExp.$1, (RegExp.$1.length == 1) ? (o[k]) : (("00" + o[k]).substr(("" + o[k]).length)));
    return fmt;
}

// 接收地址栏参数
var getUrlParam = function (name) {
    var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)");
    var r = window.location.search.substr(1).match(reg);
    if (r != null)
        return unescape(r[2]);
    return null;
}

// 将表单序列化为JSON对象   
$.fn.serializeObject = function () {
    var o = {};
    var a = this.serializeArray();
    $.each(a, function () {
        if (o[this.name]) {
            if (!o[this.name].push) {
                o[this.name] = [o[this.name]];
            }
            o[this.name].push(this.value.trim() || '');
        } else {
            o[this.name] = this.value.trim() || '';
        }
    });
    return o;
}

// 清空表单
function clearForm(form) {
    // input清空
    $(':input', form).each(function () {
        var type = this.type;
        var tag = this.tagName.toLowerCase(); // normalize case
        if (type == 'text' || type == 'password' || tag == 'textarea')
            this.value = "";
            // 多选checkboxes清空
            // select 下拉框清空
        else if (tag == 'select')
            this.selectedIndex = -1;
    });
};

String.prototype.format = function (args) {
    var result = this;
    if (arguments.length > 0) {
        if (arguments.length == 1 && typeof (args) == "object") {
            for (var key in args) {
                if (args[key] != undefined) {
                    var reg = new RegExp("({" + key + "})", "g");
                    result = result.replace(reg, args[key]);
                }
            }
        }
        else {
            for (var i = 0; i < arguments.length; i++) {
                if (arguments[i] != undefined) {
                    //var reg = new RegExp("({[" + i + "]})", "g");//这个在索引大于9时会有问题，谢谢何以笙箫的指出
                    var reg = new RegExp("({)" + i + "(})", "g");
                    result = result.replace(reg, arguments[i]);
                }
            }
        }
    }
    return result;
}

String.format = function () {
    if (arguments.length == 0)
        return null;

    var str = arguments[0];
    for (var i = 1; i < arguments.length; i++) {
        var re = new RegExp('\\{' + (i - 1) + '\\}', 'gm');
        str = str.replace(re, arguments[i]);
    }
    return str;
}

//把数组转换为服务端的hashstring
var ArrayToHashString = function (objIDs) {
    var result = "";
    for (var i = 0; i < objIDs.length; i++) {
        if (Ext.isDate($(objIDs[i]).getValue()))
            result += String.format("{0}:{1};", objIDs[i], escape($(objIDs[i]).getValue().format(DateTimeFormat)));
        else
            result += String.format("{0}:{1};", objIDs[i], escape($(objIDs[i]).getValue()));
    }

    return result.substring(0, result.length - 1);
}
// ======================Jqgrid表格操作方法===============

// 获取鼠标选中一行的记录
function getRows(elem) {
    return $(elem).jqGrid("getGridParam", "selrow");
}

// 获取jqgrid 所有行数据
function GetGridDataAll(elem) {
    var rowIds = elem.getDataIDs();
    var arrayData = new Array();
    if (rowIds.length > 0) {
        for (var i = 0; i < rowIds.length; i++) {
            arrayData.push($ElemTable.getRowData(rowIds[i]));
        }
    }
    return arrayData;
}


// 权限操作按钮是否可用
$.fn.authorizeButton = function () {
    var moduleURL = top.$(".zhaoJian_iframe:visible").attr("src");
    var AuthorizeMenuJson = [];
    $.ajax({
        url: '../../IServer/Handler1.ashx',
        type: 'post',
        async: false,
        data: { action: 'GetAuthorizeMenuButton', moduleURL: moduleURL },
        dataType: 'json',
        success: function (result) {
            if (result.success == false) {
                if (window.top != null && window.top.document.URL != document.URL) {
                    setTimeout(function () {
                        window.location.href = result.msg;
                    }, 300);
                }
                alert(result.msg);
            }
            else {
                AuthorizeMenuJson = result;
            }
        },
        error: function () {
            top.layer.msg('程序在执行过程中发生错误，请刷新系统重试！');
        }
    });
    var $element = $(this);
    $element.find('a[authorize=yes]').attr('authorize', 'no');
    if (AuthorizeMenuJson != undefined) {
        $.each(AuthorizeMenuJson, function (i) {
            if (AuthorizeMenuJson[i].IsRight == true)
                $element.find("#" + AuthorizeMenuJson[i].ControlName).attr('authorize', 'yes');
            else
                $element.find("#" + AuthorizeMenuJson[i].ControlName).remove();
        });
    }
    $element.find('[authorize=no]').remove();
}
// 初始化页面
$(function () {
    initPage();
    bindSelectData();
});

var URL = "/IServer/Customer/CusManage.ashx?action=";
var initPage = function () {
    $('#table_list').jqGrid({
        url: URL + 'GetCustomerManagerPager',
        mtype: 'post',
        datatype: 'json',
        viewrecords: true,  //  是否显示总记录数
        styleUI: 'Bootstrap',
        colModel: [
                { label: '主键ID', name: 'PKID', hidden: true, key: true, },
                { label: '客户性质', name: 'CustomerNatureName', width: 120 },
                { label: '机构代码', name: 'EnterpriseCode', width: 120 },
                { label: '客户名称', name: 'Name', width: 180 },
                { label: '邮政编码', name: 'Zip', width: 120 },
                { label: '客户地址', name: 'Address', width: 180 },
                { label: '电子邮件', name: 'Fax', width: 120 },
                { label: '电话', name: 'Phone', width: 120 },
                { label: '所属单位', name: 'CompetentDepartment', width: 180 },
                { label: '单位联系人', name: 'ExecutiveDepartment', width: 180 },
                { label: '单位座机', name: 'MainBusiness', width: 180, },
                { label: '创建日期', name: 'CreateTime', width: 120, formatter: "date", formatoptions: { newformat: 'Y-m-d' } },
        ],
        autowidth: true,
        height: 568,
        shrinkToFit: false,
        rownumbers: true,   // 显示行号
        rowNum: 50,
        rowList: [50, 100, 200, 500],
        pager: "#pager_list",
        pagerpos: 'left',
        jsonReader: {
            records: "totalCount", root: "dataList", total: "totalpages", page: "currPage",
            repeatitems: false
        }
    });

    // 查询
    $('#btnSearch').click(function () {
        $('#table_list').jqGrid('setGridParam', {
            url: URL + 'GetCustomerManagerPager',
            mtype: 'post',
            postData: {
                CustomersearchContext: GetSearchString()
            },
            page: 1
        }).trigger('reloadGrid');  // 重新载入
    });

    // 添加
    $("#btnAdd").click(function () {
        layer.open({
            title: "添加",
            type: 2,
            skin: 'layui-layer-molv',
            area: ['1000px', '780px'],
            content: '../../Page/MajorAccount/CustomerEdit.html',
        });
        $('#opType').val("add");
    });

    // 编辑
    $("#btnEdit").click(function () {
        var rowId = getRows("#table_list");
        if (rowId != null) {
            var rowData = $("#table_list").jqGrid('getRowData', rowId);
            var customerId = rowData.PKID;
            layer.open({
                title: "添加",
                type: 2,
                skin: 'layui-layer-molv',
                area: ['1000px', '780px'],
                content: "../../Page/MajorAccount/CustomerEdit.html?customerId=" + customerId,
            });
            $('#opType').val("update");
        }
        else {
            layer.msg("请至少选择一个项目");
        }
    })
}

// 封装搜索条件
var GetSearchString = function () {
    var code = $('input[name=Code]').val();  // 企业代码
    var name = $('input[name=Name]').val();  // 企业名称
    var ContactName = $('select[name=ContactName]').val(); // 一级客户性质
    var ContactName2 = $('select[name=ContactName2]').val();  // 二级客户性质
    var StringToJson = JSON.stringify({ "EnterpriseCode": code, "Name": name, "CustomerNatureID": ContactName, "CustomerNature2ID": ContactName2 });
    return StringToJson;
}

// 获取当前用户所属经销店
$.post('/IServer/Dealer/Dealer.ashx?action=GetUserDealerByPKID', function (data) {
    var json = $.parseJSON(data);
    $('input[name=DealerId]').val(json.DealerId);
    $('input[name=DealerName]').val(json.DealerName);
});

// 判断点击是添加还是修改按钮
var opType = $('#opType', window.parent.document).val();
if (opType == 'add') {
    $('#opType').val(opType);
    var now = new Date();
    $('input[name=CreateTime]').val(now.Format("yyyy-MM-dd"));
    BindCContactDataTable();
}

else if (opType == 'update') {
    $('#opType').val(opType);
    var customerID = getUrlParam("customerId");
    $.ajax({
        url: URL + 'GetCustomerByPKID',
        type: 'post',
        async: false,
        dataType: 'json',
        data: { customerId: customerID },
        success: function (data) {
            $('input[name=DealerId]').val(data.DealerID);
            $('input[name=DealerName]').val(data.DealerName);
            $('input[name=EnterpriseCode]').val(data.EnterpriseCode);
            $('input[name=Name]').val(data.Name);
            $('input[name=Zip]').val(data.Zip);
            $('input[name=Address]').val(data.Address);
            $('input[name=Eamil]').val(data.Eamil);
            $('input[name=Phone]').val(data.Phone);
            $('input[name=CreateTime]').val(data.CreateTime.format("yyyy-MM-dd"));
            $('input[name=CreateTime]').attr('readonly', 'true');
            $('input[name=ExecutiveDepartment]').val(data.ExecutiveDepartment);
            $('input[name=MainBusiness]').val(data.MainBusiness);
            $('input[name=Remark]').val(data.Remark);
            $('input[name=CompetentDepartment]').val(data.CompetentDepartment);
            $('select[name=ContactName]').append("<option value='" + data.CustomerNatureID + "'>" + data.CustomerNatureName + "</option>");
            $('select[name=ContactName]').attr('disabled', true);
            $('select[name=ContactName2]').append("<option value='" + data.CustomerNature2ID + "'>" + data.CustomerNatureName2 + "</option>");
            $('select[name=ContactName2]').attr('disabled', true);
            $('textarea[name=Remark]').val(data.Remark);
        }
    });
    BindCContactDataTable(customerID);
};

function bindSelectData() {
    // 获取客户性质一级列表
    $.ajax({
        url: URL + 'GetList',
        type: 'post',
        async: false,
        dataType: 'json',
        success: function (data) {
            for (var i = 0; i < data.length; i++) {
                $('select[name=ContactName]').append("<option value='" + data[i].PKID + "'>" + data[i].Name + "</option>");
            }
        }
    });

    // 获取客户性质二级列表
    $('select[name=ContactName]').change(function () {
        $.ajax({
            url: URL + 'GetCustomerContact2',
            type: 'post',
            async: false,
            dataType: 'json',
            data: { contactId: $(this).val() },
            success: function (data) {
                $('select[name=ContactName2]').empty();
                for (var i = 0; i < data.length; i++) {
                    $('select[name=ContactName2]').append("<option value='" + data[i].PKID + "'>" + data[i].Name + "</option>");
                }
            }
        });
    })
}

function BindCContactDataTable(customerID) {
    $('#TableCContactId').jqGrid({
        url: URL + "GetCustomerContactsPager",
        mtype: 'post',
        styleUI: 'Bootstrap',
        datatype: 'json',
        postData: { customerId: customerID },
        colModel: [
            { label: 'PKID', name: 'PKID', key: true, hidden: true },
            { label: '姓名', name: 'Name', width: 120, sortable: false },
            { label: '职务', name: 'Job', sortable: false, width: 120 },
            { label: 'RoleID', name: 'RoleID', hidden: true },
            { label: '角色', name: 'RoleName', sortable: false, width: 120 },
            { label: '电话', name: 'Phone', sortable: false, width: 120 },
            { label: '移动电话', name: 'MobileTel', sortable: false, width: 120 },
        ],
        height: 100,
        width: window.screen.availWidth - 1000,
        autowidth: false,
        shrinkToFit: false,
        sortable: false,
        rowNum: 'all',
        viewrecords: true,  //  是否显示总记录数
        jsonReader: {
            root: "dataList", repeatitems: false
        }
    });
}

// 取消
$("#cancle").click(function () {
    var index = parent.layer.getFrameIndex(window.name); //先得到当前iframe层的索引
    parent.layer.close(index); //再执行关闭
});

// 角色下拉列表
$.ajax({
    url: URL + 'GetJobList',
    type: 'post',
    async: false,
    dataType: 'json',
    success: function (data) {
        for (var i = 0; i < data.length; i++) {
            $('select[name=roleId]').append("<option value='" + data[i].PKID + "'>" + data[i].ListName + "</option>");
        }
    }
});

// 保存客户
$('#btnSave').click(function () {
    var flag = $('#from1').valid();
    if (!flag) {
        return;
    }
    if ("公务员" == $('select[name=ContactName]').find("option:selected").text() && ($('input[name=Eamil]').val() == "" || $('input[name=CompetentDepartment]').val() == "" || $('input[name=ExecutiveDepartment]').val() == "" || $('input[name=MainBusiness]').val() == "")) {
        $('input[name=Email]').focus();
        layer.alert("公务员必须填写“电子邮箱，所属单位，单位联系人以及单位座机信息。”。");
        return;
    }

    $('select[name=ContactName]').attr('disabled', false); // 将disabled属性设置false，防止后台获取不到值
    $('select[name=ContactName2]').attr('disabled', false);

    var params = $("#from1").serialize();

    var arrCCData = new Array();
    var record = $('#TableCContactId').jqGrid('getRowData');
    for (var i = 0; record != null && i < record.length; i++) {
        arrCCData.push(record[i]);
    }
    var SaveLoading = layer.msg('数据提交中，请稍候', { icon: 16, time: false, shade: 0.8 });
    $.ajax({
        url: URL + 'SaveCustomer&contactData=' + JSON.stringify(arrCCData) + "&cusId=" + getUrlParam('customerId'),
        type: 'post',
        data: params,
        dataType: 'json',
        success: function (result) {
            var index = parent.layer.getFrameIndex(window.name); //先得到当前iframe层的索引
            if (result.success) {
               layer.close(SaveLoading);
                layer.alert("保存成功");
                parent.layer.close(index); //再执行关闭
                parent.location.reload();
            }
            else if (result.state = "T") {
               layer.close(SaveLoading);
                layer.alert("客户信息已经存在，请修改。")
            }
            else {
                top.layer.close(SaveLoading);
                layer.alert(data.msg);
            }
        }, error: function (data) {
            top.layer.close(index);
        }
    });
    return false;
});

// 删除
var DeleteData = function () {
    var rowId = $("#table_list").jqGrid("getGridParam", "selrow");
    if (rowId != null) {
        var rowData = $("#table_list").jqGrid('getRowData', rowId);
        var customerId = rowData.PKID;
        layer.confirm('您确定要删除选择的客户吗?', { icon: 3, title: '提示信息' }, function (index) {
            var index = layer.msg('正在处理作废订单，请稍候', { icon: 16, time: false, shade: 0.8 });
            setTimeout(function () {
                $.ajax({
                    url: URL + 'DeleteData',
                    type: 'post',
                    async: false,
                    data: { customerIds: customerId },
                    dataType: 'json',
                    success: function (data) {
                        var index = parent.layer.getFrameIndex(window.name); //先得到当前iframe层的索引
                        if (data.success) {
                            layer.close(index);
                            layer.msg("删除成功");
                            CusTableRender2.reload();
                        }
                        else {
                            layer.close(index);
                            common.cmsLayErrorMsg(data.msg);
                        }
                    }, error: function (data) {
                        top.layer.close(index);
                    }
                });
                return false;
            }, 2000);
        });
    }
    else {
        layer.msg("请至少选择一个项目");
    }
}

let opContact = '';

// 添加客户联系人
function add_Click() {
    opContact = 'addContact';
    $('.modal-title').text('添加');
    $('#Name').val('');
    $('#Job').val('');
    $('#ContactPhone').val('');
    $('#MobileTel').val('');
    $('#myModal').modal('show');
}

// 修改客户联系人
function edit_Click() {
    var rowId = getRows("#TableCContactId");
    if (rowId != null) {
        opContact = 'editContact';
        $('.modal-title').text('编辑');
        var rowData = $("#TableCContactId").jqGrid('getRowData', rowId);
        $('#customerID').val(rowId);
        $('#Name').val(rowData.Name);
        $('#Job').val(rowData.Job);
        $('#ContactPhone').val(rowData.Phone);
        $('#MobileTel').val(rowData.MobileTel);
        $('select[name=roleId]').append("<option value='" + rowData.RoleID + "'>" + rowData.RoleName + "</option>");
        $('#roleId').val(rowData.RoleID);
        $('#myModal').modal('show');
    }
    else {
        layer.msg("请至少选择一个项目");
        $('#myModal').modal('hide');
        return;
    }
}

// 删除客户联系人
var DelContact = function () {
    var rowId = getRows("#TableCContactId");
    if (rowId != null) {
        layer.confirm('您确定要删除选定的数据吗？', { icon: 3, title: '提示信息' }, function (index) {
            var index = layer.msg('正在删除，请稍候', { icon: 16, time: false, shade: 0.8 });
            $("#TableCContactId").jqGrid('delRowData', rowId);
            layer.close(index);
            layer.msg("删除成功");
        })
    }
    else {
        layer.msg("请至少选择一个项目");
    }
}

// 保存客户联系人
var SaveContact = function () {
    var flag = $('#from2').valid();
    if (!flag) {
        return;
    }
    var rowId = $("#TableCContactId").jqGrid("getGridParam", "selrow");
    var rowData = $("#TableCContactId").jqGrid('getRowData', rowId);
    var dataRow = {
        CustomerID: $('#customerID').val(),
        Name: $('#Name').val(),
        Job: $('#Job').val(),
        RoleID: $('#roleId').val(),
        RoleName: $('#roleId').find("option:selected").text(),
        Phone: $('#ContactPhone').val(),
        MobileTel: $('#MobileTel').val()
    };
    if (opContact == 'addContact') {
        if ($('#ContactPhone').val().length == 0 && $('#MobileTel').val().length == 0) {
            layer.alert('请填写电话或移动电话。');
            return;
        }
        $("#TableCContactId").jqGrid("addRowData", 0, dataRow, "last");
    }
    else {
        if (rowId == 0) {
            $("#TableCContactId").jqGrid("setRowData", 0, dataRow, "after", rowId);
        }
        else {
            $("#TableCContactId").jqGrid('setRowData', rowData.PKID, dataRow);
        }
    }
    layer.msg('保存成功。');
    $('#myModal').modal('hide');
}

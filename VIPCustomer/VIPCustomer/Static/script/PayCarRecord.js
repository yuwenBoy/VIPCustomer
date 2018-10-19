$(function () {
    initPage();
});

var initPage = function () {
    $('#table_list').jqGrid({
        url: URL,
        mtype: 'post',
        datatype: 'json',
        viewrecords: true,  //  是否显示总记录数
        styleUI: 'Bootstrap',
        colModel: [
              { label: '主键ID', name: 'PKID', hidden: true, key: true, },
              { label: '配车情况', name: 'PKID', width: 80 },
              {
                  label: '订单状态', name: 'ToExamineState', width: 120, formatter: function (cellvalue, options, rowObject) {
                      switch (rowObject.ToExamineState) {
                          case 200:
                              return '<span style="color:red;">未审核</span>';
                          case -200:
                              return '<span style="color:red;">已驳回</span>';
                          default:
                              return '已审核';
                      }
                  }
              },
            { label: '记录类别', name: 'Code', width: 130 },
            { label: '配车状态', name: 'DealerName', width: 120 },
            { label: '记录编号', name: 'Code', width: 160 },
            { label: '客户名称', name: 'CreateTime', width: 120 },
            { label: '车名', name: 'CreateTime', width: 120 },
            { label: '申请数量', name: 'Replyer3', width: 120 },
            { label: '已对应', name: 'ReComment3', width: 120 },
            { label: '未对应', name: 'CreateTime', width: 120, },
            { label: '上报日期', name: 'CreateTime', width: 120, formatter: "date", formatoptions: { newformat: 'Y-m-d' } },

        ],
        autowidth: true,
        height: 568,
        shrinkToFit: false,
        rownumbers: true,   // 显示行号
        rowNum: 50,
        rowList: [50, 100, 200],
        pager: "#pager_list",
        jsonReader: {
            records: "totalCount", root: "dataList", total: "totalpages", page: "currPage",
            repeatitems: false
        }
    });
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Utilities
{
    /// <summary>
    /// 流程名称
    /// </summary>
    public enum enum流程名称枚举
    {
        订单生成 = 0,
        订单提交,
        地担审批通过,
        地担审批驳回,
        大区经理审批通过,
        大区经理审批驳回,
        大客户室审批通过,
        大客户室审批驳回,
        订单配车,
        订单交车,
        大客户室返款审批通过,
        大客户室返款审批驳回,
        订单返款,
        订单年终补款,
        订单作废
    }
    /// <summary>
    /// 订单状态
    /// </summary>
    public enum 订单状态
    {
        订单作废 = -1000,
        订单生成 = 0,
        大区审核 = 100,
        大区驳回 = -100,
        大客户审核 = 200,
        大客户驳回 = -200,
        大客户审过 = 300,
        配车队列 = 310,
        上交车信息 = 400,
        返款队列 = 410,
        业务完成 = 1000
    }
}

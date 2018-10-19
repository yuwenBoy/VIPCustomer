using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Common.Entity
{
    public class EntityBase
    {
        /// <summary>
        /// 将对象序列化为Json格式
        /// </summary>
        /// <returns></returns>
        public string ToJsonString()
        {
            return new JavaScriptSerializer().Serialize(this);
        }
    }
}

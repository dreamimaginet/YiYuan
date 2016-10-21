using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web.Script.Serialization;

namespace YiYuan.Extensions
{
    public static class Generic
    {
        /// <summary>
        /// 字符串参数格式化
        /// </summary>
        /// <param name="s"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static string Formats(this string s, params object[] args)
        {
            return string.Format(s, args);
        }
    }
}

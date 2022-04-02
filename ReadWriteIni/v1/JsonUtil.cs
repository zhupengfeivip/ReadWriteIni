using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Runtime.Serialization.Json;

namespace ReadWriteIni.v1
{
    public class JsonUtil
    {
        /// <summary>
        /// 将object转换为json字符串,obj为方法实体类
        /// </summary>
        /// <param name="obj">方法实体类</param>
        /// <returns></returns>
        public static string ObjectToJson(object obj)
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(obj.GetType());
            using (MemoryStream ms = new MemoryStream())
            {
                serializer.WriteObject(ms, obj);
                StringBuilder sb = new StringBuilder();
                sb.Append(Encoding.UTF8.GetString(ms.ToArray()));
                return sb.ToString();
            }
        }
        /// <summary>
        /// 将json字符串转化为方法实体类
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsonString"></param>
        /// <returns></returns>
        public static T JsonToObject<T>(string jsonString)
        {
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
            MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(jsonString));
            T jsonObject = (T)ser.ReadObject(ms);
            ms.Close();
            return jsonObject;
        }
        /// <summary>
        /// 规范的json建对是这样："名称":"值",本方法将json字符串中的名称添加双引号
        /// </summary>
        /// <param name="jsonString">json字符串</param>
        /// <returns></returns>
        public static string JsonFormat(string jsonString)
        {
            //(?<=:)[\w]+(?=,)  以:开头，以,结尾中间的字符
            return Regex.Replace(jsonString, "\\b[\\w]+(?=:\"[^\"]*\")", "\"$0\"");//连续字母或数字以:结尾且不包含:
        }
        /// <summary>
        /// 从外围截取边界内内容,StripTextTo(jonstr,"[", "]");
        /// </summary>
        /// <param name="jsonString"></param>
        /// <param name="sChr"></param>
        /// <param name="eChr"></param>
        /// <returns></returns>
        public string StripTextTo(string jsonString, char sChr, char eChr)
        {
            string stripTextTo = jsonString;
            int num = stripTextTo.IndexOf(sChr);
            int num2 = stripTextTo.LastIndexOf(eChr);
            int num3 = num2 - num + 1;
            if (num3 > 1)
            {
                stripTextTo = stripTextTo.Substring(num, num3);
            }
            return stripTextTo;
        }
    }
}

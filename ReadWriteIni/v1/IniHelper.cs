using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;

namespace ReadWriteIni.v1
{
    /// <summary>
    /// 
    /// </summary>
    public class IniHelper
    {
        /// <summary>
        /// 
        /// </summary>
        private string filePath;

        /// <summary>
        /// 
        /// </summary>
        public Dictionary<string, Dictionary<string, string>> dictConfig = new Dictionary<string, Dictionary<string, string>>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name=""></param>
        public IniHelper(string path)
        {
            filePath = path;
        }

        /// <summary>
        /// 写文件锁
        /// </summary>
        private object _lockObj = new object();

        /// <summary>
        /// 将一个对象按XML序列化的方式写入到一个文件，即保存配置信息到ini文件
        /// </summary>
        /// <param name="o">要序列化的对象</param>
        /// <param name="path">保存文件路径</param>
        /// <param name="encoding">编码方式</param>
        public void SerializeToFile(object obj)
        {
            Monitor.Enter(_lockObj);//添加排他锁，解决并发写入的问题
            try
            {
                Dictionary<string, List<string>> dict = new Dictionary<string, List<string>>();
                Type t = obj.GetType();
                foreach (PropertyInfo pi in t.GetProperties())
                {
                    object[] attrs = pi.GetCustomAttributes(typeof(GroupAttribute), true);
                    if (attrs.Length == 1)
                    {
                        GroupAttribute attr = (GroupAttribute)attrs[0];
                        string group = string.IsNullOrWhiteSpace(attr.Group) ? "System" : attr.Group;
                        string key = string.IsNullOrWhiteSpace(attr.Name) ? pi.Name : attr.Name;
                        if (dict.ContainsKey(group) == false)
                        {
                            //不存在时创建
                            dict.Add(group, new List<string>());
                        }

                        if (pi.PropertyType == typeof(byte) || pi.PropertyType == typeof(byte?)
                            || pi.PropertyType == typeof(ushort) || pi.PropertyType == typeof(ushort?)
                            || pi.PropertyType == typeof(short) || pi.PropertyType == typeof(short?)
                            || pi.PropertyType == typeof(int) || pi.PropertyType == typeof(int?)
                            || pi.PropertyType == typeof(decimal) || pi.PropertyType == typeof(decimal?)
                            || pi.PropertyType == typeof(string)
                            || pi.PropertyType == typeof(bool) || pi.PropertyType == typeof(bool?)
                            || pi.PropertyType == typeof(DateTime) || pi.PropertyType == typeof(DateTime?)
                            )
                        {
                            if (string.IsNullOrWhiteSpace(attr.Comment) == false)
                                dict[group].Add($"# {attr.Comment}");
                            dict[group].Add($"{key}={pi.GetValue(obj, null)}");
                        }
                        else if (pi.PropertyType == typeof(List<byte>) || pi.PropertyType == typeof(List<short>)
                            || pi.PropertyType == typeof(List<ushort>) || pi.PropertyType == typeof(List<int>)
                            || pi.PropertyType == typeof(List<int?>) || pi.PropertyType == typeof(List<string>)
                            )
                        {
                            if (string.IsNullOrWhiteSpace(attr.Comment) == false)
                                dict[group].Add($"# {attr.Comment}");
                            dict[group].Add($"{key}={JsonUtil.ObjectToJson(pi.GetValue(obj, null))}");
                        }
                        else if (pi.PropertyType.IsGenericType && pi.PropertyType.GetGenericTypeDefinition().Equals(typeof(List<>)))
                        {
                            if (string.IsNullOrWhiteSpace(attr.Comment) == false)
                                dict[group].Add($"# {attr.Comment}");
                            dict[group].Add($"{key}={JsonUtil.ObjectToJson(pi.GetValue(obj, null))}");
                        }
                        else
                        {
                            if (string.IsNullOrWhiteSpace(attr.Comment) == false)
                                dict[group].Add($"# {attr.Comment}");
                            dict[group].Add($"{key}={pi.GetValue(obj, null)}");
                        }
                    }
                }

                WriteTxtFile(dict);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                Monitor.Exit(_lockObj);
            }
        }

        /// <summary>
        /// 从字符串中反序列化对象，是从ini文件读取配置信息
        /// </summary>
        /// <typeparam name="T">结果对象类型</typeparam>
        /// <param name="s">包含对象的XML字符串</param>
        /// <param name="encoding">编码方式</param>
        /// <returns>反序列化得到的对象</returns>
        public void Deserialize<T>(ref T config)
        {

            string[] lines = File.ReadAllLines(filePath);
            string curGroup = "";
            foreach (string line in lines)
            {
                //空白行跳过
                if (string.IsNullOrEmpty(line.Replace(" ", "").Trim())) continue;

                //注释信息跳过不处理
                if (line.StartsWith("#")) continue;
                if (line.StartsWith("["))
                {
                    //分组信息
                    curGroup = line.Replace("[", "").Replace("]", "").Trim();

                    if (dictConfig.ContainsKey(curGroup))
                        throw new Exception("分组名称重复");

                    dictConfig.Add(curGroup, new Dictionary<string, string>());
                }
                else
                {
                    if (string.IsNullOrEmpty(curGroup)) continue;  //无分组时跳过，舍弃这条信息
                                                                   //配置信息
                    string[] spStr = line.Split('=');
                    if (spStr.Length < 2) continue; //异常时，舍弃这条信息

                    string name = spStr[0];
                    string value = spStr[1];
                    dictConfig[curGroup].Add(name, value);
                }
            }

            Type t = config.GetType();
            foreach (PropertyInfo pi in t.GetProperties())
            {
                object[] attrs = pi.GetCustomAttributes(typeof(GroupAttribute), true);
                if (attrs.Length == 1)
                {
                    GroupAttribute attr = (GroupAttribute)attrs[0];
                    string group = string.IsNullOrWhiteSpace(attr.Group) ? "System" : attr.Group;
                    string key = string.IsNullOrWhiteSpace(attr.Name) ? pi.Name : attr.Name;

                    if (pi.PropertyType == typeof(int?))
                    {
                        pi.SetValue(config, Convert.ChangeType(dictConfig[group][key], typeof(int)), null);
                    }
                    else if (pi.PropertyType == typeof(decimal?))
                    {
                        pi.SetValue(config, Convert.ChangeType(dictConfig[group][key], typeof(decimal)), null);
                    }
                    else if (pi.PropertyType == typeof(DateTime?))
                    {
                        pi.SetValue(config, Convert.ChangeType(dictConfig[group][key], typeof(DateTime)), null);
                    }
                    else if (pi.PropertyType == typeof(List<byte>))
                    {
                        pi.SetValue(config, Convert.ChangeType(JsonUtil.JsonToObject<List<byte>>(dictConfig[group][key]), typeof(List<byte>)), null);
                    }
                    else if (pi.PropertyType == typeof(List<short>))
                    {
                        pi.SetValue(config, Convert.ChangeType(JsonUtil.JsonToObject<List<short>>(dictConfig[group][key]), typeof(List<short>)), null);
                    }
                    else if (pi.PropertyType == typeof(List<ushort>))
                    {
                        pi.SetValue(config, Convert.ChangeType(JsonUtil.JsonToObject<List<ushort>>(dictConfig[group][key]), typeof(List<ushort>)), null);
                    }
                    else if (pi.PropertyType == typeof(List<int>))
                    {
                        pi.SetValue(config, Convert.ChangeType(JsonUtil.JsonToObject<List<int>>(dictConfig[group][key]), typeof(List<int>)), null);
                    }
                    else if (pi.PropertyType == typeof(List<int?>))
                    {
                        pi.SetValue(config, Convert.ChangeType(JsonUtil.JsonToObject<List<int?>>(dictConfig[group][key]), typeof(List<int?>)), null);
                    }
                    else if (pi.PropertyType == typeof(List<string>))
                    {
                        pi.SetValue(config, Convert.ChangeType(JsonUtil.JsonToObject<List<string>>(dictConfig[group][key]), typeof(List<string>)), null);
                    }
                    else if (pi.PropertyType == typeof(List<T>))
                    {
                        pi.SetValue(config, Convert.ChangeType(JsonUtil.JsonToObject<List<T>>(dictConfig[group][key]), typeof(List<T>)), null);
                    }
                    else if (pi.PropertyType == typeof(List<string>))
                    {
                        pi.SetValue(config, Convert.ChangeType(JsonUtil.JsonToObject<List<string>>(dictConfig[group][key]), typeof(List<string>)), null);
                    }
                    else
                    {
                        //string value = dict[attr.Group][name];
                        //object obj = JsonUtil.JsonToObject<object>(value);
                        //object v = Convert.ChangeType(obj, pi.GetType());
                        //pi.GetProperty(FieldName).SetValue(obj, v, null);
                        //pi.SetValue(config, v, null);
                        //pi.SetValue(config, Convert.ChangeType(dict[attr.Group][name], pi.PropertyType), null);
                        //throw new Exception("暂不支持的数据类型，请在github上反馈留言");
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="strMsg"></param>
        private void WriteTxtFile(Dictionary<string, List<string>> dict)
        {
            using (StreamWriter sw = File.CreateText(filePath))
            {
                foreach (var item in dict)
                {
                    sw.WriteLine($"[{item.Key}]");
                    foreach (string str in item.Value)
                    {
                        sw.WriteLine(str);
                    }
                }

            }
        }


    }
}

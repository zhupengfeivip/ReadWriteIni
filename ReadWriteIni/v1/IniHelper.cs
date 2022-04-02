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
                    string name = pi.Name.ToLower();

                    object[] attrs = pi.GetCustomAttributes(typeof(GroupAttribute), true);
                    if (attrs.Length == 1)
                    {
                        GroupAttribute attr = (GroupAttribute)attrs[0];

                        if(dict.ContainsKey(attr.Group) == false)
                        {
                            //不存在时创建
                            dict.Add(attr.Group, new List<string>());
                            dict[attr.Group].Add($"# {attr.Comment}");
                            dict[attr.Group].Add($"{attr.Name}={pi.GetValue(obj)}");
                        }
                        else
                        {
                            //存在时添加
                            dict[attr.Group].Add($"# {attr.Comment}");
                            dict[attr.Group].Add($"{attr.Name}={pi.GetValue(obj)}");
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
            Dictionary<string, Dictionary<string,string>> dict = new Dictionary<string, Dictionary<string, string>>();
            string[] lines = File.ReadAllLines(filePath);
            string curGroup = "";
            foreach (string line in lines)
            {
                //空白行跳过
                if (string.IsNullOrWhiteSpace(line.Replace(" ", "").Trim())) continue;

                //注释信息跳过不处理
                if (line.StartsWith("#")) continue;                
                if(line.StartsWith("["))
                {
                    //分组信息
                    curGroup = line.Replace("[", "").Replace("]", "").Trim();

                    if (dict.ContainsKey(curGroup))
                        throw new Exception("分组名称重复");

                    dict.Add(curGroup, new Dictionary<string, string>());
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(curGroup)) continue;  //无分组时跳过，舍弃这条信息
                    //配置信息
                   string[] spStr = line.Split("=");
                    if (spStr.Length < 2) continue; //异常时，舍弃这条信息

                    string name = spStr[0];
                    string value = spStr[1];
                    dict[curGroup].Add(name, value);
                }
            }

            Type t = config.GetType();
            foreach (PropertyInfo pi in t.GetProperties())
            {
                string name = pi.Name;

                object[] attrs = pi.GetCustomAttributes(typeof(GroupAttribute), true);
                if (attrs.Length == 1)
                {
                    GroupAttribute attr = (GroupAttribute)attrs[0];

                    if (pi.PropertyType == typeof(int?))
                    {
                        pi.SetValue(config, Convert.ChangeType(dict[attr.Group][name], typeof(int)), null);
                    }
                    else if (pi.PropertyType == typeof(decimal?))
                    {
                        pi.SetValue(config, Convert.ChangeType(dict[attr.Group][name], typeof(decimal)), null);
                    }
                    else if (pi.PropertyType == typeof(DateTime?))
                    {
                        pi.SetValue(config, Convert.ChangeType(dict[attr.Group][name], typeof(DateTime)), null);
                    }
                    else
                    {
                        pi.SetValue(config, Convert.ChangeType(dict[attr.Group][name], pi.PropertyType), null);
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
            using (StreamWriter sw = File.CreateText(this.filePath))
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

using ReadWriteIni.v1;
using System.Collections.Generic;

namespace Demo
{
    /// <summary>
    /// 测试配置文件
    /// </summary>
    public class Config
    {
        ///// <summary>
        ///// 数字配置项
        ///// </summary>
        //[Group(Group = "system", Name = "Number1", Comment = "数字配置项")]
        //public int Number1 { get; set; } = 1;

        ///// <summary>
        ///// 小票打印机
        ///// </summary>
        //[Group(Group = "system", Name = "NotePrinter", Comment = "小票打印机")]
        //public string NotePrinter { get; set; } = "defaultValue";


        ///// <summary>
        ///// 是否使用帧分离和拼接的功能 True-使用；False-不使用
        ///// </summary>
        //[Group(Group = "system", Name = "EnableSplitFrames", Comment = " True-使用；False-不使用")]
        //public bool EnableSplitFrames { get; set; } = true;


        /// <summary>
        /// 是否使用帧分离和拼接的功能 True-使用；False-不使用
        /// </summary>
        [Group(Group = "system", Name = "TestStringList", Comment = " True-使用；False-不使用")]
        public List<string> TestStringList { get; set; } = new List<string>() { "test", "test2" };


    }
}

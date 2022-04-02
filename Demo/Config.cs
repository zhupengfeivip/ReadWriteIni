using ReadWriteIni.v1;

namespace Demo
{
    /// <summary>
    /// 测试配置文件
    /// </summary>
    public class Config
    {
        /// <summary>
        /// 数字配置项
        /// </summary>
        [Group(Group = "system", Name = "Number1", Comment = "数字配置项")]
        public int Number1 { get; set; }

        /// <summary>
        /// 小票打印机
        /// </summary>
        [Group(Group = "system", Name = "NotePrinter", Comment = "小票打印机")]
        public string NotePrinter { get; set; }

    }
}

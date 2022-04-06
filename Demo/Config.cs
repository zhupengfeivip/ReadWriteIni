using ReadWriteIni.v1;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;

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
        [Group(Group = "System", Comment = "数字配置项")]
        public int Number1 { get; set; } = 1;

        /// <summary>
        /// 数据库类型 1 sqllite 2 mysql 3 mssql 9 access
        /// </summary>
        [Group(Group = "DbConfig", Comment = "数据库类型 2 sqllite 0 mysql 1 mssql")]
        public int DbType { get; set; } = 0;

        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        [Group(Group = "DbConfig", Comment = "数据库连接字符串")]
        public string DbConnString { get; set; } = "";

        /// <summary>
        /// 小票打印机
        /// </summary>
        public string NotePrinter { get; set; } = "defaultValue";


        /// <summary>
        /// 是否使用帧分离和拼接的功能 True-使用；False-不使用
        /// </summary>
        public bool EnableSplitFrames { get; set; } = true;


        /// <summary>
        /// 是否使用帧分离和拼接的功能 True-使用；False-不使用
        /// </summary>
        public List<string> TestStringList { get; set; } = new List<string>() { "test", "test2" };

        /// <summary>
        /// 设备列表 
        /// </summary>
        [Group()]
        public List<CommDevice> ComDeviceList { get; set; } = new List<CommDevice>();


    }

    /// <summary>
    /// 
    /// </summary>
    public class CommDevice
    {
        /// <summary>
        /// 
        /// </summary>
        public byte Addr = 1;

        /// <summary>
        /// 
        /// </summary>
        public string PortName = "COM1";
    }
}

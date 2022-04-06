# ReadWriteIni

一个读写 ini 文件的配置帮助类。可以将类文件序列化为 ini 文件，也可以将一个 ini 文件反序列化为一个类文件。

与普通 ini 读写类的区别是

1. 支持写 ini 注释
2. 自动序列和反序列化，不需要人工写转换代码

具体使用方法如下：

## 依赖环境

.NETFramework4.0 以上都可以使用

## 声明一个类文件，当作配置文件

```c#
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
        [Group(Group = "system", Comment = "数字配置项")]
        public int Number1 { get; set; }

        /// <summary>
        /// 小票打印机
        /// </summary>
        [Group(Group = "system", Comment = "小票打印机")]
        public string NotePrinter { get; set; }

    }
}


```

## 写配置文件

```c#

            Config config = new Config();
            config.Number1 = 99;
            config.NotePrinter = "test";

            string path = Environment.CurrentDirectory + "\\config.ini";
            ReadWriteIni.v1.IniHelper ini = new ReadWriteIni.v1.IniHelper(path);

            //写配置文件
            ini.SerializeToFile(config);
```

运行后，即在程序目录中出现一个 ini 的文件，内容如下

```ini
[system]
# 数字配置项
Number1=99
# 小票打印机
NotePrinter=test

```

## 读配置文件

```c#

            Config config = new Config();
            config.Number1 = 99;
            config.NotePrinter = "test";

            string path = Environment.CurrentDirectory + "\\config.ini";
            ReadWriteIni.v1.IniHelper ini = new ReadWriteIni.v1.IniHelper(path);

            //写配置文件
            ini.SerializeToFile(config);

            //读配置文件
            ini.Deserialize(ref config);

            Console.WriteLine($"Number1:{config.Number1}");
            Console.WriteLine($"NotePrinter:{config.NotePrinter}");

```

完全不用自己写任何转换的代码，明白怎么使用了吗？

# 目前支持序列化的数据类型

| 数据类型     |
| :----------- |
| byte         |
| short        |
| ushort       |
| int          |
| string       |
| bool         |
| List<byte>   |
| List<string> |
| List<short>  |
| List<int>    |

## 支持自定义类配置信息

```c#
    /// <summary>
    /// 测试配置文件
    /// </summary>
    public class Config
    {
        /// <summary>
        /// 设备列表
        /// </summary>
        [Group(Group = "system", Comment = " True-使用；False-不使用")]
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
```

读写配置文件示例

```c#
            Config config = new Config();
            config.ComDeviceList.Add(new CommDevice()) ;
            config.ComDeviceList.Add(new CommDevice());

            string path = Environment.CurrentDirectory + "\\config.ini";
            ReadWriteIni.v1.IniHelper ini = new ReadWriteIni.v1.IniHelper(path);

            //写配置文件
            ini.SerializeToFile(config);

            Config readValue = new Config();
            //读配置文件
            ini.Deserialize(ref readValue);
            //由于自定义类暂时无法自动转换，所以这里手动转换一下，把JSON字符串转换为类对象
            readValue.ComDeviceList = JsonUtil.JsonToObject<List<CommDevice>>(ini.dictConfig["system"]["ComDeviceList"]);
```

## 自定义属性说明

不加自定义属性时认为不保存到 ini 配置文件，即不可配置，内置的固定配置项。

| 字段名  | 是否必填 | 说明                                           |
| :------ | -------- | ---------------------------------------------- |
| Group   | 选填项   | 分组名称，不填时默认分组为 System              |
| Name    | 选填项   | 配置名称，不填时为字段名称，填写的话取填写的值 |
| Comment | 选填项   | 注释说明，不填不写注释                         |

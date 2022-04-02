# ReadWriteIni

一个读写 ini 文件的配置帮助类。可以将类文件序列化为 ini 文件，也可以将一个 ini 文件反序列化为一个类文件。

与普通 ini 读写类的区别是

1. 支持写 ini 注释
2. 自动序列和反序列化，不需要人工写转换代码

具体使用方法如下：

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
        [Group(Group = "system", Name = "Number1", Comment = "数字配置项")]
        public int Number1 { get; set; }

        /// <summary>
        /// 小票打印机
        /// </summary>
        [Group(Group = "system", Name = "NotePrinter", Comment = "小票打印机")]
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

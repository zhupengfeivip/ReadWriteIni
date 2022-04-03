using System;
using System.Collections.Generic;

namespace Demo
{
    class Program
    {
        static void Main(string[] args)
        {

            Config config = new Config();
            //config.Number1 = 99;
            //config.NotePrinter = "test";
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
            readValue.ComDeviceList = JsonUtil.JsonToObject<List<CommDevice>>(ini.dictConfig["System"]["ComDeviceList"]);

            Console.WriteLine($"zhupengfei");

            //Console.WriteLine($"Number1:{config.Number1}");
            //Console.WriteLine($"NotePrinter:{config.NotePrinter}");
        }
    }
}

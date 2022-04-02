using System;

namespace Demo
{
    class Program
    {
        static void Main(string[] args)
        {

            Config config = new Config();
            config.Number1 = 99;
            config.NotePrinter = "test";

            string path = Environment.CurrentDirectory + "\\config.ini";          
            ReadWriteIni.v1.IniHelper ini = new ReadWriteIni.v1.IniHelper(path);

            //写配置文件
            //ini.SerializeToFile(config);

            //读配置文件
            ini.Deserialize(ref config);

            Console.WriteLine($"Number1:{config.Number1}");
            Console.WriteLine($"NotePrinter:{config.NotePrinter}");
        }
    }
}

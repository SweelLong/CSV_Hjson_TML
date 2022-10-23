using System;
using System.Reflection;

namespace CSV_Hjson_TML
{
    internal class Program
    {
        internal static void Main(string[] args)
        {
            AppDomain.CurrentDomain.AssemblyResolve += (sender, arg) =>
            {
                var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("CSV_Hjson_TML.Hjson.dll"); 
                byte[] assData = new byte[stream.Length];
                stream.Read(assData, 0, assData.Length);
                return Assembly.Load(assData);
            };
            Console.Title = AppDomain.CurrentDomain.FriendlyName;
            if (args.Length < 1)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("打开方式错误！\n还是去看看教程吧！");
                Console.ReadKey();
                return;
            }
            Console.Write("请选择转换模式：\nCSV转Hjson【1】\nHjson转CSV【2】\n：");
            string mode = Console.ReadLine();
            if (mode == "1")
            {
                Core.ToHjson(args[0]);
            }
            else if (mode == "2")
            {
                Core.ToCSV(args[0]);
            }
            else
            {
                Console.WriteLine("没有该模式！");
            }
            Console.ReadKey();
        }
    }
}
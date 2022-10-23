using Hjson;
using System;
using System.Collections.Generic;
using System.IO;

namespace CSV_Hjson_TML
{
    internal class Core
    {
        internal static List<string> waitJoin = new List<string>();

        internal static void ToCSV(string path)
        {
            StreamWriter sw = new StreamWriter(Path.GetFileNameWithoutExtension(path) + ".csv");
            var jsonString = HjsonValue.Load(path).Qo().Qo("Mods").Qo("InterspaceMod");
            sw.WriteLine("键名：,命名空间：,语言文本：");
            foreach (var item in HjsonValue.Parse(jsonString.ToString()).Qo())
            {
                foreach (var i in HjsonValue.Parse(jsonString.Qo().Qo(item.Key).ToString()).Qo())
                {
                    sw.WriteLine(item.Key + "," + i.Key + "," + i.Value.ToString().Replace("\n", "\\n"));
                }
            }
            sw.Flush();
            sw.Dispose();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("转换成功");
        }

        internal static string Lang(string i)
        {
            switch (i)
            {
                case "2":return "zh-Hans";
                case "3": return "en-US";
                case "4": return "de-DE";
                case "5": return "it-IT";
                case "6": return "fr-FR";
                case "7": return "es-ES";
                case "8": return "ru-RU";
                case "9": return "pt-BR";
                case "10": return "pl-PL";
                default: return null;
            }
        }

        internal static void ToHjson(string path)
        {
            Console.Write("请选定CSV的语言列：\n【2】中文【3】英语【4】德语\n【5】意大利语【6】法语【7】西班牙语\n【8】俄语【9】葡萄牙语【10】波兰语");
            string count = Console.ReadLine();
            List<string> ParentKeys = new List<string>();
            Dictionary<string, string> Keys = new Dictionary<string, string>();
            string[] lines = File.ReadAllLines(path);
            StreamWriter sw = new StreamWriter(Lang(count) + ".hjson");
            sw.WriteLine("Mods: ");
            sw.WriteLine("{");
            sw.WriteLine("	InterspaceMod: ");
            sw.WriteLine("	{");
            foreach (string temp in lines)
            {
                string line = temp;
                if (line.Substring(0, 1) == "#")
                {
                    continue;
                }
                if (line.Contains("\""))
                {
                    line = FixText(line);
                }
                string[] parts = line.Split(',');
                if (parts[0].Trim() == "")
                {
                    continue;
                }
                if (!ParentKeys.Contains(parts[0]))
                {
                    ParentKeys.Add(parts[0]);
                }
                Keys.Add($"{parts[0]}{parts[1]}: \"{parts[int.Parse(count)]}\"", parts[0]);
            }
            foreach (var str in ParentKeys.ToArray())
            {
                sw.WriteLine("		" + str + ":");
                sw.WriteLine("		{");
                foreach (var a in Keys)
                {
                    if (a.Value == str)
                    {
                        sw.WriteLine($"			{a.Key.Replace(a.Value, "")}");
                    }
                }
                sw.WriteLine("		}");
            }
            sw.WriteLine("	}");
            sw.WriteLine("}");
            sw.Flush();
            sw.Dispose();
            string v = File.ReadAllText(Lang(count) + ".hjson");
            for (int i = 0; i < waitJoin.Count; i++)
            {
                v.Replace("{-"+ i +"-}", waitJoin[i]);
            }
            File.WriteAllText(Lang(count) + ".hjson", v);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("转换成功");
        }

        internal static string FixText(string line)
        {
            char[] fix = line.ToCharArray();
            byte tag = 0;
            int last = 0;
            for (int i = 0; i < fix.Length; i++)
            {
                if (fix[i] == '\"')// 如果字符是引号
                {
                    tag++;// 标记一下
                    if (tag == 1)
                    {
                        last = i;// 记录标记的索引
                    }
                    if (fix[i + 1] == '"' && fix[i + 2] == '"')//如果接下来的两个字符都是引号则取消标记
                    {
                        tag = 0;
                        last = 0;
                        i += 3;
                    }
                    if (tag == 2)// 如果连续被标记两下
                    {
                        string tagText = line.Substring(last, i - last + 1);
                        line = line.Remove(last, i - last + 1);
                        line = line.Insert(last, "{-" + waitJoin.Count + "-}");
                        waitJoin.Add(tagText);
                        return FixText(line);
                    }
                }
            }
            return line;
        }
    }
}
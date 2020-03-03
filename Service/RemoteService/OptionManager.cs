using System;
using System.IO;

namespace RemoteService
{
    public class OptionManager
    {
        public const string OptionPath = "./config.ini";
        private readonly string[] optionTooltip = {"ExecutionPath","ExecutionCommand"};
        private readonly string[] optionDefault = {"./spigot.jar","java -Xmx1G -jar"};
        private string[] optionData;

        public OptionManager()
        {
            optionData = new string[(int) Options.Length];
            ReadFile();
        }

        public void ReadFile()
        {
            FileStream fs = new FileStream(OptionPath, FileMode.OpenOrCreate);
            if (fs.Length == 0)
            {
                CreateDefaultOption(fs);
                fs=new FileStream(OptionPath,FileMode.Open);
            }
            StreamReader reader = new StreamReader(fs);
            while (!reader.EndOfStream)
            {
                string[] line = reader.ReadLine()?.Split('=');
                if (line != null && line.Length != 2) continue;
                if (line[0].StartsWith("//")) continue;
                for (int i = 0; i < optionTooltip.Length; i++)
                {
                    if (string.Equals(line[0], optionTooltip[i], StringComparison.OrdinalIgnoreCase))
                    {
                        optionData[i] = line[1];
                        Console.WriteLine($"Loaded {optionTooltip[i]} = {optionData[i]}");
                        break;
                    }
                }
            }

            reader.Close();
            fs.Close();
        }

        private void CreateDefaultOption(FileStream fs)
        {
            StreamWriter writer = new StreamWriter(fs);
            writer.WriteLine("//Put options below");
            writer.WriteLine("//These lines will be ignored");
            for (int i = 0; i < optionTooltip.Length; i++)
            {
                writer.WriteLine(optionTooltip[i]+'='+optionDefault[i]);
            }

            writer.Flush();
            writer.Close();
            fs.Close();
        }

        public string GetOption(Options optionType) => optionData[(int) optionType];
    }

    public enum Options
    {
        ExecutionPath = 0,
        ExecutionCommand=1,
        Length = 2
    }
}
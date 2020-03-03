using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace RemoteService
{
    public class MCRemoteService
    {
        private ServerManager server;
        private OptionManager option;
        private TrayController tray;
        private const string Diff = "||";
        private Process runningTarget;

        public MCRemoteService()
        {
            option = new OptionManager();
            server = new ServerManager(3320, Callback);
            tray = new TrayController(this);
            Application.Run(tray);
            server.NowThread?.Join();
            tray.TrayIcon.Visible = false;
        }

        public void Callback(StreamReader reader, StreamWriter writer)
        {
            string[] commands = reader.ReadLine()?.Split(new[] {Diff}, StringSplitOptions.None);
            if (commands == null || commands.Length < 1) return;
            switch (commands[0])
            {
                case "Shutdown":
                    Shutdown(commands, writer);
                    break;
                case "Status":
                    GetStatus(commands, writer);
                    break;
                case "Run":
                    Run(commands, writer);
                    break;
                default:
                    UnknownSignalCallback(commands, writer);
                    break;
            }
        }

        private void UnknownSignalCallback(string[] commands, StreamWriter writer) => writer.WriteLine("UnknownSignal");

        private void UnknownParameterCallback(string[] commands, StreamWriter writer, string param) =>
            writer.WriteLine($"UnknownParameter : {param}");

        private void Run(string[] commands, StreamWriter writer)
        {
            writer.WriteLine("OK");
            runningTarget =
                RunCommand(option.GetOption(Options.ExecutionCommand) + option.GetOption(Options.ExecutionPath));
            Console.WriteLine(
                $">> {(option.GetOption(Options.ExecutionCommand) + option.GetOption(Options.ExecutionPath))}");
        }

        private void GetStatus(string[] commands, StreamWriter writer) {
            string ret = runningTarget != null && !runningTarget.HasExited ? "Running" : "StandBy";
            writer.WriteLine(ret);
            Console.WriteLine(ret);
        }

        public void ShutdownByUI() => Environment.Exit(0);

        public void RunByUI()
        {
            runningTarget =
                RunCommand(option.GetOption(Options.ExecutionCommand) + option.GetOption(Options.ExecutionPath));
            Console.WriteLine(
                $">> {(option.GetOption(Options.ExecutionCommand) + option.GetOption(Options.ExecutionPath))}");
        }

        public void OpenSettingByUI() => Process.Start(Path.GetFullPath(OptionManager.OptionPath));

        private void Shutdown(string[] commands, StreamWriter writer)
        {
            if (commands.Length > 1)
            {
                switch (commands[1])
                {
                    case "-poweroff":
                        RunCommand("shutdown -s -t 30");
                        break;
                    default:
                        UnknownParameterCallback(commands, writer, commands[1]);
                        return;
                }
            }

            writer.WriteLine("OK");
            Environment.Exit(0);
        }

        static Process RunCommand(string command, ProcessWindowStyle windowStyle = ProcessWindowStyle.Normal)
        {
            Process process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.WindowStyle = windowStyle;
            startInfo.CreateNoWindow = true;
            startInfo.ErrorDialog = true;
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = $"/c {command}";
            process.StartInfo = startInfo;
            process.Start();
            return process;
        }


        static void Main(string[] args)
        {
            new MCRemoteService();
        }

        public void ReloadOption()
        {
            option.ReadFile();
        }
    }

    public delegate void ServerCallBack(StreamReader reader, StreamWriter writer);
}
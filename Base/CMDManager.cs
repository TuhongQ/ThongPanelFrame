using System;
using System.Diagnostics;
using System.Security.Principal;

namespace ThongPanelFrame.Base
{
    public sealed class CMDManager
    {
        private static readonly Lazy<CMDManager> _singer =
               new Lazy<CMDManager>(() => new CMDManager());

        public static CMDManager Instance
        { get { return _singer.Value; } }

        private CMDManager()
        { }
        /// <summary>
        /// 执行Adb指令
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public string ExecuteAdbCommand(string command)
        {
            var identity = WindowsIdentity.GetCurrent();
            var principal = new WindowsPrincipal(identity);
            var hasAdminRights = principal.IsInRole(WindowsBuiltInRole.Administrator);
            //判断是否有管理员权限
            if (!hasAdminRights)
            {
                Console.WriteLine("无管理员权限！");
                return "无管理员权限";
            }
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = "adb.exe";
            startInfo.Arguments = command;
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardInput = true;
            startInfo.RedirectStandardError = true;
            startInfo.RedirectStandardOutput = true;
            startInfo.CreateNoWindow = true;
            startInfo.Verb = "runas";

            try
            {
                using (Process process = new Process())
                {

                    process.StartInfo = startInfo;
                    process.Start();
                    process.WaitForExit();
                    return process.StandardOutput.ReadToEnd();
                }
            }
            catch (Exception e)
            {

                throw;
            }
        }
    }
}
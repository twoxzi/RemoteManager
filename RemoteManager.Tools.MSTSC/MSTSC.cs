using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Twoxzi.RemoteManager.Entity;
using Twoxzi.RemoteManager.Tools;

namespace RemoteManager.Tools.MSTSC
{
    /// <summary>
    /// MSTSC
    /// </summary>
    [RemoteTool(ToolCode = MSTSC.ToolCode, Memo = "Windows的远程桌面连接,ID填写格式：远程链接#账号", ToolName = "远程桌面连接(RDP)")]
    public class MSTSC : RemoteToolBase
    {
        public const string ToolCode = "MSTSC";

        public override void Open(RemoteInfo info)
        {
            if (info == null)
            {
                return;
            }
            StringBuilder cmd = new StringBuilder();

            String exe = "mstsc";

            if (!String.IsNullOrEmpty(info.ExtensionProperty))
            {
                String fullName = GetTempFile(info);
                cmd.AppendFormat("\"{0}\" ", fullName);
            }
            if (!String.IsNullOrEmpty(info.ID) && !String.IsNullOrEmpty(info.Password))
            {
                String address = info.ID;
                String id = "";
                int index = info.ID.IndexOf("#");
                if (index >= 0)
                {
                    address = info.ID.Substring(0, index);
                    cmd.Append($" /v:{address}");

                    index++;
                    if (index < info.ID.Length)
                    {
                        id = info.ID.Substring(index);
                        // 增加远程凭据
                        var p = Process.Start(new ProcessStartInfo()
                        {
                            FileName = "cmdkey.exe",
                            Arguments = $"/generic:TERMSRV/{address} /user:{id} /pass:{info.Password}"
                        });
                        p.WaitForExit();
                    }
                }
            }
            // var process =
            Process.Start(exe, cmd.ToString());
            // process.WaitForExit();
        }

        protected Boolean SplitAddressAndUserName(String source,out String address,out String name)
        {
            address = null;
            name = null;
            if (String.IsNullOrEmpty(source))
            {
                return false;
            }

            int index = source.IndexOf("#");
            if (index >= 0)
            {
                address = source.Substring(0, index);
                name = source.Substring(index);
            }
            else
            {
                address = source;
            }

            return true;
        }

        /// <summary>
        /// 生成临时的RDP文件
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        protected String GetTempFile(RemoteInfo info)
        {
            if (info == null)
            {
                return "";
            }

            // 生成 rdp文件
            String fileName = (info?.ExtensionProperty ?? "").GetHashCode().ToString();
            String fullName = Path.GetTempPath();
            fullName = Path.Combine(fullName, $"{fileName}.rdp");

            String address;
            String name;
            SplitAddressAndUserName(info.ID, out address, out name);

            if (!File.Exists(fullName))
            {
                using (StreamWriter sw = File.CreateText(fullName))
                {
                    if (!String.IsNullOrEmpty(info.ExtensionProperty))
                    {
                        sw.WriteLine(info.ExtensionProperty);
                    }
                    else
                    {
                       // sw.WriteLine("full address:s:" + address);
                    }
                }
            }

            return fullName;
        }

        /// <summary>
        /// 设置扩展属性
        /// </summary>
        /// <param name="info"></param>
        public override void ExtensionPropertySetter(RemoteInfo info)
        {
            if (info == null)
            {
                return;
            }

            // 生成 rdp文件
            //String fileName = (info?.ExtensionProperty ?? "").GetHashCode().ToString();
            String fullName = GetTempFile(info);
            //fullName = $"{fullName}\\{fileName}.rdp";
            // 异步
            Task.Run(() =>
            {
                
                var process = Process.Start(new ProcessStartInfo(fullName) { Arguments="/edit", UseShellExecute = true });
                process.WaitForExit();
                using (StreamReader stream = File.OpenText(fullName))
                {
                    info.ExtensionProperty = stream.ReadToEnd();
                }
            });
        }
    }
}

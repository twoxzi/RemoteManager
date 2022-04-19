using System;
using System.Diagnostics;
using System.IO;
using Twoxzi.RemoteManager.Entity;
using Twoxzi.RemoteManager.Tools;

namespace RemoteManager.Tools
{
    [RemoteTool(ToolCode =ToDesk.ToolCode,ToolName ="ToDesk",Memo ="ToDesk桌面远程")]
    public class ToDesk : RemoteToolBase
    {
        const String ToolCode = "ToDesk";
        public override void Open(RemoteInfo info)
        {
            String exe = GetExePath("ToDesk");
            if (String.IsNullOrEmpty(exe))
            {
                throw new Exception("未找到工具ToDesk，请确保正确安装，启动已中止");
            }
            exe = Path.Combine(exe, "ToDesk.exe");
            Process.Start(exe, $"-control -id {info.ID} -passwd {info.Password}");
        }
    }
}

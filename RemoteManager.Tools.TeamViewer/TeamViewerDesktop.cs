using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Text;
using Twoxzi.RemoteManager.Entity;

namespace Twoxzi.RemoteManager.Tools.TeamViewer
{
    /// <summary>
    /// TeamViewer桌面远程
    /// </summary>
    [RemoteTool(ToolCode = TeamViewerDesktop.ToolCode, Memo = "TeamViewer桌面远程", ToolName = "TeamViewer桌面远程")]
    public class TeamViewerDesktop : RemoteToolBase
    {
        public const string ToolCode = "TVD";

        /// <summary>
        /// 打开远程
        /// </summary>
        /// <param name="info"></param>
        public override void Open(RemoteInfo info)
        {
            Open(info, info.ToolCode == ToolCode);
        }
        /// <summary>
        /// 打开远程
        /// </summary>
        /// <param name="info"></param>
        /// <param name="isDesktop"></param>
        protected void Open(RemoteInfo info, Boolean isDesktop)
        {
            String exe = GetExePath("TeamViewer");

            if(String.IsNullOrEmpty(exe))
            {
                throw new Exception("工具TeamViewer未找到，请确保正确安装，启动已中止");
            }
            exe = Path.Combine(exe, "TeamViewer.exe");
            Process.Start(exe, $"-i {info.ID} -P {info.Password} {(isDesktop ? "" : " -m fileTransfer")}");
        }
    }
}

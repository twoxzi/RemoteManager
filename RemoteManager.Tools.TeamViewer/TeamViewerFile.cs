using System;
using System.Collections.Generic;
using System.Text;

namespace Twoxzi.RemoteManager.Tools.TeamViewer
{
    /// <summary>
    /// 文件传输
    /// </summary>
    [RemoteTool(ToolCode = TeamViewerFile.ToolCode, Memo = "TeamViewer文件", ToolName = "TeamViewer文件")]
    public class TeamViewerFile : TeamViewerDesktop
    {
        public new const string ToolCode = "TVF";
    }
}

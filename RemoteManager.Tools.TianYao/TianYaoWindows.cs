using System;
using System.Diagnostics;
using System.IO;
using Twoxzi.RemoteManager.Entity;

namespace Twoxzi.RemoteManager.Tools.TianYao
{
    [RemoteTool(ToolCode = TianYaoWindows.ToolCode,ToolName ="天玥运维Windows",Memo ="")]
    public class TianYaoWindows : RemoteToolBase
    {
        const string ToolCode = "TianYaoWindows";
        public override void Open(RemoteInfo info)
        {
            if(info == null || String.IsNullOrWhiteSpace(info.ExtensionProperty))
            {
                throw new ArgumentNullException(nameof(info),"未设置令牌，请打开扩展属性进行设置");
            }
            String fileName = (info.ExtensionProperty ?? "").PadRight(8, '0').Substring(0, 8) + ".tmp";
            using (FileStream fs = File.Create(fileName))
            {
                Byte[] bs = Convert.FromBase64String(info.ExtensionProperty);
                fs.Write(bs, 0, bs.Length);
            }
            Process.Start("tunnel", $"-f \"{fileName}\"");
        }

        public override void ExtensionPropertySetter(RemoteInfo info)
        {
            TokenPage token = new TokenPage();
            token.DataContext = new TokenPageViewModel(info);
            token.ShowDialog();
        }
    }
}

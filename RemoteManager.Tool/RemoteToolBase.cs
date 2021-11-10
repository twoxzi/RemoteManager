using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Text;
using Twoxzi.RemoteManager.Entity;

namespace Twoxzi.RemoteManager.Tools
{
    public abstract class RemoteToolBase : IRemoteTool
    {
        public virtual void ExtensionPropertySetter(RemoteInfo info) { }

        public abstract void Open(RemoteInfo info);

        /// <summary>
        /// 获取软件路径，先从配置文件中获取，其次从注册表中获取，如果是在注册表中获取到，则把路径保存到配置文件中
        /// </summary>
        /// <param name="SoftwareName"></param>
        /// <returns></returns>
        public String GetExePath(String SoftwareName)
        {
            String exe = null;
            using(StreamWriter sw = File.CreateText("logs.txt"))
            {
                //读取配置里的路径
                exe = ConfigurationManager.AppSettings[SoftwareName];
                sw.WriteLine($"从配置文件中读取【{SoftwareName}】的路径：" + exe);
                if(String.IsNullOrEmpty(exe))
                {
                    // 从注册表读取TeamViewer的安装目录
                    sw.WriteLine($"从注册表中读取【{SoftwareName}】路径");
                    if(RemoteToolHelper.GetSoftWare(SoftwareName, out exe))
                    {
                        // 写入配置
                        var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                        config.AppSettings.Settings.Remove(SoftwareName);
                        config.AppSettings.Settings.Add(SoftwareName, exe);
                        config.Save();
                    }
                    sw.WriteLine($"无法从注册表中读取【{SoftwareName}】");
                }
                sw.Flush();
            }
            return exe;
        }
    }
}

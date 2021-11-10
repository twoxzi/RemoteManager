using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Win32;

namespace Twoxzi.RemoteManager.Tools
{
    public class RemoteToolHelper
    {

        /// <summary>
        /// 软件是否安转
        /// 
        /// 
        /// </summary>
        /// <param name="SoftWareName"> 软件名称</param>
        /// <param name="SoftWarePath "> 安装路径</param>
        /// <returns> true or false </returns>
        /// <remarks>
        /// ————————————————
        /// 版权声明：本文为CSDN博主「BBJBBJ123」的原创文章，遵循CC 4.0 BY-SA版权协议，转载请附上原文出处链接及本声明。
        /// 原文链接：https://blog.csdn.net/baobingji/java/article/details/102609644
        /// </remarks>
        public static bool GetSoftWare(string SoftWareName, out string SoftWarePath)
        {
            SoftWarePath = null;
            List<RegistryKey> RegistryKeys = new List<RegistryKey>();
            RegistryKeys.Add(Registry.ClassesRoot);
            RegistryKeys.Add(Registry.CurrentConfig);
            RegistryKeys.Add(Registry.CurrentUser);
            RegistryKeys.Add(Registry.LocalMachine);
            RegistryKeys.Add(Registry.PerformanceData);
            RegistryKeys.Add(Registry.Users);
            Dictionary<string, string> Softwares = new Dictionary<string, string>();
            List<String> SubKeyNames = new List<string>();
            SubKeyNames.Add(@"Software\Microsoft\Windows\CurrentVersion\Uninstall");
            // 64位系统的32位程序
            SubKeyNames.Add(@"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall");

            foreach(var SubKeyName in SubKeyNames)
            {
                foreach(RegistryKey Registrykey in RegistryKeys)
                {
                    using(RegistryKey RegistryKey1 = Registrykey.OpenSubKey(SubKeyName, false))
                    {
                        if(RegistryKey1 == null) // 判断对象不存在
                            continue;
                        if(RegistryKey1.GetSubKeyNames() == null)
                            continue;
                        string[] KeyNames = RegistryKey1.GetSubKeyNames();
                        foreach(string KeyName in KeyNames)// 遍历子项名称的字符串数组
                        {
                            using(RegistryKey RegistryKey2 = RegistryKey1.OpenSubKey(KeyName, false)) // 遍历子项节点
                            {
                                if(RegistryKey2 == null)
                                    continue;
                                // 系统组件
                                if(RegistryKey2.GetValue("SystemComponent")?.ToString() == "1")
                                {
                                    continue;
                                }
                                string SoftwareName = RegistryKey2.GetValue("DisplayName", "").ToString(); // 获取软件名
                                string InstallLocation = RegistryKey2.GetValue("InstallLocation", "").ToString(); // 获取安装路径
                                if(!string.IsNullOrEmpty(InstallLocation)
                                 && !string.IsNullOrEmpty(SoftwareName))
                                {
                                    if(!Softwares.ContainsKey(SoftwareName))
                                        Softwares.Add(SoftwareName, InstallLocation);
                                }
                            }
                        }
                    }
                }
            }
            if(Softwares.Count <= 0)
                return false;
            foreach(string SoftwareName in Softwares.Keys)
            {
                if(SoftwareName.Contains(SoftWareName))
                {
                    SoftWarePath = Softwares[SoftwareName];
                    return true;
                }
            }
            return false;
        }

    }
}

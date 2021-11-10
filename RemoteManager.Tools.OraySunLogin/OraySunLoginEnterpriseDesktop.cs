using Microsoft.Win32;
using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Text;
using Twoxzi.RemoteManager.Entity;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Twoxzi.RemoteManager.Tools.Oray
{
    /// <summary>
    /// 向日葵桌面远程企业版
    /// </summary>
    [RemoteTool(ToolCode = OraySunLoginEnterpriseDesktop.ToolCode, Memo = "向日葵桌面远程企业版，不支持直接打开，只支持把远程方式输入到历史记录中进行快捷访问", ToolName = "向日葵桌面远程企业版")]
    public class OraySunLoginEnterpriseDesktop : RemoteToolBase
    {
        public const String ToolCode = "OSLE";
        /// <summary>
        /// 打开远程
        /// </summary>
        /// <param name="info"></param>
        public override void Open(RemoteInfo info)
        {
            Open(info, info.ToolCode == OraySunLoginEnterpriseDesktop.ToolCode);
        }
        /// <summary>
        /// 打开远程
        /// </summary>
        /// <param name="info"></param>
        /// <param name="isDesktop"></param>
        protected void Open(RemoteInfo info, Boolean isDesktop)
        {
            String exe = GetExePath("向日葵");

            if (String.IsNullOrEmpty(exe))
            {
                throw new Exception("工具 向日葵 未找到，请确保正确安装，启动已中止");
            }
            String id = info.ID ?? "";
            // 去除ID里的空格
            id = id.Replace(" ", "");

            // 添加一条历史记录
            {
                // 读取HKEY_USERS\.DEFAULT\Software\Oray\SunLogin\SunloginClient\SunloginEnterpriseInfo的fastcodehistroy_json，base64解码后以json数组解析
                var reg = Registry.Users.OpenSubKey(@".DEFAULT\Software\Oray\SunLogin\SunloginClient\SunloginEnterpriseInfo",true);
                String value;
                List<fastcodehistroy_json> list = new List<fastcodehistroy_json>();
                if (reg.GetValueNames().Any(x => x == "fastcodehistroy_json") && !String.IsNullOrEmpty(value = reg.GetValue("fastcodehistroy_json") as String))
                {
                    value = Encoding.UTF8.GetString(Convert.FromBase64String(value));
                    list = JsonConvert.DeserializeObject<List<fastcodehistroy_json>>(value);
                }
                // 移除相同的ID记录
                list.RemoveAll(x => x?.fastcode?.Replace(" ", "") == info?.ID?.Replace(" ",""));

                list.Add(new fastcodehistroy_json()
                {
                    fastcode = info.ID,
                    password = info.Password,
                    name= info.ID,
                });
                value = JsonConvert.SerializeObject(list);
                value = Convert.ToBase64String(Encoding.UTF8.GetBytes(value));
                reg.SetValue("fastcodehistroy_json", value);
            }

            exe = Path.Combine(exe, "SunloginClient.exe");
            var process = Process.Start(exe);
        }

        public class fastcodehistroy_json
        {
            public String fastcode { get; set; }
            public String image { get; set; }
            public String name { get; set; }
            public String password { get; set; }
        }
    }
}

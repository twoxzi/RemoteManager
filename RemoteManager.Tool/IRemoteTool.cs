using System;
using Twoxzi.RemoteManager.Entity;

namespace Twoxzi.RemoteManager.Tools
{
    public interface IRemoteTool
    {
        /// <summary>
        /// 打开
        /// </summary>
        /// <param name="info"></param>
        void Open(RemoteInfo info);
        /// <summary>
        /// 扩展属性编辑窗口
        /// </summary>
        /// <param name="info"></param>
        void ExtensionPropertySetter(RemoteInfo info);

    }
}

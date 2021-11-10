using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Text;

namespace Twoxzi.RemoteManager.Tools
{

    public interface IRemoteToolMetadata
    {
        String ToolCode { get; }
        String ToolName { get; }
        String Memo { get; }
    }
    /// <summary>
    /// 远程工具信息
    /// </summary>
    /// <remarks>加了AllowMultiple =true就不能导出了</remarks>
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class,Inherited =false)]
    public class RemoteToolAttribute : ExportAttribute,IRemoteToolMetadata
    {
        public RemoteToolAttribute():base(typeof(IRemoteTool))
        {

        }
        ///// <summary>
        ///// 初始化
        ///// </summary>
        ///// <param name="ToolCode">远程工具编码</param>
        //public RemoteToolAttribute(String ToolCode) : base(typeof(IRemoteTool))
        //{
        //    this.ToolCode = ToolCode;
        //}
        /// <summary>
        /// 远程工具编码
        /// </summary>
        public String ToolCode { get; set; }

        /// <summary>
        /// 远程工具名称
        /// </summary>
        public String ToolName { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public String Memo { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Twoxzi.RemoteManager.Entity
{
    /// <summary>
    /// 远程信息
    /// </summary>
    public class RemoteInfo : BindableBase
    {
        private string id;
        private string displayName;
        private string password;
        private string groupName;
        private string toolCode;
        private string memo;
        private DateTime lastTime;
        private string extensionProperty;

        /// <summary>
        /// 远程ID
        /// </summary>
        [Key]
        [Column(Order = 1)]
        public String ID { get => id; set { id = value; OnPropertyChanged(nameof(ID)); } }
        /// <summary>
        /// 远程工具编号
        /// </summary>
        [Key]
        [Column(Order = 2)]
        public String ToolCode { get => toolCode; set { toolCode = value; OnPropertyChanged(nameof(ToolCode)); } }
        /// <summary>
        /// 显示名称
        /// </summary>
        public String DisplayName { get => displayName; set { displayName = value; OnPropertyChanged(nameof(DisplayName)); } }
        /// <summary>
        /// 密码
        /// </summary>
        public String Password { get => password; set { password = value; OnPropertyChanged(nameof(Password)); } }

        public String GroupName { get => groupName; set { groupName = value; OnPropertyChanged(nameof(GroupName)); } }


        /// <summary>
        /// 备注
        /// </summary>
        public String Memo { get => memo; set { memo = value; OnPropertyChanged(nameof(Memo)); } }

        public DateTime LastTime { get => lastTime; set { lastTime = value; OnPropertyChanged(nameof(LastTime)); } }
        /// <summary>
        /// 扩展属性
        /// </summary>
        public String ExtensionProperty { get => extensionProperty; set { extensionProperty = value; OnPropertyChanged(nameof(ExtensionProperty)); } }



    }
}

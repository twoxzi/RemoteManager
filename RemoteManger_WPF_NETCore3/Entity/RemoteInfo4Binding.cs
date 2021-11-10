using System;
using System.Collections.Generic;
using System.Text;
using Twoxzi.RemoteManager.Tools;

namespace Twoxzi.RemoteManager.Entity
{
    public class RemoteInfo4Binding : RemoteInfo
    {
        private IRemoteToolMetadata remoteToolMetadata;
        private string toolName;

        public IRemoteToolMetadata RemoteToolMetadata
        {
            get => remoteToolMetadata;
            set
            {
                remoteToolMetadata = value;
                if(remoteToolMetadata != null && remoteToolMetadata.ToolCode != null)
                {
                    ToolCode = remoteToolMetadata.ToolCode;
                    ToolName = remoteToolMetadata.ToolName;
                }
                OnPropertyChanged(nameof(RemoteToolMetadata));
            }
        }

        public String ToolName { get => toolName; set { toolName = value; OnPropertyChanged(nameof(ToolName)); } }
    }
}

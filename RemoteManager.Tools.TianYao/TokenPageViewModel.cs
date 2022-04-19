using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Input;
using Twoxzi.RemoteManager.Entity;

namespace Twoxzi.RemoteManager.Tools.TianYao
{
    public class TokenPageViewModel : BindableBase
    {

        public ICommand OpenFileCommand { get; set; }

        public RemoteInfo RemoteInfo { get; set; }
        

        public TokenPageViewModel(RemoteInfo info)
        {
            OpenFileCommand = new RelayCommand(OpenFileCommand_Execute);
            RemoteInfo = info;
        }

        private void OpenFileCommand_Execute(object obj)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == true)
            {
                byte[] bs = File.ReadAllBytes(dialog.FileName);
                if(bs!=null)
                RemoteInfo.ExtensionProperty = Convert.ToBase64String(bs);
            }
        }
    }
}

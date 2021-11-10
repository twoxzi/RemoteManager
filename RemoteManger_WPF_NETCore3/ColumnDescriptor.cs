using System;
using System.Collections.Generic;
using System.Text;

namespace Twoxzi.RemoteManager
{
    public class ColumnDescriptor
    {
        public string Header { get; set; }
        public string PropertyName { get; set; }
        public bool IsAsc { get; set; } = true;
        public double Width { get; set; } = 90;
    }
}

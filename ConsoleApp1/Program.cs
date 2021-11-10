
#define  FindWindow

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Runtime.InteropServices;
using System.Text;
using Twoxzi.RemoteManager.Tools;

namespace ConsoleApp1
{
    [Export]
    class Program
    {
        //设置文本内容的消息

        private const int WM_SETTEXT = 0x000C;

        //鼠标点击消息
        const int BM_CLICK = 0x00F5;
        [DllImport("user32.dll")]


        private static extern IntPtr FindWindow(   string lpClassName,   string lpWindowName);

        [DllImport("User32.dll")]
        private static extern IntPtr FindWindowEx(   IntPtr hwndParent,   IntPtr hwndChildAfter,  string lpszClass, string lpszWindows);
        [DllImport("User32.dll")]
        private static extern Int32 SendMessage( IntPtr hWnd,  int Msg,   IntPtr wParam,  StringBuilder lParam);
        [DllImport("user32.dll ", CharSet = CharSet.Unicode)]
        public static extern IntPtr PostMessage(IntPtr hwnd, int wMsg, IntPtr wParam, IntPtr lParam);

        static void Main(string[] args)
        {

            #if FindWindow
            IntPtr hWnd = FindWindow(null, "向日葵远程控制");
            if (!hWnd.Equals(IntPtr.Zero))
            {
                //返回写字板编辑窗口句柄
                IntPtr edithWnd = FindWindowEx(hWnd, IntPtr.Zero, null, "远程协助");
                if (!edithWnd.Equals(IntPtr.Zero))
                    // 发送WM_SETTEXT 消息： "Hello World!"
                    SendMessage(edithWnd, WM_SETTEXT, IntPtr.Zero, new StringBuilder("Hello World!"));
            }
            return;
            #endif


            Console.WriteLine(AppDomain.CurrentDomain.BaseDirectory);
            DirectoryCatalog catalog = new DirectoryCatalog(AppDomain.CurrentDomain.BaseDirectory);
            CompositionContainer container = new CompositionContainer(catalog);
            

            Program program = new Program();
            container.ComposeParts(program);
            //foreach(var item in container.GetExports<IRemoteTool,IRemoteToolMetadata>())
            foreach(var item in program.List)
            {
                Console.WriteLine($"{item.Metadata.ToolCode}_{item.Metadata.ToolName}");
            }
            Console.WriteLine("End");
            Console.ReadKey();
        }

        [ImportMany(typeof(IRemoteTool))]
        //[ImportMany]
        public List<Lazy<IRemoteTool, IRemoteToolMetadata>> List { get; set; }
    }
}

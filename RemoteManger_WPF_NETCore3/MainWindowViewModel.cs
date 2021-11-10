using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;
using Microsoft.EntityFrameworkCore;
using Twoxzi.RemoteManager.Entity;
using Twoxzi.RemoteManager.Tools;

namespace Twoxzi.RemoteManager
{
    /// <summary>
    /// ViewModel
    /// </summary>
    public class MainWindowViewModel : BindableBase
    {
        private CollectionView collectionView;
        private Boolean? isGroup;
        private String searchText = "";

        private ICommand deleteCommand;
        private ICommand saveCommand;
        private ICommand linkCommand;
        private ICommand ascColumnCommand;
        private ICommand addCommand;
        private ICommand outputCommand;
        private ICommand descColumnCommand;
        private RelayCommand<RemoteInfo4Binding> extensionSettingsCommand;

        [ImportMany(typeof(IRemoteTool))]
        public List<Lazy<IRemoteTool, IRemoteToolMetadata>> RemoteToolList { get; set; }

        public List<IRemoteToolMetadata> RemoteToolMetadataList { get; set; }

        public Boolean? IsGroup
        {
            get { return isGroup; }
            set
            {
                isGroup = value;
                Configuration con = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                if (con.AppSettings.Settings["isGroup"] == null)
                {
                    con.AppSettings.Settings.Add("isGroup", isGroup == true ? "1" : "0");
                }
                else
                {
                    con.AppSettings.Settings["isGroup"].Value = isGroup == true ? "1" : "0";
                }
                con.Save();
                GroupSwitchChanged(isGroup);
                //ConfigurationManager.AppSettings.Set("isGroup", isGroup ? "1" : "0");

            }
        }

        public string SearchText
        {
            get => searchText;
            set
            {
                searchText = value?.Trim() ?? "";
                Dispatcher.CurrentDispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background, new Action(() => { collectionView.Refresh(); }));
                OnPropertyChanged(nameof(SearchText));
            }
        }
        public ObservableCollection<RemoteInfo4Binding> Collection { get; set; } = new ObservableCollection<RemoteInfo4Binding>();

        /// <summary>
        /// 列描述集合
        /// </summary>
        public ObservableCollection<ColumnDescriptor> Columns { get; set; }
        public List<ColumnDescriptor> ColumnDescriptors { get; set; }

        public MainWindowViewModel()
        {
            DirectoryCatalog catalog = new DirectoryCatalog(AppDomain.CurrentDomain.BaseDirectory);
            CompositionContainer container = new CompositionContainer(catalog);
            container.ComposeParts(this);

            RemoteToolMetadataList = RemoteToolList.Select(x => x.Metadata).ToList();

            Columns = new ObservableCollection<ColumnDescriptor>()
            {
                new ColumnDescriptor(){ Header="ID",PropertyName="ID" },
                new ColumnDescriptor(){ Header="名称",PropertyName="DisplayName" },
                new ColumnDescriptor(){ Header="访问时间",PropertyName="LastTime",Width=0},
                new ColumnDescriptor(){ Header="工具名称",PropertyName="ToolName"},
                new ColumnDescriptor(){Header="备注",PropertyName="Memo"}
            };

            var v = Dispatcher.CurrentDispatcher.BeginInvoke(new Action(loadRemoteInfo));
            v.Completed += LoadFiles_Completed;

            isGroup = ConfigurationManager.AppSettings["isGroup"] == "1";
            collectionView = (CollectionView)CollectionViewSource.GetDefaultView(Collection);

            collectionView.Filter = x =>
            {
                RemoteInfo4Binding info = x as RemoteInfo4Binding;

                if (info == null)
                {
                    return true;
                }
                String str = SearchText.Trim();
                if (String.IsNullOrEmpty(str))
                {
                    return true;
                }
                String[] array = str.Split(' ');

                foreach (var item in array)
                {
                    if (String.IsNullOrEmpty(item))
                    {
                        continue;
                    }
                    if ((info.ID?.Contains(item) == true) || (info.DisplayName?.Contains(item) == true) || (info.Memo?.Contains(item) == true) || (info.GroupName?.Contains(item) == true) || (info.RemoteToolMetadata?.ToolName?.Contains(item) == true))
                    {

                    }
                    else
                    {
                        return false;
                    }
                }
                return true;
            };
        }

        private void LoadFiles_Completed(Object sender, EventArgs e)
        {
            // 加载完成后排序
            RefreshOrder("访问时间", false);
        }

        /// <summary>
        /// 加载远程信息，清空列表，再按访问时间倒序添加到列表中
        /// </summary>
        private void loadRemoteInfo()
        {
            Collection.Clear();
            using (MyDbContext dbo = MyDbContext.CreateDb())
            {
                var query = from a in dbo.RemoteInfo.AsNoTracking().AsEnumerable()
                            join b in RemoteToolMetadataList on a.ToolCode equals b.ToolCode into tt
                            from b in tt.DefaultIfEmpty()
                            orderby a.LastTime descending
                            select new RemoteInfo4Binding()
                            {
                                ID = a.ID,
                                ToolCode = a.ToolCode,
                                DisplayName = a.DisplayName,
                                ExtensionProperty = a.ExtensionProperty,
                                GroupName = a.GroupName,
                                LastTime = a.LastTime,
                                Memo = a.Memo,
                                Password = a.Password,
                                RemoteToolMetadata = b
                            };

                foreach (var item in query)
                {
                    Collection.Add(item);
                }
            }
        }

        //private ICommand _addColumnCommand;
        //public ICommand AddColumnCommand
        //{
        //    get
        //    {
        //        if(_addColumnCommand == null)
        //        {
        //            _addColumnCommand = new RelayCommand<string>(
        //                s =>
        //                {
        //                    this.Columns.Add(new ColumnDescriptor { Header = s });
        //                });
        //        }
        //        return _addColumnCommand;
        //    }
        //}

        //private ICommand _removeColumnCommand;
        //public ICommand RemoveColumnCommand
        //{
        //    get
        //    {
        //        if(_removeColumnCommand == null)
        //        {
        //            _removeColumnCommand = new RelayCommand<string>(
        //                s =>
        //                {
        //                    this.Columns.Remove(this.Columns.FirstOrDefault(d => d.Header == s));
        //                });
        //        }
        //        return _removeColumnCommand;
        //    }
        //}


        public ICommand AddCommand
        {
            get
            {
                if (addCommand == null)
                {
                    addCommand = new RelayCommand(AddCommandExecute);
                }
                return addCommand;
            }
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="sender"></param>
        private void AddCommandExecute(object sender)
        {
            try
            {
                var listView = sender as ListView;
                var source = listView.ItemsSource;
                RemoteInfo4Binding info = new RemoteInfo4Binding() { LastTime = DateTime.Now, Memo = "" };

                // 如果由数字和空格组成,则是ID,否则是Name
                if (SearchText.All(x => x == ' ' || (x >= 48 && x <= 57)))
                {
                    info.ID = SearchText.Trim();
                }
                else
                {
                    info.DisplayName = SearchText.Trim();
                }
                Collection.Add(info);
                listView.SelectedItem = info;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private Boolean AddCommandCanExecuted(object sender)
        {
            return !String.IsNullOrEmpty(SearchText);
        }

        /// <summary>
        /// 选中的项不为空
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private Boolean SelectedItemIsNotNull(Object obj)
        {
            return (obj as ListView)?.SelectedItem != null;
        }

        /// <summary>
        /// 选中的项不为空
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private Boolean SelectedItemIsNotNull(ListView obj)
        {
            return obj?.SelectedItem != null;
        }

        /// <summary>
        /// 保存
        /// </summary>
        public ICommand SaveCommand
        {
            get
            {
                if (saveCommand == null)
                {
                    saveCommand = new RelayCommand(obj =>
                                    {
                                        var listView = obj as ListView;
                                        if (listView == null)
                                        {
                                            return;
                                        }
                                        var info = listView.SelectedItem as RemoteInfo4Binding;

                                        if (String.IsNullOrWhiteSpace(info.ID) || String.IsNullOrWhiteSpace(info.ToolCode))
                                        {
                                            MessageBox.Show("必须输入ID和选择远程工具");
                                        }

                                        // 保存
                                        using (MyDbContext dbo = MyDbContext.CreateDb())
                                        {
                                            var list = RemoteToolMetadataList;
                                            if (dbo.RemoteInfo.AsNoTracking().Any(x => x.ID == info.ID && x.ToolCode==info.ToolCode))
                                            {
                                                dbo.RemoteInfo.Attach(info);
                                                dbo.Entry(info).State = EntityState.Modified;
                                            }
                                            else
                                            {
                                                dbo.RemoteInfo.Add(info);
                                            }

                                            dbo.SaveChanges();
                                        }
                                        loadRemoteInfo();
                                    }, SelectedItemIsNotNull);
                }
                return saveCommand;
            }
        }
        /// <summary>
        /// 删除
        /// </summary>
        public ICommand DeleteCommand
        {
            get
            {
                if (deleteCommand == null)
                {
                    deleteCommand = new RelayCommand(obj =>
                      {
                          var listView = obj as ListView;
                          RemoteInfo4Binding info = listView.SelectedItem as RemoteInfo4Binding;
                          if (info == null)
                          {
                              return;
                          }
                          try
                          {
                              // 如果是新增的，则删除会出错
                              using (MyDbContext dbo = MyDbContext.CreateDb())
                              {

                                  var entity = dbo.RemoteInfo.Where(x => x.ID == info.ID && x.ToolCode == info.ToolCode).FirstOrDefault();
                                  if (entity != null)
                                  {
                                      dbo.RemoteInfo.Remove(entity);
                                      dbo.SaveChanges();
                                  }
                              }
                          }
                          catch (Exception ex)
                          {
                              MessageBox.Show(ex.Message);
                          }
                          var index = listView.SelectedIndex;
                          // 重新设置列表的选中项，如果选中项为最后一项，则index-1
                          listView.SelectedIndex = Collection.Count == index ? index - 1 : index;
                          Collection.Remove(info);

                      }, SelectedItemIsNotNull);
                }
                return deleteCommand;
            }
        }

        /// <summary>
        /// 桌面连接
        /// </summary>
        public ICommand LinkCommand
        {
            get
            {
                if (linkCommand == null)
                {
                    linkCommand = new RelayCommand<ListView>(listView =>
                    {
                        LinkExecuteBase(listView);
                    }
                    , SelectedItemIsNotNull
                    );
                }
                return linkCommand;
            }
        }

        /// <summary>
        /// 打开
        /// </summary>
        /// <param name="listView"></param>
        private void LinkExecuteBase(ListView listView)
        {
            try
            {
                if (listView == null)
                {
                    return;
                }
                RemoteInfo4Binding info = listView.SelectedItem as RemoteInfo4Binding;
                if (info == null)
                {
                    //MessageBox.Show("未选中连接对象，请先在列表中选择目标。"); 
                    return;
                }

                IRemoteTool tool = RemoteToolList.Where(x => x.Metadata.ToolCode == info.ToolCode).FirstOrDefault()?.Value;
                if (tool == null)
                {
                    MessageBox.Show($"没有找到编码为{info.ToolCode}的工具,请检查插件是否正确加载");
                    return;
                }
                if (info.Password != null && info.Password.Length > 0)
                {
                    Clipboard.SetText(info.Password);
                }

                tool.Open(info);

                using (MyDbContext dbo = MyDbContext.CreateDb())
                {
                    info.LastTime = DateTime.Now;
                    dbo.RemoteInfo.Attach(info);
                    dbo.Entry(info).State = EntityState.Modified;
                    dbo.SaveChanges();
                }
                var window = Window.GetWindow(listView);
                if (window != null)
                {
                    window.WindowState = WindowState.Minimized;
                }
                RefreshOrder("访问时间", false);
                Collection.Move(listView.SelectedIndex, 0);
                listView.SelectedItem = info;
            }
            catch (Exception ex)
            {
#if DEBUG
                MessageBox.Show(ex.Message + ex.StackTrace, "错误");
#else
                MessageBox.Show(ex.Message,"错误");
#endif
            }
        }

        /// <summary>
        /// 排序
        /// </summary>
        /// <param name="header"></param>
        /// <param name="isAsc"></param>
        private void RefreshOrder(String header, Boolean isAsc)
        {
            ColumnDescriptor cd = Columns.FirstOrDefault(x => x.Header == header);
            if (cd != null)
            {
                CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(Collection);
                view.SortDescriptions.Clear();
                view.SortDescriptions.Add(new System.ComponentModel.SortDescription(cd.PropertyName, isAsc ? System.ComponentModel.ListSortDirection.Ascending : System.ComponentModel.ListSortDirection.Descending));
            }
        }
        /// <summary>
        /// 顺序
        /// </summary>
        public ICommand AscColumnCommand
        {
            get
            {
                if (ascColumnCommand == null)
                {
                    ascColumnCommand = new RelayCommand(x => SortByColumnExecute(x, true));
                }
                return ascColumnCommand;
            }
        }
        /// <summary>
        /// 倒序
        /// </summary>
        public ICommand DescColumnCommand
        {
            get
            {
                if (descColumnCommand == null)
                {
                    descColumnCommand = new RelayCommand(x => SortByColumnExecute(x, false));
                }
                return descColumnCommand;
            }
        }

        //单击表头排序
        private void SortByColumnExecute(object sender, Boolean isAsc)
        {
            if (sender is GridViewColumnHeader)
            {
                //获得点击的列  
                GridViewColumn clickedColumn = (sender as GridViewColumnHeader).Column;
                if (clickedColumn != null)
                {
                    RefreshOrder(clickedColumn.Header.ToString(), isAsc);
                }
            }
        }
        /// <summary>
        /// 分组开关
        /// </summary>
        /// <param name="IsChecked"></param>
        private void GroupSwitchChanged(Boolean? IsChecked)
        {
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(Collection);
            if (IsChecked == true)
            {
                PropertyGroupDescription groupDescription = new PropertyGroupDescription("GroupName");
                view.GroupDescriptions.Add(groupDescription);
            }
            else
            {
                view.GroupDescriptions.Clear();
            }
        }
        /// <summary>
        /// 导出
        /// </summary>
        public ICommand OutputCommand
        {
            get
            {
                if (outputCommand == null)
                {
                    outputCommand = new RelayCommand<ListView>(OutputExecute);
                }
                return outputCommand;
            }
        }

        private void OutputExecute(ListView listView)
        {
            //String subDirName = "output";
            //DirectoryInfo di = new DirectoryInfo(Path.Combine(RemoteInfo.Folder, subDirName));
            //if(di.Exists)
            //{
            //    di.Delete(true);
            //}
            //di.Create();
            //if(listView.SelectedItems.Count == 0)
            //{

            //    return;
            //}
            //var list = listView.SelectedItems.OfType<RemoteInfo>();
            //foreach(var item in list)
            //{
            //    FileInfo fi = new FileInfo(item.FilePath);
            //    fi.CopyTo(Path.Combine(di.FullName, fi.Name), true);
            //}
            //try
            //{
            //    Clipboard.SetText(di.FullName);
            //}
            //catch { }
            //MessageBox.Show($"已导出到目录{di.FullName},已将目录路径复制到粘贴板");
        }

        public ICommand ExtensionSettingsCommand
        {
            get
            {
                if (extensionSettingsCommand == null)
                {
                    extensionSettingsCommand = new RelayCommand<RemoteInfo4Binding>(extensionSettings,
                        x => true //SelectedItemIsNotNull(x) && x.RemoteToolMetadata != null
                        ) ;
                }
                return extensionSettingsCommand;
            }
        }

        private void extensionSettings(RemoteInfo4Binding obj)
        {
            var tool = RemoteToolList.Where(x => x.Metadata.ToolCode == obj.ToolCode).FirstOrDefault();
            if (tool == null)
            {
                return;
            }
            tool.Value.ExtensionPropertySetter(obj);
        }
    }
}

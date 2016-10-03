using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;

namespace Wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public ObservableCollection<ExportInfo> ExInfo { get; set; } = new ObservableCollection<ExportInfo>();
        readonly MainWndControl MainWndControl = new MainWndControl();
        private void MainWindow_OnInitialized(object sender, EventArgs e)
        {
            DataGrid.ItemsSource = ExInfo;
            StackPanel.DataContext = MainWndControl;
            for (int i = 0; i < 20; i++)
            {
                ComboBox.Items.Add(i + 1);
            }

            
        }

        private void BtnExport_OnClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "txt|*.txt";
            ofd.ShowDialog();
            if (!string.IsNullOrEmpty(ofd.FileName))
            {
                string txt = File.ReadAllText(ofd.FileName, Encoding.Default);
                MatchCollection mc = Regex.Matches(txt, "(.*?)----(.*?)----(.*?)----(.*?)----(.*?)----(.*?)----(.*?)----(.*?)----(.*)");

                foreach (Match m in mc)
                {
                    ExportInfo ei = new ExportInfo()
                    {
                        Id = ExInfo.Count+1,
                        GoodsId = m.Groups[1].Value,
                        Attribution = m.Groups[2].Value,
                        Name = m.Groups[3].Value,
                        IdCard = m.Groups[4].Value,
                        Mobile = m.Groups[5].Value,
                        City = m.Groups[6].Value,
                        District = m.Groups[7].Value,
                        BussinessHall = m.Groups[8].Value,
                        Price = m.Groups[9].Value,
                    };
                    ExInfo.Add(ei);
                }
            }
            //乌鲁木齐----张三----130300199001010001----13933330000----乌鲁木齐市----天山区----乌鲁木齐大巴扎营业厅----价格
        }

        private void BtnClear_OnClick(object sender, RoutedEventArgs e)
        {
            ExInfo.Clear();
        }

        private void BtnBegin_OnClick(object sender, RoutedEventArgs e)
        {
            MainWndControl.Begin = true;
            
            Task.Factory.StartNew(() =>
            {
                Parallel.ForEach(ExInfo, (info) =>
                {
                    XinJiang3gCardHttp http = new XinJiang3gCardHttp();
                    while (true)
                    {
                        if (MainWndControl.Stop) break;
                        info.Status = "购买中...";
                        bool res = http.Order(info);
                        if (res)
                        {
                            info.Status = "成功";
                            
                            break;
                        }
                        else
                        {
                            info.Status = "价格不符";
                            Thread.Sleep(MainWndControl.Sleep * 1000);
                        }
                    }
                });

                MainWndControl.Begin = false;
            });
        }

        
        private void BtnStop_OnClick(object sender, RoutedEventArgs e)
        {
            MainWndControl.Begin = false;
        }
    }

    public class MainWndControl:INotifyPropertyChanged
    {
        private bool _begin;

        public bool Begin
        {
            get { return _begin; }
            set
            {
                _begin = value;
                Stop = !value;
                OnPropertyChanged(nameof(Begin));
                OnPropertyChanged(nameof(Stop));
            }
        }

        public bool Stop { get; private set; } = true;
        public int Sleep { get; set; } = 5;
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

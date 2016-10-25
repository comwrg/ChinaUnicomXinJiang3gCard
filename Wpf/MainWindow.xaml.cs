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
using Wpf.XinJiang3gCard;

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

        private ObservableCollection<ExportInfo> ExInfo { get; set; } = new ObservableCollection<ExportInfo>();
        readonly MainWndControl _mainWndControl = new MainWndControl();
        private PriceList _priceList = new PriceList();

        private void MainWindow_OnInitialized(object sender, EventArgs e)
        {
            DataGrid.ItemsSource = ExInfo;
            StackPanel.DataContext = _mainWndControl;
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
                MatchCollection mc = Regex.Matches(txt,
                    "(.*?)----(.*?)----(.*?)----(.*?)----(.*?)----(.*?)----(.*?)----(.*?)----(.*)");

                foreach (Match m in mc)
                {
                    ExportInfo ei = new ExportInfo()
                    {
                        Id = ExInfo.Count + 1,
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
                    _priceList.Add(ei.GoodsId);
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
            _mainWndControl.Begin = true;
            Func<bool> IsFinish = () =>
            {
                foreach (var ei in ExInfo)
                {
                    if (ei.StatusCode != ExportInfo.StatusCodes.FinishBuy)
                        return false;
                }
                return false;
            };

            Task.Factory.StartNew(() =>
            {
                while (!IsFinish())
                {
                    if (_mainWndControl.Stop) break;
                    _priceList.Refresh();
                    Parallel.ForEach(ExInfo, new ParallelOptions { MaxDegreeOfParallelism = _mainWndControl.MaxDegreeOfParallelism },(info) =>
                    {
                        if (info.StatusCode == ExportInfo.StatusCodes.FinishBuy) return;

                        if (_mainWndControl.Stop)
                        {
                            info.StatusCode = ExportInfo.StatusCodes.Stop;
                            return;
                        }
                        
                        info.StatusCode = ExportInfo.StatusCodes.IsBuying;
                        bool res = false;
                        if (_priceList[info.GoodsId] <= decimal.Parse(info.Price))
                        {
                            res = info.Http.Order(info);
                        }
                        else
                        {
                            Thread.Sleep(1000);
                        }
                        
                        info.StatusCode = res ? ExportInfo.StatusCodes.FinishBuy : ExportInfo.StatusCodes.NotPrice;

                        if (_mainWndControl.Stop)
                        {
                            info.StatusCode = ExportInfo.StatusCodes.Stop;
                        }

                    });


                }
            });
        }

        
        private void BtnStop_OnClick(object sender, RoutedEventArgs e)
        {
            _mainWndControl.Begin = false;
            return;
            Thread.Sleep(1000);
            foreach (var ei in ExInfo)
            {
                ei.StatusCode = ExportInfo.StatusCodes.Stop;
            }
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
        public int MaxDegreeOfParallelism { get; set; } = 10;
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

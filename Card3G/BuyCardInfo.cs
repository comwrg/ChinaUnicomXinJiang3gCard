// ----------------------------------------------------------------
// Author      : Wrg
// Email       : comwrg@qq.com
// Date        : 2016/10/03 13:27
// Description : 
// ----------------------------------------------------------------

using System.ComponentModel;
using System.Net.Cache;
using System.Runtime.CompilerServices;

namespace Wpf
{
    public sealed class BuyCardInfo : INotifyPropertyChanged
    {
        public enum StatusCodes
        {
            None,
            IsBuying,
            NotPrice,
            FinishBuy,
            Stop,
        }

        public int StatusSleep
        {
            get { return _statusSleep; }
            set
            {
                _statusSleep = value;
                OnPropertyChanged(nameof(Status));
            }
        }
        private int    _id;
        private string _name;
        private string _idCard;
        private string _city;
        private string _mobile;
        private string _district;
        private string _bussinessHall;
        private string _attribution;
        private string _price;
        private StatusCodes _statusCode;
        private string _goodsId;
        private int _statusSleep;

        public int Id
        {
            get { return _id; }
            set
            {
                _id = value;
                OnPropertyChanged();
            }
        }
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }
        public string IdCard
        {
            get { return _idCard; }
            set
            {
                _idCard = value;
                OnPropertyChanged();
            }
        }

        public string City
        {
            get
            {
                return _city;
            }

            set
            {
                _city = value;
            }
        }

        public string Mobile
        {
            get
            {
                return _mobile;
            }

            set
            {
                _mobile = value;
            }
        }

        public string District
        {
            get
            {
                return _district;
            }

            set
            {
                _district = value;
            }
        }

        public string BussinessHall
        {
            get
            {
                return _bussinessHall;
            }

            set
            {
                _bussinessHall = value;
            }
        }

        public string Attribution
        {
            get
            {
                return _attribution;
            }

            set
            {
                _attribution = value;
            }
        }

        public string Price
        {
            get
            {
                return _price;
            }

            set
            {
                _price = value;
            }
        }

        public string Status
        {
            get
            {
                if (StatusSleep != 0) return StatusSleep.ToString();
                switch (StatusCode)
                {
                    case StatusCodes.None:
                        return "等待开始...";
                    case StatusCodes.IsBuying:
                        return "正在购买中...";
                    case StatusCodes.NotPrice:
                        return "价格不符.";
                    case StatusCodes.FinishBuy:
                        return "购买成功.";
                    case StatusCodes.Stop:
                        return "停止中.";
                    default:
                        return "ERROR";
                }
            }
        }

        public StatusCodes StatusCode
        {
            get
            {
                return _statusCode;
            }

            set
            {
                _statusCode = value;
                OnPropertyChanged(nameof(Status));
            }
        }
        public string GoodsId
        {
            get
            {
                return _goodsId;
            }

            set
            {
                _goodsId = value;
            }
        }
        public XinJiang3GCardHttp Http { get; set; } = new XinJiang3GCardHttp();


        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}


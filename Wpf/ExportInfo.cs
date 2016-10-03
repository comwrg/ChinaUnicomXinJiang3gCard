// ----------------------------------------------------------------
// Author      : Wrg
// Email       : comwrg@qq.com
// Date        : 2016/10/03 13:27
// Description : 
// ----------------------------------------------------------------

using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Wpf
{
    public class ExportInfo : INotifyPropertyChanged
    {
        private int    _id;
        private string _name;
        private string _idCard;
        private string _city;
        private string _mobile;
        private string _district;
        private string _bussinessHall;
        private string _attribution;
        private string _price;
        private string _status;
        private string _goodsId;
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
                return _status;
            }

            set
            {
                _status = value;
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

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace Lab2
{
    namespace NorthWnd
    {
        public class ViewModel : INotifyPropertyChanged
        {
            public Order SelectedOrder
            {
                get => selectedOrder;
                set
                {
                    if (selectedOrder != value)
                    {
                        selectedOrder = value;
                        NotifyPropertyChanged("SelectedOrder");
                    }
                }
            }

            private Order selectedOrder;
            public event PropertyChangedEventHandler PropertyChanged;
            protected void NotifyPropertyChanged(string propertyName)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}

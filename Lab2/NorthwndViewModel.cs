using System.Collections.ObjectModel;
using System.Data.Entity;

namespace Lab2
{
namespace NorthWnd
{
    public class NorthwndViewModel : ViewModel
    {
        public NorthwndViewModel()
        {
            Entities = new NORTHWNDEntities();
        }

        public NORTHWNDEntities Entities { get; private set; }
        bool isOrdersLoaded = false;
        public ObservableCollection<Order> Orders
        {
            get
            {
                if (!isOrdersLoaded)
                {
                    Entities.Orders.Load();
                    isOrdersLoaded = true;
                }
                return Entities.Orders.Local;
            }
        }
    }
}
}


using PRBD_Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
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

namespace prbd_1819_g07
{
    /// <summary>
    /// Logique d'interaction pour BasketView.xaml
    /// </summary>
    public partial class BasketView : UserControlBase
    {
        private ObservableCollection<User> users;
        public ObservableCollection<User> Users
        {
            get { return users; }
            set
            {
                users = value;
                RaisePropertyChanged(nameof(Users));
                // RaisePropertyChanged(nameof(BasketListView));
            }
        }

        public ICommand ConfirmBasket { get; set; }
        public ICommand ClearBasket { get; set; }


        public BasketView()
        {
            InitializeComponent();
        }
    }
}

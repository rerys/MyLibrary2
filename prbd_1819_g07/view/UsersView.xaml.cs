using PRBD_Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
    /// Logique d'interaction pour UsersView.xaml
    /// </summary>
    public partial class UsersView : UserControlBase
    {
        public ICommand NewUser { get; set; }

        public UsersView()
        {
            InitializeComponent();

            if (DesignerProperties.GetIsInDesignMode(this))
                return;

            DataContext = this;
            Users = new ObservableCollection<User>(App.Model.Users);
        }

        private ObservableCollection<User> users;
        public ObservableCollection<User> Users
        {

            get => users;

            set => SetProperty<ObservableCollection<User>>(ref users, value, () =>
            {
            });

        }


        private void ListViewMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index = ListView.SelectedIndex;
            switch (index)
            {
                case 0:


                    break;
            }
        }
    }
}

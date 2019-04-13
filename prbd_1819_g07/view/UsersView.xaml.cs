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
        public ICommand ClearFilter { get; set; }

        public UsersView()
        {
            InitializeComponent();

            if (DesignerProperties.GetIsInDesignMode(this))
                return;

            DataContext = this;
            Users = new ObservableCollection<User>(App.Model.Users);
            ClearFilter = new RelayCommand(() => Filter = "");
        }

        private ObservableCollection<User> users;
        public ObservableCollection<User> Users
        {

            get => users;

            set => SetProperty<ObservableCollection<User>>(ref users, value, () =>
            {
            });

        }

        private string filter;

        public string Filter
        {
            get => filter;

            set => SetProperty<string>(ref filter, value, ApplyFilterAction);
        }

        private void ApplyFilterAction()
        {
            var model = Model.CreateModel(DbType.MsSQL);

            var query = from u in model.Users
                        let text = u.UserName.Contains(Filter) || u.FullName.Contains(Filter) || u.Email.Contains(Filter)
                       // ||u.Birthdate.Contains(Filter) || u.Role.Contains(Filter) 
                        where text
                        select u;

            Users = new ObservableCollection<User>(query);
        }


        //A modifier
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

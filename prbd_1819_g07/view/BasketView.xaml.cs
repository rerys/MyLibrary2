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
    /// Logique d'interaction pour BasketView.xaml
    /// </summary>
    public partial class BasketView : UserControlBase
    {
        private ObservableCollection<User> users;
        public ObservableCollection<User> Users
        {
            get => users;

            set => SetProperty<ObservableCollection<User>>(ref users, value, () =>
            {
            });

        }

        User selectedUser;
        public User SelectedUser
        {
            get => selectedUser;
            set => SetProperty<User>(ref selectedUser, value);
        }

        public override bool Validate()
        {
            ClearErrors();
            if (SelectedUser != null)
            {
                SelectedUser.Validate();
                this.errors.SetErrors(selectedUser.GetErrors());
            }
            NotifyAllFields();
            return HasErrors;
        }

        private void NotifyAllFields()
        {
            RaisePropertyChanged(nameof(Basket));
        }

        public ICommand ConfirmBasket { get; set; }
        public ICommand ClearBasket { get; set; }


        public BasketView()
        {
            InitializeComponent();

            DataContext = this;

            var model = Model.CreateModel(DbType.MsSQL);
            Users = new ObservableCollection<User>(model.Users);
            SelectedUser = App.Model.Users.Where(u => u.UserName.Contains(App.CurrentUser.UserName)).SingleOrDefault();
        }

        public ObservableCollection<Book> Basket
        {
            get
            {
                var query = from b in SelectedUser.Basket.Items select b.BookCopy.Book;
                return new ObservableCollection<Book>(query);
            }
        }

    }
}

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
            get { return users; }
            set
            {
                users = value;
                RaisePropertyChanged(nameof(Users));
                // RaisePropertyChanged(nameof(BasketListView));
            }
        }

        private CollectionView basketView = null;
        public CollectionView BasketOfUserView
        {
            get
            {
                basketView = (CollectionView)CollectionViewSource.GetDefaultView(users);
                if (basketView != null && basketView.SortDescriptions.Count == 0)
                    basketView.SortDescriptions.Add(new SortDescription("Title", ListSortDirection.Descending));
                return basketView;
            }
        }

        public Rental Title
        {
            get { return App.CurrentUser.Basket; }
            set
            {
                RaisePropertyChanged(nameof(Title));
                Validate();
            }
        }

        //public string Author
        //{
        //    get { return App.CurrentUser.Basket; }
        //    set
        //    {
        //        RaisePropertyChanged(nameof(Title));
        //        Validate();
        //    }
        //}
        User selectedUser;
        public User SelectedUser
        {
            get { return selectedUser; }
            set
            {
                selectedUser = value;
                RaisePropertyChanged(nameof(SelectedUser));
                NotifyAllFields();
                Console.WriteLine(selectedUser);
            }
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
            RaisePropertyChanged(nameof(Title));
           // RaisePropertyChanged(nameof(Author));
        }

        public ICommand ConfirmBasket { get; set; }
        public ICommand ClearBasket { get; set; }


        public BasketView()
        {
            InitializeComponent();
        }
    }
}

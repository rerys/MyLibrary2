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

        /*********************************************************************************************************************************
         *
         *   PROPERTIES
         *
         *********************************************************************************************************************************/

        //Propriété de la liste des utilisateurs pour le combobox
        private ObservableCollection<User> users;
        public ObservableCollection<User> Users
        {
            get => users;

            set => SetProperty<ObservableCollection<User>>(ref users, value, () =>
            {
            });

        }

        //Propriété de la liste de rentalItemn qui sont dans le panier de l'user selectionné. 
        public ObservableCollection<RentalItem> Basket
        {
            get
            {
                if (SelectedUser.Basket != null)
                {
                    var query = from b in SelectedUser.Basket.Items select b;
                    return new ObservableCollection<RentalItem>(query);
                }
                return null;
            }
        }

        //Propriété de l'utilisateur sélectionnée dans le combobox. Par défaut, c'est l'user connecté. 
        User selectedUser;
        public User SelectedUser
        {
            get { return selectedUser; }
            set
            {
                selectedUser = value;
                App.SelectedUser = selectedUser;
                RaisePropertyChanged(nameof(SelectedUser));
                RaisePropertyChanged(nameof(Basket));
            }
        }

        //Méthode de validation pour la selection de l'user dans le combobox
        //Lorsqu'on selectionne une user, refresh la liste du panier. 
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

        //Méthode pour refresh tous les champs lorsqu'il y a changement user selectionée. 
        private void NotifyAllFields()
        {
            RaisePropertyChanged(nameof(Basket));
        }

        //Renvoie true si l'user connecté est admin.
        public bool IsAdmin
        {
            get { return App.CurrentUser.Role == Role.Admin; }
        }

        public bool NotEmptyBasket
        {
            get { return SelectedUser.Basket != null; }
        }
        /*********************************************************************************************************************************
         *
         *   ICOMMAND
         *
         *********************************************************************************************************************************/

        //Commande pour confirmer le panier
        public ICommand ConfirmBasket { get; set; }

        //Commande pour clear le panier.
        public ICommand ClearBasket { get; set; }

        public ICommand DeleteFromBasket { get; set; }

        /*********************************************************************************************************************************
         *
         *   VIEW CONSTRUCTOR
         *
         *********************************************************************************************************************************/

        public BasketView()
        {
            InitializeComponent();

            DataContext = this;

            //var model = Model.CreateModel(DbType.MsSQL);
            Users = new ObservableCollection<User>(App.Model.Users);
            SelectedUser = App.Model.Users.Where(u => u.UserName.Contains(App.CurrentUser.UserName)).SingleOrDefault();
            ConfirmBasket = new RelayCommand(() =>
            {
                SelectedUser.ConfirmBasket();
                App.Model.SaveChanges();
                NotifyAllFields();
            });

            ClearBasket = new RelayCommand(() =>
            {

                SelectedUser.ClearBasket();
                App.Model.SaveChanges();
                NotifyAllFields();
            });

            DeleteFromBasket = new RelayCommand<RentalItem>(item =>
            {
                SelectedUser.RemoveFromBasket(item);
                NotifyAllFields();
            });
        }
    }
}

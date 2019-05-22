using PRBD_Framework;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using static prbd_1819_g07.App;

namespace prbd_1819_g07
{
    /// <summary>
    /// Logique d'interaction pour BasketView.xaml
    /// </summary>
    public partial class BasketView : UserControlBase
    {

        /******************************
         *                            *
         *   PROPERTIES               * 
         *                            *
         *****************************/

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
        private User selectedUser;
        public User SelectedUser
        {
            get { return selectedUser; }
            set
            {
                selectedUser = value;
                App.SelectedUser = selectedUser;
                selectedUser.Validate();
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


        /******************************
         *                            *
         *   ICOMMAND                 * 
         *                            *
         *****************************/


        //Commande pour confirmer le panier
        public ICommand ConfirmBasket { get; set; }

        //Commande pour clear le panier.
        public ICommand ClearBasket { get; set; }

        public ICommand DeleteFromBasket { get; set; }


        /******************************
         *                            *
         *   VIEW CONSTRUCTOR         * 
         *                            *
         *****************************/
        public BasketView()
        {
            InitializeComponent();

            DataContext = this;

            //var model = Model.CreateModel(DbType.MsSQL);
            Users = new ObservableCollection<User>(App.Model.Users);
            SelectedUser = App.Model.Users.Where(u => u.UserName.Contains(App.CurrentUser.UserName)).FirstOrDefault();
            ConfirmBasket = new RelayCommand(ConfirmBasketAction, () => NotEmptyBasket());
            ClearBasket = new RelayCommand(ClearAllBasket, () => NotEmptyBasket());
            DeleteFromBasket = new RelayCommand<RentalItem>(item => { DeleteFromBasketAction(item); });

            App.Register(this, AppMessages.MSG_RENTAL_CHANGED, () => { RaisePropertyChanged(nameof(Basket)); });
            App.Register<Book>(this, AppMessages.MSG_BOOK_CHANGED, (b) => { RaisePropertyChanged(nameof(Basket)); });
        }



        /*************************************************
         *                                               *
         *   METHODE D'ACTIVATION DES BOUTONS            * 
         *                                               *
         *************************************************/


        //Renvoie si true le panier n'est pas vide false s'il est vide. 
        private bool NotEmptyBasket()
        {
            if (SelectedUser.Basket != null)
            {
                return SelectedUser.Basket.Items != null;
            }
            return false;
        }

        /******************************
         *                            *
         *   METHODE ACTION           * 
         *                            *
         *****************************/

        private void DeleteFromBasketAction(RentalItem item)
        {
            SelectedUser.RemoveFromBasket(item);
            NotifyAllFields();
            App.NotifyColleagues(AppMessages.MSG_RENTAL_CHANGED);

        }

        //Méthode d'action pour le bouton ConfirmBasket
        private void ConfirmBasketAction()
        {
            SelectedUser.ConfirmBasket();
            App.Model.SaveChanges();
            NotifyAllFields();
            App.NotifyColleagues(AppMessages.MSG_RENTAL_CHANGED);

        }

        //Méthode d'action pour le bouton ClearBasket
        private void ClearAllBasket()
        {
            SelectedUser.ClearBasket();
            App.Model.SaveChanges();
            NotifyAllFields();
            App.NotifyColleagues(AppMessages.MSG_RENTAL_CHANGED);
        }

    }
}


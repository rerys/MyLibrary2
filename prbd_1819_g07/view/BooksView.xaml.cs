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
using PRBD_Framework;
using static prbd_1819_g07.App;

namespace prbd_1819_g07
{
    /// <summary>
    /// Logique d'interaction pour BooksView.xaml
    /// </summary>
    public partial class BooksView : UserControlBase
    {



        /*********************************************************************************************************************************
         *
         *   ICOMMAND
         *
         *********************************************************************************************************************************/

        //commande de création d'un nouveau livre
        public ICommand NewBook { get; set; }

        //commande d'affichage du livre selectionné
        public ICommand DisplayBookDetails { get; set; }

        //commande d'ajout du livre dans le panier
        public ICommand AddToBasket { get; set; }

        //commande d'effacement des données dans le filtre
        public ICommand ClearFilter { get; set; }



        /*********************************************************************************************************************************
         *
         *   VIEW CONSTRUCTOR
         *
         *********************************************************************************************************************************/


        public BooksView()
        {

            InitializeComponent();

            if (DesignerProperties.GetIsInDesignMode(this))
                return;

            DataContext = this;

            AddToBasket = new RelayCommand<Book>((book) =>
            {
                App.SelectedUser.AddToBasket(book);
                ApplyFilterAction();
                App.NotifyColleagues(AppMessages.MSG_RENTAL_CHANGED);


            }, (book) => CanAddBasket(book));
            //,(book) => CanAddBasket(book)
            Books = new ObservableCollection<Book>(App.Model.Books);

            

            ClearFilter = new RelayCommand(() =>
            {
                Filter = "";
                FilterCat = null;
            });

            NewBook = new RelayCommand(() =>
            {

                App.NotifyColleagues(AppMessages.MSG_NEW_BOOK);

            });
            //,()=> CanCreatNewBook()
            DisplayBookDetails = new RelayCommand<Book>(book =>
            {
                App.NotifyColleagues(AppMessages.MSG_DISPLAY_BOOK, book);
            });
            FilterCat = CategoryAll;
            App.Register<Book>(this, AppMessages.MSG_BOOK_CHANGED, book => { ApplyFilterAction(); });
            App.Register(this, AppMessages.MSG_CATEGORY_CHANGED, () => { RaisePropertyChanged(nameof(Categories)); });
            App.Register(this, AppMessages.MSG_RENTAL_CHANGED, () => { ApplyFilterAction(); });
        }


        /*********************************************************************************************************************************
         *
         *   PROPERTIES
         *
         *********************************************************************************************************************************/

        /*
         * Liste des livres en base de données
         */

        private ObservableCollection<Book> books;

        public ObservableCollection<Book> Books
        {

            get => books;

            set => SetProperty<ObservableCollection<Book>>(ref books, value);
        }



        /*
         * liste de toutes les categories disponible pour le filtre
         */

        public ObservableCollection<Category> Categories { get {
               
                var ls = new ObservableCollection<Category>(App.Model.Categories);
                ls.Insert(0, CategoryAll);
                return ls;
            } }


        /*
         * Proprieté du filtre textuelle
         */

        private string filter;
        public string Filter
        {
            get => filter;

            set => SetProperty<string>(ref filter, value, ApplyFilterAction);
        }


        /*
         * Proprieté du filtre categorie
         */

        private Category filterCat;
        public Category FilterCat
        {
            get => filterCat;
            set => SetProperty<Category>(ref filterCat, value, ApplyFilterAction);
        }

        /*
         * fake category All
         */
        private Category catAll;
        private Category CategoryAll {
            get
            {
                if (catAll == null)
                {
                    catAll = App.Model.Categories.Create();
                    catAll.Name = "All Categories";
                    catAll.CategoryId = -1;
                }
                return catAll;
            }
        }
        


        /********************************************************************************************************************************
         * 
         * METHODES D'ACTION
         * 
         *********************************************************************************************************************************/


        /*
         *Méthode d'application du filtre 
         * 
         * si reçoit un filtre text retourne une liste de livres contenant le text
         * 
         * si pas de filtre retourne la liste de tous les livres de la base de données
         * 
         * si reçoit un filtre categorie, filtre la liste des book precedement recu un des deux retour precedent
         * 
         */

        private void ApplyFilterAction()
        {

            IQueryable<Book> query;

            if (!string.IsNullOrEmpty(Filter))
            {
                query = from m in App.Model.Books
                        where
                        m.Title.Contains(Filter) || m.Author.Contains(Filter) || m.Editor.Contains(Filter)
                        select m;
            }
            else
            {
                query = App.Model.Books;
            }

            if (FilterCat != null && FilterCat.CategoryId != -1 )
            {
                query = from m in query
                        where (from c in m.Categories where c.CategoryId == filterCat.CategoryId select c).Count() > 0
                        select m;

            }
            Books = new ObservableCollection<Book>(query.OrderBy(b => b.Title));
            RaisePropertyChanged(nameof(Books));
        }


        public bool CanAddBasket(Book b)
        {
            if (b == null) return false;
            return b.NumAvailableCopies != 0;
        }


        public bool CanCreatNewBook
        {
            get{ return App.CurrentUser.Role == Role.Admin; }
        }

    }
}

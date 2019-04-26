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
        public ICommand NewBook { get; set; }
        public ICommand DisplayBookDetails { get; set; }

        public BooksView()
        {

            InitializeComponent();

            if (DesignerProperties.GetIsInDesignMode(this))
                return;

            DataContext = this;

            var model = Model.CreateModel(DbType.MsSQL);

            Books = new ObservableCollection<Book>(model.Books);

            Categories = new ObservableCollection<Category>(model.Categories);

            ClearFilter = new RelayCommand(() =>
            {
                Filter = "";
                FilterCat = null;
            });

            NewBook = new RelayCommand(() =>
            {

                App.NotifyColleagues(AppMessages.MSG_NEW_BOOK);

            });

            DisplayBookDetails = new RelayCommand<Book>(book =>
            {
                App.NotifyColleagues(AppMessages.MSG_DISPLAY_BOOK, book);
            });

            App.Register<Book>(this, AppMessages.MSG_BOOK_CHANGED, book => { ApplyFilterAction(); });
        }

        public ICommand ClearFilter { get; set; }

        private ObservableCollection<Book> books;

        public ObservableCollection<Book> Books
        {

            get => books;

            set => SetProperty<ObservableCollection<Book>>(ref books, value);
        }

        private ObservableCollection<Category> categories;

        public ObservableCollection<Category> Categories
        {

            get => categories;

            set => SetProperty<ObservableCollection<Category>>(ref categories, value, () =>
            {
            });

        }

        private string filter;
        public string Filter
        {
            get => filter;

            set => SetProperty<string>(ref filter, value, ApplyFilterAction);
        }

        private Category filterCat;
        public Category FilterCat
        {
            get => filterCat;
            set => SetProperty<Category>(ref filterCat, value, ApplyFilterAction);
        }

        private void ApplyFilterAction()
        {
            //IEnumerable<Book> query = App.Model.Books;

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

            if (FilterCat != null)
            {
                query = from m in query
                        where (from c in m.Categories where c.CategoryId == filterCat.CategoryId select c).Count() > 0                
                        select m;
              
            }
            Books = new ObservableCollection<Book>(query.OrderBy(b=> b.Title));



        }
    }
}

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

        public BooksView()
        {

            InitializeComponent();

            if (DesignerProperties.GetIsInDesignMode(this))
                return;

            DataContext = this;

            var model = Model.CreateModel(DbType.MsSQL);

            Books = new ObservableCollection<Book>(model.Books);

            ClearFilter = new RelayCommand(() => Filter = "");

            NewBook = new RelayCommand(() => {

                App.NotifyColleagues(AppMessages.MSG_NEW_BOOK);

            });

        }

        public ICommand ClearFilter { get; set; }

        private ObservableCollection<Book> books;

        public ObservableCollection<Book> Books
        {

            get => books;

            set => SetProperty<ObservableCollection<Book>>(ref books, value, () => {
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

            var query = from m in model.Books
                        where
                            m.Title.Contains(Filter) || m.Author.Contains(Filter) || m.Editor.Contains(Filter)
                        select m;

            Books = new ObservableCollection<Book>(query);
        }
    }
}

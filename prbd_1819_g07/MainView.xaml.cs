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
using PRBD_Framework;
using static prbd_1819_g07.App;

namespace prbd_1819_g07
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainView : WindowBase
    {

        public string currentUserPseudo
        {
            get
            {
                return App.CurrentUser.UserName;
            }
        }

        public Role currentUserRole
        {
            get { return App.CurrentUser.Role; }
        }

        private BooksView books = new BooksView();

        private BasketView basket = new BasketView();

        private RentalsView rentals = new RentalsView();

        private UsersView users = new UsersView();

        private CategoriesView categories = new CategoriesView();

        public MainView()
        {
            DataContext = this;

            //var model = Model.CreateModel(DbType.MsSQL);


            App.Register(this, AppMessages.MSG_NEW_USER, () =>
            {
                var user = App.Model.Users.Create();
                GridPrincipal.Children.Clear();
                GridPrincipal.Children.Add(new NewUserView(user));
            });

            App.Register(this, AppMessages.MSG_CANCEL_NEWUSER, () =>
              {
                  GridPrincipal.Children.Clear();
                  GridPrincipal.Children.Add(new UsersView());
              });

            App.Register(this, AppMessages.MSG_NEW_BOOK, () =>
            {

                var book = App.Model.Books.Create();
                App.Model.Books.Add(book);
                GridPrincipal.Children.Clear();
                GridPrincipal.Children.Add(new BookDetailsView(book, true));


            });

            App.Register(this, AppMessages.MSG_CANCEL_VIEWDETAIL_BOOK, () =>
            {
                GridPrincipal.Children.Clear();
                GridPrincipal.Children.Add(books);


            });

            App.Register<Book>(this, AppMessages.MSG_DISPLAY_BOOK, book =>
            {
                if (book != null)
                {
                    GridPrincipal.Children.Clear();
                    GridPrincipal.Children.Add(new BookDetailsView(book, false));
                }
            });
            InitializeComponent();
        }

        private void ButtonFechar_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void ListViewMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index = ListViewMenu.SelectedIndex;
            MoveCursorMenu(index);

            switch (index)
            {
                case 0:
                    GridPrincipal.Children.Clear();
                    GridPrincipal.Children.Add(new HomeView());
                    break;
                case 1:
                    GridPrincipal.Children.Clear();
                    GridPrincipal.Children.Add(books);
                    break;
                case 2:
                    GridPrincipal.Children.Clear();
                    GridPrincipal.Children.Add(categories);
                    break;
                case 3:
                    GridPrincipal.Children.Clear();
                    GridPrincipal.Children.Add(basket);
                    break;
                case 4:
                    GridPrincipal.Children.Clear();
                    GridPrincipal.Children.Add(rentals);
                    break;
                case 5:
                    GridPrincipal.Children.Clear();
                    GridPrincipal.Children.Add(users);
                    break;
                default:
                    break;
            }
        }

        private void MoveCursorMenu(int index)
        {
            TrainsitionigContentSlide.OnApplyTemplate();
            GridCursor.Margin = new Thickness(0, (190 + (60 * index)), 0, 0);
        }

    }
}

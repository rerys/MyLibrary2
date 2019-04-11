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

  

        public MainView()
        {
            DataContext = this;

            var model = Model.CreateModel(DbType.MsSQL);

           // App.Register(this, AppMessages.MSG_NEW_BOOK, () => {

              //  var tab = new TabItem()
              //  {

              //      Header = "<new book>"

              //  };

                //tabControl.Items.Add(tab);

              //  Dispatcher.InvokeAsync(() => tab.Focus());

           // });

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
                    //GridPrincipal.Children.Add(new BooksView());
                    break;
                case 1:
                    GridPrincipal.Children.Clear();
                    GridPrincipal.Children.Add(new BooksView());
                    // GridPrincipal.Children.Add(new UserControlEscolha());
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

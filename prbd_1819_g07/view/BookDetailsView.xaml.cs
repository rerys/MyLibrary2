using PRBD_Framework;
using System;
using System.Collections.Generic;
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
using System.Collections.ObjectModel;
using System.ComponentModel;
using static prbd_1819_g07.App;


namespace prbd_1819_g07
{
    /// <summary>
    /// Logique d'interaction pour BookDetailsView.xaml
    /// </summary>
    public partial class BookDetailsView : UserControlBase
    {

        public ICommand Cancel { get; set; }

        public BookDetailsView()
        {
            InitializeComponent();

            DataContext = this;

            Cancel = new RelayCommand(() => {

                App.NotifyColleagues(AppMessages.MSG_CANCEL_VIEWDETAIL_BOOK);

            });

        }
    }
}

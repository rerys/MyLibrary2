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


namespace prbd_1819_g07
{
    /// <summary>
    /// Logique d'interaction pour CategoriesView.xaml
    /// </summary>
    public partial class CategoriesView : UserControlBase
    {
        public CategoriesView()
        {
            InitializeComponent();

            if (DesignerProperties.GetIsInDesignMode(this))
                return;

            DataContext = this;
            Categories = new ObservableCollection<Category>(App.Model.Categories);
        }

        private ObservableCollection<Category> categories;
        public ObservableCollection<Category> Categories
        {
            get { return categories; }
            set
            {
                categories = value;
                RaisePropertyChanged(nameof(categories));
            }
        }

        private Category selectedCategory;
        public Category SelectedCategory
        {
            get { return selectedCategory; }
            set
            {
                selectedCategory = value;
                RaisePropertyChanged(nameof(SelectedCategory));
            }
        }


    }
}

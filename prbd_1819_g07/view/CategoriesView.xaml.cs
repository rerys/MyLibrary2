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
using prbd_1819_g07;


namespace prbd_1819_g07
{
    /// <summary>
    /// Logique d'interaction pour CategoriesView.xaml
    /// </summary>
    public partial class CategoriesView : UserControlBase
    {
        ICommand DisplayCategory { get; set; }
        public ICommand Add { get; set; }
        public ICommand Update { get; set; }
        public ICommand Cancel { get; set; }
        public ICommand Delete { get; set; }



        public CategoriesView()
        {
            InitializeComponent();

            if (DesignerProperties.GetIsInDesignMode(this))
                return;

            DataContext = this;
            Categories = new ObservableCollection<Category>(App.Model.Categories);

            DisplayCategory = new RelayCommand<Category>(cat =>
            {
                SelectedCategory = cat;
            });

            Add = new RelayCommand(AddAction, () => CanAddAction());
            Update = new RelayCommand(UpdateAction, () => CanUpdateAction());
            Cancel = new RelayCommand(CancelAction, () => CanCancelAction());
            Delete = new RelayCommand(DeleteAction, () => CanDelete());

            App.Register(this, AppMessages.MSG_CATEGORY_CHANGED, () => { RefreshCategories(); });
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

        private bool IsCheckCategory()
        {
            return SelectedCategory != null;
        }

        private Category selectedCategory;
        public Category SelectedCategory
        {
            get { return selectedCategory; }
            set
            {
                selectedCategory = value;
                RaisePropertyChanged(nameof(SelectedCategory));
                RaisePropertyChanged(nameof(SelectedCategoryName));
            }
        }

        private string selectedCategoryName;
        public string SelectedCategoryName
        { 
            get { return (SelectedCategory != null) ? SelectedCategory.Name : selectedCategoryName; }
            set
            {

                //SetProperty<string>(ref selectedCategoryName, value, () => Validate());
                selectedCategoryName = value;
                if(SelectedCategory != null)
                {
                    SelectedCategory.Name = value;
                }
                

                RaisePropertyChanged(nameof(SelectedCategoryName));
                RaisePropertyChanged(nameof(SelectedCategory));
            }
        }


        private bool CanDelete()
        {
            return SelectedCategory != null;
        }

        private void DeleteAction()
        {
            SelectedCategory.Delete();
            App.Model.SaveChanges();
            RaisePropertyChanged(nameof(Categories));
            App.NotifyColleagues(AppMessages.MSG_CATEGORY_CHANGED);
        }


        private bool CanCancelAction()
        {
            return selectedCategoryName != "" && selectedCategoryName != null;
        }
        private void CancelAction()
        {

            if (SelectedCategory!= null && !SelectedCategory.IsUnchanged)
            {
                SelectedCategory.Reload();
                RaisePropertyChanged(nameof(SelectedCategoryName));
            }

            selectedCategoryName = "";

            RaisePropertyChanged(nameof(SelectedCategoryName));
        }


        private bool CanAddAction()
        {
            var NotExistCategory = (from c in App.Model.Categories
                             where c.Name == selectedCategoryName
                             select c).Count() == 0;

            return SelectedCategory == null && selectedCategoryName != "" && selectedCategoryName != null && NotExistCategory; ;
        }

        private void AddAction()
        {
            if (SelectedCategory == null)
            {
                App.Model.CreateCategory(SelectedCategoryName);

            }
            App.Model.SaveChanges();
            App.NotifyColleagues(AppMessages.MSG_CATEGORY_CHANGED);
            CancelAction();

        }


        public bool CanUpdateAction()
        {
            var UnicitytCategory = true;

            if(selectedCategory != null)
            {
                UnicitytCategory = (from c in App.Model.Categories
                                    where c.Name == selectedCategoryName && c.CategoryId != SelectedCategory.CategoryId
                                    select c).Count() == 0;

            }

            return SelectedCategory != null && !SelectedCategory.IsUnchanged && UnicitytCategory;
        }

        private void UpdateAction()
        {
            App.Model.SaveChanges();
            App.NotifyColleagues(AppMessages.MSG_CATEGORY_CHANGED);
            CancelAction();
        }


        private void RefreshCategories()
        {
            Categories = new ObservableCollection<Category>(App.Model.Categories);
        }


        public override bool Validate()
        {
            ClearErrors();
 

            if (SelectedCategoryName.Length < 3)
            {
                AddError("SelectedCategoryName", "dsfdsfds");
            }



            RaiseErrors();
            return !HasErrors;
        }

    }




}

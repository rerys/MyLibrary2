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
    /// Logique d'interaction pour UsersView.xaml
    /// </summary>
    public partial class UsersView : UserControlBase
    {
        public ICommand NewUser { get; set; }
        public ICommand ClearFilter { get; set; }

        public UsersView()
        {
            InitializeComponent();

            if (DesignerProperties.GetIsInDesignMode(this))
                return;

            DataContext = this;
            Users = new ObservableCollection<User>(App.Model.Users);
            ClearFilter = new RelayCommand(() => Filter = "");
        }

        private ObservableCollection<User> users;
        public ObservableCollection<User> Users
        {
            get { return users; }
            set
            {
                users = value;
                RaisePropertyChanged(nameof(Users));
                RaisePropertyChanged(nameof(UsersView));
            }
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

            var query = from u in model.Users
                        let text = u.UserName.Contains(Filter) || u.FullName.Contains(Filter) || u.Email.Contains(Filter)
                       // ||u.Birthdate.Contains(Filter) || u.Role.Contains(Filter) 
                        where text
                        select u;

            Users = new ObservableCollection<User>(query);
        }
        User selectedUser;
        public User SelectedUser
        {
            get { return selectedUser; }
            set
            {
                selectedUser = value;
                RaisePropertyChanged(nameof(SelectedUser));
                NotifyAllFields();
                Console.WriteLine(selectedUser);
            }
        }
        private void NotifyAllFields()
        {
            RaisePropertyChanged(nameof(Email));
            RaisePropertyChanged(nameof(UserName));
            RaisePropertyChanged(nameof(FullName));
            //RaisePropertyChanged(nameof(BirthDate));
            //RaisePropertyChanged(nameof(Role));
        }


        private bool editMode = false;
        public bool EditMode
        {
            get { return editMode; }
            set
            {
                editMode = value;
                RaisePropertyChanged(nameof(EditMode));
                RaisePropertyChanged(nameof(ReadMode));
            }
        }
        public bool ReadMode
        {
            get { return !EditMode; }
            set { EditMode = !value; }
        }

        public string UserName
        {
            get { return SelectedUser?.UserName; }
            set
            {
                SelectedUser.UserName = value;
                EditMode = true;
                RaisePropertyChanged(nameof(UserName));
                Validate();
            }
        }

        public string FullName
        {
            get { return SelectedUser?.FullName; }
            set
            {
                SelectedUser.FullName = value;
                EditMode = true;
                RaisePropertyChanged(nameof(FullName));
                Validate();
            }
        }

        public string Email
        {
            get { return SelectedUser?.Email; }
            set
            {
                SelectedUser.Email = value;
                EditMode = true;
                RaisePropertyChanged(nameof(Email));
                Validate();
            }
        }

        //public DateTime BirthDate
        //{
        //    get { return SelectedUser?.BirthDate; }
        //    set
        //    {
        //        SelectedUser.BirthDate = value;
        //        EditMode = true;
        //        RaisePropertyChanged(nameof(BirthDate));
        //        Validate();
        //    }
        //}

        //public Role Role
        //{
        //    get { return SelectedUser?.Role; }
        //    set
        //    {
        //        SelectedUser.Role = value;
        //        EditMode = true;
        //        RaisePropertyChanged(nameof(Role));
        //        Validate();
        //    }
        //}
    }
}

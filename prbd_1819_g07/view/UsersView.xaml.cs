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
using prbd_1819_g07;
using static prbd_1819_g07.App;

namespace prbd_1819_g07
{
    /// <summary>
    /// Logique d'interaction pour UsersView.xaml
    /// </summary>
    public partial class UsersView : UserControlBase
    {
        private ObservableCollection<User> users;
        public ObservableCollection<User> Users
        {
            get { return users; }
            set
            {
                users = value;
                RaisePropertyChanged(nameof(Users));
                RaisePropertyChanged(nameof(UsersListView));
            }
        }


        private string filter;
        public string Filter { get => filter; set => SetProperty<string>(ref filter, value, ApplyFilterAction); }

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
            RaisePropertyChanged(nameof(Birthdate));
            //  RaisePropertyChanged(nameof(Role));
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

        public DateTime? Birthdate
        {
            get { return SelectedUser?.BirthDate; }
            set
            {
                SelectedUser.BirthDate = value ?? System.DateTime.Now;
                EditMode = true;
                RaisePropertyChanged(nameof(Birthdate));
                Validate();
            }
        }

        //public IList<Role> Roles
        //{
        //    get
        //    {
        //        // Will result in a list like {"Tester", "Engineer"}
        //        return Enum.GetValues(typeof(Role)).Cast<Role>().ToList<Role>();
        //    }
        //}

        private Role[] roles;
        public Role[] Roles
        {
            get
            {
                Role[] roles = { Role.Admin, Role.Manager, Role.Member };
                return roles;
            }
            set
            {
                RaisePropertyChanged(nameof(Roles));
            }
        }

        //private Role? role;
        //public Role? RoleUser
        //{
        //    get
        //    {
        //        return SelectedUser?.Role;

        //    }
        //    set
        //    {
        //       // SelectedUser.Role = role;
        //        EditMode = true;
        //        RaisePropertyChanged(nameof(RoleUser));
        //        Validate();
        //    }
        //}

        // Validations métier sur le message en cours d'édition (SelectedUser)
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

        private CollectionView usersView = null;
        public CollectionView UsersListView
        {
            get
            {
                usersView = (CollectionView)CollectionViewSource.GetDefaultView(users);
                if (usersView != null && usersView.SortDescriptions.Count == 0)
                    usersView.SortDescriptions.Add(new SortDescription("UserName", ListSortDirection.Ascending));
                return usersView;
            }
        }

        public ICommand NewUser { get; set; }
        public ICommand ClearFilter { get; set; }
        public ICommand DeleteUser { get; set; }
        public ICommand SaveOneCommand { get; set; }
        public ICommand RefreshCommand { get; set; }
        public ICommand CancelCommand { get; set; }


        public UsersView()
        {
            InitializeComponent();

            if (DesignerProperties.GetIsInDesignMode(this))
                return;

            DataContext = this;
            Users = new ObservableCollection<User>(App.Model.Users);
            NewUser = new RelayCommand(() =>
            {

                App.NotifyColleagues(AppMessages.MSG_NEW_USER);

            });
            ClearFilter = new RelayCommand(() => Filter = "");
            DeleteUser = new RelayCommand(() =>
            {
                App.Model.Users.Remove(SelectedUser);
                Users.Remove(SelectedUser);
                App.Model.SaveChanges();
                SelectedUser = null;
            },
          () => { return ReadMode && SelectedUser != null; });
            SaveOneCommand = new RelayCommand(() =>
            {
                var UserID = selectedUser.UserId;
                App.Model.SaveChanges();
                RaisePropertyChanged(nameof(UsersListView));
                RefreshGrid();
                EditMode = false;
                SelectedUser = (from u in Users where u.UserId == UserID select u).SingleOrDefault();
            },
            () => { return EditMode && !HasErrors; });
            RefreshCommand = new RelayCommand(() =>
            {
                App.CancelChanges();
                RefreshGrid();
                Users = new ObservableCollection<User>(App.Model.Users);
            },
            () => { return ReadMode; });
            CancelCommand = new RelayCommand(() =>
            {
                var UserID = SelectedUser.UserId;
                App.CancelChanges();
                RaisePropertyChanged(nameof(UsersListView));
                RefreshGrid();
                EditMode = false;
                Users = new ObservableCollection<User>(App.Model.Users);
                SelectedUser = (from u in Users where u.UserId == UserID select u).SingleOrDefault();
                Validate();
            },
            () => { return EditMode; });
        }

        private void RefreshGrid()
        {
            int UserID = -1;
            if (SelectedUser != null)
                UserID = SelectedUser.UserId;
            Users.RefreshFromModel(App.Model.Users);
            if (SelectedUser != null)
                SelectedUser = (from m in Users where m.UserId == UserID select m).FirstOrDefault();
            RaisePropertyChanged(nameof(UsersListView));
        }
    }
}

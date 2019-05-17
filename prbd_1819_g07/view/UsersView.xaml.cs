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



        /******************************
         *                            *
         *   PROPERTIES               * 
         *                            *
         *****************************/

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



        User selectedUser;
        public User SelectedUser
        {
            get { return selectedUser; }
            set
            {
                selectedUser = value;
                RaisePropertyChanged(nameof(SelectedUser));
                if (HasUserSelected)
                {
                    birthDate = selectedUser.BirthDate;
                    role = selectedUser.Role;
                }
                NotifyAllFields();
                Console.WriteLine(selectedUser);
            }
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

        private DateTime? birthDate;
        public DateTime? BirthDate
        {
            get { return birthDate; }
            set
            {
                SetProperty<DateTime?>(ref birthDate, value, () => Validate());
                EditMode = true;
                RaisePropertyChanged(nameof(BirthDate));
            }
        }

        private Role role;
        public Role Role
        {
            get { return role; }
            set
            {
                SetProperty<Role>(ref role, value, () => Validate());
                EditMode = true;
                RaisePropertyChanged(nameof(Role));
            }
        }

       // private Role[] roles;
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

        //Renvoie true si un rental a été selectionné. 
        public bool HasUserSelected
        {
            get { return selectedUser != null; }
        }


        /******************************
         *                            *
         *   ICOMMAND                 * 
         *                            *
         *****************************/


        public ICommand NewUser { get; set; }
        public ICommand ClearFilter { get; set; }
        public ICommand DeleteUser { get; set; }
        public ICommand SaveOneCommand { get; set; }
        public ICommand RefreshCommand { get; set; }
        public ICommand CancelCommand { get; set; }



        /******************************
         *                            *
         *   VIEW CONSTRUCTOR         * 
         *                            *
         *****************************/


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
            DeleteUser = new RelayCommand(DeleteAction, () => ReadMode && SelectedUser != null);
            SaveOneCommand = new RelayCommand(SaveAction, () => EditMode && !HasErrors);
            RefreshCommand = new RelayCommand(RefreshAction, () => ReadMode);
            CancelCommand = new RelayCommand(CancelAction, () => EditMode);
        }


        /******************************
         *                            *
         *   METHODE ACTION           * 
         *                            *
         *****************************/

        private void DeleteAction()
        {
            App.Model.Users.Remove(SelectedUser);
            Users.Remove(SelectedUser);
            App.Model.SaveChanges();
            SelectedUser = null;
        }

        private void SaveAction()
        {
            var UserID = selectedUser.UserId;
            App.Model.SaveChanges();
            RaisePropertyChanged(nameof(UsersListView));
            RefreshGrid();
            EditMode = false;
            SelectedUser = (from u in Users where u.UserId == UserID select u).SingleOrDefault();
        }

        private void RefreshAction()
        {
            App.CancelChanges();
            RefreshGrid();
            Users = new ObservableCollection<User>(App.Model.Users);
        }

        private void CancelAction()
        {
            var UserID = SelectedUser.UserId;
            App.CancelChanges();
            RaisePropertyChanged(nameof(UsersListView));
            RefreshGrid();
            EditMode = false;
            Users = new ObservableCollection<User>(App.Model.Users);
            SelectedUser = (from u in Users where u.UserId == UserID select u).SingleOrDefault();
            Validate();
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


        private void ApplyFilterAction()
        {
            var query = from u in App.Model.Users
                        let text = u.UserName.Contains(Filter) || u.FullName.Contains(Filter) || u.Email.Contains(Filter)
                        // ||u.Birthdate.Contains(Filter) || u.Role.Contains(Filter) 
                        where text
                        select u;

            Users = new ObservableCollection<User>(query);
        }

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

        private void NotifyAllFields()
        {
            RaisePropertyChanged(nameof(Email));
            RaisePropertyChanged(nameof(UserName));
            RaisePropertyChanged(nameof(FullName));
            RaisePropertyChanged(nameof(BirthDate));
            //  RaisePropertyChanged(nameof(Role));
        }
    }
}

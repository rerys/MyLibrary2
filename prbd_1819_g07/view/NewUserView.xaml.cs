using PRBD_Framework;
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
using static prbd_1819_g07.App;

namespace prbd_1819_g07
{
    /// <summary>
    /// Logique d'interaction pour NewUserView.xaml
    /// </summary>
    public partial class NewUserView : UserControlBase
    {
        public User User { get; set; }

        private ObservableCollection<User> users;
        public ObservableCollection<User> Users
        {
            get { return users; }
            set
            {
                users = value;
                RaisePropertyChanged(nameof(Users));
            }
        }

        private string userName;
        public string UserName
        {
            get { return userName; }
            set
            {
                SetProperty<string>(ref userName, value, () => Validate());
            }
        }


        private string fullName;
        public string FullName
        {
            get { return fullName; }
            set
            {
                SetProperty<string>(ref fullName, value, () => Validate());
            }
        }
        private string email;
        public string Email
        {
            get { return email; }
            set
            {
                SetProperty<string>(ref email, value, () => Validate());
            }
        }

        private Nullable<DateTime> birthDate;
        public Nullable<DateTime> BirthDate
        {
            get { return birthDate; }
            set
            {
                birthDate = value;
                RaisePropertyChanged(nameof(birthDate));
            }
        }

        private Role role;
        public Role Role
        {
            get { return role; }
            set
            {
                role = value;
                RaisePropertyChanged(nameof(role));
            }
        }
        public override bool Validate()
        {
            ClearErrors();
            var member = App.Model.Users.Where(u => u.UserName == UserName).SingleOrDefault();
            if (string.IsNullOrEmpty(UserName))
            {
                AddError("UserName", Properties.Resources.Error_Required);
            }
            else
            {
                if (UserName.Length < 3)
                {
                    AddError("UserName", Properties.Resources.Error_LengthGreaterEqual3);
                }
            }
            if (string.IsNullOrEmpty(FullName))
            {
                AddError("FullName", Properties.Resources.Error_Required);
            }
            if (string.IsNullOrEmpty(Email))
            {
                AddError("Email", Properties.Resources.Error_Required);
            }
            RaiseErrors();
            return !HasErrors;
        }

        public ICommand Save { get; set; }
        public ICommand Cancel { get; set; }


        public NewUserView(User user)
        {
            InitializeComponent();
            DataContext = this;
            Save = new RelayCommand(SaveAction);
            Cancel = new RelayCommand(CancelAction);
        }

        private void CancelAction()
        {

            App.NotifyColleagues(AppMessages.MSG_CANCEL_NEWUSER);
        }

        private void SaveAction()
        {
            if (Validate())
            {
                User user1 = App.Model.CreateUser(userName, userName, fullName, email, birthDate, Role.Member);
                App.Model.Users.Add(user1);
                App.Model.SaveChanges();
                var member = App.Model.Users.Where(u => u.UserName == UserName).SingleOrDefault();
                App.CurrentUser = member;
                App.Model.SaveChanges();
                App.NotifyColleagues(AppMessages.MSG_CANCEL_NEWUSER);
                App.NotifyColleagues(AppMessages.MSG_USER_CHANGED, user1);
            }

        }

    }
}



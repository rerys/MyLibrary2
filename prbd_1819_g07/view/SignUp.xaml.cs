using System;
using PRBD_Framework;
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
using System.Windows.Shapes;

namespace prbd_1819_g07.view
{
    /// <summary>
    /// Logique d'interaction pour SignUp.xaml
    /// </summary>
    public partial class SignUp : WindowBase
    {
        public User User { get; set; }
        private ImageHelper imageHelper;


        public ICommand Save { get; set; }
        public ICommand Cancel { get; set; }
        public ICommand ClearImage { get; set; }
        public ICommand Delete { get; set; }
        public ICommand LoadImage { get; set; }


        public SignUp()
        {
            //InitializeComponent();
            DataContext = this;
            Save = new RelayCommand(SaveAction, CanSaveOrCancelAction);
            //Cancel = new RelayCommand(CancelAction);
            //ClearImage = new RelayCommand(ClearImageAction);
            //Delete = new RelayCommand(DeleteAction);
            //LoadImage = new RelayCommand(LoadImageAction);
        }
        private bool CanSaveOrCancelAction()
        {
            return !string.IsNullOrEmpty(UserName) && !HasErrors;
        }


        private void SaveAction()
        {
            if (Validate())
            {
                //Static pas static fuuck
                //User user1 = Model.CreateUser(userName, password, fullName, email, birthDate, role);
                //Model.Users.Add(user1);
                //Model.SaveChanges();
                var member = Model.Users.Find(UserName); // on recherche le membre 
                App.CurrentUser = member; // le membre connecté devient le membre courant
                ShowMainView(); // ouverture de la fenêtre principale
                Close();
            }

        }

        private static void ShowMainView()
        {
            var mainView = new MainView();
            mainView.Show();
            Application.Current.MainWindow = mainView;
        }

        private string userName;
        public string UserName
        {
            get { return userName; }
            set
            {
                userName = value;
                RaisePropertyChanged(nameof(UserName));
            }
        }

        private string fullName;
        public string FullName
        {
            get { return fullName; }
            set
            {
                fullName = value;
                RaisePropertyChanged(nameof(FullName));
            }
        }

        private string absolutePicturePath;
        public string AbsolutePicturePath
        {
            get { return absolutePicturePath; }
            set
            {
                absolutePicturePath = value;
                RaisePropertyChanged(nameof(AbsolutePicturePath));
            }
        }

        private string password;
        public string Password
        {
            get { return password; }
            set
            {
                password = value;
                RaisePropertyChanged(nameof(Password));
            }
        }

        private string confirmPassword;
        public string ConfirmPassword
        {
            get { return confirmPassword; }
            set
            {
                confirmPassword = value;
                RaisePropertyChanged(nameof(confirmPassword));
            }
        }
        private string email;
        public string Email
        {
            get { return email; }
            set
            {
                email = value;
                RaisePropertyChanged(nameof(email));
            }
        }

        private DateTime birthDate;
        public DateTime BirthDate
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
                role = Role.Member;
                RaisePropertyChanged(nameof(role));
            }
        }
        public override bool Validate()
        {
            ClearErrors();
            var member = Model.Users.Find(UserName);
            if (string.IsNullOrEmpty(UserName))
            {
                AddError("Pseudo", Properties.Resources.Error_Required);
            }
            else
            {
                if (UserName.Length < 3)
                {
                    AddError("Pseudo", Properties.Resources.Error_LengthGreaterEqual3);
                }
            }

            if (string.IsNullOrEmpty(Password) || string.IsNullOrEmpty(ConfirmPassword))
            {
                AddError("Password", Properties.Resources.Error_Required);
            }
            if (ConfirmPassword != Password)
            {
                AddError("ConfirmPassword", Properties.Resources.Error_PasswordDontMatch);
            }

            RaiseErrors();
            return !HasErrors;
        }
    }
}

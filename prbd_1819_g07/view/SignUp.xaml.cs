﻿using System;
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
using prbd_1819_g07;


namespace prbd_1819_g07
{
    /// <summary>
    /// Logique d'interaction pour SignUp.xaml
    /// </summary>
    public partial class SignUp : WindowBase
    {
        public User User { get; set; }


        public ICommand Save { get; set; }
        public ICommand Cancel { get; set; }
        public ICommand CancelApp { get; set; }


        public SignUp()
        {
            InitializeComponent();
            DataContext = this;
            Save = new RelayCommand(SaveAction, CanSaveOrCancelAction);
            Cancel = new RelayCommand(CancelAction);
            CancelApp = new RelayCommand(() => Close());
        }
        private bool CanSaveOrCancelAction()
        {
            return !string.IsNullOrEmpty(UserName) && !HasErrors;
        }

        private void CancelAction()
        {
            ShowLogIn();
            Close();
        }
        private void ShowLogIn()
        {
            var loginview = new LoginView();
            loginview.Show();
            Application.Current.MainWindow = loginview;
        }

        private void SaveAction()
        {
            if (Validate())
            {
                User user1 = App.Model.CreateUser(userName, password, fullName, email, birthDate, role);
                App.Model.Users.Add(user1);
                App.Model.SaveChanges();
                var member = App.Model.Users.Where(u => u.UserName == UserName).SingleOrDefault();
                App.CurrentUser = member;
                ShowMainView();
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


        private string password;
        public string Password
        {
            get { return password; }
            set
            {
                SetProperty<string>(ref password, value, () => Validate());
            }
        }

        private string confirmPassword;
        public string ConfirmPassword
        {
            get { return confirmPassword; }
            set
            {
                SetProperty<string>(ref confirmPassword, value, () => Validate());
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

        private DateTime? birthDate;
        public DateTime? Birthdate
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
            if (member != null)
            {
                if (UserName == member.UserName)
                {
                    AddError("UserName", Properties.Resources.Error_NotAvailable);
                }
                if (Email == member.Email)
                {
                    AddError("Email", Properties.Resources.Error_NotAvailable);
                }

            }

            if (string.IsNullOrEmpty(Password))
            {
                AddError("Password", Properties.Resources.Error_Required);
            }
            if (string.IsNullOrEmpty(ConfirmPassword))
            {
                AddError("ConfirmPassword", Properties.Resources.Error_Required);
            }
            if (ConfirmPassword != Password)
            {
                AddError("ConfirmPassword", Properties.Resources.Error_PasswordDontMatch);
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
    }
}

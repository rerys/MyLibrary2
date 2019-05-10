using prbd_1819_g07;
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
using System.Windows.Shapes;

namespace prbd_1819_g07
{
    /// <summary>
    /// Logique d'interaction pour LoginView.xaml
    /// </summary>
    public partial class LoginView : WindowBase
    {
        private string pseudo;
        public string Pseudo { get => pseudo; set => SetProperty<string>(ref pseudo, value, () => Validate()); }

        private string password;
        public string Password { get => password; set => SetProperty<string>(ref password, value, () => Validate()); }

        public ICommand Login { get; set; }
        public ICommand Cancel { get; set; }
        public ICommand SignUp { get; set; }

        public LoginView()
        {
            InitializeComponent();
            DataContext = this;
            Login = new RelayCommand(LoginAction,
                        () => { return pseudo != null && password != null && !HasErrors; });
            Cancel = new RelayCommand(() => Close());
            SignUp = new RelayCommand(() => SignUpAction());
        }

        private void SignUpAction()
        {
            ShowSignUp();
            Close();

        }

        private static void ShowSignUp()
        {
            var signupview = new SignUp();
            signupview.Show();
            Application.Current.MainWindow = signupview;
        }


        private void LoginAction()
        {
            if (Validate())
            { // si aucune erreurs
                var user = App.Model.Users.Where(u => u.UserName == Pseudo).SingleOrDefault(); // on recherche le membre 
                App.CurrentUser = user; // le membre connecté devient le membre courant
                App.SelectedUser = App.CurrentUser;
                ShowMainView(); // ouverture de la fenêtre principale
                Close(); // fermeture de la fenêtre de login
            }
        }
        private static void ShowMainView()
        {
            var mainView = new MainView();
            mainView.Show();
            Application.Current.MainWindow = mainView;
        }

        public override bool Validate()
        {
            ClearErrors();
            var member = App.Model.Users.Where(u => u.UserName == Pseudo).SingleOrDefault();
            if (string.IsNullOrEmpty(Pseudo))
            {
                AddError("Pseudo", Properties.Resources.Error_Required);
            }
            else
            {
                if (Pseudo.Length < 3)
                {
                    AddError("Pseudo", Properties.Resources.Error_LengthGreaterEqual3);
                }
                else
                {
                    if (member == null)
                    {
                        AddError("Pseudo", Properties.Resources.Error_DoesNotExist);
                    }
                }
            }

            if (string.IsNullOrEmpty(Password))
            {
                AddError("Password", Properties.Resources.Error_Required);
            }
            else
            {
                if (member != null && member.Password != Password)
                {
                    AddError("Password", Properties.Resources.Error_WrongPassword);
                }
            }

            RaiseErrors();
            return !HasErrors;
        }
    }
}
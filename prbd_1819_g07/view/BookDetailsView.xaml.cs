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
using Microsoft.Win32;
using System.Data.Entity;
using System.IO;
using System.Text.RegularExpressions;

namespace prbd_1819_g07
{
    /// <summary>
    /// Logique d'interaction pour BookDetailsView.xaml
    /// </summary>
    public partial class BookDetailsView : UserControlBase
    {



        /*********************************************************************************************************************************
         *
         *   INNER CLASS
         *
         *********************************************************************************************************************************/

        /*
         *Class interne pour la géstion des categories dans les checkbox
         */
        public class CheckedCategory
        {
            public int IdCategory { get; set; }
            public string Name { get; set; }
            public bool IsChecked { get; set; }
        }


        /*********************************************************************************************************************************
         *
         *   ICOMMAND
         *
         *********************************************************************************************************************************/

        // Commande pour l'ajout d'un nouveau livre
        public ICommand Add { get; set; }

        //Commande pour la sauvegarde (modification ou ajout d'un nouveau livre)
        public ICommand Save { get; set; }

        // Commande d'annulation des modifications
        public ICommand Cancel { get; set; }

        // Commande de suppression d'un livre
        public ICommand Delete { get; set; }

        // Commande de chargement d'une nouvelle image
        public ICommand LoadImage { get; set; }

        // Commande de remise de l'image associée au livre
        public ICommand ClearImage { get; set; }

        // Commande de fermeture de la page
        public ICommand Exit { get; set; }

        // Commande pour enregistrer si une categories a été selectioné
        public ICommand CategorieChanged { get; set; }

        // Commande pour l'ajout de copies
        public ICommand AddCopies { get; set; }



        /*********************************************************************************************************************************
         *
         *   VIEW CONSTRUCTOR
         *
         *********************************************************************************************************************************/

        public BookDetailsView(Book book, bool isNew)
        {
            InitializeComponent();

            if (DesignerProperties.GetIsInDesignMode(this))
                return;

            DataContext = this;

            Book = book;
            IsNew = isNew;

            SetCopies();
            imageHelper = new ImageHelper(App.IMAGE_PATH, Book.PicturePath);

            Save = new RelayCommand(SaveAction, CanSaveOrCancelAction);
            Cancel = new RelayCommand(CancelAction, CanSaveOrCancelAction);
            Delete = new RelayCommand(DeleteAction, () => !IsNew);
            LoadImage = new RelayCommand(LoadImageAction);
            ClearImage = new RelayCommand(ClearImageAction, () => PicturePath != null);
            CategorieChanged = new RelayCommand(Change);
            AddCopies = new RelayCommand(AddCopiesAction, CanAddCopies);

            Exit = new RelayCommand(() =>
            {
                CancelAction();
                App.NotifyColleagues(AppMessages.MSG_CANCEL_VIEWDETAIL_BOOK);

            });

        }


        /*********************************************************************************************************************************
         *
         *   PROPERTIES
         *
         *********************************************************************************************************************************/

        //Propriété du livre courant 
        public Book Book { get; set; }

        //Propriété du géstionnaire d'image
        private ImageHelper imageHelper;

        //Propriété copies a été ajoutées
        private bool copiesAdded = false;
        public bool CopiesAdded
        {
            get => copiesAdded;
            set
            {
                copiesAdded = value;
                RaisePropertyChanged(nameof(CopiesAdded));
            }
        }


        //Propriété categories a été ajouté ou retiré
        private bool catChanged = false;
        public bool CatChanged
        {
            get => catChanged;
            set
            {
                catChanged = value;
                RaisePropertyChanged(nameof(CatChanged));
            }
        }


        //Propriété de la liste des copies du livre courant 
        private ObservableCollection<BookCopy> bookCopies;
        public ObservableCollection<BookCopy> BookCopies
        {

            get => bookCopies;

            set => SetProperty<ObservableCollection<BookCopy>>(ref bookCopies, value);
        }


        //Propriété du titre de la vue
        private string viewName;
        public string ViewName
        {
            get
            {
                if (IsNew)
                {
                    viewName = "New Book";
                }
                else
                {
                    viewName = Isbn;
                }

                return viewName;
            }
        }

        //Propriété si le livre courant est nouvau ou une modification
        private bool isNew;
        public bool IsNew
        {
            get { return isNew; }
            set
            {
                isNew = value;
                RaisePropertyChanged(nameof(IsNew));
                RaisePropertyChanged(nameof(IsExisting));
            }
        }

        //Propriété si le livre existe dans la base de données
        public bool IsExisting { get => !isNew; }

        //Propriété ISBN du livre courant
        private string isbn;
        public string Isbn
        {
            get { return Book.Isbn; }
            set
            {
                Book.Isbn = value;
                SetProperty<string>(ref isbn, value, () => Validate());
                RaisePropertyChanged(nameof(Isbn));
            }
        }

        //Propriété titre du livre courant
        private string title;
        public string Title
        {
            get { return Book.Title; }
            set
            {
                Book.Title = value;
                SetProperty<string>(ref title, value, () => Validate());
                RaisePropertyChanged(nameof(Title));
            }
        }

        //Propriété auteur du livre courant
        private string author;
        public string Author
        {
            get { return Book.Author; }
            set
            {
                Book.Author = value;
                SetProperty<string>(ref author, value, () => Validate());
                RaisePropertyChanged(nameof(Author));
            }
        }

        //Propriété editeur du livre courant
        private string editor;
        public string Editor
        {
            get { return Book.Editor; }
            set
            {
                Book.Editor = value;
                SetProperty<string>(ref editor, value, () => Validate());
                RaisePropertyChanged(nameof(Editor));
            }
        }

        //Propriété image du livre courant
        public string PicturePath
        {
            get { return Book.AbsolutePicturePath; }
            set
            {
                RaisePropertyChanged(nameof(PicturePath));
                Book.PicturePath = value;
            }
        }

        //Propriété date d'acquisition des copies à ajouter au livre courant
        private DateTime dateCopiesToAdd = DateTime.Now;
        public DateTime DateCopiesToAdd
        {
            get { return dateCopiesToAdd; }
            set
            {
                SetProperty<DateTime>(ref dateCopiesToAdd, value, () => Validate());
                RaisePropertyChanged(nameof(DateCopiesToAdd));
            }
        }

        //Propriété de quantité du nombre de copies à ajouter au livre courant
        private int nbCopiesToAdd = 1;
        public int NbCopiesToAdd
        {
            get { return nbCopiesToAdd; }

            set
            {
                SetProperty<int>(ref nbCopiesToAdd, value, () => Validate());
                RaisePropertyChanged(nameof(NbCopiesToAdd));
            }
        }



        /*
         * Propriété des categories
         * 
         * 
         * retourne toutes les categories 
         * 
         * crées de nouveaux objects avec la class interne de categories pour chaque categories
         * 
         * acces en base de données pour savoir si la categorie courante est associer au livre
         * 
         * change l'atribut isChecked pour la selection dans la vue 
         * 
         */

        private ObservableCollection<CheckedCategory> categories;
        public ObservableCollection<CheckedCategory> Categories
        {

            get
            {
                if (categories == null)
                {
                    SetCategories();
                }
                return categories;
            }
        }





        /*********************************************************************************************************************************
         *
         *   METHODES 
         *
         *********************************************************************************************************************************/




        public override void Dispose()
        {
#if DEBUG_USERCONTROLS_WITH_TIMER
            timer.Stop();
#endif
            base.Dispose();
            if (imageHelper.IsTransitoryState)
            {
                imageHelper.Cancel();
                PicturePath = imageHelper.CurrentFile;
            }
        }

        //Méthode de changement d'état si une catégorie a été selectionnée
        public void Change()
        {
            CatChanged = true;
        }

        //Méthode de changement d'état si des copies ont été ajoutées
        public void CopiesAddAction()
        {
            CopiesAdded = true;
        }

        //acces base de données, retourne la liste de categories
        private void SetCategories()
        {
            categories = new ObservableCollection<CheckedCategory>(

            from c in App.Model.Categories
            orderby c.Name
            select new CheckedCategory
            {
                IdCategory = c.CategoryId,
                Name = c.Name,
                IsChecked = (from b in App.Model.Books
                             where b.BookId == Book.BookId && b.Categories.Contains(c)
                             select b).Count() > 0
            });
        }


        //remise des données de la base de données dans copies 
        //efface les données ajoutées et non sauvegardé
        private void SetCopies()
        {
            BookCopies = new ObservableCollection<BookCopy>(from c in App.Model.BookCopies
                                                            where c.Book.BookId == Book.BookId
                                                            select c);
        }


        //remise de l'image avant changement
        private void ResetPicture()
        {
            if (imageHelper.IsTransitoryState)
            {
                imageHelper.Cancel();
            }
        }

        //reset nouveau livre ou livre existant
        private void ResetBook()
        {
            if (IsNew)
            {
                ResetNewBook();
            }
            else
            {
                ResetExistingBook();
            }
        }


        //remise à zero d'un nouveau livre
        private void ResetNewBook()
        {
            if (IsNew)
            {
                Isbn = null;
                Title = null;
                Author = null;
                Editor = null;
                PicturePath = imageHelper.CurrentFile;
                RaisePropertyChanged(nameof(Book));
            }
        }

        //remise des données avant changement d'un livre
        private void ResetExistingBook()
        {
            if (!Book.IsUnchanged)
            {
                Book.Reload();
                RaisePropertyChanged(nameof(Isbn));
                RaisePropertyChanged(nameof(Title));
                RaisePropertyChanged(nameof(Author));
                RaisePropertyChanged(nameof(Editor));
                RaisePropertyChanged(nameof(PicturePath));
            }
        }

        //remise des données avant changement des categories
        private void ResetCategories()
        {
            if (CatChanged)
            {
                SetCategories();
                CatChanged = false;
                RaisePropertyChanged(nameof(Categories));
            }
        }

        //remise des données avant changement des copies
        private void ResetCopies()
        {
            var change = (from c in App.Model.ChangeTracker.Entries<BookCopy>()
                          where c.Entity.Book.BookId == Book.BookId
                          where c.State != EntityState.Unchanged
                          select c.Entity).ToList();
            App.Model.BookCopies.RemoveRange(change);

            SetCopies();
            CopiesAdded = false;
            RaisePropertyChanged(nameof(BookCopies));

        }



        /*********************************************************************************************************************************
         *
         *   METHODES D'ACTIVATION DES BOUTONS
         *
         *********************************************************************************************************************************/




        /*
         * Méthode d'activation du bouton d'ajout de copies
         * 
         * Est activé si le nombre de copies est égale ou supérieur à 0
         * et que la date acquisition ne soit pas null et inferieur à la date d'aujourd'hui 
         */
        public bool CanAddCopies()
        {
            return NbCopiesToAdd > 0 && (DateCopiesToAdd != null && DateCopiesToAdd <= DateTime.Now);
        }


        /*
         *  Méthode d'activation du bouton save ou cancel 
         * 
         * En cas de nouveau livre, tous les champs doivent être remplis et sans erreur
         * En cas de modification d'un livre existant, l'activation se fait en cas de changement d'une ou plusieurs données et qu'il n'y ait pas d'erreur
         * si une categorie a changée d'état
         * et si des copies ont été ajoutées
         */
        private bool CanSaveOrCancelAction()
        {
            if (IsNew)
            {
                return !string.IsNullOrEmpty(Isbn) && !string.IsNullOrEmpty(Title)
                    && !string.IsNullOrEmpty(Author) && !string.IsNullOrEmpty(Editor) && !HasErrors;
            }

            return (!Book.IsUnchanged || CatChanged || CopiesAdded) && !HasErrors;
        }


        /********************************************************************************************************************************
         * 
         * METHODES D'ACTION
         * 
         *********************************************************************************************************************************/



        /*
         * Méthode de sauvegarde d'un livre
         * 
         * 
         * En cas de savegarde d'un nouveau livre, on ajoute le livre dans le model (préparation à la sauvegarde)
         * 
         * En cas de selection d'une ou plusieurs catégories, parcour de toutes les catégories (ajoute ou supprime d'une catégorie si catégorie selectionée ou non)
         * 
         * Gestion de la photo du livre avec imageHelper
         * 
         */

        private void SaveAction()
        {
            if (IsNew)
            {
                App.Model.Books.Add(Book);
                IsNew = false;
            }
            if (imageHelper.IsTransitoryState)
            {
                imageHelper.Confirm(Book.Isbn);
            }

            if (CatChanged)
            {
                foreach (var c in Categories)
                {
                    if (c.IsChecked)
                    {
                        Book.AddCategory(App.Model.Categories.Find(c.IdCategory));
                    }
                    else
                    {
                        Book.RemoveCategory(App.Model.Categories.Find(c.IdCategory));
                    }
                }
                CatChanged = false;

                App.NotifyColleagues(AppMessages.MSG_CATEGORY_CHANGED);

            }
            if (CopiesAdded)
            {

                CopiesAdded = false;

            }
            PicturePath = imageHelper.CurrentFile;
            App.Model.SaveChanges();
          //  App.Model.Database.Log = null;

            App.NotifyColleagues(AppMessages.MSG_BOOK_CHANGED, Book);
            // App.NotifyColleagues(AppMessages.MSG_CANCEL_VIEWDETAIL_BOOK);
        }



        /*
         * Méthode de suppression d'un livre 
         */

        private void DeleteAction()
        {
            this.CancelAction();
            if (File.Exists(PicturePath))
            {
                //File.Delete(PicturePath);
            }
            Book.Delete();
            App.Model.SaveChanges();
            App.NotifyColleagues(AppMessages.MSG_BOOK_CHANGED, Book);
            App.NotifyColleagues(AppMessages.MSG_CATEGORY_CHANGED);
            App.NotifyColleagues(AppMessages.MSG_CANCEL_VIEWDETAIL_BOOK);
        }


        /*
         * Méthode d'annulation des modifications
         * 
         * En cas de nouveau livre, remise a null de toutes les valeurs et remise de la photo par default
         * 
         * En cas d'un livre existant, rechargement des données du livre  
         */
        private void CancelAction()
        {

            ResetPicture();
            ResetBook();
            ResetCategories();
            ResetCopies();
        }


        /*
         * Méthode de chargement d'une nouvelle image 
         */

        private void LoadImageAction()
        {
            var fd = new OpenFileDialog();
            if (fd.ShowDialog() == true)
            {
                imageHelper.Load(fd.FileName);
                PicturePath = imageHelper.CurrentFile;
            }
            RaisePropertyChanged(nameof(PicturePath));
        }


        /*
         * Méthode de clear image 
         * 
         * Remise de l'image par default si nouveau livre
         * 
         * Remise de l'image associé au livre avant modification 
         */
        private void ClearImageAction()
        {
            imageHelper.Clear();
            PicturePath = imageHelper.CurrentFile;
            RaisePropertyChanged(nameof(PicturePath));
        }

        /*
         * Méthode d'jaout de copies en local 
         */
        public void AddCopiesAction()
        {
            var copies = BookCopies;
            for (var i = 0; i < NbCopiesToAdd; ++i)
            {
                var copie = Book.CreateBookCopy(DateCopiesToAdd, Book);
                copies.Add(copie);
            }

            bookCopies = copies;
            CopiesAddAction();

        }




        /*********************************************************************************************************************************
         *
         *   VALIDATION
         *
         *********************************************************************************************************************************/

        //Méthode de validation de tous les champs de la vue

        public override bool Validate()
        {

            ClearErrors();

            string pattern = @"^[0-9]+$";
             var regex = new Regex(pattern);

            var book = App.Model.Books.Where(u => u.Isbn == Isbn && u.BookId != Book.BookId).SingleOrDefault();
            if (string.IsNullOrEmpty(Isbn))
            {
                AddError("Isbn", Properties.Resources.Error_Required);
            }
            else if (book != null)
            {
                AddError("Isbn", "Isbn existe");
            }
            else if (!regex.IsMatch(Isbn))
            {
                AddError("Isbn", "Only number");
            }
            else if (Isbn.Length != 13)
            {
                AddError("Isbn", "length must be 13 caractere");
            }           
            else if (string.IsNullOrEmpty(Title))
            {
                AddError("Title", Properties.Resources.Error_Required);
            }
            else if (string.IsNullOrEmpty(Author))
            {
                AddError("Author", Properties.Resources.Error_Required);
            }
            else if (string.IsNullOrEmpty(Editor))
            {
                AddError("Editor", Properties.Resources.Error_Required);
            }
            else if (DateCopiesToAdd == null)
            {
                AddError("DateCopiesToAdd", Properties.Resources.Error_Required);
            }
            else if (DateCopiesToAdd > DateTime.Now)
            {
                AddError("DateCopiesToAdd", Properties.Resources.Max_Date);
            }
            else if (NbCopiesToAdd < 1)
            {
                AddError("NbCopiesToAdd", Properties.Resources.Min_Copies);
            }

            RaiseErrors();
            return !HasErrors;
        }
    }
}

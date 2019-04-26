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


namespace prbd_1819_g07
{
    /// <summary>
    /// Logique d'interaction pour BookDetailsView.xaml
    /// </summary>
    public partial class BookDetailsView : UserControlBase
    {

        public class CheckedCategory
        {
            public int IdCategory { get; set; }
            public string Name { get; set; }
            public bool IsChecked { get; set; }
        }



        public Book Book { get; set; }
        private ImageHelper imageHelper;


        public ICommand Add { get; set; }
        public ICommand Save { get; set; }
        public ICommand Cancel { get; set; }
        public ICommand Delete { get; set; }
        public ICommand LoadImage { get; set; }
        public ICommand ClearImage { get; set; }
        public ICommand Exit { get; set; }


        public BookDetailsView(Book book, bool isNew)
        {
            InitializeComponent();

            if (DesignerProperties.GetIsInDesignMode(this))
                return;

            DataContext = this;
            var model = Model.CreateModel(DbType.MsSQL);

            Book = book;
            IsNew = isNew;

            BookCopies = new ObservableCollection<BookCopy>(model.BookCopies);
            imageHelper = new ImageHelper(App.IMAGE_PATH, Book.PicturePath);

            Save = new RelayCommand(SaveAction, CanSaveOrCancelAction);
            Cancel = new RelayCommand(CancelAction, CanSaveOrCancelAction);
            Delete = new RelayCommand(DeleteAction, () => !IsNew);
            LoadImage = new RelayCommand(LoadImageAction);
            ClearImage = new RelayCommand(ClearImageAction, () => PicturePath != null);

            Exit = new RelayCommand(() =>
            {

                App.NotifyColleagues(AppMessages.MSG_CANCEL_VIEWDETAIL_BOOK);

            });

        }


        private ObservableCollection<BookCopy> bookCopies;

        public ObservableCollection<BookCopy> BookCopies
        {

            get => bookCopies;

            set => SetProperty<ObservableCollection<BookCopy>>(ref bookCopies, value);
        }

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

        public bool IsExisting { get => !isNew; }

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

        public string PicturePath
        {
            get { return Book.AbsolutePicturePath; }
            set
            {
                Book.PicturePath = value;
                RaisePropertyChanged(nameof(PicturePath));
            }
        }

        private ObservableCollection<CheckedCategory> categories;

        public ObservableCollection<CheckedCategory> Categories
        {

            get
            {
                if (categories == null)
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
                return categories;
            }
        }
    

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
            App.NotifyColleagues(AppMessages.MSG_CANCEL_VIEWDETAIL_BOOK);
        }

        private void LoadImageAction()
        {
            var fd = new OpenFileDialog();
            if (fd.ShowDialog() == true)
            {
                imageHelper.Load(fd.FileName);
                PicturePath = imageHelper.CurrentFile;
            }
        }

        private void ClearImageAction()
        {
            imageHelper.Clear();
            PicturePath = imageHelper.CurrentFile;
        }

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

            PicturePath = imageHelper.CurrentFile;
            App.Model.SaveChanges();
            App.NotifyColleagues(AppMessages.MSG_CANCEL_VIEWDETAIL_BOOK);
            App.NotifyColleagues(AppMessages.MSG_BOOK_CHANGED, Book);
        }

        private void CancelAction()
        {
            if (imageHelper.IsTransitoryState)
            {
                imageHelper.Cancel();
            }
            if (IsNew)
            {

                Isbn = null;
                Title = null;
                Author = null;
                Editor = null;
                PicturePath = imageHelper.CurrentFile;
                RaisePropertyChanged(nameof(Book));
            }
            else
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
        }

        private bool CanSaveOrCancelAction()
        {
            if (IsNew)
            {
                return !string.IsNullOrEmpty(Isbn) && !string.IsNullOrEmpty(Title)
                    && !string.IsNullOrEmpty(Author) && !string.IsNullOrEmpty(Editor) && !HasErrors;
            }

            var change = (from c in App.Model.ChangeTracker.Entries<Book>()
                          where c.Entity == Book
                          select c).FirstOrDefault();
            return change != null && change.State != EntityState.Unchanged;
        }

        public override bool Validate()
        {
            ClearErrors();
            var book = App.Model.Books.Where(u => u.Isbn == Isbn).SingleOrDefault();
            if (string.IsNullOrEmpty(Isbn))
            {
                AddError("Isbn", Properties.Resources.Error_Required);
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


            RaiseErrors();
            return !HasErrors;
        }
    }
}

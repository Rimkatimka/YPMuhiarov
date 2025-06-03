using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Contexts;
using System.Windows;
using System.Windows.Input;
using YPMuhiarov.MVVM.Model;
using YPMuhiarov.MVVM.ViewModel;

namespace YPMuhiarov.MVVM.ViewModel
{
    public class BooksVM : INotifyPropertyChanged
    {
        YPMuhiarovEntities db = new();
        private BooksModel _selectedBook;
        private Authors _selectedAuthor;
        private string _sortProperty;
        private bool _isSortAscending = true;

        private ObservableCollection<BooksModel> _listBooks = new();
        private ObservableCollection<Authors> _listAuthors = new();

        public ObservableCollection<BooksModel> ListBooks
        {
            get => _listBooks;
            set
            {
                _listBooks = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Authors> ListAuthors
        {
            get => _listAuthors;
            set
            {
                _listAuthors = value;
                OnPropertyChanged();
            }
        }

        public ICommand AddBookCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand DeleteBookCommand { get; }
        public ICommand AddAuthorCommand { get; }
        public ICommand RemoveAuthorCommand { get; }

        public BooksVM()
        {
            UpdateBooks();

            SaveCommand = new RelayObjCommand(SaveChanges, param => HasChanges());
            AddBookCommand = new RelayObjCommand(AddBook, _ => CanAddBook());
            DeleteBookCommand = new RelayObjCommand(DeleteBook);
            AddAuthorCommand = new RelayCommand(AddAuthor, () => _selectedBook != null && _selectedAuthor != null);
            RemoveAuthorCommand = new RelayCommand<BookAuthors>(RemoveAuthor, ba => _selectedBook != null && ba != null);
        }

        public void UpdateBooks()
        {
            ListBooks.Clear();
            ListAuthors.Clear();

            try
            {
                db.Books
                    .Include(b => b.BookAuthors.Select(ba => ba.Authors))
                    .Load();

                foreach (var book in db.Books.Local)
                {
                    ListBooks.Add(new BooksModel(book));
                }
                System.Diagnostics.Debug.WriteLine($"Loaded {ListBooks.Count} books");

                foreach (var author in db.Authors.ToList())
                {
                    ListAuthors.Add(author);
                }
                System.Diagnostics.Debug.WriteLine($"Loaded {ListAuthors.Count} authors");

                OnPropertyChanged(nameof(ListBooks));
                OnPropertyChanged(nameof(ListAuthors));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"UpdateBooks error: {ex.Message}");
            }
        }

        public bool HasChanges()
        {
            return db.ChangeTracker.Entries()
                .Any(e => e.State != EntityState.Unchanged);
        }

        private void SaveChanges(object parameter)
        {
            try
            {
                if (HasChanges())
                {
                    db.SaveChanges();
                    MessageBox.Show("Изменения сохранены!", "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool CanAddBook()
        {
            return !string.IsNullOrEmpty(_selectedBook?.Title) &&
                   !string.IsNullOrEmpty(_selectedBook?.GenreName) &&
                   !string.IsNullOrEmpty(_selectedBook?.ConditionName) &&
                   !string.IsNullOrEmpty(_selectedBook?.StatusName) &&
                   _selectedAuthor != null &&
                   _selectedBook.WearPercentage >= 0 && _selectedBook.WearPercentage <= 100;
        }

        private void AddBook(object parameter)
        {
            try
            {
                var book = new Books
                {
                    BookId = db.Books.Any() ? db.Books.Max(b => b.BookId) + 1 : 1,
                    ISBN = Guid.NewGuid().ToString().Substring(0, 13),
                    Title = _selectedBook?.Title ?? "Без названия",
                    GenreName = _selectedBook?.GenreName ?? "Не указан",
                    PublicationYear = _selectedBook?.PublicationYear ?? 2025,
                    Annotation = _selectedBook?.Annotation ?? "Нет аннотации",
                    Language = _selectedBook?.Language ?? "Русский",
                    Location = _selectedBook?.Location ?? "Нефтекамск",
                    BranchId = 1,
                    ConditionName = _selectedBook?.ConditionName ?? "Не указано",
                    StatusName = _selectedBook?.StatusName ?? "Не указан",
                    WearPercentage = _selectedBook?.WearPercentage ?? 0,
                    BookAuthors = new List<BookAuthors>()
                };

                if (_selectedAuthor != null)
                {
                    book.BookAuthors.Add(new BookAuthors
                    {
                        BookAuthorID = db.BookAuthors.Any() ? db.BookAuthors.Max(ba => ba.BookAuthorID) + 1 : 1,
                        BookId = book.BookId,
                        AuthorId = _selectedAuthor.AuthorId
                    });
                }

                db.Books.Add(book);
                var wrapper = new BooksModel(book);
                ListBooks.Add(wrapper);
                SelectedBook = wrapper;

                MessageBox.Show("Книга добавлена");

                OnPropertyChanged(nameof(ListBooks));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"AddBook error: {ex.Message}");
            }
        }

        private void DeleteBook(object parameter)
        {
            if (SelectedBook == null) return;

            if (MessageBox.Show($"Удалить {SelectedBook.Title}?",
                "Подтверждение",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question) != MessageBoxResult.Yes) return;

            try
            {
                var model = SelectedBook.GetModel();
                db.Books.Remove(model);
                ListBooks.Remove(SelectedBook);

                db.SaveChanges();

                SelectedBook = ListBooks.FirstOrDefault();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка удаления: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddAuthor()
        {
            try
            {
                if (_selectedBook != null && _selectedAuthor != null)
                {
                    var book = _selectedBook.GetModel();
                    var bookAuthor = new BookAuthors
                    {
                        BookAuthorID = db.BookAuthors.Any() ? db.BookAuthors.Max(b => b.BookAuthorID) + 1 : 1,
                        BookId = book.BookId,
                        AuthorId = _selectedAuthor.AuthorId
                    };

                    db.BookAuthors.Add(bookAuthor);
                    book.BookAuthors.Add(bookAuthor);
                    OnPropertyChanged(nameof(SelectedBook));
                    MessageBox.Show("Автор добавлен");
                    UpdateBooks();
                    SelectedBook = new BooksModel(book);
                    OnPropertyChanged(nameof(ListAuthors));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"AddAuthor error: {ex.Message}");
            }
        }

        private void RemoveAuthor(BookAuthors bookAuthor)
        {
            try
            {
                if (_selectedBook != null && bookAuthor != null)
                {
                    var book = _selectedBook.GetModel();
                    var authorToRemove = book.BookAuthors.FirstOrDefault(ba => ba.AuthorId == bookAuthor.AuthorId);
                    if (authorToRemove != null)
                    {
                        db.BookAuthors.Remove(authorToRemove);
                        book.BookAuthors.Remove(authorToRemove);
                        OnPropertyChanged(nameof(SelectedBook));
                        MessageBox.Show("Автор удалён");
                        UpdateBooks();
                        SelectedBook = new BooksModel(book);
                        OnPropertyChanged(nameof(ListAuthors));

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"RemoveAuthor error: {ex.Message}");
            }
        }

        private void SortBooks()
        {
            if (string.IsNullOrEmpty(_sortProperty))
                return;

            var sortedBooks = _sortProperty switch
            {
                "System.Windows.Controls.ComboBoxItem: Название" => _isSortAscending
                    ? ListBooks.OrderBy(b => b.Title).ToList()
                    : ListBooks.OrderByDescending(b => b.Title).ToList(),
                "System.Windows.Controls.ComboBoxItem: Жанр" => _isSortAscending
                    ? ListBooks.OrderBy(b => b.GenreName).ToList()
                    : ListBooks.OrderByDescending(b => b.GenreName).ToList(),
                "System.Windows.Controls.ComboBoxItem: Год выпуска" => _isSortAscending
                    ? ListBooks.OrderBy(b => b.PublicationYear).ToList()
                    : ListBooks.OrderByDescending(b => b.PublicationYear).ToList(),
                "System.Windows.Controls.ComboBoxItem: Состояние" => _isSortAscending
                    ? ListBooks.OrderBy(b => b.ConditionName).ToList()
                    : ListBooks.OrderByDescending(b => b.ConditionName).ToList(),
                "System.Windows.Controls.ComboBoxItem: Статус" => _isSortAscending
                    ? ListBooks.OrderBy(b => b.StatusName).ToList()
                    : ListBooks.OrderByDescending(b => b.StatusName).ToList(),
                "System.Windows.Controls.ComboBoxItem: Износ" => _isSortAscending
                    ? ListBooks.OrderBy(b => b.WearPercentage).ToList()
                    : ListBooks.OrderByDescending(b => b.WearPercentage).ToList(),
                _ => ListBooks.ToList()
            };

            for (int i = 0; i < sortedBooks.Count; i++)
            {
                ListBooks[i] = sortedBooks[i];
            }
            OnPropertyChanged(nameof(ListBooks));
        }

        public BooksModel SelectedBook
        {
            get => _selectedBook;
            set
            {
                if (_selectedBook != value)
                {
                    _selectedBook = value;
                    SelectedAuthor = null;
                    OnPropertyChanged();
                    OnPropertyChanged("ReplacementRequired");
                }
            }
        }

        public Authors SelectedAuthor
        {
            get => _selectedAuthor;
            set
            {
                _selectedAuthor = value;
                OnPropertyChanged();
            }
        }

        public string SortProperty
        {
            get => _sortProperty;
            set
            {
                _sortProperty = value;
                SortBooks();
                OnPropertyChanged();
            }
        }

        public bool IsSortAscending
        {
            get => _isSortAscending;
            set
            {
                _isSortAscending = value;
                SortBooks();
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsSortDescending));
            }
        }

        public bool IsSortDescending
        {
            get => !_isSortAscending;
            set
            {
                if (IsSortDescending != value)
                {
                    IsSortAscending = !value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
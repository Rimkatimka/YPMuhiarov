using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace YPMuhiarov.MVVM.Model
{
    public class BooksModel : INotifyPropertyChanged
    {
        private readonly Books _book;

        public BooksModel(Books book)
        {
            _book = book ?? throw new ArgumentNullException(nameof(book));
            _book.BookAuthors ??= new List<BookAuthors>();
        }

        public int BookId
        {
            get => _book.BookId;
            set
            {
                _book.BookId = value;
                OnPropertyChanged();
            }
        }

        public string ISBN
        {
            get => _book.ISBN;
            set
            {
                _book.ISBN = value;
                OnPropertyChanged();
            }
        }

        public string Title
        {
            get => _book.Title;
            set
            {
                _book.Title = value;
                OnPropertyChanged();
            }
        }

        public string GenreName
        {
            get => _book.GenreName;
            set
            {
                _book.GenreName = value;
                OnPropertyChanged();
            }
        }

        public int? PublicationYear
        {
            get => _book.PublicationYear;
            set
            {
                _book.PublicationYear = value;
                OnPropertyChanged();
            }
        }

        public string ConditionName
        {
            get => _book.ConditionName;
            set
            {
                _book.ConditionName = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ReplacementRequired));
            }
        }

        public string StatusName
        {
            get => _book.StatusName;
            set
            {
                _book.StatusName = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ReplacementRequired));
            }
        }

        public decimal WearPercentage
        {
            get => _book.WearPercentage;
            set
            {
                _book.WearPercentage = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ReplacementRequired));
            }
        }

        public string Publisher
        {
            get => _book.Publisher;
            set
            {
                _book.Publisher = value;
                OnPropertyChanged();
            }
        }

        public string Annotation
        {
            get => _book.Annotation;
            set
            {
                _book.Annotation = value;
                OnPropertyChanged();
            }
        }

        public string Language
        {
            get => _book.Language;
            set
            {
                _book.Language = value;
                OnPropertyChanged();
            }
        }

        public string Location
        {
            get => _book.Location;
            set
            {
                _book.Location = value;
                OnPropertyChanged();
            }
        }

        public int BranchId
        {
            get => _book.BranchId;
            set
            {
                _book.BranchId = value;
                OnPropertyChanged();
            }
        }

        public int? PageCount
        {
            get => _book.PageCount;
            set
            {
                _book.PageCount = value;
                OnPropertyChanged();
            }
        }

        public DateTime? AcquisitionDate
        {
            get => _book.AcquisitionDate;
            set
            {
                _book.AcquisitionDate = value;
                OnPropertyChanged();
            }
        }

        public decimal? Cost
        {
            get => _book.Cost;
            set
            {
                _book.Cost = value;
                OnPropertyChanged();
            }
        }

        public virtual ICollection<BookAuthors> BookAuthors
        {
            get => _book.BookAuthors;
            set
            {
                _book.BookAuthors = value;
                OnPropertyChanged();
            }
        }

        public string ReplacementRequired
        {
            get
            {
                if (_book.StatusName == "Утеряна")
                    return "Замена";
                if (_book.ConditionName == "Отличное")
                    return "Нет";
                if (_book.ConditionName == "Хорошее" && WearPercentage < 60)
                    return "Нет";
                if (_book.ConditionName == "Хорошее" && WearPercentage >= 60)
                    return "Восстановить";
                if (_book.ConditionName == "Удовлетворительное")
                    return "Восстановить";
                if (_book.ConditionName == "Плохое" || WearPercentage >= 80)
                    return "Замена";
                return "Не определено";
            }
        }

        public Books GetModel() => _book;

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

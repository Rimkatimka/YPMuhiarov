using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace YPMuhiarov.MVVM.Model
{
    public class AuthorModel : INotifyPropertyChanged
    {
        private readonly Authors _author;

        public AuthorModel(Authors author)
        {
            _author = author ?? throw new ArgumentNullException(nameof(author));
            _author.BookAuthors ??= new List<BookAuthors>();
        }
        public string FIO
        {
            get => _author.FIO;
            set
            {
                _author.FIO = value;
                OnPropertyChanged();
            }
        }
        public Nullable<System.DateTime> BirthDate
        {
            get => _author.BirthDate;
            set
            {
                _author.BirthDate = value;
                OnPropertyChanged();
            }
        }
        public Nullable<System.DateTime> DeathDate
        {
            get => _author.DeathDate;
            set
            {
                _author.DeathDate = value;
                OnPropertyChanged();
            }
        }
        public string Country
        {
            get => _author.Country;
            set
            {
                _author.Country = value;
                OnPropertyChanged();
            }
        }
        public string MainGenres
        {
            get => _author.MainGenres;
            set
            {
                _author.MainGenres = value;
                OnPropertyChanged();
            }
        }

        public virtual ICollection<BookAuthors> BookAuthors
        {
            get => _author.BookAuthors;
            set
            {
                _author.BookAuthors = value;
                OnPropertyChanged();
            }
        }

        public Authors GetModel() => _author;

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    
}

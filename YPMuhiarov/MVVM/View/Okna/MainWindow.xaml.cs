using System.Windows;
using YPMuhiarov.MVVM.ViewModel;

namespace YPMuhiarov
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new BooksVM();
        }
    }
}

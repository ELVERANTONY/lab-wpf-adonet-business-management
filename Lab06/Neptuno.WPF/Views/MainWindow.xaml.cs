using System.Windows;
using Neptuno.WPF.ViewModels;

namespace Neptuno.WPF.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainViewModel();
    }
}

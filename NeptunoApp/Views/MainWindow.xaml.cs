using System.Windows;
using NeptunoApp.ViewModels;

namespace NeptunoApp.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainViewModel();
    }
}

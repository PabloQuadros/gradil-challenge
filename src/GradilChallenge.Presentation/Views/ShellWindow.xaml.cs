using System.Windows;
using GradilChallenge.Presentation.ViewModels;


namespace GradilChallenge.Presentation.Views;

public partial class ShellWindow : Window
{
    public ShellWindow(ShellViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
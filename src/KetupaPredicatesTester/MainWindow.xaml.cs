namespace KetupaPredicatesTester
{
    using System.Windows;

    using Trogon.KetupaPredicates.Tester.Logic;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Logic and data for the UI.
        /// </summary>
        public MainViewModel ViewModel { get; protected set; }

        /// <summary>
        /// Initialize ViewModel instance and bind it to the view initialization.
        /// </summary>
        public MainWindow()
        {
            ViewModel = new MainViewModel();
            DataContext = ViewModel;
            InitializeComponent();
        }
    }
}
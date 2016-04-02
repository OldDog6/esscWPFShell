using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Data;
using System.Data.SqlClient;
using esscWPFShell;
using TestHarness.ViewModels;
using TestHarness.Dialogs;
using System.Deployment.Application;

namespace TestHarness
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private ApplicationViewModel _appViewModel;

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            _appViewModel = new ApplicationViewModel();

            if (ApplicationDeployment.IsNetworkDeployed)
            {
                _appViewModel.CurrentDataSource = "Production";
                _appViewModel.DeployMode = "Deployed Mode";
                _appViewModel.CurrentVersion = ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString(4);
            }
            else
            {
                _appViewModel.CurrentDataSource = "Production";
                _appViewModel.DeployMode = "Development Mode";
                _appViewModel.CurrentVersion = "0";
            }

            #if DEBUG
                
            #else
                _connection = new Connectors("Data Source=Accounting;Initial Catalog=IIR;Persist Security  Info=True;Trusted_Connection=True","Production");
            #endif


            if (_appViewModel != null)
            {
                MainWindowViewModel mainWinViewModel = new MainWindowViewModel(_appViewModel);
                _appViewModel.MainWindow = mainWinViewModel;

                // Creation and binding of the Mainwindows bits...
                ApplicationHeaderViewModel appHeadervm = new ApplicationHeaderViewModel(ApplicationHeader.Create(Strings.HIVince,"A test of esscWPFShell"));
                mainWinViewModel.AppHeader = appHeadervm;
                StatusBarViewModel statusBarvm = new StatusBarViewModel(_appViewModel);
                mainWinViewModel.AppStatusBar = statusBarvm;

                InjectCommands();

                // Create the mainwindow itself
                MainWindow mainWin = new MainWindow();
                mainWin.DataContext = mainWinViewModel;
                
                mainWin.Show();

                // Confirmation test...

                ConfirmationViewModel vm = new ConfirmationViewModel("Friendly", "Very Long");

                var newwin = new ConfirmationDlg(vm);
                newwin.DataContext = vm;
                vm.ParentWindow = newwin;

                newwin.ShowDialog();

            }
        }

        #region RoleBasedCommands

        private void InjectCommands()
        {
            // ez security, no commands to do anything is not authentic
            
            // two roles, can say hello and can only exit
            _appViewModel.MainWindow.Model.InjectCommandViewModel(new CommandViewModel("Exit",new RelayCommand(param => this.ExitCommand())));
            _appViewModel.MainWindow.Model.InjectCommandViewModel(new CommandViewModel("New Workspace", new RelayCommand(param => this.OpenNewWorkspace())));
            
        }

        #endregion

        #region Application Level Commands

        private void ExitCommand()
        {
            this.Shutdown();
        }

        private void OpenNewWorkspace()
        {
            // To create a new workspace and call, must have a valid MainWindowViewModel 
            if (_appViewModel.MainWindow != null)
            {
                HelloWorldViewModel vm = new HelloWorldViewModel();
                // vm.DisplayName = "Hello World";
                vm.Hello = "Good Morning";
                
                _appViewModel.MainWindow.InjectWorkSpace(vm);
            }
        }

 
        #endregion
    }
}

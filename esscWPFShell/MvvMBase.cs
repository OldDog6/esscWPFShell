using System;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Data;
using System.Data;
using System.Data.SqlClient;
using System.Security.Principal;
using System.Linq;
using esscWPFShell.Supporting;
using System.Deployment.Application;

//--------------------------------------------------------------------------------------------------
//- esscWPFShell                                                                                   -
//- Supporting classes for an MvvM pattern Mainwindow Shell.   To be used as a starting element    -
//- for essc standard WPF Windows applications.                                                     -
//- Paul Osnes, June 2012                                                                          -
//--------------------------------------------------------------------------------------------------  

namespace esscWPFShell
{

#region -------------------------------- Shell Viewmodel Classes -------------------------------------

   
    /// <summary>
    /// Model of the application... Dependancy Injection for DBMSError, SQL COmmand for error log table 
    /// </summary>
    public class ApplicationViewModel
    {
        #region Private Properties

        private ApplicationModel _applicationmodel;
        private MainWindowViewModel _mainwindow;
        private SqlCommand _posterrorcommand;
        private String _version;
        private String _datasource;
        private String _deplymode;

        #endregion

        #region Constructor

        public ApplicationViewModel()
        {
            _applicationmodel = ApplicationModel.Create(this);
        }

        #endregion

        #region Public Properties

        public SqlCommand DBMSErrorPostCommand
        {
            get { return _posterrorcommand; }
            set
            {
                if (value != null)
                {
                    _posterrorcommand = value;
                }
                else
                {
                    // create without a sqlcommand... will disable post button
                }
            }
        }

        public String UserName
        {
            get { return _applicationmodel.User.Name; }
        }

        public String FriendlyUserName
        {
            get 
            {   
                string fname =  _applicationmodel.User.Name;

                return fname.Substring(fname.LastIndexOf('\\') + 1);
            }

        }

        public String CurrentVersion { get; set; }

        public String CurrentDataSource { get; set; }

        public String DeployMode { get; set; }

        public MainWindowViewModel MainWindow
        {
            get { return _mainwindow; }
            set
            {
                if (value != null)
                    _mainwindow = value;
            }
        }


        #endregion

        #region Public Methods

        public void ShowDialog(Window dlg)
        {
            dlg.Show();
        }

        public void CloseDialog(Window dlg)
        {
            dlg.Close();
        }
 
        /// <summary>
        /// If the user is not valid, then we stop the application here...
        /// 
        /// </summary>
        public void StartUp()
        {
            
        }

        #endregion

        #region Delegates

        public delegate void ExitHandler();
        public ExitHandler ApplicationExit;

        public delegate void DialogCancel();
        public DialogCancel DialogCanceled;

        #endregion
 
    }

    /// <summary>
    /// Model of the MainWindow
    /// </summary>
    public class MainWindowViewModel : WorkspaceViewModel
    {
        #region Private Properties

        private ApplicationViewModel _parent;
        private MainWindowModel _mainwin;
        private StatusBarViewModel _appstatusbar;
        private ApplicationHeaderViewModel _appheader;
        private ObservableCollection<WorkspaceViewModel> _workspaces;

        #endregion

        #region Constructor

        public MainWindowViewModel(ApplicationViewModel parent)
        {
            _parent = parent;
            if (parent != null)
            {
                _mainwin = MainWindowModel.Create(parent);
            }
        }

        #endregion

        #region Public Properties

        public ApplicationHeaderViewModel AppHeader
        {
            get { return _appheader; }
            set
            {
                if (_appheader == value)
                    return;

                _appheader = value;
                OnPropertyChanged("AppHeader");
            }
        }

        public StatusBarViewModel AppStatusBar
        {
            get { return _appstatusbar; }
            set
            {
                if (value == _appstatusbar)
                    return;

                _appstatusbar = value;
                OnPropertyChanged("AppStatusBar");
            }
        }

        public ReadOnlyCollection<CommandViewModel> Commands
        {
            get
            {
                if (_mainwin.AppCommands != null)
                {
                    return new ReadOnlyCollection<CommandViewModel>(_mainwin.AppCommands);
                }
                // no commands, return as an empty list...
                return new ReadOnlyCollection<CommandViewModel>(new List<CommandViewModel>());
            }
        }

        public MainWindowModel Model
        {
            get { return _mainwin; }
        }

        #endregion

        #region Workspaces

        public ObservableCollection<WorkspaceViewModel> Workspaces
        {
            get
            {
                if (_workspaces == null)
                {
                    _workspaces = new ObservableCollection<WorkspaceViewModel>();
                    _workspaces.CollectionChanged += this.OnWorkspacesChanged;
                }
                return _workspaces;
            }
        }

        public void InjectWorkSpace(WorkspaceViewModel vm)
        {
            this.Workspaces.Add(vm);
            this.SetActiveWorkspace(vm);
        }

        private void OnWorkspacesChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null && e.NewItems.Count != 0)
                foreach (WorkspaceViewModel workspace in e.NewItems)
                    workspace.RequestClose += this.OnWorkspaceRequestClose;

            if (e.OldItems != null && e.OldItems.Count != 0)
                foreach (WorkspaceViewModel workspace in e.OldItems)
                    workspace.RequestClose -= this.OnWorkspaceRequestClose;
        }

        private void OnWorkspaceRequestClose(object sender, EventArgs e)
        {
            WorkspaceViewModel workspace = sender as WorkspaceViewModel;
            workspace.Dispose();
            this.Workspaces.Remove(workspace);
        }

        private void SetActiveWorkspace(WorkspaceViewModel workspace)
        {
            Debug.Assert(this.Workspaces.Contains(workspace));

            ICollectionView collectionView = CollectionViewSource.GetDefaultView(this.Workspaces);
            if (collectionView != null)
                collectionView.MoveCurrentTo(workspace);
        }


        #endregion // Workspaces

    }

    /// <summary>
    /// Status Bar view model has places for messages, application status stuff
    /// </summary>
    public class StatusBarViewModel : ViewModelBase
    {
        #region Private Properties

        private ApplicationViewModel _parent;
        private StatusBarModel _statusbarmodel;

        #endregion

        #region Constructor

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parent"></param>
        public StatusBarViewModel(ApplicationViewModel parent)
        {
            if (parent != null)
            {
                _parent = parent;
                String IntroMessage = String.Format("Good {0} {1}, I am in {2} using {3} data as version {4}.",
                                                        Supporting.myConversions.AsGoodMorning(DateTime.Now),
                                                        _parent.FriendlyUserName,
                                                        parent.DeployMode,
                                                        parent.CurrentDataSource,
                                                        parent.CurrentVersion);
                _statusbarmodel = StatusBarModel.Create(parent.FriendlyUserName, "Production", IntroMessage, false);
            }
        }
        #endregion

        #region Public Properties
        public String UserName
        {
            get { return _statusbarmodel.UserName; }
        }

        public String DisplayMessage
        {
            get { return _statusbarmodel.Message; }
            set
            {
                if (value == _statusbarmodel.Message)
                    return;

                _statusbarmodel.Message = value;
                OnPropertyChanged("DisplayMessage");
            }
        }

        #endregion
    }

    /// <summary>
    /// A Bannor with the application title and subtitle
    /// </summary>
    public class ApplicationHeaderViewModel : ViewModelBase
    {
        #region private properties

        private ApplicationHeader _applicationheader;

        #endregion

        #region Constructor

        public ApplicationHeaderViewModel(ApplicationHeader header)
        {
            _applicationheader = header;

        }

        #endregion

        #region Public Properties

        public String ApplicationTitle
        {
            get { return _applicationheader.ApplicationTitle; }
            set
            {
                if (_applicationheader.ApplicationTitle == value)
                    return;

                _applicationheader.ApplicationTitle = value;
                OnPropertyChanged("ApplicationTitle");
            }
        }

        public String ApplicationSubTitle
        {
            get { return _applicationheader.ApplicationSubTitle; }
            set
            {
                if (_applicationheader.ApplicationSubTitle == value)
                    return;

                _applicationheader.ApplicationSubTitle = value;
                OnPropertyChanged("ApplicationSubTitle");
            }

        }

        #endregion
    }

    /// <summary>
    /// A Viewmodel suitable for error dialog boxes with option to post errors to DBMS
    /// DBMS Error nneds constructor injection of SQL Command
    /// </summary>
    public class DBMSErrorViewModel
    {
        #region Private Properties

        // DBMSError _error;
        RelayCommand _posttolog;

        #endregion

        #region Constructor

        public DBMSErrorViewModel() //DBMSError error)
        {
//            _error = error;
            _posttolog = new RelayCommand(param => this.LogErrorEvent());
        }

        #endregion

        #region Public Properties

        public bool FailStatus
        {
            get { return true; }
 //           set { _error.FailStatus = value; }
        }

        public String FriendlyMessage
        {
            get { return "No Message Defined"; }
        }

        public String ExceptionMessage
        {
            get { return "No Meesages Defined"; }
        }

        public RelayCommand PostLogFailure
        {
            get { return _posttolog; }
        }

        #endregion

        #region Commands

        void LogErrorEvent()
        {
            // DBMSError.Repository.LogFailure(_error);
        }

        #endregion
    }

    public class ConfirmationViewModel : ViewModelBase
    {
        public ConfirmationViewModel(String friendlymessage, String detailmessage)
        {
            _result = 0;
            _friendlyMessage = friendlymessage;
            _detailmessage = detailmessage;

            yescommand = new RelayCommand(param => this.ExecuteYesCommand());
            _no = new RelayCommand(param => this.ExecuteNoCommand());
            _cancel = new RelayCommand(param => this.ExecuteCancel());
        }

        #region private properties

        int _result;

        String _friendlyMessage;
        String _detailmessage;

        RelayCommand yescommand;
        RelayCommand _no;
        RelayCommand _cancel;

        #endregion

        #region private methods

        private void closeDialog()
        {
            try
            {
                (ParentWindow as Window).Close();
            }
            finally { }
        }

        private void ExecuteYesCommand()
        {
            _result = 1;
            closeDialog();
        }
        private void ExecuteNoCommand()
        {
            _result = 2;
            closeDialog();
        }
        private void ExecuteCancel()
        {
            _result = 0;
            closeDialog();
        }

        #endregion

        #region public properties

        public Object ParentWindow { get; set; }
 
        public String FriendlyMessage
        {
            get { return _friendlyMessage; }
        }
        public String DetailMessage
        {
            get { return _detailmessage; }
        }
        public int Result
        {
            get { return _result; }
        }

        public RelayCommand YesCommand
        {
            get { return yescommand; }
        }
        public RelayCommand NoCommand
        {
            get { return _no; }
        }
        public RelayCommand CancelCommand
        {
            get { return _cancel; }
        }

        #endregion
    }


#endregion

#region ------------------------------- Base ViewModel Classes -----------------------------------
    /// <summary>
    /// Base class for all ViewModel classes.
    /// It provides support for property change notifications 
    /// and has a DisplayName property. Is Abstract
    /// </summary>
    public abstract class ViewModelBase : INotifyPropertyChanged, IDisposable
    {
        #region Constructor

        protected ViewModelBase()
        {
        }

        #endregion // Constructor

        #region DisplayName

        /// <summary>
        /// Returns the user-friendly name of this object.
        /// Child classes can set this property to a new value,
        /// or override it to determine the value on-demand.
        /// </summary>
        public virtual string DisplayName { get; protected set; }

        #endregion // DisplayName

        #region Debugging Aides

        /// <summary>
        /// Warns the developer if this object does not have
        /// a public property with the specified name. This 
        /// method does not exist in a Release build.
        /// </summary>
        [Conditional("DEBUG")]
        [DebuggerStepThrough]
        public void VerifyPropertyName(string propertyName)
        {
            // Verify that the property name matches a real,  
            // public, instance property on this object.
            if (TypeDescriptor.GetProperties(this)[propertyName] == null)
            {
                string msg = "Invalid property name: " + propertyName;

                if (this.ThrowOnInvalidPropertyName)
                    throw new Exception(msg);
                else
                    Debug.Fail(msg);
            }
        }

        /// <summary>
        /// Returns whether an exception is thrown, or if a Debug.Fail() is used
        /// when an invalid property name is passed to the VerifyPropertyName method.
        /// The default value is false, but subclasses used by unit tests might 
        /// override this property's getter to return true.
        /// </summary>
        protected virtual bool ThrowOnInvalidPropertyName { get; private set; }

        #endregion // Debugging Aides

        #region INotifyPropertyChanged Members

        /// <summary>
        /// Raised when a property on this object has a new value.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises this object's PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">The property that has a new value.</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            this.VerifyPropertyName(propertyName);

            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                var e = new PropertyChangedEventArgs(propertyName);
                handler(this, e);
            }
        }

        #endregion // INotifyPropertyChanged Members

        #region IDisposable Members

        /// <summary>
        /// Invoked when this object is being removed from the application
        /// and will be subject to garbage collection.
        /// </summary>
        public void Dispose()
        {
            this.OnDispose();
        }

        /// <summary>
        /// Child classes can override this method to perform 
        /// clean-up logic, such as removing event handlers.
        /// </summary>
        protected virtual void OnDispose()
        {
        }

#if DEBUG
        /// <summary>
        /// Useful for ensuring that ViewModel objects are properly garbage collected.
        /// </summary>
        ~ViewModelBase()
        {
            string msg = string.Format("{0} ({1}) ({2}) Finalized", this.GetType().Name, this.DisplayName, this.GetHashCode());
            System.Diagnostics.Debug.WriteLine(msg);
        }
#endif

        #endregion // IDisposable Members

    }

    /// <summary>
    /// Base class to encapsulate bound commands
    /// </summary>
    public class RelayCommand : ICommand
    {
        #region Fields

        readonly Action<object> _execute;
        readonly Predicate<object> _canExecute;

        #endregion // Fields

        #region Constructors

        /// <summary>
        /// Creates a new command that can always execute.
        /// </summary>
        /// <param name="execute">The execution logic.</param>
        public RelayCommand(Action<object> execute)
            : this(execute, null)
        {
        }

        /// <summary>
        /// Creates a new command.
        /// </summary>
        /// <param name="execute">The execution logic.</param>
        /// <param name="canExecute">The execution status logic.</param>
        public RelayCommand(Action<object> execute, Predicate<object> canExecute)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");

            _execute = execute;
            _canExecute = canExecute;
        }

        #endregion // Constructors

        #region ICommand Members

        [DebuggerStepThrough]
        public bool CanExecute(object parameter)
        {
            return _canExecute == null ? true : _canExecute(parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public void Execute(object parameter)
        {
            _execute(parameter);
        }

        #endregion // ICommand Members
    }
    
    /// <summary>
    /// Used to populate the main CommandViewModel mainscreen element
    /// </summary>
    public class CommandViewModel : ViewModelBase
    {
        public CommandViewModel(string displayName, ICommand command)
        {
            if (command == null)
                throw new ArgumentNullException("command");

            base.DisplayName = displayName;
            this.Command = command;
        }

        public ICommand Command { get; private set; }

    }

    /// <summary>
    /// A closeable dynamic view suitable for TabViews on the main screen
    /// </summary>
    public abstract class WorkspaceViewModel : ViewModelBase
    {
        #region Fields

        RelayCommand _closeCommand;

        #endregion // Fields

        #region Constructor

        protected WorkspaceViewModel()
        {
        }

        #endregion // Constructor

        #region CloseCommand

        /// <summary>
        /// Returns the command that, when invoked, attempts
        /// to remove this workspace from the user interface.
        /// </summary>
        public ICommand CloseCommand
        {
            get
            {
                if (_closeCommand == null)
                    _closeCommand = new RelayCommand(param => this.OnRequestClose());

                return _closeCommand;
            }
        }

        #endregion // CloseCommand

        #region RequestClose [event]

        /// <summary>
        /// Raised when this workspace should be removed from the UI.
        /// </summary>
        public event EventHandler RequestClose;

        void OnRequestClose()
        {
            EventHandler handler = this.RequestClose;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        #endregion // RequestClose [event]
    }



#endregion

#region------------------------------------- Base Model Classes --------------------------------------

    public class ApplicationModel
    {
        #region Creation

        public static ApplicationModel Create() { return new ApplicationModel(); }
        public static ApplicationModel Create(ApplicationViewModel parent)
        {
            return new ApplicationModel
            {
          //      _parent = parent;

          //      _user = new WindowsIdentity.GetCurrent();
                
            };
        }

        #endregion

        #region Private Properties

        private WindowsIdentity _user;
        private ApplicationViewModel _parent;

        #endregion

        #region Public Properties

        public WindowsIdentity User
        {
            get { return _user; }
        }

        #endregion

        #region Constructor
        public ApplicationModel()
        {
            // If Debug Allow the Impersonion a dev user
            _user = WindowsIdentity.GetCurrent();
        }

        #endregion
    }

    /// <summary>
    /// Base Model for the Application Header ViewModel
    /// </summary>
    public class ApplicationHeader
    {
        #region Creation

        public static ApplicationHeader Create() { return new ApplicationHeader(); }

        public static ApplicationHeader Create(
            String applicationtitle,
            String applicationsubtitle)
        {
            return new ApplicationHeader
            {
                ApplicationTitle = applicationtitle,
                ApplicationSubTitle = applicationsubtitle
            };
        }

        #endregion

        #region public properties

        public String ApplicationTitle { get; set; }
        public String ApplicationSubTitle { get; set; }

        #endregion
    }

    /// <summary>
    /// Base Model for the APplications Status Bar
    /// </summary>
    public class StatusBarModel
    {
        #region Creation
        public static StatusBarModel Create() { return new StatusBarModel(); }

        public static StatusBarModel Create(
            String username,
            String applicationmode,
            String message,
            bool ismodified)
        {
            return new StatusBarModel
            {
                UserName = username,
                ApplicationMode = applicationmode,
                Message = message,
                IsModified = ismodified
            };
        }
        #endregion

        #region Public Properties

        public String UserName { get; set; }
        public String ApplicationMode { get; set; }
        public String Message { get; set; }
        public bool IsModified { get; set; }
        
        #endregion

    }

    /// <summary>
    /// Base Model for the Main Window
    /// </summary>
    public class MainWindowModel
    {
        #region Creation

        public static MainWindowModel Create() { return new MainWindowModel();}
        public static MainWindowModel Create(ApplicationViewModel parent)
        {
            return new MainWindowModel
            {
                _parent = parent
            };
        }

        #endregion

        #region Constructor
        public MainWindowModel()
        {
            CreateCommands();
            _applicationheader = new ApplicationHeaderViewModel(ApplicationHeader.Create(Strings.ApplicationName,Strings.Subtitle));
            _statusbar = new StatusBarViewModel(_parent);
        }

        #endregion

        #region Private Properties

        private ApplicationViewModel _parent;
        private ApplicationHeaderViewModel _applicationheader;
        private StatusBarViewModel _statusbar;
        private List<CommandViewModel> _commands;

        #endregion

        #region Public Properties

        public ApplicationViewModel Parent
        {
            get { return _parent; }
        }

        public List<CommandViewModel> AppCommands
        {
            get { return _commands;}
        }
                
        #endregion

        #region Private Methods

        private List<CommandViewModel> CreateCommands()
        {
            return new List<CommandViewModel>
            {
                new CommandViewModel("Exit",new RelayCommand(param => this.ExitCommand())),
            };
        }

        #endregion

        #region Static Commands

        void ExitCommand()
        {
            _parent.ApplicationExit();
        }

        #endregion

        #region Command Injection

        /// <summary>
        /// Insert a new commandviewmodel to the application, place on top of list
        /// </summary>
        /// <param name="newcmd"></param>
        public void InjectCommandViewModel(CommandViewModel newcmd)
        {
            List<CommandViewModel> _newlist = new List<CommandViewModel>();

            // Newest Command is always on top...
            _newlist.Add(newcmd);
            // Add all the existing commands, if any
            if (AppCommands != null)
            foreach (CommandViewModel elem in AppCommands)
            {
                _newlist.Add(elem);
            }
            
            _commands = _newlist;
        }

        #endregion

    }


#endregion

}

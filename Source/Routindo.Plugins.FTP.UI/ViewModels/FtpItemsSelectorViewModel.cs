using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Routindo.Contract.Arguments;
using Routindo.Contract.UI;
using Routindo.Plugins.FTP.Components.Watchers;
using Routindo.Plugins.FTP.UI.Enums;

namespace Routindo.Plugins.FTP.UI.ViewModels
{
    public abstract class FtpItemsSelectorViewModel: PluginConfiguratorViewModelBase
    {
        private string _host;
        private string _username;
        private string _password;
        private int _port = FtpItemsSelector.DefaultFtpPort;
        private string _remoteWorkingDir;
        private int _maximumFiles = 1;
        private int _createdBefore = 1;
        private int _createdAfter = 1;
        private int _editedBefore = 1;
        private int _editedAfter = 1;
        private TimePeriod _createdBeforePeriod;
        private TimePeriod _createdAfterPeriod;
        private TimePeriod _editedBeforePeriod;
        private TimePeriod _editedAfterPeriod;
        private bool _filterByCreatedBefore;
        private bool _filterByCreatedAfter;
        private bool _filterByEditedBefore;
        private bool _filterByEditedAfter;
        private ItemsSelectionSortingCriteria _sortingCriteria;
        private bool _filterByCreationTime;
        private bool _filterByEditionTime;
        private string _exampleCreationTimeString;
        private string _exampleEditionTimeString;
        private string _searchPattern = "*.*";

        public FtpItemsSelectorViewModel()
        {
            TimePeriods = new ObservableCollection<TimePeriod>(Enum.GetValues<TimePeriod>());
            SortingCriterias = new ObservableCollection<ItemsSelectionSortingCriteria>(Enum.GetValues<ItemsSelectionSortingCriteria>());

            RefreshExampleCreationTimeStringCommand = new RelayCommand(RefreshExampleCreationTimeString);
            RefreshExampleEditionTimeStringCommand = new RelayCommand(RefreshExampleEditionTimeString);
        }

        public string Host
        {
            get => _host;
            set
            {
                _host = value;
                ClearPropertyErrors();
                ValidateNonNullOrEmptyString(value);
                OnPropertyChanged();
            }
        }

        public string Username
        {
            get => _username;
            set
            {
                ClearPropertyErrors();
                _username = value;
                ValidateNonNullOrEmptyString(value);
                OnPropertyChanged();
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                ClearPropertyErrors();
                _password = value;
                ValidateNonNullOrEmptyString(value);
                OnPropertyChanged();
            }
        }

        public int Port
        {
            get => _port;
            set
            {
                ClearPropertyErrors();
                _port = value;
                ValidatePortNumber(_port);
                OnPropertyChanged();
            }
        }

        
        public string RemoteWorkingDir
        {
            get => _remoteWorkingDir;
            set
            {
                _remoteWorkingDir = value;
                OnPropertyChanged();
            }
        }

        public int MaximumFiles
        {
            get => _maximumFiles;
            set
            {
                _maximumFiles = value;
                ClearPropertyErrors();
                ValidateNumber(MaximumFiles, i => i > 0);
                OnPropertyChanged();
            }
        }

        public string SearchPattern
        {
            get => _searchPattern;
            set
            {
                _searchPattern = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<TimePeriod> TimePeriods { get; }

        #region sorting
        public ObservableCollection<ItemsSelectionSortingCriteria> SortingCriterias { get; }

        public ItemsSelectionSortingCriteria SortingCriteria
        {
            get => _sortingCriteria;
            set
            {
                _sortingCriteria = value;
                OnPropertyChanged();
            }
        }
        #endregion 

        #region Creation time
        public bool FilterByCreationTime
        {
            get => _filterByCreationTime;
            set
            {
                _filterByCreationTime = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(FilterByCreationTimeHasError));
                if (!value)
                {
                    if (FilterByCreatedAfter)
                        FilterByCreatedAfter = false;

                    if (FilterByCreatedBefore)
                        FilterByCreatedBefore = false;
                }
            }
        }

        public bool FilterByCreationTimeHasError
        {
            get
            {
                ClearPropertyErrors();
                var result = FilterByCreationTime &&
                             this.FilterByCreatedBefore
                             && FilterByCreatedAfter
                             && GetTimeSpanFromFilter(this.CreatedAfterPeriod, this.CreatedAfter).TotalMilliseconds <=
                             GetTimeSpanFromFilter(this.CreatedBeforePeriod, this.CreatedBefore).TotalMilliseconds;
                if (result)
                {
                    AddPropertyError(nameof(FilterByCreationTimeHasError), "Creation Time filter has errors");
                }
                return result;
            }
        }

        public ICommand RefreshExampleCreationTimeStringCommand { get; set; }
        private void RefreshExampleCreationTimeString()
        {
            ExampleCreationTimeString = this.GetExampleTimeString(FilterByCreatedBefore, CreatedBeforePeriod,
                CreatedBefore, FilterByCreatedAfter, CreatedAfterPeriod, CreatedAfter, "created");
        }

        private string GetExampleTimeString(bool before, TimePeriod timePeriodBefore, int timeValueBefore, bool after, TimePeriod timePeriodAfter, int timeValueAfter, string actionName)
        {
            var creationTimeBefore = DateTime.Now.AddMilliseconds(-GetTimeSpanFromFilter(timePeriodBefore, timeValueBefore).TotalMilliseconds);
            var creationTimeAfter = DateTime.Now.AddMilliseconds(-GetTimeSpanFromFilter(timePeriodAfter, timeValueAfter).TotalMilliseconds);

            if (before && after)
            {
                return
                    $"Files {actionName} between [{creationTimeAfter:G}] and [{creationTimeBefore:G}]";
            }

            if (before)
            {
                return
                    $"Files {actionName} before [{creationTimeBefore:G}]";
            }

            if (after)
            {
                return
                    $"Files {actionName} after [{creationTimeAfter:G}]";
            }
            return string.Empty;
        }

        public string ExampleCreationTimeString
        {
            get => _exampleCreationTimeString;
            set
            {
                _exampleCreationTimeString = value;
                OnPropertyChanged();
            }
        }

        public ICommand RefreshExampleEditionTimeStringCommand { get; set; }
        private void RefreshExampleEditionTimeString()
        {
            ExampleEditionTimeString = this.GetExampleTimeString(FilterByEditedBefore, EditedBeforePeriod,
                EditedBefore, FilterByEditedAfter, EditedAfterPeriod, EditedAfter, "edited");
        }
        public string ExampleEditionTimeString
        {
            get => _exampleEditionTimeString;
            set
            {
                _exampleEditionTimeString = value;
                OnPropertyChanged();
            }
        }

        public bool FilterByCreatedBefore
        {
            get => _filterByCreatedBefore;
            set
            {
                _filterByCreatedBefore = value;
                OnPropertyChanged();
                FilterByCreationTime = _filterByCreatedBefore || FilterByCreatedAfter;
                OnPropertyChanged(nameof(FilterByCreationTimeHasError));
            }
        }

        public bool FilterByCreatedAfter
        {
            get => _filterByCreatedAfter;
            set
            {
                _filterByCreatedAfter = value;
                OnPropertyChanged();
                FilterByCreationTime = _filterByCreatedAfter || FilterByCreatedBefore;
                OnPropertyChanged(nameof(FilterByCreationTimeHasError));
            }
        }


        public TimePeriod CreatedBeforePeriod
        {
            get => _createdBeforePeriod;
            set
            {
                _createdBeforePeriod = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(FilterByCreationTimeHasError));
            }
        }

        public TimePeriod CreatedAfterPeriod
        {
            get => _createdAfterPeriod;
            set
            {
                _createdAfterPeriod = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(FilterByCreationTimeHasError));
            }
        }

        public int CreatedBefore
        {
            get => _createdBefore;
            set
            {
                _createdBefore = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(FilterByCreationTimeHasError));
            }
        }

        public int CreatedAfter
        {
            get => _createdAfter;
            set
            {
                _createdAfter = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(FilterByCreationTimeHasError));
            }
        }
        #endregion

        #region Edition time

        public bool FilterByEditionTime
        {
            get => _filterByEditionTime;
            set
            {
                _filterByEditionTime = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(FilterByEditionTimeHasError));

                if (!value)
                {
                    if (FilterByEditedAfter)
                        FilterByEditedAfter = false;

                    if (FilterByEditedBefore)
                        FilterByEditedBefore = false;
                }
            }
        }

        public bool FilterByEditionTimeHasError
        {
            get
            {
                ClearPropertyErrors();
                var result = FilterByEditionTime &&
                             this.FilterByEditedBefore
                             && FilterByEditedAfter
                             && GetTimeSpanFromFilter(this.EditedAfterPeriod, this.EditedAfter).TotalMilliseconds <=
                             GetTimeSpanFromFilter(this.EditedBeforePeriod, this.EditedBefore).TotalMilliseconds;

                if (result)
                    AddPropertyError(nameof(FilterByEditionTimeHasError), "Edition Time filter has errors");

                return result;
            }
        }

        public TimePeriod EditedBeforePeriod
        {
            get => _editedBeforePeriod;
            set
            {
                _editedBeforePeriod = value;
                OnPropertyChanged();
                FilterByEditionTime = _filterByEditedBefore || FilterByEditedAfter;
                OnPropertyChanged(nameof(FilterByEditionTimeHasError));
            }
        }

        public TimePeriod EditedAfterPeriod
        {
            get => _editedAfterPeriod;
            set
            {
                _editedAfterPeriod = value;
                OnPropertyChanged();
                FilterByEditionTime = _filterByEditedAfter || FilterByEditedBefore;
                OnPropertyChanged(nameof(FilterByEditionTimeHasError));
            }
        }

        public bool FilterByEditedBefore
        {
            get => _filterByEditedBefore;
            set
            {
                _filterByEditedBefore = value;
                OnPropertyChanged();
                FilterByEditionTime = _filterByEditedBefore || FilterByEditedAfter;
                OnPropertyChanged(nameof(FilterByEditionTimeHasError));
            }
        }

        public bool FilterByEditedAfter
        {
            get => _filterByEditedAfter;
            set
            {
                _filterByEditedAfter = value;
                OnPropertyChanged();
                FilterByEditionTime = _filterByEditedAfter || FilterByEditedBefore;
                OnPropertyChanged(nameof(FilterByEditionTimeHasError));
            }
        }

        public int EditedBefore
        {
            get => _editedBefore;
            set
            {
                _editedBefore = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(FilterByEditionTimeHasError));
            }
        }

        public int EditedAfter
        {
            get => _editedAfter;
            set
            {
                _editedAfter = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(FilterByEditionTimeHasError));
            }
        }

        #endregion

        private TimeSpan GetTimeSpanFromFilter(TimePeriod timePeriod, int timeValue)
        {
            TimeSpan timeSpan;
            switch (timePeriod)
            {
                case TimePeriod.Days:
                    {
                        timeSpan = TimeSpan.FromDays(timeValue);
                        break;
                    }
                case TimePeriod.Hours:
                    {
                        timeSpan = TimeSpan.FromHours(timeValue);
                        break;
                    }
                case TimePeriod.Minutes:
                    {
                        timeSpan = TimeSpan.FromMinutes(timeValue);
                        break;
                    }
                case TimePeriod.Seconds:
                    {
                        timeSpan = TimeSpan.FromSeconds(timeValue);
                        break;
                    }
                default:
                    {
                        timeSpan = TimeSpan.FromMilliseconds(timeValue);
                        break;
                    }
            }

            return timeSpan;
        }

        private bool TryGetTimePeriodFromMilliseconds(ulong? milliseconds, out TimePeriod timePeriod, out int timeValue)
        {
            if (milliseconds.HasValue)
            {
                var ts = TimeSpan.FromMilliseconds(milliseconds.Value);
                if (ts.Days > 0)
                {
                    timePeriod = TimePeriod.Days;
                    timeValue = ts.Days;
                }
                else if (ts.Hours > 0)
                {
                    timePeriod = TimePeriod.Hours;
                    timeValue = ts.Hours;
                }
                else if (ts.Minutes > 0)
                {
                    timePeriod = TimePeriod.Minutes;
                    timeValue = ts.Minutes;
                }
                else if (ts.Seconds > 0)
                {
                    timePeriod = TimePeriod.Seconds;
                    timeValue = ts.Seconds;
                }
                else
                {
                    timePeriod = TimePeriod.Milliseconds;
                    timeValue = ts.Milliseconds;
                }

                return true;
            }

            timePeriod = TimePeriod.Milliseconds;
            timeValue = 0;
            return false;
        }

        public override void Configure()
        {
            this.InstanceArguments = ArgumentCollection.New()
                .WithArgument(FtpItemsSelectorArgs.Host, Host)
                .WithArgument(FtpItemsSelectorArgs.Port, Port)
                .WithArgument(FtpItemsSelectorArgs.Username, Username)
                .WithArgument(FtpItemsSelectorArgs.Password, Password)
                .WithArgument(FtpItemsSelectorArgs.RemoteWorkingDir, RemoteWorkingDir)
                .WithArgument(FtpItemsSelectorArgs.SearchPattern, SearchPattern)
                .WithArgument(FtpItemsSelectorArgs.MaximumFiles, MaximumFiles)
                .WithArgument(FtpItemsSelectorArgs.SelectFiles, true)
                .WithArgument(FtpItemsSelectorArgs.SelectDirectories, false)
                .WithArgument(FtpItemsSelectorArgs.SortingCriteria, SortingCriteria);

            if (FilterByCreatedBefore)
            {
                TimeSpan createdBeforeTimeSpan = GetTimeSpanFromFilter(CreatedBeforePeriod, CreatedBefore);
                InstanceArguments = InstanceArguments.WithArgument(FtpItemsSelectorArgs.CreatedBefore,
                    Convert.ToUInt64(createdBeforeTimeSpan.TotalMilliseconds));
            }
            else
            {
                InstanceArguments = InstanceArguments.WithArgument(FtpItemsSelectorArgs.CreatedBefore, null);
            }

            if (FilterByCreatedAfter)
            {
                TimeSpan createdAfterTimeSpan = GetTimeSpanFromFilter(CreatedAfterPeriod, CreatedAfter);
                InstanceArguments = InstanceArguments.WithArgument(FtpItemsSelectorArgs.CreatedAfter,
                    Convert.ToUInt64(createdAfterTimeSpan.TotalMilliseconds));
            }
            else
            {
                InstanceArguments = InstanceArguments.WithArgument(FtpItemsSelectorArgs.CreatedAfter, null);
            }

            if (FilterByEditedBefore)
            {
                TimeSpan editedBeforeTimeSpan = GetTimeSpanFromFilter(EditedBeforePeriod, EditedBefore);
                InstanceArguments = InstanceArguments.WithArgument(FtpItemsSelectorArgs.EditedBefore,
                    Convert.ToUInt64(editedBeforeTimeSpan.TotalMilliseconds));
            }
            else
            {
                InstanceArguments = InstanceArguments.WithArgument(FtpItemsSelectorArgs.EditedBefore, null);
            }

            if (FilterByEditedAfter)
            {
                TimeSpan editedAfterTimeSpan = GetTimeSpanFromFilter(EditedAfterPeriod, EditedAfter);
                InstanceArguments = InstanceArguments.WithArgument(FtpItemsSelectorArgs.EditedAfter,
                    Convert.ToUInt64(editedAfterTimeSpan.TotalMilliseconds));
            }
            else
            {
                InstanceArguments = InstanceArguments.WithArgument(FtpItemsSelectorArgs.EditedAfter, null);
            }
        }

        public override void SetArguments(ArgumentCollection arguments)
        {
            if (arguments == null || !arguments.Any())
                return;

            if (arguments.HasArgument(FtpItemsSelectorArgs.Host))
                Host = arguments.GetValue<string>(FtpItemsSelectorArgs.Host);

            if (arguments.HasArgument(FtpItemsSelectorArgs.Username))
                Username = arguments.GetValue<string>(FtpItemsSelectorArgs.Username);

            if (arguments.HasArgument(FtpItemsSelectorArgs.Password))
                Password = arguments.GetValue<string>(FtpItemsSelectorArgs.Password);

            if (arguments.HasArgument(FtpItemsSelectorArgs.Port))
            {
                var port = arguments.GetValue<int>(FtpItemsSelectorArgs.Port);
                if (port > 0)
                    Port = port;
            }

            if (arguments.HasArgument(FtpItemsSelectorArgs.RemoteWorkingDir))
                RemoteWorkingDir = arguments.GetValue<string>(FtpItemsSelectorArgs.RemoteWorkingDir);

            if (arguments.HasArgument(FtpItemsSelectorArgs.SearchPattern))
                SearchPattern = arguments.GetValue<string>(FtpItemsSelectorArgs.SearchPattern);

            if (arguments.HasArgument(FtpItemsSelectorArgs.MaximumFiles))
            {
                if (int.TryParse(arguments.GetValue<string>(FtpItemsSelectorArgs.MaximumFiles), out int maximumFiles))
                {
                    MaximumFiles = maximumFiles;
                }
            }

            if (arguments.HasArgument(FtpItemsSelectorArgs.CreatedBefore))
            {
                var createdBefore = arguments.GetValue<ulong?>(FtpItemsSelectorArgs.CreatedBefore);
                if (TryGetTimePeriodFromMilliseconds(createdBefore, out var timePeriod, out var timeValue))
                {
                    FilterByCreatedBefore = true;
                    CreatedBeforePeriod = timePeriod;
                    CreatedBefore = timeValue;
                }
            }

            if (arguments.HasArgument(FtpItemsSelectorArgs.CreatedAfter))
            {
                var createdAfter = arguments.GetValue<ulong?>(FtpItemsSelectorArgs.CreatedAfter);
                if (TryGetTimePeriodFromMilliseconds(createdAfter, out var timePeriod, out var timeValue))
                {
                    FilterByCreatedAfter = true;
                    CreatedAfterPeriod = timePeriod;
                    CreatedAfter = timeValue;
                }
            }

            if (arguments.HasArgument(FtpItemsSelectorArgs.EditedBefore))
            {
                var editedBefore = arguments.GetValue<ulong?>(FtpItemsSelectorArgs.EditedBefore);
                if (TryGetTimePeriodFromMilliseconds(editedBefore, out var timePeriod, out var timeValue))
                {
                    FilterByEditedBefore = true;
                    EditedBeforePeriod = timePeriod;
                    EditedBefore = timeValue;
                }
            }

            if (arguments.HasArgument(FtpItemsSelectorArgs.EditedAfter))
            {
                var editedAfter = arguments.GetValue<ulong?>(FtpItemsSelectorArgs.EditedAfter);
                if (TryGetTimePeriodFromMilliseconds(editedAfter, out var timePeriod, out var timeValue))
                {
                    FilterByEditedAfter = true;
                    EditedAfterPeriod = timePeriod;
                    EditedAfter = timeValue;
                }
            }

            if (arguments.HasArgument(FtpItemsSelectorArgs.SortingCriteria))
            {
                SortingCriteria = arguments.GetValue<ItemsSelectionSortingCriteria>(FtpItemsSelectorArgs.SortingCriteria);
            }
        }
    }
}

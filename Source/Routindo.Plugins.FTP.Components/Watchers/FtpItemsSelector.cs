using System;
using System.Collections.Generic;
using System.Linq;
using FluentFTP;
using Routindo.Contract.Attributes;
using Routindo.Contract.Services;

namespace Routindo.Plugins.FTP.Components.Watchers
{
    public abstract class FtpItemsSelector
    {
        protected abstract ILoggingService Logger { get; }

        public const int DefaultFtpPort = 21; 
        [Argument(FtpItemsSelectorArgs.Host, true)] public string Host { get; set; }

        [Argument(FtpItemsSelectorArgs.Port, true)] public int Port { get; set; } = DefaultFtpPort;

        [Argument(FtpItemsSelectorArgs.Username, true)] public string Username { get; set; }

        [Argument(FtpItemsSelectorArgs.Password, true)] public string Password { get; set; }

        [Argument(FtpItemsSelectorArgs.RemoteWorkingDir)] public string RemoteWorkingDir { get; set; }

        [Argument(FtpItemsSelectorArgs.SelectFiles)] public bool SelectFiles { get; set; } = true;

        [Argument(FtpItemsSelectorArgs.SelectDirectories)] public bool SelectDirectories { get; set; }

        [Argument(FtpItemsSelectorArgs.SortingCriteria)] public ItemsSelectionSortingCriteria SortingCriteria { get; set; }

        [Argument(FtpItemsSelectorArgs.MaximumFiles)] public int MaximumFiles { get; set; } = 1;

        [Argument(FtpItemsSelectorArgs.CreatedBefore)] public ulong? CreatedBefore { get; set; }

        [Argument(FtpItemsSelectorArgs.CreatedAfter)] public ulong? CreatedAfter { get; set; }

        [Argument(FtpItemsSelectorArgs.EditedBefore)] public ulong? EditedBefore { get; set; }

        [Argument(FtpItemsSelectorArgs.EditedAfter)] public ulong? EditedAfter { get; set; }

        public List<string> Select()
        {
            try
            {
                List<string> items;
                using (FtpClient ftpClient = new FtpClient(Host, Port, Username, Password))
                {
                    if (!string.IsNullOrWhiteSpace(RemoteWorkingDir))
                        ftpClient.SetWorkingDirectory(RemoteWorkingDir);

                    var listing = ftpClient.GetListing().ToList();
                    Logger.Debug($"First Listing: {listing.Count}");
                    List<FtpFileSystemObjectType> targetTypes = new List<FtpFileSystemObjectType>();

                    if (SelectFiles)
                        targetTypes.Add(FtpFileSystemObjectType.File);

                    if (SelectDirectories)
                        targetTypes.Add(FtpFileSystemObjectType.Directory);

                    listing = listing.Where(item => targetTypes.Contains(item.Type)).ToList();
                    Logger.Debug($"Limited by type: {listing.Count}");
                    listing = GetFilesFilteredByTime(listing);
                    Logger.Debug($"Filtered per time: {listing.Count}");
                    items = GetSortedFiles(listing)
                        .Take(MaximumFiles)
                        .Select(e => e.FullName).ToList();
                }

                return items;
            }
            catch (Exception exception)
            {
                Logger.Error(exception);
                return new List<string>();
            }
        }

        private List<FtpListItem> GetFilesFilteredByTime(List<FtpListItem> selectedItems)
        {
            if (CreatedBefore.HasValue)
            {
                selectedItems = selectedItems
                    .Where(f => f.Created < DateTime.Now.AddMilliseconds(-Convert.ToDouble(CreatedBefore.Value))).ToList();
            }

            if (CreatedAfter.HasValue)
            {
                selectedItems = selectedItems
                    .Where(f => f.Created > DateTime.Now.AddMilliseconds(-Convert.ToDouble(CreatedAfter.Value))).ToList();
            }

            if (EditedBefore.HasValue)
            {
                selectedItems = selectedItems
                    .Where(f => f.Modified < DateTime.Now.AddMilliseconds(-Convert.ToDouble(EditedBefore.Value))).ToList();
            }

            if (EditedAfter.HasValue)
            {
                selectedItems = selectedItems
                    .Where(f => f.Modified > DateTime.Now.AddMilliseconds(-Convert.ToDouble(EditedAfter.Value))).ToList();
            }

            return selectedItems;
        }

        private List<FtpListItem> GetSortedFiles(List<FtpListItem> selectedFiles)
        {
            if (SortingCriteria == ItemsSelectionSortingCriteria.CreationTimeAscending)
            {
                return selectedFiles
                    .OrderBy(f => f.Created)
                    .ToList();
            }

            if (SortingCriteria == ItemsSelectionSortingCriteria.EditionTimeAscending)
            {
                return selectedFiles
                    .OrderBy(f => f.Modified)
                    .ToList();
            }

            if (SortingCriteria == ItemsSelectionSortingCriteria.CreationTimeDescending)
            {
                return selectedFiles
                    .OrderByDescending(f => f.Created)
                    .ToList();
            }

            if (SortingCriteria == ItemsSelectionSortingCriteria.EditionTimeDescending)
            {
                return selectedFiles
                    .OrderByDescending(f => f.Modified)
                    .ToList();
            }

            return selectedFiles;
        }
    }
}

using System.Collections.Immutable;
using System.Diagnostics;
using System.Management.Automation;

namespace OPassCliPS.Common
{
    public abstract class CacheBasedCmdlet : BaseCmdlet
    {
        [Parameter()]
        public SwitchParameter RefreshCache { get; set; }

        internal void UpdateCache(bool force, bool createCache = false)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            var allItems = SharedState.OPManager.SearchForItems(includeArchive: true);
            var items = SharedState.OPManager.SearchForItems();
            var archivedItems = allItems.ExceptBy(items.Select(e => e.Id), x => x.Id);

            string cacheAction;
            string cacheMessage;
            if (createCache)
            {
                cacheAction = "creation";
                cacheMessage = "Creating module cache for vaults, items, and accounts.";
            }
            else
            {
                cacheAction = "refreshing";
                cacheMessage = "Refreshing module cache.";
            }
            if (force)
            {
                WriteVerbose(cacheMessage);
                SharedState.Cache.Vaults = SharedState.OPManager.GetVaults();
                SharedState.Cache.Items = items;
                SharedState.Cache.ArchivedItems = archivedItems.ToImmutableList();
                SharedState.Cache.Accounts = SharedState.OPManager.GetAccounts();
                string elapsedTime = stopwatch.Elapsed.ToString("mm':'ss':'fff");
                WriteVerbose($"Cache {cacheAction} time: {elapsedTime}");
            }
            stopwatch.Stop();

        }
    
        internal void WriteCacheMessage()
        {
            if (!SharedState.SuppressCacheMessage)
            {
                WriteVerbose("This command uses the module cache. If you want to force a refresh of the cache, please use the -RefreshCache parameter.");
            }
        }
    }
}

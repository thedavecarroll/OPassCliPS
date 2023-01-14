using OnePassword.Items;
using OnePassword.Vaults;
using OPassCliPS.Common;
using System.Collections.Immutable;
using System.Management.Automation;

namespace OPassCliPS.Items
{
    [Cmdlet(VerbsCommon.Get, "OPItem", DefaultParameterSetName = "ItemSearch")]
    [OutputType(typeof(List<Item>), typeof(Item))]
    public class GetOPItemCommand : CacheBasedCmdlet
    {

        [Parameter(Position = 0, ValueFromPipeline = true)]
        [Parameter(ParameterSetName = "ItemSearch", Mandatory = false)]
        [Parameter(ParameterSetName = "GetItem", Mandatory = true)]
        [ValidateNotNullOrEmpty]
        public string Vault { get; set; }

        [Parameter(Position = 1, ValueFromPipeline = true, ValueFromRemainingArguments = true)]
        [Parameter(ParameterSetName = "GetItem", Mandatory = true)]
        [ValidateNotNullOrEmpty]
        public string Item { get; set; }

        [Parameter(Position = 2)]
        [Parameter(ParameterSetName = "ItemSearch", Mandatory = false)]
        public List<Category> Category { get; set; }

        [Parameter(Position = 3)]
        [Parameter(ParameterSetName = "ItemSearch", Mandatory = false)]
        public List<string> Tag { get; set; }

        [Parameter(Position = 4)]
        [Parameter(ParameterSetName = "ItemSearch", Mandatory = false)]
        [Parameter(ParameterSetName = "GetItem", Mandatory = false)]
        public SwitchParameter IncludeArchive { get; set; }

        [Parameter(Position = 5)]
        [Parameter(ParameterSetName = "ItemSearch", Mandatory = false)]
        public SwitchParameter Favorite { get; set; }

        private IVault? _vault = null;
        private bool _includeArchive;
        private bool _favorite;
        IReadOnlyCollection<Category>? _categories = null;
        IReadOnlyCollection<string>? _tags = null;
        List<string> searchCriteria = new();

        protected override void BeginProcessing()
        {
            ValidateSession();
            UpdateCache(RefreshCache);
            WriteCacheMessage();

            if (MyInvocation.BoundParameters.ContainsKey("Vault"))
            {
                _vault = SharedState.Cache.Vaults.First(x => x.Name == Vault);
                searchCriteria.Add("Vault");
            }
            if (IncludeArchive)
            {
                _includeArchive = true;
                searchCriteria.Add("IncludeArchive");
            }
            if (Favorite)
            {
                _favorite = true;
                searchCriteria.Add("Favorite");
            }
            if (MyInvocation.BoundParameters.ContainsKey("Category"))
            {
                _categories = Category.AsReadOnly();
                searchCriteria.Add("Category");
            }
            if (MyInvocation.BoundParameters.ContainsKey("Tag"))
            {
                _tags = Tag.AsReadOnly();
                searchCriteria.Add("Tag");
            }
        }

        protected override void ProcessRecord()
        {
            Item getItem;
            ImmutableList<Item> searchForItems;
            Item? item = null;

            if (ParameterSetName == "GetItem")
            {
                try
                {
                    try
                    {
                        item = SharedState.Cache.Items.First(x => x.Title == Item);
                    }
                    catch { }

                    item = SharedState.Cache.ArchivedItems.First(x => x.Title == Item);
                    WriteWarning($"The secret {Item} has been archived.");

                }
                catch (Exception exception)
                {

                    ThrowTerminatingError(new ErrorRecord(
                         exception,
                         "NotConnected",
                         ErrorCategory.AuthenticationError,
                         null)
                    );
                }

                string verboseMessage;
                if (_vault != null)
                {
                    verboseMessage = $"Getting details for item {item.Id} from vault {_vault.Name}.";
                }
                else
                {
                    verboseMessage = $"Getting details for item {item.Id}.";
                }
                WriteVerbose(verboseMessage);
                getItem = SharedState.OPManager.SearchForItem(item, _vault, _includeArchive);
                WriteObject(getItem);

            }
            else
            {
                string searchCriteriaText = searchCriteria.Count > 0 ? $" based on the following critera: {string.Join(", ", searchCriteria)}" : " without any criteria.";
                WriteVerbose($"Searching for all items{searchCriteriaText}");
                searchForItems = SharedState.OPManager.SearchForItems(_vault, _includeArchive, _favorite, _categories, _tags);
                foreach (Item foundItem in searchForItems)
                {
                    WriteObject(foundItem);
                }
            }
        }
    }
}

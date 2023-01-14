using OnePassword.Items;
using OnePassword.Vaults;
using OPassCliPS.Common;
using System.Collections.Immutable;
using System.Management.Automation;

namespace OPassCliPS.Items
{
    [Cmdlet(VerbsCommon.Get, "OPVaultItem", DefaultParameterSetName = "VaultItems")]
    [OutputType(typeof(List<Item>), typeof(Item))]
    public class GetOPVaultItemCommand : CacheBasedCmdlet
    {

        [Parameter(Position = 0, ValueFromPipeline = true, Mandatory = true)]
        [Parameter(ParameterSetName = "VaultItems")]
        [Parameter(ParameterSetName = "GetItem")]
        [ValidateNotNullOrEmpty]
        public string Vault { get; set; }

        [Parameter(Position = 1, ValueFromPipeline = true, ValueFromRemainingArguments = true)]
        [Parameter(ParameterSetName = "GetItem")]
        [ValidateNotNullOrEmpty]
        public string Item { get; set; }

        private IVault _vault;
        private IItem _item;

        protected override void BeginProcessing()
        {
            ValidateSession();
            UpdateCache(RefreshCache);
            WriteCacheMessage();

            Vault vault = SharedState.Cache.Vaults.First(x => x.Name == Vault);
            WriteVerbose($"Getting details for vault with name of {Vault}");
            _vault = SharedState.OPManager.GetVault(vault);

            if (ParameterSetName == "GetItem")
            {
                Item item = SharedState.Cache.Items.First(x => x.Title == Item);
                WriteVerbose($"Getting item with name of {Item}");
                _item = SharedState.OPManager.GetItem(item, _vault);
            }
        }

        protected override void ProcessRecord()
        {
            ImmutableList<Item> vaultItems;
            Item getItem;
            string verboseMessage;

            if (ParameterSetName == "VaultItems")
            {
                verboseMessage = $"Retrieving items for vault {Vault}.";
                WriteVerbose(verboseMessage);
                vaultItems = SharedState.OPManager.GetItems(_vault);
                foreach (IItem item in vaultItems)
                {
                    WriteObject(item);
                }
            }
            else
            {
                verboseMessage = $"Getting details for item {Item} from vault {Vault}.";
                WriteVerbose(verboseMessage);
                getItem = SharedState.OPManager.GetItem(_item, _vault);
                WriteObject(getItem);
            }
        }
    }
}

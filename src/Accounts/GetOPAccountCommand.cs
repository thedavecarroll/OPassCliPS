using OnePassword.Accounts;
using OPassCliPS.Common;
using System.Management.Automation;

namespace OPassCliPS.Accounts
{

    [Cmdlet(VerbsCommon.Get, "OPAccount", DefaultParameterSetName = "AllAccounts")]
    [OutputType(typeof(AccountDetails))]
    public class GetOPAccountCommand : CacheBasedCmdlet
    {
        [Parameter(Position = 0, Mandatory = false, ValueFromPipeline = true, ParameterSetName = "GetAccount")]
        [ValidateNotNullOrEmpty]
        public Account Account { get; set; }

        protected override void BeginProcessing()
        {
            ValidateSession();
            UpdateCache(RefreshCache);
            WriteCacheMessage();
        }

        protected override void ProcessRecord()
        {
            if (ParameterSetName == "AllAccounts")
            {
                foreach (Account account in SharedState.Cache.Accounts)
                {
                    WriteObject(SharedState.OPManager.GetAccount(account.Id));
                }
            }
            else
            {
                WriteObject(SharedState.OPManager.GetAccount(Account.Id));
            }

        }

    }
}

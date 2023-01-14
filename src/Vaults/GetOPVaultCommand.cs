using OnePassword.Vaults;
using OPassCliPS.Common;
using System.Management.Automation;

namespace OPassCliPS.Vaults
{
    [Cmdlet(VerbsCommon.Get, "OPVault")]
    [OutputType(typeof(Vault), typeof(List<Vault>))]
    public class GetOPVaultCommand : CacheBasedCmdlet
    {

        [Parameter(Position = 0, Mandatory = false)]
        [ValidateNotNullOrEmpty]
        public string Vault { get; set; }

        protected override void BeginProcessing()
        {
            ValidateSession();
            UpdateCache(RefreshCache);
            WriteCacheMessage();
        }

        protected override void ProcessRecord()
        {
            if (MyInvocation.BoundParameters.ContainsKey("Vault"))
            {
                Vault vault = SharedState.Cache.Vaults.First(x => x.Name == Vault);
                WriteObject(SharedState.OPManager.GetVault(vault));
            }
            else
            {
                foreach (Vault vault in SharedState.Cache.Vaults)
                {
                    WriteObject(vault);
                }
            }
        }
    }
}

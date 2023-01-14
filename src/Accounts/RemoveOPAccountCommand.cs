using OPassCliPS.Common;
using System.Management.Automation;

namespace OPassCliPS.Accounts
{
    [Cmdlet(VerbsCommon.Remove, "OPAccount")]
    public class RemoveOPAccountCommand : BaseCmdlet
    {
        protected override void BeginProcessing()
        {
            ValidateSession();
        }
        protected override void ProcessRecord()
        {
            WriteVerbose("Removing 1Password account from this system.");
            SharedState.OPManager.ForgetAccount();

        }

    }
}

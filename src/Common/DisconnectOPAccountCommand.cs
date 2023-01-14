using System.Management.Automation;

namespace OPassCliPS.Common
{
    [Cmdlet(VerbsCommunications.Disconnect, "OPAccount")]
    public class DisconnectOPAccountCommand : BaseCmdlet
    {
        protected override void ProcessRecord()
        {
            TerminateSession();
        }
    }
}

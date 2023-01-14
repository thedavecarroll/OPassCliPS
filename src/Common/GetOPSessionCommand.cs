using OPassCliPS.Common;
using System.Management.Automation;

namespace OPassCliPS
{
    [Cmdlet(VerbsCommon.Get, "OPSession")]
    [OutputType(typeof(SessionState))]
    public class GetOPSessionCommand : BaseCmdlet
    {
        protected override void ProcessRecord()
        {
            WriteObject(SharedState);
        }

    }
}

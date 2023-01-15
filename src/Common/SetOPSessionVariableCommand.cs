using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace OPassCliPS.Common
{
    [Cmdlet(VerbsCommon.Set,"OPSessionVariable", SupportsShouldProcess = true)]
    public class SetOPSessionVariableCommand : BaseCmdlet
    {
        [Parameter()]
        [ValidateNotNullOrEmpty()]
        public bool SuppressCacheMessage { get; set; }

        [Parameter()]
        [ValidateNotNullOrEmpty()]
        public string CliPath { get; set; }

        protected override void ProcessRecord()
        {
            if (MyInvocation.BoundParameters.ContainsKey("SuppressCacheMessage"))
            {
                WriteVerbose($"{DateTime.Now} : Current SuppressCacheMessage value: {SharedState.SuppressCacheMessage}");
                SharedState.SuppressCacheMessage = SuppressCacheMessage;
            }
            if (MyInvocation.BoundParameters.ContainsKey("CliPath"))
            {
                bool checkOverrite = false;
                if (SharedState.CliPath != null)
                {
                    WriteVerbose($"{DateTime.Now} : Current CliPath value: {SharedState.CliPath}");
                    checkOverrite = true;
                }
                string verboseDescription = "Overwrite current CliPath value";
                string verboseWarning = $"Are you sure you want to overwrite {SharedState.CliPath} with {CliPath}";
                string shouldProcessReason = "Set CliPath";

                if (checkOverrite) {
                    if (ShouldProcess(verboseDescription,verboseWarning,shouldProcessReason)) {
                        SetCliPath(CliPath);
                    }
                } else
                {
                    SetCliPath(CliPath);
                }
                
            }
        }
    }
}

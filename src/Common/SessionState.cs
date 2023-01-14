using OnePassword;
using OnePassword.Accounts;
using OnePassword.Items;
using OnePassword.Vaults;
using OPassCliPS.Common;
using System.Collections.Immutable;
using System.Management.Automation;
using System.Runtime.CompilerServices;

namespace OPassCliPS
{
    internal sealed class SessionState
    {
        private static readonly ConditionalWeakTable<PSModuleInfo, SessionState> stateMap = new();

        public static SessionState GetForCmdlet(PSCmdlet cmdlet) =>
            stateMap.GetValue(cmdlet.MyInvocation.MyCommand.Module, static _ => new());

        internal OnePasswordManager OPManager { get; set; }
        internal Cache Cache { get; set; } = new Cache();
        internal bool SuppressCacheMessage { get; set; } = false;

        public FileInfo CliPath { get; set; }
        public bool IsAppIntegrated { get; set; } = false;
        public bool IsAuthenticated { get; set; } = false;
        public AccountDetails? AuthenticatedAccount { get; set; } = null;
    }
}

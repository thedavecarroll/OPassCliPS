using OPassCliPS.Common;
using System.Management.Automation;
using System.Security;

namespace OPassCliPS.Accounts
{
    [Cmdlet(VerbsCommon.Add, "OPAccount")]
    public class AddOPAccountCommand : BaseCmdlet
    {
        [Parameter(Position = 0, Mandatory = true)]
        [ValidateNotNullOrEmpty]
        [Alias("Address")]
        public string Domain { get; set; }

        [Parameter(Position = 1, Mandatory = true)]
        [ValidateNotNullOrEmpty]
        public string Email { get; set; }

        [Parameter(Position = 2, Mandatory = true)]
        [ValidateNotNullOrEmpty]
        public SecureString SecretKey { get; set; }

        [Parameter(Position = 3, Mandatory = true)]
        [ValidateNotNullOrEmpty]
        public SecureString Password { get; set; }

        [Parameter(Position = 4, Mandatory = false)]
        [ValidateNotNullOrEmpty]
        public string Shorthand { get; set; }

        protected override void BeginProcessing()
        {
            ValidateSession();
        }
        protected override void ProcessRecord()
        {
            WriteVerbose($"{DateTime.Now} : Adding account for {Email}.");
            SharedState.OPManager.AddAccount(
                address: Domain,
                secretKey: ConvertFromSecureString(SecretKey),
                password: ConvertFromSecureString(Password),
                email: Email,
                shorthand: Shorthand
            );

        }
    }
}

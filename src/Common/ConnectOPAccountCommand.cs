using OnePassword.Accounts;
using System.Management.Automation;
using System.Security;
using System.Security.Authentication;

namespace OPassCliPS.Common
{
    [Cmdlet(VerbsCommunications.Connect, "OPAccount", DefaultParameterSetName = "AppIntegrated")]
    public class ConnectOPAccountCommand : CacheBasedCmdlet
    {
        [Parameter(Position = 0, Mandatory = true, ParameterSetName = "NotAppIntegrated")]
        [ValidateNotNullOrEmpty]
        public string Domain { get; set; }

        [Parameter(Position = 1, Mandatory = true, ParameterSetName = "NotAppIntegrated")]
        [ValidateNotNullOrEmpty]
        public string Email { get; set; }

        [Parameter(Position = 2, Mandatory = true, ParameterSetName = "NotAppIntegrated")]
        [ValidateNotNullOrEmpty]
        public SecureString SecretKey { get; set; }

        [Parameter(Position = 1, Mandatory = true, ParameterSetName = "SignInAccount")]
        [Parameter(Position = 3, Mandatory = true, ParameterSetName = "NotAppIntegrated")]
        [ValidateNotNullOrEmpty]
        public SecureString Password { get; set; }

        [Parameter(Position = 0, Mandatory = true, ParameterSetName = "SignInAccount")]
        [ValidateNotNullOrEmpty]
        public Account Account { get; set; }

        [Parameter(Mandatory = false)]
        [ValidateNotNullOrEmpty]
        public string CliPath { get; set; }

        [Parameter(Mandatory = false)]
        public SwitchParameter ShowAccount { get; set; }

        [Parameter(Mandatory = false)]
        public SwitchParameter NewSession { get; set; }

        private string? onePasswordPath;
        private bool createCache = false;

        protected override void BeginProcessing()
        {
            onePasswordPath = MyInvocation.BoundParameters.ContainsKey("CliPath") ? CliPath : null;
            switch (ParameterSetName)
            {
                case "AppIntegrated":
                    CreateSession(cliPath: onePasswordPath, force: NewSession);
                    break;

                case "NotAppIntegrated":
                    CreateSession(cliPath: onePasswordPath, secretKey: SecretKey, password: Password, domain: Domain, email: Email, force: NewSession);
                    break;

                case "SignInAccount":
                    CreateSession(cliPath: onePasswordPath, password: Password, email: Email, force: NewSession);
                    break;
            }

        }

        protected override void ProcessRecord()
        {
            WriteVerbose($"1Password CLI Version: {SharedState.OPManager.Version}");

            if (!SharedState.IsAuthenticated || ParameterSetName == "SignInAccount") {
                createCache = true;
                WriteVerbose("Signing into 1Password.");
                try
                {
                    switch (ParameterSetName) {
                        case "AppIntegrated":
                            SharedState.OPManager.SignIn();
                            break;

                        case "NotAppIntegrated":
                            SharedState.OPManager.SignIn(ConvertFromSecureString(Password));
                            break;

                        case "SignInAccount":
                            SharedState.OPManager.SignIn(ConvertFromSecureString(Password));
                            break;
                    }
                    SharedState.IsAuthenticated = true;
                    SharedState.IsAppIntegrated = ParameterSetName == "AppIntegrated";
                }
                catch (Exception exception)
                {
                    ThrowTerminatingError(new ErrorRecord(
                        exception: new AuthenticationException(string.Format("Signing into 1Password failed."), exception.InnerException),
                        errorId: MyInvocation.MyCommand.Name,
                        errorCategory: ErrorCategory.AuthenticationError,
                        targetObject: "1Password CLI App Signin")
                    );

                }
            }
            Thread.Sleep(500);
            try
            {
                WriteVerbose("Retrieving connected account.");
                SharedState.AuthenticatedAccount = SharedState.OPManager.GetAccount();
            }
            catch (Exception exception)
            {
                ThrowTerminatingError(new ErrorRecord(
                    exception: new AuthenticationException("Unable to retrieve the current user's acocunt.", exception.InnerException),
                    errorId: MyInvocation.MyCommand.Name,
                    errorCategory: ErrorCategory.InvalidOperation,
                    targetObject: "GetAccount")
                );
            }

            if (ShowAccount)
            {
                WriteObject(SharedState.AuthenticatedAccount);
            }

        }

        protected override void EndProcessing()
        {
            UpdateCache(createCache: createCache, force: true);
        }

    }
}

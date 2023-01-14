using System.Management.Automation;
using System.Net;
using System.Security;

namespace OPassCliPS.Common
{
    public abstract class BaseCmdlet : PSCmdlet
    {
        private SessionState _sharedState;
        private protected SessionState SharedState => _sharedState ??= OPassCliPS.SessionState.GetForCmdlet(this);

        internal void SetCliPath(string? cliPath = null)
        {
            string? onePasswordPath;

            if (cliPath == null)
            {
                WriteVerbose("Attempting to discover the path to 1Password CLI.");
                var onePasswordCli = new System.Management.Automation.SessionState().InvokeCommand.
                InvokeScript(@"(Get-Command -Name op.exe -CommandType Application).Path");

                onePasswordPath = onePasswordCli[0]?.ToString();

                if (onePasswordPath == null || onePasswordPath == string.Empty)
                {
                    string notFoundMessage = $"The 1Password CLI program (op.exe) as not found. Please check the installation and try again. Alternatively, you can provide the CLI path using the CliPath parameter.";
                    throw new ItemNotFoundException(notFoundMessage);
                }
            }
            else
            {
                WriteVerbose("User provided path to 1Password CLI.");
                onePasswordPath = cliPath;
            }

            FileInfo onePasswordCliPath = new(onePasswordPath);
            if (!File.Exists(onePasswordPath))
            {
                string notFoundMessage = $"The path '{onePasswordPath}' does not exist. Please check the installation and try again.";
                throw new ItemNotFoundException(notFoundMessage);
            }

            WriteVerbose($"Setting 1Password CLI Path session variable.");
            SharedState.CliPath = onePasswordCliPath;

        }

        internal static string ConvertFromSecureString(SecureString secureString)
        {
            string plainTextPassword = new NetworkCredential(string.Empty, secureString).Password;
            return plainTextPassword;
        }

        internal void CreateSession(string? cliPath = null, SecureString? secretKey = null, SecureString? password = null, string? domain = null, string? email = null, bool force = false)
        {
            if (SharedState.IsAuthenticated && SharedState.OPManager != null)
            {
                WriteVerbose("There is an active 1Password session.");
                if (force)
                {
                    WriteVerbose("A new session will be created, after signout from existing session.");
                    TerminateSession();
                }
                else
                {
                    WriteVerbose("A new session will not be created.");
                    return;
                }
            }
            else
            {
                SetCliPath(cliPath);
            }

            string appIntegrationVerboseText = "Creating 1Password session {0} app integration.";

            string? plainTextPassword;
            string? plainTextSecretKey;

            if (secretKey == null && password == null && domain == null && email == null)
            {
                WriteVerbose(string.Format(appIntegrationVerboseText, "with"));
                SharedState.OPManager = new(path: SharedState.CliPath.DirectoryName, executable: SharedState.CliPath.Name, appIntegrated: true);
                SharedState.IsAppIntegrated = true;
            }
            else if (secretKey != null && password != null && domain != null && email != null)
            {
                plainTextPassword = ConvertFromSecureString(password);
                plainTextSecretKey = ConvertFromSecureString(secretKey);

                WriteVerbose(string.Format(appIntegrationVerboseText, "without"));
                SharedState.OPManager = new(path: SharedState.CliPath.DirectoryName, executable: SharedState.CliPath.Name, appIntegrated: false);

                WriteVerbose($"Adding account for {email}.");
                SharedState.OPManager.AddAccount(domain, email, plainTextSecretKey, plainTextPassword);
            }
            else if (password != null && email != null)
            {
                WriteVerbose($"Signing in with account {email}.");
            }

        }

        internal void TerminateSession()
        {
            ValidateSession();
            WriteVerbose("Signing out of 1Password session.");
            SharedState.OPManager.SignOut(true);
            SharedState.IsAuthenticated = false;
            Thread.Sleep(500);
        }

        internal void ValidateSession()
        {
            if (SharedState.OPManager == null && !SharedState.IsAuthenticated)
            {
                ThrowTerminatingError(new ErrorRecord(
                    new InvalidOperationException("There is no active session to 1Password. Please use Connect-OPAccount to connect and create a new session."),
                    "NoActiveSession",
                    ErrorCategory.InvalidOperation,
                    null)
                );
            }
        }

    }
}

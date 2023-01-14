# OPassCliPS

PowerShell 7 Module for 1Password CLI

## Requirements

In order to use this module, you must have the following:

- 1Password CLI installed.
  - 1Password app integration is supported (at least on Windows 10, more testing to follow).
- PowerShell 7.2 or later.
  - The underlying .NET library used, [OnePassword.NET](https://github.com/jscarle/OnePassword.NET), is written for .NET 6, the same version used by PowerShell 7.2.

## Basic Usage

After importing the module, which is currently just the dll file and not the psd1 file, you will have access to the following commands:

```text
CommandType Name                  Version Source
----------- ----                  ------- ------
Cmdlet      Add-OPAccount         0.1.0.0 OPassCliPS
Cmdlet      Connect-OPAccount     0.1.0.0 OPassCliPS
Cmdlet      Disconnect-OPAccount  0.1.0.0 OPassCliPS
Cmdlet      Get-OPAccount         0.1.0.0 OPassCliPS
Cmdlet      Get-OPItem            0.1.0.0 OPassCliPS
Cmdlet      Get-OPSession         0.1.0.0 OPassCliPS
Cmdlet      Get-OPVault           0.1.0.0 OPassCliPS
Cmdlet      Get-OPVaultItem       0.1.0.0 OPassCliPS
Cmdlet      Remove-OPAccount      0.1.0.0 OPassCliPS
Cmdlet      Set-OPSessionVariable 0.1.0.0 OPassCliPS
```

### Connecting to 1Password

#### With App Integration

```powershell
Connect-OPAccount
```

#### Without App Integration

```powershell
$SecretKey = Read-Host -Prompt 'SecretKey' -AsSecureString
$Password =  Read-Host -Prompt 'Password' -AsSecureString
$Domain =  Read-Host -Prompt 'Domain'
$Email =  Read-Host -Prompt 'Email'
Connect-OPAccount -Domain $Domain -Email $Email -SecretKey $SecretKey -Password $Password
```

## Commands

These are mostly here to temporarily document which methods from OnePassword.NET this module supports.

## Accounts and Signin

### Account Methods

```csharp
public ImmutableList<Account> GetAccounts() {} // CacheBasedCmdlet class
public AccountDetails GetAccount(string account = "") {} // Get-OPAccount
public void AddAccount(string address, string email, string secretKey, string password, string shorthand = "") {} // Add-OPAccount
public void UseAccount(string account) {}
public void SignIn(string password) {} // Connect-OPAccount
public void SignIn() {} // Connect-OPAccount, added to support 1Password app integration
public void SignOut(bool all = false) {} // Disconnect-OPAccount
public ImmutableList<string> ForgetAccount(bool all = false) {} // Remove-OPAccount
```

### Account Cmdlets

- Connect-OPAccount
- Remove-OPAccount
- Disconnect-OPAccount
- Get-OPAccount
- Add-OPAccount
- Use-OPAccount (TBD)

## Vaults

### Vault Methods

```csharp
public ImmutableList<Vault> GetVaults() {} // CacheBasedCmdlet class
public VaultDetails GetVault(IVault vault) {} // Get-OPVault, Get-OPVaultItem
public ImmutableList<Vault> GetVaults(IGroup group) {}
public ImmutableList<Vault> GetVaults(IUser user) {}
public VaultDetails CreateVault(string name, string? description = null, VaultIcon icon = VaultIcon.Default, bool? allowAdminsToManage = null) {}
public void EditVault(IVault vault, string? name = null, string? description = null, VaultIcon icon = VaultIcon.Default, bool? travelMode = null) {}
public void DeleteVault(IVault vault) {}
public void GrantPermissions(IVault vault, IReadOnlyCollection<VaultPermission> permissions, IGroup group) {}
public void GrantPermissions(IVault vault, IReadOnlyCollection<VaultPermission> permissions, IUser user) {}
public void RevokePermissions(IVault vault, IReadOnlyCollection<VaultPermission> permissions, IGroup group) {}
public void RevokePermissions(IVault vault, IReadOnlyCollection<VaultPermission> permissions, IUser user) {}
```

### Vault Cmdlets

- Get-OPVault
- New-OPVault (TBD)
- Set-OPVault (TBD)
- Remove-OPVault (TBD)
- Set-OPVaultPermission (TBD)

## Items

### Item Methods

```csharp
public ImmutableList<Item> GetItems(IVault vault) {} // Get-OPVaultItem
public ImmutableList<Item> SearchForItems(IVault? vault = null, bool? includeArchive = null, bool? favorite = null, IReadOnlyCollection<Category>? categories = null, IReadOnlyCollection<string>? tags = null) {} // Get-OPItem, CacheBasedCmdlet class
public Item GetItem(IItem item, IVault vault) {} // Get-OPVaultItem
public Item SearchForItem(IItem item, IVault? vault = null, bool? includeArchive = null) {} // Get-OPItem
public Item CreateItem(Template template, IVault vault) {}
public Item EditItem(Item item, IVault vault) {}
public void ArchiveItem(IItem item, IVault vault) {}
public void DeleteItem(IItem item, IVault vault) {}
```

### Item Cmdlets

- Get-OPItem
- Get-OPVaultItem
- Add-OPItem (TBD)
- Remove-OPItem (TBD)
- Set-OPItem (TBD)

## Templates

### Template Methods

```csharp
public ImmutableList<TemplateInfo> GetTemplates() {}
public Template GetTemplate(ITemplate template) {}
public Template GetTemplate(Category category) {}
```

### Template Cmdlets

- Get-OPTemplate

## Users

### User Methods

```csharp
public ImmutableList<User> GetUsers() {}
public UserDetails GetUser(IUser user) {}
public UserDetails ProvisionUser(string name, string emailAddress, Language language = Language.Default) {}
public void ConfirmUser(IUser user) {} // this may not work
public void ConfirmAllUsers() {} // this may not work
public void EditUser(IUser user, string? name = null, bool? travelMode = null) {}
public void DeleteUser(IUser user) {}
public void SuspendUser(IUser user, int? deauthorizeDevicesDelay = null) {}
public void ReactivateUser(IUser user) {}
public ImmutableList<VaultUser> GetUsers(IVault vault) {}
public ImmutableList<GroupUser> GetUsers(IGroup group) {}
```

### User Cmdlets

## Groups

### Group Methods

```csharp
public ImmutableList<Group> GetGroups() {}
public GroupDetails GetGroup(IGroup group) {}
public GroupDetails CreateGroup(string name, string? description = null) {}
public void EditGroup(IGroup group, string? name = null, string? description = null) {}
public void DeleteGroup(IGroup group) {}
public void GrantAccess(IGroup group, IUser user, UserRole userRole = UserRole.Member) {}
public void RevokeAccess(IGroup group, IUser user) {}
public ImmutableList<VaultGroup> GetGroups(IVault vault) {}
public ImmutableList<UserGroup> GetGroups(IUser user) {}
```

### Group Cmdlets

## Common

### Common Methods

```csharp
public bool Update() {}
``

using OnePassword.Accounts;
using OnePassword.Items;
using OnePassword.Vaults;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OPassCliPS.Common
{
    internal sealed class Cache
    {
        public ImmutableList<Account> Accounts { get; set; }
        public ImmutableList<Vault> Vaults { get; set; }
        public ImmutableList<Item> Items { get; set; }
        public ImmutableList<Item> ArchivedItems { get; set; }
    }
}

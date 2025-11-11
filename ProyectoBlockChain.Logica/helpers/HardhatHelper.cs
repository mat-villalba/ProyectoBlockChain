using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using System.Linq;
using System.Threading.Tasks;

public static class HardhatHelper
{
    public static async Task<Account> GetAccountAsync(string nodeUrl)
    {
        // Conectamos a Hardhat nodo local
        var web3 = new Web3(nodeUrl);

        // Pedimos las cuentas locales (20 por defecto)
        var accounts = await web3.Eth.Accounts.SendRequestAsync();

        // Usamos la primera cuenta
        var localAccountAddress = accounts.First();

        // Esta private key corresponde a la cuenta (0) del nodo local.
        var localPrivateKey = "0xdf57089febbacf7ba0bc227dafbffa9fc08a93fdc68e1e42411a14efcf23656e";

        Console.WriteLine($"Usando cuenta local de Hardhat: {localAccountAddress}");

        return new Account(localPrivateKey);
    }
}

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

        // Usamos la cuenta 0 (owner del contrato)
        var localPrivateKey = "0xac0974bec39a17e36ba4a6b4d238ff944bacb478cbed5efcae784d7bf4f2ff80";

        Console.WriteLine($"Usando cuenta local de Hardhat: {accounts.First()}");

        return new Account(localPrivateKey);
    }
}

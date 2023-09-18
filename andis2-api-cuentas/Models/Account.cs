using System.Collections;

namespace andis2_api_cuentas.Models;
public class Account
{
    public int accountNumber { get; set; }
    public int accountBalance { get; set; }
    public string accountName { get; set; }
    public int ownerId { get; set; }
    public List<string> permissions { get; set; }
}

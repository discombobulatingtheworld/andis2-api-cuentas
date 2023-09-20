using System.ComponentModel.DataAnnotations;

namespace gRPC_andis2_api_cuentas.Models;
public class AccountModel
{
    [Key]
    public int accountNumber { get; set; }
    public int accountBalance { get; set; }
    public string accountName { get; set; } = "";
    public int ownerId { get; set; }
    public string permissions { get; set; } = "";
}

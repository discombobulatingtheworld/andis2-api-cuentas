using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace andis2_api_cuentas.Models;
public class Account
{
    [Key]
    public int accountNumber { get; set; }
    public int accountBalance { get; set; }
    public string accountName { get; set; }
    public int ownerId { get; set; }
    public string permissions { get; set; }
}

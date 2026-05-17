
using System.ComponentModel.DataAnnotations.Schema;

using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace Models;


[Table("Users")]
[PrimaryKey("UserId")]
public class User
{
    public int UserId { get; set; }
    public string Username { get; set; }
    [JsonIgnore]
    public string Password { get; set; }
    public string Email { get; set; }
}
[Table("Roles")]
[PrimaryKey("RoleId")]
public class Role
{
    public int RoleId { get; set; }
    public string RoleName { get; set; }
}
[Table("UserRoles")]
[PrimaryKey("UserId", "RoleId")]
public class UserRoles
{
    public int UserId { get; set; }
    public int RoleId { get; set; }
}

public class FaultContract
{
    public int FaultId { get; set; }
    public string FaultName { get; set; } = string.Empty; //Exception Class name
    public string FaultDescription { get; set; }// exception message
    public string FaultType { get; set; } // controller, repository, external call, 
    
}

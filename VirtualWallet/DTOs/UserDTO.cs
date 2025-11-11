namespace VirtualWallet.DTOs;


public class UserDTO
{
    public string Username { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
}

public class UserRegisterDTO
{
    public string Username { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
}

public class UserLoginDTO
{
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
}
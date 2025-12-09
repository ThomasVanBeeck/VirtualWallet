namespace VirtualWallet.Dtos;

public class UserDto
{
    public required string Username { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Email { get; set; }
}

public class UserRegisterDto
{
    public required string Username { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
}

public class UserLoginDto
{
    public required string Username { get; set; }
    public required string Password { get; set; }
}
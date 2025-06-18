namespace SnowballMobile.Models;

public class UserCartDto
{
    public string UserId { get; set; }
    public string CartId { get; set; }
    public string ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}
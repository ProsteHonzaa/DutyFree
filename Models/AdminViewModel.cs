namespace DutyFree.Models;

public class AdminViewModel
{
    public List<ProductModel> Products { get; set; }
    public List<UserModel> Users { get; set; }
    public List<OrderModel> Orders { get; set; }
    public UserModel CurrentUser { get; set; }
    public OrderModel CurrentUserOrder { get; set; }

}
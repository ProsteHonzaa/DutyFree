using DutyFree.Models;
using Dapper;
using System.Data;
using System.Collections.Generic;
using DutyFree.Controllers;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace DutyFree.Data;

public class Database
{
    private readonly IDbConnection _connection;

    public Database(IDbConnection connection)
    {
        _connection = connection;
    }

    public IEnumerable<ProductModel> GetProducts()
    {
        string query = "select * from dutyfree.dbo.products where IsDeleted = 0";
        return _connection.Query<ProductModel>(query).ToList();
    }

    public IEnumerable<ProductModel> GetProducts2()
    {
        string query = "select * from dutyfree.dbo.products where Quantity > 0 AND IsDeleted = 0";
        return _connection.Query<ProductModel>(query).ToList();
    }

    public List<OrderModel> GetOrders(int userid)
    {
        string query = "select * from dutyfree.dbo.orders where UserId = @UserId";
        var par = new { UserId = userid };
        return _connection.Query<OrderModel>(query, par).ToList();
    }
    
    public List<OrderModel> GetOrders2()
    {
        string query = "select * from dutyfree.dbo.orders";
        return _connection.Query<OrderModel>(query).ToList();
    }

    public void InsertProduct(string Name, int Price, int Quantity, string ImageUrl, int createdby)
    {
        string query = "INSERT INTO dutyfree.dbo.products (DateCreated, CreatedBy, DateUpdated, UpdatedBy, IsDeleted, Name, Price, Quantity, ImageUrl) VALUES (GETDATE(), @CreatedBy, GETDATE(), @CreatedBy, 0, @Name, @Price, @Quantity, @ImageUrl);";
        var par = new { Name = Name, Price = Price, Quantity = Quantity, ImageUrl = ImageUrl, CreatedBy = createdby};
        _connection.ExecuteScalar(query, par);
    }

    public void EditProduct(int updatedby,int productid, string name, int price, int quantity, string? imageurl)
    {
        if (imageurl == null)
        {
            string query = "UPDATE dutyfree.dbo.products set UpdatedBy = @UpdatedBy, Name = @Name, Price = @Price, Quantity = @Quantity, DateUpdated = GETDATE() WHERE ProductId = @ProductId";
            var par = new { UpdatedBy = updatedby,ProductId = productid, Name = name, Quantity = quantity, Price = price };
            _connection.Execute(query, par);
        }
        else
        {
            string query = "UPDATE dutyfree.dbo.products set UpdatedBy = @UpdatedBy, Name = @Name, Price = @Price, Quantity = @Quantity, DateUpdated = GETDATE(), ImageUrl = @ImageUrl WHERE ProductId = @ProductId";
            var par = new { UpdatedBy = updatedby,ProductId = productid, Name = name, Quantity = quantity, Price = price, ImageUrl = imageurl };
            _connection.Execute(query, par);
        }
    }

    public void DeleteProduct(int productId, int updatedby)
    {
        string query = "UPDATE dutyfree.dbo.Products SET IsDeleted = 1, Quantity = 0, DateUpdated = GETDATE(), UpdatedBy = @UpdatedBy WHERE ProductId=@ProductId";
        var par = new { ProductId = productId, UpdatedBy = updatedby };
        _connection.ExecuteScalar(query, par);
    }

    public UserModel GetUser(int id)
    {
        string query = "SELECT * FROM dutyfree.dbo.users WHERE UserId = @UserId";
        var par = new { UserId = id };
        var user = _connection.QuerySingleOrDefault<UserModel>(query, par);
        return user;
    }

    public void BuyProduct(int productId, int userId, string name, int price)
    {
        string query = "INSERT INTO dutyfree.dbo.orders (DateCreated, Name, Price, UserId, ProductId) VALUES (GETDATE(), @Name, @Price, @UserId, @ProductId); UPDATE dutyfree.dbo.products SET Quantity = Quantity - 1 WHERE ProductId = @ProductId;";
        var par = new { ProductId = productId, UserId = userId, Name = name, Price = price };
        _connection.ExecuteScalar(query, par);
    }

    public void DeleteOrder(int orderId)
    {
        string query = "DELETE FROM dutyfree.dbo.Orders WHERE OrderId=@OrderId";
        var par = new { OrderId = orderId };
        _connection.ExecuteScalar(query, par);
    }

    public ProductModel GetProduct(int ProductId)
    {
        string query = "SELECT * FROM dutyfree.dbo.products WHERE ProductId = @ProductId";
        var par = new { ProductId = ProductId };
        var product = _connection.QuerySingleOrDefault<ProductModel>(query, par);
        return product;
    }

    public IEnumerable<UserModel> GetUsers()
    {
        string query = "exec dbo.ProcUsers";
        return _connection.Query<UserModel>(query).ToList();
    }
}


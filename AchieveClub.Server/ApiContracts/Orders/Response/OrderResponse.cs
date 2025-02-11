namespace AchieveClub.Server.ApiContracts.Orders.Response;

public record OrderResponse(int Id, string ProductType, string ProductTitle, int Price, string Color, string Photo, DateTime OrderDate, string DeliveryStatus, string DeliveryColor);
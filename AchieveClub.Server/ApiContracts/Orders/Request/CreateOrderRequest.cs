namespace AchieveClub.Server.ApiContracts.Orders.Request;

public record CreateOrderRequest(int productId, int variantId);
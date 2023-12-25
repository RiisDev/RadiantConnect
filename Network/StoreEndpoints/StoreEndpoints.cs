using RadiantConnect.Network.StoreEndpoints.DataTypes;
namespace RadiantConnect.Network.StoreEndpoints;

public class StoreEndpoints
{
    public enum ItemType
    {
        Agents,
        Contracts,
        Sprays,
        GunBuddies,
        Cards,
        Skins,
        SkinVariants,
        Titles,
    }

    internal static ValorantNet Net = Initiator.InternalSystem.Net;
    internal static string Url = Initiator.InternalSystem.ClientData.PdUrl;
    internal static Dictionary<ItemType, string> ItemTypes = new()
    {
        { ItemType.Agents, "01bb38e1-da47-4e6a-9b3d-945fe4655707" },
        { ItemType.Contracts, "f85cb6f7-33e5-4dc8-b609-ec7212301948" },
        { ItemType.Sprays, "d5f120f8-ff8c-4aac-92ea-f2b5acbe9475" },
        { ItemType.GunBuddies, "dd3bf334-87f3-40bd-b043-682a57a8dc3a" },
        { ItemType.Cards, "3f296c07-64c3-494c-923b-fe692a4fa1bd" },
        { ItemType.Skins, "e7c63390-eda7-46e0-bb7a-a6abdacd2433" },
        { ItemType.SkinVariants, "3ad1b2b2-acdb-4524-852f-954a76ddae0a" },
        { ItemType.Titles, "de7caa6b-adf7-4588-bbd1-143831e786c6" },
    };


    public static async Task<Storefront?> FetchStorefront(string userId)
    {
        return await ValorantNet.GetAsync<Storefront>(Url, $"/store/v2/storefront/{userId}");
    }

    public static async Task<BalancesMain?> FetchBalances(string userId)
    {
        return await ValorantNet.GetAsync<BalancesMain>(Url, $"/store/v2/storefront/{userId}");
    }

    public static async Task<OwnedItem?> FetchOwnedItemByTypeAsync(ItemType type, string userId)
    {
        return await ValorantNet.GetAsync<OwnedItem>(Url, $"/store/v1/entitlements/{userId}/{ItemTypes[type]}");
    }
}
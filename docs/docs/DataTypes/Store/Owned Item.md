## OwnedItem

```csharp
public record Entitlement(
    [property: JsonPropertyName("TypeID")] string TypeID,
    [property: JsonPropertyName("ItemID")] string ItemID
);

public record OwnedItem(
    [property: JsonPropertyName("ItemTypeID")] string ItemTypeID,
    [property: JsonPropertyName("Entitlements")] IReadOnlyList<Entitlement> Entitlements
);
```
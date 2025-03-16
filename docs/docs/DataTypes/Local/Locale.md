## LocaleInternal Record

```csharp
public record LocaleInternal(
    [property: JsonPropertyName("locale")] string Locale,
    [property: JsonPropertyName("region")] string Region,
    [property: JsonPropertyName("webLanguage")] string WebLanguage,
    [property: JsonPropertyName("webRegion")] string WebRegion
);
```
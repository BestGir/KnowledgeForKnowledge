namespace API.Helpers;

public static class FileValidator
{
    private static readonly Dictionary<string, byte[]> MagicBytes = new()
    {
        { "image/jpeg", new byte[] { 0xFF, 0xD8, 0xFF } },
        { "image/png",  new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A } },
        { "image/webp", new byte[] { 0x52, 0x49, 0x46, 0x46 } },  // RIFF
        { "application/pdf", new byte[] { 0x25, 0x50, 0x44, 0x46 } },  // %PDF
    };

    private static readonly string[] AllowedImageTypes = ["image/jpeg", "image/png", "image/webp"];
    private static readonly string[] AllowedProofTypes = ["image/jpeg", "image/png", "image/webp", "application/pdf"];

    private const long MaxImageSize = 5 * 1024 * 1024;   // 5 MB
    private const long MaxProofSize = 10 * 1024 * 1024;  // 10 MB
    private const int MaxProofsPerUser = 20;

    public static async Task<string?> ValidateImageAsync(IFormFile file)
    {
        if (file.Length > MaxImageSize)
            return $"Размер изображения не должен превышать {MaxImageSize / 1024 / 1024} МБ.";

        if (!AllowedImageTypes.Contains(file.ContentType.ToLower()))
            return "Допустимые форматы: JPEG, PNG, WebP.";

        if (!await MatchesMagicBytesAsync(file, file.ContentType.ToLower()))
            return "Файл не соответствует заявленному типу.";

        return null;
    }

    public static async Task<string?> ValidateProofAsync(IFormFile file)
    {
        if (file.Length > MaxProofSize)
            return $"Размер файла не должен превышать {MaxProofSize / 1024 / 1024} МБ.";

        if (!AllowedProofTypes.Contains(file.ContentType.ToLower()))
            return "Допустимые форматы: JPEG, PNG, WebP, PDF.";

        if (!await MatchesMagicBytesAsync(file, file.ContentType.ToLower()))
            return "Файл не соответствует заявленному типу.";

        return null;
    }

    public static int GetMaxProofsPerUser() => MaxProofsPerUser;

    private static async Task<bool> MatchesMagicBytesAsync(IFormFile file, string contentType)
    {
        if (!MagicBytes.TryGetValue(contentType, out var magic))
            return true; // неизвестный тип — пропускаем

        var buffer = new byte[magic.Length];
        await using var stream = file.OpenReadStream();
        var read = await stream.ReadAsync(buffer.AsMemory(0, magic.Length));

        if (read < magic.Length) return false;

        // WebP дополнительно проверяем байты 8-11 (WEBP)
        if (contentType == "image/webp")
        {
            if (!buffer.Take(4).SequenceEqual(magic)) return false;
            var webpSig = new byte[4];
            stream.Seek(8, SeekOrigin.Begin);
            await stream.ReadAsync(webpSig.AsMemory(0, 4));
            return webpSig.SequenceEqual(new byte[] { 0x57, 0x45, 0x42, 0x50 }); // WEBP
        }

        return buffer.Take(magic.Length).SequenceEqual(magic);
    }
}

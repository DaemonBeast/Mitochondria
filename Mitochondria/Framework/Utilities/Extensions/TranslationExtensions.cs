namespace Mitochondria.Framework.Utilities.Extensions;

public static class TranslationExtensions
{
    public static string GetTranslation(this StringNames stringName)
    {
        return TranslationController.Instance.GetString(stringName);
    }
}
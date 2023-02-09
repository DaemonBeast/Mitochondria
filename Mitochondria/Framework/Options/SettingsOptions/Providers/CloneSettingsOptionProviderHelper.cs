using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Mitochondria.Framework.Options.SettingsOptions.Providers;

public static class CloneSettingsOptionProviderHelper
{
    public static GameObject? NumberOptionTemplate { get; set; }
    
    public static GameObject? ToggleOptionTemplate { get; set; }
    
    public static GameObject? StringOptionTemplate { get; set; }

    public static bool TryGetNewOption(
        GameObject? template,
        [NotNullWhen(true)] out GameObject? gameObject,
        Transform? parent = null)
    {
        if (template == null)
        {
            gameObject = null;
            return false;
        }

        gameObject = parent == null ? Object.Instantiate(template) : Object.Instantiate(template, parent);
        
        gameObject.SetActive(true);

        return true;
    }
}
using AmongUs.GameOptions;

namespace Mitochondria.Framework.Options.SettingsOptions;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
public class CustomSettingsOptionAttribute : Attribute
{
    public GameModes GameMode { get; }
    
    public int? Order { get; }
    
    public CustomSettingsOptionAttribute(GameModes gameMode = GameModes.Normal, int order = -1)
    {
        GameMode = gameMode;
        Order = order < 0 ? null : order;
    }
}
namespace Mitochondria.Framework.Utilities.Extensions;

public static class PlayerExtensions
{
    public static bool IsHost(this PlayerControl player)
        => AmongUsClient.Instance.GetHost().Character.PlayerId == player.PlayerId;
}
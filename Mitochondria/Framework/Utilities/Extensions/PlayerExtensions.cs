using System.Diagnostics.CodeAnalysis;
using Il2CppInterop.Runtime;
using Mitochondria.Framework.Roles;

namespace Mitochondria.Framework.Utilities.Extensions;

public static class PlayerExtensions
{
    public static bool IsHost(this PlayerControl player)
        => AmongUsClient.Instance.GetHost().Character.PlayerId == player.PlayerId;

    public static CustomRoleBehaviour GetCustomRoleBehaviour(this PlayerControl player)
    {
        return player.TryGetComponent(Il2CppType.Of<CustomRoleBehaviour>(), out var component)
            ? component.Cast<CustomRoleBehaviour>()
            : player.gameObject.AddComponent<CustomRoleBehaviour>();
    }

    public static bool TryGetCustomRoleBehaviour(
        this PlayerControl player,
        [NotNullWhen(true)] out CustomRoleBehaviour? customRoleBehaviour)
    {
        var result = player.TryGetComponent(Il2CppType.Of<CustomRoleBehaviour>(), out var component);
        customRoleBehaviour = result ? component.Cast<CustomRoleBehaviour>() : null;

        return result;
    }
}
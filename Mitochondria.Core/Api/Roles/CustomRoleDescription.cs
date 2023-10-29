using Reactor.Localization.Utilities;
using UnityEngine;

namespace Mitochondria.Core.Api.Roles;

public abstract class CustomRoleDescription : ICustomRoleDescription
{
    public abstract string Title { get; }

    public abstract string CutsceneDescription { get; }

    public abstract string RoleMenuDescription { get; }

    public virtual string TaskListDescription { get; }

    public StringNames TitleName => _titleName ??= CustomStringName.CreateAndRegister(Title);

    public StringNames CutsceneDescriptionName =>
        _cutsceneDescriptionName ??= CustomStringName.CreateAndRegister(CutsceneDescription);

    public StringNames RoleMenuDescriptionName =>
        _roleMenuDescriptionName ??= CustomStringName.CreateAndRegister(RoleMenuDescription);

    public StringNames TaskListDescriptionName =>
        _taskListDescriptionName ??= CustomStringName.CreateAndRegister(TaskListDescription);

    public abstract Color Color { get; }

    private StringNames? _titleName;
    private StringNames? _cutsceneDescriptionName;
    private StringNames? _roleMenuDescriptionName;
    private StringNames? _taskListDescriptionName;

    protected CustomRoleDescription()
    {
        TaskListDescription = string.Empty;
    }
}
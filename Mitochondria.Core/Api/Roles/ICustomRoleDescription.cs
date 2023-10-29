using UnityEngine;

namespace Mitochondria.Core.Api.Roles;

public interface ICustomRoleDescription
{
    public string Title { get; }

    public string CutsceneDescription { get; }

    public string RoleMenuDescription { get; }
    
    public string TaskListDescription { get; }
    
    public StringNames TitleName { get; }
    
    public StringNames CutsceneDescriptionName { get; }
    
    public StringNames RoleMenuDescriptionName { get; }
    
    public StringNames TaskListDescriptionName { get; }
    
    public Color Color { get; }
}
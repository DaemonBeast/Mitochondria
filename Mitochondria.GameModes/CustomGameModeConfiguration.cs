namespace Mitochondria.GameModes;

public class CustomGameModeConfigurationBuilder
{
    private CustomGameModeConfiguration.PlayersConfiguration _players;
    private CustomGameModeConfiguration.RoleSelectionConfiguration _roles;
    private CustomGameModeConfiguration.IntroCutsceneConfiguration _introCutscene;
    private CustomGameModeConfiguration.TasksConfiguration _tasks;

    public PlayersConfigurationBuilder Players => new(this);
    public RoleSelectionConfigurationBuilder Roles => new(this);
    public IntroCutsceneConfigurationBuilder IntroCutscene => new(this);
    public TasksConfigurationBuilder Tasks => new(this);

    public CustomGameModeConfigurationBuilder(CustomGameModeConfiguration originalConfiguration)
    {
        _players = originalConfiguration.Players;
        _roles = originalConfiguration.Roles;
        _introCutscene = originalConfiguration.IntroCutscene;
        _tasks = originalConfiguration.Tasks;
    }

    public class PlayersConfigurationBuilder
    {
        private readonly CustomGameModeConfigurationBuilder _builder;

        public PlayersConfigurationBuilder(CustomGameModeConfigurationBuilder builder)
        {
            _builder = builder;
        }

        public CustomGameModeConfigurationBuilder MinimumOf(int minPlayers)
        {
            _builder._players = _builder._players with { MinPlayers = minPlayers };
            return _builder;
        }

        public CustomGameModeConfigurationBuilder MaximumOf(int maxPlayers)
        {
            _builder._players = _builder._players with { MaxPlayers = maxPlayers };
            return _builder;
        }
    }

    public class RoleSelectionConfigurationBuilder
    {
        private readonly CustomGameModeConfigurationBuilder _builder;

        public RoleSelectionConfigurationBuilder(CustomGameModeConfigurationBuilder builder)
        {
            _builder = builder;
        }

        public CustomGameModeConfigurationBuilder Enable()
        {
            // ReSharper disable once WithExpressionModifiesAllMembers
            _builder._roles = _builder._roles with { Enabled = true };
            return _builder;
        }

        public CustomGameModeConfigurationBuilder Disable()
        {
            // ReSharper disable once WithExpressionModifiesAllMembers
            _builder._roles = _builder._roles with { Enabled = false };
            return _builder;
        }
    }

    public class IntroCutsceneConfigurationBuilder
    {
        private readonly CustomGameModeConfigurationBuilder _builder;

        public IntroCutsceneConfigurationBuilder(CustomGameModeConfigurationBuilder builder)
        {
            _builder = builder;
        }

        public CustomGameModeConfigurationBuilder ShowEmblem()
        {
            _builder._introCutscene = _builder._introCutscene with { ShowEmblem = true };
            return _builder;
        }

        public CustomGameModeConfigurationBuilder HideEmblem()
        {
            _builder._introCutscene = _builder._introCutscene with { ShowEmblem = false };
            return _builder;
        }

        public CustomGameModeConfigurationBuilder ShowTeamAndRole()
        {
            _builder._introCutscene = _builder._introCutscene with { ShowTeamAndRole = true };
            return _builder;
        }

        public CustomGameModeConfigurationBuilder HideTeamAndRole()
        {
            _builder._introCutscene = _builder._introCutscene with { ShowTeamAndRole = false };
            return _builder;
        }

        public CustomGameModeConfigurationBuilder Append<TCustomGameModeIntroCutscene>()
            where TCustomGameModeIntroCutscene : BaseCustomGameModeIntroCutscene
        {
            _builder._introCutscene = _builder._introCutscene with
            {
                IntroCutsceneTypes = _builder._introCutscene.IntroCutsceneTypes
                    .Append(typeof(TCustomGameModeIntroCutscene)).ToArray()
            };

            return _builder;
        }
    }

    public class TasksConfigurationBuilder
    {
        private readonly CustomGameModeConfigurationBuilder _builder;

        public TasksConfigurationBuilder(CustomGameModeConfigurationBuilder builder)
        {
            _builder = builder;
        }

        public CustomGameModeConfigurationBuilder Enable()
        {
            // ReSharper disable once WithExpressionModifiesAllMembers
            _builder._tasks = _builder._tasks with { Enabled = true };
            return _builder;
        }

        public CustomGameModeConfigurationBuilder Disable()
        {
            // ReSharper disable once WithExpressionModifiesAllMembers
            _builder._tasks = _builder._tasks with { Enabled = false };
            return _builder;
        }
    }

    internal CustomGameModeConfiguration Build()
        => new(_players, _roles, _introCutscene, _tasks);
}

public record CustomGameModeConfiguration(
    CustomGameModeConfiguration.PlayersConfiguration Players,
    CustomGameModeConfiguration.RoleSelectionConfiguration Roles,
    CustomGameModeConfiguration.IntroCutsceneConfiguration IntroCutscene,
    CustomGameModeConfiguration.TasksConfiguration Tasks)
{
    public static CustomGameModeConfiguration Default { get; } = new(
        new PlayersConfiguration(4, 15),
        new RoleSelectionConfiguration(true),
        new IntroCutsceneConfiguration(true, true, Array.Empty<Type>()),
        new TasksConfiguration(true));

    public record PlayersConfiguration(
        int MinPlayers,
        int MaxPlayers);

    public record RoleSelectionConfiguration(
        bool Enabled);

    public record IntroCutsceneConfiguration(
        bool ShowEmblem,
        bool ShowTeamAndRole,
        Type[] IntroCutsceneTypes);

    public record TasksConfiguration(
        bool Enabled);
}

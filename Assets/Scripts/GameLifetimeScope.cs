using TreasureHunters;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class GameLifetimeScope : LifetimeScope
{
    [SerializeField] private GenerateLevel _generateLevel;
    [SerializeField] private PlayerMovement _playerMovement;

    protected override void Configure(IContainerBuilder builder)
    {
        builder.Register<Game>(Lifetime.Singleton);
        builder.RegisterComponent(_generateLevel);
        builder.RegisterComponent(_playerMovement);
    }
}

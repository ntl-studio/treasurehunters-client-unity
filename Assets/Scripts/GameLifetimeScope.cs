using TreasureHunters;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class GameLifetimeScope : LifetimeScope
{
    [SerializeField] private BoardView _boardView;
    [SerializeField] private PlayerMovement _playerMovement;

    protected override void Configure(IContainerBuilder builder)
    {
        builder.Register<Game>(Lifetime.Singleton);
        builder.RegisterComponent(_boardView);
        builder.RegisterComponent(_playerMovement);
    }
}

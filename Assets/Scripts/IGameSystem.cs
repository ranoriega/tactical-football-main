using System;

public interface IGameSystem
{
    event Action OnReady;
    void Initialize();
}

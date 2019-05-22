using UnityEngine;

public class BoltGameObjectEntity<T> : Bolt.EntityBehaviour<T> where T : IGameObjectState
{
    public override void Attached()
    {
        base.Attached();

        state.AddCallback("IsGameObjectActive", RefreshGameObjectActiveness);
        
        if (entity.IsOwner)
        {
            state.IsGameObjectActive = gameObject.activeSelf;
        }
        else
        {
            RefreshGameObjectActiveness();
        }
    }

    void RefreshGameObjectActiveness()
    {
        if (gameObject.activeSelf != state.IsGameObjectActive)
        {
            gameObject.SetActive(state.IsGameObjectActive);
        }
    }
}

public class BoltGameObjectEntity : BoltGameObjectEntity<IGameObjectState>
{
}
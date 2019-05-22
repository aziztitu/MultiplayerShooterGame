using UnityEngine;

public class GameManagerVisualizer : MonoBehaviour
{
    [SerializeField] private GameManagerOld _gameManager;

    void Awake()
    {
        _gameManager = GameManagerOld.Instance;
    }
}
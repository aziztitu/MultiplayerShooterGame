using UnityEngine;

public class GameManagerVisualizer : MonoBehaviour
{
    [SerializeField] private GameManager _gameManager;

    void Awake()
    {
        _gameManager = GameManager.Instance;
    }
}
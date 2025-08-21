using UnityEngine;
using Zenject;

public class PlayerController : MonoBehaviour
{
    public PlayerData PlayerData { get; set; }
    public RigidbodyController PlayerMovement { get; set; }
    public PlayerCustomization PlayerCustomization { get; set; }

    private bool isDead = false;
    public bool IsDead => isDead;

    public GameEvent _gameEvent;

    // [Inject]
    // public void Construct(GameEvent gameEvent)
    // {
    //     _gameEvent = gameEvent;
    //     Debug.Log("GameEvent Instalado");
    // }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("DeathBox"))
        {
            isDead = true;
            _gameEvent.onPlayerDeath.Invoke();
        }
    }
}

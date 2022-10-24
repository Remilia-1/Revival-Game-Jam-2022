using System;
using System.Threading.Tasks;
using UnityEngine;

public class LevelEndPlatform : MonoBehaviour
{
    public event Action onLevelEndEntered;

    [SerializeField] private LayerMask playerMask;
    [SerializeField] private SphereCollider playerCheckCollider;
    [SerializeField] private float checkDelay;

    private Collider[] playerSearchBuffer;

    private bool coroutinePlayerCheck = true;



    private void Awake()
    {
        playerSearchBuffer = new Collider[1];

        CheckPlayer();
    }

    private async void CheckPlayer()
    {
        int delayMS = (int)(checkDelay * 1000f);
        Vector3 position = transform.position + playerCheckCollider.center;
        float radius = playerCheckCollider.radius;

        while (coroutinePlayerCheck)
        {
            await Task.Delay(delayMS);

            playerSearchBuffer[0] = null;

            Physics.OverlapSphereNonAlloc(position, radius, playerSearchBuffer, playerMask, QueryTriggerInteraction.Ignore);

            if (playerSearchBuffer[0] != null)
            {
                onLevelEndEntered?.Invoke();
            }
        }
    }
}

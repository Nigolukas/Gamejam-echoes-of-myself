using UnityEngine;

public class FollowPLayer : MonoBehaviour
{
    PlayerController player;
    [SerializeField] private Vector3 offset;
    void Awake()
    {
        player = FindFirstObjectByType<PlayerController>();
    }

    void Update()
    {
        if(player.hasControl == true)
        {
            transform.position = player.transform.position + offset;
        }
        else
        {
            transform.position = player.activeShadow.transform.position + offset;
        }
    }
}

using UnityEngine;

public class CameraFocus : MonoBehaviour
{
    private PlayerController playerController;
    [SerializeField] private Transform player;
    [SerializeField] private float smoothTime = 0.4f;

    private Transform focusOverride;   
    private Vector3 velocity;

    private void Awake()
    {
        playerController = player.GetComponent<PlayerController>();
    }
    private Transform GetTarget()
    {
        if (focusOverride != null) return focusOverride;

        if (playerController != null && playerController.activeShadow != null)
            return playerController.activeShadow.transform;

        return player;
    }

    void LateUpdate()
    {
        Transform target = GetTarget();
        if (target == null) return;

        Vector3 desired = new Vector3(target.position.x, target.position.y, transform.position.z);

        transform.position = Vector3.SmoothDamp(
            transform.position, desired, ref velocity,
            smoothTime, Mathf.Infinity, Time.unscaledDeltaTime);
    }

    public void SnapToTarget()
    {
        Transform target = GetTarget();
        if (target == null) return;

        velocity = Vector3.zero;
        transform.position = new Vector3(target.position.x, target.position.y, transform.position.z);
    }

    public void FocusOn(Transform target) => focusOverride = target;
    public void ReturnToPlayer() => focusOverride = null;
}
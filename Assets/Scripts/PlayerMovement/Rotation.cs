using UnityEngine;
using Input;

public class Rotation : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 500f;

    private Vector2 directionToRotate;
    private float angle;
    private float velocity;
    private Quaternion targetRotation;

    private void Update()
    {
        directionToRotate = (Vector2)Camera.main.ScreenToWorldPoint(InputHandler.LookDirection) - (Vector2)transform.position;

        // Если курсор находится прямо на позиции игрока, не крутим
        if (directionToRotate.sqrMagnitude < 0.001f) return;

        angle = Mathf.Atan2(directionToRotate.y, directionToRotate.x) * Mathf.Rad2Deg;
        targetRotation = Quaternion.Euler(0f, 0f, angle);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
}

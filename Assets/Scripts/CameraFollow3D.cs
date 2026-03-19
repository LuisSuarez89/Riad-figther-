using UnityEngine;

public class CameraFollow3D : MonoBehaviour
{
    public Transform target;
    public Vector3 localOffset = new Vector3(0, 5f, -10f);
    public float smoothSpeed = 10f;
    public float rotationSmoothSpeed = 8f;

    void LateUpdate()
    {
        if (target != null)
        {
            // Posición deseada basada en la rotación actual del coche
            Vector3 desiredPosition = target.TransformPoint(localOffset);
            
            // Movimiento suave hacia la posición
            transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

            // Rotación deseada (misma que el coche + 15 grados hacia abajo para enfocar la pista)
            Quaternion desiredRotation = target.rotation * Quaternion.Euler(15f, 0, 0);
            
            // Rotación suave hacia el nuevo ángulo
            transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, rotationSmoothSpeed * Time.deltaTime);
        }
    }
}

using UnityEngine;

public class DebugGizmos : MonoBehaviour
{
    [Tooltip("Which shape to draw (support Sphere or Cube)")]
    public PrimitiveType type = PrimitiveType.Sphere;

    [Tooltip("Which color to draw")]
    public Color color = Color.magenta;

    [Tooltip("Which size for cube, or radius for sphere")]
    public float size = 2f;

    private void OnDrawGizmos()
    {
        Gizmos.color = color;

        switch (type)
        {
            case PrimitiveType.Sphere:
            Gizmos.DrawSphere(transform.position, size);
            break;

            case PrimitiveType.Cube:
            Gizmos.DrawCube(transform.position, Vector3.one * size);
            break;
        }
    }
}
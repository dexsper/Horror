using UnityEngine;

public class Surface
{
    /// <summary>
    /// The normal of the surface
    /// </summary>
    public Vector3 Normal { get; private set; }

    /// <summary>
    /// The point where contact between motor and surface was made
    /// </summary>
    public Vector3 Point { get; private set; }

    /// <summary>
    /// The transform of the surface
    /// </summary>
    public Transform Transform { get; private set; }

    /// <summary>
    /// The angle of the normal relative to up in world space
    /// </summary>
    public float Angle
    {
        get { return Vector3.Angle(Vector3.up, Normal); }
    }

    /// <summary>
    /// Copies the values of another surface
    /// </summary>
    /// <param name="other">Surface to copy</param>
    public void Copy(Surface other)
    {
        Normal = other.Normal;
        Point = other.Point;
        Transform = other.Transform;
    }


    public Surface(Collision collision)
    {
        ContactPoint contact = collision.GetContact(0);

        Normal = contact.normal;
        Point = contact.point;
        Transform = collision.transform;
    }
}

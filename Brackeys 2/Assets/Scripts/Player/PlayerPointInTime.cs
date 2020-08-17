using UnityEngine;

public struct PlayerPointInTime
{
    public Vector3 position;
    public Quaternion rotation;

    public float health;

    public PlayerPointInTime(Vector3 position, Quaternion rotation, float health)
    {
        this.position = position;
        this.rotation = rotation;
        this.health = health;
    }
}

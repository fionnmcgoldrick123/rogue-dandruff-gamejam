using UnityEngine;

public interface IHittable
{
    /// <summary>
    /// Called when this object is hit by a bullet
    /// </summary>
    void OnHit();
}

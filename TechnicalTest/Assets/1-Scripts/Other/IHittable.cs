using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHittable // Interface definition in order to apply the same method on different objects in different ways
{
    void OnHit(Vector3 hitPoint, float damage, Vector3 hitForce);

}

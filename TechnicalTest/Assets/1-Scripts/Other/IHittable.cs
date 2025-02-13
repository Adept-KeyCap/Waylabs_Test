using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHittable
{
    void OnHit(Vector3 hitPoint, float damage, Vector3 hitForce);

}

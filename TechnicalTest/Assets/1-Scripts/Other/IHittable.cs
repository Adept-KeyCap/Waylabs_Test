using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHittable
{
    void OnHit(Vector3 hitPoint, Vector3 hitForce);

}

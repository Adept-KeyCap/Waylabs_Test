using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float destroyAfter;
    [SerializeField] private float forceSpeed;
    [SerializeField] private LayerMask mask;

    void Start()
    {
        //Start destroy timer
        StartCoroutine(DestroyAfter());
    }

    public void GetWeaponStat(float speed)
    {
        forceSpeed = speed;
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Ignore collisions with other projectiles.
        if (collision.gameObject.GetComponent<Bullet>() != null)
            return;

        IHittable hittableObj = collision.collider.GetComponent<IHittable>();
        if (hittableObj != null)
        {
            Vector3 forceDir = transform.forward * forceSpeed/6;
            hittableObj.OnHit(collision.collider.ClosestPoint(transform.position), forceDir);
            Destroy(gameObject);
        }
    }


    private IEnumerator DestroyAfter()
    {
        //Wait for set amount of time
        yield return new WaitForSeconds(destroyAfter);
        //Destroy bullet object
        Destroy(gameObject);
    }
}

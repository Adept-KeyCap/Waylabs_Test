using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float destroyAfter;
    [SerializeField] private LayerMask mask;

    private float forceSpeed;
    private float damage;

    void Start()
    {
        //Start destroy timer
        StartCoroutine(DestroyAfter());
    }

    public void GetWeaponStat(float speed, float weaponDamage) // Read the weapon stats in order to apply it to the IHittable object
    {
        forceSpeed = speed;
        damage = weaponDamage;
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
            hittableObj.OnHit(collision.collider.ClosestPoint(transform.position), damage, forceDir);
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

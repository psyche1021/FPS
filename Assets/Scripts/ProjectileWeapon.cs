using UnityEngine;

public class ProjectileWeapon : Weapon
{
    public GameObject projectilePrefab;
    public float projectileAngle = 30f;
    public float projectileForce = 10f;
    public float projectileTime = 5f;

    protected override void Fire()
    {
        ProjectileFire();
    }

    void ProjectileFire()
    {
        Camera cam = Camera.main;

        Vector3 forward = cam.transform.forward;
        Vector3 up = cam.transform.up;

        Vector3 direction = forward + up * Mathf.Tan(projectileAngle * Mathf.Deg2Rad);

        direction.Normalize();
        direction *= projectileForce;

        GameObject go = Instantiate(projectilePrefab);
        go.transform.position = firingPosition.position;
        go.GetComponent<Rigidbody>().AddForce(direction, ForceMode.Impulse);
        go.GetComponent<Bomb>().time = projectileTime;
    }
}
using UnityEngine;
using System.Collections;
using TMPro;

public class Weapon : MonoBehaviour
{
    public GameObject trailPrefab;
    public Transform firingPosition;
    public GameObject particlePrefab;
    public AudioClip gunShotSound;

    public TextMeshProUGUI bulletText;
    public int currentBullet = 8;
    public int totalBullet = 32;
    public int maxBulletInMagazine = 8;
    
    public float damage = 1f;

    Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        bulletText.text = currentBullet + " / " + totalBullet;
    }

    public void FireWeapon()
    {
        if (currentBullet > 0)
        {
            if (anim != null)
            {
                if (anim.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
                {
                    anim.SetTrigger("Fire");
                    currentBullet--;
                    Fire();
                }
            }
            else
            {
                currentBullet--;
                Fire();
            }
        }
    }

    protected virtual void Fire()
    {
        RayCastFire();
    }


    public void ReloadWeapon()
    {
        if (totalBullet > 0)
        {
            if (anim != null)
            {
                if (anim.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
                {
                    anim.SetTrigger("Reload");
                    Reload();
                }
            }
            else
            {
                Reload();
            }
        }
    }

    void Reload()
    {
        if (totalBullet >= maxBulletInMagazine - currentBullet)
        {
            totalBullet -= maxBulletInMagazine - currentBullet;
            currentBullet = maxBulletInMagazine;
        }
        else
        {
            currentBullet = maxBulletInMagazine;
            totalBullet = 0;
        }
    }

    void RayCastFire()
    {
        GetComponent<AudioSource>().PlayOneShot(gunShotSound);

        Camera cam = Camera.main;

        RaycastHit hit;
        Ray r = cam.ViewportPointToRay(Vector3.one / 2); // 카메라의 정중앙

        Vector3 hitPosition = r.origin + r.direction * 200; // 카메라 방향 200m

        if (Physics.Raycast(r, out hit, 1000)) // 카메라 중앙으로 1000m 레이를 발사하고, 그 결과(bool)를 hit에 넣음
        {
            hitPosition = hit.point; // 아무것도 부딪히지 않았을 시 hitPosition까지 레이저 도달

            GameObject particle = Instantiate(particlePrefab);
            particle.transform.position = hitPosition;
            particle.transform.forward = hit.normal;

            if (hit.collider.tag == "Enemy")
            {
                hit.collider.GetComponent<Health>().Damage(damage);
            }
        }

        // 목적지까지 레이 그리기
        GameObject go = Instantiate(trailPrefab);
        Vector3[] pos = new Vector3[] { firingPosition.position, hitPosition };
        go.GetComponent<LineRenderer>().SetPositions(pos);

        Destroy(go, 1.0f);
    }
}

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
        Ray r = cam.ViewportPointToRay(Vector3.one / 2); // ī�޶��� ���߾�

        Vector3 hitPosition = r.origin + r.direction * 200; // ī�޶� ���� 200m

        if (Physics.Raycast(r, out hit, 1000)) // ī�޶� �߾����� 1000m ���̸� �߻��ϰ�, �� ���(bool)�� hit�� ����
        {
            hitPosition = hit.point; // �ƹ��͵� �ε����� �ʾ��� �� hitPosition���� ������ ����

            GameObject particle = Instantiate(particlePrefab);
            particle.transform.position = hitPosition;
            particle.transform.forward = hit.normal;

            if (hit.collider.tag == "Enemy")
            {
                hit.collider.GetComponent<Health>().Damage(damage);
            }
        }

        // ���������� ���� �׸���
        GameObject go = Instantiate(trailPrefab);
        Vector3[] pos = new Vector3[] { firingPosition.position, hitPosition };
        go.GetComponent<LineRenderer>().SetPositions(pos);

        Destroy(go, 1.0f);
    }
}

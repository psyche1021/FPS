using UnityEngine;

public class Bomb : MonoBehaviour
{
    public float time;
    public float damage = 3f;
    public AudioClip explodeSound;

    void Update()
    {
        time -= Time.deltaTime;
        if (time < 0)
        {
            GetComponent<Animator>().SetTrigger("Explode");
            Destroy(gameObject, 2f);
        }
    }

    public void PlaySound()
    {
        GetComponent<AudioSource>().PlayOneShot(explodeSound);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            other.GetComponent<Health>().Damage(damage);
        }
    }
}

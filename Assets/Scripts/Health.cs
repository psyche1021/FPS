using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    public float hp = 3;
    public float maxHp = 3;
    public float invincibleTime = 0.5f; // 중복피격방지 무적시간
    float lastDamagedTime;
    public Image hpGauge;

    public AudioClip dieSound;
    public AudioClip hurtSound;

    IHealthListener healthListener;

    void Start()
    {
        // IHealthLister가 Enemy에 붙어 있으니 거기있는 IHealthLister를 할당함
        // Enemy와 Health가 동시에 같은 오브젝트에 붙어있어서 가능
        healthListener = GetComponent<IHealthListener>();
    }

    public void Damage(float damage)
    {
        if (hp > 0 && lastDamagedTime + invincibleTime < Time.time)
        {
            hp -= damage;

            if (hpGauge != null)
            {
                hpGauge.fillAmount = hp / maxHp;
            }

            lastDamagedTime = Time.time;

            if (hp <= 0)
            {
                GetComponent<CapsuleCollider>().enabled = false;

                if (dieSound != null) GetComponent<AudioSource>().PlayOneShot(dieSound);

                if (healthListener != null)
                {
                    healthListener.OnDie();
                }
            }
            else
            {
                if (hurtSound != null) GetComponent<AudioSource>().PlayOneShot(hurtSound);
            }
        }
    }

    public interface IHealthListener 
    {
        void OnDie();
    }
}

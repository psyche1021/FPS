using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    public float hp = 3;
    public float maxHp = 3;
    public float invincibleTime = 0.5f; // �ߺ��ǰݹ��� �����ð�
    float lastDamagedTime;
    public Image hpGauge;

    public AudioClip dieSound;
    public AudioClip hurtSound;

    IHealthListener healthListener;

    void Start()
    {
        // IHealthLister�� Enemy�� �پ� ������ �ű��ִ� IHealthLister�� �Ҵ���
        // Enemy�� Health�� ���ÿ� ���� ������Ʈ�� �پ��־ ����
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

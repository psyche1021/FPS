using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour, Health.IHealthListener
// Enemy Ŭ������ Health.IHelathListner �������̽��� �����ؾ� �Ѵ�.
// ����� C#�� ����� 1�� �ۿ� ���� ���� �� ������ �������̽��� ����
// ���⼭�� �̹� MonoBehaivior��� Ŭ������ ��ӹ޾Ҵٰ� �� �� �ְ� �߰��� �������̽��� �����ؾ��Ѵٰ� ���
{
    enum State
    {
        Idle, Follow, Attack, Die
    }

    GameObject player;
    NavMeshAgent agent;
    Animator anim;
    new AudioSource audio;

    State state;
    float currnetStateTime;
    public float timeForNextState = 2f;

    void Start()
    {
        player = GameObject.FindWithTag("Player");
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        audio = GetComponent<AudioSource>();

        state = State.Idle;
        currnetStateTime = timeForNextState;
    }

    void Update()
    {
        switch (state)
        {
            case State.Idle:
                currnetStateTime -= Time.deltaTime;
                if (currnetStateTime < 0)
                {
                    // �÷��̾�� �� ���� �� �Ÿ�
                    float dist = (player.transform.position - transform.position).magnitude;

                    if (dist <= 2.0f)
                    {
                        StartAttack();
                    }
                    else
                    {
                        StartFollow();
                    }
                }
                break;

            case State.Follow:
                // �÷��̾� �� �Ÿ��� 2.0m �̳��̰ų� �������� ��ΰ� ������
                if (agent.remainingDistance <= 2.0f || !agent.hasPath)
                {
                    StartIdle();
                }
                break;

            case State.Attack:
                currnetStateTime -= Time.deltaTime;
                if (currnetStateTime < 0)
                {
                    StartIdle();
                }
                break;
        }
    }

    void StartIdle()
    {
        audio.Stop();
        state = State.Idle;
        currnetStateTime = timeForNextState;
        agent.isStopped = true;
        anim.SetTrigger("Idle");
    }

    void StartFollow()
    {
        audio.Play();
        state = State.Follow;
        agent.destination = player.transform.position;
        agent.isStopped = false;
        anim.SetTrigger("Run");
    }

    void StartAttack()
    {
        state = State.Attack;
        currnetStateTime = timeForNextState;
        anim.SetTrigger("Attack");
    }

    public void OnDie()
    {
        // �׷��Ƿ� Health.IHelathListner�� OnDie��� �Լ��� �ʼ��� ����.
        // �������� ������ ���� �߻�
        state = State.Die;
        agent.isStopped = true;
        anim.SetTrigger("Die");
        Invoke("DestroyThis", 2);
    }

    void DestroyThis()
    {
        GameManager.Instance.EnemyDie();
        Destroy(gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<Health>().Damage(1);
        }
    }
}

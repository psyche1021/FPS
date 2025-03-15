using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour, Health.IHealthListener
// Enemy 클래스는 Health.IHelathListner 인터페이스를 구현해야 한다.
// 참고로 C#은 상속을 1개 밖에 받지 못해 그 단점을 인터페이스로 보완
// 여기서는 이미 MonoBehaivior라는 클래스를 상속받았다고 볼 수 있고 추가로 인터페이스도 구현해야한다고 명시
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
                    // 플레이어와 적 유닛 간 거리
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
                // 플레이어 간 거리가 2.0m 이내이거나 갈수없는 경로가 없을때
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
        // 그러므로 Health.IHelathListner의 OnDie라는 함수를 필수로 구현.
        // 구현하지 않으면 에러 발생
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

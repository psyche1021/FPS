using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    static GameManager instance = null;
    public static GameManager Instance
    {
        get { return instance; }
    }

    public bool isPlaying;
    public GameObject gameoverCanvas;
    public TextMeshProUGUI text;
    public int enemyNumber = 3;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        isPlaying = true;
    }

    public void PlayerDie()
    {
        isPlaying = false;
        text.text = "»ç¸Á...";
        Invoke("GameEnd", 2f);
    }

    public void AgainPressed()
    {
        SceneManager.LoadScene(0);
    }

    public void QuitPressed()
    {
        Debug.Log("Quit Pressed");
        Application.Quit();
    }

    public void EnemyDie()
    {
        enemyNumber--;
        if (enemyNumber <= 0)
        {
            isPlaying = false;
            text.text = "½Â¸®!";
            Invoke("GameEnd", 1f);
        }
    }

    void GameEnd()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
        gameoverCanvas.SetActive(true);
    }
}

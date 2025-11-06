using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using static NoiseSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("참조용")]
    [SerializeField] private GhostManager ghostManager;
    [SerializeField] private NoiseSystem noiseSystem;
    [SerializeField] private MapControlloer mapControlloer;
    [SerializeField] private trapManager trapManager;
    [SerializeField] private ItemMnanger itemMnanger;


    [Header("게임 흐름상태")]
    public bool isPlaying = false;
    public bool isHunting = false;
    public bool isGameOver = false;

    [Header("유령및 탈출 관련")]
    public static float ghostSpeedMulitplier = 1.0f;
    public static string GhostWeakness = "";
    public static string FullWeaknessName = "";
    public static List<string> SafePassword = new List<string>();
    

    [Header("가지고만 있어도 발휘되는 아이템")]
    public static bool hasCharm = false; //부적 효과
    public static bool hasExtinguisher = false; //소화기 효과.


    [SerializeField] public PlayerController playerController;
    [SerializeField] private string mainMenuSceneName = "MainMenu";
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else 
        {
            Destroy(gameObject);
        }

    }

    private void Start()
    {
        if(noiseSystem ==null)
        {
            noiseSystem = FindObjectOfType<NoiseSystem>();
        }

        if (ghostManager == null)
        {
            ghostManager = FindObjectOfType<GhostManager>();
        }

        if (mapControlloer == null)
        {
            mapControlloer = FindObjectOfType<MapControlloer>();
        }

        if (trapManager == null)
        {
            trapManager = FindObjectOfType<trapManager>();
        }

        if (itemMnanger == null)
        {
            itemMnanger = FindObjectOfType<ItemMnanger>();
        }


        if (noiseSystem != null)
        {
            noiseSystem.HuntTriggered += OnHuntTriggered;
        }
        StartCoroutine(StartGame());
    }

    private void OnDestroy()
    {
        if(noiseSystem != null)
        {
            noiseSystem.HuntTriggered -= OnHuntTriggered;
        }
    }

    private IEnumerator StartGame()
    {
        yield return new WaitForSeconds(1.0f);
        isPlaying = true;
        isHunting = false;
        isGameOver = false;
    }

    private void OnHuntTriggered()
    {
        if (isHunting || isGameOver) return;
        isHunting = true;
        Debug.Log ("사냥 시작이다요!");
        ghostManager.StartHunt();
    }

    public void OnHuntEnd()
    {
        isHunting = false;
        foreach (var p in FindObjectsOfType<InvestigatePoint>())
            p.ResetHide();
    }


    public void OnPlayerCaught()
    { 
        if (isGameOver) return;

        if(hasExtinguisher)
        {
            hasExtinguisher = false;
            Debug.Log("그런데..! 마지막 발악으로 소화기를 뿌렸더니.. 유령이 달아나버렸다요.");
            ghostManager.ForceEndHunt();
            return;
        }

        if(hasCharm)
        {
            hasCharm = false;
            Debug.Log("하지만, 부적이 날 지켜줬다요!");
            ghostManager.ForceEndHunt();
            return;
        }

        UiManager.instance?.ShowGameOver();

        GameOver();
    }

    public void GameOver()
    {
        isGameOver = true;
        isPlaying = false;
        isHunting = false;

        ghostManager.ForceEndHunt();
        StartCoroutine(RestartGame());
    }

    private IEnumerator RestartGame()
    {
        yield return new WaitForSeconds(3f);
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void GameClear()
    {
        if (isGameOver) return;
        isPlaying = false;
        isGameOver = true;

        Debug.Log("유령을 격퇴했다요! 게임 클리어다요!");

        StartCoroutine(EndGameRoutine());
    }

    private IEnumerator EndGameRoutine()
    {
        yield return new WaitForSeconds(3f);
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);

    }

    public void OnGameClear()
    {
        Debug.Log("탈출성공!");
        UiManager.instance?.ShowGameClear();
    }


}

using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class GhostManager : MonoBehaviour
{
    public static GhostManager Instance { get; private set; }

    [SerializeField] GameObject ghostPreFab;
    [SerializeField] Transform spawnPoint;
    [SerializeField] float huntDuration = 20f;

    private GameObject currentGhost;
    private Coroutine huntRoutine;

    private bool isWandering = false;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }
    void Start()
    {
        if (NoiseSystem.Instance != null)
        {
            NoiseSystem.Instance.HuntTriggered += OnHuntStart;
        }
        foreach (var g in FindObjectsOfType<GhostChase>())
        {
            Destroy(g.gameObject);
        }
    }

    private void OnDestroy()
    {
        if (NoiseSystem.Instance != null)
        { 
            NoiseSystem.Instance.HuntTriggered -= OnHuntStart;
        }
    }

    private void OnHuntStart()
    {
        Debug.Log("헌팅 타임이다요");
        StartHunt();

        //나중에 사운드나 UI를 작성할때 가독성을 위해서 거쳐가도록 작성.
    }

    public void StartHunt()
    {
        if (currentGhost != null)
        {
            Debug.Log("이미 유령있다요");
            return;
        }
            if (huntRoutine != null)
        {
            StopCoroutine(huntRoutine);
        }
        huntRoutine = StartCoroutine(HuntCoroutine());
    }

    private IEnumerator HuntCoroutine()
    {
        currentGhost = Instantiate(ghostPreFab, spawnPoint.position, Quaternion.identity);
        yield return new WaitForSeconds(huntDuration);
        ForceEndHunt();
    }

    public void ForceEndHunt()
    {
        if (currentGhost != null)
        {
            Destroy(currentGhost);
            Debug.Log("유령 제거");
        }
        currentGhost = null;
        foreach (var ghost in FindObjectsOfType<GhostChase>())
        {
            Destroy(ghost.gameObject);
        }
        currentGhost = null;


        GameManager.Instance?.OnHuntEnd();
        NoiseSystem.Instance?.EndHunt();
    }
    public void SetGhostToWanderMode(bool wander)
    {
        if (currentGhost == null) return;
        var ai = currentGhost.GetComponent<GhostChase>(); // 유령 이동 담당 스크립트
        if (ai != null)
            ai.SetWanderMode(wander);

        isWandering = wander;
        Debug.Log($" 유령 상태 변경: {(wander ? "순찰" : "추적")}");
    }


}

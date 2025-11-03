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
            Destroy(currentGhost);

        currentGhost = null;

        if (GameManager.Instance != null)
            GameManager.Instance.OnHuntEnd();

        if (NoiseSystem.Instance != null)
            NoiseSystem.Instance.currentNoise = 0;
    }
   
}

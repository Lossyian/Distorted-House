using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostManager : MonoBehaviour
{
    public static GhostManager Instance { get; private set; }

    [SerializeField] GameObject ghostPreFab;
    [SerializeField] Transform spawnPoint;
    [SerializeField] float huntDuration = 20f;

    private GameObject currentGhost;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }
    void Start()
    {
        NoiseSystem.Instance.HuntTriggered += OnHuntStart;
    }

    private void OnDestroy()
    {
        if (NoiseSystem.Instance != null)
            NoiseSystem.Instance.HuntTriggered -= OnHuntStart;
    }

    private void OnHuntStart()
    {
        Debug.Log("헌팅 타임 시작!");
        StartCoroutine(HuntCoroutine());
    }
    private IEnumerator HuntCoroutine()
    {
        currentGhost = Instantiate(ghostPreFab, spawnPoint.position, Quaternion.identity);
        yield return new WaitForSeconds(huntDuration);

        if (currentGhost != null) Destroy(currentGhost);

        Debug.Log("[ghostManager]헌팅 종료.");
        NoiseSystem.Instance.currentNoise = 0;
    }

    public void OnplayerCaught()
    {
        Inventory inv = FindObjectOfType<Inventory>();
        if (GameManager.hasExtinguisher && inv != null)
        {
            Debug.Log("소화기를 뿌리니 유령이 달아났다요.");
            inv.ConsumeItem("낡은소화기");

            if (currentGhost != null)
                Destroy(currentGhost);
            return;
        }
    }
}

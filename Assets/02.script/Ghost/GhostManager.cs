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
        Debug.Log("«Â∆√ ≈∏¿” Ω√¿€!");
        StartCoroutine(HuntCoroutine());
    }
    private System.Collections.IEnumerator HuntCoroutine()
    {
        currentGhost = Instantiate(ghostPreFab, spawnPoint.position, Quaternion.identity);
        yield return new WaitForSeconds(huntDuration);

        if (currentGhost != null) Destroy(currentGhost);

        Debug.Log("[ghostManager]«Â∆√ ¡æ∑·.");
        NoiseSystem.Instance.currentNoise = 0;
    }
}

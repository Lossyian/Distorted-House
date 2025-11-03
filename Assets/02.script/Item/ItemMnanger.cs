using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemMnanger : MonoBehaviour
{
    public static ItemMnanger Instance;

    private NoiseSystem noiseSystem;
    private trapManager trapManager;
    private PlayerController player;
    private MapControlloer mapControlloer;
    private Inventory Inventory;

    [Header("위자보드 지속시간")]
    [SerializeField] private float revealDuration = 10.0f;


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
        noiseSystem = FindObjectOfType<NoiseSystem>();
        trapManager = FindObjectOfType<trapManager>();
        mapControlloer = FindObjectOfType<MapControlloer>();
        player = FindObjectOfType<PlayerController>();
        Inventory = FindObjectOfType<Inventory>();
    }

    public void UseItem(string itemName)
    {
        switch (itemName)
        {
            case "수상한 부적":
                UseCharm();
                break;
            case "낡은소화기":
                UseExtinguisher();
                break;
            case "팔각패":
                UseOctagontally();
                break;
            case "다우징 막대":
                UseDowsingRod();
                break;
            case "위자보드":
                UseOuijaBoard();
                break;
            case "청진기":
                UseStethoscope();
                break;
            case "무당의 방울":
                UseShamanBell();
                break;
            case "십자가":

                break;
            case "말뚝":

                break;
            case "기름통":

                break;
            


        }
    }
   
    private void UseCharm()
    {
        // 수상한 부적 = 다음 함정 1회 무효화.
        GameManager.hasCharm = true;
    }

    private void UseExtinguisher()
    {
        // 낡은 소화기 = 유령에게 잡혀도 1회 구제
        GameManager.hasExtinguisher = true;
    }

    private void UseOctagontally()
    {
        if (noiseSystem != null)
        {
            float reduce = 20f;
            noiseSystem.currentNoise = Mathf.Max(0, noiseSystem.currentNoise - reduce);
            Debug.Log($" 팔각패 쓴다요 - 소음 {reduce} 감소! 현재 소음: {noiseSystem.currentNoise}");
        }

    }

    private void UseDowsingRod()
    {
        Room currentRoom = player.GetComponentInParent<Room>();
        if (currentRoom != null) return;

        bool hasTrap = currentRoom.points.Exists(p => p.type == InvestigateType.Trap);
        Debug.Log(hasTrap ? "방에 함정이있다요!" : "안전한 방이다요.");
    }
    private void UseOuijaBoard()
    {
        Debug.Log(" 모든 함정을 표시한다요.");
        StartCoroutine(RevealTrapsTemporarily());
    }

    private IEnumerator RevealTrapsTemporarily()
    {
        List<InvestigatePoint> allPoints = FindObjectsOfType<InvestigatePoint>().ToList();

        foreach ( var p in allPoints)
        {
            if (p.type == InvestigateType.Trap)
            {
                p.SetVisible(true);
            }
        }

        yield return new WaitForSeconds(revealDuration);

        foreach ( var p in allPoints)
        {
            if (p.type == InvestigateType.Trap)
                p.SetVisible(false);
        }

    }

    private void UseStethoscope()
    {
        if (GameManager.SafePassword != null && GameManager.SafePassword.Count > 0)
        {
            string hint = GameManager.SafePassword[Random.Range(0,GameManager.SafePassword.Count)];
            Debug.Log($"청진기 사용 - 금고 비밀번호 힌트: {hint}");
        }
    }

    private void UseShamanBell()
    {
        Debug.Log("무당의방울쓴다요");
        StartCoroutine(SlowGhostDuringHunt());
    }
    private IEnumerator SlowGhostDuringHunt()
    {
        //헌팅 강제 시작
        noiseSystem.AddNoise(100f);

        GameManager.ghostSpeedMulitplier = 0.5f;
        yield return new WaitUntil(() => !noiseSystem.isHunting);

        GameManager.ghostSpeedMulitplier = 1f;
    }


}

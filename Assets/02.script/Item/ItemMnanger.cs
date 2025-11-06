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
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
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
        string key = itemName.Trim().Replace(" ", "");
        Debug.Log($"[UseItem] 호출됨 / 아이템 이름 = '{key}'");

        switch (key)
        {
            case "수상한부적":
                UseCharm();
                break;
            case "낡은소화기":
                UseExtinguisher();
                break;
            case "팔각패":
                UseOctagontally();
                break;
            case "다우징막대":
                UseDowsingRod();
                break;
            case "위자보드":
                UseOuijaBoard();
                break;
            case "청진기":
                UseStethoscope();
                break;
            case "무당의방울":
                UseShamanBell();
                break;
            case "십자가":
                TryExorcism(itemName);
                break;
            case "말뚝":
                TryExorcism(itemName);
                break;
            case "기름통":
                TryExorcism(itemName);
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
        Debug.Log("팔각패쓴다욧!");
        if (noiseSystem != null)
        {
            float reduce = 20f;
            noiseSystem.currentNoise = Mathf.Max(0, noiseSystem.currentNoise - reduce);
            Debug.Log($" 팔각패 쓴다요 - 소음 {reduce} 감소! 현재 소음: {noiseSystem.currentNoise}");
            UiManager.instance?.ShowDialog(" 팔각패를 사용했다. 주변이 조용해졌다");
        }

    }

    private void UseDowsingRod()
    {
        Debug.Log("다우징 쓴다요!");
        Room currentRoom = player.currentRoom;
        if (currentRoom == null)
        {
            Debug.Log("방이 아닌데유?");
            return;
        }

        bool hasTrap = currentRoom.points.Exists(p => p.type == InvestigateType.Trap);
        if (hasTrap)
        {
            UiManager.instance?.ShowDialog("다우징 막대가 흔들린다... 이 방엔 함정이 있다!");
            Debug.Log($"[Dowsing] 방 '{currentRoom.name}' 에 함정 존재!");
        }
        else
        {
            UiManager.instance?.ShowDialog("조용하다... 함정은 없는 것 같다.");
            Debug.Log($"[Dowsing] 방 '{currentRoom.name}' 에 함정 없음.");
        }
    }
    private void UseOuijaBoard()
    {
        Debug.Log("위자보드쓴다욧!");
        UiManager.instance?.ShowDialog(" 위자보드에 물었다. 함정의 위치를");
        StartCoroutine(RevealTrapsTemporarily());
    }

    private IEnumerator RevealTrapsTemporarily()
    {
        Debug.Log("지징 지징");
        List<InvestigatePoint> allPoints = FindObjectsOfType<InvestigatePoint>().ToList();

        foreach (var p in allPoints)
        {
            if (p.type == InvestigateType.Trap)
            {
                p.SetVisible(true);
            }
        }

        yield return new WaitForSeconds(revealDuration);

        foreach (var p in allPoints)
        {
            if (p.type == InvestigateType.Trap)
                p.SetVisible(false);
        }

    }

    private void UseStethoscope()
    {
        Debug.Log("청진기 쓴다욧!");
        if (GameManager.SafePassword != null && GameManager.SafePassword.Count > 0)
        {
            string hint = GameManager.SafePassword[Random.Range(0, GameManager.SafePassword.Count)];
            UiManager.instance?.ShowDialog($"청진기를 사용했다. 금고 비밀번호 힌트: {hint}");
            Debug.Log($"청진기를 사용했다. 금고 비밀번호 힌트: {hint}");

        }
    }

    private void UseShamanBell()
    {
        Debug.Log("무당방울 쓴다욧!");
        UiManager.instance?.ShowDialog("무당의방울 사용했다.");
        StartCoroutine(SlowGhostDuringHunt());
    }
    private IEnumerator SlowGhostDuringHunt()
    {
        //헌팅 강제 시작
        Debug.Log("딸랑딸랑!");
        noiseSystem.AddNoise(100f);
        GameManager.ghostSpeedMulitplier = 0.4f;

        yield return new WaitUntil(() => !noiseSystem.isHunting);

        GameManager.ghostSpeedMulitplier = 1f;
    }

    private void TryExorcism(string itemName)
    {
        if (GhostBone.instance == null)
        {
            UiManager.instance?.ShowDialog("유령의 본체는 여기 없다.");
            Debug.Log("유령의 본체는 여기 없다요.");
            return;
        }

        var player = FindObjectOfType<PlayerController>();
        if (player == null) return;

        float dist = Vector2.Distance(player.transform.position, GhostBone.instance.transform.position);
        if (dist >= 2.0f)
        {
            UiManager.instance?.ShowDialog("유령의 본체 근처에서만 쓸 수 있을것같다.");
            Debug.Log("너무 멀다요. 유령이 반응하지 않는다요.");
            return;
        }
        bool success = GhostBone.instance.TryExorcise(itemName);
        Inventory inv = FindObjectOfType<Inventory>();

        if (success)
        {
            UiManager.instance?.ShowDialog("퇴마 성공!");
            Debug.Log("퇴마 성공!");
            inv?.ConsumeItem(itemName);
        }
        else
        {
            UiManager.instance?.ShowDialog("틀렸다요.. 유령이 화가 났다요!!!");
            Debug.Log("틀렸다요.. 유령이 화가 났다요!!!");
            // 여긴 아이템 소모 안 함
        }
    }
}



            

     

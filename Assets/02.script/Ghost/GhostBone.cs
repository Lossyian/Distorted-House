using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GhostBone : MonoBehaviour
{
    public static GhostBone instance;
    private InvestigatePoint currentPoint;

    [SerializeField] private float moveInterval = 60f;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    //  Start에서는 초기화하지 않음.
    private void Start()
    {
        // 조사포인트 배치가 끝나면 직접 Initialize 호출하도록.
        StartCoroutine(WaitAndInitialize());
    }

    private IEnumerator WaitAndInitialize()
    {
        // 조사포인트 초기화가 끝날 때까지 잠시 대기
        yield return new WaitForSeconds(1f);
        InitializeGhostBone();
    }

    //  조사포인트 배치가 끝난 후 호출될 초기화 함수
    public void InitializeGhostBone()
    {
        var ghostPoint = FindObjectsOfType<InvestigatePoint>().FirstOrDefault(p => p.dataName == "유령의 본체");

        if (ghostPoint != null)
        {
            currentPoint = ghostPoint;
            transform.position = ghostPoint.transform.position;
            Debug.Log($"유령 본체 위치 초기화 완료: {ghostPoint.name}");
        }
        
        StartCoroutine(MoveRoutine());
    }

    private IEnumerator MoveRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(moveInterval);
            MoveToRandomEmptyPoint();
        }
    }

    private void MoveToRandomEmptyPoint()
    {
        var allPoints = FindObjectsOfType<InvestigatePoint>().ToList();
        var emptyPoints = allPoints.Where(p => p.type == InvestigateType.Empty).ToList();
        if (emptyPoints.Count == 0) return;

        InvestigatePoint target = emptyPoints[Random.Range(0, emptyPoints.Count)];
        transform.position = target.transform.position;

        if (currentPoint != null)
        {
            currentPoint.type = InvestigateType.Empty;
            currentPoint.dataName = "";
        }

        target.type = InvestigateType.special;
        target.dataName = "유령의 본체";
        currentPoint = target;

        Debug.Log("유령이가 이동했다요");
    }

    public void SetCurrentPoint(InvestigatePoint p)
    {
        currentPoint = p;
        transform.position = p.transform.position;
    }

    public bool TryExorcise(string usedItem)
    {
        
        if (string.IsNullOrEmpty(usedItem) || string.IsNullOrEmpty(GameManager.GhostWeakness))
        {
            return false;
        }

        // 공백 제거, 소문자 변환
        string a = usedItem.Trim();
        string b = GameManager.GhostWeakness.Trim();

       
        if (string.Equals(a, b, System.StringComparison.OrdinalIgnoreCase))
        {
            Debug.Log("유령을 격퇴했다요!");
            GameManager.Instance.GameClear(); 
            return true;
        }
        else
        {
            Debug.Log("틀렸다요.. 유령이 화나서 쫒아온다요!!!");
            NoiseSystem.Instance.AddNoise(100f);
            return false;
        }
    }

}


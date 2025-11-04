using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GhostBone : MonoBehaviour
{
    public static GhostBone instance;
    private InvestigatePoint currentPoint;

    [SerializeField] private float moveInterval = 60f;

    private List<InvestigatePoint> emptyPoints = new List<InvestigatePoint>();
    private void Awake()
    {
        if (instance == null&& instance !=this )
        {
            Destroy(gameObject);
            return;
        }
        instance = this; 
    }
    void Start()
    {
        var ghostPoint = FindObjectsOfType<InvestigatePoint>().FirstOrDefault(p => p.dataName == "유령의 본체");
        if (ghostPoint != null)
        {
            currentPoint = ghostPoint;
            transform.position = ghostPoint.transform.position;
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

        if(currentPoint != null)
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
    public bool TryExorcise(string UsedItem)
    {
        if (UsedItem == GameManager.GhostWeakness)
        {
            Debug.Log("유령을 격퇴했다요!");
            GameManager.Instance.GameOver();
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

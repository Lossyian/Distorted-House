using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;

public class InvestigatePointController : MonoBehaviour
{
    public List<Room> allRooms = new List<Room>();
    public List<InvestigatePoint> allPoints = new List<InvestigatePoint>();
    private bool secretDataInitialized = false;

    public void StartDistribution()
    {
        collectAllPoints();
        if (!secretDataInitialized)
        {
            SecretData();
        }
        DispositionPoints();
        PlaceGhostBone();
    }

    public void collectAllPoints()
    {
        allPoints.Clear();
        allRooms.AddRange(FindObjectsOfType<Room>());
        
        foreach (Room r in allRooms)
            allPoints.AddRange(r.points);
        
        Shuffle(allPoints);
    }



    private void SecretData()
    {
        if (secretDataInitialized) return;
        secretDataInitialized = true;

        var data = ItemTableLoader.loadedData;

        var provisos = data.FindAll(d => d.type != null && d.type.Trim().ToLower() == "proviso");

        var provisoNums = data.FindAll(d => !string.IsNullOrEmpty(d.type) && d.type.Trim().ToLower() == "provisonum");
        var provisoWeaks = data.FindAll(d => !string.IsNullOrEmpty(d.type) && d.type.Trim().ToLower() == "provisoweak");

        Debug.Log("loaded types: " + string.Join(",", data.Select(d => d.type)));


        if (provisoNums.Count < 3 || provisoWeaks.Count < 1)
        {
            Debug.LogWarning(" 단서 데이터가 모자라다요!");
            return;
        }


        GameManager.SafePassword = provisoNums.OrderBy(x => Random.value).Take(3).Select(d => d.name).ToList();

        GameManager.GhostWeakness = provisoWeaks[Random.Range(0, provisoWeaks.Count)].name;

        Debug.Log($" 숫자 단서 {provisoNums.Count}개, 약점 단서 {provisoWeaks.Count}개");


        Debug.Log($" 금고 비밀번호: {string.Join("-", GameManager.SafePassword)}");
        Debug.Log($" 약점 단서: {GameManager.GhostWeakness}");
    }

    private void DispositionPoints()
    {
        //   조사포인트 중복 제거 및 초기화
        allRooms = FindObjectsOfType<Room>().Distinct().ToList();
        allPoints = allRooms.SelectMany(r => r.points).Distinct().ToList();

        allPoints = allPoints.OrderBy(p => Random.value).ToList();

        foreach (var p in allPoints)
        {
            p.type = InvestigateType.Empty;
            p.dataName = "";
        }

        var data = ItemTableLoader.loadedData;
        if (data == null || data.Count == 0)
        {
            Debug.LogError(" ItemTableLoader 데이터가 비었다요!");
            return;
        }

        //   단서 중 금고 비밀번호 / 약점 제외
        var provisoNums = data.FindAll(d => d.type.ToLower() == "provisonum")
                              .Where(d => !GameManager.SafePassword.Contains(d.name))
                              .ToList();
        var provisoWeaks = data.FindAll(d => d.type.ToLower() == "provisoweak")
                               .Where(d => d.name != GameManager.GhostWeakness)
                               .ToList();

        var provisos = new List<ItemData>();
        provisos.AddRange(provisoNums);
        provisos.AddRange(provisoWeaks);

        //   기타 분류
        var items = data.FindAll(d => d.type.ToLower() == "item");
        var traps = data.FindAll(d => d.type.ToLower() == "trap");
        var special = data.FindAll(d => d.type.ToLower() == "special");

        //   전체 조사포인트 랜덤화
        Shuffle(allPoints);

        //   타입별 데이터도 랜덤화
        Shuffle(items);
        Shuffle(traps);
        Shuffle(provisos);
        Shuffle(special);

        //   전체 리스트 합치기 (하나의 큐처럼 처리)
        var allData = new List<(InvestigateType, ItemData)>();
        allData.AddRange(items.Select(i => (InvestigateType.item, i)));
        allData.AddRange(traps.Select(t => (InvestigateType.Trap, t)));
        allData.AddRange(provisos.Select(p => (InvestigateType.proviso, p)));
        allData.AddRange(special.Select(s => (InvestigateType.special, s)));

        Shuffle(allData);

        //   배치
        int assignCount = Mathf.Min(allPoints.Count, allData.Count);
        for (int i = 0; i < assignCount; i++)
        {
            allPoints[i].type = allData[i].Item1;
            allPoints[i].dataName = allData[i].Item2.name;
        }

        //   로그 출력
        Debug.Log($" 조사포인트 배치 완료다요: 총 {assignCount}/{allPoints.Count} 사용했다요");
        Debug.Log($"(item={items.Count}, trap={traps.Count}, proviso={provisos.Count}, special={special.Count})");
    }

    
    private void Shuffle<T>(List<T> list)
    {
        for (int i = 0; i< list.Count; i++)
        {
            int rand = Random.Range(i, list.Count);
            (list[i], list[rand]) = (list[rand], list[i]);

        }
    }

    public void removeRandom()
    {
        List<InvestigatePoint> clues = allRooms.SelectMany(r => r.points).Where(p => p.type == InvestigateType.proviso).ToList();

        
        if (clues.Count == 0)
        {
            //제거 단서 없을때.
            return;
        }

        InvestigatePoint target = clues[Random.Range(0, clues.Count)];
        target.type = InvestigateType.Empty;
        target.dataName = "";

        Debug.Log($"{target.name}단서를 지웠다요.");
    }

    private void PlaceGhostBone()
    {
        List<InvestigatePoint> emptyPoints = allPoints.Where(p=> p.type== InvestigateType.Empty).ToList();
        if (emptyPoints.Count == 0) return;

        InvestigatePoint target = emptyPoints[Random.Range(0, emptyPoints.Count)];
        target.type = InvestigateType.special;
        target.dataName = "유령의 본체";

        Debug.Log("유령의 본체 생성됬다요");

    }
}

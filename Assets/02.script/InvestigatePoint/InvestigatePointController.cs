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
        if (data == null || data.Count == 0)
        {
            Debug.LogError("ItemTableLoader 데이터가 비었다요!");
            return;
        }

        var provisoNums = data.Where(d => d.type?.Trim().ToLower() == "provisonum").ToList();
        var provisoWeaks = data.Where(d => d.type?.Trim().ToLower() == "provisoweak").ToList();

        if (provisoNums.Count < 3 || provisoWeaks.Count < 1)
        {
            Debug.LogWarning("단서 데이터가 모자라다요!");
            return;
        }

        // ① 금고 비밀번호 3자리 랜덤 선택
        GameManager.SafePassword = provisoNums
            .OrderBy(x => Random.value)
            .Take(3)
            .Select(d => d.name.Trim())
            .ToList();

        // ② 약점 하나 선택
        string chosenWeakRaw = provisoWeaks[Random.Range(0, provisoWeaks.Count)].name.Trim();

        // 원본(X붙은) 저장
        GameManager.FullWeaknessName = chosenWeakRaw;

        // X 제거 후 실제 약점 저장
        GameManager.GhostWeakness = chosenWeakRaw.EndsWith("X")
            ? chosenWeakRaw.Substring(0, chosenWeakRaw.Length - 1)
            : chosenWeakRaw;

        Debug.Log($"숫자 단서 {provisoNums.Count}개, 약점 단서 {provisoWeaks.Count}개");
        Debug.Log($"금고 비밀번호: {string.Join("-", GameManager.SafePassword)}");
        Debug.Log($"약점 원본 이름(제외용): {GameManager.FullWeaknessName}");
        Debug.Log($"약점 실제 이름(퇴마용): {GameManager.GhostWeakness}");
    }


    private void DispositionPoints()
    {
        // 조사포인트 중복 제거 및 초기화
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
            Debug.LogError("ItemTableLoader 데이터가 비었다요!");
            return;
        }

        // 단서 중 금고 비밀번호 / 약점 제외
        var provisoNums = data.Where(d => d.type.ToLower() == "provisonum").Where(d => !GameManager.SafePassword.Contains(d.name.Trim())).ToList();

        // 약점 단서 중 정답(FullWeaknessName) 제외
        var provisoWeaks = data.Where(d => d.type.ToLower() == "provisoweak").Where(d => !string.Equals(d.name.Trim(), GameManager.FullWeaknessName.Trim(), System.StringComparison.OrdinalIgnoreCase)).ToList();

        var provisos = new List<ItemData>();
        provisos.AddRange(provisoNums);
        provisos.AddRange(provisoWeaks);

        // 기타 분류
        var items = data.FindAll(d => d.type.ToLower() == "item");
        var traps = data.FindAll(d => d.type.ToLower() == "trap");
        var special = data.FindAll(d => d.type.ToLower() == "special");

        // 전부 랜덤 섞기
        Shuffle(allPoints);
        Shuffle(items);
        Shuffle(traps);
        Shuffle(provisos);
        Shuffle(special);

        // 전체 리스트 합치기
        var allData = new List<(InvestigateType, ItemData)>();
        allData.AddRange(items.Select(i => (InvestigateType.item, i)));
        allData.AddRange(traps.Select(t => (InvestigateType.Trap, t)));
        allData.AddRange(provisos.Select(p => (InvestigateType.proviso, p)));
        allData.AddRange(special.Select(s => (InvestigateType.special, s)));

        Shuffle(allData);

        // 실제 배치
        int assignCount = Mathf.Min(allPoints.Count, allData.Count);
        for (int i = 0; i < assignCount; i++)
        {
            allPoints[i].type = allData[i].Item1;
            allPoints[i].dataName = allData[i].Item2.name;
        }

        Debug.Log($"조사포인트 배치 완료다요: 총 {assignCount}/{allPoints.Count} 사용했다요");
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

        if (GhostBone.instance != null)
            GhostBone.instance.InitializeGhostBone();

    }
}

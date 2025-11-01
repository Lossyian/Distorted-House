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
            Debug.LogWarning(" �ܼ� ������ ����! ��Ʈ Ȯ�� �ʿ�!");
            return;
        }


        GameManager.SafePassword = provisoNums.OrderBy(x => Random.value).Take(3).Select(d => d.name).ToList();

        GameManager.GhostWeakness = provisoWeaks[Random.Range(0, provisoWeaks.Count)].name;

        Debug.Log($" ���� �ܼ� {provisoNums.Count}��, ���� �ܼ� {provisoWeaks.Count}��");


        Debug.Log($" �ݰ� ��й�ȣ: {string.Join("-", GameManager.SafePassword)}");
        Debug.Log($" ���� �ܼ�: {GameManager.GhostWeakness}");
    }

    private void DispositionPoints()
    {
        //  0. ��������Ʈ �ߺ� ���� �� �ʱ�ȭ
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
            Debug.LogError(" ItemTableLoader �����Ͱ� ����ֽ��ϴ�!");
            return;
        }

        //  1. �ܼ� �� �ݰ� ��й�ȣ / ���� ����
        var provisoNums = data.FindAll(d => d.type.ToLower() == "provisonum")
                              .Where(d => !GameManager.SafePassword.Contains(d.name))
                              .ToList();
        var provisoWeaks = data.FindAll(d => d.type.ToLower() == "provisoweak")
                               .Where(d => d.name != GameManager.GhostWeakness)
                               .ToList();

        var provisos = new List<ItemData>();
        provisos.AddRange(provisoNums);
        provisos.AddRange(provisoWeaks);

        //  2. ��Ÿ �з�
        var items = data.FindAll(d => d.type.ToLower() == "item");
        var traps = data.FindAll(d => d.type.ToLower() == "trap");
        var special = data.FindAll(d => d.type.ToLower() == "special");

        //  3. ��ü ��������Ʈ ����ȭ
        Shuffle(allPoints);

        //  4. Ÿ�Ժ� �����͵� ����ȭ
        Shuffle(items);
        Shuffle(traps);
        Shuffle(provisos);
        Shuffle(special);

        //  5. ��ü ����Ʈ ��ġ�� (�ϳ��� ťó�� ó��)
        var allData = new List<(InvestigateType, ItemData)>();
        allData.AddRange(items.Select(i => (InvestigateType.item, i)));
        allData.AddRange(traps.Select(t => (InvestigateType.Trap, t)));
        allData.AddRange(provisos.Select(p => (InvestigateType.proviso, p)));
        allData.AddRange(special.Select(s => (InvestigateType.special, s)));

        Shuffle(allData);

        //  6. ��ġ
        int assignCount = Mathf.Min(allPoints.Count, allData.Count);
        for (int i = 0; i < assignCount; i++)
        {
            allPoints[i].type = allData[i].Item1;
            allPoints[i].dataName = allData[i].Item2.name;
        }

        //  7. �α� ���
        Debug.Log($" ��������Ʈ ��ġ �Ϸ�: �� {assignCount}/{allPoints.Count} ����");
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
            //���� �ܼ� ������.
            return;
        }

        InvestigatePoint target = clues[Random.Range(0, clues.Count)];
        target.type = InvestigateType.Empty;
        target.dataName = "";

        Debug.Log($"{target.name}�ܼ��� �����ٿ�.");
    }
}

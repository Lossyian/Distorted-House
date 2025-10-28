using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;

public class InvestigatePointController : MonoBehaviour
{
    public List<Room> allRooms = new List<Room>();
    private List<InvestigatePoint> allPoints = new List<InvestigatePoint>();


    public void StartDistribution()
    {
        collectAllPoints();
        SecretData();
        DispositionPoints(); 
    }

    private void collectAllPoints()
    {
        allPoints.Clear();
        allRooms.AddRange(FindObjectsOfType<Room>());
        
        foreach (Room r in allRooms)
            allPoints.AddRange(r.points);
        
        Shuffle(allPoints);
    }



    private void SecretData()
    {
        var data = ItemTableLoader.loadedData;

        var provisos = data.FindAll(d => d.type != null && d.type.Trim().ToLower() == "proviso");

        var provisoNums = data.FindAll(d => !string.IsNullOrEmpty(d.type) && d.type.Trim().ToLower() == "provisonum");
        var provisoWeaks = data.FindAll(d => !string.IsNullOrEmpty(d.type) && d.type.Trim().ToLower() == "provisoweak");

        Debug.Log("loaded types: " + string.Join(",", data.Select(d => d.type)));


        if (provisoNums.Count < 3 || provisoWeaks.Count < 1)
        {
            Debug.LogWarning(" 단서 데이터 부족! 시트 확인 필요!");
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
        var data = ItemTableLoader.loadedData;

        // 정답이 될 단서들 제외.
        var provisoNums = data.FindAll(d => d.type.ToLower() == "provisonum").Where(d => !GameManager.SafePassword.Contains(d.name)).ToList();
        var provisoWeaks = data.FindAll(d => d.type.ToLower() == "provisoweak").Where(d => d.name != GameManager.GhostWeakness).ToList();

        var provisos = new List<ItemData>();
        provisos.AddRange(provisoNums);
        provisos.AddRange(provisoWeaks);

        var items = data.FindAll(d => d.type.ToLower() == "item");
        var traps = data.FindAll(d => d.type.ToLower() == "trap");
       
        var special = data.FindAll(d => d.type.ToLower() == "special");

        int index = 0;

        void Assign (List<ItemData> list, InvestigateType type)
        {
            for (int i = 0; i < list.Count && index < allPoints.Count; i++)
            {
                allPoints[index].type = type;
                allPoints[index].dataName = list[i].name;
                index++;
            }

        }
        Assign(items, InvestigateType.item);
        //아이템
        Assign(traps, InvestigateType.Trap);
        //함정
        Assign(provisos, InvestigateType.proviso);
        //단서
        Assign(special, InvestigateType.special);
        //특수

        

        Debug.Log($"배치 완료: items={items.Count}, traps={traps.Count}, special={special.Count}, provisos={provisos.Count}");
    
    }

    private void Shuffle<T>(List<T> list)
    {
        for (int i = 0; i< list.Count; i++)
        {
            int rand = Random.Range(i, list.Count);
            (list[i], list[rand]) = (list[rand], list[i]);

        }
    }


}

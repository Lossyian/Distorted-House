using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvestigatePointController : MonoBehaviour
{
    public List<Room> allRooms;

    [Header("아이템 모음")]
    public List<string> itemNames;       
    public List<string> trapNames;       
    public List<string> provisoNumNames;  
    public List<string> provisoWeakNames; 
    public List<string> specialNames;


    private List<InvestigatePoint> allPoints = new List<InvestigatePoint>();

    //선택된 금고 비밀번호와, 약점단서.
    public List<string> SelectedNum = new List<string>();
    public string selectedWeak;

    public void StartDistribution()
    {
        collectAllPoints();
        SecretData();
        DispositionPoints(); 
    }

    private void collectAllPoints()
    {
        allPoints.Clear();
        
        foreach (Room r in allRooms)
        {
            foreach (var p in r.points)
                if (!allPoints.Contains(p)) 
                    allPoints.Add(p);
        }
            

        Shuffle(allPoints);
    }

    private void SecretData()
    {
        Shuffle(provisoNumNames);
        Shuffle(provisoWeakNames);

        SelectedNum = provisoNumNames.GetRange(0, 3);
        selectedWeak = provisoWeakNames[0];

        Debug.Log($"금고 비밀번호 : {string.Join("-", SelectedNum)}");
        Debug.Log($"유령 약점 : {selectedWeak}");
    }

    private void DispositionPoints()
    {
        int index = 0;
        //아이템
        for (int i = 0; i < itemNames.Count; i++)
        {

            allPoints[index].type = InvestigateType.item;
            allPoints[index].dataName = itemNames[i];
            index++;
        }
        //함정
        for (int i = 0; i<6; i ++)
        {
            allPoints[index].type = InvestigateType.Trap;
            allPoints[index].dataName = trapNames[Random.Range(0, trapNames.Count)];
            index++;
        }

        //단서들 세팅
        List<string> RemainingNum = new List<string>(provisoNumNames);
        RemainingNum.RemoveAll(x => SelectedNum.Contains(x));

        List<string> RemainingWeek = new List<string>(provisoWeakNames);
        RemainingWeek.Remove(selectedWeak);

        List<string> allRemainings = new List<string>();
        allRemainings.AddRange(RemainingNum.GetRange(0, Mathf.Min(6, RemainingNum.Count)));
        allRemainings.AddRange(RemainingWeek.GetRange(0, Mathf.Min(2, RemainingWeek.Count)));

        Shuffle(allRemainings);

        foreach (string clue in allRemainings)
        {
            allPoints[index].type = InvestigateType.proviso;
            allPoints[index].dataName = clue;
            index++;
        }

        //특수 세팅
        for (int i = 0; i<specialNames.Count; i ++)
        {
            allPoints[index].type = InvestigateType.special;
            allPoints[index].dataName = specialNames[i];
            index++;
        }

        //남은곳 비어있는곳으로 전부 세팅

        for(int i = index; i<allPoints.Count; i++)
        {
            allPoints[i].type = InvestigateType.Empty;
            allPoints[i].dataName = "";
        }
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

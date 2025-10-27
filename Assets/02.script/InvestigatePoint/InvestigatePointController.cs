using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvestigatePointController : MonoBehaviour
{
    public List<Room> allRooms;

    [Header("������ ����")]
    public List<string> itemNames;       
    public List<string> trapNames;       
    public List<string> provisoNumNames;  
    public List<string> provisoWeakNames; 
    public List<string> specialNames;


    private List<InvestigatePoint> allPoints = new List<InvestigatePoint>();

    //���õ� �ݰ� ��й�ȣ��, �����ܼ�.
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

        Debug.Log($"�ݰ� ��й�ȣ : {string.Join("-", SelectedNum)}");
        Debug.Log($"���� ���� : {selectedWeak}");
    }

    private void DispositionPoints()
    {
        int index = 0;
        //������
        for (int i = 0; i < itemNames.Count; i++)
        {

            allPoints[index].type = InvestigateType.item;
            allPoints[index].dataName = itemNames[i];
            index++;
        }
        //����
        for (int i = 0; i<6; i ++)
        {
            allPoints[index].type = InvestigateType.Trap;
            allPoints[index].dataName = trapNames[Random.Range(0, trapNames.Count)];
            index++;
        }

        //�ܼ��� ����
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

        //Ư�� ����
        for (int i = 0; i<specialNames.Count; i ++)
        {
            allPoints[index].type = InvestigateType.special;
            allPoints[index].dataName = specialNames[i];
            index++;
        }

        //������ ����ִ°����� ���� ����

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

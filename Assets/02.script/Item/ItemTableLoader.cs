using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using static System.Net.WebRequestMethods;

public class ItemTableLoader : MonoBehaviour
{

    public static List<ItemData> loadedData = new List<ItemData>();
    public static event System.Action OnLoadCompleted;
    private void Start()
    {
        StartCoroutine(LoadCSV());
    }

    IEnumerator LoadCSV()
    {
        string url = "https://docs.google.com/spreadsheets/d/1OmWgqH8dcErM2st_3iAmWpE21GPydS_9r8AeaBCx9r4/export?format=csv";
        UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {


            ParseCSV(www.downloadHandler.text);
            OnLoadCompleted?.Invoke();
        }
       
    }

    void ParseCSV(string csv)
    {
        loadedData.Clear();
        string[] lines = csv.Split('\n');
        if (lines.Length <= 1) return;

        string[] headers = lines[0].Trim().Split(',');

        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;

            string[] values = lines[i].Trim().Split(',');
            ItemData data = new ItemData();

            for (int j = 0; j < headers.Length && j < values.Length; j++)
            {
                string header = headers[j].Trim().ToLower();
                string value = values[j].Trim();

                switch (header)
                {
                    case "id": data.id = value; break;
                    case "type": data.type = value; break;
                    case "name": data.name = value; break;
                    case "desc": data.desc = value; break;
                    case "value": data.value = value; break;
                }
            }
            loadedData.Add(data);
        }
        

       
    }

}

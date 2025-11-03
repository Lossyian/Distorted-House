using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public static bool hasCharm = false; //부적 효과
    public static bool hasExtinguisher = false; //소화기 효과.
    public static float ghostSpeedMulitplier = 1.0f;

    public static List<string> SafePassword = new List<string>();
    public static string GhostWeakness = "";
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad (gameObject);
        }
        else Destroy (gameObject);
    }
}

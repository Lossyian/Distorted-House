using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseEmitter : MonoBehaviour
{
    [Header("소음량 설정")]
    [SerializeField] float investigateNoise = 15f;
    [SerializeField] float scanNoise = 10f; 

    public void OnInvestigate()
    {
        if (NoiseSystem.Instance != null)
        {
            NoiseSystem.Instance.AddNoise(investigateNoise);
        }
    }

    public void OnScan()
    {
        if (NoiseSystem.Instance !=null)
        {
            NoiseSystem.Instance.AddNoise(scanNoise);
        }
    }
}

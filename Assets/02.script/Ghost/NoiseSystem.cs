using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseSystem : MonoBehaviour
{

    public static NoiseSystem Instance { get; private set; }

    [Header("���� ����")]
    [Range(0f, 100f)] public float currentNoise = 0f;
    [SerializeField] float decaySpeed = 0f;
    [SerializeField] float maxNoise = 100f; //����Ÿ�� �ߵ� ���� ����ġ

    public bool isHunting => currentNoise >= maxNoise;

    public delegate void OnNoiseChange(float valuie);
    public event OnNoiseChange NoiseChanged;

    public delegate void onHuntTriggered();
    public event onHuntTriggered HuntTriggered;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    void Update()
    {
        float prev = currentNoise;

        if (!isHunting && currentNoise >0)
        {
            currentNoise = Mathf.Max(0, currentNoise - decaySpeed *  Time.deltaTime);

            

            NoiseChanged?.Invoke(currentNoise);
        }
    }

    public void AddNoise(float amount)
    {
        if (isHunting) return;

        currentNoise = Mathf.Clamp(currentNoise + amount, 0, maxNoise);
        NoiseChanged?.Invoke(currentNoise);

        Debug.Log($"���� +{amount:F1} �� ���� ������: {currentNoise:F1}");

        if (currentNoise >= maxNoise)
        {
            Debug.Log("���� Ÿ��..! ������ ����� ����ϱ� ���� �i�ƿɴϴ�.");
            HuntTriggered?.Invoke();
        }

    }
}

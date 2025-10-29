using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public enum InvestigateType
{
    Empty,  //����Ÿ�ӽ� ������
    item,   //������
    Trap,   //����
    proviso,// �ܼ�
    special //���������,�ݰ�,�ٴڹ�

}
[RequireComponent(typeof(SpriteRenderer),typeof(Collider2D))]
public class InvestigatePoint : MonoBehaviour
{
    public InvestigateType type;
    public string dataName;

    private SpriteRenderer sprite;
    private Color baseColor;

    [Header("���� ����")]
    [Range(0f, 1f)] public float hiddenAlpha = 0f;
    [Range(0f, 1f)] public float visibleAlpha = 1f;
    [SerializeField] float fadeSpeed = 5f;

    private float targetAlpha;

    private void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
        baseColor = sprite.color;
        targetAlpha = hiddenAlpha;
        SetAlpha(hiddenAlpha);
    }

    private void Update()
    {
        Color c = sprite.color;
        c.a = Mathf.Lerp(c.a, targetAlpha, Time.deltaTime*fadeSpeed);
        sprite.color = c;
    }

    public void SetVisible(bool visible)
    {
        targetAlpha = visible ? visibleAlpha : hiddenAlpha;
    }

    public void SetVisibleByWave(float distance, float waveRadius)
    {
        if(distance <= waveRadius)
        {
            float t = Mathf.InverseLerp(waveRadius - 1F, waveRadius, distance);
            targetAlpha = Mathf.Lerp(visibleAlpha, hiddenAlpha, t);
        }
        else
        {
            targetAlpha = hiddenAlpha;
        }
    }

    private void SetAlpha(float a)
    {
        Color c = baseColor;
        c.a = a;
        sprite.color = c;
    }

    public void Interact()
    {

        var inventory = FindObjectOfType<Inventory>();


        switch (type)
        {

            case InvestigateType.Empty:
                TryHide();
                break;
            case InvestigateType.item:
                HandleItemInteraction(inventory);
                break;
            case InvestigateType.Trap:
                TrapTrigger(); 
                break;
            case InvestigateType.proviso:
                catchProviso();
                break;
            case InvestigateType.special:
                FindSpecial();
                break;
        }
    }

    private void HandleItemInteraction(Inventory inventory)
    {
        if (!inventory.hasItem)
        {
            inventory.Pickup(dataName);
            ConvertToEmpty();
        }
        else
        {
            string dropped = inventory.DropItem();
            inventory.Pickup(dataName);
            dataName = dropped;
        }
    }


    private void ConvertToEmpty()
    {
        type = InvestigateType.Empty;
        dataName = "";

    }


    private void TrapTrigger()
    {
        Debug.Log(" ���� �ߵ�! ");
        //���߿� ������ ���� �޼��� �ۼ�.
    }
    private void catchProviso()
    {
        Debug.Log($"�ܼ� �߰�({dataName}");
        //���߿� �ܼ��� ���� �޼��� �ۼ�.
    }

    private void FindSpecial()
    {
        Debug.Log($"Ư�� ������ �߰�:{dataName} ");
        //���߿� Ư�������ۿ� ���� �޼��� �ۼ�.
    }

    private void TryHide()
    {
        Debug.Log("������� �����ϴ�");
        //���߿� ��ﰣ�� ���� �޼��� �ۼ�.
    }
}



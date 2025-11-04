using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public enum InvestigateType
{
    Empty,  //헌팅타임시 숨을곳
    item,   //아이템
    Trap,   //함정
    proviso,// 단서
    special //격퇴아이템,금고,바닥문

}
[RequireComponent(typeof(SpriteRenderer),typeof(Collider2D))]
public class InvestigatePoint : MonoBehaviour
{
    public InvestigateType type;
    public string dataName;

    private SpriteRenderer sprite;
    private Color baseColor;

    [Header("투명도 설정")]
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
        Debug.Log(" 함정 발동이다요! ");
        trapManager.Instance.activeateTrap(dataName);
    }
    private void catchProviso()
    {
        Debug.Log($"단서 발견({dataName}");
        //나중에 단서에 대한 메서드 작성.
    }

    private void FindSpecial()
    {
        Debug.Log($"특수 아이템 발견:{dataName} ");
        //나중에 특수아이템에 대한 메서드 작성.
    }

    private void HandleSpecial()
    {
        // 유령 본체 감지
        if (dataName == "유령의 본체")
        {
            Debug.Log("…차가운 기운이 느껴진다요. 여긴 뭔가 있다요.");
            GhostBone.instance?.SetCurrentPoint(this);
            return;
        }

        Debug.Log($"특수 아이템 발견: {dataName}");
    }
    private void TryHide()
    {
        Debug.Log("빈공간에 숨는다요");
        //나중에 빈곤간에 숨는 메서드 작성.
    }
}



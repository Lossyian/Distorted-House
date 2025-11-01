using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class trapManager : MonoBehaviour
{
    public static trapManager Instance;

    private MapControlloer mapControlloer;
    private PlayerController player;
    private NoiseSystem noiseSystem;
    private Inventory inventory;
    private InvestigatePointController investigatePointController;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }
    void Start()
    {
        mapControlloer = FindObjectOfType<MapControlloer>();
        player = FindObjectOfType<PlayerController>();
        noiseSystem = FindObjectOfType<NoiseSystem>();
        inventory = FindObjectOfType<Inventory>();
        investigatePointController = FindObjectOfType<InvestigatePointController>();
    }

    public void activeateTrap(string trapName)
    {
        Debug.Log("�����̴ٿ�");
        switch(trapName)
        {
            case "��� �ظ��ϲ���.":
                TouchShuffleRoom();
                //���� ���̴� ����
                break;
            case "�� �ڿ��־�":
                DeathTrap();
                //��� ����(�̱���)
                break;
            case "�������� ����":
                NoiseParty();
                //���� ����(��� �߰���)
                break;
            case "������ �ʴ� ��":
                LockDoor();
                //����� ����
                break;
            case "�װ��� ����":
                stealItem();
                //������ �P��� ����
                break;
            case "������ �ð�":
                ProvisoEraser();
                //�ܼ� �������� ����
                break;
        }
    }

    private void TouchShuffleRoom()
    {
        if (mapControlloer != null)
        {
            mapControlloer.shuffleRoom();
        }
    }

    private void DeathTrap()
    {
        if(player != null)
        {
            Debug.Log("�׾��ٿ�..");
            //���߿� ���ӸŴ������� ó��.
        }
    }

    private void NoiseParty()
    {
        if (noiseSystem != null)
        {
            noiseSystem.AddNoise(100f);
        }
    }
    
    private void stealItem()
    {
        if (inventory != null&& inventory.hasItem)
        {
            string lostItem = inventory.DropItem();
        }
        else
        {
            Debug.Log(" ������ ���ٿ�..������.");
        }
    }
    private void ProvisoEraser()
    {
        if (investigatePointController!=null)
        {
            investigatePointController.removeRandom();
        }
    }
    private void LockDoor()
    {
        if (player == null) return;
        Room currentRoom = player.GetComponentInParent<Room>();
        if ( currentRoom != null)
        {
            currentRoom.LockrandomDoor();
        }
    }
}

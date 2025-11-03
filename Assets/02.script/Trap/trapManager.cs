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
        if(GameManager.hasCharm && inventory != null)
        {
            Debug.Log("부적이 지켜줬다요!");
            inventory.ConsumeItem("수상한 부적");
            return;
        }

        
        Debug.Log("함정이다욧");
        switch(trapName)
        {
            case "계속 해매일꺼야.":
                TouchShuffleRoom();
                //맵이 섞이는 함정
                break;
            case "네 뒤에있어":
                DeathTrap();
                //즉사 함정(미구현)
                break;
            case "무너지는 선반":
                noiseSystem.AddNoise(100f);
                //소음 함정(즉시 추격전)
                break;
            case "열리지 않는 문":
                LockDoor();
                //문잠김 함정
                break;
            case "그것을 원해":
                stealItem();
                //아이템 뻇기는 함정
                break;
            case "절망의 시간":
                ProvisoEraser();
                //단서 지워지는 함정
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
            Debug.Log("죽었다요..");
            //나중에 게임매니저에서 처리.
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
            Debug.Log(" 뺏을게 없다요..가난쓰.");
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

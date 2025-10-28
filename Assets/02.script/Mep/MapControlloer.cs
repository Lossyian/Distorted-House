using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MapControlloer : MonoBehaviour
{
    [SerializeField] private List<GameObject> roomPrefabs;

  
    [SerializeField] private LayerMask roomLayer;
    [SerializeField] private int maxAttempts = 20;

    private List<Room> PlacedRooms = new List<Room> ();
    private List<Door> openDoors = new List<Door>();


    public List<string> itemNames;


    void Start()
    {
        DrawingLobby();
        

    }

 

    public void DrawingLobby()
    {   // �� �������� ������ ����Ʈ�� ����ִٸ�, �ߴ�.
        if (roomPrefabs.Count == 0) return;
        //����Ʈ 0 �� ���� �κ� ȸ�� ���� �����϶�
        GameObject startDrawRoom = Instantiate(roomPrefabs[0],Vector2.zero,Quaternion.identity);
        //������ �κ��� ��ũ��Ʈ�� ������, StartRoom ������ ����.
        Room startRoom = startDrawRoom.GetComponent<Room>();
        //�̹� ��ġ�� �κ� ��� StartRoom������ PlacedRooms ����Ʈ�� ��� ��ġ ���� ����Ʈ�� ���� �ʵ��� �Ѵ�.
        PlacedRooms.Add(startRoom);

        // 1~9�� ����Ʈ�� ����� ����� ����Ʈ ���� 0���� �κ�� PlacedRooms�� �̹� ���������� ����.
        List<GameObject> remainingPrefabs = new List<GameObject>(roomPrefabs);
        remainingPrefabs.RemoveAt(0);
        // �κ��� ���� �������� Room ��ũ��Ʈ ���ִ� doors����Ʈ�� �߰�..
        openDoors.AddRange(startRoom.doors);

        Debug.Log(" �κ� ���� �Ϸ�");

        // ����� ����� ����Ʈ������ְ�, �濡 �ִ� ���� ���ٸ� �ߴ�.
        while (remainingPrefabs.Count > 0 && openDoors.Count>0)
        {
            
            //����Ʈ�� 0���� ��� ���� connectForm�� ��´�.
            Door connectForm = openDoors[0];
            //����Ʈ���� ��ݻ� 0���� �����Ѵ�.
            openDoors.RemoveAt(0);
            //������ ��ȣ�� ���� ����Ʈ���� ������, ������ �ű��, ���� ����Ʈ���� ����.
            GameObject newRoomprefab = remainingPrefabs[Random.Range(0, remainingPrefabs.Count)];
            remainingPrefabs.Remove(newRoomprefab);

            //��ŷ�� �޼����, ��ŷ�� �õ��Ѵ�.
            bool success = TryDokingRoom(connectForm, newRoomprefab);
            
            //���� �����Ѵٸ�...
            if (!success)
            {
                //�ٽ� ����Ʈ�� �ǵ�����.
                Debug.Log($"{newRoomprefab.name} �� ��ġ ����. �ٸ� ������ �õ��մϴ�.");
                remainingPrefabs.Add(newRoomprefab);
            }

        }
    }

    bool TryDokingRoom(Door attachTo,GameObject roomPrefab)
    {
        Debug.Log($"{roomPrefab.name}�� ��ŷ�� �õ��մϴ�. ");
        // �ִ� �õ� Ƚ����ŭ �ݺ� �� �ݺ��Ҷ����� i�� �� 1�� ����.
        for (int i = 0; i < maxAttempts; i++) 
        {   //roomprefab�ȿ� ����ִ¹�ȣ�� ���� ����
            GameObject newRoomObj = Instantiate(roomPrefab);
            //���� ��ũ��Ʈ ȣ��
            Room newRoom = newRoomObj.GetComponent<Room>();
            //Room ��ũ��Ʈ ���� �޼��带 ȣ����, ������ ���� ����
            Door newDoor = newRoom.GetRandomDoor();

            //�����ϰ� 90��,180��,270��,360�� ȸ�� ��Ų��.
            newRoom.transform.rotation = Quaternion.Euler(0, 0, 90 * Random.Range(0, 4));

            //���� �湮 ��ġ��ŭ ���θ���湮�� �̵����Ѷ�. ����� �׸�ŭ ��������.
            Vector3 offset = attachTo.transform.position - newDoor.transform.position;
            newRoom.transform.position += offset;

            //�� ���� ������ ������ �˻�.
            float dotProduct = Vector2.Dot(attachTo.transform.up, newDoor.transform.up);
            if (dotProduct > -0.99f)
            {
                Destroy(newRoomObj);
                continue; 
            }


            // �ٸ� ���� �ݶ��̴��� �浹�ϴ��� �˻�
            Debug.Log(" �ٸ���� ��ġ���� �˻��մϴ�.");
            BoxCollider2D roomCollider = newRoom.GetComponent<BoxCollider2D>();
            roomCollider.enabled = false;
            Vector2 boxSize = roomCollider.size;
            Vector2 boxCenter = (Vector2)newRoom.transform.position + roomCollider.offset;
            float roomAngle = newRoom.transform.eulerAngles.z;

            Collider2D overlap = Physics2D.OverlapBox(boxCenter, boxSize, roomAngle, roomLayer);

            roomCollider.enabled = true;

            if (overlap == null)
            {
                Debug.Log("�ݶ��̴� �浹 ���� �� ��ġ �Ϸ�");
                //���� �ߴٸ�,
                attachTo.connectedDoor=newDoor;
                newDoor.connectedDoor=attachTo;

                PlacedRooms.Add(newRoom);
                // �� ���� ������ ���� ����Ʈ�� �߰�.
                foreach (var d in newRoom.doors)
                    if (d != newDoor)
                        openDoors.Add(d);

                return true;
            }
            Destroy(newRoomObj);

        }
        return false;
        
        
    }
    public InvestigatePointController investigatePointController;
    
    private void OnEnable()
    {
        ItemTableLoader.OnLoadCompleted += OnCSVLoaded;
    }
     
    private void OnDisable()
    {
        ItemTableLoader.OnLoadCompleted -= OnCSVLoaded;
    }

    private void OnCSVLoaded()
    {
        investigatePointController.allRooms = new List<Room>(PlacedRooms);
        investigatePointController.StartDistribution();
    }


}

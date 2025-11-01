using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class MapControlloer : MonoBehaviour
{
    [SerializeField] private List<GameObject> roomPrefabs;

  
    [SerializeField] private LayerMask roomLayer;
    [SerializeField] private int maxAttempts = 20;

    [SerializeField] private Room spawnRoomPrefab;

    private List<Room> PlacedRooms = new List<Room> ();
    private List<Door> openDoors = new List<Door>();

    public Transform reSpawn;

    public List<string> itemNames;
    public InvestigatePointController investigatePointController;

    void Start()
    {
        DrawingLobby();
        

    }

 

    public void DrawingLobby()
    {   // 방 프레펩을 저장한 리스트가 비어있다면, 중단.
        if (roomPrefabs.Count == 0) return;
        //리스트 0 즉 맵의 로비를 회전 없이 생성하라
        GameObject startDrawRoom = Instantiate(roomPrefabs[0],Vector2.zero,Quaternion.identity);
        //생성된 로비의 스크립트를 꺼내어, StartRoom 변수에 저장.
        Room startRoom = startDrawRoom.GetComponent<Room>();
        //이미 배치된 로비가 담긴 StartRoom변수를 PlacedRooms 리스트에 담아 배치 전의 리스트에 들어가지 않도록 한다.
        PlacedRooms.Add(startRoom);

        // 1~9번 리스트에 저장된 방들의 리스트 복제 0번인 로비는 PlacedRooms에 이미 저장했으니 제외.
        List<GameObject> remainingPrefabs = new List<GameObject>(roomPrefabs);
        remainingPrefabs.RemoveAt(0);
        // 로비의 문을 프레펩의 Room 스크립트 에있는 doors리스트에 추가..
        openDoors.AddRange(startRoom.doors);

        Debug.Log(" 로비 생성 완료");

        // 저장된 방들의 리스트가비어있고, 방에 있는 문도 없다면 중단.
        while (remainingPrefabs.Count > 0 && openDoors.Count>0)
        {
            
            //리스트의 0번에 담긴 문을 connectForm에 담는다.
            Door connectForm = openDoors[0];
            //리스트에서 방금뺀 0번을 제거한다.
            openDoors.RemoveAt(0);
            //랜덤한 번호의 방을 리스트에서 가져와, 변수에 옮기고, 기존 리스트에선 삭제.
            GameObject newRoomprefab = remainingPrefabs[Random.Range(0, remainingPrefabs.Count)];
            remainingPrefabs.Remove(newRoomprefab);

            //도킹룸 메서드로, 도킹을 시도한다.
            bool success = TryDokingRoom(connectForm, newRoomprefab);
            
            //만약 실패한다면...
            if (!success)
            {
                //다시 리스트로 되돌린다.
                Debug.Log($"{newRoomprefab.name} 의 배치 실패. 다른 문으로 시도합니다.");
                remainingPrefabs.Add(newRoomprefab);
            }

        }
    }

    bool TryDokingRoom(Door attachTo,GameObject roomPrefab)
    {
        Debug.Log($"{roomPrefab.name}의 도킹을 시도합니다. ");
        // 최대 시도 횟수만큼 반복 및 반복할때마다 i의 수 1씩 증가.
        for (int i = 0; i < maxAttempts; i++) 
        {   //roomprefab안에 들어있는번호의 방을 생성
            GameObject newRoomObj = Instantiate(roomPrefab);
            //방의 스크립트 호출
            Room newRoom = newRoomObj.GetComponent<Room>();
            //Room 스크립트 안의 메서드를 호출해, 랜덤의 문을 지정
            Door newDoor = newRoom.GetRandomDoor();

            //랜덤하게 90도,180도,270도,360도 회전 시킨다.
            newRoom.transform.rotation = Quaternion.Euler(0, 0, 90 * Random.Range(0, 4));

            //기존 방문 위치만큼 새로만든방문을 이동시켜라. 방또한 그만큼 움직여라.
            Vector3 offset = attachTo.transform.position - newDoor.transform.position;
            newRoom.transform.position += offset;

            //두 문의 각도가 같게!
            float dotProduct = Vector2.Dot(attachTo.transform.up, newDoor.transform.up);
            if (dotProduct > -0.99f)
            {
                Destroy(newRoomObj);
                continue; 
            }


            // 다른 방의 콜라이더와 충돌하는지 검사
            Debug.Log(" 다른방과 겹치는지 검사합니다.");
            BoxCollider2D roomCollider = newRoom.GetComponent<BoxCollider2D>();
            roomCollider.enabled = false;
            Vector2 boxSize = roomCollider.size;
            Vector2 boxCenter = (Vector2)newRoom.transform.position + roomCollider.offset;
            float roomAngle = newRoom.transform.eulerAngles.z;

            Collider2D overlap = Physics2D.OverlapBox(boxCenter, boxSize, roomAngle, roomLayer);

            roomCollider.enabled = true;

            if (overlap == null)
            {
                Debug.Log("콜라이더 충돌 없음 방 설치 완료");
                //성공 했다면,
                attachTo.ConnectDoor(newDoor);
                

                PlacedRooms.Add(newRoom);
                // 이 방의 나머지 문은 리스트에 추가.
                foreach (var d in newRoom.doors)
                    if (d != newDoor)
                        openDoors.Add(d);

                return true;
            }
            Destroy(newRoomObj);

        }
        return false;
        
        
    }



    public void shuffleRoom()
    {
        Debug.Log("맵 재생성 + 단서/아이템/함정/특수 유지");

        //  기존 조사포인트 데이터 저장
        List<InvestigatePointData> savedPoints = new List<InvestigatePointData>();
        foreach (Room room in PlacedRooms)
        {
            foreach (InvestigatePoint p in room.points)
            {
                if (p.type != InvestigateType.Empty)
                    savedPoints.Add(new InvestigatePointData(p.type, p.dataName));
            }
        }

        // 기존 맵 삭제
        foreach (Room room in PlacedRooms)
            Destroy(room.gameObject);
        PlacedRooms.Clear();
        openDoors.Clear();

        //  맵 재생성
        DrawingLobby();

        //  새 조사포인트 수집
        List<InvestigatePoint> newPoints = PlacedRooms.SelectMany(r => r.points).ToList();

        //  기존 데이터 섞어서 새 포인트에 할당
        Shuffle(savedPoints);
        Shuffle(newPoints);

        
        for (int i = 0; i < newPoints.Count; i++)
        {
            if (i < savedPoints.Count)
            {
                newPoints[i].type = savedPoints[i].type;
                newPoints[i].dataName = savedPoints[i].dataName;
            }
            else
            {
                newPoints[i].type = InvestigateType.Empty;
                newPoints[i].dataName = "";
            }
        }

        //  조사포인트 컨트롤러 갱신
        investigatePointController.allRooms = new List<Room>(PlacedRooms);
        investigatePointController.collectAllPoints();

        //  플레이어 로비 ReSpawn
        var player = FindObjectOfType<PlayerController>();
        if (player != null && spawnRoomPrefab != null)
        {
            Room spawnRoom = PlacedRooms.Find(r => r.name.Contains(spawnRoomPrefab.name));
            if (spawnRoom != null)
            {
                Transform spawnPoint = spawnRoom.transform.Find("ReSpawn");
                if (spawnPoint != null)
                    player.transform.position = spawnPoint.position;
                else
                    Debug.LogWarning("리스폰 포인트 없음");
            }
        }

        //  문 연결 재도킹
        List<Door> allDoors = new List<Door>();
        foreach (Room room in PlacedRooms)
            allDoors.AddRange(room.doors);

        while (allDoors.Count > 0)
        {
            Door connectDoor = allDoors[0];
            allDoors.RemoveAt(0);

            Room targetRoom = PlacedRooms[Random.Range(0, PlacedRooms.Count)];
            Door targetDoor = targetRoom.GetRandomDoor();

            if (targetDoor != connectDoor && connectDoor.connectedDoor == null && targetDoor.connectedDoor == null)
            {
                connectDoor.ConnectDoor(targetDoor);
            }
        }
    }


    private void Shuffle<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int rand = Random.Range(i, list.Count);
            (list[i], list[rand]) = (list[rand], list[i]);
        }
    }


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
public class InvestigatePointData
{
    public InvestigateType type;
    public string dataName;
    public InvestigatePointData(InvestigateType t, string name)
    {
        type = t;
        dataName = name;
    }
}
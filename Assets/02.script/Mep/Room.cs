using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public List<Door> doors;

    public Door GetRandomDoor()
    {    // ���� ��ӹ��� �ڽĿ�����Ʈ �� �� �������� �ϳ� ����)
        return doors[Random.Range(0, doors.Count)];
    }
}

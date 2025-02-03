using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] Transform player;  // ĳ������ Transform�� �Ҵ��� ����
    [SerializeField] Vector3 offset;    // ī�޶�� ĳ���� ������ �Ÿ� (������)

    // LateUpdate�� ��� ������Ʈ�� ���� �� ȣ��� (�÷��̾� �̵� ���� ī�޶� �̵�)
    void LateUpdate()
    {
        // ī�޶��� ��ġ�� ĳ���� ��ġ�� ���� ������Ʈ (ī�޶��� Z���� ����)
        transform.position = new Vector3(transform.position.x, player.position.y + offset.y, transform.position.z);
    }
}

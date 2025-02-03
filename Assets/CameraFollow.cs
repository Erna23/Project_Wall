using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] Transform player;  // 캐릭터의 Transform을 할당할 변수
    [SerializeField] Vector3 offset;    // 카메라와 캐릭터 사이의 거리 (고정값)

    // LateUpdate는 모든 업데이트가 끝난 후 호출됨 (플레이어 이동 이후 카메라 이동)
    void LateUpdate()
    {
        // 카메라의 위치를 캐릭터 위치에 맞춰 업데이트 (카메라의 Z축은 유지)
        transform.position = new Vector3(transform.position.x, player.position.y + offset.y, transform.position.z);
    }
}

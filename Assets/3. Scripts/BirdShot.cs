using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BirdShot : MonoBehaviour
{
    // LineRenderer 배열 : 발사 각도 시각화
    public LineRenderer[] lineRenderers;
    // 발사 지점 위치 배열
    public Transform[] stripPositions;
    // 발사 지점
    public Transform center; 
    // Idle 상태의 발사 위치
    public Transform idlePosition; 
    // 새 종류 배열
    public Sprite[] birdSprites; 

    // 현재 위치
    public Vector3 currentPosition;
    // 최대 길이 (발사 거리)
    public float maxLength;
    // 하단 (발사 가능 범위)
    public float bottomBoundary;

    // 마우스 클릭 상태 여부
    bool isMouseDown;

   
    public GameObject birdPrefab;
    public GameObject target;
    public float birdPositionOffset;
    Rigidbody2D bird;
    Collider2D birdCollider;

    // 발사력
    public float force;

    void Start()
    {
        // Line Renderer 의 포지션 카운트를 2로 설정
        lineRenderers[0].positionCount = 2;
        lineRenderers[1].positionCount = 2;
        // Line Renderer 의 시작 위치를 설정
        lineRenderers[0].SetPosition(0, stripPositions[0].position);
        lineRenderers[1].SetPosition(0, stripPositions[1].position);

        // 게임 시작 시 새 초기화
        StartGameBird();
    }

    void StartGameBird()
    {
        // 새 인스턴스 생성
        bird = Instantiate(birdPrefab).GetComponent<Rigidbody2D>();
        birdCollider = bird.GetComponent<Collider2D>();
        // 콜라이더 비활성화 (발사 전까지)
        birdCollider.enabled = false;
        
        // 랜덤한 새 스프라이트 설정
        SpriteRenderer spriteRenderer = bird.GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = birdSprites[UnityEngine.Random.Range(0, birdSprites.Length)];

        // 물리적 움직임 비활성화
        bird.isKinematic = true;
        ResetStrips(); // 발사 시각화 초기화
    }

    void Update()
    {
        // 마우스 클릭 중인 경우
        if (isMouseDown)
        {
            // 마우스 위치 가져오기
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = 10;

            // 마우스 위치를 월드 좌표로 변환
            currentPosition = Camera.main.ScreenToWorldPoint(mousePosition);
            // 최대 발사 거리로 클램프
            currentPosition = center.position + Vector3.ClampMagnitude(
                currentPosition - center.position, maxLength);
            // 경계 내로 위치 클램프
            currentPosition = ClampBoundary(currentPosition);

            // LineRenderer 위치 설정
            SetStrips(currentPosition);

            // 발사 전 Collider 활성화
            if (birdCollider)
            {
                birdCollider.enabled = true;
            }
        }
        else
        {
            // 마우스가 떼어졌을 때 초기화
            ResetStrips();
        }
    }

    void Shot()
    {
        // 새가 존재하는 경우
        if (bird != null)
        {
            bird.isKinematic = false;
            Vector3 birdForce = (currentPosition - center.position) * force * -1; // 발사 방향 계산
            bird.velocity = birdForce;
            bird = null;
            birdCollider = null;
            Invoke("StartGameBird", 4);
        }
        else
        {
            Debug.Log("새로운 새 생성까지 잠시만 기다려주세요!"); // 새가 없을 때 줄을 당길 시 메시지 출력
        }
    }

    private void OnMouseDown()
    {
        // 마우스 클릭 시작
        isMouseDown = true;
    }

    private void OnMouseUp()
    {
        // 마우스 클릭 종료
        isMouseDown = false;
        Shot(); // 발사
    }

    void ResetStrips()
    {
        // 발사 위치를 idlePosition으로 초기화
        currentPosition = idlePosition.position;
        SetStrips(currentPosition);
    }

    void SetStrips(Vector3 position)
    {
        // LineRenderer 의 끝 위치 설정
        lineRenderers[0].SetPosition(1, position);
        lineRenderers[1].SetPosition(1, position);
        if (bird)
        {
            // 새의 위치, 방향
            Vector3 dir = position - center.position;
            bird.transform.position = position + dir.normalized * birdPositionOffset;
            bird.transform.right = -dir.normalized;
        }

        // 목표물 이동
        if (target != null)
        {
            target.transform.position = position; // 지지대 이동
        }
    }

    Vector3 ClampBoundary(Vector3 vector)
    {
        // y 좌표를 하단 경계로 클램프
        vector.y = Mathf.Clamp(vector.y, bottomBoundary, 1000);
        return vector;
    }
}

    
    
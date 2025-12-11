using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using UnityEngine;

public class GuidelineRender : MonoBehaviour
{
    [Header("점")]
    public GameObject dotPrefab;
    [Tooltip("점들 사이의 간격(월드 유닛)")]
    public float dotSpacing = 2f;
    [Tooltip("점들의 스케일")]
    public Vector3 dotScale = new Vector3(0.5f, 0.5f, 0.5f);

    [Space]
    [Header("시뮬레이션 관련")]
    [Tooltip("최대 경로 길이")]
    public float maxPathLength = 25f;
    [Tooltip("최대 반사 횟수")]
    public int maxBounces = 3;
    [Tooltip("충돌 레이어 마스크")]
    public LayerMask collisionMasks;
    [Tooltip("풀 사이즈")]
    public int poolSize = 30;

    private List<GameObject> dotPool = new List<GameObject>();
    private List<GameObject> activeDots = new List<GameObject>();

    private static readonly int ColorID = Shader.PropertyToID("_Color");

    private void Awake()
    {
        if(collisionMasks.value == 0)
        {
            collisionMasks = LayerMask.GetMask("Default");
            if(LayerMask.NameToLayer("Donut") != -1)
            {
                collisionMasks |= LayerMask.GetMask("Donut");
            }
        }
        InitializePool();
    }

    private void InitializePool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject dot = Instantiate(dotPrefab, transform);
            dot.SetActive(false);
            dotPool.Add(dot);
        }
        Debug.Log($"Initialized dot pool with {poolSize} dots.");
    }

    /// <summary>
    /// 풀에서 사용 가능한 Dot을 가져옵니다. 없으면 새로 생성
    /// </summary>
    /// <returns></returns>
    private GameObject GetDotFromPool()
    {
        for(int i = 0; i < dotPool.Count; i++)
        {
            if (!dotPool[i].activeInHierarchy)
            {
                dotPool[i].SetActive(true);
                return dotPool[i];
            }
        }
        
        GameObject newDot = Instantiate(dotPrefab, transform);
        newDot.SetActive(true);
        dotPool.Add(newDot);
        return newDot;
    }
     public void DrawGuideline(Vector3 startPosition, Vector3 shotDirection, float maxDistance)
    {
        // 1. 이전 점들 풀로 반환
        ReturnActiveDotsToPool();
        
        // 2. 경로를 따라 점 배치
        float remainingDistance = Mathf.Min(maxDistance, maxPathLength);
        Vector3 currentPosition = startPosition;
        Vector3 currentDirection = shotDirection.normalized;
        int bounces = 0;
        float segmentDistance = 0f;

        float totalDistanceTraveled = 0f;

        float nextDotDistance = 0f;

        // 점 간격만큼 이동하며 점을 배치합니다.
        while(remainingDistance > 0f && bounces <= maxBounces)
        {
            RaycastHit hit;
            float rayLength = remainingDistance;

            Vector3 rayStart = currentPosition + currentDirection * 0.1f; // 약간 앞에서 시작
            float dotClearance = 0.1f;

            if(Physics.Raycast(rayStart, currentDirection, out hit, rayLength + dotClearance, collisionMasks))
            {
                float distanceToHit = hit.distance + dotClearance;

                PlaceDotsInSegment(currentPosition, currentDirection, distanceToHit, ref totalDistanceTraveled, ref nextDotDistance);

                currentPosition = hit.point + hit.normal * 0.01f;
                currentDirection = Vector3.Reflect(currentDirection, hit.normal);
                remainingDistance -= distanceToHit;
                bounces++;
            }
            else
            {
                PlaceDotsInSegment(currentPosition, currentDirection, remainingDistance, ref totalDistanceTraveled, ref nextDotDistance);
                remainingDistance = 0f;
            }
        }
    }

    private void PlaceDotsInSegment(Vector3 segmentStart, Vector3 direction, float segmentLength, ref float totalDistanceTraveled, ref float nextDotDistance)
    {
        float segmentEndDistance = totalDistanceTraveled + segmentLength;
        
        // 다음 점 배치 위치(nextDotDistance)가 세그먼트의 시작점을 넘어섰는지 확인하며 루프를 돌립니다.
        while (nextDotDistance < segmentEndDistance)
        {
            // 현재 세그먼트 내에서의 점 위치 계산
            float distanceInSegment = nextDotDistance - totalDistanceTraveled;
            
            // 점 위치
            Vector3 dotPosition = segmentStart + direction * distanceInSegment;
            
            if (activeDots.Count < poolSize)
            {
                GameObject dot = GetDotFromPool();
                dot.transform.position = dotPosition;
                dot.transform.localScale = dotScale;
                dot.transform.rotation = Quaternion.identity; // 회전 초기화 (원형 점이므로 중요)
                activeDots.Add(dot);
            }
            else
            {
                // 풀 크기 초과 시 중단
                break;
            }

            // 다음 점 배치 위치 업데이트
            nextDotDistance += dotSpacing;
        }

        totalDistanceTraveled = segmentEndDistance;
    }

    /// <summary>
    /// 모든 활성 Dot을 숨기고 풀로 돌려보냅니다.
    /// </summary>
    public void HideGuideline()
    {
        ReturnActiveDotsToPool();
    }

    private void ReturnActiveDotsToPool()
    {
        foreach (var dot in activeDots)
        {
            dot.SetActive(false);
        }
        activeDots.Clear();
    }
}
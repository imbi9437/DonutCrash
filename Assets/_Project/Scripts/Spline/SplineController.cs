using UnityEngine;
using UnityEngine.Splines;

[RequireComponent(typeof(SplineContainer))]
public class SplineController : MonoBehaviour
{
    private Spline _spline;
    
    // 타원 가이드라인의 기본 크기 및 민감도 상수
    private const float BaseSize = 3.0f; // 초기 원 크기
    private const float MaxStretch = 30f;
    [SerializeField] private float Sensitivity = 1f; // 드래그 민감도

    private void Awake()
    {
        if (TryGetComponent(out SplineContainer container) == false)
        {
            Debug.LogWarning($"<color=yellow>[Spline Controller] Spline Controller requires a component type of SplineContainer</color>");
        }

        if (container.Spline == null)
            container.Spline = new Spline();

        _spline = container.Spline;
    }
    
    /// <summary>
    /// 스플라인을 통한 원을 그리는 메서드입니다.
    /// 스플라인이 포함된 오브젝트는 월드 좌표 (0, 0, 0)에 두고 해당 메서드를 통해 제어해야만 의도한 대로 결과물이 출력됩니다.
    /// </summary>
    /// <param name="mainFoci">원의 머리가 될 집중점 (도넛 위치)</param>
    /// <param name="subFoci">원의 꼬리가 될 집중점 (드래그 방향)</param>
    public void SetEllipse(Vector3 mainFoci, Vector3 subFoci)
    {
        // Show();
        
        // Y 좌표를 0.2f로 고정합니다.
        mainFoci.y = 0.2f;
        subFoci.y = 0.2f;

        // 드래그 방향 벡터. (시작점 -> 당기는 지점)
        Vector3 dir = - subFoci + mainFoci; 

        float magnitude = dir.magnitude;
        
        // [수정] magnitude가 0에 가까우면 기본 원형을 유지합니다.
        if (magnitude < 0.001f)
        {
            magnitude = 0.001f; // 0으로 나누는 것을 방지
            dir = Vector3.forward; 
        }

        // 회전 계산: Z축이 dir을 향하도록 회전합니다.
        Quaternion rot = Quaternion.LookRotation(dir.normalized, Vector3.up); // dir을 정규화하여 회전 계산의 정확도를 높입니다.

        // 늘어나는 길이: 드래그 크기에 비례하여 BaseSize에서 MaxStretch까지 늘어납니다.
        float stretchLength = Mathf.Clamp(magnitude * Sensitivity, BaseSize, MaxStretch);
        
        // 타원의 폭: 늘어날수록 폭이 좁아져 길쭉해지는 효과를 줍니다.
        // 늘어나는 정도(magnitude)를 0~MaxStretch 범위로 정규화합니다.
        float normalizedMagnitude = Mathf.Clamp01(magnitude * Sensitivity / MaxStretch); 
        
        float widthLength = BaseSize / Mathf.Lerp(1f, 5f, normalizedMagnitude);
        widthLength = Mathf.Clamp(widthLength, 0.1f, BaseSize);


        BezierKnot[] knots = new BezierKnot[4];
        
        // --- 1. Knot Position 계산 ---
        // Knot 0: Left (폭)
        Vector3 knotPos0 = rot * (Vector3.left * widthLength) + mainFoci;
        // Knot 1: Forward (당기는 방향, 늘어남)
        Vector3 knotPos1 = rot * (Vector3.forward * stretchLength) + mainFoci;
        // Knot 2: Right (폭)
        Vector3 knotPos2 = rot * (Vector3.right * widthLength) + mainFoci;
        // Knot 3: Back (당기는 방향의 반대, 베이스)
        Vector3 knotPos3 = rot * (Vector3.back * widthLength) + mainFoci;

        // 모든 Y 좌표를 0.2f로 고정합니다.
        knotPos0.y = 0.2f; knotPos1.y = 0.2f; knotPos2.y = 0.2f; knotPos3.y = 0.2f;
        
        knots[0] = new BezierKnot(knotPos0);
        knots[1] = new BezierKnot(knotPos1);
        knots[2] = new BezierKnot(knotPos2);
        knots[3] = new BezierKnot(knotPos3);
        
        _spline.Knots = knots;
        
        // 탄젠트 모드 설정
        _spline.SetTangentMode(TangentMode.AutoSmooth); 
    }
}
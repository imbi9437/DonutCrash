using _Project.Scripts.InGame;
using UnityEngine;


[RequireComponent(typeof(Rigidbody), typeof(Collider), typeof(DonutObject))]
public class DonutMove : MonoBehaviour
{
    private Rigidbody _rigidbody;

    private const float ForceMultiplier = 12f;

    [Space]
    [Header("물리 설정")]
    [SerializeField] private float dragCoefficient = 0.98f; //마찰 감쇠
    // [SerializeField] private float minVelocity = 0.05f; //최소 속도


    // [Space]
    // [Header("최대 드래그 거리 설정")]
    // [SerializeField] private float maxDragDistance = 200f; //최대 드래그 거리 (픽셀 단위) 또는 최대 힘의 기준

    // [Space]
    // [Header("최소 드래그 거리 설정")]
    // [SerializeField] private float minDragDistance = 50f; //최소 드래그 거리 (픽셀 단위) 또는 최소 힘의 기준

    public float Speed => _rigidbody.velocity.magnitude;

    void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        if(_rigidbody == null)
        {
            Debug.LogError($"{name}에 Rigidbody가 없습니다! DonutMove가 정상작동하지 않습니다.");
        }
    }
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();

        _rigidbody.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationY;
    }
    
    void FixedUpdate()
    {
        if (_rigidbody.isKinematic == false)
            ApplyDragAndCheckStop();
    }
    
    public void Shot(Vector2 force)
    {
        //월드의 Y축 (수직)힘은 0으로 설정
        Vector3 worldForce = new (force.x, 0, force.y);

        //Rigidbody에 힘 적용
        _rigidbody.AddForce(worldForce * ForceMultiplier, ForceMode.Impulse);
    }

    private void ApplyDragAndCheckStop()
    {
        if (_rigidbody.velocity.magnitude > .01f)
        {
            _rigidbody.velocity *= dragCoefficient;
        }
        else
        {
            _rigidbody.velocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;
        }
    }

    public void SetSpeed(float speed) => _rigidbody.velocity = _rigidbody.velocity.normalized * speed;
    public bool IsStop() => (_rigidbody?.velocity.magnitude ?? 0f) < .5f;
    public void SetKinematic(bool value)
{
    if (_rigidbody == null)
    {
        Debug.LogWarning($"{name}: Rigidbody가 null 상태입니다. SetKinematic 호출 무시.");
        return;
    }

    _rigidbody.isKinematic = value;
}
}
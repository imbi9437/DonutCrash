using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

[RequireComponent(typeof(SplineContainer))]
public class SplineMeshRenderer : MonoBehaviour
{
    [SerializeField] private int subdivisions = 64;
    [SerializeField] private Material material;

    public Material Material => _meshRenderer?.material;

    private Spline _spline;
    private bool _dirty;
    private Vector3[] _points;
    private SplineContainer _splineContainer;
    private MeshFilter _meshFilter;
    private MeshRenderer _meshRenderer;
    private Mesh _mesh;

    void Awake()
    {
        _splineContainer = GetComponent<SplineContainer>();
        _spline = _splineContainer.Spline;
        _meshFilter = gameObject.AddComponent<MeshFilter>();
        _meshRenderer = gameObject.AddComponent<MeshRenderer>();
        _meshRenderer.material = material;
        
        _mesh = new Mesh();
        _meshFilter.mesh = _mesh;
    }

    void OnEnable()
    {
        Spline.Changed += OnSplineChanged;
        _dirty = true;
    }

    void OnDisable()
    {
        Spline.Changed -= OnSplineChanged;
    }

    private void OnSplineChanged(Spline spline, int knotIndex, SplineModification modificationType)
    {
        if (_splineContainer.Spline != spline)
            return;

        _spline = _splineContainer.Spline;
        _dirty = true;
    }

    void Update()
    {
        if (!_dirty)
            return;

        _dirty = false;

        if (_spline == null)
            return;
        
        _mesh.Clear(); 

        if (_points?.Length != subdivisions + 1)
            _points = new Vector3[subdivisions + 1];
        
        Vector2[] uv = new  Vector2[subdivisions + 1]; 
        var trs = transform.worldToLocalMatrix;

        _points[0] = math.transform(trs, _spline.EvaluatePosition(.5f));
        for (int i = 0; i < subdivisions; i++)
        {
            float3 pos = math.transform(trs, _spline.EvaluatePosition(i / (subdivisions - 1f)));
            _points[i + 1] = pos;
            
        }
        
        int[] triangles = new int[(_points.Length - 1) * 3];
        for (int i = 1; i < _points.Length; i++)
        {
            int index = (i - 1) * 3;
            int start = 0;
            int current = i;
            int next = i % (_points.Length - 1) + 1;

            triangles[index] = current;
            triangles[index + 1] = next;
            triangles[index + 2] = start;
        }
        
        _mesh.vertices = _points;
        _mesh.triangles = triangles.ToArray();
        
        _mesh.RecalculateNormals(); 
    }
}
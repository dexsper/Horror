using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[ExecuteInEditMode]
public class FieldOfView : MonoBehaviour
{
    [Title("FieldView Properties")]
    [SerializeField] private float _distance = 10f;
    [SerializeField, Range(0, 180f)] private float _angle = 60f;
    [SerializeField, Range(0.1f, 5f)] private float _height = 4f;

    [Title("Scan Properties")]
    [SerializeField, Range(10, 60)] private float _scanFrequency = 30;
    [SerializeField] private LayerMask _scanMask;
    [SerializeField] private LayerMask _occlusionMask;

    [Title("Debug")]
    [SerializeField] private bool _debug = true;
    [SerializeField] private Color _distanceColor = Color.blue;
    [SerializeField] private Color _meshColor = Color.green;
    [SerializeField] private Color _scanObjectColor = Color.red;

    private Mesh _debugMesh;

    private float _scanInterval = 0f;
    private float _scanTimer = 0f;
    private int _scanObjectsCount = 0;
    private Collider[] _colliders = new Collider[10];
    private List<GameObject> _objects = new List<GameObject>();

    private void Start()
    {
        _scanInterval = 1.0f / _scanFrequency;
    }
    private void Update()
    {
        _scanTimer -= Time.deltaTime;

        if (_scanTimer <= 0f)
        {
            Scan();

            _scanTimer += _scanInterval;
        }
    }

    public bool IsInSight(GameObject obj)
    {
        Vector3 origin = transform.position;
        Vector3 dest = obj.transform.position;
        Vector3 direction = dest - origin;

        if (direction.y < -(_height / 4) || direction.y > _height)
            return false;

        direction.y = 0;
        float deltaAngle = Vector3.Angle(direction, transform.forward);

        if (deltaAngle > _angle)
            return false;

        origin.y += _height / 2f;
        dest.y = origin.y;

        if (Physics.Linecast(origin, dest, _occlusionMask))
            return false;

        return true;
    }
    private void Scan()
    {
        _scanObjectsCount = Physics.OverlapSphereNonAlloc(transform.position, _distance, _colliders, _scanMask, QueryTriggerInteraction.Collide);

        _objects.Clear();

        for (int i = 0; i < _scanObjectsCount; ++i)
        {
            GameObject obj = _colliders[i].gameObject;

            if (IsInSight(obj))
                _objects.Add(obj);
        }
    }

    private Mesh CreateWedgeMesh()
    {
        Mesh mesh = new Mesh();

        int segments = 14;
        int numTriangles = (segments * 4) + 2 + 2;
        int numVertices = numTriangles * 3;

        int[] triangles = new int[numVertices];
        Vector3[] vertices = new Vector3[numVertices];

        Vector3 bottomCenter = Vector3.zero;
        Vector3 bottomLeft = Quaternion.Euler(0, -_angle, 0) * Vector3.forward * _distance;
        Vector3 bottomRight = Quaternion.Euler(0, _angle, 0) * Vector3.forward * _distance;

        Vector3 topCenter = bottomCenter + Vector3.up * _height;
        Vector3 topLeft = bottomLeft + Vector3.up * _height;
        Vector3 topRight = bottomRight + Vector3.up * _height;

        int vert = 0;

        vertices[vert++] = bottomCenter;
        vertices[vert++] = bottomLeft;
        vertices[vert++] = topLeft;

        vertices[vert++] = topLeft;
        vertices[vert++] = topCenter;
        vertices[vert++] = bottomCenter;

        vertices[vert++] = bottomCenter;
        vertices[vert++] = topCenter;
        vertices[vert++] = topRight;

        vertices[vert++] = topRight;
        vertices[vert++] = bottomRight;
        vertices[vert++] = bottomCenter;

        float currentAngle = -_angle;
        float deltaAngle = (_angle * 2) / segments;

        for (int i = 0; i < segments; ++i)
        {
            bottomLeft = Quaternion.Euler(0, currentAngle, 0) * Vector3.forward * _distance;
            bottomRight = Quaternion.Euler(0, currentAngle + deltaAngle, 0) * Vector3.forward * _distance;

            topLeft = bottomLeft + Vector3.up * _height;
            topRight = bottomRight + Vector3.up * _height;

            vertices[vert++] = bottomLeft;
            vertices[vert++] = bottomRight;
            vertices[vert++] = topRight;

            vertices[vert++] = topRight;
            vertices[vert++] = topLeft;
            vertices[vert++] = bottomLeft;

            vertices[vert++] = topCenter;
            vertices[vert++] = topLeft;
            vertices[vert++] = topRight;

            vertices[vert++] = bottomCenter;
            vertices[vert++] = bottomRight;
            vertices[vert++] = bottomLeft;

            currentAngle += deltaAngle;
        }

        for (int i = 0; i < numVertices; ++i)
        {
            triangles[i] = i;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        return mesh;
    }

    private void OnValidate()
    {
        _debugMesh = CreateWedgeMesh();
        _scanInterval = 1.0f / _scanFrequency;
    }
    private void OnDrawGizmosSelected()
    {
        if (_debugMesh)
        {
            Gizmos.color = _meshColor;
            Gizmos.DrawMesh(_debugMesh, transform.position, transform.rotation);

            Gizmos.color = _distanceColor;
            Gizmos.DrawWireSphere(transform.position, _distance);

            for (int i = 0; i < _objects.Count; ++i)
            {
                Gizmos.color = _scanObjectColor;
                Gizmos.DrawSphere(_objects[i].transform.position, 0.5f);
            }
        }
    }
}
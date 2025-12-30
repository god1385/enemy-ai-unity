using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class EnemyFOVVisualizer : MonoBehaviour
{
    [SerializeField] private float viewAngle = 90f;
    [SerializeField] private int resolution = 30;
    [SerializeField] private float intervalToCheckTarget = 0.2f;
    [SerializeField] private Enemy enemyObject;
    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private LayerMask obstacleLayer;

    public float ViewRadius { get; set;}
    private LineRenderer line;
    private float timer;
    private Vector2 debugRay;

    private void Awake()
    {
        timer = intervalToCheckTarget;
        line = GetComponent<LineRenderer>();
        line.positionCount = resolution + 2;
    }

    private void Start()
    {
        if (ViewRadius <= 0f)
            ViewRadius = 5f;

        DrawFOV();
    }

    private void Update()
    {
        if (enemyObject.transform.rotation.eulerAngles.sqrMagnitude > 0.2f || enemyObject.transform.position.sqrMagnitude > 0.2f)
            DrawFOV();

        timer -= Time.deltaTime;

        if (timer < 0)
        {
            CheckVision();
            timer = intervalToCheckTarget;
        }
    }

    private void DrawFOV()
    {
        float angleStep = viewAngle / resolution;
        float startAngle = viewAngle / 2;

        line.SetPosition(0, transform.position);

        for (int i = 0; i <= resolution; i++)
        {
            float angle = startAngle + angleStep * i;
            Vector3 dir = DirFromAngle(angle);
            Vector3 point = transform.position + dir * ViewRadius;

            line.SetPosition(i + 1, point);
        }

        line.SetPosition(resolution + 1, transform.position);
    }

    Vector3 DirFromAngle(float angle)
    {
        angle += transform.eulerAngles.z;
        return new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad),
                           Mathf.Sin(angle * Mathf.Deg2Rad));
    }

    public void CheckVision()
    {
        Collider2D[] targets =
            Physics2D.OverlapCircleAll(transform.position, ViewRadius, targetLayer);

        foreach (var target in targets)
        {
            Vector2 dir = (target.transform.position - transform.position).normalized;
            debugRay = dir;
            if (Vector2.Angle(enemyObject.transform.up, dir) < viewAngle / 2)
            {
                float dist = Vector2.Distance(transform.position, target.transform.position);

                if (!Physics2D.Raycast(transform.position, dir, dist, obstacleLayer))
                {
                    enemyObject.SetFovData(true, target.transform);
                    return;
                }
            }
        }

        enemyObject.SetFovData(false, null);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, enemyObject.transform.up * 15f);
    }
}
using UnityEngine;

public class TunnelGenerator : MonoBehaviour
{
    public enum CellType { Empty = 0, Dynamic = 1, Fixed = 2 }

    [Header("Tunnel Size (world units)")]
    public float tunnelWidth = 20f;   // interior X span
    public float tunnelHeight = 20f;   // interior Y span
    public float tunnelLength = 300f;  // Z from entrance→exit

    [Header("Prefabs & Physics")]
    public GameObject dynamicPrefab;      // 1×1×1 cube w/ Rigidbody
    public GameObject fixedPrefab;        // 1×1×1 cube, no Rigidbody
    public PhysicsMaterial zeroFricMat;    // friction=0, bounce=0, combine=Minimum

    [Header("Boundary Settings")]
    public float cubeDepth = 1f;    // thickness of each slice
    public float boundaryThickness = 0.1f;
    public int cubeLayer = 8;
    public int boundaryLayer = 9;

    const int GRID = 7;
    const float COLLIDER_MARGIN = 0.99f;
    // shrink collider by 0.5% and set contactOffset
    const float CONTACT_OFFSET = 0.001f;

    CellType[,] obs1 = new CellType[GRID, GRID]
    {
        { CellType.Fixed, CellType.Fixed, CellType.Empty, CellType.Empty, CellType.Fixed, CellType.Fixed, CellType.Fixed },
        { CellType.Empty, CellType.Fixed, CellType.Fixed, CellType.Fixed, CellType.Fixed, CellType.Empty, CellType.Fixed },
        { CellType.Empty, CellType.Fixed, CellType.Empty, CellType.Empty, CellType.Empty, CellType.Fixed, CellType.Empty },
        { CellType.Empty, CellType.Dynamic, CellType.Fixed, CellType.Empty, CellType.Fixed, CellType.Dynamic, CellType.Empty },
        { CellType.Empty, CellType.Fixed, CellType.Empty, CellType.Empty, CellType.Empty, CellType.Fixed, CellType.Empty },
        { CellType.Fixed, CellType.Empty, CellType.Fixed, CellType.Fixed, CellType.Fixed, CellType.Fixed, CellType.Empty },
        { CellType.Fixed, CellType.Fixed, CellType.Fixed, CellType.Empty, CellType.Empty, CellType.Fixed, CellType.Fixed },
    };

    CellType[,] obs2 = new CellType[GRID, GRID]
    {
        { CellType.Fixed, CellType.Fixed, CellType.Fixed, CellType.Dynamic, CellType.Fixed, CellType.Fixed,   CellType.Fixed   },
        { CellType.Fixed, CellType.Fixed, CellType.Fixed, CellType.Dynamic, CellType.Fixed, CellType.Fixed,   CellType.Fixed   },
        { CellType.Fixed, CellType.Fixed, CellType.Fixed, CellType.Empty, CellType.Fixed, CellType.Fixed,   CellType.Fixed   },
        { CellType.Fixed, CellType.Fixed, CellType.Fixed, CellType.Empty, CellType.Fixed, CellType.Fixed,   CellType.Fixed   },
        { CellType.Fixed, CellType.Empty, CellType.Fixed, CellType.Empty, CellType.Fixed, CellType.Empty,   CellType.Fixed   },
        { CellType.Fixed, CellType.Empty, CellType.Empty, CellType.Empty, CellType.Empty, CellType.Empty,   CellType.Fixed   },
        { CellType.Fixed, CellType.Fixed, CellType.Fixed, CellType.Fixed, CellType.Fixed, CellType.Fixed,   CellType.Fixed   },
    };
    CellType[,] obs3 = new CellType[GRID, GRID]
    {
        { CellType.Dynamic, CellType.Dynamic, CellType.Dynamic, CellType.Dynamic, CellType.Dynamic, CellType.Dynamic, CellType.Dynamic },
        { CellType.Dynamic, CellType.Fixed,   CellType.Fixed,   CellType.Dynamic,   CellType.Fixed,   CellType.Fixed,   CellType.Dynamic },
        { CellType.Dynamic, CellType.Fixed,   CellType.Empty,   CellType.Empty,   CellType.Empty,   CellType.Fixed,   CellType.Dynamic },
        { CellType.Dynamic, CellType.Empty,   CellType.Empty,   CellType.Empty,   CellType.Empty,   CellType.Empty,   CellType.Dynamic },
        { CellType.Dynamic, CellType.Fixed,   CellType.Empty,   CellType.Empty,   CellType.Empty,   CellType.Fixed,   CellType.Dynamic },
        { CellType.Dynamic, CellType.Fixed,   CellType.Fixed,   CellType.Dynamic,   CellType.Fixed,   CellType.Fixed,   CellType.Dynamic },
        { CellType.Dynamic, CellType.Dynamic, CellType.Dynamic, CellType.Dynamic, CellType.Dynamic, CellType.Dynamic, CellType.Dynamic },
    };

    Transform obstaclesRoot;
    Vector3 center;

    void Awake()
    {
        // improve solver accuracy
        Physics.defaultSolverIterations = 12;
        Physics.defaultSolverVelocityIterations = 8;
    }

    void Start()
    {
        // assume this generator sits at tunnel center:
        center = transform.position;

        // create unscaled container so cubes remain true 1×1×1
        obstaclesRoot = new GameObject("ObstaclesRoot").transform;
        obstaclesRoot.SetParent(transform, true);
        obstaclesRoot.localPosition = Vector3.zero;

        SpawnObstacles();
    }

    void SpawnObstacles()
    {
        var slices = new CellType[][,] { obs1, obs2, obs3 };
        float dz = tunnelLength / (slices.Length + 1);

        // exact cell dims
        float cellX = tunnelWidth / GRID;
        float cellY = tunnelHeight / GRID;

        for (int i = 0; i < slices.Length; i++)
        {
            var pattern = slices[i];
            float zPos = center.z - tunnelLength / 2f + dz * (i + 1);

            // spawn 7×7 grid
            for (int y = 0; y < GRID; y++)
                for (int x = 0; x < GRID; x++)
                {
                    var type = pattern[y, x];
                    if (type == CellType.Empty) continue;

                    Vector3 pos = new Vector3(
                        center.x + (x - (GRID / 2f - 0.5f)) * cellX,
                        center.y + (y - (GRID / 2f - 0.5f)) * cellY,
                        zPos
                    );

                    // pick prefab
                    var prefab = (type == CellType.Dynamic) ? dynamicPrefab : fixedPrefab;
                    var cube = Instantiate(prefab, pos, Quaternion.identity);
                    cube.layer = cubeLayer;
                    cube.transform.SetParent(obstaclesRoot, true);

                    // VISUAL scale = full cell
                    cube.transform.localScale = new Vector3(cellX, cellY, cubeDepth);

                    // COLLIDER shrink + frictionless + tiny contact offset
                    var col = cube.GetComponent<Collider>();
                    if (col != null)
                    {
                        col.contactOffset = CONTACT_OFFSET;
                        col.material = zeroFricMat;

                        if (col is BoxCollider bc)
                            bc.size = Vector3.one * COLLIDER_MARGIN;
                    }

                    if (type == CellType.Dynamic)
                    {
                        var rb = cube.GetComponent<Rigidbody>() ?? cube.AddComponent<Rigidbody>();
                        rb.freezeRotation = true;
                        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
                        rb.interpolation = RigidbodyInterpolation.Interpolate;
                        rb.sleepThreshold = 0f;
                        rb.WakeUp();
                    }
                    else
                    {
                        // fixed: ensure absolutely no Rigidbody
                        var rb = cube.GetComponent<Rigidbody>();
                        if (rb != null) Destroy(rb);
                    }
                }

            // invisible front/back boundaries

        }
    }
}

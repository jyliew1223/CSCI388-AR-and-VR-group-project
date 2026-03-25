using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;

public class SpherePuzzleGameManager : MonoBehaviour
{
    // Struct for storing game data
    struct PuzzleState
    {
        public float puzzlePosition; // Y Position of GameObject
        public float puzzleEulerAngles; // Y Rotation of GameObject
        public PuzzleState(float position, float rotation)
        {
            puzzlePosition = position;
            puzzleEulerAngles = rotation;
        }
    }
    // enum for checking triggers
    enum StateTrigger
    {
        MinRotation,
        MaxRotation,
        MinZPos,
        MaxZPos
    }

    [SerializeField]
    private GameObject puzzle;
    [SerializeField]
    private TriggerBehaviour leftTrigger;
    [SerializeField]
    private TriggerBehaviour rightTrigger;
    [SerializeField]
    [Tooltip("Data sheet(CSV) for Puzzle Game")]
    private TextAsset dataSheet;
    [SerializeField]
    private float rotateSpeed = 10f;
    [SerializeField]
    [Tooltip("If true, the puzzle game starts automatically on scene load.")]
    private bool isAutoStart = false;
    [SerializeField]
    [Tooltip("Y threshold allowed for Rotation")]
    private float yRotationThreshold = .5f;
    [SerializeField]
    [Tooltip("If enabled, logs all debug message to the console during gameplay.")]
    private bool isConsoleEchoEnabled = false;

    // Puzzle Related
    private List<PuzzleState> states = new();
    private int currentState = 0;
    private StateTrigger stateTrigger;
    private float minRotationLimit, maxRotationLimit;

    // Component from others
    private ConfigurableJoint joint;

    // Ohter variables
    private bool isGameEnabled = false;
    private Vector3 startingPosition = new();
    private Quaternion startingRotation = new();
    private Coroutine gameCoroutine;

    private void Awake()
    {
        // Auto assign value if SerializeField is null
        if (puzzle == null)
        {
            puzzle = GameObject.Find("SpherePuzzle");
            if (puzzle != null)
            {
                Debug.LogWarning($"{this.GetType().Name}: 'Puzzle' not assigned, auto assigned");
            }
            else
            {
                Debug.LogError($"{this.GetType().Name}: failed to auto assign 'Puzzle', try assign it in Inspector");
            }
        }

        if (dataSheet == null)
        {
            dataSheet = Resources.Load<TextAsset>("SpherePuzzleStateData");
            if (puzzle != null)
            {
                Debug.LogWarning($"{this.GetType().Name}: 'Data Sheet' not assigned, auto assigned");
            }
            else
            {
                Debug.LogError($"{this.GetType().Name}: failed to auto assign 'Data Sheet', try assign it in Inspector");
            }
        }

        // Prompt message
        if (isConsoleEchoEnabled)
        {
            if (isAutoStart)
            {
                Debug.Log($"{this.GetType().Name}: AutoStart enabled!");
            }
            else
            {
                Debug.Log($"{this.GetType().Name}: AutoStart disabled!");
            }
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Getting other component
        if (!puzzle.TryGetComponent<ConfigurableJoint>(out joint))
        {
            Debug.LogError($"{this.GetType().Name}: {puzzle.name} does not have Configurable Joint Component");
        }

        // Getting game data from CSV file
        string consoleOut = "";
        string[] lines = dataSheet.text.Split('\n');
        if ((lines.Length > 0))
        {
            foreach (string line in lines)
            {
                string[] data = line.Split(",");
                if (data.Length > 0)
                {
                    try
                    {
                        // Normallize value to make it usable for the game
                        float position = float.Parse(data[0]);
                        float rotation = float.Parse(data[1]);
                        position *= -3f;
                        position -= 5f;
                        rotation *= -1f;
                        states.Add(new(position, rotation));
                        consoleOut += $"{position} {rotation}\n";
                    }
                    catch (FormatException)
                    {
                        continue;
                    }
                }
            }

            if (!isConsoleEchoEnabled)
            {
                Debug.Log(consoleOut);
            }
        }
        else
        {
            Debug.LogError($"{this.GetType().Name}: No data read from {dataSheet.name}");
        }

        // Getting puzzle starting transfrom for reset purposes
        startingPosition = puzzle.transform.position;
        startingRotation = puzzle.transform.rotation;

        // Auto Start check
        if (isAutoStart)
        {
            StartGame();
        }
    }
    // Update is called once per frame
    void Update()
    {

    }
    void StartGame()
    {
        // Reset transform in case it mess up
        puzzle.transform.position = startingPosition;
        puzzle.transform.rotation = startingRotation;

        // Update Limiters Once to get limiters in place
        UpdateLimitersAndTrigger();

        isGameEnabled = true;

        gameCoroutine = StartCoroutine(GameLoop());

        if (isConsoleEchoEnabled)
        {
            Debug.Log($"{this.GetType().Name}: Game started!");
        }
    }
    IEnumerator GameLoop()
    {
        // Check is the game currently running
        while (isGameEnabled)
        {
            ClampPuzzleRotation();

            if (leftTrigger.GetIsTrigger())
            {
                puzzle.transform.Rotate(Vector3.up * -rotateSpeed * Time.deltaTime);
            }
            if (rightTrigger.GetIsTrigger())
            {
                puzzle.transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime);
            }

            //Decide should game move into next/ previous state
            if (ShouldProceedToNextState())
            {
                currentState++;
                UpdateLimitersAndTrigger();
                if (isConsoleEchoEnabled)
                {
                    Debug.Log($"{this.GetType().Name}: Proceed to Next state!");
                }
            }
            else if (ShouldProceedToPreviousState())
            {
                currentState--;
                UpdateLimitersAndTrigger();
                if (isConsoleEchoEnabled)
                {
                    Debug.Log($"{this.GetType().Name}: Proceed to Previous state!");
                }
            }
            //Wait for next frame

            yield return null;
        }
    }
    void ClampPuzzleRotation()
    {
        if (minRotationLimit == 0 && currentState == 0)
        {
            minRotationLimit = 360;
        }

        bool warps = minRotationLimit > maxRotationLimit;
        float currentRotation = puzzle.transform.localEulerAngles.y;
        float clampedRotation;

        // iIf angle goes across 360
        if (!warps)
        {
            clampedRotation = Mathf.Clamp(currentRotation, minRotationLimit, maxRotationLimit);
        }
        else
        {
            if (currentRotation >= minRotationLimit || currentRotation <= maxRotationLimit)
            {
                // Angle within range
                clampedRotation = currentRotation;
            }
            else
            {
                // Angle out of range
                float distToMin = Mathf.DeltaAngle(currentRotation, minRotationLimit);
                float distToMax = Mathf.DeltaAngle(currentRotation, maxRotationLimit);
                clampedRotation = Mathf.Abs(distToMin) < Mathf.Abs(distToMax) ? minRotationLimit : maxRotationLimit;
            }
        }
        // Apply rotation
        puzzle.transform.localEulerAngles = new Vector3(puzzle.transform.localEulerAngles.x, clampedRotation, puzzle.transform.localEulerAngles.z);
    }
    void UpdateLimitersAndTrigger()
    {
        // End of game
        // Last state is exit therefore last state should be consider
        if (currentState >= states.Count - 1)
        {
            EndOfGame();
            return;
        }

        // Getting variables
        float currentRotation = states[currentState].puzzleEulerAngles;
        float nextRotation = states[currentState + 1].puzzleEulerAngles;
        float currentAnchorPos = states[currentState].puzzlePosition;
        float nextAnchorPos = states[currentState + 1].puzzlePosition;

        // Setting StateTrigger
        if (currentRotation == nextRotation)
        {
            // vertical path
            if (currentAnchorPos < nextAnchorPos)
            {
                // Puzzle should move up
                stateTrigger = StateTrigger.MaxZPos;
            }
            else
            {
                // Puzzle shold move down
                stateTrigger = StateTrigger.MinZPos;
            }

        }
        else if (currentAnchorPos == nextAnchorPos)
        {
            // horizontal path
            if (currentRotation > nextRotation)
            {
                // move anti-clockwise
                stateTrigger = StateTrigger.MinRotation;
            }
            else
            {
                // move clockwise
                stateTrigger = StateTrigger.MaxRotation;
            }
        }

        // checking rotation
        float minYRotation = Mathf.Min(currentRotation, nextRotation);
        float maxYRotation = Mathf.Max(currentRotation, nextRotation);

        // Assign new Rotation Limits
        minRotationLimit = minYRotation % 360;
        maxRotationLimit = maxYRotation % 360;

        // Assign new connected anchor to Configurable Joint
        float anchorPos = currentAnchorPos + nextAnchorPos;
        anchorPos /= 2;
        joint.connectedAnchor = new Vector3(joint.connectedAnchor.x, anchorPos, joint.connectedAnchor.z);

        // Assign new linear limit to Configurable Joint
        float linearLimit = Mathf.Abs(currentAnchorPos - nextAnchorPos);
        linearLimit /= 2;
        SoftJointLimit jointLimit = joint.linearLimit;
        jointLimit.limit = linearLimit;
        joint.linearLimit = jointLimit;

        if (isConsoleEchoEnabled)
        {
            Debug.Log($"{this.GetType().Name}: Limiters & Trigger Updated\n" +
                        $"Rotation Limit: {minRotationLimit}->{maxRotationLimit}\n" +
                        $"Connected Anchor Y: {joint.connectedAnchor.y}\n " +
                        $"Linear Limit: {joint.linearLimit.limit}" +
                        $"Next State Trigger: {stateTrigger}");
        }
    }
    bool ShouldProceedToNextState()
    {
        switch (stateTrigger)
        {
            case StateTrigger.MinRotation:
                if (Mathf.Abs(puzzle.transform.localEulerAngles.y - minRotationLimit) < yRotationThreshold)
                {
                    return true;
                }
                break;
            case StateTrigger.MaxRotation:
                if (Mathf.Abs(puzzle.transform.localEulerAngles.y - maxRotationLimit) < yRotationThreshold)
                {
                    return true;
                }
                break;
            case StateTrigger.MinZPos:
                return (IsReachingLimit() == 0);
            case StateTrigger.MaxZPos:
                return (IsReachingLimit() == 1);
        }
        return false;
    }
    bool ShouldProceedToPreviousState()
    {
        if (currentState == 0)
        {
            return false;
        }
        switch (stateTrigger)
        {
            case StateTrigger.MinRotation:
                if (Mathf.Approximately(puzzle.transform.localEulerAngles.y, maxRotationLimit))
                {
                    return true;
                }
                break;
            case StateTrigger.MaxRotation:
                if (Mathf.Approximately(puzzle.transform.localEulerAngles.y, minRotationLimit))
                {
                    return true;
                }
                break;
            case StateTrigger.MinZPos:
                return (IsReachingLimit() == 1);
            case StateTrigger.MaxZPos:
                return (IsReachingLimit() == 0);
        }
        return false;
    }

    int IsReachingLimit()
    {
        // Get anchor position
        Vector3 anchorWorld = joint.transform.TransformPoint(joint.anchor);
        // Get connected anchor posistion
        Vector3 connectedAnchorWorld = joint.connectedAnchor;

        // Update connected anchor if there is a rigidbody assigned to Configuratable Joint
        if (joint.connectedBody != null)
        {
            connectedAnchorWorld = joint.connectedBody.transform.TransformPoint(joint.connectedAnchor);
        }

        // Getting delta of anchor to connected anchor
        Vector3 worldDelta = anchorWorld - connectedAnchorWorld;
        Vector3 localDelta = joint.transform.InverseTransformVector(worldDelta);

        // Getting limit
        float limit = joint.linearLimit.limit;

        // Getting delta y
        float xOffset = localDelta.y;

        // Threshold for checking
        float threshold = 0.01f;

        if (xOffset >= (limit - threshold))
        {
            return 1;
        }
        else if (xOffset <= (-limit + threshold))
        {
            return 0;
        }
        return -1;
    }
    void EndOfGame()
    {
        isGameEnabled = false;
        StopCoroutine(gameCoroutine);
        Destroy(joint);
        Collider[] colls = puzzle.GetComponentsInChildren<Collider>();
        foreach (Collider coll in colls)
        {
            coll.enabled = true;
        }
        if (isConsoleEchoEnabled)
        {
            Debug.Log($"{this.GetType().Name}: Game ended!");
        }
    }
    public void Cheated()
    {
        isGameEnabled = false;
    }
}

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AISight : AIModule
{
    AIBrain aiBrain; // Brain is only used as a means to cast it into a IVisualReceptor
    IVisualReceptor brainVisualReceptor;
    [SerializeField] float minimumCheckTime = 0.2f;
    [SerializeField] float maximumCheckTime = 0.5f;

    HashSet<Transform> previouslyVisibleTargets = new HashSet<Transform>(); // TODO: Should this be kept here or in Brain?
    HashSet<Transform> visibleTargets = new HashSet<Transform>();

    #region FOV Settings
    [Header("FOV Settings")]
    [Tooltip("Which layers to ignore when casting a line between the eyes and the spotting point")]
    [SerializeField] private LayerMask ignore;
    [SerializeField] private Transform eyes;
    [Range(0, 360)]
    [SerializeField] private float fovMaxHorizontalAngle;
    [Range(0, 180)]
    [SerializeField] private float fovMaxVerticalAngle;
    [Range(0, 360)]
    [SerializeField] private float fovGoodViewHorizontalAngle;
    [Range(0, 180)]
    [SerializeField] private float fovGoodViewVerticalAngle;
    [Range(0, 360)]
    [SerializeField] private float fovSharpViewHorizontalAngle;
    [Range(0, 180)]
    [SerializeField] private float fovSharpViewVerticalAngle;
    private bool seeingTarget;
    #endregion

    private List<Collider> allSpotableTargets = new List<Collider>();

    #region FOV Visuals
#if UNITY_EDITOR
    [SerializeField] private bool seeMaxFOV;
    [SerializeField] private bool seeGoodFOV;
    [SerializeField] private bool seeSharpFOV;
    [SerializeField] private float fovLinesSize;

    void OnDrawGizmosSelected()
    {
        Handles.color = Color.magenta;
        Handles.matrix = eyes.localToWorldMatrix;
        if (seeMaxFOV)
        {
            Handles.color = Color.red;
            if (seeingTarget) Handles.color = Color.green;
            DrawFOVLines(fovMaxHorizontalAngle, fovMaxVerticalAngle);
        }
        if (seeGoodFOV)
        {
            Handles.color = Color.yellow;
            DrawFOVLines(fovGoodViewHorizontalAngle, fovGoodViewVerticalAngle);
        }
        if (seeSharpFOV)
        {
            Handles.color = Color.cyan;
            DrawFOVLines(fovSharpViewHorizontalAngle, fovSharpViewVerticalAngle);
        }
    }

    void DrawFOVLines(float fovHorizontalAngle, float fovVerticalAngle)
    {
        float fovSizeHalfExtent = fovLinesSize / 2;
        switch (fovHorizontalAngle)
        {
            case (<= 90):
                {
                    float angle = (fovHorizontalAngle / 2) * (Mathf.PI / 180);
                    float lineMagnitude = fovSizeHalfExtent / Mathf.Cos(angle);
                    float xPoint = Mathf.Sqrt(Mathf.Pow(lineMagnitude, 2) - Mathf.Pow(fovSizeHalfExtent, 2));
                    Handles.DrawLine(Vector3.zero, new Vector3(xPoint, 0, fovSizeHalfExtent));
                    Handles.DrawLine(Vector3.zero, new Vector3(-xPoint, 0, fovSizeHalfExtent));
                }
                break;
            case (<= 180):
                {
                    float deltaAngle = (90 - (fovHorizontalAngle / 2)) * (Mathf.PI / 180);
                    float lineMagnitude = fovSizeHalfExtent / Mathf.Cos(deltaAngle);
                    float zPoint = Mathf.Sqrt(Mathf.Pow(lineMagnitude, 2) - Mathf.Pow(fovSizeHalfExtent, 2));
                    Handles.DrawLine(Vector3.zero, new Vector3(fovSizeHalfExtent, 0, zPoint));
                    Handles.DrawLine(Vector3.zero, new Vector3(-fovSizeHalfExtent, 0, zPoint));
                }
                break;
            case (<= 270):
                {
                    float deltaAngle = ((fovHorizontalAngle / 2) - 90) * (Mathf.PI / 180);
                    float lineMagnitude = fovSizeHalfExtent / Mathf.Cos(deltaAngle);
                    float zPoint = -Mathf.Sqrt(Mathf.Pow(lineMagnitude, 2) - Mathf.Pow(fovSizeHalfExtent, 2));
                    Handles.DrawLine(Vector3.zero, new Vector3(fovSizeHalfExtent, 0, zPoint));
                    Handles.DrawLine(Vector3.zero, new Vector3(-fovSizeHalfExtent, 0, zPoint));
                }
                break;
            default:
                {
                    float deltaAngle = (180 - (fovHorizontalAngle / 2)) * (Mathf.PI / 180);
                    float lineMagnitude = (-fovSizeHalfExtent) / Mathf.Cos(deltaAngle);
                    float xPoint = Mathf.Sqrt(Mathf.Pow(lineMagnitude, 2) - Mathf.Pow(-fovSizeHalfExtent, 2));
                    Handles.DrawLine(Vector3.zero, new Vector3(xPoint, 0, -fovSizeHalfExtent));
                    Handles.DrawLine(Vector3.zero, new Vector3(-xPoint, 0, -fovSizeHalfExtent));
                }
                break;
        }
        switch (fovVerticalAngle)
        {
            case (<= 90):
                {
                    float angle = (fovVerticalAngle / 2) * (Mathf.PI / 180);
                    float lineMagnitude = fovSizeHalfExtent / Mathf.Cos(angle);
                    float yPoint = Mathf.Sqrt(Mathf.Pow(lineMagnitude, 2) - Mathf.Pow(fovSizeHalfExtent, 2));
                    Handles.DrawLine(Vector3.zero, new Vector3(0, yPoint, fovSizeHalfExtent));
                    Handles.DrawLine(Vector3.zero, new Vector3(0, -yPoint, fovSizeHalfExtent));
                }
                break;
            case (<= 180):
                {
                    float deltaAngle = (90 - (fovVerticalAngle / 2)) * (Mathf.PI / 180);
                    float lineMagnitude = fovSizeHalfExtent / Mathf.Cos(deltaAngle);
                    float zPoint = Mathf.Sqrt(Mathf.Pow(lineMagnitude, 2) - Mathf.Pow(fovSizeHalfExtent, 2));
                    Handles.DrawLine(Vector3.zero, new Vector3(0, fovSizeHalfExtent, zPoint));
                    Handles.DrawLine(Vector3.zero, new Vector3(0, -fovSizeHalfExtent, zPoint));
                }
                break;
            case (<= 270):
                {
                    float deltaAngle = ((fovVerticalAngle / 2) - 90) * (Mathf.PI / 180);
                    float lineMagnitude = fovSizeHalfExtent / Mathf.Cos(deltaAngle);
                    float zPoint = -Mathf.Sqrt(Mathf.Pow(lineMagnitude, 2) - Mathf.Pow(fovSizeHalfExtent, 2));
                    Handles.DrawLine(Vector3.zero, new Vector3(0, fovSizeHalfExtent, zPoint));
                    Handles.DrawLine(Vector3.zero, new Vector3(0, -fovSizeHalfExtent, zPoint));
                }
                break;
            default:
                {
                    float deltaAngle = (180 - (fovVerticalAngle / 2)) * (Mathf.PI / 180);
                    float lineMagnitude = (-fovSizeHalfExtent) / Mathf.Cos(deltaAngle);
                    float yPoint = Mathf.Sqrt(Mathf.Pow(lineMagnitude, 2) - Mathf.Pow(fovSizeHalfExtent, 2));
                    Handles.DrawLine(Vector3.zero, new Vector3(0, yPoint, -fovSizeHalfExtent));
                    Handles.DrawLine(Vector3.zero, new Vector3(0, -yPoint, -fovSizeHalfExtent));
                }
                break;
        }
    }
#endif
    #endregion


    #region Initialization
    void Start()
    {
        aiBrain = modulesDatabase.Brain;
        if (!aiBrain)
        {
            Debug.LogError("AIBrain not found on " + transform.root.name + ", AISight will not function.");
            enabled = false;
            return;
        }
        try
        {
            brainVisualReceptor = (IVisualReceptor)aiBrain;
        }
        catch (System.InvalidCastException)
        {
            Debug.LogError("AIBrain on " + transform.root.name + "does not implement the IVisualReceptor Interface, AISight will not function.");
            enabled = false;
            return;
        }
        // TODO: Switch from sphere collider to spacial grid? (More performant?)
        SphereCollider maxVisualDistanceSphere = GetComponent<SphereCollider>();
        maxVisualDistanceSphere.isTrigger = true;
        maxVisualDistanceSphere.includeLayers = LayerMask.GetMask("Visual Target");
        maxVisualDistanceSphere.excludeLayers = ~LayerMask.GetMask("Visual Target");
        // The time will always be randomized to avoid AI all checking at the same time (worse performance)
        nextCheckTime = Random.Range(minimumCheckTime, maximumCheckTime);
    }
    #endregion

    public bool IsSeeingTarget(Transform target) => visibleTargets.Contains(target);

    void OnTriggerEnter(Collider other) => allSpotableTargets.Add(other);

    void OnTriggerExit(Collider other) => allSpotableTargets.Remove(other);

    // If the priority of a target is less than the current priority, calculations will be ignored unless stated otherwise
    // TODO: Split into various iterations?
    public bool SpotForTargets()
    {
        // Swap sets to track changes
        HashSet<Transform> temp = previouslyVisibleTargets;
        previouslyVisibleTargets = visibleTargets;
        visibleTargets = temp;
        visibleTargets.Clear();

        //TODO: PRIORITY SYSTEM! (if performant)
        for (int i = 0; i < allSpotableTargets.Count; i++)
        {
            Transform targetTransform = allSpotableTargets[i].transform;
            Vector3 targetPosition = targetTransform.position;
            Vector3 targetDirection = targetPosition - eyes.transform.position;

            // TODO: Trade Angle for Dot Product?
            float angle = Vector3.Angle(eyes.forward, targetDirection);
            if (angle <= fovMaxHorizontalAngle / 2)
            {
                //RaycastHit hit;
                // Case: Target is visible
                if (!Physics.Linecast(eyes.transform.position, targetTransform.position, ~ignore))
                {
                    Transform targetRoot = allSpotableTargets[i].transform.root;
                    visibleTargets.Add(targetRoot);
                    Debug.DrawLine(eyes.transform.position, targetTransform.position, Color.red, 0.1f);

                    if (!previouslyVisibleTargets.Contains(targetRoot))
                    {
                        brainVisualReceptor.TransmitVisualTarget(allSpotableTargets[i]); // When a new target is visible, it will be sent to the AIBrain for processing   
                    }
                }
                //else Debug.Log("Collision: " +hit.transform.name);
            }
        }
        return seeingTarget;
    }

    float timeSinceLastCheck = 0;
    float nextCheckTime = 0;
    void Update()
    {
        timeSinceLastCheck += Time.deltaTime;
        if (timeSinceLastCheck >= nextCheckTime)
        {
            SpotForTargets();
            timeSinceLastCheck = 0;
            nextCheckTime = Random.Range(minimumCheckTime, maximumCheckTime); // TODO: Use faster library?
        }
    }
}
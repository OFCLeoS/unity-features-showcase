using UnityEngine;

public class TacticalPosition : MonoBehaviour
{
    [SerializeField] Collider positionCollider;
    public Collider GetPositionCollider => positionCollider;

    [SerializeField] bool isCover;
    public bool IsCover => isCover;

    [SerializeField] bool isRightPeek;
    public bool IsRightPeek => isRightPeek;

    Vector3 cornerPosition;
    public Vector3 CornerPosition => cornerPosition;

    #region Initialization
    void Awake()
    {
        CalculatePeekSide();
        CalculateCornerPosition();
    }

    /// <summary>
    /// Checks whether a right or left peek is needed
    /// </summary>
    void CalculatePeekSide()
    {
        Bounds positionColliderBounds = positionCollider.bounds;

        Vector3 colliderCenter = positionColliderBounds.center;

        Vector3 currentTacticalPositionPosition = transform.position;

        Vector3 directionToColliderCenter = colliderCenter - currentTacticalPositionPosition;
        float dotProduct = Vector3.Dot(transform.right, directionToColliderCenter);
        isRightPeek = dotProduct < 0;
    }

    /// <summary>
    /// Calculates the Tactical Position Corner Position
    /// </summary>
    void CalculateCornerPosition()
    {
        Bounds positionColliderBounds = positionCollider.bounds;

        Vector3 colliderCenter = positionColliderBounds.center;
        Vector3 colliderExtents = positionColliderBounds.extents;

        Vector3 currentTacticalPositionPosition = transform.position;

        bool positiveX = currentTacticalPositionPosition.x > colliderCenter.x;
        bool positiveZ = currentTacticalPositionPosition.z > colliderCenter.z;

        //TODO: Offset?
        cornerPosition = new Vector3(
        positiveX ? colliderCenter.x + colliderExtents.x : colliderCenter.x - colliderExtents.x,
        transform.position.y,
        positiveZ ? colliderCenter.z + colliderExtents.z : colliderCenter.z - colliderExtents.z
        );
    }
    #endregion
}

using UnityEngine;
using UnityEngine.Splines;

public class ActiveAxeController : MonoBehaviour
{
    #region Inspector Fields

    [Header("Axes")]
    public GameObject physicsAxe;
    public GameObject aniAxe;
    public Transform throwPoint;
    public float throwForce = 25f;
    public float spinForce = 12f;

    [Header("Recall Curve Settings")]
    public float curveHeight = 2.5f;
    public float curveSide = 1.5f;
    public float recallDuration = 0.4f;
    public float recallSpinSpeed = 720f;

    #endregion

    #region Private Fields

    [Header("Runtime References")]
    private GameObject activeAxe;
    private Rigidbody rb;
    private bool isRecalling;
    private float recallTimer;
    private Vector3 recallStartPos;
    private Vector3 recallControlPos;

    #endregion

    #region Unity Lifecycle

    private void Update()
    {
        HandleRecallInput();

        if (isRecalling)
        {
            UpdateRecall();
        }
    }

    private void OnDrawGizmos()
    {
        if (isRecalling && activeAxe != null)
        {
            DrawRecallCurveGizmos();
            DrawControlPointGizmos();
        }
    }

    #endregion

    #region Input Handling

    void HandleRecallInput()
    {
        if (Input.GetKeyDown(KeyCode.R) && activeAxe != null && !isRecalling)
        {
            RecallAxe();
        }
    }

    #endregion

    #region Axe Throwing

    public void ActiveAxe()
    {
        aniAxe.SetActive(false);

        SpawnAxe();
        ApplyThrowPhysics();
    }

    void SpawnAxe()
    {
        Quaternion offset = Quaternion.Euler(-48f, -90f, 30f);
        Quaternion finalRotation = throwPoint.rotation * offset;

        activeAxe = Instantiate(physicsAxe, throwPoint.position, finalRotation);
        rb = activeAxe.GetComponent<Rigidbody>();
    }

    void ApplyThrowPhysics()
    {
        rb.AddForce(throwPoint.forward * throwForce, ForceMode.Impulse);
        rb.AddTorque(activeAxe.transform.forward * spinForce, ForceMode.Impulse);
    }

    #endregion

    #region Axe Recall

    public void RecallAxe()
    {
        if (isRecalling || activeAxe == null) return;

        InitializeRecall();
        StopAxePhysics();
        CalculateControlPoint();
    }

    void InitializeRecall()
    {
        isRecalling = true;
        recallTimer = 0f;
        recallStartPos = activeAxe.transform.position;
    }

    void StopAxePhysics()
    {
        rb.isKinematic = true;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    void CalculateControlPoint()
    {
        Vector3 toPlayer = throwPoint.position - activeAxe.transform.position;
        Vector3 direction = toPlayer.normalized;
        Vector3 midpoint = (activeAxe.transform.position + throwPoint.position) * 0.5f;
        Vector3 right = Vector3.Cross(direction, Vector3.up).normalized;

        recallControlPos = midpoint + (Vector3.up * curveHeight) + (right * curveSide);
    }

    void UpdateRecall()
    {
        if (!ValidateRecallState()) return;

        recallTimer += Time.deltaTime;
        float t = Mathf.Clamp01(recallTimer / recallDuration);

        Vector3 newPosition = CalculateRecallPosition(t);
        MoveAxeAlongCurve(newPosition);
        SpinAxe();
        DrawDebugLines();

        if (IsRecallComplete(t))
        {
            OnRecallComplete();
        }
    }

    bool ValidateRecallState()
    {
        if (activeAxe == null)
        {
            isRecalling = false;
            return false;
        }
        return true;
    }

    Vector3 CalculateRecallPosition(float t)
    {
        Vector3 currentEndPos = throwPoint.position;
        Vector3 dynamicControlPos = CalculateDynamicControlPoint(currentEndPos);

        return CalculateQuadraticBezierPoint(
            t,
            recallStartPos,
            dynamicControlPos,
            currentEndPos
        );
    }

    Vector3 CalculateDynamicControlPoint(Vector3 currentEndPos)
    {
        Vector3 toPlayer = currentEndPos - recallStartPos;
        Vector3 direction = toPlayer.normalized;
        Vector3 midpoint = (recallStartPos + currentEndPos) * 0.5f;
        Vector3 right = Vector3.Cross(direction, Vector3.up).normalized;

        return midpoint + (Vector3.up * curveHeight) + (right * curveSide);
    }

    void MoveAxeAlongCurve(Vector3 newPosition)
    {
        activeAxe.transform.position = newPosition;
    }

    void SpinAxe()
    {
        activeAxe.transform.Rotate(Vector3.forward, recallSpinSpeed * Time.deltaTime, Space.Self);
    }

    void DrawDebugLines()
    {
        Vector3 currentEndPos = throwPoint.position;
        Vector3 dynamicControlPos = CalculateDynamicControlPoint(currentEndPos);

        Debug.DrawLine(recallStartPos, dynamicControlPos, Color.yellow);
        Debug.DrawLine(dynamicControlPos, currentEndPos, Color.yellow);
        Debug.DrawLine(activeAxe.transform.position, throwPoint.position, Color.green);
    }

    bool IsRecallComplete(float t)
    {
        return t >= 1f || Vector3.Distance(activeAxe.transform.position, throwPoint.position) < 0.2f;
    }

    void OnRecallComplete()
    {
        SnapAxeToHand();
        ResetRecallState();
        ReactivateAnimatedAxe();
    }

    void SnapAxeToHand()
    {
        activeAxe.transform.SetParent(throwPoint);
        activeAxe.transform.localPosition = Vector3.zero;
        activeAxe.transform.localRotation = Quaternion.identity;
    }

    void ResetRecallState()
    {
        rb.isKinematic = false;
        isRecalling = false;
        recallTimer = 0f;
    }

    void ReactivateAnimatedAxe()
    {
        aniAxe.SetActive(true);
        Destroy(activeAxe);
        activeAxe = null;
    }

    #endregion

    #region Bezier Curve Math

    Vector3 CalculateQuadraticBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;

        Vector3 point = (uu * p0) + (2 * u * t * p1) + (tt * p2);
        return point;
    }

    #endregion

    #region Debug Visualization

    void DrawRecallCurveGizmos()
    {
        Gizmos.color = Color.cyan;
        Vector3 previousPoint = recallStartPos;

        int segments = 20;
        for (int i = 1; i <= segments; i++)
        {
            float t = i / (float)segments;
            Vector3 point = CalculateGizmoPoint(t);

            Gizmos.DrawLine(previousPoint, point);
            previousPoint = point;
        }
    }

    Vector3 CalculateGizmoPoint(float t)
    {
        Vector3 currentEndPos = throwPoint.position;
        Vector3 toPlayer = currentEndPos - recallStartPos;
        Vector3 direction = toPlayer.normalized;
        Vector3 midpoint = (recallStartPos + currentEndPos) * 0.5f;
        Vector3 right = Vector3.Cross(direction, Vector3.up).normalized;
        Vector3 dynamicControlPos = midpoint + (Vector3.up * curveHeight) + (right * curveSide);

        return CalculateQuadraticBezierPoint(t, recallStartPos, dynamicControlPos, currentEndPos);
    }

    void DrawControlPointGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(recallStartPos, 0.1f);
        Gizmos.DrawSphere(recallControlPos, 0.1f);
        Gizmos.DrawSphere(throwPoint.position, 0.1f);
    }

    #endregion
}
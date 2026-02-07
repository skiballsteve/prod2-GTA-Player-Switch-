using UnityEngine;

public class AxeStick : MonoBehaviour
{
    #region Inspector Fields

    public Rigidbody rb;
    public Collider bladeTrigger;

    #endregion

    #region Private Fields

    private bool stuck = false;

    #endregion

    #region Unity Lifecycle

    private void Start()
    {
        InitializeRigidbody();
    }

    void OnTriggerEnter(Collider other)
    {
        Stick(other);
    }

    #endregion

    #region Initialization

    void InitializeRigidbody()
    {
        rb = gameObject.GetComponent<Rigidbody>();
    }

    #endregion

    #region Stick Mechanics

    void Stick(Collider other)
    {
        stuck = true;

        DisablePhysics();
        ParentToSurface(other);
        EmbedInSurface();
    }

    void DisablePhysics()
    {
        rb.isKinematic = true;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    void ParentToSurface(Collider other)
    {
        transform.SetParent(other.transform);
    }

    void EmbedInSurface()
    {
        transform.position += transform.forward * 0.02f;
    }

    #endregion
}

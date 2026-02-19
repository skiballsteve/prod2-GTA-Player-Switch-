using UnityEngine;

public class PlayerSelect : MonoBehaviour
{
    [Header("Player Info")]
    public int playerswitchNumber;
    public TranstionController TC;

    private void Start()
    {
        GameObject obj = GameObject.FindGameObjectWithTag("Blend");
        TC = obj.GetComponent<TranstionController>();
    }
    public void OnPlayerSelect()
    {
        TC.PlayerIndex = playerswitchNumber;
    }
}

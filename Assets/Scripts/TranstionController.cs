using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class TranstionController : MonoBehaviour
{

    [Header("Cina Cameras")]
    public CinemachineCamera[] VirtualCameras;
    private CinemachineBrain brain;

    [Header("Blend Properties")]
    public float ChangeCamDelay = 3f;
    public float blendProgress;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        brain = Camera.main.GetComponent<CinemachineBrain>();
        
    }

    // Update is called once per frame
    void Update()
    {
        PlayerInputCheck();

        if (brain.IsBlending)
        {
            blendProgress = brain.ActiveBlend.TimeInBlend / brain.ActiveBlend.Duration;
            Debug.Log($"Blend progress: {blendProgress * 100}%");


        }
    }

    public void PlayerInputCheck()
    {
        if(Input.GetKeyDown(KeyCode.LeftAlt) )
        {
           StartCoroutine( ChangeCmaeras(VirtualCameras[0], VirtualCameras[1], VirtualCameras[2]));
            
        }
    }

    public void ChangePlayer()
    {
        

    }

    public IEnumerator ChangeCmaeras(CinemachineCamera pCam, CinemachineCamera overCam, CinemachineCamera skyCam)
    {
       
        if (pCam.gameObject.activeSelf )
        {
            pCam.gameObject.SetActive(false);
            overCam.gameObject.SetActive(true);

        }
        yield return new WaitForSeconds(ChangeCamDelay);

        if (overCam.gameObject.activeSelf)
        {
            overCam.gameObject.SetActive(false);
            skyCam.gameObject.SetActive(true);
        } 
        

    }

    public IEnumerator ChangeFromSkyCameras(CinemachineCamera SkyCam1, CinemachineCamera SkyCam2, CinemachineCamera poverCam, CinemachineCamera playerCam)
    {
        if (SkyCam1.gameObject.activeSelf)
        {
            SkyCam1.gameObject.SetActive(false);
            SkyCam2.gameObject.SetActive(true);

        }
        yield return new WaitForSeconds(ChangeCamDelay);

        if (SkyCam2.gameObject.activeSelf)
        {
            SkyCam2.gameObject.SetActive(false);
            poverCam.gameObject.SetActive(true);
        }

        yield return new WaitForSeconds(ChangeCamDelay);
        if (poverCam.gameObject.activeSelf)
        {
            poverCam.gameObject.SetActive(false);
            playerCam.gameObject.SetActive(true);
        }
    }
}

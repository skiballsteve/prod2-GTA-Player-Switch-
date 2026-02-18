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

    [Header("Players")]
    public int PlayerIndex;
    public GameObject Player1;
    public GameObject Player2;
    public GameObject Player3;
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
            ChangePlayer();


        }
    }

    public void ChangePlayer()
    {
        int Player1Cam = 0;
        int Player1OverCam = 1;
        int Player1SkyCam = 2;

        int Player2Cam = 3;
        int Player2OverCam = 4;
        int Player2SkyCam = 5;

        int Player3Cam = 6;
        int Player3OverCam = 7;
        int Player3SkyCam = 8;

        //Player1
        if (Player1.activeSelf && PlayerIndex == 2)
        {
            StartCoroutine(ChangeCmaeras(VirtualCameras[Player1Cam], VirtualCameras[Player1OverCam], VirtualCameras[Player1SkyCam],Player2Cam,Player2OverCam,Player2SkyCam, Player1SkyCam));
        }

        if (Player1.activeSelf && PlayerIndex == 3)
        {
            StartCoroutine(ChangeCmaeras(VirtualCameras[Player1Cam], VirtualCameras[Player1OverCam], VirtualCameras[Player1SkyCam], Player3Cam, Player3OverCam, Player3SkyCam, Player1SkyCam));
        }

        //Player2
        if (Player2.activeSelf && PlayerIndex == 1)
        {
            StartCoroutine(ChangeCmaeras(VirtualCameras[Player2Cam], VirtualCameras[Player2OverCam], VirtualCameras[Player2SkyCam], Player1Cam, Player1OverCam, Player1SkyCam, Player2SkyCam));
        }

        if (Player2.activeSelf && PlayerIndex == 3)
        {
            StartCoroutine(ChangeCmaeras(VirtualCameras[Player2Cam], VirtualCameras[Player2OverCam], VirtualCameras[Player2SkyCam], Player3Cam, Player3OverCam, Player3SkyCam, Player2SkyCam));
        }

        //Player3
        if (Player3.activeSelf && PlayerIndex == 1)
        {
            StartCoroutine(ChangeCmaeras(VirtualCameras[Player3Cam], VirtualCameras[Player3OverCam], VirtualCameras[Player3SkyCam], Player1Cam, Player1OverCam, Player1SkyCam, Player3SkyCam));
        }

        if (Player3.activeSelf && PlayerIndex == 2)
        {
            StartCoroutine(ChangeCmaeras(VirtualCameras[Player3Cam], VirtualCameras[Player3OverCam], VirtualCameras[Player3SkyCam], Player2Cam, Player2OverCam, Player2SkyCam, Player3SkyCam));
        }




    }

    public IEnumerator ChangeCmaeras(CinemachineCamera pCam, CinemachineCamera overCam, CinemachineCamera skyCam, int SecondPlayerCam, int SecondPlayerOverCam, int SecondPlayerSkyCam, int PreviousePlayerSkyCam)
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
        yield return new WaitForSeconds(ChangeCamDelay);

        StartCoroutine(ChangeFromSkyCameras(VirtualCameras[PreviousePlayerSkyCam], VirtualCameras[SecondPlayerSkyCam], VirtualCameras[SecondPlayerOverCam], VirtualCameras[SecondPlayerCam]));

    }

    public IEnumerator ChangeFromSkyCameras(CinemachineCamera SkyCam1, CinemachineCamera SkyCam2, CinemachineCamera poverCam, CinemachineCamera playerCam)
    {
        if (SkyCam1.gameObject.activeSelf)
        {
            SkyCam1.gameObject.SetActive(false);
            SkyCam2.gameObject.SetActive(true);

        }
        yield return new WaitForSeconds(7f);

        if (SkyCam2.gameObject.activeSelf)
        {
            SkyCam2.gameObject.SetActive(false);
            poverCam.gameObject.SetActive(true);
        }

        yield return new WaitForSeconds(3f);
        if (poverCam.gameObject.activeSelf)
        {
            poverCam.gameObject.SetActive(false);
            playerCam.gameObject.SetActive(true);
        }
    }
}

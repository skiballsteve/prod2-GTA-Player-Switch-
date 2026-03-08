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
    public bool IsBlending;

    [Header("Players")]
    public int PlayerIndex;
    public int storactive;
    public GameObject Player1;
    public GameObject Player2;
    public GameObject Player3;
    public bool player1IsActive;
    public bool player2IsActive;
    public bool player3IsActive;
    [Header("Canvas")]
    public GameObject UICanvas;
    public GameObject PausePanle;

    [Header("SloMo")]
    public float slowSpeed = 0.25f;
    public float transitionSpeed = 5f;

    public float minTimeScale = 0.1f;
    public float maxTimeScale = 1f;
    public float slowDownSpeed = 10f;
    public float speedUpSpeed = 1f;

    public bool isSloMo;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        brain = Camera.main.GetComponent<CinemachineBrain>();
        
    }

    // Update is called once per frame
    void Update()
    {
        
        PlayerInputCheck();
        
        ActivePlayerController();

        if (!PausePanle.activeSelf)
        {
            EnableSlowMotion();

        }
        Time.fixedDeltaTime = 0.02f * Time.timeScale;

    }

    public void PlayerInputCheck()
    {
        if(Input.GetKeyDown(KeyCode.LeftAlt) )
        {
            storactive = PlayerIndex;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            isSloMo = true;
            

            UICanvas.SetActive(true);
           

        }
      
        if (Input.GetKeyUp(KeyCode.LeftAlt))
        {
            if (storactive != PlayerIndex)
            {
                ChangePlayer();
                Disableanimators();
               

            }
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            UICanvas.SetActive(false);
            

            isSloMo = false;
        }
    }
   

    void ActivePlayerController()
    {
        if (PlayerIndex == 1)
        {
            player1IsActive = true;
            if (IsBlending)
            {
                DisablePlayerControllers();
                Debug.Log("Is blending");


            }
            else
            {
                Player1.GetComponent<PlayerController>().enabled = true;
                Player2.GetComponent<PlayerController>().enabled = false;
                Player3.GetComponent<PlayerController>().enabled = false;
               // EnableAnimators();
            }

            //Sky cams
            VirtualCameras[5].transform.rotation = VirtualCameras[2].transform.rotation;
            VirtualCameras[8].transform.rotation = VirtualCameras[2].transform.rotation;

            //Over Cams
            VirtualCameras[4].transform.rotation = VirtualCameras[1].transform.rotation;
            VirtualCameras[7].transform.rotation = VirtualCameras[1].transform.rotation;

            

        }
        else
        {
            player1IsActive =false;
        }


        if (PlayerIndex == 2)
        {
            player2IsActive =true;
            if (IsBlending)
            {
                DisablePlayerControllers();
                Debug.Log("Is blending");
                
            }
            else
            {
                Player2.GetComponent<PlayerController>().enabled = true;


                Player1.GetComponent<PlayerController>().enabled = false;
                Player3.GetComponent<PlayerController>().enabled = false;
               // EnableAnimators();

            }
           

            //Sky cams
            VirtualCameras[2].transform.rotation = VirtualCameras[5].transform.rotation;
            VirtualCameras[8].transform.rotation = VirtualCameras[5].transform.rotation;

            //Over Cams
            VirtualCameras[1].transform.rotation = VirtualCameras[4].transform.rotation;
            VirtualCameras[7].transform.rotation = VirtualCameras[4].transform.rotation;

        
        }
        else
        {
            player2IsActive = false;



        }


        if (PlayerIndex == 3)
        {
            player3IsActive = true;

            if (IsBlending)
            {
                DisablePlayerControllers();
                Debug.Log("Is blending");

                //Disableanimators();
            }
            else
            {
                Player3.GetComponent<PlayerController>().enabled = true;


                Player1.GetComponent<PlayerController>().enabled = false;
                Player2.GetComponent<PlayerController>().enabled = false;
                //EnableAnimators();

            }
           

            //Sky cams
            VirtualCameras[5].transform.rotation = VirtualCameras[8].transform.rotation;
            VirtualCameras[2].transform.rotation = VirtualCameras[8].transform.rotation;

            //Over Cams
            VirtualCameras[4].transform.rotation = VirtualCameras[7].transform.rotation;
            VirtualCameras[1].transform.rotation = VirtualCameras[7].transform.rotation;

           
        }
        else
        {
            player3IsActive = false;



        }

    }

    void DisablePlayerControllers()
    {
            Player1.GetComponent<PlayerController>().enabled = false;
            Player2.GetComponent<PlayerController>().enabled = false;
            Player3.GetComponent<PlayerController>().enabled = false;
        
    }

    void Disableanimators()
    {

        ResetAllBools(Player1.GetComponentInChildren<Animator>());
        ResetAllBools(Player2.GetComponentInChildren<Animator>());
        ResetAllBools(Player3.GetComponentInChildren<Animator>());

    }
    void EnableSlowMotion()
    {
        if (isSloMo)
        {
            Time.timeScale = Mathf.Max(minTimeScale, Time.timeScale - slowDownSpeed * Time.unscaledDeltaTime);

        }
        else
        {
            Time.timeScale = Mathf.Min(maxTimeScale, Time.timeScale + speedUpSpeed * Time.unscaledDeltaTime);

        }
    }

    void ResetAllBools(Animator animator)
    {
        // Get all parameters from the animator
        foreach (AnimatorControllerParameter parameter in animator.parameters)
        {
            // Check if it's a bool type
            if (parameter.type == AnimatorControllerParameterType.Bool)
            {
                animator.SetBool(parameter.name, false);
                Debug.Log($"Reset bool: {parameter.name}");
            }
        }
    }
    void EnableAnimators()
    {
        Player1.GetComponentInChildren<Animator>().enabled = true;
        Player2.GetComponentInChildren<Animator>().enabled = true;
        Player3.GetComponentInChildren<Animator>().enabled = true;
    }

    IEnumerator AnimatorDelay(Animator ani)
    {
        yield return new WaitForSeconds(5f);
        ani.enabled = false;    
        yield return new WaitForSeconds(1f);
       // ani.enabled = true;

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
        if (Player1.activeSelf && PlayerIndex == 2 && player1IsActive == false)
        {
            StartCoroutine(ChangeCmaeras(VirtualCameras[Player1Cam], VirtualCameras[Player1OverCam], VirtualCameras[Player1SkyCam],Player2Cam,Player2OverCam,Player2SkyCam, Player1SkyCam));
        }
        else
        {
            Debug.Log("donothing");
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
        IsBlending = true;
       
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

        IsBlending = false;
    }
}

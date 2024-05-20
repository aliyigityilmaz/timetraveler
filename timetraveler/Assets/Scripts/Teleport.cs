using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class Teleport : MonoBehaviour
{
    public Text youCanTeleport;
    private float timeLeft = 10.0f;
    public Text youCanTeleportGalaxy;
    private float galaxyTimeLeft = 10.0f;
    private GameObject player;

    [Header("TeleportPoints")]
    public GameObject egyptTeleport;
    public GameObject futureTeleport;


    public AudioClip egyptMusic;
    public AudioClip futuristicMusic;
    public AudioClip modernMusic;

    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        player = this.gameObject;
        audioSource = GetComponent<AudioSource>();
        audioSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
        if (player != null)
        {
            if (player.GetComponent<FirstPersonController>().egyptmap == true && timeLeft > 0f)
            {
                youCanTeleport.gameObject.SetActive(true);
                timeLeft -= Time.deltaTime;
            }
            else
            {
                youCanTeleport.gameObject.SetActive(false);
            }

            if (player.GetComponent<FirstPersonController>().galaxymap == true && galaxyTimeLeft > 0f)
            {
                youCanTeleportGalaxy.gameObject.SetActive(true);
                galaxyTimeLeft -= Time.deltaTime;
            }
            else
            {
                youCanTeleportGalaxy.gameObject.SetActive(false);
            }
        }

        
        

        if (player != null)
        {
            if(Input.GetKeyDown(KeyCode.U) && player.GetComponent<FirstPersonController>().egyptmap == true)
            {
                audioSource.clip = egyptMusic;
                audioSource.Play();
                StartCoroutine(Teleporting(egyptTeleport));
                Debug.Log("Teleporting to Egypt");
            }
            if(Input.GetKeyDown(KeyCode.I) && player.GetComponent<FirstPersonController>().galaxymap == true)
            {
                audioSource.clip = futuristicMusic;
                audioSource.Play();
                StartCoroutine(Teleporting(futureTeleport));
            }
        }

        IEnumerator Teleporting(GameObject teleportPoint)
        {
            player.GetComponent<FirstPersonController>().enabled = false;
            player.transform.position = teleportPoint.transform.position;
            yield return new WaitForSeconds(0.2f);
            player.GetComponent<FirstPersonController>().enabled = true;
        }
    }



}

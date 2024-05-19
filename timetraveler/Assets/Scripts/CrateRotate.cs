using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrateRotate : MonoBehaviour
{
    // Event declaration
    public static event Action<string, int> Rotated;

    private bool coroutineAllowed;
    private int numberShown;

    private void Start()
    {
        coroutineAllowed = true;
        numberShown = 0;
    }


    //private void OnMouseDown()
    //{
    //    Debug.Log("asdfdsafdsafad xdxdxd");
    //    int randomNumber = UnityEngine.Random.Range(1, 10); // Örnek bir rasgele sayı oluştur
    //    Rotated?.Invoke(gameObject.name, randomNumber); // Rotated olayını tetikle
    //}

    //private void OnMouseDown()
    //{
    //    if (coroutineAllowed)
    //    {
    //        StartCoroutine("RotateWheel");
    //    }
    //}

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Change 0 to 1 for right mouse button
        {
            // Get the main camera
            Camera mainCamera = Camera.main;

            // Check if the main camera exists
            if (mainCamera != null)
            {
                // Get the mouse position
                Vector3 mousePosition = Input.mousePosition;

                // Create a ray from the camera through the mouse position
                Ray ray = mainCamera.ScreenPointToRay(mousePosition);

                // Create a RaycastHit variable to store information about the hit
                RaycastHit hit;

                // Perform the raycast
                if (Physics.Raycast(ray, out hit))
                {
                    // If the ray hits something, you can access information about the hit
                    Debug.Log("Hit object: " + hit.collider.gameObject.name);
                    Debug.Log("Hit point: " + hit.point);
                    Debug.Log("Hit normal: " + hit.normal);

                    if(gameObject.name == hit.collider.gameObject.name)
                        if (coroutineAllowed)
                            {
                                StartCoroutine("RotateWheel");
                            }
                }
                else
                {
                    // If the ray doesn't hit anything, you can handle this case here
                    Debug.Log("No object hit.");
                }
            }
        }
    }


    private IEnumerator RotateWheel()
    {
        coroutineAllowed = false;

        for (int i = 0; i <= 11; i++) {

            transform.Rotate(new Vector3(-3.75f, 0f, 0f));

            yield return new WaitForSeconds(0.01f);
        }
        coroutineAllowed = true;

        numberShown += 1;

        if (numberShown > 7)
        {
            numberShown = 0;
        }

        Rotated(name, numberShown);
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
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
    //    int randomNumber = UnityEngine.Random.Range(1, 10); // Örnek bir rasgele sayı oluştur
    //    Rotated?.Invoke(gameObject.name, randomNumber); // Rotated olayını tetikle
    //}

    private void OnMouseDown()
    {
        if (coroutineAllowed)
        {
            StartCoroutine("RotateWheel");
        }
    }

    private IEnumerator RotateWheel()
    {
        coroutineAllowed = false;

        for (int i = 0; i <= 11; i++) {

            transform.Rotate(0f, 0f, -3f);
            yield return new WaitForSeconds(0.01f);
        }
        coroutineAllowed = true;

        numberShown += 1;

        if (numberShown > 9)
        {
            numberShown = 0;
        }

        Rotated(name, numberShown);
    }
}

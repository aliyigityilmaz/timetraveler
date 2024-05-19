using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockControl : MonoBehaviour
{
    private int[] result, correctCombination;

    private void Start()
    {
        result = new int[] { 0, 0, 0, 0 }; // 5, 5, 5 varsayılan değerlerle başlatıldı
        correctCombination = new int[] { 3, 7, 9, 2 }; // Doğru kombinasyon ayarlandı

        // Rotate.Rotated olayına CheckResults metodu eklendi
        Rotate.Rotated += CheckResults;
    }

    private void CheckResults(string wheelName, int number)
    {
        switch (wheelName)
        {
            case "wheel1": // "wheell" yerine "wheel1" düzeltildi
                result[0] = number; // Doğru indeks düzeltildi
                break;
            case "wheel2":
                result[1] = number;
                break;
            case "wheel3":
                result[2] = number;
                break;
            case "wheel4":
                result[2] = number;
                break;
        }

        // Doğru kombinasyon kontrol edildi ve konsola "Opened!" yazdırıldı
        if (result[0] == correctCombination[0] && result[1] == correctCombination[1] && result[2] == correctCombination[2] && result[3] == correctCombination[3])
        {
            Debug.Log("Opened!");
        }
    }

    private void OnDestroy()
    {
        // OnDestroy() çağrıldığında Rotate.Rotated olayından CheckResults metodunu kaldır
        Rotate.Rotated -= CheckResults;
    }
}

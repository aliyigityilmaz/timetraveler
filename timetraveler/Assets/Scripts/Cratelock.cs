using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrateLockControl : MonoBehaviour
{
    private int[] result, correctCombination;
    public Animator animator;

    private void Start()
    {
        result = new int[] { 0, 0, 0, 0, 0, 0 }; // 5, 5, 5 varsayılan değerlerle başlatıldı
        correctCombination = new int[] { 4, 5, 2, 1, 7, 3 }; // Doğru kombinasyon ayarlandı

        // Rotate.Rotated olayına CheckResults metodu eklendi
        CrateRotate.Rotated += CheckResults;
    }

    private void CheckResults(string wheelName, int number)
    {
        switch (wheelName)
        {
            case "egyptianwheel1": // "wheell" yerine "wheel1" düzeltildi
                result[0] = number; // Doğru indeks düzeltildi
                break;
            case "egyptianwheel2":
                result[1] = number;
                break;
            case "egyptianwheel3":
                result[2] = number;
                break;
            case "egyptianwheel4":
                result[3] = number;
                break;
            case "egyptianwheel5":
                result[4] = number;
                break;
            case "egyptianwheel6":
                result[5] = number;
                break;
        }

        // Doğru kombinasyon kontrol edildi ve konsola "Opened!" yazdırıldı
        if (result[0] == correctCombination[0] && result[1] == correctCombination[1] && result[2] == correctCombination[2] 
         && result[3] == correctCombination[3] && result[4] == correctCombination[4] && result[5] == correctCombination[5])
        {
            Debug.Log("Opened!");
            animator.SetBool("CrateOpened", true);
            for(int i = 2; i < 8; i++)
                Destroy(transform.GetChild(i).gameObject);
        }
    }

    private void OnDestroy()
    {
        // OnDestroy() çağrıldığında Rotate.Rotated olayından CheckResults metodunu kaldır
        CrateRotate.Rotated -= CheckResults;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class PlayerUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI promtText;

    [SerializeField] private TextMeshProUGUI ammoText;

    [SerializeField] private TextMeshProUGUI ammoInventoryText;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    public void UpdateText(string promptMessage)
    {
        promtText.text = promptMessage;
    }

    public void UpdateAmmo(int currentAmmo, int magSize)
    {
        ammoText.text = $"{currentAmmo} / {magSize}";
    }

    public void UpdateAmmoInventory(int ammoInventory)
    {
        ammoInventoryText.text = $"{ammoInventory}";
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI stepCountLabel;
    public TextMeshProUGUI bioblockCountLabel;

    private Color originalColor;

    private void Awake()
    {
        if (stepCountLabel == null)
            Debug.LogError($"stepCountLabel not assigned in {this}");

        if (bioblockCountLabel == null)
            Debug.LogError($"bioblockCountLabel not assigned in {this}");

        originalColor = bioblockCountLabel.color;
    }


    // called once per step by BioBlockManager
    public void UpdateUI ()
    {
        StringBuilder label = new StringBuilder();

        label = new StringBuilder();
        label.Append("Step: ");
        label.Append(BioBlockManager.Instance.step_count.ToString());

        stepCountLabel.text = label.ToString();

        label.Clear();
        label.Append("Number of BioBlocks: ");
        label.Append(BioBlockManager.Instance.totalBioBlocks.ToString());

        bioblockCountLabel.text = label.ToString();
        if (BioBlockManager.Instance.totalBioBlocks >= BioBlockManager.maxNumBioBlocks)
            bioblockCountLabel.color = Color.red;
        else
            bioblockCountLabel.color = originalColor;

    }

}

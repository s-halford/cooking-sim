using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NameTag : MonoBehaviour
{
    [SerializeField] TMP_Text nameText = null;

    public void UpdateName(string name)
    {
        nameText.text = name;
    }
}

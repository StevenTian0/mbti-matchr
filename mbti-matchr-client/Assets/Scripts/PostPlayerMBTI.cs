using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PostPlayerMBTI : MonoBehaviour
{
    public TMPro.TMP_Dropdown dropdown;
    public string m_Text;

    void Update()
    {
        TMPro.TMP_Dropdown[] myDropDownList = GetComponentsInChildren<TMPro.TMP_Dropdown>();

        m_Text = dropdown.captionText.GetParsedText();
        Debug.Log(m_Text);
    }

}

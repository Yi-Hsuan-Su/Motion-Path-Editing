using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace UniHumanoid
{
    public class UIController : MonoBehaviour
    {
        // Start is called before the first frame update
        public Dropdown m_Dropdown;
        public GameObject unityChan;

        public void Switch()
        {
            if (m_Dropdown.value == 2)
            {
                unityChan.SetActive(true);
                unityChan.GetComponent<HumanPoseTransfer>().SetTransferSource();
            }
            else
            {
                GlobalData.Switch(m_Dropdown.value);
            }

        }
    }
}
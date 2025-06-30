using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialHintTrigger : MonoBehaviour
{
    // Start is called before the first frame update
    public string tutorialText;
    void OnTriggerEnter(Collider other)
    {
        PlayerController player = other.GetComponent<PlayerController>();
        if (player != null)
        {
            player.UpdateHintText(tutorialText);
        }
    }
    void OnTriggerExit(Collider other)
    {
        PlayerController player = other.GetComponent<PlayerController>();
        if (player != null)
        {
            player.UpdateHintText("");
        }
    }
}

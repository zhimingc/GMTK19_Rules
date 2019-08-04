using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WordFakeBehaviour : MonoBehaviour
{
  public NarrativeController nc;

  private void Awake() {
    nc = GameObject.FindObjectOfType<NarrativeController>();
  }

  private void OnMouseDown() {
    nc.textDisplay.TriggerTextDisplay("Hey! You don't think I'll let you do that again, do you? :(");
    gameObject.SetActive(false);
  }
}

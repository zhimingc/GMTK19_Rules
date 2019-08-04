using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CreditsDisplay : MonoBehaviour
{
  public TextDisplay textDisplay;
  public float delay;

  private TextMeshPro textMesh;
  private string toPrint;

  private void Awake() {
    textMesh = GetComponent<TextMeshPro>();
    toPrint = textMesh.text;
    textMesh.text = "";
    textDisplay.Init();
  }

  private void Start() {
    LeanTween.delayedCall(delay, ()=> {
      GetComponentInChildren<ParticleSystem>().Play();
      textDisplay.TriggerTextDisplay(toPrint);
    });
  }

  private void Update() {
    textDisplay.Update();
  }
}

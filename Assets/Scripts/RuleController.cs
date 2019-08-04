using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using TMPro;

public class RuleController : MonoBehaviour
{
  public PlayerController pc;
  public NarrativeController nc;
  public CinemachineVirtualCamera ruleCam;
  public TextDisplay textDisplay;
  public List<string> rulesText;

  private bool isRuleDisplayed;
  public bool isRuleBroken;

  private void Start() {
    textDisplay.Init();    
  }

  public void Reset()
  {
    isRuleBroken = false;
    textDisplay.Init();
  }

  private void Update() {
    // DEBUG CONTROLS
    if (Input.GetKeyDown(KeyCode.X))
    {
      TriggerRuleBreak();
    }
    //

    if (!isRuleBroken && RuleBreakPolling())
    {
      TriggerRuleBreak();
    }
    textDisplay.Update();
  }

  private void TriggerRuleBreak()
  {
    if (!isRuleDisplayed) TriggerRule();
    float breakDuration = 1.5f;
    float returnTime = 0.5f;
    textDisplay.shakeEffect.TriggerShake(breakDuration);
    isRuleBroken = true;

    // move cam
    ruleCam.gameObject.SetActive(true);
    TextMeshPro tm = textDisplay.displayObj;
    float originalFontSize = tm.fontSize;
    LeanTween.value(gameObject, tm.fontSize, tm.fontSize + 2.0f, breakDuration).setOnUpdate( (float val)=>{ tm.fontSize = val; } );
    LeanTween.delayedCall(breakDuration, ()=> {
      //Toolbox.Instance.camMod.ShakeCam(-1, -1, ruleCam);
      Toolbox.Instance.camMod.ShakeCam(0.25f, 2.0f, ruleCam);
      tm.fontSize = originalFontSize;
      textDisplay.displayObj.fontStyle = TMPro.FontStyles.Strikethrough;
      
      // particles
      textDisplay.displayObj.GetComponentInChildren<ParticleSystem>().Play();

      ruleCam.gameObject.SetActive(false);

      LeanTween.delayedCall(returnTime, () => {
      nc.TriggerRuleBreak();
      //LeanTween.value(gameObject, tm.fontSize, originalFontSize, 2.0f).setOnUpdate( (float val)=>{ tm.fontSize = val; } );
      });
    });
  }

  public void TriggerRule()
  {
    isRuleDisplayed = true;
    textDisplay.TriggerTextDisplay(rulesText[nc.narrativeLevel]);
    textDisplay.displayObj.fontStyle = TMPro.FontStyles.Normal;
  }

  public bool RuleBreakPolling()
  {
    switch (nc.narrativeLevel)
    {
      case 0:
        if (pc.pi.horizontal > 0.0f) return true;
      break;
      case 1:
        if (pc.IsRuleBroken) return true;
      break;
      case 2:
        if (pc.IsRuleBroken) return true;
      break;
      case 3:
        if (pc.IsRuleBroken) return true;
      break;
      case 4:
      break;
      case 5:
      break;
      case 6:
      break;
      case 7:
      break;
      case 8:
      break;
    }

    return false;
  }
}

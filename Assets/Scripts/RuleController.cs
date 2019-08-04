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
  private bool isRuleBroken;

  private void Start() {
    textDisplay.Init();    
  }

  public void ResetRuleController()
  {
    isRuleDisplayed = false;
    isRuleBroken = false;
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
    textDisplay.shakeEffect.TriggerShake(2.5f);
    isRuleBroken = true;

    // move cam
    //ruleCam.gameObject.SetActive(true);
    float breakDuration = 1.5f;
    TextMeshPro tm = textDisplay.displayObj;
    float originalFontSize = tm.fontSize;
    LeanTween.value(gameObject, tm.fontSize, tm.fontSize + 2.0f, breakDuration).setOnUpdate( (float val)=>{ tm.fontSize = val; } );
    LeanTween.delayedCall(breakDuration, ()=> {
      //Toolbox.Instance.camMod.ShakeCam(-1, -1, ruleCam);
      Toolbox.Instance.camMod.ShakeCam(3.0f);
      tm.fontSize = originalFontSize;
      textDisplay.displayObj.fontStyle = TMPro.FontStyles.Strikethrough;

      LeanTween.delayedCall(0.5f, () => {
      nc.TriggerRuleBreak();
      //ruleCam.gameObject.SetActive(false);
      //LeanTween.value(gameObject, tm.fontSize, originalFontSize, 2.0f).setOnUpdate( (float val)=>{ tm.fontSize = val; } );
      });
    });
  }

  public void TriggerRule()
  {
    isRuleDisplayed = true;
    textDisplay.TriggerTextDisplay(rulesText[nc.narrativeLevel]);
  }

  public bool RuleBreakPolling()
  {
    switch (nc.narrativeLevel)
    {
      case 0:
        if (pc.pi.horizontal > 0.0f)
        {
          return true;
        }
      break;
      case 1:
      break;
      case 2:
      break;
      case 3:
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

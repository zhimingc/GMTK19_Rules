using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Cinemachine;
using System.Reflection;

public enum NARRATIVE_STATE { IDLE, NEXT_LEVEL }

public class NarrativeController : MonoBehaviour
{
  public PlayerController playerController;
  public RuleController ruleController;
  public TextDisplay textDisplay;
  public NARRATIVE_STATE state;
  public int narrativeLevel;
  public List<NarrativeSequence> idleSeqs, nextSeqs;
  public NarrativeEvent curEvent;
  private NarrativeSequence seqToUse;

  private bool isDoneWithSeq;
  
  [Header("Timer")]
  public float timer;
  private float timeTracker;

  private int eventTracker;

  private void Awake() {
     
  }

  private void Start() {
    SetNarrativeState(NARRATIVE_STATE.IDLE);
  }

  private void Update() {
    // DEBUG CONTROLS
    if (Input.GetKeyDown(KeyCode.Z))
    {
      SetNarrativeState(NARRATIVE_STATE.NEXT_LEVEL);
    }
    //
    
    NarrationUpdate();
    textDisplay.Update();
  }

  void NarrationUpdate() {
    if (!isDoneWithSeq) 
    {
      if (textDisplay.IsScrollDone) timer += Time.deltaTime;

      if (timer - timeTracker >= curEvent.time) 
      {
        timeTracker = timer;
        TextDisplayTrigger(curEvent.text);
        GetNextEvent();
      }
    }
  }

  void SetNarrativeState(NARRATIVE_STATE newState)
  {
    state = newState;
    switch (state) {
      case NARRATIVE_STATE.IDLE:
        ResetSequenceTrackers();
        seqToUse = idleSeqs[narrativeLevel];
        GetNextEvent();
      break;
      case NARRATIVE_STATE.NEXT_LEVEL:
        ResetSequenceTrackers();
        seqToUse = nextSeqs[narrativeLevel];
        GetNextEvent();
      break;
    }
  }

  void GetNextEvent() {
    if (eventTracker < seqToUse.events.Count) 
    {
      curEvent = seqToUse.events[eventTracker];
      ++eventTracker;
    }
    else 
    {
      isDoneWithSeq = true;
    }
  }

  void TextDisplayTrigger(string text) {
    textDisplay.TriggerTextDisplay(text);
  }

  void ResetSequenceTrackers() 
  {
    textDisplay.Init();
    timeTracker = 0.0f;
    timer = 0.0f;
    eventTracker = 0;
    isDoneWithSeq = false;
  }

  public void TriggerRule() 
  {
    ruleController.TriggerRule();
  }

  public void TriggerRuleBreak()
  {
    SetNarrativeState(NARRATIVE_STATE.NEXT_LEVEL);
  }

  public void DeactivatePlayer()
  {
    playerController.Active = false;
  }

  public void ActivatePlayer()
  {
    playerController.Active = true;
  }

}

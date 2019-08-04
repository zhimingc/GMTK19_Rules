using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Cinemachine;
using System.Reflection;

public enum NARRATIVE_STATE { NEXT_RULE, IDLE, NEXT_LEVEL, END }

public class NarrativeController : MonoBehaviour
{
  public PlayerController playerController;
  public RuleController ruleController;
  public TextDisplay textDisplay;
  public NARRATIVE_STATE state;
  public int narrativeLevel;
  public List<NarrativeSequence> ruleSeqs, idleSeqs, nextSeqs;
  public NarrativeEvent curEvent;
  private NarrativeSequence seqToUse;

  private bool isDoneWithSeq;
  
  [Header("Timer")]
  public float timer;
  private float timeTracker;

  private int eventTracker;
  private LevelController levelController;

  private void Awake() {
     levelController = GetComponent<LevelController>();
  }

  private void Start() {
    textDisplay.Init();
    SetNarrativeState(NARRATIVE_STATE.NEXT_RULE);
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

    switch(state)
    {
      case NARRATIVE_STATE.NEXT_RULE:
      // trigger idle state
      if (isDoneWithSeq && textDisplay.IsScrollDone)
      {
        SetNarrativeState(NARRATIVE_STATE.IDLE);
      }
      break;
      case NARRATIVE_STATE.NEXT_LEVEL:
      // trigger next level
      if (isDoneWithSeq && textDisplay.IsScrollDone)
      {
        ResetLevel();
      }
      break;
    }
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
      case NARRATIVE_STATE.NEXT_RULE:
        BuildLevel();
        ResetSequenceTrackers();
        seqToUse = ruleSeqs[narrativeLevel];
        GetNextEvent();
      break;
      case NARRATIVE_STATE.IDLE:
        ResetSequenceTrackers();
        seqToUse = idleSeqs[0];
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
    textDisplay.ResetTextDisplay();
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
    ActivatePlayer();
  }

  public void DeactivatePlayer()
  {
    playerController.Active = false;
  }

  public void ActivatePlayer()
  {
    playerController.Active = true;
  }

  public void BuildLevel()
  {
    levelController.ToggleLevel(narrativeLevel);
  }

  public void ResetLevel()
  {
    playerController.ResetPC();
    if (narrativeLevel == ruleSeqs.Count - 1)
    {
      // end of sequences
      SetNarrativeState(NARRATIVE_STATE.END);
    }
    else 
    {
      ++narrativeLevel;
      ruleController.Reset();
      SetNarrativeState(NARRATIVE_STATE.NEXT_RULE);
    }
  }

}

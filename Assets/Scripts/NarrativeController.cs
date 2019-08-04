using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Cinemachine;
using System.Reflection;

public enum NARRATIVE_STATE { NEXT_RULE, IDLE, NEXT_LEVEL, END }

public class NarrativeController : MonoBehaviour
{
  public PlayerController playerController;
  public WordController wordController;
  public RuleController ruleController;
  public TextDisplay textDisplay;
  public NARRATIVE_STATE state;
  public int narrativeLevel;
  public List<NarrativeSequence> ruleSeqs, idleSeqs, nextSeqs;
  public NarrativeEvent curEvent;
  private NarrativeSequence seqToUse;
  private int level4Seq;

  private bool isDoneWithSeq;
  private bool pauseSeq;
  
  [Header("Timer")]
  public float timer;
  private float timeTracker;

  private int eventTracker;
  private LevelController levelController;

  private void Awake() {
     levelController = GetComponent<LevelController>();
  }

  private void Start() {
    level4Seq = 1;
    textDisplay.Init();
    SetNarrativeState(NARRATIVE_STATE.NEXT_RULE);
  }

  private void Update() {
    // DEBUG CONTROLS
    if (Input.GetKeyDown(KeyCode.C))
    {
      //textDisplay.FinishTextDisplay();
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
    if (!isDoneWithSeq && !pauseSeq) 
    {
      if (textDisplay.IsScrollDone) 
      {
        timer += Time.deltaTime;
        if (timer - timeTracker >= curEvent.time) 
        {
          timeTracker = timer;
          TextDisplayTrigger(curEvent.text);
          GetNextEvent();
        }
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
        // for level 4
        if (narrativeLevel == 4) seqToUse = idleSeqs[level4Seq];
        GetNextEvent();
      break;
      case NARRATIVE_STATE.NEXT_LEVEL:
        ResetSequenceTrackers();
        seqToUse = nextSeqs[narrativeLevel];
        GetNextEvent();
      break;
    }
  }

  public void GetNextEvent() {
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
    
    // player rule level
    if (narrativeLevel == 4)
    {
      wordController.MovePlayerRuleDown();
    }
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
    ResetPC();
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

  public void ResetPC()
  {
    playerController.ResetPC();
  }

  public void TriggerYourRule()
  {
    wordController.TriggerPlayerRuleDisplay();
  }

  public void TriggerWordOptions()
  {
    wordController.TriggerWordOptions();    
  }

  public void ToggleNarratorWord()
  {
    wordController.ToggleNarratorWord();
  }

  public void ToggleNarratorFake()
  {
    wordController.ToggleNarratorFake(true);
  }

  public void ToggleNarratorFakeOff()
  {
    wordController.ToggleNarratorFake(false);
  }

  public void Pause()
  {
    pauseSeq = true;
  }

  public void Resume()
  {
    pauseSeq = false;
  }

  public void LoopSeq()
  {
    eventTracker = 0;
    GetNextEvent();
    isDoneWithSeq = false;
  }

  public void NextLevel4Idle()
  {
    ++level4Seq;
    textDisplay.FinishTextDisplay();
    SetNarrativeState(NARRATIVE_STATE.IDLE);
  }

  public void TriggerEnd()
  {
    Pause();
    LeanTween.delayedCall(2.5f, ()=>{SceneManager.LoadScene("Jam_End");});
  }

  public void TriggerFollowEnd()
  {
    Pause();
    LeanTween.delayedCall(2.5f, ()=>{SceneManager.LoadScene("Jam_End_2");});    
  }
}

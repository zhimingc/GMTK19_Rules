﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Cinemachine;
using System.Reflection;

[System.Serializable]
public class CameraModule {
  //public CameraFlavour camFlav;
  
  public float defaultShake = 2.0f;
  public float defaultShakeTime = 0.25f;
  private CinemachineVirtualCamera vcam;
  private CinemachineBasicMultiChannelPerlin noiseChannel;
  public float shakeTimer;

  public void Init() {
    vcam = GameObject.FindObjectOfType<CinemachineVirtualCamera>();
    noiseChannel = vcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
  }

  public void Update() {
    if (shakeTimer > 0.0f) {
      shakeTimer -= Time.deltaTime;
      if (shakeTimer < 0.0f) {
        noiseChannel.m_AmplitudeGain = 0.0f;
      }
    }
  }

  public void ShakeCam(float shakeTime = -1, float shakeAmt = -1, CinemachineVirtualCamera curCam = null) {
    if (shakeTime == -1) shakeTime = defaultShakeTime;
    if (shakeAmt == -1) shakeAmt = defaultShake;

    if (curCam == null) noiseChannel = vcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    else noiseChannel = curCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

    noiseChannel.m_AmplitudeGain = shakeAmt;
    shakeTimer = shakeTime;
  }
}

[System.Serializable]
public class TextDisplay {
  public NarrativeController nc;
  public CameraModule camMod;
  public TextMeshPro displayObj;
  public float scrollDelay;
  public string textToDisplay;
  
  [Header("Flavour narration")]
  public NarrativeFlavour flavour;

  [Header("Effects")]
  public ShakeEffect shakeEffect;

  private float curScroll;
  private float curDelay;
  private int textTracker;
  private bool isScrollDone;

  public bool IsScrollDone {get {return isScrollDone;}}
  
  public void Init() {
    ResetTextDisplay();
    displayObj.text = "";
    camMod.Init();
    shakeEffect.Init(displayObj.transform);
  }

  public void ResetTextDisplay()
  {
    ResetTextTracker();
    ToggleScrollDone(true);
  }

  private void ResetTextTracker() {
    textTracker = 0;
  }

  private void ToggleScrollDone(bool flag) {
    isScrollDone = flag;
  }

  private void ResetScroll() {
    curScroll = 0.0f;
    curDelay = scrollDelay;
  }

  private void ParseToken(string token) {
    switch (token[0]) {
      case '!':
        Debug.Log("this is a [!] token");
      break;
      case 'f':
        string fnName = token.Substring(2, token.Length - 3);
        MethodInfo fn = nc.GetType().GetMethod(fnName);
        fn.Invoke(nc, new object[0]);
      break;
      case 'c':
        displayObj.text += "<#F565E3>";
      break;
    }
  }

  private bool ParseText(char word) {
    bool res = false;
    switch (word) {
      case ',':
      case '?':
        res = true;
        Toolbox.Instance.SfxMan.PlaySoundWithPitch("Blips", 0.9f, 1.1f, 0.2f);
        curDelay += flavour.commaPause;
      break;
      case '!':
        res = true;
        Toolbox.Instance.SfxMan.PlaySoundWithPitch("Blips", 0.9f, 1.1f, 0.2f);
        curDelay += flavour.commaPause;
        camMod.ShakeCam();
      break;
      case '.':
      case ':':
        res = true;
        Toolbox.Instance.SfxMan.PlaySoundWithPitch("Blips", 0.9f, 1.1f, 0.2f);
        curDelay += flavour.elipsePause;
        break;
      case ')':
      case ']':
        res = true;
        Toolbox.Instance.SfxMan.PlaySoundWithPitch("Smile", 0.95f, 1.05f, 0.25f);
        curDelay += flavour.elipsePause;
        break;
      case '(':
      case '/':
        res = true;
        Toolbox.Instance.SfxMan.PlaySoundWithPitch("Sad", 0.95f, 1.05f, 0.25f);
        curDelay += flavour.elipsePause;
      break;
    }

    return res;
  }

  public void Update() {
    shakeEffect.Update();
    camMod.Update();
    UpdateTextDisplay();
  }

  private void UpdateTextDisplay() {
    if (isScrollDone) return;

    curScroll += Time.deltaTime;
    
    if (curScroll >= curDelay) {
      ResetScroll();

      // check for token
      while (textTracker < textToDisplay.Length && textToDisplay[textTracker] == '[') {
        int endOfToken = textToDisplay.IndexOf(']', textTracker+1);
        // send token into parser
        string token = textToDisplay.Substring(textTracker + 1, endOfToken - textTracker - 1);
        textToDisplay = textToDisplay.Remove(textTracker, endOfToken - textTracker + 1);          

        ParseToken(token);
      }
      
      if (textTracker >= textToDisplay.Length) {
        ToggleScrollDone(true);
        return;
      }
      
      displayObj.text += textToDisplay[textTracker];
      bool parsed = ParseText(textToDisplay[textTracker]);

      // sfx
      if (!parsed && textTracker % 3 == 0) Toolbox.Instance.SfxMan.PlaySoundWithPitch("Blips", 0.9f, 1.1f, 0.2f);

      ++textTracker;
    }
  }

  public void FinishTextDisplay()
  {
    textTracker = textToDisplay.Length;
    displayObj.text = textToDisplay;
    ToggleScrollDone(true);
  }

  public void TriggerTextDisplay(string newText) {
    displayObj.text = "";
    isScrollDone = false;
    textToDisplay = new string(newText.ToCharArray());
    ResetTextTracker();
    ResetScroll();
    ToggleScrollDone(false);
  }
}

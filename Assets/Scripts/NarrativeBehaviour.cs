using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Cinemachine;

[System.Serializable]
public class CameraModule {
  public CinemachineVirtualCamera vcam;
  public float defaultShake;
  public float defaultShakeTime;
  
  private CinemachineBasicMultiChannelPerlin noiseChannel;
  private float shakeTimer;

  public void Init() {
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

  public void ShakeCam(float shakeTime = -1, float shakeAmt = -1) {
    if (shakeTime == -1) shakeTime = defaultShakeTime;
    if (shakeAmt == -1) shakeAmt = defaultShake;

    noiseChannel.m_AmplitudeGain = shakeAmt;
    shakeTimer = shakeTime;
  }
}

[System.Serializable]
public class TextDisplay {
  public CameraModule camMod;
  public TextMeshPro displayObj;
  public float scrollDelay;
  public string textToDisplay;
  
  [Header("Flavour narration")]
  public float commaPause;
  public float elipsePause;

  private float curScroll;
  private float curDelay;
  private int textTracker;
  private bool isScrollDone;

  public void Init() {
    ResetTextTracker();
    ToggleScrollDone(true);
    displayObj.text = "";
    camMod.Init();
  }

  private void ResetTextTracker() {
    textTracker = 1;
  }

  private void ToggleScrollDone(bool flag) {
    isScrollDone = flag;
  }

  private void ResetScroll() {
    curScroll = 0.0f;
    curDelay = scrollDelay;
  }

  private void ParseToken(string token) {
    switch (token) {
      case "[!]":
        Debug.Log("this is a [!] token");
      break;
    }
  }

  private void ParseText(char word) {
    switch (word) {
      case ',':
        curDelay += commaPause;
      break;
      case '!':
        curDelay += commaPause;
        camMod.ShakeCam();
      break;
      case '.':
        curDelay += elipsePause;
      break;
    }
  }

  public void Update() {
    camMod.Update();
    UpdateTextDisplay();
  }

  public void UpdateTextDisplay() {
    if (isScrollDone) return;

    curScroll += Time.deltaTime;
    
    if (curScroll >= curDelay) {
      ResetScroll();

      displayObj.text = textToDisplay.Substring(0, textTracker);
      ParseText(textToDisplay[textTracker-1]);

      ++textTracker;

      // check for token
      if (textTracker < textToDisplay.Length && textToDisplay[textTracker] == '[') {
        // send token into parser
        string token = textToDisplay.Substring(textTracker, 3);
        ParseToken(token);

        textToDisplay = textToDisplay.Remove(textTracker, 3);
      }

      if (textTracker > textToDisplay.Length) {
        ToggleScrollDone(true);
      }
    }
  }

  public void TriggerTextDisplay(string newText) {
    textToDisplay = new string(newText.ToCharArray());
    ResetTextTracker();
    ToggleScrollDone(false);
  }
}

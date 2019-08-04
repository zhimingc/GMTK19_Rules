using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WordController : MonoBehaviour
{
  public NarrativeController nc;
  public LayerMask wordLayer;


  [Header("Level 4")]
  public List<string> playerRuleWords;
  public TextMeshPro playerRuleText;
  public List<WordBehaviour> wordOptions;
  public GameObject bigBlock;
  public WordBehaviour narratorWord;
  public GameObject narratorFake;

  private Vector3 oldPos;
  private GameObject curWord;
  private int pcRuleProgress;
  private bool playerRuleDone;

  public GameObject CurWord {get {return curWord;}}

  // Start is called before the first frame update
  void Start()
  {
    ToggleOptionWords(false);
    playerRuleText.gameObject.SetActive(false);
    playerRuleDone = false;
    playerRuleText.text = "Your Rule:<#9D37EC>";
  }

  // Update is called once per frame
  void Update()
  {
    if (Input.GetMouseButtonDown(0))
    {
      RaycastHit wordHit = GetRaycast(wordLayer);
      if (wordHit.collider != null)
      {
        SetCurrentWordDrag(wordHit.transform.gameObject);
      }
    }

    if (Input.GetMouseButtonUp(0))
    {
      SetCurrentWordDrag(null);
    }

    if (Input.GetMouseButton(0) && curWord != null)
    {
      curWord.transform.position += GetMousePos() - oldPos;
      oldPos = GetMousePos();
    }
  }

  private RaycastHit GetRaycast(LayerMask layer)
  {
    RaycastHit hit;
    Camera cam = Camera.main;      
    Physics.Raycast(cam.transform.position, GetMousePos() - cam.transform.position, out hit, 20.0f, wordLayer);
    return hit;
  }

  private Vector3 GetMousePos()
  {
    Vector3 mousePos = Input.mousePosition;
    mousePos.z = 10.0f;
    return Camera.main.ScreenToWorldPoint(mousePos);
  }

  public void SetCurrentWordDrag(GameObject word)
  {
    if (word != null)
    {
      word.GetComponent<WordBehaviour>().TriggerDrag();    

      if (nc.narrativeLevel == 4)
      {
        if (CheckNarratorWord(word))
        {
          if (!word.GetComponent<MeshRenderer>().enabled)
          {
            nc.NextLevel4Idle();
          }
        }
      }
      
      word.GetComponent<MeshRenderer>().enabled = true;
      oldPos = GetMousePos();      
    }
    else
    {
      curWord?.GetComponent<WordBehaviour>().ReleaseDrag();
    }

    curWord = word;
  }

  public void UpdatePlayerRule()
  {
    if (playerRuleDone) return;

    playerRuleText.text += " " + playerRuleWords[pcRuleProgress];
    ++pcRuleProgress;

    if (pcRuleProgress == playerRuleWords.Count)
    {
      // player is done creating rule
      playerRuleDone = true;
      ToggleOptionWords(false);
      nc.Resume();
      nc.textDisplay.FinishTextDisplay();
      return;
    }

    ResetWords(playerRuleWords[pcRuleProgress]);
  }

  private void ResetWords(string text)
  {
    for (int i = 0; i < wordOptions.Count; ++i)
    {
      wordOptions[i].ResetWord(text);
    }
  }

  private void ToggleOptionWords(bool flag)
  {
    foreach(WordBehaviour word in wordOptions)
    {
      word.gameObject.SetActive(flag);
    }
  }

  private bool CheckNarratorWord(GameObject obj)
  {
    if (obj == narratorWord.gameObject) return true;
    return false;
  }

  public void TriggerPlayerRuleDisplay()
  {
    playerRuleText.gameObject.SetActive(true);
    playerRuleText.GetComponentInChildren<ParticleSystem>().Play();
  }

  public void TriggerWordOptions()
  {
    //ToggleOptionWords(true);
    for (int i = 0; i < wordOptions.Count; ++i)
    {
      wordOptions[i].ResetWord(playerRuleWords[pcRuleProgress], i * 0.5f);
    }
  }

  public void MovePlayerRuleDown()
  {
    playerRuleText.GetComponentInChildren<ParticleSystem>().Play();
    playerRuleText.transform.position -= new Vector3(0, 1.0f, 0);
    bigBlock.SetActive(true);
  }

  public void ToggleNarratorWord()
  {
    if (narratorWord.GetComponent<MeshRenderer>().enabled) return;
    narratorWord.gameObject.SetActive(!narratorWord.gameObject.activeSelf);
  }

  public void ToggleNarratorFake(bool flag)
  {
    narratorFake.gameObject.SetActive(flag);
  }
}

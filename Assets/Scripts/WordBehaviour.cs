using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WordBehaviour : MonoBehaviour
{
  public WordController wordController;
  public Vector3 origin;
  public bool isNarratorWord;
  public Color inactiveColor;
  public Color originColor;

  private ParticleSystem burst;
  private TextMeshPro textObj;
  private BoxCollider2D boxCollider2D;

  // Start is called before the first frame update
  void Awake()
  {
    burst = GetComponentInChildren<ParticleSystem>();
    textObj = GetComponent<TextMeshPro>();
    boxCollider2D = GetComponentInChildren<BoxCollider2D>();
    origin = transform.position;
    originColor = textObj.color;
  }

  public void ResetWord(string text, float delay = 0.0f)
  {    
    LeanTween.delayedCall(delay, () => {
      gameObject.SetActive(true);
      textObj.text = text;
      transform.position = origin;
      burst.Play();
    });
  }

  public void TriggerDrag()
  {
    if (isNarratorWord)
    {
      textObj.color = inactiveColor;
      boxCollider2D.gameObject.SetActive(false);;
    }
  }

  public void ReleaseDrag()
  {
    if (isNarratorWord)
    {
      LeanTween.delayedCall(0.5f, ()=>{
        textObj.color = originColor;
        boxCollider2D.gameObject.SetActive(true);
      });
    }
  }
}

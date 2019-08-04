using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WordDotBehaviour : MonoBehaviour
{
  public Color defaultColor, hoverColor;
  public List<Vector3> dotPos;
  public bool isDone;

  private WordController wc;
  private SpriteRenderer spriteRender;
  private int curPos;

  private void Awake() {
    curPos = 0;
    UpdateDotPos();
    isDone = false;
    wc = GameObject.FindObjectOfType<WordController>();
    spriteRender = GetComponent<SpriteRenderer>();
  }

  private void OnMouseOver() {
    if (wc.CurWord != null)
    {
      spriteRender.color = hoverColor;
      if (Input.GetMouseButtonUp(0))
      {
        wc.UpdatePlayerRule();
        UpdateDotPos();
      }
    }
  }

  private void UpdateDotPos()
  {
    if (curPos < dotPos.Count)
    {
      transform.position = dotPos[curPos];
      ++curPos;
    }
    else
    {
      // done creating word
      isDone = true;
      gameObject.SetActive(false);
    }
  }

  private void OnMouseExit() {
    spriteRender.color = defaultColor;  
  }
}

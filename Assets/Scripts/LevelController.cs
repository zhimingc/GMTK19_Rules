using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{
  public List<GameObject> levelObjs;

  public void ToggleLevel(int index)
  {
    for (int i = 0; i < levelObjs.Count; ++i)
    {
      if (levelObjs[i] == null) continue;
      
      if (i == index) levelObjs[i].SetActive(true);
      else levelObjs[i].SetActive(false);
    }
  }
}

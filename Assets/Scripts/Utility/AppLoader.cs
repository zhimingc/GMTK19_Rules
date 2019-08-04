using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppLoader : MonoBehaviour
{
  public SFXDatabase sfxDb;
  
  // Start is called before the first frame update
  void Awake()
  {
    Toolbox.Instance.WakeUp();
    Toolbox.Instance.SfxMan.db = sfxDb;
  }

}

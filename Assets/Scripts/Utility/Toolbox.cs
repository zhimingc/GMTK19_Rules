using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Toolbox : Singleton<Toolbox>
{
  // (Optional) Prevent non-singleton constructor use.
  protected Toolbox() { }

  public CameraModule camMod;

  public void WakeUp() {
    camMod = new CameraModule();
  }

  private void Start() {
    camMod.Init();
  }

  private void Update() {
    camMod.Update();
  }
}
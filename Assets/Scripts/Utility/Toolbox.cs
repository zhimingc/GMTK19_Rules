using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Toolbox : Singleton<Toolbox>
{
  // Used to track any global components added at runtime.
  private Dictionary<Type, Component> m_Components = new Dictionary<Type, Component>();

  public SFXManager SfxMan { get {return (SFXManager) m_Components[typeof(SFXManager)]; }}

  // (Optional) Prevent non-singleton constructor use.
  protected Toolbox() { }

  public CameraModule camMod;
  
  public void WakeUp() {

  }

  private void Awake()
  {
    // Put initialization code here.
    Add(typeof(SFXManager));
    camMod = new CameraModule();
  }

  private void Start() {
    camMod.Init();
  }

  private void Update() {
    if (Input.GetKeyDown(KeyCode.R))
    {
      SceneManager.LoadScene("Main_Level");
    }

    camMod.Update();
  }

    // The methods below allow us to add global components at runtime.
  public Component Add(Type componentID)
  {
    if (m_Components.ContainsKey(componentID))
    {
      //Debug.LogWarning("[Toolbox] Global component ID \"" + componentID + "\" already exist! Returning that.");
      return Get(componentID);
    }

    var newComponent = gameObject.AddComponent(componentID);
    m_Components.Add(componentID, newComponent);
    return newComponent;
  }

  public void Remove(Type componentID)
  {
    Component component;

    if (m_Components.TryGetValue(componentID, out component))
    {
      Destroy(component);
      m_Components.Remove(componentID);
    }
    else
    {
      Debug.LogWarning("[Toolbox] Trying to remove nonexistent component ID \"" + componentID + "\"! Typo?");
    }
  }

  public Component Get(Type componentID)
  {
    Component component;

    if (m_Components.TryGetValue(componentID, out component))
    {
      return component;
    }

    Debug.LogWarning("[Toolbox] Global component ID \"" + componentID + "\" doesn't exist! Typo?");
    return null;
  }
}
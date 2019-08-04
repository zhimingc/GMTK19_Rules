using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// to be edited in the inspector
[System.Serializable]
public struct AudioData
{
  public string name;
  public AudioClip clip;
};

// to be edited in the inspector
[System.Serializable]
public struct AudioBatchData
{
  public string name;
  public List<AudioClip> clips;
};


[CreateAssetMenu(fileName = "SFXDatabase", menuName = "Constants/SFXDatabase")]
public class SFXDatabase : ScriptableObject
{
  public List<AudioData> sfxData;
  public List<AudioBatchData> sfxBatchData;
}

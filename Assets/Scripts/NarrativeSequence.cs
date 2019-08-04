using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NarrativeEvent {
  public float time;
  public string text;
}

[CreateAssetMenu(fileName = "NarrativeSequence", menuName = "NarrativeSeq")]
public class NarrativeSequence : ScriptableObject
{
  public List<NarrativeEvent> events;
}

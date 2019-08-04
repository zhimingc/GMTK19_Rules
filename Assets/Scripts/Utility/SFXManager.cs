using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AudioProps
{
  public AudioProps()
  {}

  public AudioProps(AudioClip c)
  {
    clip = c;
    pitch = 1.0f;
    vol = 1.0f;
  }

  public string name;
  public AudioClip clip;
  public float pitch;
  public float vol;
};

public class SFXManager : MonoBehaviour {

  //private GameObject obj;
  public Dictionary<string, List<AudioClip>> soundLib;
  public bool isMute;
  public SFXDatabase db;

  private List<AudioSource> audioSources;

  public void Awake()
  {
    //obj = parent;
    isMute = false;
    audioSources = new List<AudioSource>();
    soundLib = new Dictionary<string, List<AudioClip>>();
  }

  public void Start() {
    InitializeSounds();
  }

  void InitializeSounds() {
    foreach(AudioData data in db.sfxData) {
      if (data.clip != null) AddSound(data.name, data.clip);
    }
    foreach(AudioBatchData batch in db.sfxBatchData) {
      if (batch.clips.Count > 0) AddSoundBatch(batch.name, batch.clips);
    } 
  }

  void AddSound(string name, AudioClip clip)
  {
    if (!soundLib.ContainsKey(name)) {
      soundLib.Add(name, new List<AudioClip>());
    }

    soundLib[name].Add(clip);
  }

  void AddSoundBatch(string name, List<AudioClip> clips) {
    soundLib.Add(name, clips);
  }

  public bool ToggleMute()
  {
    isMute = !isMute;

    if (isMute)
    {
      foreach (AudioSource src in audioSources)
      {
        src.Stop();
      }
    }

    return isMute;
  }

  public AudioProps GetAudio(string name)
  {
    if (soundLib.ContainsKey(name) == false)
    {
      Debug.Log("Sound library does not contain " + name + ".");
      return new AudioProps();
    }

    List<AudioClip> clips = soundLib[name];
    AudioClip currentClip = clips[Random.Range(0,clips.Count)];

    return new AudioProps(currentClip);
  }

  public void PlaySoundWithPitch(string name, float min = 1.0f, float max = 1.5f, float vol = 1.0f)
  {
    var clip = GetAudio(name);
    clip.pitch = Random.Range(min, max);
    PlaySound(clip, vol);
  }

  public void PlaySound(string name, float vol = 1.0f)
  {
    if (soundLib.ContainsKey(name) == false)
    {
      Debug.Log("Sound library does not contain " + name + ".");
    }

    AudioProps props = GetAudio(name);
    PlaySound(props, vol);
  }

  public void PlaySound(AudioProps props, float vol = 1.0f)
  {
    if (isMute) return;

    props.vol = vol;

    // Look for a free audio source
    foreach (AudioSource source in audioSources)
    {
      if (source.isPlaying) continue;
      ApplyProperties(source, props);
      source.volume = props.vol;
      source.Play();
      return;
    }

    // No free audio sources; create and use a new one
    AudioSource src = gameObject.AddComponent<AudioSource>();
    ApplyProperties(src, props);
    src.volume = props.vol;
    src.Play();
    audioSources.Add(src);
  }

  public void StopSound(string name) {
    var clip = GetAudio(name);

    // Note: Might cause bug for multiple sources playing the same clip
    foreach(AudioSource source in audioSources) {
      if (source.clip == clip.clip) {
        source.Stop();
        return;
      } 
    }
  }

// Private 
  void ApplyProperties(AudioSource src, AudioProps props)
  {
    src.clip = props.clip;
    src.pitch = props.pitch;
  }

}

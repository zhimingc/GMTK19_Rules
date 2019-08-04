using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ShakeEffect
{
  public float radius;
  public float minSpeed;
  public float maxSpeed;
  public bool isShaking;

  private const float threshold = 0.02f;
  private Vector3 startingPos;
  private Vector3 destinationPos;
  private Vector3 velocity;
  private Transform transform;
  private float shakeTime;

  public void Init(Transform tn)
  {
    transform = tn;
    startingPos = destinationPos = transform.position;
  }
  public void Update()
  {
    if (isShaking)
    {
      Shake();
      shakeTime -= Time.deltaTime;
      if (shakeTime <= 0.0f) isShaking = false;
    }
  }

  public void TriggerShake(float duration)
  {
    shakeTime = duration;
    isShaking = true;
  }

  // This is a 'physics' frame update
  private void Shake()
  {
    if (Vector3.Distance(transform.position, destinationPos) < threshold)
    {
      SetNewShakePoint();
    }

    // Move toward destination
    transform.position += velocity * Time.fixedDeltaTime;
  }

  private void SetNewShakePoint()
  {
    // Pick new destination and speed
    destinationPos = startingPos + Random.insideUnitSphere * radius;
    velocity = (destinationPos - transform.position) * Random.Range(minSpeed, maxSpeed);
  }
}

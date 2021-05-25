using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeOverTime : MonoBehaviour
{
  SpriteRenderer spriteRenderer;

  void Awake()
  {
    spriteRenderer = GetComponent<SpriteRenderer>();
  }

  void Update()
  {
    if (spriteRenderer.color.a > 0f)
    {
      Color newColor = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, spriteRenderer.color.a - 0.2f);
      spriteRenderer.color = newColor;
    }
    else
    {
      Destroy(gameObject);
    }
  }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleFullscreen : MonoBehaviour
{
  void Update()
  {
    if (Input.GetKeyDown(KeyCode.Escape))
    {
      FullscreenGameView.Toggle();
    }
  }
}

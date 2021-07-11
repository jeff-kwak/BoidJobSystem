using UnityEngine;
using TMPro;

public class DisplayFps : MonoBehaviour
{
  public TMP_Text Display;

  private void Update()
  {
    Display.text = $"{ 1 / Time.unscaledDeltaTime } fps";
  }
}

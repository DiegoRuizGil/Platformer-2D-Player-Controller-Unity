using UnityEngine;

public class Frames : MonoBehaviour
{
    [Header("Settings")]
    public int frames;

    void Start()
    {
        Application.targetFrameRate = frames;
    }
}

using UnityEditor.Profiling.Memory.Experimental;
using UnityEngine;

public class SoundOnCollides : MonoBehaviour
{
    [SerializeField]
    private LayerMask excludeLayer;
    [SerializeField]
    private float startupAudioMuteDuration = .1f;

    private AudioSource audioSource;
    private bool allowAudio = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        // Disable audio when staring up
        Invoke(nameof(AllowAudio), startupAudioMuteDuration);
    }
    private void AllowAudio()
    {
        allowAudio = true;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (!allowAudio) return;
        // Check is it collide with excluded layer
        if ((excludeLayer.value & (1 << collision.gameObject.layer)) == 0)
        {
            audioSource.Play();
        }
    }
}

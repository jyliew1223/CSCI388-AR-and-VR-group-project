using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class PuzzleBehaviour : MonoBehaviour
{
    [SerializeField]
    private AudioClip destroyClip;
    [SerializeField]
    private AudioSource destroyAudio;
    [SerializeField]
    private GameObject destroyEffectPrefab;
    [SerializeField]
    SpherePuzzleGameManager gameManager;

    [SerializeField]
    private int cheatCount = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Setting up
        destroyAudio.clip = destroyClip;
    }
    // Update is called once per frame
    void Update()
    {
    }
    private void OnCollisionEnter(Collision collision)
    {
        // check layer of the collided gameObject is not Excluded
        if (destroyEffectPrefab != null && collision.gameObject.isStatic)
        {
            destroyAudio.Play();

            // spwan praticle at the first contact point
            Vector3 contactPoint = collision.contacts[0].point;
            Instantiate(destroyEffectPrefab, contactPoint, Quaternion.identity);


            Destroy(gameObject);
        }
    }
    // allow player to cheat
    public void Cheating()
    {
        cheatCount++;

        if (cheatCount >= 4)
        {
            gameManager.Cheated();

            destroyAudio.Play();

            Instantiate(destroyEffectPrefab, transform.position, Quaternion.identity);

            Destroy(gameObject);
        }
    }
}

using System.Collections;
using TMPro;
using UnityEngine;

public class TrophyRoomController : MonoBehaviour
{
    public GameObject[] trophy;
    public Light[] lighting;
    public TextMeshProUGUI timetext;
    public AudioClip spot, horn, cheer;

    private bool lighted, whileLighting;
    private float currentTime;
    private AudioSource audioSource;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        if (GameManager.Instance)
        {
            currentTime = GameManager.Instance.GetTimer();
            GameManager.Instance.StopTimer();
        }

        if (currentTime > 1800f || currentTime < 1f)
        {
            trophy[2].SetActive(true);
            timetext.color = Color.red;
        }
        else if (currentTime > 600f)
        {
            trophy[1].SetActive(true);
            timetext.color = Color.yellow;
        }
        else if (currentTime >= 1f)
        {
            trophy[0].SetActive(true);
            timetext.color = Color.green;
        }

        UpdateTimerUI();
    }

    public void onGrab()
    {
        StartCoroutine(Lightshow());
    }

    private void UpdateTimerUI()
    {
        int hours = Mathf.FloorToInt(currentTime / 3600);
        int minutes = Mathf.FloorToInt((currentTime % 3600) / 60);
        int seconds = Mathf.FloorToInt(currentTime % 60);

        timetext.text = $"{hours:D2}:{minutes:D2}:{seconds:D2}";

    }

    IEnumerator Lightshow()
    {
        if (whileLighting) yield break;

        if (!lighted)
        {
            whileLighting = true;
            int count = 0;

            while (count < 6)
            {
                lighting[count].gameObject.SetActive(true);
                count++;
                lighting[count].gameObject.SetActive(true);
                count++;
                if (spot != null && audioSource != null)
                    audioSource.PlayOneShot(spot);
                yield return new WaitForSeconds(1f);
            }
            lighted = true;
            if (horn != null && audioSource != null)
                audioSource.PlayOneShot(horn);

            yield return new WaitForSeconds(2f);
            if (horn != null && audioSource != null)
                audioSource.PlayOneShot(cheer);
            whileLighting = false;
        }
    }
}

using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ButtonScript : MonoBehaviour
{
    Button button;

    private CustomJumpProvider jumpProvider;
    private bool canPress = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        jumpProvider = FindFirstObjectByType<CustomJumpProvider>();
        if (jumpProvider == null)
        {
            Debug.LogError("Cannot find jumpprovider in scene!");
        }
    }

    public void Flip()
    {
        if (canPress)
        {
            jumpProvider.boolSwap("fb");
            jumpProvider.Jump();
            StartCoroutine(Pressed());
        }
    }

    public void Rotate()
    {
        if (canPress)
        {
            jumpProvider.boolSwap("rb");
            jumpProvider.Jump();
            StartCoroutine(Pressed());
        }
    }

    IEnumerator Pressed()
    {
        canPress = false;

        yield return new WaitForSeconds(4f);

        canPress = true;
    }
}

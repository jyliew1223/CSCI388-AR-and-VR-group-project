using UnityEngine;

public class TestRoomController : MonoBehaviour
{
    public Light lighting;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeLightColour(Color lightColour)
    {
        lighting.color = lightColour;
    }
}

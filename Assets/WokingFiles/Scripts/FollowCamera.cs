using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    [SerializeField]
    private Camera m_Camera;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (m_Camera == null)
        {
            m_Camera = Camera.main;
        }
        else
        {
            Debug.LogWarning($"{this.GetType().Name}: Camera not assigned auto assigned");
        }
    }

    // Late Update is called after Update()
    void LateUpdate()
    {
        // set it to camera position
        transform.position = m_Camera.transform.position;

        // make it follow camera rotation on y axis
        Vector3 currentEuler = transform.localRotation.eulerAngles;
        Vector3 camEuler = m_Camera.transform.localRotation.eulerAngles;
        transform.localRotation = Quaternion.Euler(currentEuler.x, camEuler.y, currentEuler.z);
    }
}

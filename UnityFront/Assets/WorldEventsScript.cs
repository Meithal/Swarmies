using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WorldEventsScript : MonoBehaviour
{
    public UnityEvent m_event;
    
    // Start is called before the first frame update
    void Start()
    {
        if (m_event == null)
            m_event = new UnityEvent();

        m_event.AddListener(Ping);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKeyDown && m_event != null)
        {
            //m_event.Invoke();
            //Debug.Log(UnityEventQueueSystem.GenerateEventIdForPayload("MyCustomEvent"));

        }

    }
    
    void Ping()
    {
        Debug.Log("Ping");
    }

    public void Tick()
    {
        Debug.Log("Tick");
    }
}

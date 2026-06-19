using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventQueueSystem : MonoBehaviour
{
    public static EventQueueSystem Instance;

    private Queue<IGameEvent> queue = new Queue<IGameEvent>();
    private bool processing;

    private void Awake()
    {
        Instance = this;
    }

    public void Enqueue(IGameEvent gameEvent)
    {
        queue.Enqueue(gameEvent);

        if (!processing)
            StartCoroutine(ProcessQueue());
    }

    private IEnumerator ProcessQueue()
    {
        processing = true;

        while (queue.Count > 0)
        {
            IGameEvent gameEvent = queue.Dequeue();
            yield return StartCoroutine(gameEvent.Execute());
        }

        processing = false;
    }
}
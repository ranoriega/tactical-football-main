using System.Collections;
using UnityEngine;

public class BallManager : MonoBehaviour
{
    public static BallManager Instance;

    [SerializeField] GameObject ballPrefab;
    private GameObject currentBall;

    public Transform currentHolder;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
    }
    private void Update()
    {
     

        // 📌 Radio para detectar rivales
        // float stealRange = 0.6f;

        // Collider[] hits = Physics.OverlapSphere(currentHolder.position, stealRange);

        // foreach (Collider col in hits)
        // {
        //     PlayerTeam team = col.GetComponent<PlayerTeam>();

        //     if (team != null && team.teamID != currentHolder.GetComponent<PlayerTeam>().teamID)
        //     {
        //         // ⚡ Robo → rival quita el balón
        //         MyDebug.Log($"⚡ {col.name} intercepto el balón a {currentHolder.name}");
        //         GiveBallTo(col.transform);
        //         return; // salir en cuanto alguien robe
        //     }
        // }
    }



    public Transform GetCurrentBallHolder()
    {
        return currentHolder;
    }


    public void GiveBallTo(Transform holder)
    {
        currentHolder = holder;

        if (currentBall == null)
        {
            currentBall = Instantiate(ballPrefab, holder.position + Vector3.up * 0.5f, Quaternion.identity);
        }

        currentBall.transform.SetParent(holder);
        currentBall.transform.localPosition = new Vector3(0f, -0.084f, 0.62f); // ← balón en los pies
    }

Vector3 Flat(float t, Vector3 start, Vector3 end)
{
    return Vector3.Lerp(start, end, t);
}

Vector3 High(float t, Vector3 start, Vector3 end)
{
    Vector3 pos = Vector3.Lerp(start, end, t);
    pos.y += Mathf.Sin(t * Mathf.PI) * 1.5f;
    return pos;
}
Vector3 Curve(float t, Vector3 start, Vector3 end)
{
    Vector3 pos = Vector3.Lerp(start, end, t);

    // curva lateral (eje X o Z depende de tu juego)
    pos.x += Mathf.Sin(t * Mathf.PI) * 1.0f;

    return pos;
}

Vector3 Combine(float t, Vector3 start, Vector3 end)
{
    Vector3 pos = Vector3.Lerp(start, end, t);

    // curva lateral
    pos.x += Mathf.Sin(t * Mathf.PI) * 1.0f;

    // altura
    pos.y += Mathf.Sin(t * Mathf.PI) * 1.5f;

    return pos;
}
public void PassBallTo(Transform targetPlayer, PassType type, System.Action onComplete)
{
      Vector3 start = currentBall.transform.position;
        Vector3 end = targetPlayer.position + new Vector3(0f, 0.05f, 0.25f);

    if (currentBall == null || currentHolder == null)
    {
        onComplete?.Invoke();
        return;
    }

    // switch (type)
    // {
    //     case PassType.Ground:
    //         StartCoroutine(AnimateBallPass(targetPlayer, onComplete));
    //         break;

    //     case PassType.High:
    //         StartCoroutine(AnimateBallPassHigh(targetPlayer, onComplete));
    //         break;

    //     default:
    //         StartCoroutine(AnimateBallPass(targetPlayer, onComplete));
    //         break;
    // }
    System.Func<float, Vector3> trajectory = type switch
    {
        PassType.High => t => High(t, start, end),
         PassType.curve =>  t => Curve(t,start,end),
        _ => t => Flat(t, start, end)
    };

    StartCoroutine(AnimateBall(targetPlayer, trajectory, onComplete));
}

//   IEnumerator AnimateBallPass(Transform target, System.Action onComplete)
//     {
//          Vector3 start = currentBall.transform.position;
//         Vector3 end = target.position + new Vector3(0f, 0.05f, 0.25f);

//         float duration = 0.8f;
//         float elapsed = 0f;

//         currentBall.transform.SetParent(null); // balón libre

//         Transform interceptor = null;

//         while (elapsed < duration)
//         {
//             elapsed += Time.deltaTime;
//             float t = elapsed / duration;
//             Vector3 newPos = Vector3.Lerp(start, end, t);

//             // 📌 Chequear interceptores cercanos al balón
//             Collider[] hits = Physics.OverlapSphere(newPos, 0.3f); // radio de detección
//             foreach (Collider col in hits)
//             {
//                 PlayerTeam team = col.GetComponent<PlayerTeam>();
//                 if (team != null && team.teamID != currentHolder.GetComponent<PlayerTeam>().teamID)
//                 {
//                     // Es un rival → intercepta
//                     interceptor = col.transform;
//                     break;
//                 }
//             }

//             if (interceptor != null)
//             {
//                 MyDebug.Log($"Balón interceptado por {interceptor.name}");
//                 GiveBallTo(interceptor);
//                 yield break; // cancelar pase
//             }

//             currentBall.transform.position = newPos;
//             yield return null;
//         }

//         // Si nadie interceptó, llega al objetivo
//         GiveBallTo(target);
//           onComplete?.Invoke(); // 🔥 IMPORTANTE
//     }
    
IEnumerator AnimateBall(
    Transform target,
    System.Func<float, Vector3> trajectory,
    System.Action onComplete)
{
    float duration = 0.8f;
    float elapsed = 0f;

    currentBall.transform.SetParent(null);

    Transform interceptor = null;

    while (elapsed < duration)
    {
        elapsed += Time.deltaTime;
        float t = elapsed / duration;

        // 🔥 AQUÍ ESTÁ EL CAMBIO CLAVE
        Vector3 newPos = trajectory(t);

        // 📌 Intercepción (esto se queda igual)
        Collider[] hits = Physics.OverlapSphere(newPos, 0.3f);
        foreach (Collider col in hits)
        {
            PlayerTeam team = col.GetComponent<PlayerTeam>();
            if (team != null && team.teamID != currentHolder.GetComponent<PlayerTeam>().teamID)
            {
                interceptor = col.transform;
                break;
            }
        }

        if (interceptor != null)
        {
            MyDebug.Log($"Balón interceptado por {interceptor.name}");
            GiveBallTo(interceptor);
            yield break;
        }

        currentBall.transform.position = newPos;
        yield return null;
    }

    GiveBallTo(target);
    onComplete?.Invoke();
}


public void ShotBallTo(Transform shooter, Transform goalCenter, Vector3 offset_,System.Action onComplete)
    {
         if (currentBall == null || currentHolder == null)
    {
        onComplete?.Invoke();
        return;
    }
    StartCoroutine(AnimateBallShot( shooter,  goalCenter,  offset_, onComplete));
        
    }
    public IEnumerator AnimateBallShot(Transform shooter, Transform goalCenter, Vector3 offset_,System.Action onComplete)
    {
        if (shooter == null)
        {
            MyDebug.LogError("AnimateBallShot: shooter es null.");
            yield break;
        }
        if (goalCenter == null)
        {
            MyDebug.LogError("AnimateBallShot: goal es null.");
            yield break;
        }
        if (currentBall == null)
        {
            MyDebug.LogError("AnimateBallShot: currentBall es null.");
            yield break;
        }
        Vector3 start = currentBall.transform.position;
        Vector3 worldTarget = goalCenter.TransformPoint(offset_);  // No usar shooter para la posición inicial
        Vector3 end = worldTarget;

        float duration = 0.35f;
        float elapsed = 0f;

        currentBall.transform.SetParent(null);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            currentBall.transform.position = Vector3.Lerp(start, end, t);
            yield return null;
        }

       onComplete?.Invoke(); // 🔥 IMPORTANTE
        // Aquí puedes agregar verificación de gol, efectos, etc.
    }


  IEnumerator AnimateBallPassHigh(Transform target, System.Action onComplete)
    {
        Vector3 start = currentBall.transform.position;
        Vector3 end = target.position + new Vector3(0f, 0.05f, 0.25f);

        float duration = 0.8f; // un pase bombeado tarda un poco más
        float elapsed = 0f;

        currentBall.transform.SetParent(null); // balón libre

        Transform interceptor = null;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            // Línea base entre inicio y fin
            Vector3 newPos = Vector3.Lerp(start, end, t);

            // 📌 Añadir altura con parábola: sube al inicio, baja al final
            float height = Mathf.Sin(t * Mathf.PI) * 1.5f; // 1.5f = altura máxima
            newPos.y += height;

            // 📌 Chequear interceptores (jugadores que salten/corten)
            Collider[] hits = Physics.OverlapSphere(newPos, 0.3f);
            foreach (Collider col in hits)
            {
                PlayerTeam team = col.GetComponent<PlayerTeam>();
                if (team != null && team.teamID != currentHolder.GetComponent<PlayerTeam>().teamID)
                {
                    interceptor = col.transform;
                    break;
                }
            }

            if (interceptor != null)
            {
                MyDebug.Log($"⚡ Balón interceptado en el aire por {interceptor.name}");
                GiveBallTo(interceptor);
                yield break;
            }

            currentBall.transform.position = newPos;
            yield return null;
        }

        // ✅ Llega al receptor
        GiveBallTo(target);
            onComplete?.Invoke(); // 🔥 IMPORTANTE
    }


    public void DropBall()
    {
        if (currentBall != null)
        {
            currentBall.transform.SetParent(null);
            currentHolder = null;
        }
    }
    


    public bool HasBall(Transform player)
    {
        return currentHolder == player;
    }
}

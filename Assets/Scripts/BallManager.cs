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
    // Solo si alguien tiene el balón
    if (currentHolder == null) return;

    // 📌 Radio para detectar rivales
    float stealRange = 0.6f;

    Collider[] hits = Physics.OverlapSphere(currentHolder.position, stealRange);

    foreach (Collider col in hits)
    {
        PlayerTeam team = col.GetComponent<PlayerTeam>();

        if (team != null && team.teamID != currentHolder.GetComponent<PlayerTeam>().teamID)
        {
            // ⚡ Robo → rival quita el balón
            MyDebug.Log($"⚡ {col.name} robó el balón a {currentHolder.name}");
            GiveBallTo(col.transform);
            return; // salir en cuanto alguien robe
        }
    }
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


     public void PassBallTo(Transform targetPlayer)
    {
          MyDebug.Log($"hay pase");
        if (currentBall == null || currentHolder == null) return;

        StartCoroutine(AnimateBallPass(targetPlayer));
    }
   IEnumerator AnimateBallPass(Transform target)
{
    Vector3 start = currentBall.transform.position;
    Vector3 end = target.position + new Vector3(0f, 0.05f, 0.25f);

    float duration = 0.5f;
    float elapsed = 0f;

    currentBall.transform.SetParent(null); // balón libre

    Transform interceptor = null;

    while (elapsed < duration)
    {
        elapsed += Time.deltaTime;
        float t = elapsed / duration;
        Vector3 newPos = Vector3.Lerp(start, end, t);

        // 📌 Chequear interceptores cercanos al balón
        Collider[] hits = Physics.OverlapSphere(newPos, 0.3f); // radio de detección
        foreach (Collider col in hits)
        {
            PlayerTeam team = col.GetComponent<PlayerTeam>();
            if (team != null && team.teamID != currentHolder.GetComponent<PlayerTeam>().teamID)
            {
                // Es un rival → intercepta
                interceptor = col.transform;
                break;
            }
        }

        if (interceptor != null)
        {
            MyDebug.Log($"Balón interceptado por {interceptor.name}");
            GiveBallTo(interceptor);
            yield break; // cancelar pase
        }

        currentBall.transform.position = newPos;
        yield return null;
    }

    // Si nadie interceptó, llega al objetivo
    GiveBallTo(target);
}

    public void DropBall()
    {
        if (currentBall != null)
        {
            currentBall.transform.SetParent(null);
            currentHolder = null;
        }
    }

 
public IEnumerator AnimateBallShot(Transform shooter, Transform goalCenter, Vector3 offset_)
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

    MyDebug.Log("¡Tiro completado!");

    // Aquí puedes agregar verificación de gol, efectos, etc.
}

    


    public bool HasBall(Transform player)
    {
        return currentHolder == player;
    }
}

using System.Collections;
using UnityEngine;

public class PassUI : MonoBehaviour
{
    Coroutine currentRoutine;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public static PassUI Instance;
    public GameObject panel; // Panel con botones Pase Raso y Pase Alto
     public GameObject panel2; 
    private Transform passer;
    private Transform receiver;

    private void Awake()
    {
        Instance = this;
        panel.SetActive(false);
         panel2.SetActive(false);
    }

    public void OpenPassMenu(Transform passer, Transform receiver)
    {
        this.passer = passer;
        this.receiver = receiver;
        panel.SetActive(true);
    }
    public void OpenModeMenu()
{
    panel2.SetActive(true);

    if (currentRoutine != null)
        StopCoroutine(currentRoutine);

    currentRoutine = StartCoroutine(CloseAfterSeconds(1f));
}

IEnumerator CloseAfterSeconds(float seconds)
{
    yield return new WaitForSeconds(seconds);
    panel2.SetActive(false);
    currentRoutine = null;
}

    // Botones OnClick
    public void SelectPassLow()
    {
        PlayerActionQueue passerAccion = passer.GetComponent<PlayerActionQueue>();
        if (passerAccion == null)
        {
            Debug.LogError("El pasador no tiene PlayerActionQueue");
            return;
        }
        passerAccion.QueueAction(new PassAction(passer, receiver, PassType.Ground));
        panel.SetActive(false);
        this.passer = null;
        this.receiver = null;
        
    }

    public void SelectPassHigh()
    {
        PlayerActionQueue passerAccion = passer.GetComponent<PlayerActionQueue>();
        if (passerAccion == null)
        {
            Debug.LogError("El pasador no tiene PlayerActionQueue");
            return;
        }
        passerAccion.QueueAction(new PassAction(passer, receiver, PassType.High));
        panel.SetActive(false);
        this.passer = null;
        this.receiver = null;
        

    }
        public void SelectPassCurve()
    {
        PlayerActionQueue passerAccion = passer.GetComponent<PlayerActionQueue>();
        if (passerAccion == null)
        {
            Debug.LogError("El pasador no tiene PlayerActionQueue");
            return;
        }
        passerAccion.QueueAction(new PassAction(passer, receiver, PassType.curve));
        panel.SetActive(false);
        this.passer = null;
        this.receiver = null;
        

    }
   
      public void SelectPassCombine()
    {
        PlayerActionQueue passerAccion = passer.GetComponent<PlayerActionQueue>();
        if (passerAccion == null)
        {
            Debug.LogError("El pasador no tiene PlayerActionQueue");
            return;
        }
        passerAccion.QueueAction(new PassAction(passer, receiver, PassType.combine));
        panel.SetActive(false);
        this.passer = null;
        this.receiver = null;
        

    }

  
    
}

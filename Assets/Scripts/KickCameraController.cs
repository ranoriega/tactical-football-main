using UnityEngine;
using Unity.Cinemachine;
using System.Collections;

public class KickCameraController : MonoBehaviour
{
  
    public static KickCameraController Instance;
    public CinemachineCamera  kickCam;
    public GameObject panel;

    private void Awake()
    {
        Instance = this;
    }
    public void OnKick(Transform kicker)
    {
    Transform cameraPoint = kicker.Find("kickCamerapoint");

    kickCam.Follow = cameraPoint;
    kickCam.LookAt = cameraPoint;
        panel.SetActive(true);

       

        
        StartCoroutine(CloseAfterDelay(1.7f));
    }

IEnumerator CloseAfterDelay(float t)
{
    yield return new WaitForSeconds(t);
    EndKick();
}
    public void EndKick()
    {
        panel.SetActive(false);
       
    }
}

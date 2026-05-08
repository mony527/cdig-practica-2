using UnityEngine;
using Vuforia;

public class TargetDetector : MonoBehaviour
{
    public string id; 

    private Controlador controlador;

    void Start()
    {
        controlador = FindObjectOfType<Controlador>();
        var observer = GetComponent<ObserverBehaviour>();
        observer.OnTargetStatusChanged += OnStatusChanged;
    }

    private void OnStatusChanged(ObserverBehaviour behaviour, TargetStatus status)
    {
        if (status.Status == Status.TRACKED || status.Status == Status.EXTENDED_TRACKED)
        {
            controlador.ITPresente(id, transform);
        }
        else
        {
            controlador.ITAusente(id);
        }
    }
}

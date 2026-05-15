using UnityEngine;
using Vuforia;

public class VuforiaTorrijaDetector : MonoBehaviour
{
    public string id; 

    private ControladorAR controlador;

    void Start()
    {
        controlador = FindObjectOfType<ControladorAR>();
        var observer = GetComponent<ObserverBehaviour>();
        if (observer != null)
        {
            observer.OnTargetStatusChanged += OnStatusChanged;
        }
    }

    private void OnStatusChanged(ObserverBehaviour behaviour, TargetStatus status)
    {
        if (controlador != null)
        {
            if (status.Status == Status.TRACKED)
            {
                controlador.ITPresente(id);
            }
            else
            {
                controlador.ITAusente(id);
            }
        }
    }
}

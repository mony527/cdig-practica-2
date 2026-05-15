 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class ControladorUI : MonoBehaviour
{
    public ControladorAR controladorAR;
    public TextMeshProUGUI textoEstado;

    public List<string> ingredientesIDs = new List<string> { "panSeco", "leche", "canela", "aceite", "azucar", "bolHuevo" };
    public List<string> utensiliosIDs = new List<string> { "bandeja", "sarten", "plato" };

    private bool mostrarInfo = false;

    public static ControladorUI instancia { get; private set; }

    private void Awake()
    {
        if (instancia != null && instancia != this)
        {
            Destroy(this.gameObject);
            return;
        }
        instancia = this;
    }

    void Update()
    {
        ActualizarTextoEstado();
        DetectarInputInfo();
    }

    private void ActualizarTextoEstado()
    {
        if (controladorAR == null || textoEstado == null) return;

        bool faltanIngredientes = false;
        foreach (var id in ingredientesIDs)
        {
            if (!controladorAR.IsTargetPresent(id))
            {
                faltanIngredientes = true;
                break;
            }
        }

        bool faltanUtensilios = false;
        foreach (var id in utensiliosIDs)
        {
            if (!controladorAR.IsTargetPresent(id))
            {
                faltanUtensilios = true;
                break;
            }
        }

        if (!faltanIngredientes && !faltanUtensilios)
        {
            textoEstado.text = "Receta completa";
            textoEstado.color = Color.green;
        }
        else if (!faltanIngredientes && faltanUtensilios)
        {
            textoEstado.text = "Faltan utensilios";
            textoEstado.color = Color.yellow;
        }
        else if (faltanIngredientes && !faltanUtensilios)
        {
            textoEstado.text = "Faltan ingredientes";
            textoEstado.color = Color.yellow;
        }
        else
        {
            textoEstado.text = "Faltan elementos";
            textoEstado.color = Color.red;
        }
    }

    private void DetectarInputInfo()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            mostrarInfo = !mostrarInfo;
            controladorAR.ToggleLabels(mostrarInfo);
            controladorAR.ActualizarLabels();
        }
    }

    public bool IsRecetaCompleta()
    {
        return textoEstado.text.Equals("Receta completa");
    }

    public bool IsInfoActive()
    {
        return mostrarInfo;
    }
}

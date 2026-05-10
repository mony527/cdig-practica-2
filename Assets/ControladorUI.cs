 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class ControladorUI : MonoBehaviour
{
    [Header("Referencias")]
    public ControladorAR controladorAR;
    public TextMeshProUGUI textoEstado;

    [Header("Configuración de Elementos")]
    public List<string> ingredientesIDs = new List<string> { "panSeco", "leche", "canela", "aceite", "azucar", "bolHuevo" };
    public List<string> utensiliosIDs = new List<string> { "bandeja", "sarten", "plato" };

    private bool mostrarInfo = false;

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
        }
    }

    public bool IsRecetaCompleta()
    {
        return textoEstado.text.Equals("Receta completa");
    }
}

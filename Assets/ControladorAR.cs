using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using static ControladorAR;

public class ControladorAR : MonoBehaviour
{
    [System.Serializable]
    public class IngredienteData
    {
        public string id;              
        public string label;
        public GameObject prefab;      
        public Vector3 positionOffset; 
        public Vector3 rotationOffset; 
        [HideInInspector] public GameObject instancia; 
        [HideInInspector] public bool presente;        
    }

    public List<IngredienteData> ingredientes = new List<IngredienteData>();

    public GameObject txtIngrediente;
    public GameObject txtPanes;

    public enum PasosReceta
    {
        PanSeco,
        PanMojado,
        PanRebozado,
        PanFrito,
        PanDulce,
        Torrija
    }

    private PasosReceta pasoActual = PasosReceta.PanSeco;
    private bool sartenLista = false;
    private bool mezclaDulce = false;

    private Dictionary<string, IngredienteData> mapaIngredientes;

    void Start()
    {
        mapaIngredientes = new Dictionary<string, IngredienteData>();

        foreach (var t in ingredientes)
        {
            if (!mapaIngredientes.ContainsKey(t.id))
            {
                mapaIngredientes.Add(t.id, t);
                if (t.prefab != null)
                {
                    t.instancia = Instantiate(t.prefab);
                    t.instancia.name = t.id + "_Model";
                    t.instancia.SetActive(false);

                    AniadirEtiqueta(t.instancia, t.label);
                }
            }
        }
    }

    private void AniadirEtiqueta(GameObject model, string name)
    {
        if (model != null)
        {
            GameObject etiquetaObj = null;
            GameObject etiquetaObjOpt = null;
            TextMeshPro textMesh = null;
            TextMeshPro textMeshOpt = null;

            if (txtIngrediente != null)
            {
                if (name.StartsWith("Pan"))
                {
                    if (name.Equals("Pan dulce"))
                    {
                        etiquetaObjOpt = Instantiate(txtPanes, model.transform);
                        etiquetaObjOpt.name = "InfoLabelOpt";
                        textMeshOpt = etiquetaObjOpt.GetComponentInChildren<TextMeshPro>();
                    }
                    etiquetaObj = Instantiate(txtPanes, model.transform);
                }
                else
                {
                    etiquetaObj = Instantiate(txtIngrediente, model.transform);
                }

                etiquetaObj.name = "InfoLabel";
                textMesh = etiquetaObj.GetComponentInChildren<TextMeshPro>();
            }

            if (textMesh != null)
            {
                textMesh.text = name;
            }

            if (name.Equals("Pan dulce") && textMeshOpt != null)
            {
                textMeshOpt.text = "Torrija";
            }

            if (etiquetaObj != null)
            {
                var panelEtiqueta = etiquetaObj.GetComponent<PanelEtiqueta>();
                if (panelEtiqueta == null) panelEtiqueta = etiquetaObj.AddComponent<PanelEtiqueta>();
                panelEtiqueta.ingredienteModel = model;
                etiquetaObj.SetActive(false);
            }

            if (name.Equals("Pan dulce") && etiquetaObjOpt != null)
            {
                var panelEtiqueta = etiquetaObjOpt.GetComponent<PanelEtiqueta>();
                if (panelEtiqueta == null) panelEtiqueta = etiquetaObjOpt.AddComponent<PanelEtiqueta>();
                panelEtiqueta.ingredienteModel = model;
                etiquetaObjOpt.SetActive(false);
            }
        }
    }

    public void ActivarEtiquetas(bool visible)
    {
        foreach (var t in ingredientes)
        {
            if (t.instancia != null)
            {
                if (t.id.Equals("Pan dulce") && pasoActual == PasosReceta.Torrija)
                {
                    Transform labelOpt = t.instancia.transform.Find("InfoLabelOpt");
                    if (labelOpt != null) labelOpt.gameObject.SetActive(visible);
                }
                else { 
                    Transform label = t.instancia.transform.Find("InfoLabel");
                    if (label != null) label.gameObject.SetActive(visible);
                }
            }
        }
    }


    public bool IsTargetPresent(string id)
    {
        return mapaIngredientes.ContainsKey(id) && mapaIngredientes[id].presente;
    }

    public void ITPresente(string id)
    {
        if (mapaIngredientes.ContainsKey(id))
        {
            mapaIngredientes[id].presente = true;
            ActualizarEstados();
        }
    }

    public void ITAusente(string id)
    {
        if (mapaIngredientes.ContainsKey(id))
        {
            mapaIngredientes[id].presente = false;
            ComprobarPasosPrevios(id);
            ActualizarEstados();
        }
    }

    private void ComprobarPasosPrevios(string id)
    {
        if ((id == "bandeja" || id == "leche" || id == "panSeco") && pasoActual >= PasosReceta.PanMojado) pasoActual = PasosReceta.PanSeco;
        if (id == "bolHuevo" && pasoActual >= PasosReceta.PanRebozado) pasoActual = PasosReceta.PanMojado;
        if ((id == "sarten" || id == "aceite") && (pasoActual >= PasosReceta.PanFrito || sartenLista)) 
        {
            if (pasoActual >= PasosReceta.PanFrito) pasoActual = PasosReceta.PanRebozado;
            sartenLista = false;
        }
        if ((id == "azucar" || id == "canela") && (pasoActual >= PasosReceta.PanDulce || mezclaDulce))
        {
            if (pasoActual >= PasosReceta.PanDulce) pasoActual = PasosReceta.PanFrito;
            mezclaDulce = false;
        }
        if (id == "plato" && pasoActual == PasosReceta.Torrija) pasoActual = PasosReceta.PanDulce;
    }

    private void ActualizarEstados()
    {
        if (!sartenLista && IsTargetPresent("aceite") && IsTargetPresent("sarten"))
            sartenLista = true;

        if (!mezclaDulce && IsTargetPresent("azucar") && IsTargetPresent("canela"))
            mezclaDulce = true;

        if (pasoActual == PasosReceta.PanSeco)
        {
            if (IsTargetPresent("panSeco") && IsTargetPresent("leche") && IsTargetPresent("bandeja"))
                pasoActual = PasosReceta.PanMojado;
        }

        if (pasoActual == PasosReceta.PanMojado)
        {
            if (IsTargetPresent("bolHuevo"))
                pasoActual = PasosReceta.PanRebozado;
        }

        if (pasoActual == PasosReceta.PanRebozado)
        {
            if (sartenLista && IsTargetPresent("sarten"))
                pasoActual = PasosReceta.PanFrito;
        }

        if (pasoActual == PasosReceta.PanFrito)
        {
            if (mezclaDulce && IsTargetPresent("azucar"))
                pasoActual = PasosReceta.PanDulce;
        }

        if (pasoActual == PasosReceta.PanDulce)
        {
            if (IsTargetPresent("plato"))
                pasoActual = PasosReceta.Torrija;
            ActualizarEtiquetas();
        }

        ActualizarVisuales();
    }

    public void ActualizarEtiquetas()
    {
        IngredienteData panDulce = mapaIngredientes["panDulce"];
        Transform labelOpt = panDulce.instancia.transform.Find("InfoLabelOpt");
        Transform label = panDulce.instancia.transform.Find("InfoLabel");

        if (pasoActual == PasosReceta.Torrija)
        {
            if (label != null) label.gameObject.SetActive(false);
            if (labelOpt != null) labelOpt.gameObject.SetActive(ControladorUI.instancia.IsInfoActive());
        }

        if (pasoActual == PasosReceta.PanDulce)
        {
            if (labelOpt != null) labelOpt.gameObject.SetActive(false);
            if (label != null) label.gameObject.SetActive(ControladorUI.instancia.IsInfoActive());
        }
    }

    private void ActualizarVisuales()
    {
        foreach (var t in ingredientes)
        {
            if (t.instancia != null) t.instancia.SetActive(false);
        }

        foreach (var t in ingredientes)
        {
            if (t.presente && t.instancia != null)
            {
                if (IngredienteBaseNoConsumido(t.id))
                {
                    ShowAt(t.id, t.id);
                }
            }
        }

        if (sartenLista)
        {
            ShowAt("sartenLista", "sarten");
        }

        if (mezclaDulce)
        {
            ShowAt("mezclaDulce", "azucar");
        }

        switch (pasoActual)
        {
            case PasosReceta.PanMojado:
                ShowAt("panMojado", "bandeja"); 
                break;
            case PasosReceta.PanRebozado:
                ShowAt("panRebozado", "bolHuevo");
                break;
            case PasosReceta.PanFrito:
                ShowAt("panFrito", "sartenLista");
                break;
            case PasosReceta.PanDulce:
                ShowAt("panDulce", "mezclaDulce");
                break;
            case PasosReceta.Torrija:
                ShowAt("panDulce", "plato");
                break;
        }

    }

    private bool IngredienteBaseNoConsumido(string id)
    {
        bool isConsumable = (id == "panSeco" || id == "canela" || id == "leche" || id == "aceite" || id == "sarten" || id == "azucar");

        if (!isConsumable) return true;

        if (pasoActual >= PasosReceta.PanMojado && (id == "panSeco" || id == "leche")) return false;

        if (sartenLista && (id == "aceite" || id == "sarten")) return false;
        if (mezclaDulce && (id == "canela" || id == "azucar")) return false;

        return true;
    }

    private void ShowAt(string origenId, string destinoId)
    {
        if (mapaIngredientes.ContainsKey(origenId) && mapaIngredientes.ContainsKey(destinoId))
        {
            GameObject ingredienteOrigen = mapaIngredientes[origenId].instancia;
            if (ingredienteOrigen != null)
            {
                VuforiaTorrijaDetector td = FindTargetDetector(destinoId);
                if (td != null)
                {
                    ingredienteOrigen.SetActive(true);
                    ingredienteOrigen.transform.SetParent(td.transform);
                    ingredienteOrigen.transform.localPosition = mapaIngredientes[origenId].positionOffset;
                    ingredienteOrigen.transform.localRotation = Quaternion.Euler(mapaIngredientes[origenId].rotationOffset);
                }
                else
                {
                    GameObject ingredienteDestino = mapaIngredientes[destinoId].instancia;
                    if (ingredienteDestino != null && ingredienteDestino.activeInHierarchy)
                    {
                        ingredienteOrigen.SetActive(true);
                        ingredienteOrigen.transform.SetParent(ingredienteDestino.transform);
                        ingredienteOrigen.transform.localPosition = mapaIngredientes[origenId].positionOffset;
                        ingredienteOrigen.transform.localRotation = Quaternion.Euler(mapaIngredientes[origenId].rotationOffset);
                    }
                }
            }
        }
    }

    public VuforiaTorrijaDetector FindTargetDetector(string id)
    {
        VuforiaTorrijaDetector[] detectors = FindObjectsOfType<VuforiaTorrijaDetector>();
        int i = 0;
        bool found = false;
        VuforiaTorrijaDetector result = null;
        while (i < detectors.Length && !found) 
        {
            var detector = detectors[i];
            if (detector.id == id)
            {
                found = true;
                result = detector;
            }
            i++;
        }
        return result;
    }

    public Transform GetDetectorTransform(string id)
    {
        VuforiaTorrijaDetector td = FindTargetDetector(id);
        return td != null ? td.transform : null;
    }

    public void SetVisibilidadIngrediente(string id, bool visible)
    {
        if (mapaIngredientes.ContainsKey(id) && mapaIngredientes[id].instancia != null)
        {
            mapaIngredientes[id].instancia.SetActive(visible);
        }
    }
}

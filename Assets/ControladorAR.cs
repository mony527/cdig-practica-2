using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using static ControladorAR;

public class ControladorAR : MonoBehaviour
{
    [System.Serializable]
    public class TargetData
    {
        public string id;              
        public GameObject prefab;      
        public Vector3 positionOffset; 
        public Vector3 rotationOffset; 
        [HideInInspector] public GameObject instancia; 
        [HideInInspector] public bool presente;        
    }

    public List<TargetData> targets = new List<TargetData>();

    public enum RecipeStep
    {
        PanSeco,
        PanMojado,
        PanRebozado,
        PanFrito,
        PanDulce,
        Torrija
    }

    private RecipeStep currentStep = RecipeStep.PanSeco;
    private bool sartenLista = false;
    private bool mezclaDulce = false;

    private Dictionary<string, TargetData> mapa;

    void Start()
    {
        mapa = new Dictionary<string, TargetData>();

        foreach (var t in targets)
        {
            if (mapa.ContainsKey(t.id)) continue;
            mapa.Add(t.id, t);
            if (t.prefab != null)
            {
                t.instancia = Instantiate(t.prefab);
                t.instancia.name = t.id + "_Model";
                t.instancia.SetActive(false);
                
                AddLabelToModel(t.instancia, t.id);
            }
        }
    }

    private void AddLabelToModel(GameObject model, string name)
    {
        GameObject labelObj = new GameObject("InfoLabel");
        labelObj.transform.SetParent(model.transform);

        var textMesh = labelObj.AddComponent<TextMeshPro>();
        textMesh.text = name;
        textMesh.fontSize = 5;
        textMesh.alignment = TextAlignmentOptions.Center;
        textMesh.color = Color.blue;
        
        var billboard = labelObj.AddComponent<LabelBillboard>();
        billboard.targetModel = model;
        
        labelObj.SetActive(false);
    }

    public void ToggleLabels(bool visible)
    {
        foreach (var t in targets)
        {
            if (t.instancia != null)
            {
                Transform label = t.instancia.transform.Find("InfoLabel");
                if (label != null) label.gameObject.SetActive(visible);
            }
        }
    }

    public bool IsTargetPresent(string id)
    {
        return IsPresent(id);
    }

    public void ITPresente(string id)
    {
        if (mapa.ContainsKey(id))
        {
            mapa[id].presente = true;
            ActualizarEstados();
        }
    }

    public void ITAusente(string id)
    {
        if (mapa.ContainsKey(id))
        {
            mapa[id].presente = false;
            CheckReversion(id);
            ActualizarEstados();
        }
    }

    private void CheckReversion(string id)
    {
        if (id == "bandeja" && currentStep == RecipeStep.PanMojado) currentStep = RecipeStep.PanSeco;
        if (id == "bolHuevo" && currentStep == RecipeStep.PanRebozado) currentStep = RecipeStep.PanMojado;
        if (id == "sarten" && (currentStep == RecipeStep.PanFrito || sartenLista)) 
        {
            if (currentStep == RecipeStep.PanFrito) currentStep = RecipeStep.PanRebozado;
            sartenLista = false;
        }
        if (id == "azucar" && (currentStep == RecipeStep.PanDulce || mezclaDulce))
        {
            if (currentStep == RecipeStep.PanDulce) currentStep = RecipeStep.PanFrito;
            mezclaDulce = false;
        }
        if (id == "plato" && currentStep == RecipeStep.Torrija) currentStep = RecipeStep.PanDulce;
    }

    private void ActualizarEstados()
    {
        if (!sartenLista && IsPresent("aceite") && IsPresent("sarten"))
            sartenLista = true;

        if (!mezclaDulce && IsPresent("azucar") && IsPresent("canela"))
            mezclaDulce = true;

        if (currentStep == RecipeStep.PanSeco)
        {
            if (IsPresent("panSeco") && IsPresent("leche") && IsPresent("bandeja"))
                currentStep = RecipeStep.PanMojado;
        }

        if (currentStep == RecipeStep.PanMojado)
        {
            if (IsPresent("bolHuevo"))
                currentStep = RecipeStep.PanRebozado;
        }

        if (currentStep == RecipeStep.PanRebozado)
        {
            if (sartenLista && IsPresent("sarten"))
                currentStep = RecipeStep.PanFrito;
        }

        if (currentStep == RecipeStep.PanFrito)
        {
            if (mezclaDulce && IsPresent("azucar"))
                currentStep = RecipeStep.PanDulce;
        }

        if (currentStep == RecipeStep.PanDulce)
        {
            if (IsPresent("plato"))
                currentStep = RecipeStep.Torrija;
        }

        ActualizarVisuales();
    }

    private bool IsPresent(string id)
    {
        return mapa.ContainsKey(id) && mapa[id].presente;
    }

    private void ActualizarVisuales()
    {
        foreach (var t in targets)
        {
            if (t.instancia != null) t.instancia.SetActive(false);
        }

        foreach (var t in targets)
        {
            if (t.presente && t.instancia != null)
            {
                if (IsBaseIngredientNotConsumed(t.id))
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

        switch (currentStep)
        {
            case RecipeStep.PanMojado:
                ShowAt("panMojado", "bandeja"); 
                break;
            case RecipeStep.PanRebozado:
                ShowAt("panRebozado", "bolHuevo");
                break;
            case RecipeStep.PanFrito:
                ShowAt("panFrito", "sartenLista");
                break;
            case RecipeStep.PanDulce:
                ShowAt("panDulce", "mezclaDulce");
                break;
            case RecipeStep.Torrija:
                ShowAt("panDulce", "plato");
                break;
        }

    }

    private bool IsBaseIngredientNotConsumed(string id)
    {
        // Consumable elements: basic ingredients and utensils when replaced by mixtures
        bool isConsumable = (id == "panSeco" || id == "canela" || id == "leche" || id == "aceite" || id == "sarten" || id == "azucar");

        if (!isConsumable) return true;

        if (currentStep >= RecipeStep.PanMojado && (id == "panSeco" || id == "leche")) return false;

        if (sartenLista && (id == "aceite" || id == "sarten")) return false; // sarten is replaced by sartenLista
        if (mezclaDulce && (id == "canela" || id == "azucar")) return false; // azucar is replaced by mezclaDulce

        return true;
    }

    private void ShowAt(string productId, string targetId)
    {
        if (mapa.ContainsKey(productId) && mapa.ContainsKey(targetId))
        {
            GameObject product = mapa[productId].instancia;
            if (product == null) return;

            VuforiaTorrijaDetector td = FindTargetDetector(targetId);
            if (td != null)
            {
                product.SetActive(true);
                product.transform.SetParent(td.transform);
                product.transform.localPosition = mapa[productId].positionOffset;
                product.transform.localRotation = Quaternion.Euler(mapa[productId].rotationOffset);
                return;
            }

            GameObject targetModel = mapa[targetId].instancia;
            if (targetModel != null && targetModel.activeInHierarchy)
            {
                product.SetActive(true);
                product.transform.SetParent(targetModel.transform);
                product.transform.localPosition = mapa[productId].positionOffset;
                product.transform.localRotation = Quaternion.Euler(mapa[productId].rotationOffset);
            }
        }
    }

    public VuforiaTorrijaDetector FindTargetDetector(string id)
    {
        VuforiaTorrijaDetector[] detectors = FindObjectsOfType<VuforiaTorrijaDetector>();
        foreach (var d in detectors)
        {
            if (d.id == id) return d;
        }
        return null;
    }

    public RecipeStep GetCurrentStep()
    {
        return currentStep;
    }

    public Transform GetDetectorTransform(string id)
    {
        VuforiaTorrijaDetector td = FindTargetDetector(id);
        return td != null ? td.transform : null;
    }

    public void SetModelVisibility(string id, bool visible)
    {
        if (mapa.ContainsKey(id) && mapa[id].instancia != null)
        {
            mapa[id].instancia.SetActive(visible);
        }
    }
}

public class LabelBillboard : MonoBehaviour
{
    public GameObject targetModel;
    public float verticalOffset = 0.2f;
    public float cameraBias = 0.1f;

    void LateUpdate()
    {
        if (targetModel == null || !targetModel.activeInHierarchy) return;

        Renderer[] renderers = targetModel.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0) return;

        Bounds bounds = new Bounds();
        bool first = true;
        foreach (var r in renderers)
        {
            if (!r.enabled || r.gameObject == gameObject) continue;
            
            if (first)
            {
                bounds = r.bounds;
                first = false;
            }
            else
            {
                bounds.Encapsulate(r.bounds);
            }
        }

        if (first) return;

        Vector3 camPos = Camera.main.transform.position;
        Vector3 closestPoint = bounds.ClosestPoint(camPos);

        Vector3 targetPos = new Vector3(closestPoint.x, bounds.max.y + verticalOffset, closestPoint.z);

        Vector3 dirToCam = (camPos - targetPos).normalized;
        targetPos += dirToCam * cameraBias;

        transform.position = targetPos;
        transform.rotation = Quaternion.LookRotation(transform.position - camPos);
    }
}
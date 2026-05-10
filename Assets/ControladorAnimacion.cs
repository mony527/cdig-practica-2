using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControladorAnimacion : MonoBehaviour
{
    [Header("Referencias")]
    public ControladorAR controladorAR;
    public ControladorUI controladorUI;
    public GameObject prefabPan;

    [Header("Configuración Animación")]
    public float velocidad = 2.0f;
    public float distanciaUmbral = 0.1f;
    public Vector3 offsetPosicion = new Vector3(0, 1.0f, 0);
    public Vector3 offsetRotacion = new Vector3(90, 0, 0);

    private GameObject panAnimado;
    private int indiceActual = 0;
    private string[] secuenciaIDs = { "bandeja", "bolHuevo", "sarten", "azucar", "plato" };
    private bool animacionActivaAnteriormente = false;

    void Start()
    {
        if (prefabPan != null)
        {
            panAnimado = Instantiate(prefabPan);
            panAnimado.name = "Pan_Animacion_Loop";
            panAnimado.SetActive(false);
        }
    }

    void Update()
    {
        bool recetaCompleta = controladorUI != null && controladorUI.IsRecetaCompleta();
        bool teclaPulsada = Input.GetKey(KeyCode.A);

        if (recetaCompleta && teclaPulsada)
        {
            if (!animacionActivaAnteriormente)
            {
                IniciarSesionAnimacion();
            }
            EjecutarAnimacion();
            animacionActivaAnteriormente = true;
        }
        else
        {
            if (animacionActivaAnteriormente)
            {
                ResetearAnimacion();
            }
            animacionActivaAnteriormente = false;
        }
    }

    private void IniciarSesionAnimacion()
    {
        if (panAnimado != null)
        {
            indiceActual = 0;
            panAnimado.SetActive(true);

            controladorAR.SetModelVisibility("panDulce", false);

            Transform firstTarget = controladorAR.GetDetectorTransform(secuenciaIDs[0]);
            if (firstTarget != null)
            {
                panAnimado.transform.position = firstTarget.position + firstTarget.TransformDirection(offsetPosicion);
                panAnimado.transform.rotation = firstTarget.rotation * Quaternion.Euler(offsetRotacion);
            }
        }
    }

    private void EjecutarAnimacion()
    {
        if (panAnimado != null)
        {
            string idObjetivo = secuenciaIDs[indiceActual];
            Transform targetTransform = controladorAR.GetDetectorTransform(idObjetivo);

            if (targetTransform != null)
            {
                Vector3 worldOffset = targetTransform.TransformDirection(offsetPosicion);
                Vector3 targetPos = targetTransform.position + worldOffset;
                Quaternion targetRot = targetTransform.rotation * Quaternion.Euler(offsetRotacion);

                // Mover hacia el objetivo
                panAnimado.transform.position = Vector3.MoveTowards(
                    panAnimado.transform.position,
                    targetPos,
                    velocidad * Time.deltaTime
                );

                // Rotar hacia la orientación tumbada del objetivo
                panAnimado.transform.rotation = Quaternion.Slerp(
                    panAnimado.transform.rotation,
                    targetRot,
                    velocidad * Time.deltaTime
                );

                // Si llegamos al punto, pasar al siguiente
                if (Vector3.Distance(panAnimado.transform.position, targetPos) < distanciaUmbral)
                {
                    indiceActual = (indiceActual + 1) % secuenciaIDs.Length;
                }
            }
            else
            {
                // Si el tracker no está visible, intentamos saltar para no bloquear el bucle
                indiceActual = (indiceActual + 1) % secuenciaIDs.Length;
            }
        }
    }

    private void ResetearAnimacion()
    {
        if (panAnimado != null)
        {
            panAnimado.SetActive(false);
            indiceActual = 0;
            controladorAR.SetModelVisibility("panDulce", true);
        }
    }
}

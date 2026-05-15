using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelEtiqueta : MonoBehaviour
{
    public GameObject ingredienteModel;
    public float verticalOffset = 1.0f;
    public float cameraBias = 0.1f;
    private Vector3 initialLocalScale;

    void Start()
    {
        initialLocalScale = transform.localScale;
    }

    void LateUpdate()
    {
        if (ingredienteModel != null && ingredienteModel.activeInHierarchy)
        {
            if (transform.parent != null)
            {
                Vector3 parentScale = transform.parent.lossyScale;
                transform.localScale = new Vector3(
                    initialLocalScale.x / (parentScale.x != 0 ? parentScale.x : 1f),
                    initialLocalScale.y / (parentScale.y != 0 ? parentScale.y : 1f),
                    initialLocalScale.z / (parentScale.z != 0 ? parentScale.z : 1f)
                );
            }

            Renderer[] renderers = ingredienteModel.GetComponentsInChildren<Renderer>();
            if (renderers.Length != 0)
            {
                Bounds bounds = new Bounds();
                bool first = true;
                foreach (var r in renderers)
                {
                    if (r.enabled && r.gameObject != gameObject)
                    {
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
                }

                if (!first)
                {
                    Vector3 camPos = Camera.main.transform.position;

                    Vector3 targetPos = new Vector3(bounds.center.x, bounds.max.y + verticalOffset, bounds.center.z);

                    Vector3 dirToCam = (camPos - targetPos).normalized;
                    targetPos += dirToCam * cameraBias;

                    transform.position = targetPos;
                    transform.rotation = Quaternion.LookRotation(transform.position - camPos);
                }

            }
        }
    }
}

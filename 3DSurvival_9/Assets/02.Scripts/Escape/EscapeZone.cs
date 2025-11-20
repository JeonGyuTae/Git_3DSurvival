using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscapeZone : MonoBehaviour
{
    [SerializeField] private Vector3 areaScale = new Vector3(10f, 0, 10f);

    private void Start()
    {
        LineRenderer[] lineRenderers = GetComponentsInChildren<LineRenderer>();
        Vector3 pivot = this.transform.position;

        DrawRay(lineRenderers[0], new Vector3(pivot.x + areaScale.x / 2, pivot.y, pivot.z + areaScale.z / 2), 
                                  new Vector3(pivot.x - areaScale.x / 2, pivot.y, pivot.z + areaScale.z / 2));

        DrawRay(lineRenderers[1], new Vector3(pivot.x + areaScale.x / 2, pivot.y, pivot.z - areaScale.z / 2),
                                  new Vector3(pivot.x + areaScale.x / 2, pivot.y, pivot.z + areaScale.z / 2));

        DrawRay(lineRenderers[2], new Vector3(pivot.x - areaScale.x / 2, pivot.y, pivot.z - areaScale.z / 2),
                                  new Vector3(pivot.x - areaScale.x / 2, pivot.y, pivot.z + areaScale.z / 2));

        DrawRay(lineRenderers[3], new Vector3(pivot.x - areaScale.x / 2, pivot.y, pivot.z - areaScale.z / 2),
                                  new Vector3(pivot.x + areaScale.x / 2, pivot.y, pivot.z - areaScale.z / 2));
    }

    private void DrawRay(LineRenderer renderer, Vector3 startPos, Vector3 endPos)
    {
        renderer.SetPosition(0, startPos);
        renderer.SetPosition(1, endPos);
    }

    public Vector3 AreaScale { get { return areaScale; } }
    public Vector3 Pivot { get { return this.transform.position; } }
}

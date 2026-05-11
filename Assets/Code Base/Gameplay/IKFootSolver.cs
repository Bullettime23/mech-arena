using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class IKFootSolver : MonoBehaviour
{
    /// <summary>
    /// Used in raycast to calculate foot
    /// </summary>
    [SerializeField] LayerMask terrainLayer = default;
    /// <summary>
    /// Used in raycast calcualtion
    /// </summary>
    [SerializeField] Transform body = default;
    [SerializeField] IKFootSolver otherFoot = default;
    [SerializeField] float speed = 1;
    /// <summary>
    /// Minimal distance when step needs to be done
    /// </summary>
    [SerializeField] float stepDistance = 4;
    /// <summary>
    /// In calculation of new foot position
    /// </summary>
    [SerializeField] float stepLength = 4;
    /// <summary>
    /// Foot max elevation during the step
    /// </summary>
    [SerializeField] float stepHeight = 1;
    /// <summary>
    /// Some initial distance of the foot
    /// </summary>
    [SerializeField] Vector3 footOffset = default;

    [SerializeField] Transform footTip;
    /// <summary>
    /// X of the Solver position
    /// </summary>
    float footSpacing;
    Vector3 oldPosition, currentPosition, newPosition;
    Vector3 oldNormal, currentNormal, newNormal;
    float lerp;

    private void Start()
    {
        footSpacing = transform.localPosition.x;
        currentPosition = newPosition = oldPosition = transform.position;
        currentNormal = newNormal = oldNormal = transform.up;
        lerp = 1;
    }

    // Update is called once per frame

    void Update()
    {
        transform.position = currentPosition;
        transform.rotation = footTip.rotation;
        transform.up = currentNormal;

        Ray ray = new Ray(body.position + (body.right * footSpacing), Vector3.down);

        if (Physics.Raycast(ray, out RaycastHit info, 10, terrainLayer.value))
        {
            if (Vector3.Distance(newPosition, info.point) > stepDistance && !otherFoot.IsMoving() && lerp >= 1)
            {
                lerp = 0;
                // Forward or backward
                int direction = body.InverseTransformPoint(info.point).z > body.InverseTransformPoint(newPosition).z ? 1 : -1;
                // set new step position
                newPosition = info.point + (body.forward * stepLength * direction) + footOffset;
                newNormal = info.normal;
            }
        }

        if (lerp < 1)
        {
            Vector3 tempPosition = Vector3.Lerp(oldPosition, newPosition, lerp);
            double tempY = (double)Mathf.Sin(lerp * Mathf.PI) * stepHeight;
            tempPosition.y += (float)tempY;

            currentPosition = tempPosition;
            currentNormal = Vector3.Lerp(oldNormal, newNormal, lerp);
            lerp += Time.deltaTime * speed;
        }
        else
        {
            oldPosition = newPosition;
            oldNormal = newNormal;
        }
    }

#if UNIY_EDITOR

    private Color gizmoColor = new Color(1f, 0, 0, 0.5f);
    private Color gizmoColorGreen = new Color(0, 1, 0, 0.5f);

    private void OnDrawGizmos()
    {

        Gizmos.color = gizmoColor;
        Gizmos.DrawSphere(newPosition, 0.5f);
        Gizmos.color = gizmoColorGreen;

        Gizmos.DrawLine(body.position + (body.right * footSpacing), Vector3.down * 20);
    }

#endif

    /// <summary>
    /// Only one foot can move
    /// </summary>
    /// <returns></returns>
    public bool IsMoving()
    {
        return lerp < 1;
    }



}

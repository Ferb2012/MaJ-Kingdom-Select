using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class KingdomSelect : MonoBehaviour
{
    public List<Kindom> kindoms = new List<Kindom>();

    [Space]

    [Header("Public References")]
    public GameObject kindomPointPrefab;
    public GameObject kindomButtonPrefab;
    public Transform modelTransform;
    public Transform kindomButtonsContainer;

    [Space]

    [Header("Tween Settings")]
    public float lookDuration;
    public Ease lookEase;

    [Space]
    public Vector2 visualOffset;

    private void Start()
    {
        foreach (Kindom k in kindoms)
        {
            SpawnKindomPoint(k);
        }

        if (kindoms.Count > 0)
        {
            LookAtKindom(kindoms[0]);
            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(kindomButtonsContainer.GetChild(0).gameObject);
        }
    }

    private void SpawnKindomPoint(Kindom k)
    {
        GameObject kindom = Instantiate(kindomPointPrefab, modelTransform);
        kindom.transform.localEulerAngles = new Vector3(k.y + visualOffset.y, -k.x - visualOffset.x, 0);
        k.visulaPoint = kindom.transform.GetChild(0);

        SpawnKindomButton(k);
    }

    private void SpawnKindomButton(Kindom k)
    {
        Button kindomButton = Instantiate(kindomButtonPrefab, kindomButtonsContainer).GetComponent<Button>();
        kindomButton.onClick.AddListener(() => LookAtKindom(k));

        kindomButton.transform.GetChild(0).GetComponentInChildren<Text>().text = k.name;
    }

    public void LookAtKindom(Kindom k)
    {
        Transform cameraParent = Camera.main.transform.parent;
        Transform cameraPivot = cameraParent.parent;

        cameraParent.DOLocalRotate(new Vector3(k.y, 0, 0), lookDuration, RotateMode.Fast).SetEase(lookEase);
        cameraPivot.DOLocalRotate(new Vector3(0, -k.x, 0), lookDuration, RotateMode.Fast).SetEase(lookEase);

        FindObjectOfType<FollowTarget>().target = k.visulaPoint;
    }

    private void OnDrawGizmos()
    {
#if UNITY_EDITOR
        Gizmos.color = Color.red;

        // only draw if there is at least one stage
        if (kindoms.Count > 0)
        {
            for (int i = 0; i < kindoms.Count; i++)
            {
                // creat two empt objects
                GameObject point = new GameObject();
                GameObject parent = new GameObject();
                // move the point object to the front of the world sphere
                point.transform.position += -new Vector3(0, 0, .5f);
                // parent the point to the "parent" object in the center
                point.transform.parent = parent.transform;
                // set the visual offset
                parent.transform.eulerAngles = new Vector3(visualOffset.y, -visualOffset.x, 0);

                if (!Application.isPlaying)
                {
                    Gizmos.DrawWireSphere(point.transform.position, 0.02f);
                }

                // spint the parent object based one the stage coordinates
                parent.transform.eulerAngles += new Vector3(kindoms[i].y, -kindoms[i].x, 0);
                // draw a gizmo sphere // handle label in the point object's position
                Gizmos.DrawSphere(point.transform.position, 0.07f);
                // destory all
                DestroyImmediate(point);
                DestroyImmediate(parent);
            }
        }
#endif
    }
}

[System.Serializable]
public class Kindom
{
    public string name;

    [Range(-180, 180)]
    public float x;
    [Range(-180, 89)]
    public float y;

    [HideInInspector]
    public Transform visulaPoint;
}

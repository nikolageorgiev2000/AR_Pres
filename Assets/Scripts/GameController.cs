using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

//[RequireComponent (typeof (ARRaycastManager))]
public class GameController : MonoBehaviour {

    //m_ are member variables
    public GameObject m_ballPrefab;
    public GameObject m_ARCamera;
    public GameObject m_hoopPrefab;
    public GameObject m_content;
    public GameObject m_sliders;

    List<ARRaycastHit> rayHits = new List<ARRaycastHit> ();

    ARRaycastManager m_RayCastManager;
    ARSessionOrigin m_SessionOrigin;

    private bool m_hoopPlaced;
    private GameObject m_spawnedHoop;
    private GameObject m_spawnedBall;

    Quaternion m_hoopRotation;

    //Property allows RotationController to access it
    public Quaternion rotation
    {
        get { return m_hoopRotation; }
        set
        {
            m_hoopRotation = value;
            if (m_SessionOrigin != null)
                m_SessionOrigin.MakeContentAppearAt(m_content.transform, m_content.transform.position, m_hoopRotation);
        }
    }

    void Start () {
        m_hoopPlaced = false;
        m_RayCastManager = GetComponent<ARRaycastManager>();
        m_SessionOrigin = GetComponent<ARSessionOrigin>();

        m_spawnedHoop = null;
        m_spawnedBall = null;
    }

    void Update () {
        if (Input.touchCount == 0)
        {
            return;
        }

        Vector3 touchPosition = Input.GetTouch(0).position;

        if (m_hoopPlaced)
        {
            if(m_spawnedBall == null)
            {
                m_spawnedBall = Instantiate(m_ballPrefab, m_ARCamera.transform.position, m_ARCamera.transform.rotation);
            } else
            {
                m_spawnedBall.transform.position = m_ARCamera.transform.position;
                m_spawnedBall.GetComponent<Rigidbody>().velocity = (m_ARCamera.transform.forward + Vector3.up / 2).normalized * 4.5f;
            }
        } else
        {
            if (m_RayCastManager.Raycast(touchPosition, rayHits, TrackableType.PlaneWithinPolygon))
            {
                Pose hitPose = rayHits[0].pose;

                if (m_spawnedHoop == null)
                {
                    m_spawnedHoop = Instantiate(m_hoopPrefab, Vector3.zero, hitPose.rotation, m_content.transform);
                } else
                {
                    m_SessionOrigin.MakeContentAppearAt(m_content.transform, hitPose.position, m_hoopRotation);
                }
            }
        }


    }

    public void OnHoopReadyButtonPress()
    {
        if (!m_hoopPlaced)
        {
            m_hoopPlaced = true;
            m_sliders.SetActive(false);
        } else
        {
            m_hoopPlaced = false;
            m_sliders.SetActive(true);
        }
    }
}
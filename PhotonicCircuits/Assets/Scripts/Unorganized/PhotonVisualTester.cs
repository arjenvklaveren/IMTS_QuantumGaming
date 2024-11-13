using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//COMPLETE TEST SCRIPT, CODE QUALITY SHOULD NOT BE TAKEN SERIOUSELY OR TAKEN AS A REFERENCE
public class PhotonVisualTester : MonoBehaviour
{
    [SerializeField] PhotonManager photonManager;
    [SerializeField] GameObject photonCopy;

    [SerializeField] Image boundsImage;
    [SerializeField] Image phaseImage;
    [SerializeField] Vector2Int boundsArea = new Vector2Int(500,500);
    private List<Image> photonVisuals = new List<Image>();

    int selectedPhotonIndex = 0;
    bool movePhotons = false;

    void Start()
    {
        InitializePhotons();
    }

    void InitializePhotons()
    {
        List<Photon> initialPhotons = new List<Photon>()
        {
            new Photon(Random.Range(380.0f, 750.0f), 0.5f, 0, 0, new Vector2(Random.Range(-boundsArea.x/2, boundsArea.x/2), Random.Range(-boundsArea.y/2, boundsArea.y/2)), new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f,1.0f)).normalized),
            new Photon(Random.Range(380.0f, 750.0f), 0.5f, 0, 0, new Vector2(Random.Range(-boundsArea.x/2, boundsArea.x/2), Random.Range(-boundsArea.y/2, boundsArea.y/2)), new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f,1.0f)).normalized),
            new Photon(Random.Range(380.0f, 750.0f), 0.5f, 0, 0, new Vector2(Random.Range(-boundsArea.x/2, boundsArea.x/2), Random.Range(-boundsArea.y/2, boundsArea.y/2)), new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f,1.0f)).normalized),
            new Photon(Random.Range(380.0f, 750.0f), 0.5f, 0, 0, new Vector2(Random.Range(-boundsArea.x/2, boundsArea.x/2), Random.Range(-boundsArea.y/2, boundsArea.y/2)), new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f,1.0f)).normalized),
            new Photon(Random.Range(380.0f, 750.0f), 0.5f, 0, 0, new Vector2(Random.Range(-boundsArea.x/2, boundsArea.x/2), Random.Range(-boundsArea.y/2, boundsArea.y/2)), new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f,1.0f)).normalized),
            new Photon(Random.Range(380.0f, 750.0f), 0.5f, 0, 0, new Vector2(Random.Range(-boundsArea.x/2, boundsArea.x/2), Random.Range(-boundsArea.y/2, boundsArea.y/2)), new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f,1.0f)).normalized),
            new Photon(Random.Range(380.0f, 750.0f), 0.5f, 0, 0, new Vector2(Random.Range(-boundsArea.x/2, boundsArea.x/2), Random.Range(-boundsArea.y/2, boundsArea.y/2)), new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f,1.0f)).normalized),
            new Photon(Random.Range(380.0f, 750.0f), 0.5f, 0, 0, new Vector2(Random.Range(-boundsArea.x/2, boundsArea.x/2), Random.Range(-boundsArea.y/2, boundsArea.y/2)), new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f,1.0f)).normalized),
            new Photon(Random.Range(380.0f, 750.0f), 0.5f, 0, 0, new Vector2(650, -250), new Vector2(-1, -1).normalized),

        };

        for(int i = 0; i < initialPhotons.Count; i++)
        {
            photonManager.AddNewPhoton(initialPhotons[i]);
        }

        Photon entanglePhoton1 = new Photon(Random.Range(380.0f, 750.0f), 0.5f, 0, 0, new Vector2(Random.Range(-boundsArea.x / 2, boundsArea.x / 2), Random.Range(-boundsArea.y / 2, boundsArea.y / 2)), new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)).normalized);
        Photon entanglePhoton2 = new Photon(Random.Range(380.0f, 750.0f), 0.5f, 0, 0, new Vector2(Random.Range(-boundsArea.x / 2, boundsArea.x / 2), Random.Range(-boundsArea.y / 2, boundsArea.y / 2)), new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)).normalized);
        //photonManager.AddEntangledPhotonPair(entanglePhoton1, entanglePhoton2);

        SyncPhotons();
    }

    private void OnValidate()
    {
        UpdateBounds();
    }

    void FixedUpdate()
    {
        UpdatePhotons();
    }
    private void Update()
    {
        CheckInput();
        Debug.Log("SELECTED PHOTON PHASE:" + photonManager.GetAllPhotons()[selectedPhotonIndex].GetPhase() + " RADIANS");
    }

    void CreatePhotonObjectFromPhoton(Photon photon)
    {
        Image newPhotonVisual = Instantiate(photonCopy, transform).GetComponent<Image>();
        newPhotonVisual.transform.localPosition = photon.GetPosition();
        Color photonColor = photon.GetColor();
        newPhotonVisual.GetComponent<Image>().color = photonColor;
        newPhotonVisual.gameObject.SetActive(true);
        photonVisuals.Add(newPhotonVisual);
    }

    void PrintPhotonData()
    {
        if (photonManager.GetAllPhotons().Count == 0) return;
        string debugString = "PHOTON STRUCTURE (WAVELENGTH AS VALUE): \n";
        foreach (List<Photon> photonList in photonManager.GetAllPhotonsRaw())
        {
            debugString += "[";
            foreach (Photon photon in photonList)
            {
                debugString += "{" + (int)photon.GetWaveLength() + "}";
            }
            debugString += "]\n";
        }
        Debug.Log(debugString);
    }
    
    void SyncPhotons()
    {
        for(int i = 0; i < photonVisuals.Count; i++)
        {
            Destroy(photonVisuals[i].gameObject);
        }
        photonVisuals.Clear();
        for (int i = 0; i < photonManager.GetAllPhotons().Count; i++)
        {
            CreatePhotonObjectFromPhoton(photonManager.GetAllPhotons()[i]);
        }
        PrintPhotonData();
    }

    void UpdatePhotons()
    {
        if (photonManager.GetAllPhotons().Count == 0) return;
        int? selectI = photonManager.FindPhotonListIndex(photonManager.GetAllPhotons()[selectedPhotonIndex]);
        for (int i = 0; i < photonManager.GetAllPhotons().Count; i++)
        {
            if (selectI.HasValue)
            {
                int? currentI = photonManager.FindPhotonListIndex(photonManager.GetAllPhotons()[i]);
                if(currentI.HasValue) 
                {
                    if(currentI.Value == selectI.Value)
                    {
                        photonVisuals[i].GetComponent<Outline>().enabled = true;
                        if (i != selectedPhotonIndex) photonVisuals[i].GetComponent<Outline>().effectColor = Color.grey;
                        else photonVisuals[i].GetComponent<Outline>().effectColor = Color.black;
                    }
                    else photonVisuals[i].GetComponent<Outline>().enabled = false;
                }
            }
            if (photonManager.GetAllPhotons()[i].GetPosition().x > 0) photonManager.GetAllPhotons()[i].SetPhase(Mathf.PI);
            else photonManager.GetAllPhotons()[i].SetPhase(0);
            photonVisuals[i].transform.localPosition = photonManager.GetAllPhotons()[i].GetPosition();
            if(movePhotons) photonManager.GetAllPhotons()[i].Move();
            CheckPhotonBounds(i);
        }
    }
    void CheckPhotonBounds(int index)
    {
        float posX = photonVisuals[index].transform.localPosition.x;
        float posY = photonVisuals[index].transform.localPosition.y;
        float boundsX = boundsImage.transform.localPosition.x;
        float boundsY = boundsImage.transform.localPosition.x;
        float boundsWidth = boundsImage.rectTransform.sizeDelta.x / 2;
        float boundsHeight = boundsImage.rectTransform.sizeDelta.y / 2;
        if (Mathf.Abs(posX) > Mathf.Abs(boundsX + boundsWidth) || Mathf.Abs(posY) > Mathf.Abs(boundsY + boundsHeight)) 
        {
            Vector2 normalVec = Vector2.zero;
            if (posX > boundsX + boundsWidth)
            {
                normalVec = Vector2.left;
            }
            if (posX < boundsX - boundsWidth)
            {
                normalVec = Vector2.right;
            }
            if (posY > boundsY + boundsHeight)
            {
                normalVec = Vector2.down;
            }
            if (posY < boundsY - boundsHeight)
            {
                normalVec = Vector2.up;
            }
            Vector2 angleVec = Vector2.Reflect(photonManager.GetAllPhotons()[index].GetPropagation(), normalVec);
            float angle = Vector2.Angle(Vector2.up, angleVec);
            Debug.Log(angle);
            photonManager.GetAllPhotons()[index].SetPropagation(angleVec);
            photonManager.GetAllPhotons()[index].SetPosition(photonManager.GetAllPhotons()[index].GetPosition() + photonManager.GetAllPhotons()[index].GetPropagation() * 5);
        }
    }
    void UpdateBounds()
    {
        boundsImage.rectTransform.sizeDelta = boundsArea;
        phaseImage.rectTransform.sizeDelta = new Vector2(boundsArea.x / 2, boundsArea.y);
    }

    void CheckInput()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            selectedPhotonIndex--;
            if (selectedPhotonIndex < 0) selectedPhotonIndex = photonManager.GetAllPhotons().Count - 1;
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            selectedPhotonIndex++;
            if (selectedPhotonIndex > photonManager.GetAllPhotons().Count - 1) selectedPhotonIndex = 0;
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            Photon p = photonManager.GetAllPhotons()[selectedPhotonIndex];
            Photon newP1 = p.Clone();
            Photon newP2 = p.Clone();
            Vector2 randVec = new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)) * 50;
            newP1.SetPosition(p.GetPosition() + randVec);
            newP2.SetPosition(p.GetPosition() - randVec);
            newP1.SetPropagation(new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)).normalized);
            newP2.SetPropagation(new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)).normalized);
            photonManager.ReplacePhoton(p, new List<Photon>() { newP1, newP2 });
            SyncPhotons();
        }
        if(Input.GetKeyDown(KeyCode.D))
        {
            photonManager.RemovePhoton(photonManager.GetAllPhotons()[selectedPhotonIndex], true);
            SyncPhotons();
            selectedPhotonIndex = 0;
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            movePhotons = !movePhotons;
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            photonManager.GetAllPhotons()[selectedPhotonIndex].SetPropagationByAngle(90);
        }
    }
}

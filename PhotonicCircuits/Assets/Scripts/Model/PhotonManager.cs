using Game.Data;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PhotonManager : MonoBehaviour
{
    [SerializeField] List<List<Photon>> photons = new List<List<Photon>>();
    List<List<int>> entanglementIndexes = new List<List<int>>();

    //Photon addition
    public void AddNewPhoton(Photon photon)
    {
        photons.Add(new List<Photon>() { photon }); 
    }
    public void AddEntangledPhotonPair(Photon photon1, Photon photon2)
    {
        AddEntangledPhotons(new List<Photon>() { photon1, photon2 });
    }
    public void AddEntangledPhotons(List<Photon> photons)
    {
        if (photons.Count < 2) return;
        List<int> newEntanglementList = new List<int>();
        foreach(Photon photon in photons)
        {
            int photonListIndex = this.photons.Count;
            this.photons.Add(new List<Photon> { photon });
            newEntanglementList.Add(photonListIndex);
        }
        entanglementIndexes.Add(newEntanglementList);
    }

    //Photon removing
    public void RemovePhoton(Photon photon, bool isMeasure, bool undestructiveMeasure = false)
    {
        Vector2Int? photonIndex = FindPhotonIndex2D(photon);
        if (!photonIndex.HasValue) return;

        if (isMeasure && !undestructiveMeasure)
        {
            photons.RemoveAt(photonIndex.Value.x);
            ShiftEntanglementIndexes(photonIndex.Value.x);

            List<Photon> entangled = GetPhotonEntanglements(photon);
            if(entangled.Count > 0)
            {
                //TODO Entanglement measurement trigger logic
                for(int i = 0; i < entangled.Count; i++)
                {

                }
            }
        }
        else
        {
            photons[photonIndex.Value.x].RemoveAt(photonIndex.Value.y);
            if (photons[photonIndex.Value.x].Count == 0)
            {
                photons.RemoveAt(photonIndex.Value.x);
                ShiftEntanglementIndexes(photonIndex.Value.x);
            }
        }
    }

    //Photon replacing
    public void ReplacePhoton(Photon photon, List<Photon> replacements)
    {
        Vector2Int? photonIndex = FindPhotonIndex2D(photon);
        if (!photonIndex.HasValue) return;
        photons[photonIndex.Value.x].RemoveAt(photonIndex.Value.y);
        photons[photonIndex.Value.x].InsertRange(photonIndex.Value.y, replacements);
    }

    //Photon receiving
    public List<Photon> GetAllPhotons()
    {
        List<Photon> fullArray = new List<Photon>();
        for (int i = 0; i < photons.Count; i++)
        {
            fullArray.AddRange(photons[i]);
        }
        return fullArray;
    }
    public List<List<Photon>> GetAllPhotonsRaw() { return photons; }

    public List<Photon> GetPhotonSuperpositions(Photon photon)
    {
        int? listIndex = FindPhotonListIndex(photon);
        if (listIndex.HasValue) return photons[listIndex.Value];
        return new List<Photon>();
    }
    public List<Photon> GetPhotonEntanglements(Photon photon, bool includeSelf = true)
    {
        List<Photon> outPhotonList = new List<Photon>();
        int? photonIndex = FindPhotonListIndex(photon);
        int? entangleIndex = null;

        for(int i = 0; i < entanglementIndexes.Count; i++)
        {
            for(int j = 0; j < entanglementIndexes[i].Count; j++)
            {
                if (entanglementIndexes[i][j] == photonIndex) { entangleIndex = i; break; }
            }
        }
        if(entangleIndex.HasValue)
        {
            for(int i = 0; i < entanglementIndexes[entangleIndex.Value].Count; i++)
            {
                if (!includeSelf && entanglementIndexes[entangleIndex.Value][i] == photonIndex) continue;
                outPhotonList.AddRange(photons[entanglementIndexes[entangleIndex.Value][i]]);
            }
        }
        return outPhotonList;
    }
    public List<Photon> GetAllLinkedOfPhoton(Photon photon)
    {
        List<Photon> outPhotonList = GetPhotonSuperpositions(photon);
        outPhotonList.AddRange(GetPhotonEntanglements(photon, false));
        return outPhotonList;
    }
    public List<Photon> GetPhotonsByIndex(int index)
    {
        return photons[index];
    }

    public int GetPhotonListCount() { return photons.Count; }

    //Photon finding
    public Photon FindPhoton(Photon photon)
    {
        foreach (List<Photon> photonList in photons)
        {
            foreach (Photon p in photonList)
            {
                if (photon == p) return p;
            }
        }
        return null;
    }
    public int? FindPhotonListIndex(Photon photon)
    {
        Vector2Int? index2D = FindPhotonIndex2D(photon);
        if (index2D.HasValue) return index2D.Value.x;
        else return null;
    }
    public Vector2Int? FindPhotonIndex2D(Photon photon)
    {
        for (int i = 0; i < photons.Count; i++)
        {
            for (int j = 0; j < photons[i].Count; j++)
            {
                if (photons[i][j] == photon) return new Vector2Int(i,j);
            }
        }
        return null;
    }
    void ShiftEntanglementIndexes(int cutoffIndex)
    {
        for(int i = 0; i < entanglementIndexes.Count; i++)
        {
            for(int j = 0; j < entanglementIndexes[i].Count; j++)
            {
                if (entanglementIndexes[i][j] > cutoffIndex)
                {
                    entanglementIndexes[i][j]--;
                }
            }
        }
    }

    Color GetRelativePhotonColor(Photon photon)
    {
        int? index = FindPhotonListIndex(photon);
        float opacity = 1.0f;
        return Color.black;
    }
}

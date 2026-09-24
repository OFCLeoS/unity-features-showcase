#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using Unity.AI.Navigation;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Translates DrawIO blueprints to actual game geometry (If Set-Up Correctly)
/// </summary>
public class DrawIOTranslator : MonoBehaviour
{
    public string fileName;
    public MapRandomSector[] availableRandomSectors;

    void Awake()
    {
        Destroy(gameObject);
    }

    string GetDataValueInLine(string line, string dataName)
    {
        string information = "";
        int dataStartIndex = line.IndexOf(dataName);
        if (dataStartIndex != -1)
        {
            dataStartIndex += dataName.Length;
            // We move fowards until we find something that is not an = or a "
            while (line[dataStartIndex].Equals('=') || line[dataStartIndex].Equals('"'))
            {
                dataStartIndex++;
            }
            int dataEndIndex = dataStartIndex;
            // We move fowards until we find a " or a ;
            while (!line[dataEndIndex].Equals('"') && !line[dataEndIndex].Equals(';'))
            {
                dataEndIndex++;
            }
            information = line.Substring(dataStartIndex, dataEndIndex - dataStartIndex);
        }
        return information;
    }

    [ContextMenu("Load Draw.io File")]
    public void LoadFloorMap()
    {
        string filePath = Application.streamingAssetsPath + "/" + fileName + ".drawio";
        if (!File.Exists(filePath))
        {
            Debug.LogError("ERROR! No file at: " + filePath);
            return;
        }

        string[] fileLines = File.ReadAllLines(filePath);

        MapLayout layoutParent = new GameObject().AddComponent<MapLayout>();
        NavMeshSurface navMeshSurface = layoutParent.AddComponent<NavMeshSurface>();
        navMeshSurface.useGeometry = UnityEngine.AI.NavMeshCollectGeometry.PhysicsColliders;
        layoutParent.gameObject.name = fileName;
        layoutParent.transform.position = Vector3.zero;
        layoutParent.transform.rotation = Quaternion.identity;
        layoutParent.transform.localScale = Vector3.one;

        Dictionary<string, MapRandomSector> idSectorPairs = new Dictionary<string, MapRandomSector>();
        Dictionary<string, List<Vector2>> idDoorPointsDeltasPairs = new Dictionary<string, List<Vector2>>();
        // Used for door points that are not in delta form
        Dictionary<string, List<Vector2>> idDoorPointsLineEndPairs = new Dictionary<string, List<Vector2>>();
        for (int i = 0; i < fileLines.Length; i++)
        {
            string sectorTypeValue = GetDataValueInLine(fileLines[i], "Sector_Type");
            if (!sectorTypeValue.Equals(""))
            {
                // ID should be in the same line as sector
                string sectorTypeIDValue = GetDataValueInLine(fileLines[i], " id");
                float rotation = 0;
                bool foundAttributes = false;

                // We find attempt to find the attributes of
                while (!foundAttributes && i < fileLines.Length)
                {
                    // Rotation is written before other attributes in draw.io
                    string rotationValue = GetDataValueInLine(fileLines[i], "rotation");
                    if (!rotationValue.Equals(""))
                    {
                        rotation = Mathf.Abs(float.Parse(rotationValue));
                    }

                    string lengthValue = GetDataValueInLine(fileLines[i], "height");
                    // Length and all other values are on the same line
                    if (!lengthValue.Equals(""))
                    {
                        foundAttributes = true;
                        float length = int.Parse(lengthValue) / 10.0f;
                        float width = int.Parse(GetDataValueInLine(fileLines[i], "width")) / 10.0f;

                        if (rotation != 0 && rotation != 90)
                        {
                            Debug.LogWarning($"An invalid rotation was detected on a sector of type \"{sectorTypeValue}\". It will not be created.");
                            break;
                        }

                        double x = 0;
                        double z = 0;

                        string xText = GetDataValueInLine(fileLines[i], " x");
                        if (!xText.Equals(""))
                        {
                            // Rounding to 1 decimal seems to make everything be connected better
                            x = Math.Round(double.Parse(xText) / 10.0f, 1);
                        }
                        string zText = GetDataValueInLine(fileLines[i], " y");
                        if (!zText.Equals(""))
                        {
                            // Rounding to 1 decimal seems to make everything be connected better
                            z = Math.Round(-double.Parse(zText) / 10.0f, 1);
                        }
                        if (rotation == 90)
                        {
                            x += ((width - length) / 2.0f) + length;
                            z += (width - length) / 2.0f;
                        }

                        Vector2 dimensions = new Vector2(width, length);
                        MapRandomSector matchingRandomSector = null;

                        foreach (MapRandomSector randomSector in availableRandomSectors)
                        {
                            if (randomSector.Dimensions == dimensions)
                            {
                                matchingRandomSector = Instantiate(randomSector, new Vector3((float)x, 0, (float)z), Quaternion.Euler(-90, rotation, 0), layoutParent.transform);
                                layoutParent.AddMapSection(matchingRandomSector);
                                idSectorPairs.Add(sectorTypeIDValue, matchingRandomSector);
                            }
                        }

                        if (matchingRandomSector == null)
                        {
                            Debug.LogWarning($"A matching Random Sector of type \"{sectorTypeValue}\" and of dimensions {dimensions} was not found. It will not be created.");
                            break;
                        }
                    }
                    i++;
                }
                if (!foundAttributes)
                {
                    Destroy(layoutParent);
                    Debug.LogError($"The attributes for a Sector of type \"{sectorTypeValue}\" was not found. Terminating Translation...");
                    return;
                }
            }
            else
            {
                // For Doors
                string sourceIDValue = GetDataValueInLine(fileLines[i], " source");
                if (!sourceIDValue.Equals(""))
                {
                    string dXValue = GetDataValueInLine(fileLines[i], "exitX");
                    if (!dXValue.Equals(""))
                    {
                        float dX = float.Parse(dXValue);
                        float dY = float.Parse(GetDataValueInLine(fileLines[i], "exitY"));
                        List<Vector2> sourceDoorPointDeltas;
                        if (!idDoorPointsDeltasPairs.TryGetValue(sourceIDValue, out sourceDoorPointDeltas))
                        {
                            sourceDoorPointDeltas = new List<Vector2>();
                            idDoorPointsDeltasPairs.Add(sourceIDValue, sourceDoorPointDeltas);
                        }
                        sourceDoorPointDeltas.Add(new Vector2(dX, dY));
                    }
                    else
                    {
                        // If " source" was detected, but not exit, it means that the point will be in either 0.5 or 1 of x or y
                        bool foundPoints = false;
                        while (!foundPoints && i < fileLines.Length)
                        {
                            float x = 0;
                            float y = 0;

                            string xText = GetDataValueInLine(fileLines[i], " x");
                            // Delta Points will never be (0,0), its sufficient that only one point is found
                            if (!xText.Equals(""))
                            {
                                foundPoints = true;
                                x = float.Parse(xText) / 10.0f;
                            }
                            string yText = GetDataValueInLine(fileLines[i], " y");
                            if (!yText.Equals(""))
                            {
                                foundPoints = true;
                                y = -float.Parse(yText) / 10.0f;
                            }
                            if (!foundPoints)
                            {
                                i++;
                                continue;
                            }
                            List<Vector2> sectorDoorPointsLineEnd;
                            if (!idDoorPointsLineEndPairs.TryGetValue(sourceIDValue, out sectorDoorPointsLineEnd))
                            {
                                sectorDoorPointsLineEnd = new List<Vector2>();
                                idDoorPointsLineEndPairs.Add(sourceIDValue, sectorDoorPointsLineEnd);
                            }
                            Debug.Log($"{sourceIDValue}: Point {x},{y} added");
                            sectorDoorPointsLineEnd.Add(new Vector2(x, y));
                            i++;
                        }
                    }
                }
            }
        }
        // DOOR CONNECTION

        foreach (KeyValuePair<string, MapRandomSector> idSectorPair in idSectorPairs)
        {
            // Doors with deltas
            List<Vector2> sourceDoorPointDeltas;
            if (idDoorPointsDeltasPairs.TryGetValue(idSectorPair.Key, out sourceDoorPointDeltas))
            {
                idSectorPair.Value.SetActiveDoorPoints(sourceDoorPointDeltas);
            }
            else
            {
                Debug.LogWarning($"Sector {idSectorPair.Value.transform.name} does not have any room connections. Ignore if intended.");
            }

            // Doors without deltas (we will now convert)
            List<Vector2> sectorDoorPointsLineEnd;
            if (idDoorPointsLineEndPairs.TryGetValue(idSectorPair.Key, out sectorDoorPointsLineEnd))
            {
                Debug.Log($"COUNT FOR {idSectorPair.Value.transform.name}: {sectorDoorPointsLineEnd.Count}");
                for (int i = 0; i < sectorDoorPointsLineEnd.Count; i++)
                {
                    // We will check in which axis the 0.5 is on, it will be the axis that whose point difference is smaller
                    // The data collected is the line's end point
                    float xPointMinusHalfWidth = sectorDoorPointsLineEnd[i].x - (idSectorPair.Value.Dimensions.x / 2);
                    float yPointMinusHalfLength = sectorDoorPointsLineEnd[i].y + (idSectorPair.Value.Dimensions.y / 2);

                    float dX = xPointMinusHalfWidth - idSectorPair.Value.transform.position.x;
                    float dY = yPointMinusHalfLength - idSectorPair.Value.transform.position.z;
                    if (Mathf.Abs(dX) > Mathf.Abs(dY))
                    {
                        // 0.5 Will be on x axis
                        float xPosition = xPointMinusHalfWidth < idSectorPair.Value.transform.position.x ? 1 : 0;
                        Debug.Log($"Y FOR {idSectorPair.Value.transform.name}: dx: {xPosition}, dy: 0.5 (Was {sectorDoorPointsLineEnd[i]})");
                        sectorDoorPointsLineEnd[i] = new Vector2(xPosition, 0.5f);
                    }
                    else
                    {
                        // 0.5 Will be on y axis
                        float yPosition = yPointMinusHalfLength < idSectorPair.Value.transform.position.z ? 1 : 0;
                        Debug.Log($"X FOR {idSectorPair.Value.transform.name}: dx: 0.5, dy: {yPosition} (Was {sectorDoorPointsLineEnd[i]})");
                        sectorDoorPointsLineEnd[i] = new Vector2(0.5f, yPosition);
                    }
                }
                idSectorPair.Value.SetActiveDoorPoints(sectorDoorPointsLineEnd);
            }
        }
        Debug.Log("Translation Successful!");
    }
}
#endif
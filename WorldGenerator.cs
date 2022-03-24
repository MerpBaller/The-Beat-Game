using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.AI;
using System.Xml;
using System.Linq;
using PathCreation;

public class WorldGenerator : MonoBehaviour
{
    //100x100: 0:03
    //200x200: 0:12
    //300x300: 1:18
    public int size;
    public int divisions;
    private int totalSizeX;
    private int totalSizeY;
    public bool[,] isRoad;
    public Tile[] roadTiles;
    public Tile[] buildingTiles;
    public GameObject[] wayPoints;
    public Tilemap grid;
    public GameObject WaypointManager;
    private RoadDivision[] roadDivisions;
    private int xIncreaseAmountMax = 0;
    private List<GameObject> NSRoadWaypoints = new List<GameObject>();
    private List<GameObject> EWRoadWaypoints = new List<GameObject>();
    private List<GameObject> InstersectionWaypoints = new List<GameObject>();

    class RoadDivision
    {
        public int startX;
        public int startY;
        public int sizeX;
        public int sizeY;
        public int creationLengthX;
        public int creationLengthY;

        public RoadDivision(int a, int b, int c, int d, int e, int f)
        {
            this.startX = a;
            this.startY = b;
            this.sizeX = c;
            this.sizeY = d;
            this.creationLengthX = e;
            this.creationLengthY = f;
        }

        public void PrintAll()
        {
            Debug.Log(startX + " " +  startY + " " + sizeX + " " + sizeY + " " + creationLengthX + " " + creationLengthY);
        }
        
    }

    struct Coordinates
    {
        public int x;
        public int y;
    }

    void Start()
    {
        totalSizeX = size;
        totalSizeY = size;
        //divisions = UnityEngine.Random.Range(8, 20);
        //divisions should be a number that can be taken the square root of in order to make a nice looking city.
        CreateNewDivisions(divisions);
        CreateDivisionConnections();

        for (int i = 0; i < totalSizeX; i++)
        {
            for (int j = 0; j < totalSizeY; j++)
            {
                if (isRoad[i, j] == true)
                {
                    GenerateRoad(i, j);
                }
                else
                {
                    isRoad[i, j] = false;
                }
            }
        }
        Debug.Log("Got here");
        GenerateWaypointPaths();
    }

    

    private void CreateNewDivisions(int divisions)
    {
        int tempStartX = 0;
        int tempStartY = 0;
        int tempSizeX = 0;
        int tempSizeY = 0;
        int tempCreationLengthX;
        int tempCreationLengthY;
        int tempVar;
        int currentDivision = 0;
        int yIncreaseAmount = 0;
        int slicesPerSide;
        int xIncreaseAmount;
        int[] creationLengthValues = { 6, 8, 10, 12, 14, 16 };

        roadDivisions = new RoadDivision[divisions];

        slicesPerSide = Mathf.RoundToInt(Mathf.Sqrt(divisions));

        yIncreaseAmount = totalSizeY / slicesPerSide;
        xIncreaseAmount = totalSizeX / slicesPerSide;

        while (yIncreaseAmount % 2 != 0)
        {
            yIncreaseAmount++;
        }
        while (xIncreaseAmount % 2 != 0)
        {
            xIncreaseAmount++;
        }

        tempSizeX = xIncreaseAmount;
        //Can merge the while loops after making rows and stuff

        Debug.Log(xIncreaseAmount + " " + yIncreaseAmount);
        while (currentDivision < divisions)
        {
            for (int i = 0; i < slicesPerSide; i++)
            {
                tempVar = UnityEngine.Random.Range(1, 3);
                if (tempVar == 1)
                {
                    tempCreationLengthX = UnityEngine.Random.Range(0, 2);
                    tempCreationLengthY = UnityEngine.Random.Range(1, 6);

                    tempCreationLengthX = creationLengthValues[tempCreationLengthX];
                    tempCreationLengthY = creationLengthValues[tempCreationLengthY];
                }
                else
                {
                    tempCreationLengthY = UnityEngine.Random.Range(0, 2);
                    tempCreationLengthX = UnityEngine.Random.Range(1, 6);

                    tempCreationLengthX = creationLengthValues[tempCreationLengthX];
                    tempCreationLengthY = creationLengthValues[tempCreationLengthY];
                }

                tempSizeY += yIncreaseAmount + 4;



                while (tempSizeX % tempCreationLengthX != 0)
                {
                    tempSizeX += 1;
                }
                while (tempSizeY % tempCreationLengthY != 0)
                {
                    tempSizeY += 1;
                }

                while (tempSizeY % 2 != 0)
                {
                    tempSizeY += 1;
                }

                while (tempSizeY > totalSizeY)
                {
                    totalSizeY += 1;
                }
                while (tempSizeX > totalSizeX)
                {
                    totalSizeX += 1;
                }

                roadDivisions[currentDivision] = new RoadDivision(tempStartX, tempStartY, tempSizeX, tempSizeY, tempCreationLengthX, tempCreationLengthY);

                roadDivisions[currentDivision].PrintAll();

                tempStartY = tempSizeY + 4;
                currentDivision++;
            }

            tempVar = 0;
            for (int i = currentDivision - slicesPerSide; i < currentDivision; i++)
            {
                if (roadDivisions[i].sizeX >= tempVar)
                {
                    tempVar = roadDivisions[i].sizeX;
                }
            }

            tempSizeY = 0;
            tempStartX = tempVar + 6;
            totalSizeX += 6;
            tempSizeX = xIncreaseAmount + tempStartX;
            tempStartY = 0;
        }

        isRoad = new bool[totalSizeX, totalSizeY];
        Debug.Log(totalSizeX + " " + totalSizeY);

        xIncreaseAmountMax = xIncreaseAmount;
        xIncreaseAmountMax++;

        for (int k = 0; k < divisions; k++)
        {
            //Generate Truth Table
            for (int i = roadDivisions[k].startX; i < roadDivisions[k].sizeX; i += roadDivisions[k].creationLengthX - 1)
            {
                for (int j = roadDivisions[k].startY; j < roadDivisions[k].sizeY; j += roadDivisions[k].creationLengthY - 1)
                {
                    Vector3Int tempStart = new Vector3Int(i, j, 0);
                    CreateSquare(tempStart, roadDivisions[k]);
                }
            }
        }
    }

    private void CreateDivisionConnections()
    {
        string tempString = "";
        
        for (int i = 0; i < totalSizeX; i++)
        {
            for (int j = 0; j < totalSizeY; j++)
            {
                if (isRoad[i, j] == true)
                {
                    tempString = "";

                    try
                    {
                        if (isRoad[i + 1, j] == true)
                        {
                            tempString += "1";
                        }
                        else
                        {
                            tempString += "0";
                        }
                    }
                    catch (IndexOutOfRangeException)
                    {
                        tempString += "0";
                    }

                    try
                    {
                        if (isRoad[i + 1, j - 1] == true)
                        {
                            tempString += "1";
                        }
                        else
                        {
                            tempString += "0";
                        }
                    }
                    catch (IndexOutOfRangeException)
                    {
                        tempString += "0";
                    }

                    try
                    {
                        if (isRoad[i, j - 1] == true)
                        {
                            tempString += "1";
                        }
                        else
                        {
                            tempString += "0";
                        }
                    }
                    catch (IndexOutOfRangeException)
                    {
                        tempString += "0";
                    }

                    try
                    {
                        if (isRoad[i - 1, j - 1] == true)
                        {
                            tempString += "1";
                        }
                        else
                        {
                            tempString += "0";
                        }
                    }
                    catch (IndexOutOfRangeException)
                    {
                        tempString += "0";
                    }

                    try
                    {

                        if (isRoad[i - 1, j] == true)
                        {
                            tempString += "1";
                        }
                        else
                        {
                            tempString += "0";
                        }
                    }
                    catch (IndexOutOfRangeException)
                    {
                        tempString += "0";
                    }

                    try
                    {
                        if (isRoad[i - 1, j + 1] == true)
                        {
                            tempString += "1";
                        }
                        else
                        {
                            tempString += "0";
                        }
                    }
                    catch (IndexOutOfRangeException)
                    {
                        tempString += "0";
                    }


                    try
                    {
                        if (isRoad[i, j + 1] == true)
                        {
                            tempString += "1";
                        }
                        else
                        {
                            tempString += "0";
                        }
                    }
                    catch (IndexOutOfRangeException)
                    {
                        tempString += "0";
                    }

                    try
                    {
                        if (isRoad[i + 1, j + 1] == true)
                        {
                            tempString += "1";
                        }
                        else
                        {
                            tempString += "0";
                        }
                    }
                    catch (IndexOutOfRangeException)
                    {
                        tempString += "0";
                    }

                    switch (tempString)
                    {
                        case "10101000":
                            //grid.SetTile(tempVector, roadTiles[1]);
                            TestDirection(0, 1, i, j);
                            break;
                        case "10100010":
                            //grid.SetTile(tempVector, roadTiles[3]);
                            TestDirection(-1, 0, i, j);
                            break;
                        case "10001010":
                            TestDirection(0, -1, i, j);
                            break;
                        case "00101010":
                            TestDirection(1, 0, i, j);
                            break;
                        case "10100000":
                            TestDirection(-1, 1, i, j);
                            break;
                        case "10000010":
                            TestDirection(-1, -1, i, j);
                            break;
                        case "00101000":
                            TestDirection(1, 1, i, j);
                            break;
                        case "00001010":
                            TestDirection(1, -1, i, j);
                            break;
                    }
                }
            }
        }
    }

    private void TestDirection(int directionX, int directionY, int i, int j)
    {
        List<Coordinates> coordinatesList = new List<Coordinates>();
        Coordinates tempCoordinate;
        int xIncreaseAmountMin = 5;
        
        switch (directionX)
        {
            case 1:
                for(int temp = i + 1; temp <= totalSizeX; temp++)
                {
                    try
                    {
                        if (isRoad[temp, j] == true)
                        {
                            if (coordinatesList.Count >= xIncreaseAmountMin && coordinatesList.Count <= xIncreaseAmountMax)
                            {
                                for (int innerTemp = 0; innerTemp < coordinatesList.Count; innerTemp++)
                                {
                                    isRoad[coordinatesList[innerTemp].x, coordinatesList[innerTemp].y] = true;
                                }
                                break;
                            }
                            else
                            {
                                break;
                            }                            
                        }
                        else
                        {
                            tempCoordinate.x = temp;
                            tempCoordinate.y = j;
                            coordinatesList.Add(tempCoordinate);
                        }
                    }
                    catch (IndexOutOfRangeException)
                    {
                        break;
                    }
                }
                coordinatesList.Clear();
                break;
            case -1:
                for (int temp = i - 1; temp <= 0; temp--)
                {
                    try
                    {
                        if (isRoad[temp, j] == true)
                        {
                            if (coordinatesList.Count >= xIncreaseAmountMin && coordinatesList.Count <= xIncreaseAmountMax)
                            {
                                for (int innerTemp = 0; innerTemp < coordinatesList.Count; innerTemp++)
                                {
                                    isRoad[coordinatesList[innerTemp].x, coordinatesList[innerTemp].y] = true;
                                }
                                break;
                            }
                            else
                            {
                                break;
                            }
                           
                        }
                        else
                        {
                            tempCoordinate.x = temp;
                            tempCoordinate.y = j;
                            coordinatesList.Add(tempCoordinate);
                        }
                    }
                    catch (IndexOutOfRangeException)
                    {
                        break;
                    }
                }
                coordinatesList.Clear();
                break;
            default:
                break;
        }

        switch (directionY)
        {
            case 1:
                for (int temp = j + 1; temp <= totalSizeY; temp++)
                {
                    try
                    {
                        if (isRoad[i, temp] == true)
                        {
                            if (coordinatesList.Count >= xIncreaseAmountMin && coordinatesList.Count <= xIncreaseAmountMax)
                            {
                                for (int innerTemp = 0; innerTemp < coordinatesList.Count; innerTemp++)
                                {
                                    isRoad[coordinatesList[innerTemp].x, coordinatesList[innerTemp].y] = true;
                                }
                                break;
                            }
                            else
                            {
                                break;
                            }
                                
                        }
                        else
                        {
                            tempCoordinate.x = i;
                            tempCoordinate.y = temp;
                            coordinatesList.Add(tempCoordinate);
                        }
                    }
                    catch (IndexOutOfRangeException)
                    {
                        break;
                    }
                }
                coordinatesList.Clear();
                break;
            case -1:
                for (int temp = j - 1; temp <= 0; temp--)
                {
                    try
                    {
                        if (isRoad[i, temp] == true)
                        {
                            if (coordinatesList.Count >= xIncreaseAmountMin && coordinatesList.Count <= xIncreaseAmountMax)
                            {
                                for (int innerTemp = 0; innerTemp < coordinatesList.Count; innerTemp++)
                                {
                                    isRoad[coordinatesList[innerTemp].x, coordinatesList[innerTemp].y] = true;
                                }
                            }
                            else
                            {
                                break;
                            }
                                
                            break;
                        }
                        else
                        {
                            tempCoordinate.x = i;
                            tempCoordinate.y = temp;
                            coordinatesList.Add(tempCoordinate);
                        }
                    }
                    catch (IndexOutOfRangeException)
                    {
                        break;
                    }
                }
                coordinatesList.Clear();
                break;
            default:
                break;
        }
    }

    private void GenerateRoad(int i, int j)
    {
        Vector3Int tempVector = new Vector3Int(i,j,0);
        Vector3 vector3;
        GameObject tempNode;
        string tempString = "";
        try
        {
            if (isRoad[i + 1, j] == true)
            {
                tempString += "1";
            }
            else
            {
                tempString += "0";
            }
        }
        catch (IndexOutOfRangeException)
        {
            tempString += "0";
        }

        try
        {
            if (isRoad[i, j - 1] == true)
            {
                tempString += "1";
            }
            else
            {
                tempString += "0";
            }
        }
        catch (IndexOutOfRangeException)
        {
            tempString += "0";
        }

        try
        {

            if (isRoad[i - 1, j] == true)
            {
                tempString += "1";
            }
            else
            {
                tempString += "0";
            }
        }
        catch (IndexOutOfRangeException)
        {
            tempString += "0";
        }


        try
        {
            if (isRoad[i, j + 1] == true)
            {
                tempString += "1";
            }
            else
            {
                tempString += "0";
            }
        }
        catch (IndexOutOfRangeException)
        {
            tempString += "0";
        }

        switch (tempString)
        {
            case "1010":
                grid.SetTile(tempVector, roadTiles[5]);
                vector3 = grid.CellToWorld(tempVector);
                vector3 = new Vector3(vector3.x + 16f, vector3.y + 16f, vector3.z);
                tempNode = Instantiate(wayPoints[5], vector3, new Quaternion());
                tempNode.transform.SetParent(WaypointManager.transform, true);
                tempNode.name = vector3.y + " " + vector3.x;
                EWRoadWaypoints.Add(tempNode);
                break;
            case "0101":
                grid.SetTile(tempVector, roadTiles[6]);
                vector3 = grid.CellToWorld(tempVector);
                vector3 = new Vector3(vector3.x + 16f, vector3.y + 16f, vector3.z);
                tempNode = Instantiate(wayPoints[6], vector3, new Quaternion());
                tempNode.transform.SetParent(WaypointManager.transform, true);
                tempNode.name = vector3.x + " " + vector3.y;
                NSRoadWaypoints.Add(tempNode);
                break;
            case "1110":
                grid.SetTile(tempVector, roadTiles[1]);
                vector3 = grid.CellToWorld(tempVector);
                vector3 = new Vector3(vector3.x + 16f, vector3.y + 16f, vector3.z);
                tempNode = Instantiate(wayPoints[1], vector3, new Quaternion());
                tempNode.transform.Rotate(new Vector3(0, 0, 270), Space.World);
                tempNode.transform.SetParent(WaypointManager.transform, true);
                tempNode.name = vector3.x + " " + vector3.y;
                InstersectionWaypoints.Add(tempNode);
                break;
            case "1101":
                grid.SetTile(tempVector, roadTiles[3]);
                vector3 = grid.CellToWorld(tempVector);
                vector3 = new Vector3(vector3.x + 16f, vector3.y + 16f, vector3.z);
                tempNode = Instantiate(wayPoints[3], vector3, new Quaternion());
                tempNode.transform.SetParent(WaypointManager.transform, true);
                tempNode.name = vector3.x + " " + vector3.y;
                InstersectionWaypoints.Add(tempNode);
                break;
            case "1011":
                grid.SetTile(tempVector, roadTiles[2]);
                vector3 = grid.CellToWorld(tempVector);
                vector3 = new Vector3(vector3.x + 16f, vector3.y + 16f, vector3.z);
                tempNode = Instantiate(wayPoints[2], vector3, new Quaternion());
                tempNode.transform.Rotate(new Vector3(0, 0, 90), Space.World);
                tempNode.transform.SetParent(WaypointManager.transform, true);
                tempNode.name = vector3.x + " " + vector3.y;
                InstersectionWaypoints.Add(tempNode);
                break;
            case "0111":
                grid.SetTile(tempVector, roadTiles[0]);
                vector3 = grid.CellToWorld(tempVector);
                vector3 = new Vector3(vector3.x + 16f, vector3.y + 16f, vector3.z);
                tempNode = Instantiate(wayPoints[0], vector3, new Quaternion());
                tempNode.transform.Rotate(new Vector3(0, 0, 180), Space.World);
                tempNode.transform.SetParent(WaypointManager.transform, true);
                tempNode.name = vector3.x + " " + vector3.y;
                InstersectionWaypoints.Add(tempNode);
                break;
            case "1111":
                grid.SetTile(tempVector, roadTiles[4]);
                vector3 = grid.CellToWorld(tempVector);
                vector3 = new Vector3(vector3.x + 16f, vector3.y + 16f, vector3.z);
                tempNode = Instantiate(wayPoints[4], vector3, new Quaternion());
                tempNode.transform.SetParent(WaypointManager.transform, true);
                tempNode.name = vector3.x + " " + vector3.y;
                InstersectionWaypoints.Add(tempNode);
                break;
            case "1100":
                grid.SetTile(tempVector, roadTiles[8]);
                vector3 = grid.CellToWorld(tempVector);
                vector3 = new Vector3(vector3.x + 16f, vector3.y + 16f, vector3.z);
                tempNode = Instantiate(wayPoints[8], vector3, new Quaternion());
                tempNode.transform.Rotate(new Vector3(0, 0, 90), Space.World);
                tempNode.transform.SetParent(WaypointManager.transform, true);
                tempNode.name = vector3.x + " " + vector3.y;
                InstersectionWaypoints.Add(tempNode);
                break;
            case "0110":
                grid.SetTile(tempVector, roadTiles[7]);
                vector3 = grid.CellToWorld(tempVector);
                vector3 = new Vector3(vector3.x + 16f, vector3.y + 16f, vector3.z);
                tempNode = Instantiate(wayPoints[7], vector3, new Quaternion());
                tempNode.transform.SetParent(WaypointManager.transform, true);
                tempNode.name = vector3.x + " " + vector3.y;
                InstersectionWaypoints.Add(tempNode);
                break;
            case "0011":
                grid.SetTile(tempVector, roadTiles[9]);
                vector3 = grid.CellToWorld(tempVector);
                vector3 = new Vector3(vector3.x + 16f, vector3.y + 16f, vector3.z);
                tempNode = Instantiate(wayPoints[9], vector3, new Quaternion());
                tempNode.transform.Rotate(new Vector3(0, 0, 270), Space.World);
                tempNode.transform.SetParent(WaypointManager.transform, true);
                tempNode.name = vector3.x + " " + vector3.y;
                InstersectionWaypoints.Add(tempNode);
                break;
            case "1001":
                grid.SetTile(tempVector, roadTiles[10]);
                vector3 = grid.CellToWorld(tempVector);
                vector3 = new Vector3(vector3.x + 16f, vector3.y + 16f, vector3.z);
                tempNode = Instantiate(wayPoints[10], vector3, new Quaternion());
                tempNode.transform.Rotate(new Vector3(0, 0, 180), Space.World);
                tempNode.transform.SetParent(WaypointManager.transform, true);
                tempNode.name = vector3.x + " " + vector3.y;
                InstersectionWaypoints.Add(tempNode);
                break;     
        }
    }

    private void CreateSquare(Vector3Int tempStart, RoadDivision roadDivision)
    {
        Vector3Int tempVec;
        if (!(roadDivision.sizeX < tempStart.x + roadDivision.creationLengthX || roadDivision.sizeY < tempStart.y + roadDivision.creationLengthY))
        {
            for (int a = tempStart.x; a < roadDivision.creationLengthX + tempStart.x; a++)
            {
                for (int b = tempStart.y; b < roadDivision.creationLengthY + tempStart.y; b++)
                {
                    if (isRoad[a, b] != true)
                    {
                        if (a == tempStart.x || a == tempStart.x + roadDivision.creationLengthX - 1)
                        {
                            tempVec = new Vector3Int(a, b, 0);
                            isRoad[a, b] = true;
                        }
                        else if (b == tempStart.y || b == tempStart.y + roadDivision.creationLengthY - 1)
                        {
                            tempVec = new Vector3Int(a, b, 0);
                            isRoad[a, b] = true;
                        }
                    }
                }
            }
        }   
    }

    private void GenerateWaypointPaths()
    {
        GameObject temp;
        String tempString;
        float prevX = 0f;
        float prevprevX = 0f;
        bool flagOne = false;
        bool flagTwo = false;
        bool flagThree = false;
        bool flagFour = false;
        List<GameObject> tempNodes;

        //Deal With NS Roads First
        //Sort the EW Roads by name
        //Deal with EW Roads
        //Deal with intersections of left over roads

        //Creating connections for them first
        for (int i = 0; i < NSRoadWaypoints.Count; i++)
        {
            flagOne = false;
            flagTwo = false;
            tempString = NSRoadWaypoints[i].transform.position.x + " " + (NSRoadWaypoints[i].transform.position.y + 32);
            try
            {
                temp = NSRoadWaypoints[i + 1];
                if (temp.name == tempString)
                {
                    NSRoadWaypoints[i].transform.GetChild(0).gameObject.GetComponent<BasicRoadNode>().nextNode = temp.transform.GetChild(0).gameObject;
                    flagOne = true;
                }
            }
            catch (ArgumentOutOfRangeException) { }
            tempString = NSRoadWaypoints[i].transform.position.x + " " + (NSRoadWaypoints[i].transform.position.y - 32);
            try
            {
                temp = NSRoadWaypoints[i - 1];
                if (temp.name == tempString)
                {
                    NSRoadWaypoints[i].transform.GetChild(1).gameObject.GetComponent<BasicRoadNode>().nextNode = temp.transform.GetChild(1).gameObject;
                    flagTwo = true;
                }
            }
            catch (ArgumentOutOfRangeException) { }
            if (flagOne == true && flagTwo == true)
            {
                NSRoadWaypoints[i].GetComponent<Information>().isFull = true;
            }
        }
        //Clearing out the ones that are full and replacing the array
        tempNodes = new List<GameObject>();
        for(int i = 0; i < NSRoadWaypoints.Count; i++)
        {
            if (NSRoadWaypoints[i].GetComponent<Information>().isFull == false)
            {
                tempNodes.Add(NSRoadWaypoints[i]);
            }
        }
        NSRoadWaypoints = tempNodes;
        Debug.Log(NSRoadWaypoints.Count);

        //Creating the connections
        EWRoadWaypoints = EWRoadWaypoints.OrderBy(temp => temp.name).ToList();
        for(int i = 0; i < EWRoadWaypoints.Count; i++)
        {
            flagOne = false;
            flagTwo = false;

            tempString = EWRoadWaypoints[i].transform.position.y + " " + (EWRoadWaypoints[i].transform.position.x + 32);
            for(int j = i; j < EWRoadWaypoints.Count; j++)
            {
                temp = EWRoadWaypoints[j];
                if (temp.name == tempString)
                {
                    EWRoadWaypoints[i].transform.GetChild(1).gameObject.GetComponent<BasicRoadNode>().nextNode = temp.transform.GetChild(1).gameObject;
                    flagOne = true;
                    break;
                }
            }
            if (flagOne != true)
            {
                for (int j = i; j > 0; j--)
                {
                    temp = EWRoadWaypoints[j];
                    if (temp.name == tempString)
                    {
                        EWRoadWaypoints[i].transform.GetChild(1).gameObject.GetComponent<BasicRoadNode>().nextNode = temp.transform.GetChild(1).gameObject;
                        flagOne = true;
                        break;
                    }
                }
            }

            tempString = EWRoadWaypoints[i].transform.position.y + " " + (EWRoadWaypoints[i].transform.position.x - 32);

            for(int j = i; j > 0; j--)
            {
                temp = EWRoadWaypoints[j];
                if (temp.name == tempString)
                {
                    EWRoadWaypoints[i].transform.GetChild(0).gameObject.GetComponent<BasicRoadNode>().nextNode = temp.transform.GetChild(0).gameObject;
                    flagTwo = true;
                    break;
                }
            }
            if (flagTwo != true)
            {
                for (int j = i; j < EWRoadWaypoints.Count; j++)
                {
                    temp = EWRoadWaypoints[j];
                    if (temp.name == tempString)
                    {
                        EWRoadWaypoints[i].transform.GetChild(0).gameObject.GetComponent<BasicRoadNode>().nextNode = temp.transform.GetChild(0).gameObject;
                        flagTwo = true;
                        break;
                    }
                }
            }
            
            if (flagOne == true && flagTwo == true)
            {
                EWRoadWaypoints[i].GetComponent<Information>().isFull = true;
            }
        }
        //Deleting all the extras out of the list
        tempNodes = new List<GameObject>();
        for (int i = 0; i < EWRoadWaypoints.Count; i++)
        {
            if (EWRoadWaypoints[i].GetComponent<Information>().isFull == false)
            {
                tempNodes.Add(EWRoadWaypoints[i]);
            }
        }
        EWRoadWaypoints = tempNodes;
        Debug.Log(EWRoadWaypoints.Count);

        foreach(GameObject waypoint in EWRoadWaypoints)
        {
            waypoint.name = waypoint.transform.position.x + " " + waypoint.transform.position.y;
        }
        EWRoadWaypoints = EWRoadWaypoints.OrderBy(temp => temp.name).ToList();

        //Trying to find waypoints for intersections
        foreach (GameObject waypoint in InstersectionWaypoints)
        {
            tempString = waypoint.GetComponent<Information>().type;
            flagOne = false;
            switch (tempString)
            {
                case "3WE":
                    //Side South
                    tempString = waypoint.transform.position.x + " " + (waypoint.transform.position.y - 32);
                    for(int i = 0; i < NSRoadWaypoints.Count; i++)
                    {
                        if (NSRoadWaypoints[i].name == tempString)
                        {
                            NSRoadWaypoints[i].transform.GetChild(0).gameObject.GetComponent<BasicRoadNode>().nextNode = waypoint.transform.GetChild(1).gameObject;
                            waypoint.transform.GetChild(0).gameObject.GetComponent<BasicRoadNode>().nextNode = NSRoadWaypoints[i].transform.GetChild(1).gameObject;
                            break;
                        }
                    }

                    //North Side
                    tempString = waypoint.transform.position.x + " " + (waypoint.transform.position.y + 32);
                    for (int i = 0; i < NSRoadWaypoints.Count; i++)
                    {
                        if (NSRoadWaypoints[i].name == tempString)
                        {
                            NSRoadWaypoints[i].transform.GetChild(1).gameObject.GetComponent<BasicRoadNode>().nextNode = waypoint.transform.GetChild(2).gameObject;
                            waypoint.transform.GetChild(3).gameObject.GetComponent<BasicRoadNode>().nextNode = NSRoadWaypoints[i].transform.GetChild(0).gameObject;
                            break;
                        }
                    }

                    //West Side
                    tempString = (waypoint.transform.position.x - 32) + " " + waypoint.transform.position.y;
                    for(int i = 0; i < EWRoadWaypoints.Count; i++)
                    {
                        if (EWRoadWaypoints[i].name == tempString)
                        {
                            EWRoadWaypoints[i].transform.GetChild(1).gameObject.GetComponent<BasicRoadNode>().nextNode = waypoint.transform.GetChild(4).gameObject;
                            waypoint.transform.GetChild(5).gameObject.GetComponent<BasicRoadNode>().nextNode = EWRoadWaypoints[i].transform.GetChild(0).gameObject;
                            break;
                        }
                    }
                    break;
                case "3WN":
                    //Side South
                    tempString = waypoint.transform.position.x + " " + (waypoint.transform.position.y - 32);
                    for (int i = 0; i < NSRoadWaypoints.Count; i++)
                    {
                        if (NSRoadWaypoints[i].name == tempString)
                        {
                            NSRoadWaypoints[i].transform.GetChild(0).gameObject.GetComponent<BasicRoadNode>().nextNode = waypoint.transform.GetChild(4).gameObject;
                            waypoint.transform.GetChild(5).gameObject.GetComponent<BasicRoadNode>().nextNode = NSRoadWaypoints[i].transform.GetChild(1).gameObject;
                            break;
                        }
                    }

                    //West Side
                    tempString = (waypoint.transform.position.x - 32) + " " + waypoint.transform.position.y;
                    for (int i = 0; i < EWRoadWaypoints.Count; i++)
                    {
                        if (EWRoadWaypoints[i].name == tempString)
                        {
                            EWRoadWaypoints[i].transform.GetChild(1).gameObject.GetComponent<BasicRoadNode>().nextNode = waypoint.transform.GetChild(2).gameObject;
                            waypoint.transform.GetChild(3).gameObject.GetComponent<BasicRoadNode>().nextNode = EWRoadWaypoints[i].transform.GetChild(0).gameObject;
                            break;
                        }
                    }

                    //East Side
                    tempString = (waypoint.transform.position.x + 32) + " " + waypoint.transform.position.y;
                    for (int i = 0; i < EWRoadWaypoints.Count; i++)
                    {
                        if (EWRoadWaypoints[i].name == tempString)
                        {
                            EWRoadWaypoints[i].transform.GetChild(0).gameObject.GetComponent<BasicRoadNode>().nextNode = waypoint.transform.GetChild(1).gameObject;
                            waypoint.transform.GetChild(0).gameObject.GetComponent<BasicRoadNode>().nextNode = EWRoadWaypoints[i].transform.GetChild(1).gameObject;
                            break;
                        }
                    }
                    break;
                case "3WS":
                    //West Side
                    tempString = (waypoint.transform.position.x - 32) + " " + waypoint.transform.position.y;
                    for (int i = 0; i < EWRoadWaypoints.Count; i++)
                    {
                        if (EWRoadWaypoints[i].name == tempString)
                        {
                            EWRoadWaypoints[i].transform.GetChild(1).gameObject.GetComponent<BasicRoadNode>().nextNode = waypoint.transform.GetChild(1).gameObject;
                            waypoint.transform.GetChild(0).gameObject.GetComponent<BasicRoadNode>().nextNode = EWRoadWaypoints[i].transform.GetChild(0).gameObject;
                            break;
                        }
                    }

                    //East Side
                    tempString = (waypoint.transform.position.x + 32) + " " + waypoint.transform.position.y;
                    for (int i = 0; i < EWRoadWaypoints.Count; i++)
                    {
                        if (EWRoadWaypoints[i].name == tempString)
                        {
                            EWRoadWaypoints[i].transform.GetChild(0).gameObject.GetComponent<BasicRoadNode>().nextNode = waypoint.transform.GetChild(2).gameObject;
                            waypoint.transform.GetChild(3).gameObject.GetComponent<BasicRoadNode>().nextNode = EWRoadWaypoints[i].transform.GetChild(1).gameObject;
                            break;
                        }
                    }

                    //North Side
                    tempString = waypoint.transform.position.x + " " + (waypoint.transform.position.y + 32);
                    for (int i = 0; i < NSRoadWaypoints.Count; i++)
                    {
                        if (NSRoadWaypoints[i].name == tempString)
                        {
                            NSRoadWaypoints[i].transform.GetChild(1).gameObject.GetComponent<BasicRoadNode>().nextNode = waypoint.transform.GetChild(4).gameObject;
                            waypoint.transform.GetChild(5).gameObject.GetComponent<BasicRoadNode>().nextNode = NSRoadWaypoints[i].transform.GetChild(0).gameObject;
                            break;
                        }
                    }
                    break;
                case "3WW":
                    //East Side
                    tempString = (waypoint.transform.position.x + 32) + " " + waypoint.transform.position.y;
                    for (int i = 0; i < EWRoadWaypoints.Count; i++)
                    {
                        if (EWRoadWaypoints[i].name == tempString)
                        {
                            EWRoadWaypoints[i].transform.GetChild(0).gameObject.GetComponent<BasicRoadNode>().nextNode = waypoint.transform.GetChild(4).gameObject;
                            waypoint.transform.GetChild(5).gameObject.GetComponent<BasicRoadNode>().nextNode = EWRoadWaypoints[i].transform.GetChild(1).gameObject;
                            break;
                        }
                    }

                    //North Side
                    tempString = waypoint.transform.position.x + " " + (waypoint.transform.position.y + 32);
                    for (int i = 0; i < NSRoadWaypoints.Count; i++)
                    {
                        if (NSRoadWaypoints[i].name == tempString)
                        {
                            NSRoadWaypoints[i].transform.GetChild(1).gameObject.GetComponent<BasicRoadNode>().nextNode = waypoint.transform.GetChild(1).gameObject;
                            waypoint.transform.GetChild(0).gameObject.GetComponent<BasicRoadNode>().nextNode = NSRoadWaypoints[i].transform.GetChild(0).gameObject;
                            break;
                        }
                    }

                    //Side South
                    tempString = waypoint.transform.position.x + " " + (waypoint.transform.position.y - 32);
                    for (int i = 0; i < NSRoadWaypoints.Count; i++)
                    {
                        if (NSRoadWaypoints[i].name == tempString)
                        {
                            NSRoadWaypoints[i].transform.GetChild(0).gameObject.GetComponent<BasicRoadNode>().nextNode = waypoint.transform.GetChild(2).gameObject;
                            waypoint.transform.GetChild(3).gameObject.GetComponent<BasicRoadNode>().nextNode = NSRoadWaypoints[i].transform.GetChild(1).gameObject;
                            break;
                        }
                    }
                    break;
                case "4W":
                    //East Side
                    tempString = (waypoint.transform.position.x + 32) + " " + waypoint.transform.position.y;
                    for (int i = 0; i < EWRoadWaypoints.Count; i++)
                    {
                        if (EWRoadWaypoints[i].name == tempString)
                        {
                            EWRoadWaypoints[i].transform.GetChild(0).gameObject.GetComponent<BasicRoadNode>().nextNode = waypoint.transform.GetChild(7).gameObject;
                            waypoint.transform.GetChild(6).gameObject.GetComponent<BasicRoadNode>().nextNode = EWRoadWaypoints[i].transform.GetChild(1).gameObject;
                            break;
                        }
                    }

                    //North Side
                    tempString = waypoint.transform.position.x + " " + (waypoint.transform.position.y + 32);
                    for (int i = 0; i < NSRoadWaypoints.Count; i++)
                    {
                        if (NSRoadWaypoints[i].name == tempString)
                        {
                            NSRoadWaypoints[i].transform.GetChild(1).gameObject.GetComponent<BasicRoadNode>().nextNode = waypoint.transform.GetChild(1).gameObject;
                            waypoint.transform.GetChild(0).gameObject.GetComponent<BasicRoadNode>().nextNode = NSRoadWaypoints[i].transform.GetChild(0).gameObject;
                            break;
                        }
                    }

                    //Side South
                    tempString = waypoint.transform.position.x + " " + (waypoint.transform.position.y - 32);
                    for (int i = 0; i < NSRoadWaypoints.Count; i++)
                    {
                        if (NSRoadWaypoints[i].name == tempString)
                        {
                            NSRoadWaypoints[i].transform.GetChild(0).gameObject.GetComponent<BasicRoadNode>().nextNode = waypoint.transform.GetChild(5).gameObject;
                            waypoint.transform.GetChild(4).gameObject.GetComponent<BasicRoadNode>().nextNode = NSRoadWaypoints[i].transform.GetChild(1).gameObject;
                            break;
                        }
                    }

                    //West Side
                    tempString = (waypoint.transform.position.x - 32) + " " + waypoint.transform.position.y;
                    for (int i = 0; i < EWRoadWaypoints.Count; i++)
                    {
                        if (EWRoadWaypoints[i].name == tempString)
                        {
                            EWRoadWaypoints[i].transform.GetChild(1).gameObject.GetComponent<BasicRoadNode>().nextNode = waypoint.transform.GetChild(3).gameObject;
                            waypoint.transform.GetChild(2).gameObject.GetComponent<BasicRoadNode>().nextNode = EWRoadWaypoints[i].transform.GetChild(0).gameObject;
                            break;
                        }
                    }
                    break;
                case "CNE":
                    //Side South
                    tempString = waypoint.transform.position.x + " " + (waypoint.transform.position.y - 32);
                    for (int i = 0; i < NSRoadWaypoints.Count; i++)
                    {
                        if (NSRoadWaypoints[i].name == tempString)
                        {
                            NSRoadWaypoints[i].transform.GetChild(0).gameObject.GetComponent<BasicRoadNode>().nextNode = waypoint.transform.GetChild(0).gameObject;
                            waypoint.transform.GetChild(3).gameObject.GetComponent<BasicRoadNode>().nextNode = NSRoadWaypoints[i].transform.GetChild(1).gameObject;
                            break;
                        }
                    }

                    //West Side
                    tempString = (waypoint.transform.position.x - 32) + " " + waypoint.transform.position.y;
                    for (int i = 0; i < EWRoadWaypoints.Count; i++)
                    {
                        if (EWRoadWaypoints[i].name == tempString)
                        {
                            EWRoadWaypoints[i].transform.GetChild(1).gameObject.GetComponent<BasicRoadNode>().nextNode = waypoint.transform.GetChild(1).gameObject;
                            waypoint.transform.GetChild(2).gameObject.GetComponent<BasicRoadNode>().nextNode = EWRoadWaypoints[i].transform.GetChild(0).gameObject;
                            break;
                        }
                    }
                    break;
                case "CNW":
                    //Side South
                    tempString = waypoint.transform.position.x + " " + (waypoint.transform.position.y - 32);
                    for (int i = 0; i < NSRoadWaypoints.Count; i++)
                    {
                        if (NSRoadWaypoints[i].name == tempString)
                        {
                            NSRoadWaypoints[i].transform.GetChild(0).gameObject.GetComponent<BasicRoadNode>().nextNode = waypoint.transform.GetChild(1).gameObject;
                            waypoint.transform.GetChild(2).gameObject.GetComponent<BasicRoadNode>().nextNode = NSRoadWaypoints[i].transform.GetChild(1).gameObject;
                            break;
                        }
                    }

                    //East Side
                    tempString = (waypoint.transform.position.x + 32) + " " + waypoint.transform.position.y;
                    for (int i = 0; i < EWRoadWaypoints.Count; i++)
                    {
                        if (EWRoadWaypoints[i].name == tempString)
                        {
                            EWRoadWaypoints[i].transform.GetChild(0).gameObject.GetComponent<BasicRoadNode>().nextNode = waypoint.transform.GetChild(0).gameObject;
                            waypoint.transform.GetChild(3).gameObject.GetComponent<BasicRoadNode>().nextNode = EWRoadWaypoints[i].transform.GetChild(1).gameObject;
                            break;
                        }
                    }
                    break;
                case "CSE":
                    //West Side
                    tempString = (waypoint.transform.position.x - 32) + " " + waypoint.transform.position.y;
                    for (int i = 0; i < EWRoadWaypoints.Count; i++)
                    {
                        if (EWRoadWaypoints[i].name == tempString)
                        {
                            EWRoadWaypoints[i].transform.GetChild(1).gameObject.GetComponent<BasicRoadNode>().nextNode = waypoint.transform.GetChild(0).gameObject;
                            waypoint.transform.GetChild(3).gameObject.GetComponent<BasicRoadNode>().nextNode = EWRoadWaypoints[i].transform.GetChild(0).gameObject;
                            break;
                        }
                    }

                    //North Side
                    tempString = waypoint.transform.position.x + " " + (waypoint.transform.position.y + 32);
                    for (int i = 0; i < NSRoadWaypoints.Count; i++)
                    {
                        if (NSRoadWaypoints[i].name == tempString)
                        {
                            NSRoadWaypoints[i].transform.GetChild(1).gameObject.GetComponent<BasicRoadNode>().nextNode = waypoint.transform.GetChild(1).gameObject;
                            waypoint.transform.GetChild(2).gameObject.GetComponent<BasicRoadNode>().nextNode = NSRoadWaypoints[i].transform.GetChild(0).gameObject;
                            break;
                        }
                    }
                    break;
                case "CSW":
                    //North Side
                    tempString = waypoint.transform.position.x + " " + (waypoint.transform.position.y + 32);
                    for (int i = 0; i < NSRoadWaypoints.Count; i++)
                    {
                        if (NSRoadWaypoints[i].name == tempString)
                        {
                            NSRoadWaypoints[i].transform.GetChild(1).gameObject.GetComponent<BasicRoadNode>().nextNode = waypoint.transform.GetChild(0).gameObject;
                            waypoint.transform.GetChild(3).gameObject.GetComponent<BasicRoadNode>().nextNode = NSRoadWaypoints[i].transform.GetChild(0).gameObject;
                            break;
                        }
                    }

                    //East Side
                    tempString = (waypoint.transform.position.x + 32) + " " + waypoint.transform.position.y;
                    for (int i = 0; i < EWRoadWaypoints.Count; i++)
                    {
                        if (EWRoadWaypoints[i].name == tempString)
                        {
                            EWRoadWaypoints[i].transform.GetChild(0).gameObject.GetComponent<BasicRoadNode>().nextNode = waypoint.transform.GetChild(1).gameObject;
                            waypoint.transform.GetChild(2).gameObject.GetComponent<BasicRoadNode>().nextNode = EWRoadWaypoints[i].transform.GetChild(1).gameObject;
                            break;
                        }
                    }
                    break;
            }

            //Clearing excess points
            if (waypoint.transform.position.x >= 80 && prevX != waypoint.transform.position.x)
            {
                Debug.Log(prevX);
                Debug.Log(prevprevX);

                for (float j = prevX; j >= prevprevX; j -= 1)
                {
                    for (int i = 0; i < NSRoadWaypoints.Count; i++)
                    {
                        if (NSRoadWaypoints[i].transform.position.x == j)
                        {
                            NSRoadWaypoints.RemoveAt(i);
                        }
                    }
                    for (int i = 0; i < EWRoadWaypoints.Count; i++)
                    {
                        if (EWRoadWaypoints[i].transform.position.x == j)
                        {
                            EWRoadWaypoints.RemoveAt(i);
                        }
                    }
                }
                flagOne = true;
            }
            if (flagOne == true)
            {
                prevprevX = prevX;
                prevX = waypoint.transform.position.x;
            }
        }

        Debug.Log(NSRoadWaypoints.Count);
        Debug.Log(EWRoadWaypoints.Count);

        NSRoadWaypoints.Clear();
        EWRoadWaypoints.Clear();


        /*
        foreach (GameObject waypoint in currentWaypoints)
        {
            //Gets the type of directions it needs to test for.
            tempString = waypoint.GetComponent<Information>().type;
            switch (tempString)
            {
                case "NS":
                    tempString = waypoint.transform.position.x + " " + (waypoint.transform.position.y + 32);
                    temp = GameObject.Find(tempString);
                    if (temp != null)
                    {
                        waypoint.transform.GetChild(0).gameObject.GetComponent<BasicRoadNode>().nextNode = temp.transform.GetChild(0).gameObject;
                    }
                    tempString = waypoint.transform.position.x + " " + (waypoint.transform.position.y - 32);
                    temp = GameObject.Find(tempString);
                    if (temp != null)
                    {
                        waypoint.transform.GetChild(1).gameObject.GetComponent<BasicRoadNode>().nextNode = temp.transform.GetChild(1).gameObject;
                    }
                    break;
                case "EW":
                    tempString = (waypoint.transform.position.x + 32) + " " + waypoint.transform.position.y;
                    temp = GameObject.Find(tempString);
                    if (temp != null)
                    {
                        waypoint.transform.GetChild(1).gameObject.GetComponent<BasicRoadNode>().nextNode = temp.transform.GetChild(1).gameObject;
                    }
                    tempString = (waypoint.transform.position.x - 32) + " " + waypoint.transform.position.y;
                    temp = GameObject.Find(tempString);
                    if (temp != null)
                    {
                        waypoint.transform.GetChild(0).gameObject.GetComponent<BasicRoadNode>().nextNode = temp.transform.GetChild(0).gameObject;
                    }
                    break;
            }
            /*
            tempString = waypoint.transform.position.x + 32 + " " + waypoint.transform.position.y;
            Debug.Log(tempString);
            temp = currentWaypoints.Where(waypoint => waypoint.name == tempString).SingleOrDefault();
            if (temp != null)
            {
                Debug.Log(temp);
            }
            */
        //}
    }

    private void isRoadFull(GameObject gameObject, int index)
    {
        string type = gameObject.GetComponent<Information>().type;
        GameObject tempOne = gameObject.transform.GetChild(0).gameObject.GetComponent<BasicRoadNode>().nextNode;
        GameObject tempTwo = gameObject.transform.GetChild(1).gameObject.GetComponent<BasicRoadNode>().nextNode;
        if (tempOne != null && tempTwo != null)
        {
            if (type == "NS")
            {
                NSRoadWaypoints.RemoveAt(index);
            }
            else
            {
                EWRoadWaypoints.RemoveAt(index);
            }
        }
    }
}

using MainProject.Domain.Basic;
using System.Threading.Tasks;

namespace MainProject.Domain.WorldRelated;

public class WorldCellularAutomaton
{
    public List<Triangle> Triangles = new List<Triangle>();
    public List<Point> Points = new List<Point>();
    public List<Cube> Cubes = new List<Cube>();

    public Cube[,,] mapOfAutomatonCubes;
    public int[,,] mapOfAutomaton;
    public int sizeX;
    public int sizeY;
    public int sizeZ;

    public const double sizeOfSingleCube = 0.5;
    public const double startDistanceFromAutomaton = 10;

    List<int[]> colors = new List<int[]>
    {
        new int[] { 0, 0, 0 },
        new int[] { 255, 255, 0 },
        new int[] { 255, 85, 0 },
        new int[] { 255, 170, 0 },
        new int[] { 255, 0, 0 }
    };

    public WorldCellularAutomaton(int x, int y, int z)
    {
        mapOfAutomatonCubes = new Cube[x, y, z];
        mapOfAutomaton = new int[x, y, z];
        sizeX = x;
        sizeY = y;
        sizeZ = z;

        SetUpAutomatonWorld();
        // InitializeStartMap(Maps.map2);
        InitializeStartHaos();
    }

    public void SetUpAutomatonWorld()
    {
        // calculateCoordinates
        double left = -sizeX * sizeOfSingleCube / 2;
        double right = sizeX * sizeOfSingleCube / 2;
        double top = sizeY * sizeOfSingleCube / 2;
        double bottom = -sizeY * sizeOfSingleCube / 2;
        double front = startDistanceFromAutomaton;
        double back = startDistanceFromAutomaton + sizeZ * sizeOfSingleCube;

        Point[,,] tempPoints = new Point[sizeX + 1, sizeY + 1, sizeZ + 1];
        for (int i = 0; i < sizeX + 1; i++)
        {
            for (int j = 0; j < sizeY + 1; j++)
            {
                for (int k = 0; k < sizeZ + 1; k++)
                {
                    Point3D point3d = new Point3D(
                        left + i * sizeOfSingleCube,
                        bottom + j * sizeOfSingleCube,
                        front + k * sizeOfSingleCube
                    );
                    Point point = new Point(point3d);
                    tempPoints[i, j, k] = point;
                    Points.Add(point);
                }
            }
        }

        for (int i = 0; i < sizeX; i++)
        {
            for (int j = 0; j < sizeY; j++)
            {
                for (int k = 0; k < sizeZ; k++)
                {
                    Cube cube = new Cube();
                    // Cube cube = CreateCube(
                    //     tempPoints[i, j, k],
                    //     tempPoints[i, j + 1, k],
                    //     tempPoints[i + 1, j + 1, k],
                    //     tempPoints[i + 1, j, k],
                    //     tempPoints[i, j, k + 1],
                    //     tempPoints[i, j + 1, k + 1],
                    //     tempPoints[i + 1, j + 1, k + 1],
                    //     tempPoints[i + 1, j, k + 1]
                    // );
                    Cubes.Add(cube);
                    cube.MakeInvisible();
                    mapOfAutomatonCubes[i, j, k] = cube;
                }
            }
        }
        
        for (int i = 0; i < sizeX; i++)
        {
            for (int j = 0; j < sizeY; j++)
            {
                for (int k = 0; k < sizeZ; k++)
                {
                    //Sciana 1 - front
                    Triangle t11 = new Triangle(tempPoints[i, j, k], tempPoints[i + 1, j, k], tempPoints[i, j + 1, k]);
                    Triangle t12 = new Triangle(tempPoints[i + 1, j, k], tempPoints[i, j + 1, k], tempPoints[i + 1, j + 1, k]);
                    Triangles.Add(t11);
                    Triangles.Add(t12);
                    mapOfAutomatonCubes[i,j,k].Triangles.Add(t11);
                    mapOfAutomatonCubes[i,j,k].Triangles.Add(t12);
                    if (k != 0)
                    {
                        mapOfAutomatonCubes[i,j,k-1].Triangles.Add(t11);
                        mapOfAutomatonCubes[i,j,k-1].Triangles.Add(t12);
                    }
                    
                    //Sciana 2 - left
                    Triangle t21 = new Triangle(tempPoints[i, j, k], tempPoints[i, j, k + 1], tempPoints[i, j + 1, k]);
                    Triangle t22 = new Triangle(tempPoints[i, j, k + 1], tempPoints[i, j + 1, k],
                        tempPoints[i, j + 1, k + 1]);
                    Triangles.Add(t21);
                    Triangles.Add(t22);
                    mapOfAutomatonCubes[i,j,k].Triangles.Add(t21);
                    mapOfAutomatonCubes[i,j,k].Triangles.Add(t22);
                    if (i != 0)
                    {
                        mapOfAutomatonCubes[i-1,j,k].Triangles.Add(t21);
                        mapOfAutomatonCubes[i-1,j,k].Triangles.Add(t22);
                    }
                    
                    //Sciana 3 - bottom
                    Triangle t31 = new Triangle(tempPoints[i, j, k], tempPoints[i + 1, j, k], tempPoints[i, j, k + 1]);
                    Triangle t32 = new Triangle(tempPoints[i + 1, j, k], tempPoints[i, j, k + 1],
                        tempPoints[i + 1, j, k + 1]);
                    Triangles.Add(t31);
                    Triangles.Add(t32);
                    mapOfAutomatonCubes[i,j,k].Triangles.Add(t31);
                    mapOfAutomatonCubes[i,j,k].Triangles.Add(t32);
                    if (j != 0)
                    {
                        mapOfAutomatonCubes[i,j-1,k].Triangles.Add(t31);
                        mapOfAutomatonCubes[i,j-1,k].Triangles.Add(t32);
                    }

                    if (i == sizeX - 1)
                    {
                        //Sciana 4 = right
                        Triangle t41 = new Triangle(tempPoints[i + 1, j, k], tempPoints[i + 1, j, k + 1],
                            tempPoints[i + 1, j + 1, k]);
                        Triangle t42 = new Triangle(tempPoints[i + 1, j + 1, k], tempPoints[i + 1, j, k + 1],
                            tempPoints[i + 1, j + 1, k + 1]);
                        Triangles.Add(t41);
                        Triangles.Add(t42);
                        mapOfAutomatonCubes[i,j,k].Triangles.Add(t41);
                        mapOfAutomatonCubes[i,j,k].Triangles.Add(t42);
                    }

                    if (j == sizeY - 1)
                    {   
                        //Sciana 5 - top
                        Triangle t51 = new Triangle(tempPoints[i, j + 1, k], tempPoints[i, j + 1, k + 1],
                            tempPoints[i + 1, j + 1, k]);
                        Triangle t52 = new Triangle(tempPoints[i, j + 1, k + 1], tempPoints[i + 1, j + 1, k],
                            tempPoints[i + 1, j + 1, k + 1]);
                        Triangles.Add(t51);
                        Triangles.Add(t52);
                        mapOfAutomatonCubes[i,j,k].Triangles.Add(t51);
                        mapOfAutomatonCubes[i,j,k].Triangles.Add(t52);
                    }

                    if (k == sizeZ - 1)
                    {
                        //Sciana 6 - back
                        Triangle t61 = new Triangle(tempPoints[i, j, k + 1], tempPoints[i, j + 1, k + 1],
                            tempPoints[i + 1, j, k + 1]);
                        Triangle t62 = new Triangle(tempPoints[i, j + 1, k + 1], tempPoints[i + 1, j, k + 1],
                            tempPoints[i + 1, j + 1, k + 1]);
                        Triangles.Add(t61);
                        Triangles.Add(t62);
                        mapOfAutomatonCubes[i,j,k].Triangles.Add(t61);
                        mapOfAutomatonCubes[i,j,k].Triangles.Add(t62);
                    }
                }
            }
        }
        
        // for (int i = 0; i < sizeX; i++)
        // {
        //     for (int j = 0; j < sizeY; j++)
        //     {
        //         for (int k = 0; k < sizeZ; k++)
        //         {
        //             Cube cube = mapOfAutomatonCubes[i, j, k];
        //             // cube.MakeInvisible();
        //         }
        //     }
        // }
    }

    public bool isBorderPoint(int i, int j, int k)
    {
        if (i == 0 || j == 0 || k == 0)
            return true;
        if (i == sizeX || j == sizeY || k == sizeZ )
            return true;
        return false;
    }

    public void IterateWorld()
    {
        int[,,] tmpMap = new int[sizeX, sizeY, sizeZ];
        int[,] tmpLayer;
        for (int z = 0; z < sizeZ; z++)
        {
            tmpLayer = IterateLayer(z);
            for (int x = 0; x < sizeX; x++)
            {
                for (int y = 0; y < sizeY; y++)
                {
                    tmpMap[x, y, z] = tmpLayer[x, y];
                }
            }
        }

        for (int x = 0; x < sizeX; x++)
        {
            for (int y = 0; y < sizeY; y++)
            {
                for (int z = 0; z < sizeZ; z++)
                {
                    mapOfAutomaton[x, y, z] = tmpMap[x, y, z];
                    if (mapOfAutomaton[x, y, z] == 0)
                    {
                        mapOfAutomatonCubes[x, y, z].MakeInvisible();
                    }
                    else
                    {
                        mapOfAutomatonCubes[x, y, z].SetColor(colors[mapOfAutomaton[x, y, z]]);
                        mapOfAutomatonCubes[x, y, z].MakeVisible();
                    }
                }
            }
        }
    }

    public void IterateWorldParallel()
    {
        int[,,] tmpMap = new int[sizeX, sizeY, sizeZ];
        Parallel.For(
            0,
            sizeZ,
            z =>
            {
                var tmpLayer = IterateLayer(z);
                for (int x = 0; x < sizeX; x++)
                {
                    for (int y = 0; y < sizeY; y++)
                    {
                        tmpMap[x, y, z] = tmpLayer[x, y];
                    }
                }
            }
        );

        Parallel.For(
            0,
            sizeX,
            x =>
            {
                for (int y = 0; y < sizeY; y++)
                {
                    for (int z = 0; z < sizeZ; z++)
                    {
                        mapOfAutomaton[x, y, z] = tmpMap[x, y, z];
                        if (mapOfAutomaton[x, y, z] == 0)
                        {
                            mapOfAutomatonCubes[x, y, z].MakeInvisible();
                        }
                        else
                        {
                            mapOfAutomatonCubes[x, y, z].SetColor(colors[mapOfAutomaton[x, y, z]]);
                            mapOfAutomatonCubes[x, y, z].MakeVisible();
                        }
                    }
                }
            }
        );
    }

    private int[,] IterateLayer(int layerZ)
    {
        int[,] tmpLayer = new int[sizeX, sizeY];
        for (int x = 0; x < sizeX; x++)
        {
            for (int y = 0; y < sizeY; y++)
            {
                tmpLayer[x, y] = IterateCube(x, y, layerZ);
            }
        }
        return tmpLayer;
    }

    private int IterateCube(int x, int y, int z)
    {
        // int neighboursCount = CountNeighbours(x, y, z, Neighbourhood.Moore);
        int neighboursCount = CountNeighboursINF(x, y, z, Neighbourhood.Moore);
        if (mapOfAutomaton[x, y, z] == 4)
        {
            if (neighboursCount == 4)
                return 4;
        }
        else if (mapOfAutomaton[x, y, z] == 0)
        {
            if (neighboursCount == 4)
            {
                return 4;
            }
            return 0;
        }

        return mapOfAutomaton[x, y, z] - 1;
    }

    private int CountNeighbours(int x, int y, int z, Neighbourhood n)
    {
        int neighboursCount = 0;

        if (x > 0 && mapOfAutomaton[x - 1, y, z] == 4)
            neighboursCount++;
        if (x < sizeX - 1 && mapOfAutomaton[x + 1, y, z] == 4)
            neighboursCount++;
        if (y > 0 && mapOfAutomaton[x, y - 1, z] == 4)
            neighboursCount++;
        if (y < sizeY - 1 && mapOfAutomaton[x, y + 1, z] == 4)
            neighboursCount++;
        if (z > 0 && mapOfAutomaton[x, y, z - 1] == 4)
            neighboursCount++;
        if (z < sizeZ - 1 && mapOfAutomaton[x, y, z + 1] == 4)
            neighboursCount++;

        if (n == Neighbourhood.Moore)
        {
            if (x > 0 && y > 0 && mapOfAutomaton[x - 1, y - 1, z] == 4)
                neighboursCount++;
            if (x > 0 && y < sizeY - 1 && mapOfAutomaton[x - 1, y + 1, z] == 4)
                neighboursCount++;
            if (x > 0 && z > 0 && mapOfAutomaton[x - 1, y, z - 1] == 4)
                neighboursCount++;
            if (x > 0 && z < sizeZ - 1 && mapOfAutomaton[x - 1, y, z + 1] == 4)
                neighboursCount++;

            if (x < sizeX - 1 && y > 0 && mapOfAutomaton[x + 1, y - 1, z] == 4)
                neighboursCount++;
            if (x < sizeX - 1 && y < sizeY - 1 && mapOfAutomaton[x + 1, y + 1, z] == 4)
                neighboursCount++;
            if (x < sizeX - 1 && z > 0 && mapOfAutomaton[x + 1, y, z - 1] == 4)
                neighboursCount++;
            if (x < sizeX - 1 && z < sizeZ - 1 && mapOfAutomaton[x + 1, y, z + 1] == 4)
                neighboursCount++;

            if (y > 0 && z > 0 && mapOfAutomaton[x, y - 1, z - 1] == 4)
                neighboursCount++;
            if (y > 0 && z < sizeZ - 1 && mapOfAutomaton[x, y - 1, z + 1] == 4)
                neighboursCount++;
            if (y < sizeY - 1 && z > 0 && mapOfAutomaton[x, y + 1, z - 1] == 4)
                neighboursCount++;
            if (y < sizeY - 1 && z < sizeZ - 1 && mapOfAutomaton[x, y + 1, z + 1] == 4)
                neighboursCount++;

            // Corners
            if (x > 0 && y > 0 && z > 0 && mapOfAutomaton[x - 1, y - 1, z - 1] == 4)
                neighboursCount++;
            if (x > 0 && y > 0 && z < sizeZ - 1 && mapOfAutomaton[x - 1, y - 1, z + 1] == 4)
                neighboursCount++;
            if (x > 0 && y < sizeY - 1 && z > 0 && mapOfAutomaton[x - 1, y + 1, z - 1] == 4)
                neighboursCount++;
            if (x > 0 && y < sizeY - 1 && z < sizeZ - 1 && mapOfAutomaton[x - 1, y + 1, z + 1] == 4)
                neighboursCount++;

            if (x < sizeX - 1 && y > 0 && z > 0 && mapOfAutomaton[x + 1, y - 1, z - 1] == 4)
                neighboursCount++;
            if (x < sizeX - 1 && y > 0 && z < sizeZ - 1 && mapOfAutomaton[x + 1, y - 1, z + 1] == 4)
                neighboursCount++;
            if (x < sizeX - 1 && y < sizeY - 1 && z > 0 && mapOfAutomaton[x + 1, y + 1, z - 1] == 4)
                neighboursCount++;
            if (
                x < sizeX - 1
                && y < sizeY - 1
                && z < sizeZ - 1
                && mapOfAutomaton[x + 1, y + 1, z + 1] == 4
            )
                neighboursCount++;
        }
        return neighboursCount;
    }

    // Count neighbours for with continuous space
    private int CountNeighboursINF(int x, int y, int z, Neighbourhood n)
    {
        int neighboursCount = 0;

        var x_minus = x > 0 ? x - 1 : sizeX - 1;
        var x_plus = x < sizeX - 1 ? x + 1 : 0;
        var y_minus = y > 0 ? y - 1 : sizeY - 1;
        var y_plus = y < sizeY - 1 ? y + 1 : 0;
        var z_minus = z > 0 ? z - 1 : sizeZ - 1;
        var z_plus = z < sizeZ - 1 ? z + 1 : 0;

        if (mapOfAutomaton[x_minus, y, z] == 4)
            neighboursCount++;
        if (mapOfAutomaton[x_plus, y, z] == 4)
            neighboursCount++;
        if (mapOfAutomaton[x, y_minus, z] == 4)
            neighboursCount++;
        if (mapOfAutomaton[x, y_plus, z] == 4)
            neighboursCount++;
        if (mapOfAutomaton[x, y, z_minus] == 4)
            neighboursCount++;
        if (mapOfAutomaton[x, y, z_plus] == 4)
            neighboursCount++;

        if (n == Neighbourhood.Moore)
        {
            if (mapOfAutomaton[x_minus, y_minus, z] == 4)
                neighboursCount++;
            if (mapOfAutomaton[x_minus, y_plus, z] == 4)
                neighboursCount++;
            if (mapOfAutomaton[x_minus, y, z_minus] == 4)
                neighboursCount++;
            if (mapOfAutomaton[x_minus, y, z_plus] == 4)
                neighboursCount++;

            if (mapOfAutomaton[x_plus, y_minus, z] == 4)
                neighboursCount++;
            if (mapOfAutomaton[x_plus, y_plus, z] == 4)
                neighboursCount++;
            if (mapOfAutomaton[x_plus, y, z_minus] == 4)
                neighboursCount++;
            if (mapOfAutomaton[x_plus, y, z_plus] == 4)
                neighboursCount++;

            if (mapOfAutomaton[x, y_minus, z_minus] == 4)
                neighboursCount++;
            if (mapOfAutomaton[x, y_minus, z_plus] == 4)
                neighboursCount++;
            if (mapOfAutomaton[x, y_plus, z_minus] == 4)
                neighboursCount++;
            if (mapOfAutomaton[x, y_plus, z_plus] == 4)
                neighboursCount++;

            // Corners
            if (mapOfAutomaton[x_minus, y_minus, z_minus] == 4)
                neighboursCount++;
            if (mapOfAutomaton[x_minus, y_minus, z_plus] == 4)
                neighboursCount++;
            if (mapOfAutomaton[x_minus, y_plus, z_minus] == 4)
                neighboursCount++;
            if (mapOfAutomaton[x_minus, y_plus, z_plus] == 4)
                neighboursCount++;

            if (mapOfAutomaton[x_plus, y_minus, z_minus] == 4)
                neighboursCount++;
            if (mapOfAutomaton[x_plus, y_minus, z_plus] == 4)
                neighboursCount++;
            if (mapOfAutomaton[x_plus, y_plus, z_minus] == 4)
                neighboursCount++;
            if (mapOfAutomaton[x_plus, y_plus, z_plus] == 4)
                neighboursCount++;
        }
        return neighboursCount;
    }

    private enum Neighbourhood
    {
        Moore,
        VonNeumann
    }

    public void InitializeStartMap(int[,,] map)
    {
        int startX = (sizeX - map.GetLength(2)) / 2;
        int startY = (sizeY - map.GetLength(1)) / 2;
        int startZ = (sizeZ - map.GetLength(0)) / 2;

        for (int x = 0; x < map.GetLength(2); x++)
        {
            for (int y = 0; y < map.GetLength(1); y++)
            {
                for (int z = 0; z < map.GetLength(0); z++)
                {
                    if (map[x, y, z] == 1)
                    {
                        mapOfAutomaton[startX + x, startY + y, startZ + z] = 4;
                        mapOfAutomatonCubes[startX + x, startY + y, startZ + z].MakeVisible();
                    }
                    else
                    {
                        mapOfAutomaton[startX + x, startY + y, startZ + z] = 0;
                        mapOfAutomatonCubes[startX + x, startY + y, startZ + z].MakeInvisible();
                    }
                }
            }
        }
    }

    public void InitializeStartHaos()
    {
        Random rnd = new Random();
        for (int i = 5; i < sizeX - 5; i++)
        {
            for (int j = 5; j < sizeY - 5; j++)
            {
                for (int k = 5; k < sizeZ; k++)
                {
                    if (rnd.Next() % 2 == 0)
                    {
                        mapOfAutomatonCubes[i, j, k].MakeVisible();
                        mapOfAutomaton[i, j, k] = 4;
                    }
                    else
                    {
                        mapOfAutomatonCubes[i, j, k].MakeInvisible();
                        mapOfAutomaton[i, j, k] = 0;
                    }
                }
            }
        }
    }

    public Cube CreateCube(Point a, Point b, Point c, Point d, Point e, Point f, Point g, Point h)
    {
        //Front
        Triangle t1 = new Triangle(a, b, d);
        Triangle t2 = new Triangle(b, c, d);

        //Back
        Triangle t3 = new Triangle(e, f, h);
        Triangle t4 = new Triangle(f, g, h);

        //Top
        Triangle t5 = new Triangle(b, c, f);
        Triangle t6 = new Triangle(c, f, g);

        //Bottom
        Triangle t7 = new Triangle(a, e, d);
        Triangle t8 = new Triangle(d, e, h);

        //Right
        Triangle t9 = new Triangle(c, d, h);
        Triangle t10 = new Triangle(c, g, h);

        //Left
        Triangle t11 = new Triangle(a, b, f);
        Triangle t12 = new Triangle(a, e, f);

        Cube cube = new Cube();
        cube.Triangles.Add(t1);
        cube.Triangles.Add(t2);
        cube.Triangles.Add(t3);
        cube.Triangles.Add(t4);
        cube.Triangles.Add(t5);
        cube.Triangles.Add(t6);
        cube.Triangles.Add(t7);
        cube.Triangles.Add(t8);
        cube.Triangles.Add(t9);
        cube.Triangles.Add(t10);
        cube.Triangles.Add(t11);
        cube.Triangles.Add(t12);

        Triangles.AddRange(cube.Triangles);
        // Cubes.Add(cube);
        return cube;
    }
}

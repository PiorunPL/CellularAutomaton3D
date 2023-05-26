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
        new int[] { 255, 0, 0 },
        new int[] { 255, 255, 0 },
        new int[] { 0, 255, 0 },
        new int[] { 0, 255, 255 },
        new int[] { 0, 0, 255 }
    };

    public WorldCellularAutomaton(int x, int y, int z)
    {
        mapOfAutomatonCubes = new Cube[x, y, z];
        mapOfAutomaton = new int[x, y, z];
        sizeX = x;
        sizeY = y;
        sizeZ = z;

        SetUpAutomatonWorld();
        InitializeStartMap(Maps.map1);
        // InitializeStartHaos();
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
                    Cube cube = CreateCube(
                        tempPoints[i, j, k],
                        tempPoints[i, j + 1, k],
                        tempPoints[i + 1, j + 1, k],
                        tempPoints[i + 1, j, k],
                        tempPoints[i, j, k + 1],
                        tempPoints[i, j + 1, k + 1],
                        tempPoints[i + 1, j + 1, k + 1],
                        tempPoints[i + 1, j, k + 1]
                    );
                    Cubes.Add(cube);
                    cube.MakeInvisible();
                    mapOfAutomatonCubes[i, j, k] = cube;
                }
            }
        }
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

        int age;
        for (int x = 0; x < sizeX; x++)
        {
            for (int y = 0; y < sizeY; y++)
            {
                for (int z = 0; z < sizeZ; z++)
                {
                    if (tmpMap[x, y, z] == 1)
                    {
                        mapOfAutomaton[x, y, z] += 1;
                        age = mapOfAutomaton[x, y, z] > 5 ? 5 : mapOfAutomaton[x, y, z];
                        mapOfAutomatonCubes[x, y, z].MakeVisible();
                        mapOfAutomatonCubes[x, y, z].SetColor(colors[age - 1]);
                    }
                    else
                    {
                        mapOfAutomatonCubes[x, y, z].MakeInvisible();
                        mapOfAutomaton[x, y, z] = 0;
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
                        if (tmpMap[x, y, z] == 1)
                        {
                            mapOfAutomaton[x, y, z] += 1;
                            int age = mapOfAutomaton[x, y, z] > 5 ? 5 : mapOfAutomaton[x, y, z];
                            mapOfAutomatonCubes[x, y, z].MakeVisible();
                            mapOfAutomatonCubes[x, y, z].SetColor(colors[age - 1]);
                        }
                        else
                        {
                            mapOfAutomatonCubes[x, y, z].MakeInvisible();
                            mapOfAutomaton[x, y, z] = 0;
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
        int neighboursCount = CountNeighbours(x, y, z, Neighbourhood.Moore);

        // if (mapOfAutomaton[x, y, z] > 0)
        // {
        //     if (neighboursCount == 4 || neighboursCount == 5)
        //         tmpMap[x, y, z] = 1;
        //     else
        //         tmpMap[x, y, z] = 0;
        // }
        // else
        // {
        //     if (neighboursCount == 4)
        //         tmpMap[x, y, z] = 1;
        // }
        if (neighboursCount == 3)
        {
            // tmpMap[x, y, z] = 1;
            return 1;
        }
        return 0;
    }

    private int CountNeighbours(int x, int y, int z, Neighbourhood n)
    {
        int neighboursCount = 0;

        if (x > 0 && mapOfAutomaton[x - 1, y, z] > 0)
            neighboursCount++;
        if (x < sizeX - 1 && mapOfAutomaton[x + 1, y, z] > 0)
            neighboursCount++;
        if (y > 0 && mapOfAutomaton[x, y - 1, z] > 0)
            neighboursCount++;
        if (y < sizeY - 1 && mapOfAutomaton[x, y + 1, z] > 0)
            neighboursCount++;
        if (z > 0 && mapOfAutomaton[x, y, z - 1] > 0)
            neighboursCount++;
        if (z < sizeZ - 1 && mapOfAutomaton[x, y, z + 1] > 0)
            neighboursCount++;

        if (n == Neighbourhood.Moore)
        {
            if (x > 0 && y > 0 && mapOfAutomaton[x - 1, y - 1, z] > 0)
                neighboursCount++;
            if (x > 0 && y < sizeY - 1 && mapOfAutomaton[x - 1, y + 1, z] > 0)
                neighboursCount++;
            if (x > 0 && z > 0 && mapOfAutomaton[x - 1, y, z - 1] > 0)
                neighboursCount++;
            if (x > 0 && z < sizeZ - 1 && mapOfAutomaton[x - 1, y, z + 1] > 0)
                neighboursCount++;

            if (x < sizeX - 1 && y > 0 && mapOfAutomaton[x + 1, y - 1, z] > 0)
                neighboursCount++;
            if (x < sizeX - 1 && y < sizeY - 1 && mapOfAutomaton[x + 1, y + 1, z] > 0)
                neighboursCount++;
            if (x < sizeX - 1 && z > 0 && mapOfAutomaton[x + 1, y, z - 1] > 0)
                neighboursCount++;
            if (x < sizeX - 1 && z < sizeZ - 1 && mapOfAutomaton[x + 1, y, z + 1] > 0)
                neighboursCount++;

            if (y > 0 && z > 0 && mapOfAutomaton[x, y - 1, z - 1] > 0)
                neighboursCount++;
            if (y > 0 && z < sizeZ - 1 && mapOfAutomaton[x, y - 1, z + 1] > 0)
                neighboursCount++;
            if (y < sizeY - 1 && z > 0 && mapOfAutomaton[x, y + 1, z - 1] > 0)
                neighboursCount++;
            if (y < sizeY - 1 && z < sizeZ - 1 && mapOfAutomaton[x, y + 1, z + 1] > 0)
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
                        mapOfAutomaton[startX + x, startY + y, startZ + z] = 1;
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
        for (int i = 0; i < sizeX; i++)
        {
            for (int j = 0; j < sizeY; j++)
            {
                for (int k = 0; k < sizeZ; k++)
                {
                    if (rnd.Next() % 10 == 0)
                    {
                        mapOfAutomatonCubes[i, j, k].MakeVisible();
                        mapOfAutomaton[i, j, k] = 1;
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

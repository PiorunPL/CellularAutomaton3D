using MainProject.Domain.Basic;

namespace MainProject.Domain.WorldRelated;

public class WorldCellularAutomaton
{
    public List<Triangle> Triangles = new List<Triangle>();
    public List<Point> Points = new List<Point>();
    public List<Cube> Cubes = new List<Cube>();

    public Cube[,,] mapOfAutomaton;
    public int sizeX;
    public int sizeY;
    public int sizeZ;

    public const double sizeOfSingleCube = 0.5;
    public const double startDistanceFromAutomaton = 10;

    int[] color1 = new int[]{255, 40, 0};
    int[] color2 = new int[]{255, 80, 0};
    int[] color3 = new int[]{255, 120, 0};
    int[] color4 = new int[]{255, 160, 0};
    int[] color5 = new int[]{255, 200, 0};
    int[] color6 = new int[]{255, 240, 0};

    public WorldCellularAutomaton(int x, int y, int z)
    {
        mapOfAutomaton = new Cube[x,y,z];
        sizeX = x;
        sizeY = y;
        sizeZ = z;
        
        SetUpAutomatonWorld();
    }

    public void SetUpAutomatonWorld()
    {
        // calculateCoordinates
        double left = - sizeX * sizeOfSingleCube / 2;
        double right = sizeX * sizeOfSingleCube / 2;
        double top = sizeY * sizeOfSingleCube / 2;
        double bottom = -sizeY * sizeOfSingleCube / 2;
        double front = startDistanceFromAutomaton;
        double back = startDistanceFromAutomaton + sizeZ * sizeOfSingleCube;

        Point[,,] tempPoints = new Point[sizeX+1,sizeY+1,sizeZ+1];
        for (int i = 0; i < sizeX+1; i++)
        {
            for (int j = 0; j < sizeY+1; j++)
            {
                for (int k = 0; k < sizeZ+1; k++)
                {
                    Point3D point3d = new Point3D(left + i*sizeOfSingleCube, bottom + j*sizeOfSingleCube, front + k*sizeOfSingleCube);
                    Point point = new Point(point3d);
                    tempPoints[i, j, k] = point;
                    Points.Add(point);
                }
            }
        }

        Random rnd = new Random();
        
        for (int i = 0; i < sizeX; i++)
        {
            for (int j = 0; j < sizeY; j++)
            {
                for (int k = 0; k < sizeZ; k++)
                {
                    Cube cube = CreateCube(
                        tempPoints[i,j,k],
                        tempPoints[i,j+1,k],
                        tempPoints[i+1,j+1,k],
                        tempPoints[i+1,j,k],
                        tempPoints[i,j,k+1],
                        tempPoints[i,j+1,k+1],
                        tempPoints[i+1,j+1,k+1],
                        tempPoints[i+1,j,k+1]);
                    Cubes.Add(cube);
                    if(rnd.Next() % 15 == 0)
                        cube.MakeVisible();
                    else
                        cube.MakeInvisible();
                    mapOfAutomaton[i, j, k] = cube;
                }
            }
        }
        
        
    }
    public Cube CreateCube(Point a, Point b, Point c, Point d, Point e, Point f, Point g, Point h)
    {
        //Front
        Triangle t1 = new Triangle(a, b, d, color1);
        Triangle t2 = new Triangle(b, c, d, color1);
        
        //Back
        Triangle t3 = new Triangle(e, f, h, color2);
        Triangle t4 = new Triangle(f, g, h, color2);
        
        //Top
        Triangle t5 = new Triangle(b, c, f, color3);
        Triangle t6 = new Triangle(c, f, g, color3);
        
        //Bottom
        Triangle t7 = new Triangle(a, e, d, color4);
        Triangle t8 = new Triangle(d, e, h, color4);
        
        //Right
        Triangle t9 = new Triangle(c, d, h, color5);
        Triangle t10 = new Triangle(c, g, h, color5);
        
        //Left
        Triangle t11 = new Triangle(a, b, f, color6);
        Triangle t12 = new Triangle(a, e, f, color6);

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
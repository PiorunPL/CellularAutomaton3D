using MainProject.Domain.Basic;
using MainProject.Domain.BSPTree;

namespace MainProject.Utility;

public class PainingAlgorithOrder
{
    public List<List<Triangle>> Order = new List<List<Triangle>>();
    public void CreateTrianglesOrder(Node? BSPTree)
    {
        if(BSPTree == null)
        {
            return;
        }
        // Console.WriteLine("CreateTrianglesOrder size of bsptree " + BSPTree.Triangles.Count);
        Triangle t = BSPTree.Plane;
        double position = t.CheckPointPosition(new Point3D(0, 0, 0));
        if(position < 0)
        {
            CreateTrianglesOrder(BSPTree.Back);
            Order.Add(BSPTree.Triangles);
            CreateTrianglesOrder(BSPTree.Front);
        }
        else
        {
            CreateTrianglesOrder(BSPTree.Front);
            Order.Add(BSPTree.Triangles);
            CreateTrianglesOrder(BSPTree.Back);
        }
    }
}
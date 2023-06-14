using MainProject.Domain.Basic;

namespace MainProject.Utility;

public static class TrianglesChooser
{
    public static List<List<Triangle>> ChooseOnlyTrianglesAhead(List<List<Triangle>> triangles)
    {
        List<List<Triangle>> chosenTriangles = new();
        List<List<Triangle>> workingTriangles = triangles.ToList();

        Triangle cameraPlane = CreateCameraPlane();

        foreach (var currentTriangles in workingTriangles)
        {
            var tempTriangles = new List<Triangle>();
            
            foreach (var triangle in currentTriangles)
            {
                int pointsOfTriangleAhead = GetNumberOfPointsOfTriangleInFrontOfCamera(triangle);
            
            if(pointsOfTriangleAhead == 3)
                tempTriangles.Add(triangle);
            else if (pointsOfTriangleAhead == 2)
            {
                BSPTreeBuilder.TrianglePosition position =
                    BSPTreeBuilder.CheckTrianglePosition(triangle, cameraPlane);

                if (position == BSPTreeBuilder.TrianglePosition.ToDivideHard)
                {
                    Point sideA1, sideA2, sideB1;
                                    (sideA1, sideA2, sideB1) = BSPTreeBuilder.GetPreparedPointsForHardDivide(triangle, cameraPlane);
                                    
                                    Point3D intersectionPoint1Position = BSPTreeBuilder.GetPoinfOfIntersection(sideA1.CurrentPosition, sideB1.CurrentPosition, cameraPlane);
                                    Point3D intersectionPoint1OriginalPosition =
                                        BSPTreeBuilder.GetPoinfOfIntersection(sideA1.OriginalPosition, sideB1.OriginalPosition, cameraPlane);
                                    Point intersectionPoint1 = new Point(intersectionPoint1OriginalPosition, intersectionPoint1Position);
                            
                                    Point3D intersectionPoint2Position = BSPTreeBuilder.GetPoinfOfIntersection(sideA2.CurrentPosition, sideB1.CurrentPosition, cameraPlane);
                                    Point3D intersectionPoint2OriginalPosition =
                                        BSPTreeBuilder.GetPoinfOfIntersection(sideA2.OriginalPosition, sideB1.OriginalPosition, cameraPlane);
                                    Point intersectionPoint2 = new Point(intersectionPoint2OriginalPosition, intersectionPoint2Position);
                                    
                                    Triangle t1 = new Triangle(sideA1, intersectionPoint1, intersectionPoint2);
                                    t1.color = triangle.color;
                                    
                                    Triangle t2 = new Triangle(sideA1, sideA2, intersectionPoint2);
                                    t2.color = triangle.color;
                                    
                                    tempTriangles.Add(t1);
                                    tempTriangles.Add(t2);
                }
                else
                {
                    tempTriangles.Add(triangle);
                }
            }
            else if (pointsOfTriangleAhead == 1)
            {
                BSPTreeBuilder.TrianglePosition position =
                    BSPTreeBuilder.CheckTrianglePosition(triangle, cameraPlane);
                if (position == BSPTreeBuilder.TrianglePosition.ToDivideEasy)
                {
                    Point front, back, plane;
                    (front, back, plane) = BSPTreeBuilder.GetPreparedPointsForEasyDivide(triangle, cameraPlane);

                    Point3D intersectionPointPosition = BSPTreeBuilder.GetPoinfOfIntersection(front.CurrentPosition, back.CurrentPosition, cameraPlane);
                    Point3D intersectionPointOriginalPosition =
                        BSPTreeBuilder.GetPoinfOfIntersection(front.OriginalPosition, back.OriginalPosition, cameraPlane);
                    Point intersectionPoint = new Point(intersectionPointOriginalPosition, intersectionPointPosition);

                    Triangle t1 = new Triangle(front, plane, intersectionPoint);
                    t1.color = triangle.color;
                    
                    tempTriangles.Add(t1);
                }
                else
                {
                    Point sideA1, sideA2, sideB1;
                    (sideA1, sideA2, sideB1) = BSPTreeBuilder.GetPreparedPointsForHardDivide(triangle, cameraPlane);

                    Point3D intersectionPoint1Position = BSPTreeBuilder.GetPoinfOfIntersection(sideA1.CurrentPosition, sideB1.CurrentPosition, cameraPlane);
                    Point3D intersectionPoint1OriginalPosition =
                        BSPTreeBuilder.GetPoinfOfIntersection(sideA1.OriginalPosition, sideB1.OriginalPosition, cameraPlane);
                    Point intersectionPoint1 = new Point(intersectionPoint1OriginalPosition, intersectionPoint1Position);
        
                    Point3D intersectionPoint2Position = BSPTreeBuilder.GetPoinfOfIntersection(sideA2.CurrentPosition, sideB1.CurrentPosition, cameraPlane);
                    Point3D intersectionPoint2OriginalPosition =
                        BSPTreeBuilder.GetPoinfOfIntersection(sideA2.OriginalPosition, sideB1.OriginalPosition, cameraPlane);
                    Point intersectionPoint2 = new Point(intersectionPoint2OriginalPosition, intersectionPoint2Position);

                    Triangle t = new Triangle(intersectionPoint1, intersectionPoint2, sideB1);
                    t.color = triangle.color;
                    
                    tempTriangles.Add(t);
                }
            } 
            }
            
            chosenTriangles.Add(tempTriangles);
        }

        return chosenTriangles;
    }

    private static int GetNumberOfPointsOfTriangleInFrontOfCamera(Triangle currentTriangle)
    {
        int result = 0;
        if (currentTriangle.P1.CurrentPosition.Z > 0.001)
            result++;
        if (currentTriangle.P2.CurrentPosition.Z > 0.001)
            result++;
        if (currentTriangle.P3.CurrentPosition.Z > 0.001)
            result++;

        return result;
    }

    public static Triangle CreateCameraPlane()
    {
        Point p1 = new Point(-1,-1,0.001);
        Point p2 = new Point(1,-1,0.001);
        Point p3 = new Point(0,1,0.001);
        
        Triangle result = new Triangle(p1,p2,p3);
        return result;
    }
}
using MainProject.Domain.Basic;
using MainProject.Utility;
using SkiaSharp;

namespace MainProject.Domain.CameraRelated;

public class Camera
{
    public ViewPort ViewPort = new ViewPort();
    // public List<Point2D> ViewPortPoints = new List<Point2D>();
    public List<Line> Lines = new List<Line>();
    public List<List<Triangle>> Triangles = new List<List<Triangle>>();
    public BitmapUtil BitmapUtil;
    public SKCanvas Canvas;

    public Camera(SKCanvas canvas)
    {
        BitmapUtil = new BitmapUtil(ViewPort);
        Canvas = canvas;
    }

    public SKBitmap CreatePhoto()
    {
        return BitmapUtil.GetBitmapFromLines(Lines);
    }

    public void CreatePhotoTriangles()
    {
        BitmapUtil.GetBitmapFromTriangles(Canvas ,Triangles);
    }

    public void PassActualWorld(List<Line> lines)
    {
        Lines.Clear();
        Lines.AddRange(lines);
    }

    public void PassActualWorld(List<List<Triangle>> triangles)
    {
        Triangles.Clear();
        Triangles.AddRange(triangles);
    }
}
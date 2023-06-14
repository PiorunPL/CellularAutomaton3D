namespace MainProject.Domain.Basic;

public class Cube
{
    public List<Triangle> Triangles = new List<Triangle>();
    private bool _isVisible = false;

    public void MakeVisible()
    {
        if (_isVisible)
            return;

        _isVisible = true;
        foreach (var triangle in Triangles)
        {
            triangle.isVisible = !triangle.isVisible;
        }
    }

    public void MakeInvisible()
    {
        if (!_isVisible)
            return;

        _isVisible = false;
        foreach (var triangle in Triangles)
        {
            triangle.isVisible = !triangle.isVisible;
        }
    }

    public void SetColor(int[] color)
    {
        foreach (var triangle in Triangles)
        {
            triangle.SetColor(color);
        }
    }

    public bool IsVisible()
    {
        return _isVisible;
    }
}

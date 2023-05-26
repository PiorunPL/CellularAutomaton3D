namespace MainProject.Domain.Basic;

public class Cube
{
    public List<Triangle> Triangles = new List<Triangle>();
    private bool _isVisible = true;

    public void MakeVisible()
    {
        if (_isVisible)
            return;

        _isVisible = true;
        foreach (var triangle in Triangles)
        {
            triangle.isVisible = true;
        }
    }

    public void MakeInvisible()
    {
        if (!_isVisible)
            return;

        _isVisible = false;
        foreach (var triangle in Triangles)
        {
            triangle.isVisible = false;
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

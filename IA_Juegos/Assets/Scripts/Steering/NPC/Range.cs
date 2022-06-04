using System;

public struct Range
{
    private float _start;
    private float _end;

    public Range(float start, float end)
    {
        _start = start;
        _end = end;
    }

    public float Start
    {
        get { return _start; }
        set { _start = value; }
    }

    public float End
    {
        get { return _end; }
        set { _end = value; }
    }

    public float Length
    {
        get { return _end - _start; }
    }

    public float MidPoint
    {
        get { return (Math.Abs(_end) - Math.Abs(_start)) / 2f; }
    }
}
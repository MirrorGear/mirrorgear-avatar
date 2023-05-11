using System;

public enum Gender
{
    female = 0,
    male = 1,
}

[Serializable]
public class CustomColor
{
    public int R;
    public int G;
    public int B;

    public CustomColor(int r, int g, int b)
    {
        R = r;
        G = g;
        B = b;
    }
}
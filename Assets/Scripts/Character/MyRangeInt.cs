[System.Serializable]
public struct MyRangeInt{
    public int max;
    public int min;
    public int value;

    public void AddValue(int val)
    {
        value += val;
        if (value > max) value = max;
        if (value < min) value = min;
    }

    public void SetValue(int val)
    {
        value = val;
        if (value > max) value = max;
        if (value < min) value = min;
    }

    public MyRangeInt(int max_ = 0, int min_ = 0)
    {
        max = max_;
        min = min_;
        if (min > max)
        {
            UnityEngine.Debug.LogError("min > max");
        }
        value = max_;
    }
}

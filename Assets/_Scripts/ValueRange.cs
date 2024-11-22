namespace WMG
{
    [System.Serializable]
    public struct ValueRange
    {
        public float Min;
        public float Max;

        public ValueRange(float min, float max)
        {
            Min = min;
            Max = max;
        }
    }
}

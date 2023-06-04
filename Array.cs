public class FastArray
{
    public FastArray(double[] value)
    {
        Value = value;
        unsafe
        {
            fixed (double* pointer = value)
            {
                Pointer = (IntPtr)pointer;
            }
        }
    }

    private double[] Value { get; }
    private IntPtr Pointer;
    public double this[int index]
    {
        get
        {
            unsafe
            {
                fixed(double* pointer = Value)
                {
                    return *(pointer + index);
                }
            }
        }
        set
        {
            unsafe
            {
                fixed(double* pointer = Value)
                {
                    *(pointer + index) = value;
                }
            }
        }
    }
}
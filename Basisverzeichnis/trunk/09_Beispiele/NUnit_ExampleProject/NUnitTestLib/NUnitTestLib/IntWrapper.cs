namespace NUnitTestLib
{
    public class IntWrapper
    {
        public IntWrapper(int value)
        {
            this.Value = value;
        }

        public int Value { get; private set; }
    }
}
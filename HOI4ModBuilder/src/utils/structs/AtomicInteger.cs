using System.Threading;

namespace HOI4ModBuilder.src.utils.structs
{
    public class AtomicInteger
    {
        private int _value;
        public int Increment() => Interlocked.Increment(ref _value);
        public int Decrement() => Interlocked.Decrement(ref _value);
        public int Get() => Volatile.Read(ref _value);
        public void Reset() => Volatile.Write(ref _value, 0);
        public void SetValue(int value) => Volatile.Write(ref _value, value);

        public override string ToString() => _value.ToString();
    }
}

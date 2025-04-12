namespace HOI4ModBuilder.src.utils.structs
{

    public class LinkedData<T>
    {
        public T data;
        public LinkedData<T> prev, next;
    }
}

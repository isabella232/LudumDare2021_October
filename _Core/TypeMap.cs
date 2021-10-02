using System;
using System.Collections.Generic;

public sealed class General_TypeMap
{
    static int newID;
    class TypeData<T>
    {
        public static readonly int ID = newID++;
        public T data;
    }
    Dictionary<int, object> map = new Dictionary<int, object>();
    public ref T Get<T>()
    {
        if (!map.TryGetValue(TypeData<T>.ID, out var val))
            map[TypeData<T>.ID] = val = new TypeData<T>();
        return ref ((TypeData<T>)val).data;
    }
}

public sealed class TypeMap
{
    static int newID;
    class TypeData<T> where T: new()
    {
        public static readonly int ID = newID++;
        public T data = new T();
    }
    Dictionary<int, object> map = new Dictionary<int, object>();
    
    /// <summary>
    /// returns the reference to T
    /// </summary>
    public ref T Get<T>() where T: new()
    {
        if (!map.TryGetValue(TypeData<T>.ID, out var val))
            map[TypeData<T>.ID] = val = new TypeData<T>();
        return ref ((TypeData<T>)val).data;
    }

    /// <summary>
    /// returns the stored value of T
    /// </summary>
    public T Read<T>() where T: struct
        => Get<T>();
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainObjects.Operations
{
    public interface IEmptyable
    {
        bool HasValue { get; }
        bool IsEmpty { get; }
        object Value { get; }
        Type UnderlyingType { get; }
        Emptyable<Tout> ConvertTo<Tout>();
        IEmptyable ConvertTo(Type type);
    }

    [System.Diagnostics.DebuggerDisplay("{IsEmpty ? this : (object)Value}")]
    //[System.Diagnostics.DebuggerTypeProxy(typeof(Emptyable<>.EmptyableDebuggerTypeProxy))]
    public struct Emptyable<T> : IEmptyable
    {
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private readonly T value;

        public bool HasValue { get; private set; }
        public bool IsEmpty => !HasValue;
        public T Value => value;

        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        object IEmptyable.Value => value;

        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        Type IEmptyable.UnderlyingType => typeof(T);

        public Emptyable(T v)
        {
            value = v;
            HasValue = true;
        }

        public static implicit operator Emptyable<T>(T value)
        {
            return new Emptyable<T>(value);
        }

      

        public static explicit operator T(Emptyable<T> value)
        {
            return value.Value;
        }

        public T GetValueOrDefault()
        {
            return value;
        }

        public T GetValueOrDefault(T defaultValue)
        {
            return HasValue ? value : defaultValue;
        }

        public override bool Equals(object other)
        {
            if (!HasValue) return other == null;
            if (other == null) return false;
            return value.Equals(other);
        }

        public override int GetHashCode()
        {
            return HasValue ? value.GetHashCode() : 0;
        }

        public override string ToString()
        {
            return HasValue ? value.ToString() : "";
        }

        public Emptyable<Tout> ConvertTo<Tout>()
        {
            Emptyable<Tout> e = new Emptyable<Tout>();
            if (!IsEmpty)
            {
                if (typeof(T) == typeof(Tout))
                    e = new Emptyable<Tout>(((Tout)(object)value));
                else if (typeof(T) != typeof(Tout))
                    e = new Emptyable<Tout>((Tout)Convert.ChangeType(value, typeof(T)));
            }

            return e;
        }

        public IEmptyable ConvertTo(Type type)
        {
            if (!IsEmpty)
            {
                if (typeof(T) == type)
                    return Emptyable.Create(value);
                else
                    return Emptyable.Create(Convert.ChangeType(value, type));
            }
            else
                return Emptyable.Create(type);
        }

        //internal class EmptyableDebuggerTypeProxy
        //{
        //    private Emptyable<T> e;
        //    public EmptyableDebuggerTypeProxy(Emptyable<T> e)
        //    {
        //        this.e = e;
        //    }

        //    [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        //    public string Description
        //    {
        //        get
        //        {
        //            return e.IsEmpty ? "{Empty}" : e.value.ToString();
        //        }
        //    }

        //    public T Value
        //    {
        //        get
        //        {
        //            return e.value;
        //        }
        //    }
        //}
    }

    public static class Emptyable
    {
        public static readonly IEmptyable Empty = new Emptyable<object>();
        public static IEmptyable Create(object value)
        {
            if (value == null)
                return new Emptyable<object>(null);
            else
                return (IEmptyable)Activator.CreateInstance(typeof(Emptyable<>).MakeGenericType(new[] { value.GetType() }), new object[] { value });
        }

        public static IEmptyable Create(Type type)
        {
            return (IEmptyable)Activator.CreateInstance(typeof(Emptyable<>).MakeGenericType(new[] { type }), new object[] { });
        }

        public static Type GetUnderlyingType(Type emptyableTyle)
        {
            if (!emptyableTyle.IsGenericType || emptyableTyle.GetGenericTypeDefinition() != typeof(Emptyable<>))
                throw new ArgumentException("Type " + emptyableTyle.Name + " is not Emptyable<>");

            return emptyableTyle.GetGenericArguments().First();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainObjects.Operations
{
    //public sealed class OperationStackBuilder
    //{
    //    Func<object> initialStateBuilder = () => null;
    //    OperationStackOptions options = new OperationStackOptions();
    //    public OperationStackBuilder<object, object, OperationEvent> WithOptions(OperationStackOptions options)
    //    {
    //        return new OperationStackBuilder<object, object, OperationEvent>(options, initialStateBuilder);
    //    }

    //    public OperationStackBuilder<T,object,OperationEvent> WithInput<T>()
    //    {
    //        return new OperationStackBuilder<T, object, OperationEvent>(options, initialStateBuilder);
    //    }

    //    public OperationStackBuilder<object, T, OperationEvent> WithState<T>(Func<T> initialStateBuilder)
    //    {
    //        return new OperationStackBuilder<object, T, OperationEvent>(options, initialStateBuilder);
    //    }

    //    public OperationStackBuilder<object, object, T> WithEvent<T>()
    //        where T : OperationEvent
    //    {
    //        return new OperationStackBuilder<object, object, T>(options, initialStateBuilder);
    //    }

    //    public OperationStack<object, object, OperationEvent> Build()
    //    {
    //        //TODO: Add initial state
    //        return new OperationStack<object, object, OperationEvent>(options, initialStateBuilder);
    //    }
    //}


    public class OperationStackBuilder<TInput, TState, TOperationEvent> 
        where TOperationEvent : OperationEvent
    {
        Func<TState> initialStateBuilder = () => default(TState);
        OperationStackOptions options = new OperationStackOptions();

        public OperationStackBuilder()
        {

        }
        internal OperationStackBuilder(OperationStackOptions options, Func<TState> initialStateBuilder)
        {
            this.options = options;
            this.initialStateBuilder = initialStateBuilder;
        }

        public OperationStackBuilder<TInput, TState, TOperationEvent> WithOptions(OperationStackOptions options)
        {
            return new OperationStackBuilder<TInput, TState, TOperationEvent>(options, initialStateBuilder);
        }

        public OperationStackBuilder<T, TState, TOperationEvent> WithInput<T>()
        {
            return new OperationStackBuilder<T, TState, TOperationEvent>(options, initialStateBuilder);
        }

        public OperationStackBuilder<TInput, T, TOperationEvent> WithState<T>(Func<T> initialStateBuilder)
        {
            if (initialStateBuilder == null)
                throw new ArgumentNullException("Initial state builder cannot be null");

            return new OperationStackBuilder<TInput, T, TOperationEvent>(options, initialStateBuilder);
        }

        public OperationStackBuilder<TInput, TState, T> WithEvent<T>()
            where T : OperationEvent
        {
            return new OperationStackBuilder<TInput, TState, T>(options, initialStateBuilder);
        }

        public OperationStack<TInput, TState, TOperationEvent> Build()
        {
            return new OperationStack<TInput, TState, TOperationEvent>(options, initialStateBuilder);
        }
    }

    public class OperationStackBuilder : OperationStackBuilder<object, object, OperationEvent>
    {
        public OperationStackBuilder()
        {

        }
        internal OperationStackBuilder(OperationStackOptions options, Func<object> initialStateBuilder) : base(options, initialStateBuilder)
        {
        }
    }

    public class OperationStackBuilder<TOperationEvent> : OperationStackBuilder<object, object, TOperationEvent>
        where TOperationEvent : OperationEvent
    {
        public OperationStackBuilder()
        {

        }
        internal OperationStackBuilder(OperationStackOptions options, Func<object> initialStateBuilder) : base(options, initialStateBuilder)
        {
        }
    }
}

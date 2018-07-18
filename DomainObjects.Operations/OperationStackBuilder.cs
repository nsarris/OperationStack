using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainObjects.Operations
{
    public class OperationStackBuilder<TInput, TState, TOperationEvent> 
        where TOperationEvent : OperationEvent
    {
        Func<TState> initialStateBuilder = () => default(TState);
        OperationStackOptions options = new OperationStackOptions();
        bool hasInput = false;

        public OperationStackBuilder()
        {

        }
        internal OperationStackBuilder(OperationStackOptions options, Func<TState> initialStateBuilder, bool hasInput)
        {
            this.options = options;
            this.initialStateBuilder = initialStateBuilder;
            this.hasInput = hasInput;
        }

        public OperationStackBuilder<TInput, TState, TOperationEvent> WithOptions(OperationStackOptions options)
        {
            return new OperationStackBuilder<TInput, TState, TOperationEvent>(options, initialStateBuilder, hasInput);
        }

        public OperationStackBuilder<T, TState, TOperationEvent> WithInput<T>()
        {
            return new OperationStackBuilder<T, TState, TOperationEvent>(options, initialStateBuilder, true);
        }

        public OperationStackBuilder<TInput, T, TOperationEvent> WithState<T>(Func<T> initialStateBuilder)
        {
            if (initialStateBuilder == null)
                throw new ArgumentNullException("Initial state builder cannot be null");

            return new OperationStackBuilder<TInput, T, TOperationEvent>(options, initialStateBuilder,hasInput);
        }

        public OperationStackBuilder<TInput, TState, T> WithEvent<T>()
            where T : OperationEvent
        {
            return new OperationStackBuilder<TInput, TState, T>(options, initialStateBuilder, hasInput);
        }

        public OperationStack<TInput, TState, TOperationEvent, IVoid> Build()
        {
            return new OperationStack<TInput, TState, TOperationEvent, IVoid>(new List<StackBlockSpecBase<TInput, TState, TOperationEvent>>(), options, initialStateBuilder, hasInput);
        }
    }

    //public abstract class OperationStackBuilder<TInput, TState, TOperationEvent, TOut, TOperationStack>
    //    where TOperationStack : OperationStack<TInput, TState, TOperationEvent, TOut>
    //    where TOperationEvent : OperationEvent
    //{
    //    public abstract TOperationStack Build();
    //}

    public class OperationStackBuilder : OperationStackBuilder<object, object, OperationEvent>
    {
        public OperationStackBuilder()
        {

        }
        internal OperationStackBuilder(OperationStackOptions options, Func<object> initialStateBuilder) 
            : base(options, initialStateBuilder,false)
        {
        }
    }

    public class OperationStackBuilder<TOperationEvent> : OperationStackBuilder<object, object, TOperationEvent>
        where TOperationEvent : OperationEvent
    {
        public OperationStackBuilder()
        {

        }
        internal OperationStackBuilder(OperationStackOptions options, Func<object> initialStateBuilder) 
            : base(options, initialStateBuilder, false)
        {
        }
    }
}

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
        private readonly Func<TState> initialStateBuilder = () => default(TState);
        private readonly OperationStackOptions options = new OperationStackOptions();
        private readonly bool hasInput = false;

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
                throw new ArgumentNullException(nameof(initialStateBuilder), "Initial state builder cannot be null");

            return new OperationStackBuilder<TInput, T, TOperationEvent>(options, initialStateBuilder,hasInput);
        }

        public OperationStackBuilder<TInput, TState, T> WithEvent<T>()
            where T : OperationEvent
        {
            return new OperationStackBuilder<TInput, TState, T>(options, initialStateBuilder, hasInput);
        }

        public OperationStack<TInput, TState, TOperationEvent> Build()
        {
            return new OperationStack<TInput, TState, TOperationEvent>(options, initialStateBuilder, hasInput);
        }
    }

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

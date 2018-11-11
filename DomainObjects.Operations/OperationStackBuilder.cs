using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainObjects.Operations
{
    public class OperationStackBuilder<TInput, TState> 
        
    {
        readonly Func<TState> initialStateBuilder = () => default(TState);
        readonly OperationStackOptions options = new OperationStackOptions();
        readonly bool hasInput = false;

        public OperationStackBuilder()
        {

        }
        internal OperationStackBuilder(OperationStackOptions options, Func<TState> initialStateBuilder, bool hasInput)
        {
            this.options = options;
            this.initialStateBuilder = initialStateBuilder;
            this.hasInput = hasInput;
        }

        public OperationStackBuilder<TInput, TState> WithOptions(OperationStackOptions options)
        {
            return new OperationStackBuilder<TInput, TState>(options, initialStateBuilder, hasInput);
        }

        public OperationStackBuilder<T, TState> WithInput<T>()
        {
            return new OperationStackBuilder<T, TState>(options, initialStateBuilder, true);
        }

        public OperationStackBuilder<TInput, T> WithState<T>(Func<T> initialStateBuilder)
        {
            if (initialStateBuilder == null)
                throw new ArgumentNullException(nameof(initialStateBuilder), "Initial state builder cannot be null");

            return new OperationStackBuilder<TInput, T>(options, initialStateBuilder,hasInput);
        }

        public OperationStack<TInput, TState, IVoid> Build()
        {
            return new OperationStack<TInput, TState, IVoid>(new List<IStackBlockSpec>(), options, initialStateBuilder, hasInput);
        }
    }

    public class OperationStackBuilder : OperationStackBuilder<object, object>
    {
        public OperationStackBuilder()
        {

        }
        internal OperationStackBuilder(OperationStackOptions options, Func<object> initialStateBuilder) 
            : base(options, initialStateBuilder,false)
        {
        }
    }
}

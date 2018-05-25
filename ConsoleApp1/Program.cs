using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainObjects.Operations;
using System.Threading;
using System.Globalization;
using NUnit.Framework;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Reflection;


namespace ConsoleApp1
{
        
    class MyOperationEvent : OperationEvent
    {
        public MyOperationEvent(Exception exception) : base(exception)
        {
        }

        public MyOperationEvent(string message) : base(message)
        {
        }
    }

    interface IMyCommandOperation<Tin> : ICommandOperationWithInput<Tin, MyOperationEvent>
    {

    }

    class Program
    {
        static void Main(string[] args)
        {
            
        }




    }

    
    class SomeService
    {
        public class Input
        {
            public int X { get; }
            public int Y { get; }
            public Input(int x, int y)
            {
                X = x;
                Y = x;
            }
            public static Input Build(int x, int y)
            {
                return new Input(x, y);
            }
        }
        IQueryOperationWithInput<(int x, int y), MyOperationEvent, int> os;
        IQueryOperationWithInput<(int x, int y), MyOperationEvent, int> GetOS()
        {
            LazyInitializer.EnsureInitialized(ref os, () =>
            new OperationStackBuilder()
                .WithInput<(int x, int y)>()
                .WithEvent<MyOperationEvent>()
                .Build()
                .ThenReturn(op => op.Return(2)));

            return os;
        }


        IQueryOperationWithInput<Input, MyOperationEvent, int> os2;
        IQueryOperationWithInput<Input, MyOperationEvent, int> GetOS2()
        {
            LazyInitializer.EnsureInitialized(ref os2, () =>
            new OperationStackBuilder()
                .WithInput<Input>()
                .WithEvent<MyOperationEvent>()
                .Build()
                .Then(op =>
                {
                    
                })
                .ThenReturn(op => op.Return(2)));

            return os2;
        }


        void Test()
        {
            os.Execute((2, 3));
            var r = os2.Execute(new Input(2, 3));
            var a = os2.Execute(Input.Build(2, 3));

            
        }
    }
}

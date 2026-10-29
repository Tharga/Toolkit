using System;

namespace Tharga.Toolkit.Tests.Compare.Entities
{
    public struct SomeAmount
    {
        public SomeAmount(decimal value)
        {
            Value = value;
        }

        public decimal Value { get; }
        public SomeAmount Absolute => new SomeAmount(Math.Abs(Value));
    }
}

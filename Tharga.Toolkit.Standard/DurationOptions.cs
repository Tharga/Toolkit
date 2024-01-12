using System;

namespace Tharga.Toolkit
{
    public class DurationOptions
    {
        public DateTime? BaseValue { get; set; }
        public EUnit MaxUnit { get; set; } = EUnit.Day;
        public EUnit MinUnit { get; set; } = EUnit.Second;
        public DurationStringOptions StringOptions { get; set; }
    }
}
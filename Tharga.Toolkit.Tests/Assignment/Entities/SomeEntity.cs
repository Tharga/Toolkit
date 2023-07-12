using System;

namespace Tharga.Toolkit.Tests.Assignment.Entities
{
    public class SomeEntity
    {
        public SomeEntity(string text, string textNotInUse, int number, DateTime dateTime, decimal @decimal, double d, SomeGeneric<string> someGeneric, SomeSubDto sub, string[] texts, bool trueOrFalse, Uri uri, TimeSpan timeSpan, ESomeEnum someEnum)
        {
            Text = text;
            Number = number;
            DateTime = dateTime;
            Decimal = @decimal;
            Double = d;
            SomeGeneric = someGeneric;
            Sub = sub;
            Texts = texts;
            TrueOrFalse = trueOrFalse;
            Uri = uri;
            TimeSpan = timeSpan;
            SomeEnum = someEnum;
        }

        public string Text { get; }
        public int Number { get; }
        public DateTime DateTime { get; }
        public decimal Decimal { get; }
        public double Double { get; }
        public SomeGeneric<string> SomeGeneric { get; }
        public SomeSubDto Sub { get; }
        public string[] Texts { get; }
        public bool TrueOrFalse { get; }
        public Uri Uri { get; }
        public TimeSpan TimeSpan { get; }
        public ESomeEnum SomeEnum { get; }
    }
}
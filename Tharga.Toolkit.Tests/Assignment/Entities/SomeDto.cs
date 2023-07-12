using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Tharga.Toolkit.Tests.Assignment.Entities
{
    public class SomeDto
    {
        public string Text { get; set; }
        public int Number { get; set; }
        public DateTime DateTime { get; set; }
        public decimal Decimal { get; set; }
        public double Double { get; set; }
        public string[] Texts1 { get; set; }
        public List<string> Texts2 { get; set; }
        public Collection<string> Texts3 { get; set; }
        public IEnumerable<string> Texts4 { get; set; }
        public Dictionary<string,string> Texts5 { get; set; }
        public IDictionary<string, string> Texts6 { get; set; }
        public IList<string> Texts7 { get; set; }
        public SomeSubDto Sub { get; set; }
        //TODO: Test this assignment: public ISomeSub ISomeSub { get; set; }
        //TODO: Test this assignment: public SomeStruct SomeStruct { get; set; }
        public SomeGeneric<string> SomeGeneric { get; set; }
        public bool TrueOrFalse { get; set; }
        public Uri Uri { get; set; }
        public TimeSpan TimeSpan { get; set; }
        public string StringField;
        public ESomeEnum SomeEnum { get; set; }
    }
}
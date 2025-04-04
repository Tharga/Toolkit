﻿using System.Collections.Generic;

namespace Tharga.Toolkit
{
    public class DurationStringOptions
    {
        public string PreString { get; set; }
        public string PostString { get; set; }
        public Dictionary<EUnit, UnitOption> UnitOptions { get; set; }
        public (EUnit, UnitOption) Millisecond => (EUnit.Millisecond, UnitOptions[EUnit.Millisecond]);
        public (EUnit, UnitOption) Second => (EUnit.Second, UnitOptions[EUnit.Second]);
        public (EUnit, UnitOption) Minute => (EUnit.Minute, UnitOptions[EUnit.Minute]);
        public (EUnit, UnitOption) Hour => (EUnit.Hour, UnitOptions[EUnit.Hour]);
        public (EUnit, UnitOption) Day => (EUnit.Day, UnitOptions[EUnit.Day]);
        public (EUnit, UnitOption) Week => (EUnit.Week, UnitOptions[EUnit.Week]);
        public (EUnit, UnitOption) Month => (EUnit.Month, UnitOptions[EUnit.Month]);
        public (EUnit, UnitOption) Year => (EUnit.Year, UnitOptions[EUnit.Year]);
        public Dictionary<EUnit, string> Now { get; set; }
        public Dictionary<EUnit, string> Resent { get; set; }
        public Dictionary<EUnit, string> Soon { get; set; }
    }
}
using System.Collections.ObjectModel;
using System.Linq;
using Tharga.Toolkit.Tests.Assignment.Entities;

namespace Tharga.Toolkit.Tests.Assignment
{
    public static class Converter
    {
        public static SomeDto ToDto(this SomeEntity entity)
        {
            return new SomeDto
            {
                Text = entity.Text,
                DateTime = entity.DateTime,
                Decimal = entity.Decimal,
                Double = entity.Double,
                Number = entity.Number,
                SomeGeneric = entity.SomeGeneric,
                Sub = entity.Sub,
                Texts1 = entity.Texts,
                Texts2 = entity.Texts.ToList(),
                Texts3 = new Collection<string>(entity.Texts),
                Texts4 = entity.Texts,
                Texts5 = entity.Texts.ToDictionary(x => x, x => x),
                Texts6 = entity.Texts.ToDictionary(x => x, x => x),
                Texts7 = entity.Texts,
                TrueOrFalse = entity.TrueOrFalse,
                Uri = entity.Uri,
                TimeSpan = entity.TimeSpan,
                StringField = entity.Text,
                SomeEnum = entity.SomeEnum
            };
        }

        public static SomeDto ToBrokenDto(this SomeEntity entity)
        {
            return new SomeDto
            {
            };
        }

        public static SomeEntity ToEntity(this SomeDto dto)
        {
            return new SomeEntity(dto.Text, dto.Text, dto.Number, dto.DateTime, dto.Decimal, dto.Double, dto.SomeGeneric, dto.Sub, dto.Texts1, dto.TrueOrFalse, dto.Uri, dto.TimeSpan, dto.SomeEnum);
        }
    }
}
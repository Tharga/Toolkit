using Tharga.Toolkit.Tests.Assignment.Entities;

namespace Tharga.Toolkit.Tests.Assignment
{
    public class ConverterInstance
    {
        public SomeDto ToDto(SomeEntity entity)
        {
            return entity.ToDto();
        }

        public SomeDto ToBrokenDto(SomeEntity entity)
        {
            return entity.ToBrokenDto();
        }

        public SomeEntity ToEntity(SomeDto dto)
        {
            return dto.ToEntity();
        }

        protected SomeEntity ToEntitrProtected(SomeDto dto)
        {
            return dto.ToEntity();
        }

        private SomeEntity ToEntitrPrivate(SomeDto dto)
        {
            return dto.ToEntity();
        }

        public static SomeEntity ToEntitrStaic(SomeDto dto)
        {
            return dto.ToEntity();
        }
    }
}
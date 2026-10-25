namespace Tharga.Toolkit.Tests.Assignment.Entities
{
    public struct SomeStruct
    {
        public SomeStruct(string text, int number)
        {
            Text = text;
            Number = number;
        }

        public string Text { get; }
        public int Number { get; }
    }
}
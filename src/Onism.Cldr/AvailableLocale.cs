namespace Onism.Cldr
{
    public class AvailableLocale
    {
        public string Code { get; }
        public bool IsModern { get; }

        public AvailableLocale(string code, bool isModern)
        {
            this.Code = code;
            this.IsModern = isModern;
        }
    }
}
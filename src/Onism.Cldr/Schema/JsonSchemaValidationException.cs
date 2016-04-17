using System;

namespace Onism.Cldr.Schema
{
    public class JsonSchemaValidationException : Exception
    {
        public JsonSchemaValidationException(string message) : base(message)
        {
        }
    }
}
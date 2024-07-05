using System;

using Newtonsoft.Json.Linq;

namespace ArchiveNow.Configuration.Readers
{
    public static class JsonExtensions
    {
        public static object ToObject(this JToken token)
        {
            if (token == null)
            {
                return null;
            }

            switch (token.Type)
            {
                case JTokenType.Integer:
                    return (int)token;

                case JTokenType.Float:
                    return (float)token;

                case JTokenType.String:
                    return (string)token;

                case JTokenType.Boolean:
                    return (bool)token;

                case JTokenType.Null:
                    return null;

                case JTokenType.Date:
                    return (DateTime)token;

                case JTokenType.Bytes:
                    return (byte[])token;

                case JTokenType.Guid:
                    return (Guid)token;

                case JTokenType.Uri:
                    return (Uri)token;

                case JTokenType.TimeSpan:
                    return (TimeSpan)token;

                default:
                    throw new InvalidOperationException();
            }
        }
    }
}
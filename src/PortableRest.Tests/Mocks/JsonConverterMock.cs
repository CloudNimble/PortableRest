// JsonConverterMock.cs
// - PortableRest
// -- PortableRest.Tests
// 
// Author: Jeff Hansen <jeff@memberlink.com>

using System;
using Newtonsoft.Json;

namespace PortableRest.Tests.Mocks
{
    public class JsonConverterMock : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override bool CanConvert(Type objectType)
        {
            Calls++;
            return false;
        }

        public int Calls { get; private set; }
    }
}
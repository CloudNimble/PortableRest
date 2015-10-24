using System;
using Newtonsoft.Json;

namespace PortableRest.Tests.Mocks
{

    /// <summary>
    /// 
    /// </summary>
    /// <author>Jeff Hansen <jeff@memberlink.com></author>
    public class JsonConverterMock : JsonConverter
    {

        #region Properties

        /// <summary>
        /// Counts the number of calls made to the converter.
        /// </summary>
        public int Calls { get; private set; }

        #endregion

        public override bool CanConvert(Type objectType)
        {
            Calls++;
            return false;
        }

        #region Not Implemented

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        #endregion

    }
}

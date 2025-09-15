using System;
using System.Collections.Generic;
using ecl.core.Tokens;
using ecl.core.Tokens.Containers;
using ecl.json;
using ecl.lang;
using ecl.merge;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ecl.mapper
{
    public abstract class EclObjectMap
    {
        protected static readonly JsonSerializerSettings SerializerSettings;


        protected EclObjectMap()
        {
            Token = new EclObject();
            Token.OnChanged += TokenOnChanged;
        }

        private void TokenOnChanged(EclProperty property, EclToken oldValue)
        {
            if (property.Parent == Token)
            {
                RaiseChanged();
            }
        }

        public event Action OnChanged = delegate { };

        public override string ToString()
        {
            return Token.ToString();
        }

        public EclObject Token { get; }
        static EclObjectMap()
        {
            SerializerSettings = new JsonSerializerSettings();
        }
        protected void RaiseChanged()
        {
            OnChanged();
        }

        public void MergeWith(EclToken other)
        {
            EclMerge.MergeInplace(Token, other);
            //Populate with Json
            JsonConvert.PopulateObject(Token.ToJToken().ToString(), this, SerializerSettings);
            RaiseChanged();
        }
    }
    public abstract class EclObjectMap<T> : EclObjectMap
        where T : EclObjectMap<T>, new()
    {

        public static T FromSource(params IEnumerable<EclSource> sources)
        {
            var map = new T();
            var t = EclLoader.Load(sources);
            map.MergeWith(t);
            
            return map;
        }
        
        static EclObjectMap()
        {
            SerializerSettings.Converters.Add(new EclObjectMapConverter());
        }
        private class EclObjectMapConverter : JsonConverter<T>
        {
            public override T ReadJson(JsonReader reader, Type objectType, T? existingValue, bool hasExistingValue, JsonSerializer serializer)
            {
                var obj = JObject.Load(reader);
                var map = existingValue ?? new T();
                map.MergeWith(obj.ToEclToken());
                return map;
            }

            public override void WriteJson(JsonWriter writer, T? value, JsonSerializer serializer)
            {
                if (value == null)
                {
                    writer.WriteNull();
                    return;
                }
                var obj = value.Token.ToJToken();
                obj.WriteTo(writer);
            }
        }
    }
}
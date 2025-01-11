using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;

namespace Rise.Common.Extensions
{
    public static class JsonExtensions
    {
        /// <summary>
        /// Creates a <see cref="JsonObject"/> from an object and returns it.
        /// </summary>
        /// <remarks>The item to convert must implement <see cref="DataContractAttribute"/>,
        /// otherwise this won't work. To do this, simply add the DataContract attribute to
        /// your class. Public fields/properties with the <see cref="DataMemberAttribute"/>
        /// will go to the JSON.</remarks>
        public static JsonObject GetJson<Type>(this Type item)
            where Type : class, new()
        {
            using MemoryStream stream = new();
            DataContractJsonSerializer serializer = new(item.GetType());

            serializer.WriteObject(stream, item);
            _ = stream.Seek(0, SeekOrigin.Begin);

            using StreamReader reader = new(stream);
            string objString = reader.ReadToEnd();
            JsonObject obj = JsonObject.Parse(objString);

            return obj;
        }

        /// <summary>
        /// Creates a <see cref="JsonObject"/> from an object and returns it.
        /// </summary>
        /// <remarks>The item to convert must implement <see cref="DataContractAttribute"/>,
        /// otherwise this won't work. To do this, simply add the DataContract attribute to
        /// your class. Public fields/properties with the <see cref="DataMemberAttribute"/>
        /// will go to the JSON.</remarks>
        public static async Task<JsonObject> GetJsonAsync<Type>(this Type item)
            where Type : class, new()
        {
            using MemoryStream stream = new();
            DataContractJsonSerializer serializer = new(item.GetType());

            serializer.WriteObject(stream, item);
            _ = stream.Seek(0, SeekOrigin.Begin);

            using StreamReader reader = new(stream);
            string objString = await reader.ReadToEndAsync();
            JsonObject obj = JsonObject.Parse(objString);

            return obj;
        }

        /// <summary>
        /// Creates a <typeparamref name="Type"/> from a <see cref="JsonObject"/> and returns it.
        /// </summary>
        /// <remarks>The type to return must implement <see cref="DataContractAttribute"/>,
        /// otherwise this won't work. To do this, simply add the DataContract attribute to
        /// your class. Public fields/properties with the <see cref="DataMemberAttribute"/>
        /// will be deserialized.</remarks>
        public static Type Deserialize<Type>(this JsonObject obj)
            where Type : class, new()
        {
            DataContractJsonSerializer serializer = new(typeof(Type));
            using MemoryStream stream = new(Encoding.UTF8.GetBytes(obj.ToString()));
            return serializer.ReadObject(stream) as Type;
        }

        /// <summary>
        /// Creates a <typeparamref name="Type"/> from a JSON string and returns it.
        /// </summary>
        /// <remarks>The type to return must implement <see cref="DataContractAttribute"/>,
        /// otherwise this won't work. To do this, simply add the DataContract attribute to
        /// your class. Public fields/properties with the <see cref="DataMemberAttribute"/>
        /// will be deserialized.</remarks>
        public static Type Deserialize<Type>(this string json)
            where Type : class, new()
        {
            DataContractJsonSerializer serializer = new(typeof(Type));
            using MemoryStream stream = new(Encoding.UTF8.GetBytes(json));
            return serializer.ReadObject(stream) as Type;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

/// <summary>
/// JSON Packet Serializator is a utility class to pack/unpack
/// packets data
/// </summary>
public static class JPacketBuilder
{
    /// <summary>
    /// Accepts any kind of object and serializes it to a string
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static string Serialize(Packet packet)
    {
        return JsonConvert.SerializeObject(packet);
    }

    /// <summary>
    /// Accepts only objects and serializes to JSON
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static string Serialize(object obj)
    {
        return JsonConvert.SerializeObject(obj);
    }

    /// <summary>
    /// Accepts a json string and deserializes it to an packet object
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="json"></param>
    /// <returns></returns>
    public static Packet Deserialize(string json)
    {
        return JsonConvert.DeserializeObject<Packet>(json);
    }

    /// <summary>
    /// Use this function when you want to convert the data inside the packet
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="json"></param>
    /// <returns></returns>
    public static T Deserialize<T>(string json)
    {
        return JsonConvert.DeserializeObject<T>(json);
    }
}
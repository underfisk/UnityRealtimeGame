using System;
using System.Collections.Generic;
using System.Text;

[Serializable]
/// <summary>
/// Base Packet Structure
/// </summary>
public class Packet
{
    /// <summary>
    /// Identifier code
    /// </summary>
    public short opcode;

    /// <summary>
    /// JSON data as String to convert further for object
    /// </summary>
    public string data;

}

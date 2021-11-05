//-----------------------------------------------------------------------
// <copyright file="SimpleJson.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// <author>Nathan Totten (ntotten.com), Jim Zimmerman (jimzimmerman.com) and Prabir Shrestha (prabir.me)</author>
// <website>https://github.com/facebook-csharp-sdk/simple-json</website>
//-----------------------------------------------------------------------

// VERSION: 0.38.0

// NOTE: uncomment the following line to make SimpleJson class internal.
// #define SIMPLE_JSON_INTERNAL

// NOTE: uncomment the following line to make JsonArray and JsonObject class internal.
// #define SIMPLE_JSON_OBJARRAYINTERNAL

// NOTE: uncomment the following line to enable dynamic support.
// #define SIMPLE_JSON_DYNAMIC

// NOTE: uncomment the following line to enable DataContract support.
// #define SIMPLE_JSON_DATACONTRACT

// NOTE: uncomment the following line to enable IReadOnlyCollection<T> and IReadOnlyList<T> support.
// #define SIMPLE_JSON_READONLY_COLLECTIONS

// NOTE: uncomment the following line to disable linq expressions/compiled lambda (better performance) instead of method.invoke().
// define if you are using .net framework <= 3.0 or < WP7.5
// #define SIMPLE_JSON_NO_LINQ_EXPRESSION

// NOTE: uncomment the following line if you are compiling under Window Metro style application/library.
// usually already defined in properties
// #define NETFX_CORE;

// If you are targetting WinStore, WP8 and NET4.5+ PCL make sure to #define SIMPLE_JSON_TYPEINFO;

// original json parsing code from http://techblog.procurios.nl/k/618/news/view/14605/14863/How-do-I-write-my-own-parser-for-JSON.html
#if NETFX_CORE
#define SIMPLE_JSON_TYPEINFO
#endif

using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
#if !SIMPLE_JSON_NO_LINQ_EXPRESSION
using System.Linq.Expressions;
#endif
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
#if SIMPLE_JSON_DYNAMIC
using System.Dynamic;
#endif
using System.Globalization;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using Dgiot_dtu.Reflection;

// ReSharper disable LoopCanBeConvertedToQuery
// ReSharper disable RedundantExplicitArrayCreation
// ReSharper disable SuggestUseVarKeywordEvident
namespace Dgiot_dtu
{
}

namespace Dgiot_dtu
{
    /// <summary>
    /// This class encodes and decodes JSON strings.
    /// Spec. details, see http://www.json.org/
    ///
    /// JSON uses Arrays and Objects. These correspond here to the datatypes JsonArray(IList&lt;object>) and JsonObject(IDictionary&lt;string,object>).
    /// All numbers are parsed to doubles.
    /// </summary>
    [GeneratedCode("simple-json", "1.0.0")]
#if SIMPLE_JSON_INTERNAL
    internal
#else
    public
#endif
 static class SimpleJson
    {
        private const int TOKENNONE = 0;
        private const int TOKENCURLYOPEN = 1;
        private const int TOKENCURLYCLOSE = 2;
        private const int TOKENSQUAREDOPEN = 3;
        private const int TOKENSQUAREDCLOSE = 4;
        private const int TOKENCOLON = 5;
        private const int TOKENCOMMA = 6;
        private const int TOKENSTRING = 7;
        private const int TOKENNUMBER = 8;
        private const int TOKENTRUE = 9;
        private const int TOKENFALSE = 10;
        private const int TOKENNULL = 11;
        private const int BUILDERCAPACITY = 2000;

        private static readonly char[] EscapeTable;
        private static readonly char[] EscapeCharacters = new char[] { '"', '\\', '\b', '\f', '\n', '\r', '\t' };
        private static readonly string EscapeCharactersString = new string(EscapeCharacters);

        static SimpleJson()
        {
            EscapeTable = new char[93];
            EscapeTable['"'] = '"';
            EscapeTable['\\'] = '\\';
            EscapeTable['\b'] = 'b';
            EscapeTable['\f'] = 'f';
            EscapeTable['\n'] = 'n';
            EscapeTable['\r'] = 'r';
            EscapeTable['\t'] = 't';
        }

        /// <summary>
        /// Parses the string json into a value
        /// </summary>
        /// <param name="json">A JSON string.</param>
        /// <returns>An IList&lt;object>, a IDictionary&lt;string,object>, a double, a string, null, true, or false</returns>
        public static object DeserializeObject(string json)
        {
            object obj;
            if (TryDeserializeObject(json, out obj))
            {
                return obj;
            }

            throw new SerializationException("Invalid JSON string");
        }

        /// <summary>
        /// Try parsing the json string into a value.
        /// </summary>
        /// <param name="json">
        /// A JSON string.
        /// </param>
        /// <param name="obj">
        /// The object.
        /// </param>
        /// <returns>
        /// Returns true if successfull otherwise false.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1007:UseGenericsWhereAppropriate", Justification="Need to support .NET 2")]
        public static bool TryDeserializeObject(string json, out object obj)
        {
            bool success = true;
            if (json != null)
            {
                char[] charArray = json.ToCharArray();
                int index = 0;
                obj = ParseValue(charArray, ref index, ref success);
            }
            else
            {
                obj = null;
            }

            return success;
        }

        public static object DeserializeObject(string json, Type type, IJsonSerializerStrategy jsonSerializerStrategy)
        {
            object jsonObject = DeserializeObject(json);
            return type == null || (jsonObject != null && ReflectionUtils.IsAssignableFrom(jsonObject.GetType(), type))
                       ? jsonObject
                       : (jsonSerializerStrategy ?? CurrentJsonSerializerStrategy).DeserializeObject(jsonObject, type);
        }

        public static object DeserializeObject(string json, Type type)
        {
            return DeserializeObject(json, type, null);
        }

        public static T DeserializeObject<T>(string json, IJsonSerializerStrategy jsonSerializerStrategy)
        {
            return (T)DeserializeObject(json, typeof(T), jsonSerializerStrategy);
        }

        public static T DeserializeObject<T>(string json)
        {
            return (T)DeserializeObject(json, typeof(T), null);
        }

        /// <summary>
        /// Converts a IDictionary&lt;string,object> / IList&lt;object> object into a JSON string
        /// </summary>
        /// <param name="json">A IDictionary&lt;string,object> / IList&lt;object></param>
        /// <param name="jsonSerializerStrategy">Serializer strategy to use</param>
        /// <returns>A JSON encoded string, or null if object 'json' is not serializable</returns>
        public static string SerializeObject(object json, IJsonSerializerStrategy jsonSerializerStrategy)
        {
            StringBuilder builder = new StringBuilder(BUILDERCAPACITY);
            bool success = SerializeValue(jsonSerializerStrategy, json, builder);
            return success ? builder.ToString() : null;
        }

        public static string SerializeObject(object json)
        {
            return SerializeObject(json, CurrentJsonSerializerStrategy);
        }

        public static string EscapeToJavascriptString(string jsonString)
        {
            if (string.IsNullOrEmpty(jsonString))
            {
                return jsonString;
            }

            StringBuilder sb = new StringBuilder();
            char c;

            for (int i = 0; i < jsonString.Length;)
            {
                c = jsonString[i++];

                if (c == '\\')
                {
                    int remainingLength = jsonString.Length - i;
                    if (remainingLength >= 2)
                    {
                        char lookahead = jsonString[i];
                        if (lookahead == '\\')
                        {
                            sb.Append('\\');
                            ++i;
                        }
                        else if (lookahead == '"')
                        {
                            sb.Append("\"");
                            ++i;
                        }
                        else if (lookahead == 't')
                        {
                            sb.Append('\t');
                            ++i;
                        }
                        else if (lookahead == 'b')
                        {
                            sb.Append('\b');
                            ++i;
                        }
                        else if (lookahead == 'n')
                        {
                            sb.Append('\n');
                            ++i;
                        }
                        else if (lookahead == 'r')
                        {
                            sb.Append('\r');
                            ++i;
                        }
                    }
                }
                else
                {
                    sb.Append(c);
                }
            }

            return sb.ToString();
        }

        private static IDictionary<string, object> ParseObject(char[] json, ref int index, ref bool success)
        {
            IDictionary<string, object> table = new JsonObject();
            int token;

            // {
            NextToken(json, ref index);

            bool done = false;
            while (!done)
            {
                token = LookAhead(json, index);
                if (token == TOKENNONE)
                {
                    success = false;
                    return null;
                }
                else if (token == TOKENCOMMA)
                {
                    NextToken(json, ref index);
                }
                else if (token == TOKENCURLYCLOSE)
                {
                    NextToken(json, ref index);
                    return table;
                }
                else
                {
                    // name
                    string name = ParseString(json, ref index, ref success);
                    if (!success)
                    {
                        success = false;
                        return null;
                    }

                    // :
                    token = NextToken(json, ref index);
                    if (token != TOKENCOLON)
                    {
                        success = false;
                        return null;
                    }

                    // value
                    object value = ParseValue(json, ref index, ref success);
                    if (!success)
                    {
                        success = false;
                        return null;
                    }

                    table[name] = value;
                }
            }

            return table;
        }

        private static JsonArray ParseArray(char[] json, ref int index, ref bool success)
        {
            JsonArray array = new JsonArray();

            // [
            NextToken(json, ref index);

            bool done = false;
            while (!done)
            {
                int token = LookAhead(json, index);
                if (token == TOKENNONE)
                {
                    success = false;
                    return null;
                }
                else if (token == TOKENCOMMA)
                {
                    NextToken(json, ref index);
                }
                else if (token == TOKENSQUAREDCLOSE)
                {
                    NextToken(json, ref index);
                    break;
                }
                else
                {
                    object value = ParseValue(json, ref index, ref success);
                    if (!success)
                    {
                        return null;
                    }

                    array.Add(value);
                }
            }

            return array;
        }

        private static object ParseValue(char[] json, ref int index, ref bool success)
        {
            switch (LookAhead(json, index))
            {
                case TOKENSTRING:
                    return ParseString(json, ref index, ref success);
                case TOKENNUMBER:
                    return ParseNumber(json, ref index, ref success);
                case TOKENCURLYOPEN:
                    return ParseObject(json, ref index, ref success);
                case TOKENSQUAREDOPEN:
                    return ParseArray(json, ref index, ref success);
                case TOKENTRUE:
                    NextToken(json, ref index);
                    return true;
                case TOKENFALSE:
                    NextToken(json, ref index);
                    return false;
                case TOKENNULL:
                    NextToken(json, ref index);
                    return null;
                case TOKENNONE:
                    break;
            }

            success = false;
            return null;
        }

        private static string ParseString(char[] json, ref int index, ref bool success)
        {
            StringBuilder s = new StringBuilder(BUILDERCAPACITY);
            char c;

            EatWhitespace(json, ref index);

            // "
            c = json[index++];
            bool complete = false;
            while (!complete)
            {
                if (index == json.Length)
                {
                    break;
                }

                c = json[index++];
                if (c == '"')
                {
                    complete = true;
                    break;
                }
                else if (c == '\\')
                {
                    if (index == json.Length)
                    {
                        break;
                    }

                    c = json[index++];
                    if (c == '"')
                    {
                        s.Append('"');
                    }
                    else if (c == '\\')
                    {
                        s.Append('\\');
                    }
                    else if (c == '/')
                    {
                        s.Append('/');
                    }
                    else if (c == 'b')
                    {
                        s.Append('\b');
                    }
                    else if (c == 'f')
                    {
                        s.Append('\f');
                    }
                    else if (c == 'n')
                    {
                        s.Append('\n');
                    }
                    else if (c == 'r')
                    {
                        s.Append('\r');
                    }
                    else if (c == 't')
                    {
                        s.Append('\t');
                    }
                    else if (c == 'u')
                    {
                        int remainingLength = json.Length - index;
                        if (remainingLength >= 4)
                        {
                            // parse the 32 bit hex into an integer codepoint
                            uint codePoint;
                            if (!(success = uint.TryParse(new string(json, index, 4), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out codePoint)))
                            {
                                return string.Empty;
                            }

                            // convert the integer codepoint to a unicode char and add to string
                            // if high surrogate
                            if (codePoint >= 0xD800 && codePoint <= 0xDBFF)
                            {
                                index += 4; // skip 4 chars
                                remainingLength = json.Length - index;
                                if (remainingLength >= 6)
                                {
                                    uint lowCodePoint;
                                    if (new string(json, index, 2) == "\\u" && uint.TryParse(new string(json, index + 2, 4), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out lowCodePoint))
                                    {
                                        // if low surrogate
                                        if (lowCodePoint >= 0xDC00 && lowCodePoint <= 0xDFFF)
                                        {
                                            s.Append((char)codePoint);
                                            s.Append((char)lowCodePoint);
                                            index += 6; // skip 6 chars
                                            continue;
                                        }
                                    }
                                }

                                success = false;    // invalid surrogate pair
                                return string.Empty;
                            }

                            s.Append(ConvertFromUtf32((int)codePoint));

                            // skip 4 chars
                            index += 4;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                else
                {
                    s.Append(c);
                }
            }

            if (!complete)
            {
                success = false;
                return null;
            }

            return s.ToString();
        }

        private static string ConvertFromUtf32(int utf32)
        {
            // http://www.java2s.com/Open-Source/CSharp/2.6.4-mono-.net-core/System/System/Char.cs.htm
            if (utf32 < 0 || utf32 > 0x10FFFF)
            {
                throw new ArgumentOutOfRangeException("utf32", "The argument must be from 0 to 0x10FFFF.");
            }

            if (utf32 >= 0xD800 && utf32 <= 0xDFFF)
            {
                throw new ArgumentOutOfRangeException("utf32", "The argument must not be in surrogate pair range.");
            }

            if (utf32 < 0x10000)
            {
                return new string((char)utf32, 1);
            }

            utf32 -= 0x10000;
            return new string(new char[] { (char)((utf32 >> 10) + 0xD800), (char)((utf32 % 0x0400) + 0xDC00) });
        }

        private static object ParseNumber(char[] json, ref int index, ref bool success)
        {
            EatWhitespace(json, ref index);
            int lastIndex = GetLastIndexOfNumber(json, index);
            int charLength = (lastIndex - index) + 1;
            object returnNumber;
            string str = new string(json, index, charLength);
            if (str.IndexOf(".", StringComparison.OrdinalIgnoreCase) != -1 || str.IndexOf("e", StringComparison.OrdinalIgnoreCase) != -1)
            {
                double number;
                success = double.TryParse(new string(json, index, charLength), NumberStyles.Any, CultureInfo.InvariantCulture, out number);
                returnNumber = number;
            }
            else
            {
                long number;
                success = long.TryParse(new string(json, index, charLength), NumberStyles.Any, CultureInfo.InvariantCulture, out number);
                returnNumber = number;
            }

            index = lastIndex + 1;
            return returnNumber;
        }

        private static int GetLastIndexOfNumber(char[] json, int index)
        {
            int lastIndex;
            for (lastIndex = index; lastIndex < json.Length; lastIndex++)
            {
                if ("0123456789+-.eE".IndexOf(json[lastIndex]) == -1)
                {
                    break;
                }
            }

            return lastIndex - 1;
        }

        private static void EatWhitespace(char[] json, ref int index)
        {
            for (; index < json.Length; index++)
            {
                if (" \t\n\r\b\f".IndexOf(json[index]) == -1)
                {
                    break;
                }
            }
        }

        private static int LookAhead(char[] json, int index)
        {
            int saveIndex = index;
            return NextToken(json, ref saveIndex);
        }

        private static int NextToken(char[] json, ref int index)
        {
            EatWhitespace(json, ref index);
            if (index == json.Length)
            {
                return TOKENNONE;
            }

            char c = json[index];
            index++;
            switch (c)
            {
                case '{':
                    return TOKENCURLYOPEN;
                case '}':
                    return TOKENCURLYCLOSE;
                case '[':
                    return TOKENSQUAREDOPEN;
                case ']':
                    return TOKENSQUAREDCLOSE;
                case ',':
                    return TOKENCOMMA;
                case '"':
                    return TOKENSTRING;
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                case '-':
                    return TOKENNUMBER;
                case ':':
                    return TOKENCOLON;
            }

            index--;
            int remainingLength = json.Length - index;

            // false
            if (remainingLength >= 5)
            {
                if (json[index] == 'f' && json[index + 1] == 'a' && json[index + 2] == 'l' && json[index + 3] == 's' && json[index + 4] == 'e')
                {
                    index += 5;
                    return TOKENFALSE;
                }
            }

            // true
            if (remainingLength >= 4)
            {
                if (json[index] == 't' && json[index + 1] == 'r' && json[index + 2] == 'u' && json[index + 3] == 'e')
                {
                    index += 4;
                    return TOKENTRUE;
                }
            }

            // null
            if (remainingLength >= 4)
            {
                if (json[index] == 'n' && json[index + 1] == 'u' && json[index + 2] == 'l' && json[index + 3] == 'l')
                {
                    index += 4;
                    return TOKENNULL;
                }
            }

            return TOKENNONE;
        }

        private static bool SerializeValue(IJsonSerializerStrategy jsonSerializerStrategy, object value, StringBuilder builder)
        {
            bool success = true;
            string stringValue = value as string;
            if (stringValue != null)
            {
                success = SerializeString(stringValue, builder);
            }
            else
            {
                IDictionary<string, object> dict = value as IDictionary<string, object>;
                if (dict != null)
                {
                    success = SerializeObject(jsonSerializerStrategy, dict.Keys, dict.Values, builder);
                }
                else
                {
                    IDictionary<string, string> stringDictionary = value as IDictionary<string, string>;
                    if (stringDictionary != null)
                    {
                        success = SerializeObject(jsonSerializerStrategy, stringDictionary.Keys, stringDictionary.Values, builder);
                    }
                    else
                    {
                        IEnumerable enumerableValue = value as IEnumerable;
                        if (enumerableValue != null)
                        {
                            success = SerializeArray(jsonSerializerStrategy, enumerableValue, builder);
                        }
                        else if (IsNumeric(value))
                        {
                            success = SerializeNumber(value, builder);
                        }
                        else if (value is bool)
                        {
                            builder.Append((bool)value ? "true" : "false");
                        }
                        else if (value == null)
                        {
                            builder.Append("null");
                        }
                        else
                        {
                            object serializedObject;
                            success = jsonSerializerStrategy.TrySerializeNonPrimitiveObject(value, out serializedObject);
                            if (success)
                            {
                                SerializeValue(jsonSerializerStrategy, serializedObject, builder);
                            }
                        }
                    }
                }
            }

            return success;
        }

        private static bool SerializeObject(IJsonSerializerStrategy jsonSerializerStrategy, IEnumerable keys, IEnumerable values, StringBuilder builder)
        {
            builder.Append("{");
            IEnumerator ke = keys.GetEnumerator();
            IEnumerator ve = values.GetEnumerator();
            bool first = true;
            while (ke.MoveNext() && ve.MoveNext())
            {
                object key = ke.Current;
                object value = ve.Current;
                if (!first)
                {
                    builder.Append(",");
                }

                string stringKey = key as string;
                if (stringKey != null)
                {
                    SerializeString(stringKey, builder);
                }
                else
                    if (!SerializeValue(jsonSerializerStrategy, value, builder))
                {
                    return false;
                }

                builder.Append(":");
                if (!SerializeValue(jsonSerializerStrategy, value, builder))
                {
                    return false;
                }

                first = false;
            }

            builder.Append("}");
            return true;
        }

        private static bool SerializeArray(IJsonSerializerStrategy jsonSerializerStrategy, IEnumerable anArray, StringBuilder builder)
        {
            builder.Append("[");
            bool first = true;
            foreach (object value in anArray)
            {
                if (!first)
                {
                    builder.Append(",");
                }

                if (!SerializeValue(jsonSerializerStrategy, value, builder))
                {
                    return false;
                }

                first = false;
            }

            builder.Append("]");
            return true;
        }

        private static bool SerializeString(string aString, StringBuilder builder)
        {
            // Happy path if there's nothing to be escaped. IndexOfAny is highly optimized (and unmanaged)
            if (aString.IndexOfAny(EscapeCharacters) == -1)
            {
                builder.Append('"');
                builder.Append(aString);
                builder.Append('"');

                return true;
            }

            builder.Append('"');
            int safeCharacterCount = 0;
            char[] charArray = aString.ToCharArray();

            for (int i = 0; i < charArray.Length; i++)
            {
                char c = charArray[i];

                // Non ascii characters are fine, buffer them up and send them to the builder
                // in larger chunks if possible. The escape table is a 1:1 translation table
                // with \0 [default(char)] denoting a safe character.
                if (c >= EscapeTable.Length || EscapeTable[c] == default(char))
                {
                    safeCharacterCount++;
                }
                else
                {
                    if (safeCharacterCount > 0)
                    {
                        builder.Append(charArray, i - safeCharacterCount, safeCharacterCount);
                        safeCharacterCount = 0;
                    }

                    builder.Append('\\');
                    builder.Append(EscapeTable[c]);
                }
            }

            if (safeCharacterCount > 0)
            {
                builder.Append(charArray, charArray.Length - safeCharacterCount, safeCharacterCount);
            }

            builder.Append('"');
            return true;
        }

        private static bool SerializeNumber(object number, StringBuilder builder)
        {
            if (number is long)
            {
                builder.Append(((long)number).ToString(CultureInfo.InvariantCulture));
            }
            else if (number is ulong)
            {
                builder.Append(((ulong)number).ToString(CultureInfo.InvariantCulture));
            }
            else if (number is int)
            {
                builder.Append(((int)number).ToString(CultureInfo.InvariantCulture));
            }
            else if (number is uint)
            {
                builder.Append(((uint)number).ToString(CultureInfo.InvariantCulture));
            }
            else if (number is decimal)
            {
                builder.Append(((decimal)number).ToString(CultureInfo.InvariantCulture));
            }
            else if (number is float)
            {
                builder.Append(((float)number).ToString(CultureInfo.InvariantCulture));
            }
            else
            {
                builder.Append(Convert.ToDouble(number, CultureInfo.InvariantCulture).ToString("r", CultureInfo.InvariantCulture));
            }

            return true;
        }

        /// <summary>
        /// Determines if a given object is numeric in any way
        /// (can be integer, double, null, etc).
        /// </summary>
        private static bool IsNumeric(object value)
        {
            if (value is sbyte)
            {
                return true;
            }

            if (value is byte)
            {
                return true;
            }

            if (value is short)
            {
                return true;
            }

            if (value is ushort)
            {
                return true;
            }

            if (value is int)
            {
                return true;
            }

            if (value is uint)
            {
                return true;
            }

            if (value is long)
            {
                return true;
            }

            if (value is ulong)
            {
                return true;
            }

            if (value is float)
            {
                return true;
            }

            if (value is double)
            {
                return true;
            }

            if (value is decimal)
            {
                return true;
            }

            return false;
        }

        private static IJsonSerializerStrategy currentJsonSerializerStrategy;

        public static IJsonSerializerStrategy CurrentJsonSerializerStrategy
        {
            get
            {
                return currentJsonSerializerStrategy ??
                    (currentJsonSerializerStrategy =
#if SIMPLE_JSON_DATACONTRACT
 DataContractJsonSerializerStrategy
#else
 PocoJsonSerializerStrategy
#endif
);
            }

            set
            {
                currentJsonSerializerStrategy = value;
            }
        }

        private static PocoJsonSerializerStrategy pocoJsonSerializerStrategy;

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public static PocoJsonSerializerStrategy PocoJsonSerializerStrategy
        {
            get
            {
                return pocoJsonSerializerStrategy ?? (pocoJsonSerializerStrategy = new PocoJsonSerializerStrategy());
            }
        }

#if SIMPLE_JSON_DATACONTRACT

        private static DataContractJsonSerializerStrategy _dataContractJsonSerializerStrategy;
        [System.ComponentModel.EditorBrowsable(EditorBrowsableState.Advanced)]
        public static DataContractJsonSerializerStrategy DataContractJsonSerializerStrategy
        {
            get
            {
                return _dataContractJsonSerializerStrategy ?? (_dataContractJsonSerializerStrategy = new DataContractJsonSerializerStrategy());
            }
        }

#endif
    }

    [GeneratedCode("simple-json", "1.0.0")]
#if SIMPLE_JSON_INTERNAL
    internal
#else
    public
#endif
 interface IJsonSerializerStrategy
    {
        [SuppressMessage("Microsoft.Design", "CA1007:UseGenericsWhereAppropriate", Justification="Need to support .NET 2")]
        bool TrySerializeNonPrimitiveObject(object input, out object output);

        object DeserializeObject(object value, Type type);
    }

#if SIMPLE_JSON_DATACONTRACT
    [GeneratedCode("simple-json", "1.0.0")]
#if SIMPLE_JSON_INTERNAL
    internal
#else
    public
#endif
 class DataContractJsonSerializerStrategy : PocoJsonSerializerStrategy
    {
        public DataContractJsonSerializerStrategy()
        {
            GetCache = new ReflectionUtils.ThreadSafeDictionary<Type, IDictionary<string, ReflectionUtils.GetDelegate>>(GetterValueFactory);
            SetCache = new ReflectionUtils.ThreadSafeDictionary<Type, IDictionary<string, KeyValuePair<Type, ReflectionUtils.SetDelegate>>>(SetterValueFactory);
        }

        internal override IDictionary<string, ReflectionUtils.GetDelegate> GetterValueFactory(Type type)
        {
            bool hasDataContract = ReflectionUtils.GetAttribute(type, typeof(DataContractAttribute)) != null;
            if (!hasDataContract)
                return base.GetterValueFactory(type);
            string jsonKey;
            IDictionary<string, ReflectionUtils.GetDelegate> result = new Dictionary<string, ReflectionUtils.GetDelegate>();
            foreach (PropertyInfo propertyInfo in ReflectionUtils.GetProperties(type))
            {
                if (propertyInfo.CanRead)
                {
                    MethodInfo getMethod = ReflectionUtils.GetGetterMethodInfo(propertyInfo);
                    if (!getMethod.IsStatic && CanAdd(propertyInfo, out jsonKey))
                        result[jsonKey] = ReflectionUtils.GetGetMethod(propertyInfo);
                }
            }
            foreach (FieldInfo fieldInfo in ReflectionUtils.GetFields(type))
            {
                if (!fieldInfo.IsStatic && CanAdd(fieldInfo, out jsonKey))
                    result[jsonKey] = ReflectionUtils.GetGetMethod(fieldInfo);
            }
            return result;
        }

        internal override IDictionary<string, KeyValuePair<Type, ReflectionUtils.SetDelegate>> SetterValueFactory(Type type)
        {
            bool hasDataContract = ReflectionUtils.GetAttribute(type, typeof(DataContractAttribute)) != null;
            if (!hasDataContract)
                return base.SetterValueFactory(type);
            string jsonKey;
            IDictionary<string, KeyValuePair<Type, ReflectionUtils.SetDelegate>> result = new Dictionary<string, KeyValuePair<Type, ReflectionUtils.SetDelegate>>();
            foreach (PropertyInfo propertyInfo in ReflectionUtils.GetProperties(type))
            {
                if (propertyInfo.CanWrite)
                {
                    MethodInfo setMethod = ReflectionUtils.GetSetterMethodInfo(propertyInfo);
                    if (!setMethod.IsStatic && CanAdd(propertyInfo, out jsonKey))
                        result[jsonKey] = new KeyValuePair<Type, ReflectionUtils.SetDelegate>(propertyInfo.PropertyType, ReflectionUtils.GetSetMethod(propertyInfo));
                }
            }
            foreach (FieldInfo fieldInfo in ReflectionUtils.GetFields(type))
            {
                if (!fieldInfo.IsInitOnly && !fieldInfo.IsStatic && CanAdd(fieldInfo, out jsonKey))
                    result[jsonKey] = new KeyValuePair<Type, ReflectionUtils.SetDelegate>(fieldInfo.FieldType, ReflectionUtils.GetSetMethod(fieldInfo));
            }
            // todo implement sorting for DATACONTRACT.
            return result;
        }

        private static bool CanAdd(MemberInfo info, out string jsonKey)
        {
            jsonKey = null;
            if (ReflectionUtils.GetAttribute(info, typeof(IgnoreDataMemberAttribute)) != null)
                return false;
            DataMemberAttribute dataMemberAttribute = (DataMemberAttribute)ReflectionUtils.GetAttribute(info, typeof(DataMemberAttribute));
            if (dataMemberAttribute == null)
                return false;
            jsonKey = string.IsNullOrEmpty(dataMemberAttribute.Name) ? info.Name : dataMemberAttribute.Name;
            return true;
        }
    }

#endif

    namespace Reflection
    {
    }
}

// ReSharper restore LoopCanBeConvertedToQuery
// ReSharper restore RedundantExplicitArrayCreation
// ReSharper restore SuggestUseVarKeywordEvident

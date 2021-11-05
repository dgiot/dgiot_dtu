//-----------------------------------------------------------------------
// <copyright file="JsonArray.cs" company="PlaceholderCompany">
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
    /// <summary>
    /// Represents the json array.
    /// </summary>
    [GeneratedCode("simple-json", "1.0.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
#if SIMPLE_JSON_OBJARRAYINTERNAL
    internal
#else
    public
#endif
 class JsonArray : List<object>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JsonArray"/> class.
        /// </summary>
        public JsonArray()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonArray"/> class.
        /// </summary>
        /// <param name="capacity">The capacity of the json array.</param>
        public JsonArray(int capacity) : base(capacity)
        {
        }

        /// <summary>
        /// The json representation of the array.
        /// </summary>
        /// <returns>The json representation of the array.</returns>
        public override string ToString()
        {
            return SimpleJson.SerializeObject(this) ?? string.Empty;
        }
    }
}

// ReSharper restore LoopCanBeConvertedToQuery
// ReSharper restore RedundantExplicitArrayCreation
// ReSharper restore SuggestUseVarKeywordEvident

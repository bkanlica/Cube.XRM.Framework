// ***********************************************************************
// Assembly         : Cube.XRM.Framework
// Author           : baris
// Created          : 03-28-2020
//
// Last Modified By : baris
// Last Modified On : 06-06-2017
// ***********************************************************************
// <copyright file="SerializeParameters.cs" company="Microsoft Corporation">
//     Copyright © Microsoft Corporation 2015
// </copyright>
// <summary></summary>
// ***********************************************************************
using Microsoft.Xrm.Sdk;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cube.XRM.Framework.Serializers
{
    /// <summary>
    /// Class SerializeHelper.
    /// </summary>
    public static partial class SerializeHelper
    {
        /// <summary>
        /// Serializes the parameter collection.
        /// </summary>
        /// <param name="parameterCollection">The parameter collection.</param>
        /// <returns>System.String.</returns>
        public static string SerializeParameterCollection(ParameterCollection parameterCollection)
        {
            if (parameterCollection == null)
            {
                return string.Empty;
            }
            StringBuilder stringBuilder = new StringBuilder();
            foreach (KeyValuePair<string, object> current in parameterCollection)
            {
                if (current.Value != null && current.Value.GetType() == typeof(Entity))
                {
                    Entity e = (Entity)current.Value;
                    stringBuilder.Append(current.Key + ": " + SerializeHelper.SerializeEntity(e));
                }
                if (current.Value != null && current.Value.GetType() == typeof(EntityReferenceCollection))
                {
                    stringBuilder.Append(string.Concat(new object[]
                    {
                current.Key,
                ": ",
                current.Value,
                "; "
                    }));
                    EntityReferenceCollection entityReferenceCollection = (EntityReferenceCollection)current.Value;
                    stringBuilder.Append(" Count: " + entityReferenceCollection.Count.ToString() + "; ");
                }
                else if (current.Value == null)
                {
                    stringBuilder.AppendLine(string.Format("Null parameter value for key {0}", current.Key));
                }
                else if (current.Value.GetType().Name == "EntityReference")
                {
                    stringBuilder.Append(string.Concat(new object[]
                    {
                "EntityReference: ",
                current.Key,
                ": ",
                ((EntityReference)current.Value).Id,
                "; "
                    }));
                }
                else if (current.Value.GetType().Name == "OptionSetValue")
                {
                    stringBuilder.Append(string.Concat(new object[]
                    {
                "OptionSetValue: ",
                current.Key,
                ": ",
                ((OptionSetValue)current.Value).Value,
                "; "
                    }));
                }
                else if (current.Value.GetType().Name == "Money")
                {
                    stringBuilder.Append(string.Concat(new object[]
                    {
                "Money: ",
                current.Key,
                ": ",
                ((Money)current.Value).Value,
                "; "
                    }));
                }
                else if (current.Value.GetType().Name == "EntityCollection")
                {
                    stringBuilder.Append(string.Concat(new string[]
                    {
                "EntityCollection: ",
                current.Key,
                ": ",
                ((EntityCollection)current.Value).EntityName,
                "::",
                ((EntityCollection)current.Value).Entities.Count.ToString(),
                "; "
                    }));
                }
                else
                {
                    stringBuilder.Append(string.Concat(new object[]
                    {
                "PrimitiveValue: ",
                current.Key,
                ": ",
                current.Value,
                "; "
                    }));
                }
                stringBuilder.AppendLine();
            }
            if (parameterCollection.Count() == 0)
            {
                stringBuilder.AppendLine("No items found.");
            }
            stringBuilder.AppendLine();
            return stringBuilder.ToString();
        }

    }
}

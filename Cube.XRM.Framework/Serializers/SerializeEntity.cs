// ***********************************************************************
// Assembly         : Cube.XRM.Framework
// Author           : baris
// Created          : 03-28-2020
//
// Last Modified By : baris
// Last Modified On : 06-06-2017
// ***********************************************************************
// <copyright file="SerializeEntity.cs" company="Microsoft Corporation">
//     Copyright © Microsoft Corporation 2015
// </copyright>
// <summary></summary>
// ***********************************************************************
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cube.XRM.Framework.Serializers
{
    /// <summary>
    /// Class SerializeHelper.
    /// </summary>
    public static partial class SerializeHelper
    {
        /// <summary>
        /// Serializes the entity.
        /// </summary>
        /// <param name="e">The e.</param>
        /// <returns>System.String.</returns>
        public static string SerializeEntity(Entity e)
        {
            if (e == null)
            {
                return "Null Entity";
            }
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine();
            stringBuilder.AppendLine("+++ Entity Trace for " + e.LogicalName + " +++");
            stringBuilder.AppendLine();
            stringBuilder.Append("LogicalName: " + e.LogicalName);
            stringBuilder.Append(Environment.NewLine);
            stringBuilder.Append("EntityId: " + e.Id);
            stringBuilder.Append(Environment.NewLine);
            stringBuilder.Append("Attributes:");
            stringBuilder.AppendLine();
            stringBuilder.AppendLine("[");
            foreach (KeyValuePair<string, object> current in e.Attributes)
            {
                stringBuilder.AppendLine();
                if (current.Value == null)
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
            }
            foreach (KeyValuePair<string, string> current2 in e.FormattedValues)
            {
                stringBuilder.AppendLine();
                stringBuilder.Append(string.Concat(new string[]
                {
            "FormattedValue: ",
            current2.Key,
            ": ",
            current2.Value,
            "; "
                }));
            }
            stringBuilder.AppendLine();
            stringBuilder.Append("]");
            stringBuilder.AppendLine();
            return stringBuilder.ToString();
        }

    }
}

// ***********************************************************************
// Assembly         : Cube.XRM.Framework
// Author           : baris
// Created          : 03-28-2020
//
// Last Modified By : baris
// Last Modified On : 06-06-2017
// ***********************************************************************
// <copyright file="SerializeEntityImage.cs" company="Microsoft Corporation">
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
        /// Serializes the entity image collection.
        /// </summary>
        /// <param name="entityImageCollection">The entity image collection.</param>
        /// <returns>System.String.</returns>
        public static string SerializeEntityImageCollection(EntityImageCollection entityImageCollection)
        {
            if (entityImageCollection == null)
            {
                return string.Empty;
            }
            StringBuilder stringBuilder = new StringBuilder();
            foreach (KeyValuePair<string, Entity> current in entityImageCollection)
            {
                stringBuilder.Append(Environment.NewLine);
                stringBuilder.Append(current.Key + ": " + SerializeHelper.SerializeEntity(current.Value));
            }
            stringBuilder.AppendLine();
            return stringBuilder.ToString();
        }
    }
}

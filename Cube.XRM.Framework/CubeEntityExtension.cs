// ***********************************************************************
// Assembly         : Cube.XRM.Framework
// Author           : baris
// Created          : 03-29-2020
//
// Last Modified By : baris
// Last Modified On : 03-29-2020
// ***********************************************************************
// <copyright file="CubeEntityExtension.cs" company="Microsoft Corporation">
//     Copyright © Microsoft Corporation 2015
// </copyright>
// <summary></summary>
// ***********************************************************************

using Microsoft.Xrm.Sdk;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Cube.XRM.Framework
{
    /// <summary>
    /// Class CubeEntityExtension.
    /// </summary>
    public static class CubeEntityExtension
    {
        /// <summary>
        /// Clones the specified cube.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="cube">The cube.</param>
        /// <returns>Result.</returns>
        public static Result Clone(this Entity entity, CubeBase cube)
        {
            try
            {
                var result = new Entity(entity.LogicalName, entity.Id)
                {
                    EntityState = entity.EntityState,
                    RowVersion = entity.RowVersion
                };
                result.KeyAttributes.AddRange(entity.KeyAttributes.Select(k => new KeyValuePair<string, object>(k.Key, k.Value)));
                result.Attributes.AddRange(entity.Attributes.Select(a => new KeyValuePair<string, object>(a.Key, a.Value)));
                return new Result(false, null, entity, null);
            }
            catch (Exception ex)
            {
                return new Result(true, ex.Message, null, cube.LogSystem);
            }
        }

        /// <summary>
        /// Merges the specified entity.
        /// </summary>
        /// <param name="baseEntity">The base entity.</param>
        /// <param name="entity">The entity.</param>
        /// <param name="cube">The cube.</param>
        /// <returns>Result.</returns>
        public static Result Merge(this Entity baseEntity, Entity entity, CubeBase cube)
        {
            try
            {
                if (baseEntity == null)
                {
                    return new Result(false, null, entity, null);
                }
                if (entity != null)
                {
                    baseEntity.Attributes.AddRange(entity.Attributes.Where(a => !baseEntity.Contains(a.Key)).Select(b => new KeyValuePair<string, object>(b.Key, b.Value)));
                }
                return new Result(false, null, baseEntity, null);
            }
            catch (Exception ex)
            {
                return new Result(true, ex.Message, null, cube.LogSystem);
            }
        }
    }
}

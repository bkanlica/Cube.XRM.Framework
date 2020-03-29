// ***********************************************************************
// Assembly         : Cube.XRM.Framework
// Author           : baris
// Created          : 03-28-2020
//
// Last Modified By : baris
// Last Modified On : 03-29-2020
// ***********************************************************************
// <copyright file="CubeEntity.cs" company="Microsoft Corporation">
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
    /// Class CubeProperty.
    /// Implements the <see cref="System.Runtime.Serialization.IExtensibleDataObject" />
    /// </summary>
    /// <seealso cref="System.Runtime.Serialization.IExtensibleDataObject" />
    public class CubeProperty : IExtensibleDataObject
    {
        /// <summary>
        /// Gets or sets the structure that contains extra data.
        /// </summary>
        /// <value>The extension data.</value>
        public ExtensionDataObject ExtensionData { get; set; }

        /// <summary>
        /// The property
        /// </summary>
        private object _Property;

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the name of the logical.
        /// </summary>
        /// <value>The name of the logical.</value>
        public string LogicalName { get; set; }

        /// <summary>
        /// Gets or sets the value string.
        /// </summary>
        /// <value>The value string.</value>
        public string ValueString { get; set; }

        /// <summary>
        /// Gets or sets the value object.
        /// </summary>
        /// <value>The value object.</value>
        public object ValueObject { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is set.
        /// </summary>
        /// <value><c>true</c> if this instance is set; otherwise, <c>false</c>.</value>
        public bool IsSet { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CubeProperty" /> class.
        /// </summary>
        /// <param name="property">The property.</param>
        public CubeProperty(object property)
        {
            _Property = property;
            if (property != null)
                IsSet = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CubeProperty" /> class.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="cube">The cube.</param>
        /// <param name="de">The de.</param>
        /// <exception cref="Exception"></exception>
        public CubeProperty(object property, CubeBase cube, Entity de)
        {
            _Property = property;
            if (property != null)
                IsSet = true;

            KeyValuePair<string, object> p = (KeyValuePair<string, object>)property;
            if (p.Value is EntityReference)
            {
                EntityReference er = (EntityReference)p.Value;
                Name = er.Name;
                LogicalName = er.LogicalName;
                ValueString = er.Id.ToString();
                ValueObject = er.Id;
            }
            else if (p.Value is OptionSetValue)
            {
                OptionSetValue osv = (OptionSetValue)p.Value;
                Result mdResult = cube.MetadataRetrieveActions.GetOptionSetText(de.LogicalName, p.Key, osv.Value);
                if (mdResult.isError)
                    throw new Exception(mdResult.Message);

                Name = mdResult.BusinessObject.ToString();
                LogicalName = p.Key;
                ValueString = osv.Value.ToString();
                ValueObject = osv.Value;
            }
            else if (p.Value is Money)
            {
                Money money = (Money)p.Value;
                Name = money.Value.ToString();
                LogicalName = p.Key;
                ValueString = money.Value.ToString();
                ValueObject = money.Value;
            }
            else
            {
                Name = p.Value.ToString();
                LogicalName = p.Key;
                ValueString = p.Value.ToString();
                ValueObject = p.Value;
            }
        }
    }

    /// <summary>
    /// Class CubeEntity.
    /// </summary>
    public class CubeEntity
    {
        /// <summary>
        /// Gets or sets the error code.
        /// </summary>
        /// <value>The error code.</value>
        public int ErrorCode { get; set; }
        /// <summary>
        /// Gets or sets the error detail.
        /// </summary>
        /// <value>The error detail.</value>
        public string ErrorDetail { get; set; }
        /// <summary>
        /// Gets or sets the name of the entity.
        /// </summary>
        /// <value>The name of the entity.</value>
        public string EntityName { get; set; }
        /// <summary>
        /// Gets or sets the ID of the entity.
        /// </summary>
        /// <value>The ID of the entity.</value>
        public Guid ID { get; set; }

        /// <summary>
        /// Gets or sets the table cube entity.
        /// </summary>
        /// <value>The table cube entity.</value>
        private Hashtable TableCubeEntity { get; set; }

        /// <summary>
        /// Gets the CubeBase
        /// </summary>
        /// <value>The Cube.</value>
        public CubeBase cube { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CubeEntity" /> class.
        /// </summary>
        public CubeEntity()
        {
            cube = ObjectCarrier.GetValue<CubeBase>("cube");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CubeEntity" /> class.
        /// </summary>
        /// <param name="Cube">The cube.</param>
        public CubeEntity(CubeBase Cube)
        {
            cube = Cube;
        }

        /// <summary>
        /// Creates this instance.
        /// </summary>
        /// <returns>Result.</returns>
        /// <exception cref="Exception"></exception>
        public Result Create()
        {
            try
            {
                Entity entity = new Entity(EntityName);
                entity.Attributes = PrepareEntity();
                Result result = cube.XRMActions.Create(entity);
                if (!result.isError)
                {
                    ID = (Guid)result.BusinessObject;
                    return result;
                }
                else
                    throw new Exception(result.Message);
            }
            catch (Exception ex)
            {
                return new Result(true, ex.Message, null, cube.LogSystem);
            }
        }

        /// <summary>
        /// Updates this instance.
        /// </summary>
        /// <returns>Result.</returns>
        /// <exception cref="Exception"></exception>
        public Result Update()
        {
            try
            {
                Entity entity = new Entity(EntityName);
                entity.Attributes = PrepareEntity();
                Result result = cube.XRMActions.Update(entity);
                if (!result.isError)
                {
                    ID = (Guid)result.BusinessObject;
                    return result;
                }
                else
                    throw new Exception(result.Message);
            }
            catch (Exception ex)
            {
                return new Result(true, ex.Message, null, cube.LogSystem);
            }
        }

        /// <summary>
        /// Deletes this instance.
        /// </summary>
        /// <returns>Result.</returns>
        /// <exception cref="Exception"></exception>
        public Result Delete()
        {
            try
            {
                Result result = cube.XRMActions.Delete(ID, EntityName);
                if (!result.isError)
                {
                    ID = (Guid)result.BusinessObject;
                    return result;
                }
                else
                    throw new Exception(result.Message);
            }
            catch (Exception ex)
            {
                return new Result(true, ex.Message, null, cube.LogSystem);
            }
        }


        /// <summary>
        /// Prepares the entity.
        /// </summary>
        /// <returns>Entity.</returns>
        /// <exception cref="NotImplementedException"></exception>
        /// <exception cref="System.NotImplementedException"></exception>
        public virtual AttributeCollection PrepareEntity()
        { throw new NotImplementedException(); }

        /// <summary>
        /// Parses the item.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>IEntity.</returns>
        /// <exception cref="NotImplementedException"></exception>
        /// <exception cref="System.NotImplementedException"></exception>
        public virtual Result ParseItem(Entity entity)
        { throw new NotImplementedException(); }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            return string.Format("Entity:{0} ID:{1}", EntityName, ID != null ? ID.ToString() : string.Empty);
        }

        /// <summary>
        /// Gets the <see cref="CubeProperty" /> with the specified attribute name.
        /// </summary>
        /// <param name="AttributeName">Name of the attribute.</param>
        /// <returns>CubeProperty.</returns>
        public CubeProperty this[string AttributeName]
        {
            get
            {
                return (CubeProperty)TableCubeEntity[AttributeName];
            }
        }

        /// <summary>
        /// Parses the entities.
        /// </summary>
        /// <param name="de">The de.</param>
        /// <param name="Schema">The schema.</param>
        /// <param name="cube">The cube.</param>
        /// <returns>Result.</returns>
        public static Result ParseEntities(Entity de, string[] Schema, CubeBase cube)
        {
            try
            {
                Hashtable parsedEntities = new Hashtable();
                if (Schema != null && Schema.Length > 0)
                {
                    ArrayList listSchema = new ArrayList(Schema);
                    foreach (KeyValuePair<string, object> p in de.Attributes)
                    {

                        for (int i = 0; i < listSchema.Count; i++)
                        {
                            if (p.Key == (string)listSchema[i])
                            {
                                parsedEntities.Add(p.Key, new CubeProperty(p, cube, de));
                                listSchema.RemoveAt(i);
                                break;
                            }
                        }
                    }

                    if (listSchema.Count > 0)
                    {
                        foreach (string s in listSchema)
                            parsedEntities.Add(s, new CubeProperty(null));
                    }
                }
                else
                {
                    foreach (KeyValuePair<string, object> p in de.Attributes)
                    {
                        parsedEntities.Add(p.Key, new CubeProperty(p, cube, de));
                    }
                }

                CubeEntity cubeE = new CubeEntity();
                cubeE.TableCubeEntity = parsedEntities;
                return new Result(false, string.Empty, cubeE, cube.LogSystem);
            }
            catch (Exception ex)
            {
                return new Result(true, ex.Message, null, cube.LogSystem);
            }
        }
    }
}

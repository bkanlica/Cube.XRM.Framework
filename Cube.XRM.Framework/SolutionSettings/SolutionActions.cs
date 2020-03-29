// ***********************************************************************
// Assembly         : Cube.XRM.Framework
// Author           : baris
// Created          : 03-28-2020
//
// Last Modified By : baris
// Last Modified On : 06-06-2017
// ***********************************************************************
// <copyright file="SolutionActions.cs" company="Microsoft Corporation">
//     Copyright © Microsoft Corporation 2015
// </copyright>
// <summary></summary>
// ***********************************************************************
using Microsoft.Xrm.Sdk;
using System;

namespace Cube.XRM.Framework.SolutionSettings
{
    /// <summary>
    /// Class SolutionActions.
    /// Implements the <see cref="Cube.XRM.Framework.CubeEntity" />
    /// </summary>
    /// <seealso cref="Cube.XRM.Framework.CubeEntity" />
    public class SolutionActions : CubeEntity
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        public string Identifier { get; set; }

        /// <summary>
        /// Gets or sets the created on.
        /// </summary>
        /// <value>The created on.</value>
        public DateTime CreatedOn { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SolutionActions" /> class.
        /// </summary>
        /// <param name="cubeBase">The cube base.</param>
        public SolutionActions(CubeBase cubeBase)
        {
            cube = cubeBase;
            EntityName = "mwns_mawenssolution";
        }

        /// <summary>
        /// Prepares the entity.
        /// </summary>
        /// <returns>Entity.</returns>
        public override AttributeCollection PrepareEntity()
        {
            AttributeCollection ac = new AttributeCollection();
            ac.Add("mwns_name", Name);
            ac.Add("mwns_identifier", Identifier);
            return ac;
        }

        /// <summary>
        /// Parses the item.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>IEntity.</returns>
        public override Result ParseItem(Entity entity)
        {
            try
            {
                SolutionActions item = new SolutionActions(cube);
                if (entity.Attributes.Contains("mwns_mawenssolutionid"))
                {
                    item.ID = new Guid(entity["mwns_mawenssolutionid"].ToString());
                }
                if (entity.Attributes.Contains("mwns_name"))
                {
                    item.Name = entity["mwns_identifier"].ToString();
                }
                if (entity.Attributes.Contains("mwns_identifier"))
                {
                    item.Identifier = entity["mwns_name"].ToString();
                }
                if (entity.Attributes.Contains("mwns_value"))
                {
                    item.CreatedOn = Convert.ToDateTime(entity["mwns_value"]);
                }

                return new Result(false, string.Empty, item, cube.LogSystem);
            }
            catch (Exception ex)
            {
                return new Result(true, ex.Message, null, cube.LogSystem);
            }
        }

        /// <summary>
        /// Gets the item.
        /// </summary>
        /// <returns>Result.</returns>
        public Result GetItem()
        {
            string fetch = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                              <entity name='mwns_mawenssolution'>
                                <attribute name='mwns_mawenssolutionid' />
                                <attribute name='createdon' />
                                <attribute name='mwns_identifier' />
                                <filter type='and'>
                                  <condition attribute='mwns_name' operator='eq' value='{0}' />
                                </filter>
                              </entity>
                            </fetch>";

            fetch = String.Format(fetch, Name);
            return cube.RetrieveActions.getItemFetch(fetch);
        }
    }
}

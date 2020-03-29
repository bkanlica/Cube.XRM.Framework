// ***********************************************************************
// Assembly         : Cube.XRM.Framework
// Author           : baris
// Created          : 03-28-2020
//
// Last Modified By : baris
// Last Modified On : 06-19-2017
// ***********************************************************************
// <copyright file="SolutionSettingActions.cs" company="Microsoft Corporation">
//     Copyright © Microsoft Corporation 2015
// </copyright>
// <summary></summary>
// ***********************************************************************
using Microsoft.Xrm.Sdk;
using System;

namespace Cube.XRM.Framework.SolutionSettings
{
    /// <summary>
    /// Class SolutionSettingActions.
    /// Implements the <see cref="Cube.XRM.Framework.CubeEntity" />
    /// </summary>
    /// <seealso cref="Cube.XRM.Framework.CubeEntity" />
    public class SolutionSettingActions : CubeEntity
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }
        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        public string Value { get; set; }
        /// <summary>
        /// Gets or sets the secure value.
        /// </summary>
        /// <value>The secure value.</value>
        public string SecureValue { get; set; }
        /// <summary>
        /// Gets or sets the solution identifier.
        /// </summary>
        /// <value>The solution identifier.</value>
        public Guid SolutionId { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SolutionSettingActions" /> class.
        /// </summary>
        /// <param name="cubeBase">The cube base.</param>
        public SolutionSettingActions(CubeBase cubeBase)
        {
            cube = cubeBase;
            EntityName = "mwns_mawenssolutionsetting";
        }

        /// <summary>
        /// Prepares the entity.
        /// </summary>
        /// <returns>Entity.</returns>
        public override AttributeCollection PrepareEntity()
        {
            AttributeCollection ac = new AttributeCollection();
            ac.Add("mwns_name", Name);
            ac.Add("mwns_value", Value);
            ac.Add("mwns_securevalue", SecureValue);
            ac.Add("mwns_mawenssolutionid", new EntityReference("mwns_mawenssolution", SolutionId));
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
                SolutionSettingActions item = new SolutionSettingActions(cube);
                if (entity.Attributes.Contains("mwns_mawenssolutionsettingid"))
                {
                    item.ID = new Guid(entity["mwns_mawenssolutionsettingid"].ToString());
                }
                if (entity.Attributes.Contains("mwns_name"))
                {
                    item.Name = entity["mwns_name"].ToString();
                }
                if (entity.Attributes.Contains("mwns_value"))
                {
                    item.Value = entity["mwns_value"].ToString();
                }
                if (entity.Attributes.Contains("mwns_securevalue"))
                {
                    item.SecureValue = entity["mwns_securevalue"].ToString();
                }
                if (entity.Attributes.Contains("mwns_mawenssolutionid"))
                {
                    item.SolutionId = ((EntityReference)entity["mwns_mawenssolutionid"]).Id;
                }

                return new Result(false, string.Empty, item, cube.LogSystem);
            }
            catch (Exception ex)
            {
                return new Result(true, ex.Message, null, cube.LogSystem);
            }
        }

        /// <summary>
        /// Gets the items.
        /// </summary>
        /// <returns>Result.</returns>
        public Result GetItems()
        {
            string fetch = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                              <entity name='mwns_mawenssolutionsetting'>
                                <attribute name='mwns_name' />
                                <attribute name='mwns_value' />
                                <attribute name='mwns_securevalue' />
                                <filter type='and'>
                                  <condition attribute='mwns_mawenssolutionid' operator='eq' value='{0}' />
                                </filter>
                              </entity>
                            </fetch>";

            fetch = String.Format(fetch, SolutionId);
            return cube.RetrieveActions.getItemsFetch(fetch);
        }

        /// <summary>
        /// Gets the license item.
        /// </summary>
        /// <returns>Result.</returns>
        public Result GetLicenseItem()
        {
            string fetch = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                              <entity name='mwns_mawenssolutionsetting'>
                                <attribute name='mwns_value' />
                                <attribute name='mwns_securevalue' />
                                <filter type='and'>
                                  <condition attribute='mwns_mawenssolutionid' operator='eq' value='{0}' />
                                  <condition attribute='mwns_name' operator='eq' value='Serial Key' />
                                </filter>
                              </entity>
                            </fetch>";

            fetch = String.Format(fetch, SolutionId);
            return cube.RetrieveActions.getItemFetch(fetch);
        }
    }
}

// ***********************************************************************
// Assembly         : Cube.XRM.Framework
// Author           : Baris Kanlica
// Created          : 09-21-2015
//
// Last Modified By : Baris Kanlica
// Last Modified On : 06-06-2017
// ***********************************************************************
// <copyright file="ISetting.cs" company="Microsoft Corporation">
//     Copyright © Microsoft Corporation 2015
// </copyright>
// <summary></summary>
// ************************************************************************
using System.Xml.Serialization;

/// <summary>
/// The Interfaces namespace.
/// </summary>
namespace Cube.XRM.Framework.Interfaces
{
    /// <summary>
    /// Interface ISetting
    /// </summary>
    public interface ISetting
    {
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        /// <value>The key.</value>
        [XmlElement("Key")]
        string Key { get; set; }
        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        [XmlElement("Value")]
        object Value { get; set; }
    }
}

// ***********************************************************************
// Assembly         : Cube.XRM.Framework.AddOn
// Author           : baris
// Created          : 03-28-2020
//
// Last Modified By : baris
// Last Modified On : 06-06-2017
// ***********************************************************************
// <copyright file="ContextWriter.cs" company="Microsoft Corporation">
//     Copyright © Microsoft Corporation 2015
// </copyright>
// <summary></summary>
// ***********************************************************************
using Microsoft.Xrm.Sdk;
using System.Text;

namespace Cube.XRM.Framework.AddOn
{
    /// <summary>
    /// Class ContextWriter.
    /// </summary>
    public static class ContextWriter
    {
        /// <summary>
        /// Writes the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>System.String.</returns>
        public static string Write(IExecutionContext context)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine();
            stringBuilder.AppendLine("+++ Start PlugIn Context Info +++");
            stringBuilder.AppendLine();
            stringBuilder.AppendLine(string.Format("UserId: {0}", context.UserId));
            stringBuilder.AppendLine(string.Format("OrganizationId: {0}", context.OrganizationId));
            stringBuilder.AppendLine(string.Format("OrganizationName: {0}", context.OrganizationName));
            stringBuilder.AppendLine(string.Format("MessageName: {0}", context.MessageName));
            stringBuilder.AppendLine(string.Format("Mode: {0}", context.Mode));
            stringBuilder.AppendLine(string.Format("PrimaryEntityName: {0}", context.PrimaryEntityName));
            stringBuilder.AppendLine(string.Format("SecondaryEntityName: {0}", context.SecondaryEntityName));
            stringBuilder.AppendLine(string.Format("BusinessUnitId: {0}", context.BusinessUnitId));
            stringBuilder.AppendLine(string.Format("CorrelationId: {0}", context.CorrelationId));
            stringBuilder.AppendLine(string.Format("Depth: {0}", context.Depth));
            stringBuilder.AppendLine(string.Format("InitiatingUserId: {0}", context.InitiatingUserId));
            stringBuilder.AppendLine(string.Format("IsExecutingOffline: {0}", context.IsExecutingOffline));
            stringBuilder.AppendLine(string.Format("IsInTransaction: {0}", context.IsInTransaction));
            stringBuilder.AppendLine(string.Format("IsolationMode: {0}", context.IsolationMode));
            stringBuilder.AppendLine(string.Format("Mode: {0}", context.Mode));
            stringBuilder.AppendLine(string.Format("OperationCreatedOn: {0}", context.OperationCreatedOn.ToString()));
            stringBuilder.AppendLine(string.Format("OperationId: {0}", context.OperationId));
            stringBuilder.AppendLine(string.Format("PrimaryEntityId: {0}", context.PrimaryEntityId));
            //stringBuilder.AppendLine(string.Format("OwningExtension LogicalName: {0}", context.OwningExtension.LogicalName));
            //stringBuilder.AppendLine(string.Format("OwningExtension Name: {0}", context.OwningExtension.Name));
            //stringBuilder.AppendLine(string.Format("OwningExtension Id: {0}", context.OwningExtension.Id));
            stringBuilder.AppendLine(string.Format("SharedVariables: {0}", (context.SharedVariables == null) ? "NULL" : Serializers.SerializeHelper.SerializeParameterCollection(context.SharedVariables)));
            stringBuilder.AppendLine(string.Format("InputParameters: {0}", (context.InputParameters == null) ? "NULL" : Serializers.SerializeHelper.SerializeParameterCollection(context.InputParameters)));
            stringBuilder.AppendLine(string.Format("OutputParameters: {0}", (context.OutputParameters == null) ? "NULL" : Serializers.SerializeHelper.SerializeParameterCollection(context.OutputParameters)));
            stringBuilder.AppendLine(string.Format("PreEntityImages: {0}", (context.PreEntityImages == null) ? "NULL" : Serializers.SerializeHelper.SerializeEntityImageCollection(context.PreEntityImages)));
            stringBuilder.AppendLine(string.Format("PostEntityImages: {0}", (context.PostEntityImages == null) ? "NULL" : Serializers.SerializeHelper.SerializeEntityImageCollection(context.PostEntityImages)));
            stringBuilder.AppendLine();
            stringBuilder.AppendLine("+++ End PlugIn Context Info +++");
            stringBuilder.AppendLine();
            return stringBuilder.ToString();
        }
    }



}

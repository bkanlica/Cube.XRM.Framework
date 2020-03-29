// ***********************************************************************
// Assembly         : Cube.XRM.Framework.AddOn
// Author           : Baris Kanlica
// Created          : 09-21-2015
//
// Last Modified By : Baris Kanlica
// Last Modified On : 03-29-2020
// ***********************************************************************
// <copyright file="PluginManager.cs" company="Microsoft Corporation">
//     Copyright © Microsoft Corporation 2015
// </copyright>
// <summary></summary>
// ***********************************************************************
using Microsoft.Xrm.Sdk;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.ServiceModel;

/// <summary>
/// The AddOn namespace.
/// </summary>
namespace Cube.XRM.Framework.AddOn
{
    /// <summary>
    /// Class PluginManager.
    /// </summary>
    public abstract class PluginManager : IPlugin
    {
        /// <summary>
        /// The cube base
        /// </summary>
        private CubeBase cubeBase = new CubeBase();

        /// <summary>
        /// Gets or sets the target entity.
        /// </summary>
        /// <value>The target entity.</value>
        public Entity TargetEntity
        {
            get
            {
                if (cubeBase.Context != null &&
                    ((IPluginExecutionContext)cubeBase.Context).InputParameters.Contains("Target") &&
                    ((IPluginExecutionContext)cubeBase.Context).InputParameters["Target"] is Entity)
                {
                    return (Entity)((IPluginExecutionContext)cubeBase.Context).InputParameters["Target"];
                }
                return null;
            }
            set
            {
                ((IPluginExecutionContext)cubeBase.Context).InputParameters["Target"] = value;
            }
        }

        /// <summary>
        /// Gets the pre image.
        /// </summary>
        /// <value>The pre image.</value>
        public Entity PreImage
        {
            get
            {
                if (cubeBase.Context != null &&
                    ((IPluginExecutionContext)cubeBase.Context).PreEntityImages != null &&
                    ((IPluginExecutionContext)cubeBase.Context).PreEntityImages.Count > 0)
                {
                    return ((IPluginExecutionContext)cubeBase.Context).PreEntityImages.Values.FirstOrDefault();
                }
                return null;
            }
        }

        /// <summary>
        /// Gets the post image.
        /// </summary>
        /// <value>The post image.</value>
        public Entity PostImage
        {
            get
            {
                if (cubeBase.Context != null &&
                   ((IPluginExecutionContext)cubeBase.Context).PostEntityImages != null &&
                    ((IPluginExecutionContext)cubeBase.Context).PostEntityImages.Count > 0)
                {
                    return ((IPluginExecutionContext)cubeBase.Context).PostEntityImages.Values.FirstOrDefault();
                }
                return null;
            }
        }

        /// <summary>
        /// The un secure string
        /// </summary>
        public readonly string UnSecureString;
        /// <summary>
        /// The secure string
        /// </summary>
        public readonly string SecureString;

        /// <summary>
        /// Initializes a new instance of the <see cref="PluginManager" /> class.
        /// </summary>
        /// <param name="_unsecureString">The _unsecure string.</param>
        /// <param name="_secureString">The _secure string.</param>
        public PluginManager(string _unsecureString, string _secureString)
        {
            UnSecureString = _unsecureString;
            SecureString = _secureString;
        }
        /// <summary>
        /// Executes the specified cube base.
        /// </summary>
        /// <param name="cubeBase">The cube base.</param>
        protected abstract void Execute(CubeBase cubeBase);

        /// <summary>
        /// Executes the specified service provider.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        /// <exception cref="ArgumentNullException">serviceProvider is null</exception>
        /// <exception cref="InvalidPluginExecutionException"></exception>
        /// <exception cref="InvalidPluginExecutionException"></exception>
        /// <exception cref="InvalidPluginExecutionException"></exception>
        /// <exception cref="System.ArgumentNullException">serviceProvider is null</exception>
        /// <exception cref="Microsoft.Xrm.Sdk.InvalidPluginExecutionException"></exception>
        public void Execute(IServiceProvider serviceProvider)
        {
            var watch = Stopwatch.StartNew();
            
            try
            {
                if (serviceProvider == null)
                {
                    throw new ArgumentNullException("serviceProvider is null");
                }

                // Obtain the tracing service from the service provider.
                cubeBase.LogSystem = new DetailedLog() { TraceService = (ITracingService)serviceProvider.GetService(typeof(ITracingService)) };

                cubeBase.LogSystem.CreateLog(string.Format(CultureInfo.InvariantCulture, "Entered the Execute() method : {0}", this.GetType().ToString()));

                // Obtain the execution context from the service provider.
                cubeBase.Context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

                // Use the factory to generate the Organization Service.
                IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
                cubeBase.XrmService = serviceFactory.CreateOrganizationService(((IPluginExecutionContext)cubeBase.Context).UserId);

                cubeBase.BaseSystemObject = serviceProvider;

                Execute(cubeBase);
            }
            catch (FaultException<Microsoft.Xrm.Sdk.OrganizationServiceFault> ex)
            {
                cubeBase.LogSystem.CreateLog("The application terminated with an Organization Service Fault error.");
                cubeBase.LogSystem.CreateLog(string.Format("Timestamp: {0}", ex.Detail.Timestamp));
                cubeBase.LogSystem.CreateLog(string.Format("Code: {0}", ex.Detail.ErrorCode));
                cubeBase.LogSystem.CreateLog(string.Format("Message: {0}", ex.Detail.Message));
                cubeBase.LogSystem.CreateLog(string.Format("Inner Fault: {0}",
                    null == ex.Detail.InnerFault ? "No Inner Fault" : "Has Inner Fault"));

                throw new InvalidPluginExecutionException(ex.Message, ex);
            }
            catch (System.TimeoutException ex)
            {
                cubeBase.LogSystem.CreateLog("The application terminated with an timeout error.");
                cubeBase.LogSystem.CreateLog(string.Format("Message: {0}", ex.Message));
                cubeBase.LogSystem.CreateLog(string.Format("Stack Trace: {0}", ex.StackTrace));
                cubeBase.LogSystem.CreateLog(string.Format("Inner Fault: {0}",
                    null == ex.InnerException.Message ? "No Inner Fault" : ex.InnerException.Message));

                throw new InvalidPluginExecutionException(ex.Message, ex);
            }
            catch (System.Exception ex)
            {
                cubeBase.LogSystem.CreateLog(string.Format(CultureInfo.InvariantCulture, "General Exception with message: {0}", ex.Message));
                if (ex.InnerException != null)
                {
                    cubeBase.LogSystem.CreateLog("Inner Exception Message:" + ex.InnerException.Message);

                    FaultException<Microsoft.Xrm.Sdk.OrganizationServiceFault> fe = ex.InnerException
                        as FaultException<Microsoft.Xrm.Sdk.OrganizationServiceFault>;
                    if (fe != null)
                    {
                        cubeBase.LogSystem.CreateLog(string.Format("Fault Exception Timestamp: {0}", fe.Detail.Timestamp));
                        cubeBase.LogSystem.CreateLog(string.Format("Fault Exception Code: {0}", fe.Detail.ErrorCode));
                        cubeBase.LogSystem.CreateLog(string.Format("Fault Exception Message: {0}", fe.Detail.Message));
                        cubeBase.LogSystem.CreateLog(string.Format("Fault Exception Trace: {0}", fe.Detail.TraceText));
                        cubeBase.LogSystem.CreateLog(string.Format("Inner Fault: {0}", null == fe.Detail.InnerFault ? "No Inner Fault" : "Has Inner Fault"));
                    }
                }

                throw new InvalidPluginExecutionException(ex.Message, ex);
            }
            finally
            {
                watch.Stop();
                cubeBase.LogSystem.CreateLog(ContextWriter.Write((IPluginExecutionContext)cubeBase.Context));
                cubeBase.LogSystem.CreateLog(string.Format(CultureInfo.InvariantCulture, "Finished the Execute() method : {0}", this.GetType().ToString()));
                cubeBase.LogSystem.CreateLog(string.Format(CultureInfo.InvariantCulture, "Internal execution time: {0} ms", watch.ElapsedMilliseconds));
            }
        }
    }
}

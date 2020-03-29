// ***********************************************************************
// Assembly         : Cube.XRM.Framework.AddOn
// Author           : Baris Kanlica
// Created          : 09-21-2015
//
// Last Modified By : Baris Kanlica
// Last Modified On : 03-29-2020
// ***********************************************************************
// <copyright file="WorkflowManager.cs" company="Microsoft Corporation">
//     Copyright © Microsoft Corporation 2015
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Linq;
using System.Activities;
using Microsoft.Xrm.Sdk.Workflow;
using System.Globalization;
using Microsoft.Xrm.Sdk;
using System.ServiceModel;
using System.Diagnostics;

/// <summary>
/// The AddOn namespace.
/// </summary>
namespace Cube.XRM.Framework.AddOn
{
    /// <summary>
    /// Class WorkflowManager.
    /// </summary>
    public abstract class WorkflowManager : CodeActivity
    {
        /// <summary>
        /// The cube base
        /// </summary>
        private CubeBase cubeBase = new CubeBase();

        /// <summary>
        /// Gets or sets the throw exception.
        /// </summary>
        /// <value>The throw exception.</value>
        [Input("Throw Exception"), Default("true")]
        public InArgument<bool> ThrowException { get; set; }

        /// <summary>
        /// Gets or sets the detailed exception status.
        /// </summary>
        /// <value>The detailed exception</value>
        [Input("Show Detailed Exception Report"), Default("false")]
        public InArgument<bool> DetailedException { get; set; }

        /// <summary>
        /// Gets or sets the is exception occured.
        /// </summary>
        /// <value>The is exception occured.</value>
        [Output("Is Exception Occured"), Default("false")]
        public OutArgument<bool> IsExceptionOccured { get; set; }

        /// <summary>
        /// Gets or sets the exception message.
        /// </summary>
        /// <value>The exception message.</value>
        [Output("Exception Message")]
        public OutArgument<string> ExceptionMessage { get; set; }

        /// <summary>
        /// Executes the specified cube base.
        /// </summary>
        /// <param name="cubeBase">The cube base.</param>
        protected abstract void Execute(CubeBase cubeBase);

        /// <summary>
        /// Parses the URL.
        /// </summary>
        /// <param name="URL">The URL.</param>
        /// <returns>Tuple&lt;Guid, System.Int32&gt;.</returns>
        public Tuple<Guid, int> ParseURL(string URL)
        {
            string[] urlParts = URL.Split("?".ToArray());
            string[] urlParams = urlParts[1].Split("&".ToCharArray());
            string objectTypeCode = urlParams[0].Replace("etc=", "");
            string id = urlParams[1].Replace("id=", "");

            return new Tuple<Guid, int>(new Guid(id), Convert.ToInt32(objectTypeCode));
        }

        /// <summary>
        /// Gets the code activity parameter.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parameter">The parameter.</param>
        /// <returns>T.</returns>
        public T GetCodeActivityParameter<T>(InArgument<T> parameter)
        {
            T result = parameter.Get((CodeActivityContext)cubeBase.BaseSystemObject);
            return result;
        }

        /// <summary>
        /// Sets the code activity parameter.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parameter">The parameter.</param>
        /// <param name="value">The value.</param>
        public void SetCodeActivityParameter<T>(OutArgument<T> parameter, T value)
        {
            parameter.Set((CodeActivityContext)cubeBase.BaseSystemObject, value);
        }

        /// <summary>
        /// Executes the specified execution context.
        /// </summary>
        /// <param name="executionContext">The execution context.</param>
        /// <exception cref="ArgumentNullException">ExecutionContext is null</exception>
        /// <exception cref="InvalidPluginExecutionException"></exception>
        /// <exception cref="InvalidPluginExecutionException"></exception>
        /// <exception cref="InvalidPluginExecutionException"></exception>
        /// <exception cref="System.ArgumentNullException">ExecutionContext is null</exception>
        /// <exception cref="Microsoft.Xrm.Sdk.InvalidPluginExecutionException"></exception>
        protected override void Execute(CodeActivityContext executionContext)
        {
            var watch = Stopwatch.StartNew();
            try
            {
                if (executionContext == null)
                {
                    throw new ArgumentNullException("ExecutionContext is null");
                }

                // Obtain the tracing service from the service provider.
                cubeBase.LogSystem = new DetailedLog() { TraceService = executionContext.GetExtension<ITracingService>() };

                cubeBase.LogSystem.CreateLog(string.Format(CultureInfo.InvariantCulture, "Entered the Main Execute() method : {0}", this.GetType().ToString()));

                // Obtain the execution context from the service provider.
                cubeBase.Context = executionContext.GetExtension<IWorkflowContext>();

                // Use the factory to generate the Organization Service.
                IOrganizationServiceFactory ServiceFactory = executionContext.GetExtension<IOrganizationServiceFactory>();
                cubeBase.XrmService = ServiceFactory.CreateOrganizationService(((IWorkflowContext)cubeBase.Context).UserId);

                cubeBase.BaseSystemObject = executionContext;

                cubeBase.LogSystem.CreateLog(string.Format("Entered the Execute() method"));
                Execute(cubeBase);
                cubeBase.LogSystem.CreateLog(string.Format("Exited the Execute() method"));
            }
            catch (FaultException<Microsoft.Xrm.Sdk.OrganizationServiceFault> ex)
            {
                cubeBase.LogSystem.CreateLog("The application terminated with an Organization Service Fault error.");
                cubeBase.LogSystem.CreateLog(string.Format("Timestamp: {0}", ex.Detail.Timestamp));
                cubeBase.LogSystem.CreateLog(string.Format("Code: {0}", ex.Detail.ErrorCode));
                cubeBase.LogSystem.CreateLog(string.Format("Message: {0}", ex.Detail.Message));
                cubeBase.LogSystem.CreateLog(string.Format("Inner Fault: {0}",
                    null == ex.Detail.InnerFault ? "No Inner Fault" : "Has Inner Fault"));

                IsExceptionOccured.Set(executionContext, true);
                ExceptionMessage.Set(executionContext, ex.Message);

                if (ThrowException.Get<bool>(executionContext))
                {
                    throw new InvalidPluginExecutionException(ex.Message, ex);
                }
            }
            catch (System.TimeoutException ex)
            {
                cubeBase.LogSystem.CreateLog("The application terminated with an timeout error.");
                cubeBase.LogSystem.CreateLog(string.Format("Message: {0}", ex.Message));
                cubeBase.LogSystem.CreateLog(string.Format("Stack Trace: {0}", ex.StackTrace));
                cubeBase.LogSystem.CreateLog(string.Format("Inner Fault: {0}",
                    null == ex.InnerException.Message ? "No Inner Fault" : ex.InnerException.Message));

                IsExceptionOccured.Set(executionContext, true);
                ExceptionMessage.Set(executionContext, ex.Message);

                if (ThrowException.Get<bool>(executionContext))
                {
                    throw new InvalidPluginExecutionException(ex.Message, ex);
                }
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

                IsExceptionOccured.Set(executionContext, true);
                ExceptionMessage.Set(executionContext, ex.Message);

                if (ThrowException.Get<bool>(executionContext))
                {
                    throw new InvalidPluginExecutionException(ex.Message, ex);
                }
            }
            finally
            {
                if (DetailedException.Get<bool>(executionContext))
                    cubeBase.LogSystem.CreateLog(ContextWriter.Write((IWorkflowContext)cubeBase.Context));

                watch.Stop();
                cubeBase.LogSystem.CreateLog(string.Format(CultureInfo.InvariantCulture, "Finished the Execute() method : {0}", this.GetType().ToString()));
                cubeBase.LogSystem.CreateLog(string.Format(CultureInfo.InvariantCulture, "Internal execution time: {0} ms", watch.ElapsedMilliseconds));
            }
        }
    }
}


// ***********************************************************************
// Assembly         : Cube.XRM.Framework.AddOn
// Author           : baris
// Created          : 03-28-2020
//
// Last Modified By : baris
// Last Modified On : 06-19-2017
// ***********************************************************************
// <copyright file="CheckAndGenerateSolutionSettings.cs" company="Microsoft Corporation">
//     Copyright © Microsoft Corporation 2015
// </copyright>
// <summary></summary>
// ***********************************************************************
using Cube.XRM.Framework.SolutionSettings;
using Microsoft.Xrm.Sdk;
using System;

namespace Cube.XRM.Framework.AddOn
{
    /// <summary>
    /// Class CheckAndGenerateSolutionSettings.
    /// </summary>
    public class CheckAndGenerateSolutionSettings
    {
        /// <summary>
        /// Executes the specified cube.
        /// </summary>
        /// <param name="cube">The cube.</param>
        /// <param name="SolutionName">Name of the solution.</param>
        /// <returns>Result.</returns>
        /// <exception cref="Exception"></exception>
        /// <exception cref="Exception"></exception>
        /// <exception cref="Exception">couldn't read Mawens solution settings</exception>
        /// <exception cref="Exception">couldn't read Mawens solution</exception>
        public static Result Execute(CubeBase cube, string SolutionName)
        {
            try
            {
                Result resultSolution = retrieveSolution(cube, SolutionName);
                if (resultSolution.isError)
                    throw new Exception(resultSolution.Message);

                if (resultSolution.BusinessObject != null)
                {
                    SolutionActions sa = (SolutionActions)resultSolution.BusinessObject;
                    Result resultSolutionSettings = retrieveSolutionSettings(cube, sa.ID);
                    if (resultSolutionSettings.isError)
                        throw new Exception(resultSolutionSettings.Message);

                    if (resultSolution.BusinessObject != null)
                        return resultSolutionSettings;
                    else
                        throw new Exception("couldn't read Mawens solution settings");
                }
                else
                    throw new Exception("couldn't read Mawens solution");
            }
            catch (Exception ex)
            {
                return new Result(true, ex.Message, null, cube.LogSystem);
            }
        }

        /// <summary>
        /// Retrieves the solution.
        /// </summary>
        /// <param name="cube">The cube.</param>
        /// <param name="SolutionName">Name of the solution.</param>
        /// <returns>Result.</returns>
        /// <exception cref="Exception"></exception>
        private static Result retrieveSolution(CubeBase cube, string SolutionName)
        {
            try
            {
                SolutionActions solAct = new SolutionActions(cube);
                solAct.Name = SolutionName;
                Result resultSolution = solAct.GetItem();
                if (resultSolution.isError)
                    throw new Exception(resultSolution.Message);

                if (resultSolution.BusinessObject != null)
                {
                    Entity e = (Entity)resultSolution.BusinessObject;
                    SolutionActions sa = new SolutionActions(cube);
                    if (e.Contains("mwns_name"))
                        sa.Name = e["mwns_name"].ToString();
                    if (e.Contains("mwns_mawenssolutionid"))
                        sa.ID = new Guid(e["mwns_mawenssolutionid"].ToString());

                    return new Result(false, string.Empty, sa, cube.LogSystem);
                }
                else
                    return CreateSolution(cube, SolutionName);
            }
            catch (Exception ex)
            {
                return new Result(true, ex.Message, null, cube.LogSystem);
            }
        }

        /// <summary>
        /// Creates the solution.
        /// </summary>
        /// <param name="cube">The cube.</param>
        /// <param name="SolutionName">Name of the solution.</param>
        /// <returns>Result.</returns>
        /// <exception cref="Exception"></exception>
        private static Result CreateSolution(CubeBase cube, string SolutionName)
        {
            try
            {
                SolutionActions solAct = new SolutionActions(cube);
                solAct.Name = SolutionName;
                Result result = solAct.Create();
                if (result.isError)
                    throw new Exception(result.Message);

                return new Result(false, string.Empty, solAct, cube.LogSystem);
            }
            catch (Exception ex)
            {
                return new Result(true, ex.Message, null, cube.LogSystem);
            }
        }

        /// <summary>
        /// Retrieves the solution settings.
        /// </summary>
        /// <param name="cube">The cube.</param>
        /// <param name="SolutionId">The solution identifier.</param>
        /// <returns>Result.</returns>
        /// <exception cref="Exception"></exception>
        private static Result retrieveSolutionSettings(CubeBase cube, Guid SolutionId)
        {
            try
            {
                SolutionSettingActions solAct = new SolutionSettingActions(cube);
                solAct.SolutionId = SolutionId;
                Result resultSolutionSettings = solAct.GetLicenseItem();
                if (resultSolutionSettings.isError)
                    throw new Exception(resultSolutionSettings.Message);

                if (resultSolutionSettings.BusinessObject != null)
                {
                    Entity e = (Entity)resultSolutionSettings.BusinessObject;
                    SolutionSettingActions ssa = new SolutionSettingActions(cube);
                    if (e.Contains("mwns_name"))
                        ssa.Name = e["mwns_name"].ToString();
                    if (e.Contains("mwns_value"))
                        ssa.Value = e["mwns_value"].ToString();

                    return new Result(false, string.Empty, ssa, cube.LogSystem);

                }
                else
                    return CreateSolutionSettings(cube, SolutionId);
            }
            catch (Exception ex)
            {
                return new Result(true, ex.Message, null, cube.LogSystem);
            }
        }

        /// <summary>
        /// Creates the solution settings.
        /// </summary>
        /// <param name="cube">The cube.</param>
        /// <param name="SolutionId">The solution identifier.</param>
        /// <returns>Result.</returns>
        /// <exception cref="Exception"></exception>
        private static Result CreateSolutionSettings(CubeBase cube, Guid SolutionId)
        {
            try
            {
                SolutionSettingActions solAct = new SolutionSettingActions(cube);
                solAct.SolutionId = SolutionId;
                solAct.Name = "Serial Key";
                solAct.Value = "Trial";
                Result result = solAct.Create();
                if (result.isError)
                    throw new Exception(result.Message);

                return new Result(false, string.Empty, solAct, cube.LogSystem);
            }
            catch (Exception ex)
            {
                return new Result(true, ex.Message, null, cube.LogSystem);
            }
        }

    }
}


/*
 * Created by SharpDevelop.
 * User: thief
 * Date: 2010/06/05
 * Time: 11:49 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.Common;
using System.Data.SqlTypes;

namespace FeihtWorx.Data
{
	/// <summary>
	/// Description of DataWorkerTask.
	/// </summary>
	public class DataWorkerTask
	{
		public System.Data.CommandType CommandType { get; set; }
		public Transaction Transaction { get; set; }
		public String CommandText { get; set; }
		public int? CommandTimeout { get; set; }
		public bool ReadResults { get; set; }
		public DataWorkerMode Mode { get; set; }
		public bool ApplyOutputParametersToResults { get; set; }
		
		public int RowsAffected { get; internal set; }
		internal Dictionary<string,DbParameter> ParamsByName { get; set; }
		
		public DataWorkerTask()
		{
			CommandType = CommandType.StoredProcedure;
			CommandTimeout = null;//30;
			CommandText = null;
			Transaction = null;
			ReadResults = false;
			Mode = DataWorkerMode.DataFields;
			RowsAffected = Constants.KnownBadStateForRowsAffected;
			ApplyOutputParametersToResults = false;
		}
		
	}
}

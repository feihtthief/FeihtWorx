/*
 * Created by SharpDevelop.
 * User: thief
 * Date: 2010/06/05
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Reflection;
using System.Data;
using System.Data.Common;
//using System.Xml.Linq;
using FeihtWorx.Util;
using log4net;
using System.Collections.Generic;

//using System.Data.SqlTypes;


namespace FeihtWorx.Data
{
	/// <summary>
	/// Description of DataWorker.
	/// </summary>
	public class DataWorker
	{
		private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
		
		public String ProviderName { get; private set; }
		public String ConnectionString { get; private set; }
		public int DefaultCommandTimeout { get; private set; }
		// todo: persistent connection boolean ??? // might needs MARS or otherwise might not be a good idea
		
		private DbProviderFactory Factory;
		public DataWorker(String providerName, String connectionString)
			: this(providerName, connectionString, 30)
		{
		}
		
		public DataWorker(String providerName, String connectionString, int defaultCommandTimeout)
		{
			if (log.IsDebugEnabled) {
				log.Debug("Creating Dataworker");
			}
			ProviderName = providerName;
			ConnectionString = connectionString;
			this.DefaultCommandTimeout = defaultCommandTimeout;
			Factory = DbProviderFactories.GetFactory(ProviderName);
		}
		
		public Transaction BeginTransaction()
		{
			var conn = MakeUsableConnectionFromScratch();
			var tran = conn.BeginTransaction();
			return new Transaction(tran);
		}
		
		//		public List<T> DoWorkXXX<T>(DataWorkerTask dataWorkerTask) {
		//			return DoWork<T>(dataWorkerTask);
		//		}
		//
		//		public List<T> DoWorkXXX<T>(DataWorkerTask dataWorkerTask,object input) {
		//			return DoWorkDirect<T>(dataWorkerTask,input);
		//		}

		public List<T> DoWorkDirect<T>(DataWorkerTask dataWorkerTask)
		{
			return DoWorkDirect<T>(dataWorkerTask, null);
		}
		
		public List<T> DoWorkDirect<T>(DataWorkerTask dataWorkerTask, object input)
		{
			if (log.IsDebugEnabled) {
				log.Debug("=== Starting DataWorker task ===");
			}
//			switch (dataWorkerTask.Mode) {
//				case DataWorkerMode.AllProperties:
//				case DataWorkerMode.Dictionary:
//				case DataWorkerMode.DataFields:
//					break;
//				default:
//					throw new ArgumentException("Unkown dataWorkerTask.Mode while applying outputData to input object", "dataWorkerTask.Mode");
//			}
			var result = new List<T>();
			DbConnection conn = GetConnectionForWorker(dataWorkerTask);
			var cmd = CreateCommand(conn, dataWorkerTask);
			BuildParameters(cmd, dataWorkerTask, input);
			//=== get list of input fields
			DataDictionary inputData = MakeInputData(dataWorkerTask, input);
			if (log.IsDebugEnabled) {
				foreach (var pair in inputData) {
					log.DebugFormat("{0} - {1}", pair.Key, pair.Value);
				}
			}
			//=== bind input to Parameters
			BindInputDataToParameters(inputData, dataWorkerTask);
			//===================
			if (dataWorkerTask.ReadResults) {
				using (var reader = cmd.ExecuteReader()) {
					if (reader.HasRows) {
						// === get list of output fields
						var columnNamesToIndexMappings = GetOutputColumns(reader);
						//===================
						
						var transferList = GetTransferList<T>(dataWorkerTask, columnNamesToIndexMappings, input);
						foreach (var item in transferList) {
							if (log.IsDebugEnabled) {
								log.DebugFormat("column {0} goes to property {1}", item.Value, item.Key);
							}
						}
						//===================
						while (reader.Read()) {
							var newEntry = Activator.CreateInstance<T>();
							// == read fields off reader onto new object
							foreach (var item in transferList) {
								if (!reader.IsDBNull(item.Value)) {
									item.Key.SetValue(newEntry, reader.GetValue(item.Value), null);
								}
							}
							result.Add(newEntry);
						}
					}
					
					dataWorkerTask.RowsAffected = reader.RecordsAffected;
				}
				
			} else {
				dataWorkerTask.RowsAffected = cmd.ExecuteNonQuery();
			}
			// === process output parameters
			//===================
			if ((input != null) && (cmd.Parameters.Count > 0)) {
				//===================
				DataDictionary outputData = BuildOutputData(cmd);
				if (log.IsDebugEnabled) {
					foreach (var pair in outputData) {
						log.DebugFormat("{0} out as {1}", pair.Key, pair.Value);
					}
				}
				//===================
				ApplyOutputDataToInputObject(outputData, dataWorkerTask, input);
				if (dataWorkerTask.ApplyOutputParametersToResults) {
					var outputMap = BuildOutputMap<T>(outputData);
					foreach (var element in result) {
						foreach (var pair in outputMap) {
							pair.Key.SetValue(element, pair.Value, null);
						}
					}
				}
			}
			//===================
			return result;
		}
		
		List<PropertyValuePair> BuildOutputMap<T>(DataDictionary outputData)
		{
			var result = new List<PropertyValuePair>();
			var props = typeof(T).GetProperties();
			foreach (var prop in props) {
				var cleanName = prop.Name.ToUpper();
				object value;
				if (outputData.TryGetValue(cleanName, out value)) {
					result.Add(new PropertyValuePair(prop, value));
				}
			}
			return result;
		}

		void ApplyOutputDataToInputObject(DataDictionary outputData, DataWorkerTask dataWorkerTask, object input)
		{
			switch (dataWorkerTask.Mode) {
				case DataWorkerMode.DataFields:
					var unfilteredProps = input.GetType().GetProperties();
					foreach (var prop in unfilteredProps) {
						if (prop.CanWrite) {
							var attr = AttributeHelper.GetFirstPropertyAttribute<DataFieldAttribute>(prop);
							if (attr != null) {
								object value;
								var name = (attr.FieldName ?? prop.Name).ToUpper();
								if (outputData.TryGetValue(name, out value)) {
									prop.SetValue(input, value, null);
								}
							}
						}
					}

					break;
				case DataWorkerMode.AllProperties:
					var allProps = input.GetType().GetProperties();
					foreach (var prop in allProps) {
						{
							object value;
							if (prop.CanWrite) {
								if (outputData.TryGetValue(prop.Name.ToUpper(), out value)) {
									prop.SetValue(input, value, null);
								}
							}
						}
					}

					break;
				case DataWorkerMode.Dictionary:
					var dict = (DataDictionary)input;
					foreach (var pair in outputData) {
						dict[pair.Key] = pair.Value;
						
					}
					break;
					
				default:
					throw new ArgumentException("Unkown dataWorkerTask.mode while applying outputData to input object", "dataWorkerTask.mode");
			}
		}

		DataDictionary BuildOutputData(DbCommand cmd)
		{
			var outputData = new DataDictionary();
			for (int i = 0; i < cmd.Parameters.Count; i++) {
				var parm = cmd.Parameters[i];
				if ((parm.Direction == ParameterDirection.InputOutput) || (parm.Direction == ParameterDirection.Output) || (parm.Direction == ParameterDirection.ReturnValue)) {
					var cleanName = parm.ParameterName.ToUpper();
					if (cleanName.StartsWith("@")) {
						cleanName = cleanName.Remove(0, 1);
					}
					outputData[cleanName] = parm.Value;
				}
			}
			return outputData;
		}

		List<PropertyIndexPair> GetTransferList<T>(DataWorkerTask dataWorkerTask, Dictionary<string, int> columnNamesToIndexMappings, object input)
		{
			// todo: rework to DataWorkerOutputMode
			switch (dataWorkerTask.Mode) {
				case DataWorkerMode.DataFields:
					return GetTransferListDatafield<T>(columnNamesToIndexMappings);
				case DataWorkerMode.AllProperties:
					return GetTransferListAllProperties<T>(columnNamesToIndexMappings);
				case DataWorkerMode.Dictionary:
					//return GetTransferListAllProperties<T>(columnNamesToIndexMappings);
					return GetTransferListDatafield<T>(columnNamesToIndexMappings);
			//	return GetTransferListDataDictionary(dataWorkerTask,columnNamesToIndexMappings,input);
				default:
					throw new ArgumentException("dataWorkerTask.Mode is invalid", "dataWorkerTask.Mode");

			}
		}

		List<PropertyIndexPair> GetTransferListDatafield<T>(Dictionary<string, int> columnNamesToIndexMappings)
		{
			var transferList = new List<PropertyIndexPair>();
			var t = typeof(T);
			var props = t.GetProperties();
			foreach (var prop in props) {
				{
					if (prop.CanWrite) {
						var attr = AttributeHelper.GetFirstPropertyAttribute<DataFieldAttribute>(prop);
						if (attr != null) {
							var name = (attr.FieldName ?? prop.Name).ToUpper();
							int index;
							if (columnNamesToIndexMappings.TryGetValue(name, out index)) {
								transferList.Add(new PropertyIndexPair(prop, index));
							}
							if (log.IsDebugEnabled) {
								log.DebugFormat("property {0} as name {1}", prop.Name, name);
							}
						}
					}
				}
			}
			return transferList;
		}

		List<PropertyIndexPair> GetTransferListAllProperties<T>(Dictionary<string, int> columnNamesToIndexMappings)
		{
			var transferList = new List<PropertyIndexPair>();
			var t = typeof(T);
			var props = t.GetProperties();
			foreach (var prop in props) {
				{
					if (prop.CanWrite) {
						var name = (prop.Name).ToUpper();
						int index;
						if (columnNamesToIndexMappings.TryGetValue(name, out index)) {
							transferList.Add(new PropertyIndexPair(prop, index));
						}
						if (log.IsDebugEnabled) {
							log.DebugFormat("property {0} as name {1}", prop.Name, name);
						}
					}
				}
			}
			return transferList;
		}
		
		//		// Bad Idea	- abandon all hope
		//		List<PropertyIndexPair> GetTransferListDataDictionary(DataWorkerTask dataWorkerTask, Dictionary<string, int> columnNamesToIndexMappings, object input)
		//		{
		//			var transferList = new List<PropertyIndexPair>();
		//			var dict = (DataDictionary)input;
		//			foreach (var pair in dict) {
		//			}
		////
		////			var t = typeof(T);
		////			var props = t.GetProperties();
		////			foreach (var prop in props) {
		////				{
		////					if (prop.CanWrite) {
		////						var name = (prop.Name).ToUpper();
		////						int index;
		////						if (columnNamesToIndexMappings.TryGetValue(name, out index)) {
		////							transferList.Add(new PropertyIndexPair(prop, index));
		////						}
		////						if (log.IsDebugEnabled) {
		////							log.DebugFormat("property {0} as name {1}", prop.Name, name);
		////						}
		////					}
		////				}
		////			}
		//			return transferList;
		//		}
		
		Dictionary<string, int> GetOutputColumns(DbDataReader reader)
		{
			var columnNamesToIndexMappings = new Dictionary<string, int>();
			for (int i = 0; i < reader.FieldCount; i++) {
				columnNamesToIndexMappings[reader.GetName(i).ToUpper()] = i;
			}
			foreach (var key in columnNamesToIndexMappings.Keys) {
				var value = columnNamesToIndexMappings[key];
				if (log.IsDebugEnabled) {
					log.DebugFormat("Column {0} is at index {1}", key, value);
				}
			}
			return columnNamesToIndexMappings;
		}

		DataDictionary MakeInputData(DataWorkerTask dataWorkerTask, object input)
		{
			var inputData = new DataDictionary();
			var nameToPropInfoMapping = new Dictionary<string, PropertyInfo>();
			if (input != null) {
				switch (dataWorkerTask.Mode) {
					case DataWorkerMode.DataFields:
						foreach (var prop in input.GetType().GetProperties()) {
							if (prop.CanRead) {
								var attrs = AttributeHelper.GetPropertyAttributes<DataFieldAttribute>(prop);
								foreach (var attr in attrs) {
									var fieldName = (attr.FieldName ?? prop.Name).ToUpper();
									{
										inputData[fieldName] = prop.GetValue(input, null);
									}
									{
										nameToPropInfoMapping[fieldName] = prop;
									}
								}
							}
						}

						break;
					case DataWorkerMode.AllProperties:
						foreach (var prop in input.GetType().GetProperties()) {
							if (prop.CanRead) {
								var fieldName = prop.Name.ToUpper();
								{
									inputData[fieldName] = prop.GetValue(input, null);
								}
								{
									nameToPropInfoMapping[fieldName] = prop;
								}
							}
						}

						break;
					case DataWorkerMode.Dictionary:
						var inputDict = (DataDictionary)input;
						foreach (var pair in inputDict) {
							inputData[pair.Key.ToUpper()] = pair.Value;
							;
						}

						break;
					default:
						throw new ArgumentException("dataWorkerTask.Mode is invalid", "dataWorkerTask.Mode");
				}
			}
			return inputData;
		}

		void BindInputDataToParameters(DataDictionary inputData, DataWorkerTask dataWorkerTask)
		{
			foreach (var dictPair in dataWorkerTask.ParamsByName) {
				object val;
				var name = dictPair.Key.ToUpper();
				if (inputData.TryGetValue(name, out val)) {
					if (val == null) {
						val = DBNull.Value;
					}
					if (log.IsDebugEnabled) {
						log.DebugFormat("Setting {0} to {1}", name, val);
					}
					dictPair.Value.Value = val;
				} else {
					if (log.IsDebugEnabled) {
						log.DebugFormat("Not Setting value for {0}", name);
					}
				}
			}
		}
		
		//		private DataDictionary LoadInputValues(DataWorkerTask dataWorkerTask)
		//		{
		//			var result = new DataDictionary();
		//			// ???
		//			return result;
		//		}
		
		private void BuildParameters(DbCommand cmd, DataWorkerTask dataWorkerTask, object input)
		{
			// todo: cache?
			var parms = new Dictionary<string, DbParameter>();
			switch (dataWorkerTask.CommandType) {
				case CommandType.StoredProcedure:
					BuildParametersFromDB(cmd);
					break;
				case CommandType.Text:
					BuildParametersFromInputObject(cmd, dataWorkerTask, input);
					break;
			}
			for (int i = 0; i < cmd.Parameters.Count; i++) {
				var parm = cmd.Parameters[i];
				var cleanName = parm.ParameterName;
				if (cleanName.StartsWith("@")) {
					cleanName = cleanName.Remove(0, 1);
				}
				parms[cleanName.ToUpper()] = parm;
			}
			foreach (var key in parms.Keys) {
				if (log.IsDebugEnabled) {
					log.DebugFormat("{0} - {1}", key, parms[key]);
				}
			}
			dataWorkerTask.ParamsByName = parms;
		}

		void BuildParametersFromDB(DbCommand cmd)
		{
			var cb = Factory.CreateCommandBuilder();
			var mi = cb.GetType().GetMethod("DeriveParameters");
			mi.Invoke(cb, new[] { cmd });
			for (int i = 0; i < cmd.Parameters.Count; i++) {
				var parm = cmd.Parameters[i];
				if (log.IsDebugEnabled) {
					log.DebugFormat("Parameter[{0}] {1}", i, parm.ParameterName);
				}
			}
		}
		
		private void BuildParametersFromInputObject(DbCommand cmd, DataWorkerTask dataWorkerTask, object input)
		{
			// xxx todo: should not be populating parameter values here!. should be seperate. CLEANUP!
			if (input != null) {
				switch (dataWorkerTask.Mode) {
					case DataWorkerMode.DataFields:
						var unfilteredProps = input.GetType().GetProperties();
						foreach (var prop in unfilteredProps) {
							if (prop.CanRead) {
								var attr = AttributeHelper.GetFirstPropertyAttribute<DataFieldAttribute>(prop);
								if (attr != null) {
									var param = Factory.CreateParameter();
									param.ParameterName = (attr.FieldName ?? prop.Name).ToUpper();
									param.Value = prop.GetValue(input, null);
									cmd.Parameters.Add(param);
								}
							}
						}
						break;
					case DataWorkerMode.AllProperties:
						var allProps = input.GetType().GetProperties();
						foreach (var prop in allProps) {
							if (prop.CanRead) {
								var param = Factory.CreateParameter();
								param.ParameterName = prop.Name.ToUpper();
								param.Value = prop.GetValue(input, null);
								cmd.Parameters.Add(param);
							}
						}
						break;
					case DataWorkerMode.Dictionary:
						var dict = (DataDictionary)input;
						foreach (var pair in dict) {
							var param = Factory.CreateParameter();
							param.ParameterName = pair.Key.ToUpper();
							param.Value = pair.Value;
							param.Direction = ParameterDirection.InputOutput;
							cmd.Parameters.Add(param);
						}
						break;
					default:
						throw new ArgumentException("Unkown dataWorkerTask.mode in BuildParametersFromInputObject", "dataWorkerTask.Mode");
				}
			}
		}
		
		
		private DbCommand CreateCommand(DbConnection conn, DataWorkerTask dataWorkerTask)
		{
			var result = conn.CreateCommand();
			result.CommandText = dataWorkerTask.CommandText;
			result.CommandType = dataWorkerTask.CommandType;
			result.CommandTimeout = dataWorkerTask.CommandTimeout ?? DefaultCommandTimeout;
			if (dataWorkerTask.Transaction != null) {
				result.Transaction = dataWorkerTask.Transaction.ActualTransaction;
			}
			return result;
		}

		private DbConnection GetConnectionForWorker(DataWorkerTask dataWorkerTask)
		{
			DbConnection result;
			if (dataWorkerTask.Transaction != null) {
				result = dataWorkerTask.Transaction.ActualTransaction.Connection;
			} else {
				result = MakeUsableConnectionFromScratch();
			}
			return result;
		}
		
		private DbConnection MakeUsableConnectionFromScratch()
		{
			DbConnection result = Factory.CreateConnection();
			result.ConnectionString = ConnectionString;
			result.Open();
			return result;
		}
		
		
		//		public object DoScalar(string command)
		//		{
		//			// todo
		//			return null;
		//		}
		//
		//		public object DoScalar(string command, object paramsObject)
		//		{
		//			// todo
		//			return null;
		//		}
		//
		//		public object DoScalar(string command, object paramsObject, Transaction transaction)
		//		{
		//			// todo
		//			return null;
		//		}
		//
		//		public object DoReader(string command)
		//		{
		//			// todo ???
		//			return null;
		//		}

		public int DoNonQuery(string command)
		{
			return DoNonQuery(command, null);
		}
		
		public int DoNonQuery(string command, object paramsObject)
		{
			return DoNonQuery(command, paramsObject, null);
		}

		public int DoNonQuery(string command, object paramsObject, Transaction transaction)
		{
			var dwt = new DataWorkerTask {
				CommandText = command,
				CommandType = CommandType.StoredProcedure,
				Mode = DataWorkerMode.DataFields,
				Transaction = transaction,
			};
			DoWorkDirect<object>(dwt, paramsObject);
			return dwt.RowsAffected;
		}

		public int DoNonQueryObj(string command)
		{
			// todo: test
			return DoNonQueryObj(command, null);
		}

		public int DoNonQueryObj(string command, object paramsObject)
		{
			// todo: test
			return DoNonQueryObj(command, paramsObject, null);
		}

		public int DoNonQueryObj(string command, object paramsObject, Transaction transaction)
		{
			// todo: test
			var dwt = new DataWorkerTask {
				CommandText = command,
				CommandType = CommandType.StoredProcedure,
				Mode = DataWorkerMode.AllProperties,
				Transaction = transaction,
			};
			DoWorkDirect<object>(dwt, paramsObject);
			return dwt.RowsAffected;
		}

		// This Signature allows for inferring the Type from the paramsObject to be inserted. The explicit version is invoked
		public bool Insert<T>(T paramsObject)
		{
//			var commandText = AttributeHelper.GetFirstPropertyAttribute<DataClassAttribute>(typeof(T)).InsertProcedure;
//			var dwt = new DataWorkerTask {
//				CommandText = commandText,
//				CommandType = CommandType.StoredProcedure,
//				Mode = DataWorkerMode.DataFields
//			};
//			DoWorkDirect<T>(dwt, paramsObject);
//			return dwt.RowsAffected != Constants.KnownBadStateForRowsAffected;
			return Insert<T>((object)paramsObject);
		}

		public bool Insert<T>(object paramsObject)
		{
			return Insert<T>(paramsObject, null);
		}
		
		public bool Insert<T>(object paramsObject, Transaction transaction)
		{
			var commandText = AttributeHelper.GetFirstPropertyAttribute<DataClassAttribute>(typeof(T)).InsertProcedure;
			var dwt = new DataWorkerTask {
				CommandText = commandText,
				CommandType = CommandType.StoredProcedure,
				Mode = DataWorkerMode.DataFields,
				Transaction = transaction,
			};
			DoWorkDirect<T>(dwt, paramsObject);
			return dwt.RowsAffected != Constants.KnownBadStateForRowsAffected;
		}

		// This Signature allows for inferring the Type from the paramsObject to be deleted. The explicit version is invoked
		public bool Delete<T>(T paramsObject)
		{
			return Delete<T>((object)paramsObject);
		}
		
		public bool Delete<T>(object paramsObject)
		{
			var commandText = AttributeHelper.GetFirstPropertyAttribute<DataClassAttribute>(typeof(T)).DeleteProcedure;
			var dwt = new DataWorkerTask {
				CommandText = commandText,
				CommandType = CommandType.StoredProcedure,
				Mode = DataWorkerMode.DataFields
			};
			DoWorkDirect<T>(dwt, paramsObject);
			return dwt.RowsAffected != Constants.KnownBadStateForRowsAffected;
		}
		
		// This Signature allows for inferring the Type from the paramsObject to be fetched. The explicit version is invoked
		public T Fetch<T>(T paramsObject) where T : new()
		{
			return Fetch<T>((object)paramsObject);
		}

		public T Fetch<T>(object paramsObject) where T : new()
		{
			var commandText = AttributeHelper.GetFirstPropertyAttribute<DataClassAttribute>(typeof(T)).FetchProcedure;
			return Fetch<T>(commandText , paramsObject);
		}
		
		public T Fetch<T>(string commandText) where T : new()
		{
			return Fetch<T>(commandText , null);
		}

		public T Fetch<T>(string commandText, object paramsObject) where T : new()
		{
			//var commandText = AttributeHelper.GetFirstPropertyAttribute<DataClassAttribute>(typeof(T)).FetchProcedure;
			var dwt = new DataWorkerTask {
				CommandText = commandText,
				CommandType = CommandType.StoredProcedure,
				Mode = DataWorkerMode.DataFields,
				ReadResults = true
			};
			var results = DoWorkDirect<T>(dwt, paramsObject);
			if ((results != null) && (results.Count > 0)) {
				return results[0];
			}
			return default(T);
		}

//		public T FetchCmd<T>(string commandText)
//		{
//			return FetchCmd<T>(commandText, null);
//		}
//		
//		public T FetchCmd<T>(string commandText, object paramsObject)
//		{
//			var dwt = new DataWorkerTask {
//				CommandText = commandText,
//				CommandType = CommandType.StoredProcedure,
//				Mode = DataWorkerMode.DataFields,
//				ReadResults = true
//			};
//			var results = DoWorkDirect<T>(dwt, paramsObject);
//			if ((results != null) && (results.Count > 0)) {
//				return results[0];
//			}
//			return default(T);
//		}
		
		public T FetchByAllProps<T>(object paramsObject)
		{
			// todo: add unit test
			// todo: fix !!! this is broken
			var commandText = AttributeHelper.GetFirstPropertyAttribute<DataClassAttribute>(typeof(T)).FetchProcedure;
			var dwt = new DataWorkerTask {
				CommandText = commandText,
				CommandType = CommandType.StoredProcedure,
				Mode = DataWorkerMode.AllProperties,
				ReadResults = true
			};
			var results = DoWorkDirect<T>(dwt, paramsObject);
			if ((results != null) && (results.Count > 0)) {
				return results[0];
			}
			return default(T);
		}
		
		// This Signature allows for inferring the Type from the paramsObject to be updated. The explicit version is invoked
		public bool Update<T>(T paramsObject)
		{
			return Update<T>((object)paramsObject);
		}
		
		public bool Update<T>(object paramsObject)
		{
			var commandText = AttributeHelper.GetFirstPropertyAttribute<DataClassAttribute>(typeof(T)).UpdateProcedure;
			var dwt = new DataWorkerTask {
				CommandText = commandText,
				CommandType = CommandType.StoredProcedure,
				Mode = DataWorkerMode.DataFields
			};
			DoWorkDirect<T>(dwt, paramsObject);
			return dwt.RowsAffected != Constants.KnownBadStateForRowsAffected;
		}

		public List<T> List<T>()
		{
			return List<T>(null);
		}
		
		// todo: should there be an inferred List<T>???
		
		public List<T> List<T>(object paramsObject)
		{
			var commandText = AttributeHelper.GetFirstPropertyAttribute<DataClassAttribute>(typeof(T)).ListProcedure;
			var dwt = new DataWorkerTask {
				CommandText = commandText,
				CommandType = CommandType.StoredProcedure,
				Mode = DataWorkerMode.DataFields,
				ReadResults = true,
			};
			var result = DoWorkDirect<T>(dwt, paramsObject);
			return result;
		}

		public List<T> ListCmd<T>(string commandText)
		{
			return ListCmd<T>(commandText, null);
		}
			
		public List<T> ListCmd<T>(string commandText, object paramsObject)
		{
			var dwt = new DataWorkerTask {
				CommandText = commandText,
				CommandType = CommandType.StoredProcedure,
				Mode = DataWorkerMode.DataFields,
				ReadResults = true,
			};
			var result = DoWorkDirect<T>(dwt, paramsObject);
			return result;
		}
		
		// TODO: implement transaction on insert, update, delete, et all
		// TODO: implement unit tests that cover transactions
		// [ ]  do count, do insert , check count is increased by 1, roll back check count back to first count
		// [ ]  do count, do delete , check count is decreased by 1, roll back check count back to first count
		// [ ]  do fetch, do update , do new fetch, check fetched value changed, roll back ,
		//		fetch again , check fetched again value is back to original

		// TODO:
		// write some documentatio of how this stuff is supposed to work and under which circumstances
		//  like the ListCmd thing is not clear, even to me right now
		//  maybe rework all the signatures to make sense
		//  decide on what matters more, being able to infer the type, or being able to inject a custom command text?
		//  decide on query<T> call signature, make unit tests for the full spectrum
		//  decide on in-out flows of paramaters to input object 
		//  decide if the output params should spill into the result set (leaning towards no right now, but it's 2:21 am, so meh)
		//  pretty sure documentation should include assumptions that make insert and delete a success
		//  rows affected can come out of execute calls (dononquery)
		
	}
}
/*
 * Created by SharpDevelop.
 * User: thief
 * Date: 2010/06/06
 * Time: 11:02 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using FeihtWorx.Util;
using FeihtWorx.Data;
using System.Data.SqlClient;
using log4net;

namespace Scratch
{
	class Program
	{
		private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
		
		private const string SqlProvider = @"System.Data.SqlClient";
		private const string SqlConnectionString = @"Data Source=.\sqlexpress;Integrated Security=SSPI;Initial Catalog=DataWorkerTest;";
		
		public static void Main(string[] args)
		{
			
			if (log.IsDebugEnabled) {
				log.Debug("Scratch Started");
			}
			var dw = new DataWorker(SqlProvider, SqlConnectionString);
			
			PerformSqlTest2();
			PerformProperyAttributesTest();
			PerformSqlTest();
			
			Console.Write("Press any key to continue . . . ");
			Console.ReadKey(true);
		}

		static void PerformProperyAttributesTest()
		{
			var props = typeof(SampleClassChild).GetProperties();
			foreach (var prop in props) {
				var list = new List<SampleAttribute>();
				Console.WriteLine("{0}", prop.Name);
				var attrs = prop.GetCustomAttributes(typeof(SampleAttribute), true);
				foreach (var attr in attrs) {
					var t = attr as SampleAttribute;
					if (t != null) {
						Console.WriteLine("+++");
					}
				}
				var test = AttributeHelper.GetPropertyAttributes<SampleAttribute>(prop);
				if (test.Length > 0) {
					Console.WriteLine("^^^");
				}
				var test2 = AttributeHelper.GetFirstPropertyAttribute<SampleAttribute>(prop);
				if (test2 != null) {
					Console.WriteLine("***");
				}
			}
		}

		static void PerformSqlTest2()
		{
			var factory = DbProviderFactories.GetFactory("System.Data.SqlClient");
			var connstr = @"Data Source=.\sqlexpress;Integrated Security=SSPI;Initial Catalog=DataWorkerTest;";
			using (var conn = factory.CreateConnection()) {
				conn.ConnectionString = connstr;
				conn.Open();
				using (var cmd = conn.CreateCommand()) {
					cmd.CommandText = "select id,name from samples where id <= @x";
					cmd.CommandType = CommandType.Text;
					var p = factory.CreateParameter();
					p.ParameterName = "x";
					p.Value = 3;
					cmd.Parameters.Add(p);
					for (int i = 0; i < cmd.Parameters.Count; i++) {
						var parm = cmd.Parameters[i];
						Console.WriteLine(parm.ParameterName);
					}
					using (var reader = cmd.ExecuteReader()) {
						while (reader.Read()) {
							int id = reader.GetInt32(0);
							string name = reader.GetString(1);
							Console.WriteLine("{0} - {1}", id, name);
						}
					}
				}
				
			}
		}
		
		static void PerformSqlTest()
		{
			using (var conn = new SqlConnection(@"Data Source=.\sqlexpress;Integrated Security=SSPI;Initial Catalog=DataWorkerTest;")) {
				conn.Open();
				using (var cmd = conn.CreateCommand()) {
					//cmd.CommandText = "ListSamples";
					cmd.CommandText = "testaffected";
					cmd.CommandType = CommandType.StoredProcedure;
					SqlCommandBuilder.DeriveParameters(cmd);
					for (int i = 0; i < cmd.Parameters.Count; i++) {
						var parm = cmd.Parameters[i];
						Console.WriteLine(parm.ParameterName);
					}
				}
			}
		}
		
	}
}
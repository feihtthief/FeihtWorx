/*
 * Created by SharpDevelop.
 * User: thief
 * Date: 2010/06/11
 * Time: 10:22 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.IO;
using System.Data;
using System.Data.SqlClient;

using log4net;
using NUnit.Framework;

using FeihtWorx.Data;
using Tests.FeihtWorx.SharedThings;

/*
 * These test make a few assumptions:
 * SQL DB Exist as specified in Constants.SqlConnectionString
 * 
 * */


namespace Tests.FeihtWorx
{
	[TestFixture]
	public class TestDataWorker
	{
		private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
		
		[TestFixtureSetUp]
		public void TestSetup()
		{
			//Console.WriteLine("#################################");
			log.Debug("Test Run Started");
		}
		
		public DataWorker GetDataWorker()
		{
			var dw = new DataWorker("System.Data.SqlClient", Constants.SqlConnectionString);
			return dw;
		}
		
		[Test]
		public void TestListCount()
		{
			var dw = GetDataWorker();
			var dwt = new DataWorkerTask();
			dwt.CommandText = "listsamples";
			dwt.ReadResults = true;
			var result = dw.DoWorkDirect<SampleClass>(dwt);
			Assert.IsNotNull(result);
			Assert.AreNotEqual(0, result.Count);
		}
		
		[Test]
		public void TestAffectedNoRead()
		{
			var dw = GetDataWorker();
			var dwt = new DataWorkerTask();
			dwt.CommandText = "testaffected";
			dwt.ReadResults = false;
			var result = dw.DoWorkDirect<SampleClass>(dwt);
			Assert.AreEqual(1, dwt.RowsAffected, "Expected to affect 1 row");
			Assert.IsNotNull(result);
			Assert.AreEqual(0, result.Count);
		}
		
		[Test]
		public void TestAffectedReadDict()
		{
			var dw = GetDataWorker();
			var dwt = new DataWorkerTask();
			dwt.CommandText = "testaffected";
			dwt.ReadResults = true;
			dwt.Mode = DataWorkerMode.Dictionary;
			var dd = new DataDictionary();
			dd["howmany"] = 2;
			var result = dw.DoWorkDirect<SampleClass>(dwt, dd);
			Assert.AreEqual(2, dwt.RowsAffected, "Expected to affect 2 rows");
			Assert.IsNotNull(result);
			Assert.AreEqual(0, result.Count); // nothing returned by procedure, even though we tried to read
		}
		
		[Test]
		public void TestAffectedReadDataFields()
		{
			var dw = GetDataWorker();
			var dwt = new DataWorkerTask();
			dwt.CommandText = "testaffected";
			dwt.ReadResults = true;
			dwt.Mode = DataWorkerMode.DataFields;
			var commandClass = new CommandClass{ HowMany = 2 };
			var result = dw.DoWorkDirect<SampleClass>(dwt, commandClass);
			Assert.AreEqual(2, dwt.RowsAffected, "Expected to affect 2 rows");
			Assert.IsNotNull(result);
			Assert.AreEqual(0, result.Count); // nothing returned by procedure, even though we tried to read
		}
		
		[Test]
		public void TestAffectedReadAllProps()
		{
			var dw = GetDataWorker();
			var dwt = new DataWorkerTask();
			dwt.CommandText = "testaffected";
			dwt.ReadResults = true;
			dwt.Mode = DataWorkerMode.AllProperties;
			var obj = new {howmany = 2};
			var result = dw.DoWorkDirect<SampleClass>(dwt, obj);
			Assert.AreEqual(2, dwt.RowsAffected, "Expected to affect 2 rows");
			Assert.IsNotNull(result);
			Assert.AreEqual(0, result.Count); // nothing returned by procedure, even though we tried to read
		}
		
		[Test]
		public void TestOptionalParamsUnpopulated()
		{
			var dw = GetDataWorker();
			var dwt = new DataWorkerTask();
			dwt.CommandText = "TestMissingAndNullInput";
			dwt.ReadResults = true;
			dwt.Mode = DataWorkerMode.AllProperties;
			var obj = new { // using anon to force non-existance of optional params
				RequiredParam = 42, // required
				//OptionalParam1 = , // defaults to null in db
				//OptionalParam2 = , // defaults to 41   in db
			};
			var result = dw.DoWorkDirect<SampleClass>(dwt, obj);
			Assert.IsNotNull(result);
			Assert.AreEqual(1, result.Count);
			Assert.AreEqual(obj.RequiredParam, result[0].RequiredParamOut);
			Assert.AreEqual(null, result[0].OptionalParam1Out);
			Assert.AreEqual(41, result[0].OptionalParam2Out);
		}

		[Test]
		public void TestOptionalParamsPopulated()
		{
			var dw = GetDataWorker();
			var dwt = new DataWorkerTask();
			dwt.CommandText = "TestMissingAndNullInput";
			dwt.ReadResults = true;
			dwt.Mode = DataWorkerMode.AllProperties;
			var obj = new SampleClass {
				RequiredParam = 42, // required
				OptionalParam1 = 43,
				OptionalParam2 = null,
			};
			var result = dw.DoWorkDirect<SampleClass>(dwt, obj);
			Assert.IsNotNull(result);
			Assert.AreEqual(1, result.Count);
			Assert.AreEqual(obj.RequiredParam, result[0].RequiredParamOut);
			Assert.AreEqual(obj.OptionalParam1, result[0].OptionalParam1Out);
			Assert.AreEqual(obj.OptionalParam2, result[0].OptionalParam2Out);
		}

		[Test]
		public void TestSelect()
		{
			var dw = GetDataWorker();
			var dwt = new DataWorkerTask();
			dwt.CommandText = "select * from samples";
			dwt.CommandType = CommandType.Text;
			dwt.ReadResults = true;
			var result = dw.DoWorkDirect<SampleClass>(dwt);
			Assert.AreNotEqual(0, result.Count);
			foreach (var item in result) {
				Assert.IsTrue(item.ID != 0);
				Assert.IsNotNullOrEmpty(item.Name);
			}
		}
		
		[Test]
		public void TestSelectColumnNameRemapping()
		{
			var dw = GetDataWorker();
			var dwt = new DataWorkerTask();
			dwt.CommandText = "ListNotSimple";
			dwt.CommandType = CommandType.StoredProcedure;
			dwt.ReadResults = true;
			dwt.Mode = DataWorkerMode.DataFields;
			var msg = "any message";
			var dataobject = new SampleClass{ SimpleColumnIn = msg };
			var result = dw.DoWorkDirect<SampleClass>(dwt, dataobject);
			Assert.AreNotEqual(0, result.Count);
			foreach (var item in result) {
				Assert.IsTrue(item.ID != 0);
				Assert.IsNotNullOrEmpty(item.Name);
				Assert.AreEqual(msg, item.SimpleColumnOut);
			}
		}

		[Test]
		public void TestOutputToReadOnlyProperty()
		{
			var dw = GetDataWorker();
			var dwt = new DataWorkerTask();
			dwt.CommandText = "select *,'bla' as ReadOnlyProperty from samples";
			dwt.CommandType = CommandType.Text;
			dwt.ReadResults = true;
			var result = dw.DoWorkDirect<SampleClass>(dwt);
			Assert.AreNotEqual(0, result.Count);
			foreach (var item in result) {
				Assert.IsTrue(item.ID != 0);
				Assert.IsNotNullOrEmpty(item.Name);
			}
		}

		[Test]
		public void TestInputFromWriteOnlyProperty()
		{
			var dw = GetDataWorker();
			var dwt = new DataWorkerTask();
			dwt.CommandText = "WriteOnlyInputTest";
			dwt.CommandType = CommandType.StoredProcedure;
			dwt.ReadResults = true;
			dwt.Mode = DataWorkerMode.DataFields;
			var msg = "Read This";
			var dataObject = new SampleClass{ WriteOnlyProperty = msg };
			var result = dw.DoWorkDirect<SampleClass>(dwt, dataObject);
			Assert.AreNotEqual(0, result.Count);
			foreach (var item in result) {
				Assert.IsTrue(item.ID != 0);
				Assert.IsNotNullOrEmpty(item.Name);
				Assert.AreNotEqual(msg, item.WriteOnlyPropertyOutput);
				Assert.IsNotNull(item.WriteOnlyPropertyOutput);
				Assert.AreEqual("Default Value Set In Stored Proc", item.WriteOnlyPropertyOutput);
			}
		}
		
		[Test]
		public void TestInputFromNull()
		{
			var dw = GetDataWorker();
			var dwt = new DataWorkerTask();
			dwt.CommandText = "ListGarbage";
			dwt.CommandType = CommandType.StoredProcedure;
			dwt.ReadResults = true;
			dwt.Mode = DataWorkerMode.DataFields;
			var result = dw.DoWorkDirect<SampleClass>(dwt, null);
			Assert.AreNotEqual(0, result.Count);
			foreach (var item in result) {
				Assert.IsTrue(item.ID != 0);
				Assert.IsNotNullOrEmpty(item.Name);
				Assert.IsNotNull(item.Feedout);
				Assert.AreEqual("Default Value Set In Stored Proc", item.Feedout);
			}
		}
		
		
		[Test]
		public void TestSelectFilteredFromDictionary()
		{
			var dw = GetDataWorker();
			var dwt = new DataWorkerTask();
			dwt.CommandText = "select * from samples where id <= @x";
			dwt.CommandType = CommandType.Text;
			dwt.ReadResults = true;
			dwt.Mode = DataWorkerMode.Dictionary;
			var dd = new DataDictionary();
			dd["x"] = 2;
			var result = dw.DoWorkDirect<SampleClass>(dwt, dd);
			Assert.AreNotEqual(0, result.Count);
			foreach (var item in result) {
				Assert.IsTrue(item.ID <= 2);
			}
		}

		[Test]
		public void TestSelectFilteredConfirm()
		{
			var dw = GetDataWorker();
			var dwt = new DataWorkerTask();
			dwt.CommandText = "select * from samples where id > @x";
			dwt.CommandType = CommandType.Text;
			dwt.ReadResults = true;
			dwt.Mode = DataWorkerMode.Dictionary;
			var dd = new DataDictionary();
			dd["x"] = 2;
			var result = dw.DoWorkDirect<SampleClass>(dwt, dd);
			Assert.AreNotEqual(0, result.Count);
			foreach (var item in result) {
				Assert.IsTrue(item.ID > 2);
			}
		}
		
		[Test]
		public void TestSelectFilteredFromDataObject()
		{
			var dw = GetDataWorker();
			var dwt = new DataWorkerTask();
			dwt.CommandText = "select * from samples where id <= @ID";
			dwt.CommandType = CommandType.Text;
			dwt.ReadResults = true;
			dwt.Mode = DataWorkerMode.DataFields;
			var dataObject = new SampleClass{ ID = 2 };
			var result = dw.DoWorkDirect<SampleClass>(dwt, dataObject);
			Assert.AreNotEqual(0, result.Count);
			foreach (var item in result) {
				Assert.IsTrue(item.ID <= 2);
			}
		}
		
		[Test]
		public void TestSelectFilteredFromNormalObject()
		{
			var dw = GetDataWorker();
			var dwt = new DataWorkerTask();
			dwt.CommandText = "select * from samples where id <= @ID";
			dwt.CommandType = CommandType.Text;
			dwt.ReadResults = true;
			dwt.Mode = DataWorkerMode.AllProperties;
			var normalObject = new SampleClassPoco{ ID = 2 };
			var result = dw.DoWorkDirect<SampleClass>(dwt, normalObject);
			Assert.AreNotEqual(0, result.Count);
			foreach (var item in result) {
				Assert.IsTrue(item.ID <= 2);
			}
		}
		
		[Test]
		public void TestTransactionRollback()
		{
			InitTestObjects();
			var before = GetCountOfTestObjects();
			Assert.AreNotEqual(0, before);
			var dw = GetDataWorker();
			var tran = dw.BeginTransaction();
			var dwt = new DataWorkerTask();
			dwt.CommandText = "delete from TestObjects";
			dwt.CommandType = CommandType.Text;
			dwt.Transaction = tran;
			dwt.ReadResults = false;
			var result = dw.DoWorkDirect<SampleClass>(dwt);
			var during = GetCountOfTestObjects();
			tran.Rollback();
			Assert.AreNotEqual(0, dwt.RowsAffected);
			var after = GetCountOfTestObjects();
			Assert.AreNotEqual(0, after);
			Assert.AreNotEqual(before, during);
			Assert.AreNotEqual(after, during);
			Assert.AreEqual(0, during);
			Assert.AreEqual(before, after);
		}

		[Test]
		public void TestTransactionCommit()
		{
			InitTestObjects();
			var before = GetCountOfTestObjects();
			Assert.AreNotEqual(0, before);
			var dw = GetDataWorker();
			var tran = dw.BeginTransaction();
			var dwt = new DataWorkerTask();
			dwt.CommandText = "delete from TestObjects";
			dwt.CommandType = CommandType.Text;
			dwt.Transaction = tran;
			dwt.ReadResults = false;
			var result = dw.DoWorkDirect<SampleClass>(dwt);
			tran.Commit();
			Assert.AreNotEqual(0, dwt.RowsAffected);
			var after = GetCountOfTestObjects();
			Assert.AreEqual(0, after);
			Assert.AreNotEqual(before, after);
		}
		
		[Test]
		public void TestOutputParameterDataObject()
		{
			InitTestObjects();
			var before = GetCountOfTestObjects();
			Assert.AreNotEqual(0, before);
			var dw = GetDataWorker();
			var dwt = new DataWorkerTask();
			dwt.CommandText = "InsertTestObject";
			dwt.CommandType = CommandType.StoredProcedure;
			dwt.ReadResults = true;
			dwt.Mode = DataWorkerMode.DataFields;
			var testObject = new SampleClass{ ID = 0 };
			var result = dw.DoWorkDirect<SampleClass>(dwt, testObject);
			Assert.AreEqual(0, result.Count);
			Assert.AreEqual(1, dwt.RowsAffected);
			Assert.AreNotEqual(0, testObject.ID);
			var after = GetCountOfTestObjects();
			Assert.AreNotEqual(0, after);
			Assert.AreEqual(before + 1, after);
		}
		
		[Test]
		public void TestOutputParameterNormalObject()
		{
			InitTestObjects();
			var before = GetCountOfTestObjects();
			Assert.AreNotEqual(0, before);
			var dw = GetDataWorker();
			var dwt = new DataWorkerTask();
			dwt.CommandText = "InsertTestObject";
			dwt.CommandType = CommandType.StoredProcedure;
			dwt.ReadResults = true;
			dwt.Mode = DataWorkerMode.AllProperties;
			var testObject = new SampleClassPoco{ ID = 0 };
			var result = dw.DoWorkDirect<SampleClass>(dwt, testObject);
			Assert.AreEqual(0, result.Count);
			Assert.AreEqual(1, dwt.RowsAffected);
			Assert.AreNotEqual(0, testObject.ID);
			var after = GetCountOfTestObjects();
			Assert.AreNotEqual(0, after);
			Assert.AreEqual(before + 1, after);
		}

		[Test]
		public void TestInt64PrimaryKey()
		{
			InitTestObjects();
			var before = GetCountOfTestObjects();
			Assert.AreNotEqual(0, before);
			var dw = GetDataWorker();
			var dwt = new DataWorkerTask();
			dwt.CommandText = "InsertTestObjectWithReallyBigID";
			dwt.CommandType = CommandType.StoredProcedure;
			dwt.ReadResults = true;
			dwt.Mode = DataWorkerMode.AllProperties;
			var testObject = new SampleClassPoco{ ID = 0 };
			var result = dw.DoWorkDirect<SampleClass>(dwt, testObject);
			Assert.AreEqual(0, result.Count);
			Assert.AreEqual(1, dwt.RowsAffected);
			Assert.AreNotEqual(0, testObject.ID);
			Assert.AreEqual(long.MaxValue, testObject.ID);
			var after = GetCountOfTestObjects();
			Assert.AreNotEqual(0, after);
			Assert.AreEqual(before + 1, after);
		}

		[Test]
		public void TestOutputParameterDataDictionary()
		{
			InitTestObjects();
			var before = GetCountOfTestObjects();
			Assert.AreNotEqual(0, before);
			var dw = GetDataWorker();
			var dwt = new DataWorkerTask();
			dwt.CommandText = "InsertTestObject";
			dwt.CommandType = CommandType.StoredProcedure;
			dwt.ReadResults = true;
			dwt.Mode = DataWorkerMode.Dictionary;
			var testDict = new DataDictionary();
			testDict["ID"] = 0;
			var result = dw.DoWorkDirect<SampleClass>(dwt, testDict);
			Assert.AreEqual(0, result.Count);
			Assert.AreEqual(1, dwt.RowsAffected);
			Assert.AreNotEqual(0, testDict["ID"]);
			var after = GetCountOfTestObjects();
			Assert.AreNotEqual(0, after);
			Assert.AreEqual(before + 1, after);
		}
		
		[Test]
		public void TestOutputParameterAnonymousObject()
		{
			InitTestObjects();
			var before = GetCountOfTestObjects();
			Assert.AreNotEqual(0, before);
			var dw = GetDataWorker();
			var dwt = new DataWorkerTask();
			dwt.CommandText = "InsertTestObject";
			dwt.CommandType = CommandType.StoredProcedure;
			dwt.ReadResults = true;
			dwt.Mode = DataWorkerMode.AllProperties;
			var testObject = new {ID = 0};
			var result = dw.DoWorkDirect<SampleClass>(dwt, testObject);
			Assert.AreEqual(0, result.Count);
			Assert.AreEqual(1, dwt.RowsAffected);
			Assert.AreEqual(0, testObject.ID); // -- Anonymous Objects are immutable
			var after = GetCountOfTestObjects();
			Assert.AreNotEqual(0, after);
			Assert.AreEqual(before + 1, after);
		}
		
		[Test]
		public void TestOutputParameterToResultsOff()
		{
			InitTestObjects();
			var before = GetCountOfTestObjects();
			Assert.AreNotEqual(0, before);
			var dw = GetDataWorker();
			var dwt = new DataWorkerTask();
			dwt.CommandText = "ListSamples";
			dwt.CommandType = CommandType.StoredProcedure;
			dwt.ReadResults = true;
			dwt.Mode = DataWorkerMode.Dictionary;
			dwt.ApplyOutputParametersToResults = false;  // <== Explicit default to differentiate with TestOutputParameterToResultsOn()
			var testDict = new DataDictionary();
			testDict["Feedout"] = "moo";
			var result = dw.DoWorkDirect<SampleClass>(dwt, testDict);
			Assert.AreNotEqual(0, result.Count);
			Assert.IsTrue(testDict.ContainsKey("FEEDOUT"));
			// === Expect Change in Input dictionary, BUT NOT in results
			Assert.AreEqual("moo_ex", testDict["FEEDOUT"]);
			foreach (var item in result) {
				Assert.IsNull(item.Feedout);
			}
			Assert.IsFalse(testDict.ContainsKey("ID"));
			var after = GetCountOfTestObjects();
			Assert.AreNotEqual(0, after);
			Assert.AreEqual(before, after);
		}

		[Test]
		public void TestOutputParameterToResultsOn()
		{
			InitTestObjects();
			var before = GetCountOfTestObjects();
			Assert.AreNotEqual(0, before);
			var dw = GetDataWorker();
			var dwt = new DataWorkerTask();
			dwt.CommandText = "ListSamples";
			dwt.CommandType = CommandType.StoredProcedure;
			dwt.ReadResults = true;
			dwt.Mode = DataWorkerMode.Dictionary;
			dwt.ApplyOutputParametersToResults = true;  // <== Explicit inversion of default to differentiate with TestOutputParameterToResultsOff()
			var testDict = new DataDictionary();
			testDict["Feedout"] = "moo";
			var result = dw.DoWorkDirect<SampleClass>(dwt, testDict);
			Assert.AreNotEqual(0, result.Count);
			Assert.IsTrue(testDict.ContainsKey("FEEDOUT"));
			// === Expect Change in Input dictionary, AND in results
			Assert.AreEqual("moo_ex", testDict["FEEDOUT"]);
			foreach (var item in result) {
				Assert.AreEqual("moo_ex", item.Feedout);
			}
			Assert.AreEqual(-1, dwt.RowsAffected);
			var after = GetCountOfTestObjects();
			Assert.AreNotEqual(0, after);
			Assert.AreEqual(before, after);
		}
		
		[Test]
		public void TestDataDictionaryInput()
		{
			InitTestObjects();
			var before = GetCountOfTestObjects();
			Assert.AreNotEqual(0, before);
			var dw = GetDataWorker();
			var dwt = new DataWorkerTask();
			dwt.CommandText = "ListSamples";
			dwt.CommandType = CommandType.StoredProcedure;
			dwt.ReadResults = true;
			dwt.Mode = DataWorkerMode.Dictionary;
			var testDict = new DataDictionary();
			testDict["FeedToTestColumn"] = "ping";
			var result = dw.DoWorkDirect<SampleClass>(dwt, testDict);
			Assert.AreNotEqual(0, result.Count);
			foreach (var item in result) {
				Assert.AreEqual("ping", item.TestColumn);
			}
			var after = GetCountOfTestObjects();
			Assert.AreNotEqual(0, after);
			Assert.AreEqual(before, after);
		}

		[Test]
		public void TestDataDictionaryOutput()
		{
			// todo: xxx imlpement
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void TestNullDbTransactionArgumentToTransaction()
		{
			var tran = new Transaction(null);
			// expected exception is thrown
		}
		
		[Test]
		public void TestObjectAsTemplate()
		{
			InitTestObjects();
			var before = GetCountOfTestObjects();
			Assert.AreNotEqual(0, before);
			var dw = GetDataWorker();
			var dwt = new DataWorkerTask();
			dwt.CommandText = "ListSamples";
			dwt.CommandType = CommandType.StoredProcedure;
			dwt.ReadResults = true;
			dwt.Mode = DataWorkerMode.AllProperties;
			var result = dw.DoWorkDirect<object>(dwt);
			Assert.AreNotEqual(0, result.Count);
			foreach (var item in result) {
				Assert.IsNotNull(item);
			}
			var after = GetCountOfTestObjects();
			Assert.AreNotEqual(0, after);
			Assert.AreEqual(before, after);
		}
		
		// further tests: xxx todo
		// * With Transaction
		// * With Transaction & Rollback
		
		public int GetCountOfTestObjects()
		{
			using (var conn = new SqlConnection(Constants.SqlConnectionString)) {
				conn.Open();
				using (var cmd = conn.CreateCommand()) {
					cmd.CommandText = "Select count(*) from TestObjects (nolock)";
					cmd.CommandType = CommandType.Text;
					return (int)cmd.ExecuteScalar();
				}
			}
		}

		public int GetCountOfSamples()
		{
			using (var conn = new SqlConnection(Constants.SqlConnectionString)) {
				conn.Open();
				using (var cmd = conn.CreateCommand()) {
					cmd.CommandText = "Select count(*) from Samples (nolock)";
					cmd.CommandType = CommandType.Text;
					return (int)cmd.ExecuteScalar();
				}
			}
		}

		public string GetNameOfSample(int id)
		{
			using (var conn = new SqlConnection(Constants.SqlConnectionString)) {
				conn.Open();
				using (var cmd = conn.CreateCommand()) {
					cmd.CommandText = "Select name from Samples (nolock) where id=" + id;
					cmd.CommandType = CommandType.Text;
					return (string)cmd.ExecuteScalar();
				}
			}
		}

		public void InitTestObjects()
		{
			// exec InitTestObjects
			using (var conn = new SqlConnection(Constants.SqlConnectionString)) {
				conn.Open();
				using (var cmd = conn.CreateCommand()) {
					cmd.CommandText = "InitTestObjects";
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.ExecuteNonQuery();
				}
			}
		}

		public void InitSamples()
		{
			// exec InitSamples
			using (var conn = new SqlConnection(Constants.SqlConnectionString)) {
				conn.Open();
				using (var cmd = conn.CreateCommand()) {
					cmd.CommandText = "InitSamples";
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.ExecuteNonQuery();
				}
			}
		}

		[Test]
		public void Test_DW_NonQuery()
		{
			//InitSamples();
			var dw = GetDataWorker();
			var testObject = new RoundTripClass {
				ValueA = 1,
				ValueB = 2,
				ValueC = 99,
			};
			var result = dw.DoNonQuery("RoundTripAdd", testObject);
			Assert.AreEqual(3, testObject.ValueC, "Expected Value C to be Value A plus Value B");
			Assert.AreEqual(-1, result, "Expected no rows to be affected");
		}
		
		[Test]
		public void Test_DW_NonQuery_AffectZeroRows()
		{
			InitSamples();
			var dw = GetDataWorker();
			var result = dw.DoNonQuery("AffectNoRows");
			Assert.AreEqual(0, result, "Expected 0 rows to be affected");
		}
		
		[Test]
		public void Test_DW_NonQuery_AffectOneRow()
		{
			InitSamples();
			var dw = GetDataWorker();
			var result = dw.DoNonQuery("AffectOneRow");
			Assert.AreEqual(1, result, "Expected 1 row to be affected");
		}

		[Test]
		public void Test_DW_Insert()
		{
			InitSamples();
			var before = GetCountOfSamples();
			Assert.AreNotEqual(0, before);
			var dw = GetDataWorker();
			var testobject = new SampleClass {
				Name = "Test Object"
			};
			Assert.AreEqual(0, testobject.ID);
			var result = dw.Insert<SampleClass>(testobject);
			var after = GetCountOfSamples();
			var expectedAfter = before + 1;
			Assert.IsTrue(result);
			Assert.AreEqual(expectedAfter, after);
			Assert.AreNotEqual(0, testobject.ID);
		}
		
		[Test]
		public void Test_DW_Delete()
		{
			InitSamples();
			var before = GetCountOfSamples();
			Assert.AreNotEqual(0, before);
			var dw = GetDataWorker();
			var testobject = new SampleClass {
				ID = 1
			};
			Assert.AreNotEqual(0, testobject.ID);
			var result = dw.Delete<SampleClass>(testobject);
			var after = GetCountOfSamples();
			var expectedAfter = before - 1;
			Assert.IsTrue(result);
			Assert.AreEqual(expectedAfter, after);
			Assert.AreNotEqual(0, testobject.ID);
		}
		
		[Test]
		public void Test_DW_Fetch()
		{
			InitSamples();
			var before = GetCountOfSamples();
			Assert.AreNotEqual(0, before);
			var dw = GetDataWorker();
			int loadID = 3;
			var testobject = new SampleClass {
				ID = loadID
			};
			Assert.AreNotEqual(0, testobject.ID);
			Assert.IsNullOrEmpty(testobject.Name);
			var result = dw.Fetch<SampleClass>(testobject);
			var after = GetCountOfSamples();
			var expectedAfter = before;
			var expectedName = "three";
			Assert.IsNotNull(result);
			Assert.AreEqual(expectedAfter, after);
			Assert.AreEqual(expectedName, result.Name);
			Assert.AreEqual(loadID, result.ID);
		}
		
		[Test]
		public void Test_DW_Fetch_InferredType()
		{
			InitSamples();
			var before = GetCountOfSamples();
			Assert.AreNotEqual(0, before);
			var dw = GetDataWorker();
			int loadID = 3;
			var testobject = new SampleClass {
				ID = loadID
			};
			Assert.AreNotEqual(0, testobject.ID);
			Assert.IsNullOrEmpty(testobject.Name);
			var result = dw.Fetch(testobject);
			var after = GetCountOfSamples();
			var expectedAfter = before;
			var expectedName = "three";
			Assert.IsNotNull(result);
			Assert.AreEqual(expectedAfter, after);
			Assert.AreEqual(expectedName, result.Name);
			Assert.AreEqual(loadID, result.ID);
		}
		
		[Test]
		public void Test_DW_Fetch_WithCustomCommandTextWithParams()
		{
			InitSamples();
			var before = GetCountOfSamples();
			Assert.AreNotEqual(0, before);
			var dw = GetDataWorker();
			int loadID = 3;
			var testobject = new SampleClass {
				ID = loadID
			};
			Assert.AreNotEqual(0, testobject.ID);
			Assert.IsNullOrEmpty(testobject.Name);
			var result = dw.Fetch<SampleClass>("FetchSampleSpecial", testobject);
			var after = GetCountOfSamples();
			var expectedAfter = before;
			var expectedName = "three";
			Assert.IsNotNull(result);
			Assert.AreEqual(expectedAfter, after);
			Assert.AreEqual(expectedName, result.Name);
			Assert.AreEqual(loadID, result.ID);
			Assert.AreEqual("[FetchSampleSpecial]", result.SpecialExtra);
		}

		[Test]
		public void Test_DW_Fetch_WithCustomCommandTextNoParams()
		{
			InitSamples();
			var before = GetCountOfSamples();
			Assert.AreNotEqual(0, before);
			var dw = GetDataWorker();
			var result = dw.Fetch<SampleClass>("FetchSampleSpecialFirst");
			var after = GetCountOfSamples();
			var expectedAfter = before;
			var expectedName = "one";
			Assert.IsNotNull(result);
			Assert.AreEqual(expectedAfter, after);
			Assert.AreEqual(expectedName, result.Name);
			Assert.AreEqual(1, result.ID);
			Assert.AreEqual("[FetchSampleSpecialFirst]", result.SpecialExtra);
		}
		
		[Test]
		public void Test_DW_FetchByAllProps()
		{
			InitSamples();
			var before = GetCountOfSamples();
			Assert.AreNotEqual(0, before);
			var dw = GetDataWorker();
			int loadID = 3;
			var testobject = new SampleClassPoco {
				ID = loadID
			};
			Assert.AreNotEqual(0, testobject.ID);
			Assert.IsNullOrEmpty(testobject.Name);
			var result = dw.FetchByAllProps<SampleClassPoco>(testobject);
			var after = GetCountOfSamples();
			var expectedAfter = before;
			var expectedName = "three";
			Assert.IsNotNull(result);
			Assert.AreEqual(expectedAfter, after);
			Assert.AreEqual(loadID, testobject.ID);
			Assert.AreEqual(expectedName, result.Name);
		}
		
		[Test]
		public void Test_DW_FetchByAllProps_NoResults_ExpectNull()
		{
			InitSamples();
			var before = GetCountOfSamples();
			Assert.AreNotEqual(0, before);
			var dw = GetDataWorker();
			int loadID = -1;
			var testobject = new SampleClassPoco {
				ID = loadID
			};
			Assert.AreNotEqual(0, testobject.ID);
			Assert.IsNullOrEmpty(testobject.Name);
			var result = dw.FetchByAllProps<SampleClassPoco>(testobject);
			var after = GetCountOfSamples();
			var expectedAfter = before;
			Assert.IsNull(result);
		}
		
		
		[Test]
		public void Test_DW_Fetch_NoResult()
		{
			InitSamples();
			var before = GetCountOfSamples();
			Assert.AreNotEqual(0, before);
			var dw = GetDataWorker();
			int loadID = -1;
			var testobject = new SampleClass {
				ID = loadID
			};
			Assert.AreNotEqual(0, testobject.ID);
			Assert.IsNullOrEmpty(testobject.Name);
			var result = dw.Fetch<SampleClass>(testobject);
			var after = GetCountOfSamples();
			var expectedAfter = before;
			Assert.IsNull(result);
			Assert.AreEqual(expectedAfter, after);
			Assert.AreEqual(loadID, testobject.ID);
		}

		
		[Test]
		public void Test_DW_Update()
		{
			InitSamples();
			var countBefore = GetCountOfSamples();
			Assert.AreNotEqual(0, countBefore);
			var dw = GetDataWorker();
			int testID = 4;
			string oldName = "four";
			string newName = "supertweetgoestherat";
			string nameBefore = GetNameOfSample(testID);
			var testobject = new SampleClass {
				ID = testID,
				Name = newName
			};
			Assert.AreNotEqual(0, testobject.ID);
			Assert.IsNotNull(testobject.Name);
			Assert.AreEqual(oldName, nameBefore);
			var result = dw.Update<SampleClass>(testobject);
			var countAfter = GetCountOfSamples();
			var nameAfter = GetNameOfSample(testID);
			var expectedCountAfter = countBefore;
			var expectedNameAfter = newName;
			Assert.IsTrue(result);
			Assert.AreEqual(expectedCountAfter, countAfter);
			Assert.AreEqual(newName, nameAfter);
			Assert.AreNotEqual(nameBefore, nameAfter);
		}
		
		[Test]
		public void Test_DW_List()
		{
			InitSamples();
			var countBefore = GetCountOfSamples();
			Assert.AreNotEqual(0, countBefore);
			var dw = GetDataWorker();
			var result = dw.List<SampleClass>();
			var countAfter = GetCountOfSamples();
			var expectedCountAfter = countBefore;
			Assert.IsNotNull(result);
			Assert.AreEqual(expectedCountAfter, countAfter);
			Assert.AreEqual(expectedCountAfter, result.Count);
			foreach (var element in result) {
				Assert.IsNotNullOrEmpty(element.Name);
				Assert.AreNotEqual(0, element.ID);
				Assert.IsNull(element.SpecialExtra);
			}
		}

		//[Test] // SLOW TEST
		public void Test_DW_List_Lots_SqlText()
		{
			var dw = GetDataWorker();

			var dwt = new DataWorkerTask();
			dwt.CommandText = "select * from LotsOfRows";
			dwt.CommandType = CommandType.Text;
			dwt.ReadResults = true;
			var startT = Environment.TickCount;
			var result = dw.DoWorkDirect<LotsClass>(dwt);
			var endT = Environment.TickCount;
			Console.WriteLine("Query Time: {0:N} ms",endT-startT);
			
			Assert.IsNotNull(result);
			foreach (var element in result) {
				Assert.AreNotEqual(0, element.ID);
				Assert.AreNotEqual(0, element.Number);
				Assert.IsNotNullOrEmpty(element.Name);
			}
			Console.WriteLine("Total Rows: {0}",result.Count);
			
		}

		//[Test] // SLOW TEST
		public void Test_DW_List_Lots_StoredProc()
		{
			var dw = GetDataWorker();

			var dwt = new DataWorkerTask();
			dwt.CommandText = "ListLotsOfRows";
			dwt.CommandType = CommandType.StoredProcedure;
			dwt.ReadResults = true;
			var startT = Environment.TickCount;
			var result = dw.DoWorkDirect<LotsClass>(dwt);
			var endT = Environment.TickCount;
			Console.WriteLine("Query Time: {0:N} ms",endT-startT);
			
			Assert.IsNotNull(result);
			foreach (var element in result) {
				Assert.AreNotEqual(0, element.ID);
				Assert.AreNotEqual(0, element.Number);
				Assert.IsNotNullOrEmpty(element.Name);
			}
			Console.WriteLine("Total Rows: {0}",result.Count);
			
		}

		//[Test] // SLOW TEST
		public void Test_DW_List_Lots_ClassListProc()
		{
			var dw = GetDataWorker();
			var startT = Environment.TickCount;
			var result = dw.List<LotsClass>();
			var endT = Environment.TickCount;
			Console.WriteLine("Query Time: {0:N} ms",endT-startT);
			
			Assert.IsNotNull(result);
			foreach (var element in result) {
				Assert.AreNotEqual(0, element.ID);
				Assert.AreNotEqual(0, element.Number);
				Assert.IsNotNullOrEmpty(element.Name);
			}
			Console.WriteLine("Total Rows: {0}",result.Count);
			
		}
		
		[Test]
		public void Test_DW_List_WithCustomCommandText()
		{
			InitSamples();
			var countBefore = GetCountOfSamples();
			Assert.AreNotEqual(0, countBefore);
			var dw = GetDataWorker();
			var result = dw.ListCmd<SampleClass>("ListSamplesSpecial");
			var countAfter = GetCountOfSamples();
			var expectedCountAfter = countBefore;
			Assert.IsNotNull(result);
			Assert.AreEqual(expectedCountAfter, countAfter);
			Assert.AreEqual(expectedCountAfter, result.Count);
			foreach (var element in result) {
				Assert.IsNotNullOrEmpty(element.Name);
				Assert.AreNotEqual(0, element.ID);
				Assert.AreEqual("[ListSamplesSpecial]",element.SpecialExtra);
			}
		}

		[Test]
		public void Test_InvalidMode()
		{
			var dw = GetDataWorker();
			var dwt = new DataWorkerTask {
				Mode = DataWorkerMode.Invalid,
				CommandText = "select null",
				CommandType = CommandType.Text,
			};
			dw.DoWorkDirect<object>(dwt);
		}

		[Test]
		[ExpectedException(typeof(ArgumentException))]
		public void Test_InvalidModeOnObject()
		{
			var dw = GetDataWorker();
			var dwt = new DataWorkerTask {
				Mode = DataWorkerMode.Invalid,
				CommandText = "select null",
				CommandType = CommandType.Text,
			};
			var testObject = new TestObject();
			dw.DoWorkDirect<object>(dwt, testObject);
		}
		
		// TODO: decimal with precision other than 1 test (once caching is implemented)
		// * IDBParamater does not have precision and scale like the sql specific implementaion.
		//   use reflection to cache all CONCRETE Parameter properties
		

		// TODO: allow property multi-mapping
		//  * single property, allow property name and datafield name(s) to map from multiple queries and as multiple inputs
	}
}

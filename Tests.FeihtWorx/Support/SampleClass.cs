/*
 * Created by SharpDevelop.
 * User: thief
 * Date: 2010/06/06
 * Time: 12:42 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using FeihtWorx.Data;

namespace Tests.FeihtWorx
{
	/// <summary>
	/// Description of SampleClass.
	/// </summary>
	[DataClass(
		InsertProcedure = "InsertSample"
		, DeleteProcedure = "DeleteSample"
		, FetchProcedure = "FetchSample"
		, UpdateProcedure = "UpdateSample"
		, ListProcedure = "ListSamples"
	)]
	public class SampleClass
	{
		[DataField]
		public long ID { get; set; }
		[DataField]
		public string Name { get; set; }
		[DataField]
		public string Feedout { get; set; }
		[DataField]
		public string TestColumn { get; set; }
		[DataField]
		public string SpecialExtra { get; set; } // used for confirming we are actually running the special fetch
		
		[DataField]
		public string ReadOnlyProperty { get { return "---"; } }
		
		[DataField]
		public string WriteOnlyProperty { set { } }
		
		[DataField]
		public string WriteOnlyPropertyOutput { get; set; }
		// == For unit testing

		[DataField("ReallyNotSimpleColumnNameThatNoOneCanEverRememberIn")]
		public string SimpleColumnIn { get; set; }
		// == For unit testing

		[DataField("ReallyNotSimpleColumnNameThatNoOneCanEverRememberOut")]
		public string SimpleColumnOut { get; set; }
		// == For unit testing
		
		[DataField]
		public int RequiredParam { get; set; }
		// == For unit testing

		[DataField]
		public int? OptionalParam1 { get; set; }
		// == For unit testing

		[DataField]
		public int? OptionalParam2 { get; set; }
		// == For unit testing

		[DataField]
		public int RequiredParamOut { get; set; }
		// == For unit testing

		[DataField]
		public int? OptionalParam1Out { get; set; }
		// == For unit testing

		[DataField]
		public int? OptionalParam2Out { get; set; }
		// == For unit testing

		// xxx todo: test all data types
		// xxx todo: test field without datafield attribute stays unpopulated on return data
		// xxx todo: test field without datafield attribute stays unpopulated for input parameters
		// xxx todo: test field with different name in datafield attribute gets populated correctly
		
		
	}
}

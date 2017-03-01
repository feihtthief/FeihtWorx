/*
 * Created by SharpDevelop.
 * User: thief
 * Date: 2010/06/17
 * Time: 08:55 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using FeihtWorx.Data;

namespace Tests.FeihtWorx
{
	/// <summary>
	/// This class is used to test the AllProperties Mode
	/// </summary>
	[DataClass(FetchProcedure = "FetchSample")]
	public class SampleClassPoco
	{
		public long ID { get; set; }
		public string Name { get; set; }
		public string Feedout { get; set; }
			
		// xxx todo test with read-only props to see what happens when they are set to output
	}
}

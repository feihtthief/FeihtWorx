/*
 * Created by SharpDevelop.
 * User: thief
 * Date: 2013/03/19
 * Time: 10:56 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using FeihtWorx.Data;

namespace Tests.FeihtWorxData.ManualTests
{
	[DataClass(
		ListProcedure = "ListDemos"
		, InsertProcedure = "InsertDemo"
		, FetchProcedure = "FetchDemo"
		, UpdateProcedure = "UpdateDemo"
		, DeleteProcedure = "DeleteDemo"
	)]
	public class Demo
	{
		[DataField]
		public int ID { get; set; }
		[DataField]
		public string Name { get; set; }
		[DataField]
		public string Group { get; set; }
		[DataField]
		public string Description { get; set; }
		[DataField]
		public DateTime ReleaseDate{ get; set; }
	}
}

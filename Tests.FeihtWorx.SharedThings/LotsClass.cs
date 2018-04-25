/*
 * Created by SharpDevelop.
 * User: thief
 * Date: 2018/04/25
 * Time: 00:29
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using FeihtWorx.Data;

namespace Tests.FeihtWorx.SharedThings
{
	/// <summary>
	/// Description of LotsClass.
	/// </summary>
	[DataClass(
		  InsertProcedure = ""
		, DeleteProcedure = ""
		, FetchProcedure = ""
		, UpdateProcedure = ""
		, ListProcedure = "ListLotsOfRows"
	)]
	public class LotsClass
	{
		[DataField]
		public int ID { get; set; }
		[DataField]
		public int Number { get; set; }
		[DataField]
		public string Name { get; set; }
		}
}

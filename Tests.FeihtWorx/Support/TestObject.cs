/*
 * Created by SharpDevelop.
 * User: thief
 * Date: 2010/12/17
 * Time: 11:30 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using FeihtWorx.Data;

namespace Tests.FeihtWorx
{
	/// <summary>
	/// WORK IN PROGRESS
	/// </summary>
	[DataClass(
		InsertProcedure="InsertTestObject"
	)]
	public class TestObject
	{
		[DataField]
		public int ID {get;set;}
		
	}
}

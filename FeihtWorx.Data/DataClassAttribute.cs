/*
 * Created by SharpDevelop.
 * User: thief
 * Date: 2010/06/05
 * Time: 11:40 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace FeihtWorx.Data
{
	/// <summary>
	/// Description of DataClassAttribute.
	/// </summary>
	[AttributeUsageAttribute(AttributeTargets.Class)]
	public class DataClassAttribute:Attribute
	{
		public String ListProcedure { get; set; }
		public String InsertProcedure { get; set; }
		public String FetchProcedure { get; set; }
		public String UpdateProcedure { get; set; }
		public String DeleteProcedure { get; set; }
		
		public DataClassAttribute()
		{
		}
	}
}

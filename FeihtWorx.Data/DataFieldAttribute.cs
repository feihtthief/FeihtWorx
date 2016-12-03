/*
 * Created by SharpDevelop.
 * User: thief
 * Date: 2010/06/05
 * Time: 11:41 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace FeihtWorx.Data
{
	/// <summary>
	/// Description of DataFieldAttribute.
	/// </summary>
	[AttributeUsageAttribute(AttributeTargets.Property,AllowMultiple=true)]
	public class DataFieldAttribute:Attribute
	{
		public String FieldName{get; set;}
		public DataFieldAttribute(){}
		public DataFieldAttribute(string fieldName) {
			this.FieldName = fieldName;
		}
	}
}

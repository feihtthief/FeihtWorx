/*
 * Created by SharpDevelop.
 * User: thief
 * Date: 2010/11/23
 * Time: 09:34 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Reflection;

namespace FeihtWorx.Data
{
	/// <summary>
	/// Description of PropertyValuePair.
	/// </summary>
	public class PropertyValuePair:Pair<PropertyInfo, object>
	{
		public PropertyValuePair(PropertyInfo propInfo, object value):base(propInfo,value){}
	}
}

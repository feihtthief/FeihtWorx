/*
 * Created by SharpDevelop.
 * User: thief
 * Date: 2010/06/16
 * Time: 10:12 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Reflection;

namespace FeihtWorx.Data
{
	/// <summary>
	/// Description of PropertyIndexPair.
	/// </summary>
	public class PropertyIndexPair:Pair<PropertyInfo, int>
	{
		public PropertyIndexPair(PropertyInfo propInfo, int index):base(propInfo,index){}
	}
}

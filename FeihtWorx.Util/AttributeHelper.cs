/*
 * Created by SharpDevelop.
 * User: thief
 * Date: 2010/06/11
 * Time: 10:40 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Reflection;
using System.Collections.Generic;

namespace FeihtWorx.Util
{
	public static class AttributeHelper
	{

		//public static T GetOnlyPropertyAttribute<T>(MemberInfo memberInfo) where T:Attribute
		//{
		//	var tmp = GetPropertyAttributes<T>(memberInfo);
		//	if (tmp.Length==1) {
		//		return tmp[0];
		//	}
		//	return null;
		//}

		public static T GetFirstPropertyAttribute<T>(MemberInfo memberInfo) where T:Attribute
		{
			var tmp = GetPropertyAttributes<T>(memberInfo);
			if (tmp.Length > 0) {
				return tmp[0];
			}
			return null;
		}

		public static T[] GetPropertyAttributes<T>(MemberInfo memberInfo) where T:Attribute
		{
			var attributes = memberInfo.GetCustomAttributes(typeof(T), true);
			var result = new T[attributes.Length];
			for (int i = 0; i < attributes.Length; i++) {
				result[i] = (T)attributes[i];
			}
			return result;
		}
		
	}
}
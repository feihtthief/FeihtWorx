/*
 * Created by SharpDevelop.
 * User: thief
 * Date: 2010/06/14
 * Time: 09:56 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace Tests.FeihtWorx
{
	/// <summary>
	/// Description of MultiUsedAttribute.
	/// </summary>
	[AttributeUsageAttribute(AttributeTargets.Property,AllowMultiple=true)]
	public class MultiUsedProofAttribute:Attribute
	{
	}
}

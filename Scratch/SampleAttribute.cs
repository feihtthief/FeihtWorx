/*
 * Created by SharpDevelop.
 * User: thief
 * Date: 2010/06/13
 * Time: 07:14 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace Scratch
{
	/// <summary>
	/// Description of SampleAttribute.
	/// </summary>
	public class SampleAttribute:Attribute
	{
		public string Name { get; set; }
		
		public SampleAttribute()
		{
			Name = null;
		}
		
		public SampleAttribute(string name)
		{
			Name = name;
		}
	}
}

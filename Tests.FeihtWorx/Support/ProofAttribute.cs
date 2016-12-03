/*
 * Created by SharpDevelop.
 * User: thief
 * Date: 2010/06/13
 * Time: 07:14 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace Tests.FeihtWorx
{
	/// <summary>
	/// Description of SampleAttribute.
	/// </summary>
	public class ProofAttribute:Attribute
	{
		public string Name {get;set;}
		
		public ProofAttribute()
		{
			Name=null;
		}
		
		public ProofAttribute(string name)
		{
			Name=name;
		}
	}
}

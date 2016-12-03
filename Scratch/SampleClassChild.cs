/*
 * Created by SharpDevelop.
 * User: thief
 * Date: 2010/06/13
 * Time: 08:19 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace Scratch
{
	/// <summary>
	/// Description of SampleClassChild.
	/// </summary>
	public class SampleClassChild:SampleClassBase
	{
		[Sample]
		public String SampleName{get;set;}
		
		[Other]
		public String OtherName{get;set;}

		public String Barename{get;set;}
	}
}

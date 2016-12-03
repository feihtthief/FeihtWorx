/*
 * Created by SharpDevelop.
 * User: thief
 * Date: 2010/06/06
 * Time: 12:42 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace Scratch
{
	/// <summary>
	/// Description of SampleClass.
	/// </summary>
	[Sample]
	public class SampleClassBase
	{
		[Sample]
		public int ID {get;set;}
		[Sample]
		public string Name {get;set;}
		
		[Other]
		public int Age {get;set;}
		[Other]
		public string OldName {get;set;}
		
		
		public int SomeThing {get;set;}
		public string Moo {get;set;}

	}
}

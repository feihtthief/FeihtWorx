/*
 * Created by SharpDevelop.
 * User: thief
 * Date: 2010/06/14
 * Time: 09:45 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using NUnit.Framework;

namespace Tests.FeihtWorx
{
	public class ProofClassForProofAttribute
	{
		[ProofAttribute]
		public string SomeString {get;set;}

		public string SomeOtherString {get;set;}

	}
}

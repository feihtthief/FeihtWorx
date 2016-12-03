/*
 * Created by SharpDevelop.
 * User: thief
 * Date: 2013/07/27
 * Time: 10:11 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace Tests.FeihtWorx
{
	/// <summary>
	/// Description of ProofClassForProofAttributeWithNameParam.
	/// </summary>
	public class ProofClassForProofAttributeWithNameParam
	{
		[ProofAttribute("SomeName")]
		public string SomeString {get;set;}

		public string SomeOtherString {get;set;}

	}
}

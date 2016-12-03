/*
 * Created by SharpDevelop.
 * User: thief
 * Date: 2010/06/14
 * Time: 09:57 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace Tests.FeihtWorx
{
	/// <summary>
	/// Description of ProofClassForMuliUsedProofAttribute.
	/// </summary>
	public class ProofClassForMultiUsedProofAttribute
	{
		// -- intentionally re-used the same attributes here
		
		[MultiUsedProofAttribute]
		[MultiUsedProofAttribute]
		public string Bla1{get;set;}
		
		[MultiUsedProofAttribute]
		[MultiUsedProofAttribute]
		[MultiUsedProofAttribute]
		public string Bla2{get;set;}
		
	}
}

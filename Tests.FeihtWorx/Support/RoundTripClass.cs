/*
 * Created by SharpDevelop.
 * User: thief
 * Date: 2016/11/14
 * Time: 21:34
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using FeihtWorx.Data;

namespace Tests.FeihtWorx
{
	/// <summary>
	/// Description of RoundTripClass.
	/// </summary>
	public class RoundTripClass
	{
		[DataField]
		public int ValueA { get; set; }
		[DataField]
		public int ValueB { get; set; }
		[DataField]
		public int ValueC { get; set; }
	}
}

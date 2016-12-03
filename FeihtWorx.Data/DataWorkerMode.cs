/*
 * Created by SharpDevelop.
 * User: thief
 * Date: 2010/06/14
 * Time: 10:08 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace FeihtWorx.Data
{
	// todo: xxx split DataWorkerMode in DataWorkerInputMode and DataWorkerOutputMode
	// input mode has all 3 (datafield, allprops and dictionary)
	// output mode has only datafield and allprops, because dictionary would be insane.
	
	
	/// <summary>
	/// Description of DataWorkerMode.
	/// </summary>
	public enum DataWorkerMode
	{
		Invalid ,
		DataFields ,
		AllProperties,
		Dictionary,
	}
}

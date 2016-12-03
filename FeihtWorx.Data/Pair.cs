/*
 * Created by SharpDevelop.
 * User: thief
 * Date: 2010/06/16
 * Time: 10:07 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace FeihtWorx.Data
{
	/// <summary>
	/// Description of Pair.
	/// </summary>
	public class Pair<TKey,TValue>
	{
		public TKey Key {get;private set;}
		public TValue Value {get;private set;}
		public Pair(TKey key,TValue value)
		{
			Key = key;
			Value = value;
		}
	}
}

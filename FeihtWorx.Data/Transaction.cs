/*
 * Created by SharpDevelop.
 * User: thief
 * Date: 2010/06/11
 * Time: 10:52 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Data;
using System.Data.Common;

namespace FeihtWorx.Data
{
	/// <summary>
	/// Description of Transaction.
	/// </summary>
	public class Transaction
	{
		internal DbTransaction ActualTransaction{ get; private set; }
		
		public Transaction(DbTransaction transaction)
		{
			if (transaction == null) {
				throw new ArgumentNullException("transaction");
			}
			ActualTransaction = transaction;
		}
		
		public void Rollback()
		{
			ActualTransaction.Rollback();			
		}
		
		public void Commit()
		{
			ActualTransaction.Commit();
		}
		
	}
}

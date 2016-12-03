/*
 * Created by SharpDevelop.
 * User: thief
 * Date: 2010/06/13
 * Time: 07:17 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using NUnit.Framework;
using FeihtWorx.Util;

namespace Tests.FeihtWorx
{
	[TestFixture]
	public class TestAttributeHelper
	{
		[Test]
		public void TestGetAttribute()
		{
			var props = typeof(ProofClassForProofAttribute).GetProperties();
			var totalFound =0;
			foreach	(var prop in props) {
				var attr = AttributeHelper.GetFirstPropertyAttribute<ProofAttribute>(prop);
				if (attr!=null){
					totalFound++;
				}
			}
			Assert.AreEqual(1,totalFound,"Expected to find exactly one attribute");
		}
		
		[Test]
		public void TestGetAttributeUsedWithNameParam()
		{
			var props = typeof(ProofClassForProofAttributeWithNameParam).GetProperties();
			var totalFound =0;
			foreach	(var prop in props) {
				var attr = AttributeHelper.GetFirstPropertyAttribute<ProofAttribute>(prop);
				if (attr!=null){
					totalFound++;
				}
			}
			Assert.AreEqual(1,totalFound,"Expected to find exactly one attribute");
		}
		
		[Test]
		public void TestGetAttributeWhenNoneUsed()
		{
			var props = typeof(ProofClassForUnusedProofAttribute).GetProperties();
			var totalInspected = 0;
			foreach	(var prop in props) {
				totalInspected++;
				var attr = AttributeHelper.GetFirstPropertyAttribute<UnusedProofAttribute>(prop);
				Assert.IsNull(attr,"Expected to get back null for unused attribute");
			}
			Assert.AreNotEqual(0,totalInspected,"Expected to have inspected some attributes");
		}
		
		
		[Test]
		public void TestGetAttributeWhenMultiUsed()
		{
			var props = typeof(ProofClassForMultiUsedProofAttribute).GetProperties();
			Assert.IsTrue(props.Length>0,"Expected to have properties");
			var totalFound =0;
			foreach	(var prop in props) {
				var attr = AttributeHelper.GetFirstPropertyAttribute<MultiUsedProofAttribute>(prop);
				if (attr!=null) {
					totalFound ++;
				}
			}
			Assert.AreEqual(props.Length,totalFound,"Expected to find 1 attribute per Property)");
		}
		
		[Test]
		public void TestGetAttributesWhenMultiUsed()
		{
			var props = typeof(ProofClassForMultiUsedProofAttribute).GetProperties();
			var totalFound =0;
			foreach	(var prop in props) {
				var attrs = AttributeHelper.GetPropertyAttributes<MultiUsedProofAttribute>(prop);
				foreach(var attr in attrs) {
					totalFound++;
				}
			}
			Assert.AreEqual(5,totalFound,"Expected to find 5 attributes");
		}
		
		[Test]
		public void TestGetAttributesWhenNone()
		{
			var props = typeof(ProofClassForUnusedProofAttribute).GetProperties();
			Assert.IsTrue(props.Length>0,"Expected to have properties");
			foreach	(var prop in props) {
				var attrs = AttributeHelper.GetPropertyAttributes<MultiUsedProofAttribute>(prop);
				Assert.IsTrue(attrs.Length==0,"Expected to find no attributes");
			}
		}
		
	}
}

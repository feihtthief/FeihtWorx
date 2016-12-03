/*
 * Created by SharpDevelop.
 * User: thief
 * Date: 2013/03/19
 * Time: 10:56 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using FeihtWorx.Data;

namespace Tests.FeihtWorxData.ManualTests
{
	class Program
	{
		private const string SqlProvider = @"System.Data.SqlClient";
		private const string SqlConnectionString = @"Data Source=.\sqlexpress;Integrated Security=SSPI;Initial Catalog=DataWorkerTest;";
		
		private const ConsoleColor BAD = ConsoleColor.Red;
		private const ConsoleColor GOOD = ConsoleColor.Green;
		private const ConsoleColor NOTE = ConsoleColor.DarkGray;

		private static FeihtWorx.Data.DataWorker dw = new DataWorker(SqlProvider, SqlConnectionString);
		
		public static void Main(string[] args)
		{
			try {
				Console.WriteLine("Lets test with some demos...");
				var baseDemo = new Demo {
					Name = string.Format("Demo to be nuked from {0:yyyy/MMM/dd HH:mm:ss}", DateTime.Now),
					Group = "[Nuke Group]",
					Description = "Test Demo"
				};
				
				// Insert
				Say(NOTE, "Testing Insert");
				var newDemo = new Demo {
					Name = baseDemo.Name,
					Group = baseDemo.Group,
					Description = baseDemo.Description,
				};
				dw.Insert(newDemo);
				Console.WriteLine("New Demo ID: {0}", newDemo.ID);

				// List
				Say(NOTE, "Testing List");
				ListDemosToScreen();
				
				// Fetch
				Say(NOTE, "Testing Fetches");
				Demo fetchedDemo;
				fetchedDemo = dw.Fetch(new Demo{ ID = 1 }); // Invokes <T>(T) with T = Demo (inferred)
				if (fetchedDemo == null) {
					Say(BAD, "Fetched Demo is Null! (Expected Data to be there)");
				} else {
					Console.WriteLine("Fetched Demo Name: {0}", fetchedDemo.Name);
				}
				
				fetchedDemo = dw.Fetch<Demo>(new Demo{ ID = 2 }); // Invokes <T>(T) with T = Demo (explicit)
				if (fetchedDemo == null) {
					Say(BAD, "Fetched Demo is Null! (Expected Data to be there)");
				} else {
					Console.WriteLine("Fetched Demo Name: {0}", fetchedDemo.Name);
				}

				fetchedDemo = dw.Fetch<Demo>(new IDObj{ ID = 3 }); // Invokes <T>(object) with T = Demo
				if (fetchedDemo == null) { 
					Say(BAD, "Fetched Demo is Null! (Expected Data to be there)");
				} else {
					Console.WriteLine("Fetched Demo Name: {0}", fetchedDemo.Name);
				}
				
				fetchedDemo = dw.FetchByAllProps<Demo>(new {ID = 4}); // as Object
				if (fetchedDemo == null) {
					Say(BAD, "Fetched Demo is Null! (Expected Data to be there)");
				} else {
					Console.WriteLine("Fetched Demo Name: {0}", fetchedDemo.Name);
				}
				
				// Update
				Say(NOTE, "Testing Update");
				newDemo.Description += "!";
				dw.Update(newDemo);
				fetchedDemo = dw.Fetch(newDemo);
				if (fetchedDemo == null) {
					Say(BAD, "Fetched Demo is Null! (Expected Data to be there)");
				} else {
					Console.WriteLine("Fetched Demo Description: {0}", fetchedDemo.Description);
				}
				
				// Delete
				Say(NOTE, "Testing Delete");
				dw.Delete(newDemo);
				fetchedDemo = dw.Fetch(newDemo);
				if (fetchedDemo == null) {
					Say(GOOD, "Fetched Demo is Null. (This is good as it was meant to be deleted)");
				} else {
					Say(BAD, "Fetched Demo is Not Null! (Expected null as should be deleted)");
				}
				
				// Insert 5 more tests
				List<int> tempIDs = new List<int>();
				Say(NOTE, "Adding more test entries.");
				for (int i = 0; i < 5; i++) {
					newDemo.Name += ".";
					if (i > 2) {
						newDemo.Group += ".";
					}
					dw.Insert(newDemo);
					Console.WriteLine(newDemo.ID);
					tempIDs.Add(newDemo.ID);
				}
				
				Say(NOTE, "List current Demos");
				ListDemosToScreen();
				
				dw.DoNonQuery("DeleteDemosByGroupName", baseDemo);
				Say(NOTE, "List after DeleteDemosByGroupName");
				ListDemosToScreen();
				
				dw.DoNonQuery("DeleteNukeDemos");
				Say(NOTE, "List after DeleteNukeDemos");
				ListDemosToScreen();
				
				
			} catch (Exception exc) {
				Say(BAD, exc.Message);
				if (exc.InnerException != null) {
					Say(BAD, exc.InnerException.Message);
				}
			}
			Console.WriteLine("<Done> Press any key to exit...");
			Console.ReadKey(true);
			Console.WriteLine("BYE");
			
		}

		static void ListDemosToScreen()
		{
			var demos = dw.List<Demo>();
			foreach (var demo in demos) {
				Console.WriteLine("{0,5}|{1,-50}|{2,-20}|{3,-20}", demo.ID, demo.Name, demo.Group, demo.Description);
			}
		}
		
		static void Say(ConsoleColor color, string message)
		{
			Console.ForegroundColor = color;
			Console.WriteLine(message);
			Console.ResetColor();
		}
		
	}
}
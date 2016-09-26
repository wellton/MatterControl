﻿using System;
using System.Threading;
using MatterHackers.Agg.UI.Tests;
using MatterHackers.GuiAutomation;
using NUnit.Framework;

namespace MatterHackers.MatterControl.Tests.Automation
{
	[TestFixture, Category("MatterControl.UI.Automation"), RunInApplicationDomain]
	public class HardwareLevelingUITests
	{
		[Test, Apartment(ApartmentState.STA), RunInApplicationDomain]
		public void HasHardwareLevelingHidesLevelingSettings()
		{
			Action<AutomationTesterHarness> testToRun = (AutomationTesterHarness resultsHarness) =>
			{
				AutomationRunner testRunner = new AutomationRunner(MatterControlUtilities.DefaultTestImages);
				{
					MatterControlUtilities.PrepForTestRun(testRunner);
					//Add printer that has hardware leveling
					MatterControlUtilities.AddAndSelectPrinter(testRunner, "Airwolf 3D", "HD");

					MatterControlUtilities.SwitchToAdvancedSettings(testRunner, resultsHarness);

					testRunner.ClickByName("Printer Tab", 1);
					testRunner.Wait(1);

					//Make sure Print Leveling tab is not visible 
					bool testPrintLeveling = testRunner.WaitForName("Print Leveling Tab", 3);
					resultsHarness.AddTestResult(testPrintLeveling == false);

					//Add printer that does not have hardware leveling
					MatterControlUtilities.AddAndSelectPrinter(testRunner, "3D Factory", "MendelMax 1.5");

					testRunner.ClickByName("Slice Settings Tab",1);

					testRunner.ClickByName("Printer Tab",1);

					//Make sure Print Leveling tab is visible
					bool printLevelingVisible = testRunner.WaitForName("Print Leveling Tab", 2);
					resultsHarness.AddTestResult(printLevelingVisible == true);

					MatterControlUtilities.CloseMatterControl(testRunner);
				}
			};

			AutomationTesterHarness testHarness = MatterControlUtilities.RunTest(testToRun, overrideHeight: 800);

			Assert.IsTrue(testHarness.AllTestsPassed(4));
		}

		[Test, Apartment(ApartmentState.STA), RunInApplicationDomain]
		public void SoftwareLevelingRequiredCorrectWorkflow()
		{
			Action<AutomationTesterHarness> testToRun = (AutomationTesterHarness resultsHarness) =>
			{
				AutomationRunner testRunner = new AutomationRunner(MatterControlUtilities.DefaultTestImages);
				{
					MatterControlUtilities.PrepForTestRun(testRunner);

					// make a jump start printer
					MatterControlUtilities.LaunchAndConnectToPrinterEmulator(testRunner, false, "JumStart", "V1");

					// make sure it is showing the correct button
					resultsHarness.AddTestResult(!testRunner.WaitForName("Start Print Button", .5), "Start Print hidden");
					resultsHarness.AddTestResult(testRunner.WaitForName("Finish Setup Button", .5), "Finish Setup showing");

					// do print leveling
					testRunner.ClickByName("Next Button", .5);
					testRunner.ClickByName("Next Button", .5);
					testRunner.ClickByName("Next Button", .5);
					for (int i = 0; i < 3; i++)
					{
						testRunner.ClickByName("Move Z positive", .5);
						testRunner.ClickByName("Next Button", .5);
						testRunner.ClickByName("Next Button", .5);
						testRunner.ClickByName("Next Button", .5);
					}

					resultsHarness.AddTestResult(testRunner.ClickByName("Done Button", .5), "Found Done button");

					// make sure the button has changed to start print
					resultsHarness.AddTestResult(testRunner.WaitForName("Start Print Button", .5), "Start Print showing");
					resultsHarness.AddTestResult(!testRunner.WaitForName("Finish Setup Button", .5), "Finish Setup hidden");

					MatterControlUtilities.CloseMatterControl(testRunner);
				}
			};

			AutomationTesterHarness testHarness = MatterControlUtilities.RunTest(testToRun, overrideHeight: 800);

			Assert.IsTrue(testHarness.AllTestsPassed(5));
		}
	}
}


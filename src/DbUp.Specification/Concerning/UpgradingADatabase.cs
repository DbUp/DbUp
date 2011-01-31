using System;
using NUnit.Framework;
using NSubstitute;
using DbUp.Specification.Contexts;

namespace DbUp.Specification.Concerning
{
	[TestFixture()]
	public class UpgradingADatabase : GivenAnOutOfDateDatabase
	{
		[Test()]
		public void ShouldDetermineIfUpgradeIsRequired ()
		{
			Assert.IsTrue(DbUpgrader.IsUpgradeRequired(Log));
		}
	}
}


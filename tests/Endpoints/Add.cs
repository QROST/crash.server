// ReSharper disable HeapView.BoxingAllocation

using System.Text;

using Microsoft.AspNetCore.Authorization.Infrastructure;

#pragma warning disable CS8602 // Dereference of a possibly null reference.

namespace Crash.Server.Tests.Endpoints
{
	public sealed class Add : CrashHubEndpoints
	{
		private static bool EqualChanges(IChange left, IChange right)
		{
			if (left.Id != right.Id)
			{
				return false;
			}

			if (left.Owner != right.Owner)
			{
				return false;
			}

			if (left.Action != right.Action)
			{
				return false;
			}

			if (left.Payload != right.Payload)
			{
				return false;
			}

			if (left.Stamp != right.Stamp)
			{
				return false;
			}

			if (left.Type != right.Type)
			{
				return false;
			}

			return true;
		}

		[TestCaseSource(nameof(ValidAddChanges))]
		public async Task Add_Successful(Change change)
		{
			var currCount = _crashHub.Database.Changes.Count();

			await _crashHub.PushChange(change);
			Assert.That(_crashHub.Database.Changes.Count(), Is.EqualTo(currCount + 1));
			Assert.That(_crashHub.Database.TryGetChange(change.Id, out var changeOut), Is.True);

			Assert.That(change.Id, Is.EqualTo(changeOut.Id));
			Assert.That(change.Owner, Is.EqualTo(changeOut.Owner));
			Assert.That(change.Type, Is.EqualTo(changeOut.Type));
			Assert.That(changeOut.HasFlag(change.Action), Is.True);
			// TODO : Add Payload Comparison
		}

		[TestCaseSource(nameof(InvalidAddChanges))]
		public async Task Add_Failure(Change change)
		{
			var currCount = _crashHub.Database.Changes.Count();

			await _crashHub.PushChange(change);
			Assert.That(_crashHub.Database.Changes.Count(), Is.EqualTo(currCount));
			Assert.That(_crashHub.Database.TryGetChange(change.Id, out var changeOut), Is.False);
			Assert.That(changeOut, Is.Null);
		}
	}
}

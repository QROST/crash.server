﻿// <auto-generated />
using System;

using Crash.Changes;
using Crash.Server.Model;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Crash.Server.Migrations
{
	[DbContext(typeof(CrashContext))]
	partial class CrashContextModelSnapshot : ModelSnapshot
	{

		protected override void BuildModel(ModelBuilder modelBuilder)
		{
			modelBuilder.HasAnnotation("ProductVersion", "6.0.10");

			modelBuilder.Entity("Crash.Changes.Change", b =>
			{
				b.Property<Guid>(nameof(Change.Id))
					.ValueGeneratedOnAdd()
					.HasColumnType("TEXT");

				b.Property<DateTime>(nameof(Change.Stamp))
					.HasColumnType("TEXT");

				b.Property<string>(nameof(Change.Owner))
					.HasColumnType("TEXT");

				b.Property<string>(nameof(Change.Payload))
					.HasColumnType("TEXT");

                b.Property<int>(nameof(Change.Type))
                    .HasColumnType("TEXT");

                b.Property<int>(nameof(Change.Action))
					.HasColumnType("INTEGER");

				b.HasKey(nameof(Change.Id));

				b.ToTable(initial_database.TABLE_NAME);
			});
		}
	}
}

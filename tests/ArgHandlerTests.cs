﻿using System.Collections;


namespace Crash.Server.Tests
{

	[TestFixture]
	public class ArgHandlerTests
	{

		#region defaults

		[Test]
		public void EnsureDefaultURLs()
		{
			ArgumentHandler argHandler = new();
			argHandler.EnsureDefaults();
			Assert.Multiple(() =>
			{
				Assert.That(Uri.TryCreate(argHandler.URL, UriKind.Absolute, out Uri? result), Is.True, "Failed to create the URI");
				Assert.That(result.AbsoluteUri, Does.StartWith(argHandler.URL), "URLs are not similiar enough");
			});
		}

		[Test]
		public void EnsureDefaultDbPath()
		{
			ArgumentHandler argHandler = new();
			argHandler.EnsureDefaults();
			var directory = Path.GetDirectoryName(argHandler.DatabaseFileName);
			Assert.Multiple(() =>
			{
				Assert.That(argHandler.DatabaseFileName, Does.EndWith(".db"), "File does not end with .db");
				Assert.That(Directory.Exists(directory), Is.True, "Directory does not exist");
				Assert.That(argHandler.DatabaseFileName, Has.Length.LessThan(255), "Filename is too long");
			});
		}

		[Test]
		public void EnsureDefaultNewDb()
		{
			ArgumentHandler argHandler = new();
			argHandler.EnsureDefaults();

			Assert.That(argHandler.ResetDB, Is.False);
		}

		[Test]
		public void EnsureHelp()
		{
			ArgumentHandler argHandler = new();
			argHandler.EnsureDefaults();
			argHandler.ParseArgs(new string[] { "--help" });

			Assert.That(argHandler.Exit, Is.True);
		}

		#endregion

		#region Argument Parsing


		#endregion

		#region URLs

		[TestCaseSource(typeof(ArgHandlerData), nameof(ArgHandlerData.Invalid_URLArguments))]
		[TestCaseSource(typeof(ArgHandlerData), nameof(ArgHandlerData.Valid_URLArguments))]
		public bool ParseURLArgs(List<string> args)
		{
			ArgumentHandler argHandler = new();
			argHandler.EnsureDefaults();
			argHandler.ParseArgs(args.ToArray());

			ArgumentHandler defaultArgHandler = new();
			defaultArgHandler.EnsureDefaults();

			return argHandler.URL != defaultArgHandler.URL;
		}

		#endregion

		#region DBPaths

		[TestCaseSource(typeof(ArgHandlerData), nameof(ArgHandlerData.Invalid_DBPathArguments))]
		[TestCaseSource(typeof(ArgHandlerData), nameof(ArgHandlerData.Valid_DBPathArguments))]
		public bool ParseDBArgs(List<string> args)
		{
			ArgumentHandler argHandler = new();
			argHandler.EnsureDefaults();
			argHandler.ParseArgs(args.ToArray());

			ArgumentHandler defaultArgHandler = new();
			defaultArgHandler.EnsureDefaults();

			return argHandler.DatabaseFileName != defaultArgHandler.DatabaseFileName;
		}

		#endregion

		#region ResetDbs

		#endregion

		#region Test Data

		public sealed class ArgHandlerData
		{

			public static IEnumerable Invalid_URLArguments
			{
				get
				{
					yield return new TestCaseData(new List<string> { "--urls", null }).Returns(false);
					yield return new TestCaseData(new List<string> { "--urls", "htp:/ww.com" }).Returns(false);
					yield return new TestCaseData(new List<string> { "--urls", "192.145.1.1" }).Returns(false);
					yield return new TestCaseData(new List<string> { "--urls", "0.1.2" }).Returns(false);
					yield return new TestCaseData(new List<string> { "--urls", "error;;" }).Returns(false);
					yield return new TestCaseData(new List<string> { "urls", GetRandomValidFullURL() }).Returns(false);
					yield return new TestCaseData(new List<string> { "--rls", GetRandomValidFullURL() }).Returns(false);
					yield return new TestCaseData(new List<string> { GetRandomValidFullURL() }).Returns(false);
				}
			}

			public static IEnumerable Valid_URLArguments
			{
				get
				{
					// Trues

					for (int i = 0; i < 10; i++)
					{
						yield return new TestCaseData(new List<string> {
							"--urls", GetRandomValidFullURL(),
						}).Returns(true);
					}

					for (int i = 0; i < 10; i++)
					{
						yield return new TestCaseData(new List<string> {
							"--urls", GetRandomValidFullIpAddress(),
						}).Returns(true);
					}
				}
			}

			public static IEnumerable Valid_DBPathArguments
			{
				get
				{
					for (int i = 0; i < 5; i++)
					{
						yield return new TestCaseData(new List<string> {
							"--path", GetRandomValidDbFileName(),
						}).Returns(true);
					}

					/* Relative paths not supported yet
					yield return new TestCaseData(new List<string> {
							"--path", @"\App_Data\fileName.db",
						}).Returns(true);
					*/

					/* Just Filenames don't work currently
					yield return new TestCaseData(new List<string> {
							"--path", "fileName.db",
						}).Returns(true);
					*/
				}
			}

			public static IEnumerable Invalid_DBPathArguments
			{
				get
				{
					yield return new TestCaseData(new List<string> {
							"--pth", GetRandomValidDbFileName(),
						}).Returns(false);

					yield return new TestCaseData(new List<string> {
							"--pth", GetRandomValidDbFileName(),
						}).Returns(false);

					yield return new TestCaseData(new List<string> {
							GetRandomValidDbFileName(),
						}).Returns(false);

					yield return new TestCaseData(new List<string> {
							"--urls", GetRandomValidDbFileName(),
						}).Returns(false);

					yield return new TestCaseData(new List<string> {
							"--urls", "error", GetRandomValidDbFileName(),
						}).Returns(false);

					yield return new TestCaseData(new List<string> {
							"PATH", GetRandomValidDbFileName(),
						}).Returns(false);
				}
			}

			private static string GetRandomValidDbFileName()
			{
				string fileName = Path.GetFileNameWithoutExtension(Path.GetRandomFileName());
				string fileNameWithDbExt = $"{fileName}.db";
				string path = Directory.GetCurrentDirectory();

				return Path.Combine(path, fileNameWithDbExt);
			}

			static readonly string[] validPrefixes = new string[] { "http://", "https://", "" };

			private static string GetRandomValidFullURL()
			{
				int prefixIndex = TestContext.CurrentContext.Random.Next(0, validPrefixes.Length - 1);

				string prefix = validPrefixes[prefixIndex];
				string url = GetRandomValidURL();
				string port = GetRandomValidPort();

				return $"{prefix}{url}{UrlPortSeparator}{port}";
			}

			private static string GetRandomValidFullIpAddress()
			{
				int prefixIndex = TestContext.CurrentContext.Random.Next(0, validPrefixes.Length - 1);

				string prefix = validPrefixes[prefixIndex];
				string ipAddress = GetRandomValidIpAddress();
				string port = GetRandomValidPort();

				return $"{prefix}{ipAddress}{UrlPortSeparator}{port}";
			}

			const int portMin = 1000;
			const int intPortMax = 9000;
			private static string GetRandomValidPort()
				=> TestContext.CurrentContext.Random.Next(portMin, intPortMax).ToString();

			private static string GetRandomValidURL()
			{
				char[] alphabet = new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
				string[] domainExtensions = new string[] { ".com", ".au", ".ca", ".co.uk", ".org" };
				int urlCount = TestContext.CurrentContext.Random.Next(5, 20);

				string url = string.Empty;
				for (int i = 0; i < urlCount; i++)
				{
					int alphaCharIndex = TestContext.CurrentContext.Random.Next(0, alphabet.Length);
					url += alphabet[alphaCharIndex];
				}

				return url;
			}

			const int maxIPNumber = 255;
			const int ipNumberCount = 4;
			const char separator = '.';
			const char UrlPortSeparator = ':';
			private static string GetRandomValidIpAddress()
			{
				string ipAddress = "";
				for (int i = 0; i < ipNumberCount; i++)
				{
					int ipNum = TestContext.CurrentContext.Random.Next(0, maxIPNumber);
					ipAddress += ipNum.ToString();
					if (i < ipNumberCount - 1)
					{
						ipAddress += separator;
					}
				}

				return ipAddress;
			}
		}

		#endregion

	}
}

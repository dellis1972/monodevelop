﻿//
// SolutionPackageRepositoryPathTests.cs
//
// Author:
//       Matt Ward <matt.ward@xamarin.com>
//
// Copyright (c) 2014 Xamarin Inc. (http://xamarin.com)
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using ICSharpCode.PackageManagement;
using NUnit.Framework;
using MonoDevelop.Core;
using MonoDevelop.PackageManagement.Tests.Helpers;

namespace MonoDevelop.PackageManagement.Tests
{
	[TestFixture]
	public class SolutionPackageRepositoryPathTests
	{
		SolutionPackageRepositoryPath repositoryPath;
		FakeProject project;
		PackageManagementOptions options;
		FakeSolution solution;

		void CreateSolutionPackageRepositoryPath ()
		{
			repositoryPath = new SolutionPackageRepositoryPath (project, options);
		}

		void CreateSolutionPackageRepositoryPath (ISolution solution)
		{
			repositoryPath = new SolutionPackageRepositoryPath (solution, options);
		}

		void CreateTestProject ()
		{
			project = new FakeProject ();
		}

		void CreateSolution (string fileName)
		{
			solution = new FakeSolution (fileName);
		}

		void CreateOptions ()
		{
			options = new TestablePackageManagementOptions ();
		}

		[Test]
		public void PackageRepositoryPath_ProjectAndSolutionHaveDifferentFolders_IsConfiguredPackagesFolderInsideSolutionFolder ()
		{
			CreateOptions ();
			CreateTestProject ();
			CreateSolution (@"d:\projects\MyProject\MySolution.sln");
			solution.BaseDirectory = @"d:\projects\MyProject\";
			project.ParentSolution = solution;
			options.PackagesDirectory = "MyPackages";
			CreateSolutionPackageRepositoryPath ();

			string path = repositoryPath.PackageRepositoryPath;
			string expectedPath = @"d:\projects\MyProject\MyPackages";

			Assert.AreEqual (expectedPath, path);
		}

		[Test]
		public void PackageRepositoryPath_PassSolutionToConstructor_IsConfiguredPackagesFolderInsideSolutionFolder ()
		{
			CreateOptions ();
			CreateSolution (@"d:\projects\MySolution\MySolution.sln");
			options.PackagesDirectory = "Packages";
			CreateSolutionPackageRepositoryPath (solution);

			string path = repositoryPath.PackageRepositoryPath;
			string expectedPath = @"d:\projects\MySolution\Packages";

			Assert.AreEqual (expectedPath, path);
		}

		[Test]
		public void GetInstallPath_GetInstallPathForPackage_ReturnsPackagePathInsideSolutionPackagesRepository ()
		{
			CreateOptions ();
			CreateSolution (@"d:\projects\Test\MySolution\MyProject.sln");
			options.PackagesDirectory = "MyPackages";
			CreateSolutionPackageRepositoryPath (solution);

			var package = FakePackage.CreatePackageWithVersion ("MyPackage", "1.2.1.40");

			string installPath = repositoryPath.GetInstallPath (package);

			string expectedInstallPath = 
				@"d:\projects\Test\MySolution\MyPackages\MyPackage.1.2.1.40";

			Assert.AreEqual (expectedInstallPath, installPath);
		}
	}
}

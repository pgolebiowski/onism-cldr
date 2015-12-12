using System;
using System.IO;
using System.Linq;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Onism.Cldr.Packages;

namespace Onism.Cldr.Test.Packages
{
    [TestFixture]
    public class CldrJsonPathExtractorTest
    {
        #region Helper methods

        private string CreateFile(string directory, string name = null)
        {
            if (name == null)
                name = Path.GetRandomFileName();

            var path = Path.Combine(directory, name + ".json");
            File.WriteAllText(path, "random file");
            return path;
        }

        private string[] CreateFiles(string directory, int count)
        {
            return Enumerable
                .Range(1, count)
                .Select(x => CreateFile(directory))
                .ToArray();
        }

        private void CreateBower(string directory, object mainContent)
        {
            var path = Path.Combine(directory, "bower.json");

            dynamic json = new JObject();
            json.main = JToken.FromObject(mainContent);

            File.WriteAllText(path, json.ToString());
        }

        private string CreateDirectory(string directory)
        {
            var tempDirectory = Path.Combine(directory, Path.GetRandomFileName());
            Directory.CreateDirectory(tempDirectory);
            return tempDirectory;
        }

        private string[] CreateDirectories(string directory, int count)
        {
            return Enumerable
                .Range(1, count)
                .Select(x => CreateDirectory(directory))
                .ToArray();
        }

        private static string MakeRelative(string filePath, string referencePath)
        {
            return filePath
                .Substring(referencePath.Length + 1)
                .Replace('\\', '/');
        }

        private string[] ExtractPaths() => CldrJsonPathExtractor
            .ExtractPaths(_tempFolder)
            .ToArray();

        #endregion

        private string _tempFolder;

        [SetUp]
        public void Init()
        {
            _tempFolder = CreateDirectory(Path.GetTempPath());
        }

        [TearDown]
        public void Dispose()
        {
            Directory.Delete(_tempFolder, true);
        }

        [Test]
        public void ExtractFiles_Explicit_OneFileOnePackage_ShouldWork()
        {
            var oneFile = CreateFile(_tempFolder, "defaultContent");
            CreateBower(_tempFolder, "defaultContent.json");

            var found = ExtractPaths();
            found.Should().HaveCount(1);
            found.First().Should().Be(oneFile);
        }

        [Test]
        public void ExtractFiles_Explicit_ManyFilesOnePackage_ShouldWork()
        {
            var files = CreateFiles(_tempFolder, 3);

            var mainContent = files.Select(x => MakeRelative(x, _tempFolder));
            CreateBower(_tempFolder, mainContent);

            ExtractPaths().Should().BeEquivalentTo(files);
        }

        [Test]
        public void ExtractFiles_Explicit_SubsetOfFiles_ShouldWork()
        {
            var files = CreateFiles(_tempFolder, 5);

            // now create som rubbish files
            CreateFiles(_tempFolder, 20);

            var mainContent = files.Select(x => MakeRelative(x, _tempFolder));
            CreateBower(_tempFolder, mainContent);

            ExtractPaths().Should().BeEquivalentTo(files);
        }

        [Test]
        public void ExtractFiles_Explicit_DeeperInsideStillOnePackage_ShouldWork()
        {
            // 100 files bottom
            var lvl1 = CreateDirectories(_tempFolder, 10);
            var lvl2 = lvl1.SelectMany(x => CreateDirectories(x, 10));
            var files = lvl2.SelectMany(x => CreateFiles(x, 10));
            
            // take half of the files
            var subset = files.Skip(500).ToArray();
            subset.Should().HaveCount(500);

            // check it
            var mainContent = subset.Select(x => MakeRelative(x, _tempFolder));
            CreateBower(_tempFolder, mainContent);

            ExtractPaths().Should().BeEquivalentTo(subset);
        }

        [Test]
        public void ExtractFiles_Wildcard_VariousLevels_ShouldWork()
        {
            // 64 directories bottom
            var lvl1 = CreateDirectories(_tempFolder, 4);
            var lvl2 = lvl1.SelectMany(x => CreateDirectories(x, 4)).ToArray();
            var lvl3 = lvl2.SelectMany(x => CreateDirectories(x, 4)).ToArray();

            // 256 files bottom
            var lvl1Files = lvl1.SelectMany(x => CreateFiles(x, 4)).ToArray();
            var lvl2Files = lvl2.SelectMany(x => CreateFiles(x, 4)).ToArray();
            var lvl3Files = lvl3.SelectMany(x => CreateFiles(x, 4)).ToArray();

            // now select 2-8-32 directories at each level
            var lvl1Subset = lvl1.Skip(2).ToArray();
            var lvl2Subset = lvl2.Skip(8).ToArray();
            var lvl3Subset = lvl3.Skip(32).ToArray();

            // now get all the files from those selected directories
            var files = lvl1Files.Where(x => lvl1Subset.Any(x.StartsWith)).Concat(
                lvl2Files.Where(x => lvl2Subset.Any(x.StartsWith))).Concat(
                    lvl3Files.Where(x => lvl3Subset.Any(x.StartsWith))).ToArray();

            files.Should().HaveCount(8 + 32 + 128);

            // now create wildcard-based paths
            var wildcard = new Func<string, string>(x => Path.Combine(x, "*.json"));
            var wildcardPaths = lvl1Subset.Select(wildcard).Concat(
                lvl2Subset.Select(wildcard)).Concat(
                lvl3Subset.Select(wildcard)).ToArray();

            // check it
            var mainContent = wildcardPaths.Select(x => MakeRelative(x, _tempFolder));
            CreateBower(_tempFolder, mainContent);

            ExtractPaths().Should().BeEquivalentTo(files);
        }

        [Test]
        public void ExtractFiles_Mixed_MultiplePackages_ShouldWork()
        {
            // make a flat stucture of 3 packages hidden there
            var lvl0 = CreateDirectories(_tempFolder, 3);
            lvl0 = lvl0.Select(CreateDirectory).ToArray();
            lvl0 = lvl0.Select(CreateDirectory).ToArray();

            // 729 directories bottom
            var lvl1 = lvl0.SelectMany(x => CreateDirectories(x, 3)).ToArray();
            var lvl2 = lvl1.SelectMany(x => CreateDirectories(x, 3)).ToArray();
            var lvl3 = lvl2.SelectMany(x => CreateDirectories(x, 3)).ToArray();
            var lvl4 = lvl3.SelectMany(x => CreateDirectories(x, 3)).ToArray();

            // single file for each directory - those will be explicit
            var lvl1Files = lvl1.Select(x => CreateFile(x)).ToArray();
            var lvl2Files = lvl2.Select(x => CreateFile(x)).ToArray();
            var lvl3Files = lvl3.Select(x => CreateFile(x)).ToArray();
            var lvl4Files = lvl4.Select(x => CreateFile(x)).ToArray();

            var explicitFiles = lvl1Files
                .Concat(lvl2Files)
                .Concat(lvl3Files)
                .Concat(lvl4Files)
                .ToArray();

            // add rubbish files
            lvl1Files = lvl1Files.Concat(lvl1.SelectMany(x => CreateFiles(x, 2))).ToArray();
            lvl2Files = lvl2Files.Concat(lvl2.SelectMany(x => CreateFiles(x, 2))).ToArray();
            lvl3Files = lvl3Files.Concat(lvl3.SelectMany(x => CreateFiles(x, 2))).ToArray();
            lvl4Files = lvl4Files.Concat(lvl4.SelectMany(x => CreateFiles(x, 2))).ToArray();

            // now some wildcards - notice the overlapping that occurs
            // first select 5-15-40-120 directories at each level
            var lvl1Subset = lvl1.Skip(4).ToArray();
            var lvl2Subset = lvl2.Skip(12).ToArray();
            var lvl3Subset = lvl3.Skip(41).ToArray();
            var lvl4Subset = lvl4.Skip(123).ToArray();

            // now get all the files from those selected directories
            var wildcardFiles = lvl1Files.Where(x => lvl1Subset.Any(x.StartsWith)).Concat(
                lvl2Files.Where(x => lvl2Subset.Any(x.StartsWith))).Concat(
                lvl3Files.Where(x => lvl3Subset.Any(x.StartsWith))).Concat(
                lvl4Files.Where(x => lvl4Subset.Any(x.StartsWith))).ToArray();

            wildcardFiles.Should().HaveCount(15 + 45 + 120 + 360);

            // now create wildcard-based paths
            var wildcard = new Func<string, string>(x => Path.Combine(x, "*.json"));
            var wildcardPaths = lvl1Subset.Select(wildcard).Concat(
                lvl2Subset.Select(wildcard)).Concat(
                lvl3Subset.Select(wildcard)).Concat(
                lvl4Subset.Select(wildcard)).ToArray();

            // merge explicit and wildcard paths
            var substringLength = lvl0.First().Length;
            var allPaths = explicitFiles
                .Concat(wildcardPaths)
                .GroupBy(x => x.Substring(0, substringLength))
                .ToDictionary(g => g.Key, g => g.ToArray());

            allPaths.Keys.Should().HaveCount(3, "because there are 3 packages");

            // create bower files
            foreach (var package in lvl0)
            {
                var mainContent = allPaths[package].Select(x => MakeRelative(x, package));
                CreateBower(package, mainContent);
            }

            // check it
            var allDistinct = wildcardFiles.Concat(explicitFiles).Distinct();
            ExtractPaths().Should().BeEquivalentTo(allDistinct);
        }

        [Test]
        public void ExtractFiles_Wildcard_DirectoryWildcards_ShouldWork()
        {
            // 64 directories bottom
            var lvl1 = CreateDirectories(_tempFolder, 4);
            var lvl2 = lvl1.SelectMany(x => CreateDirectories(x, 4)).ToArray();
            var lvl3 = lvl2.SelectMany(x => CreateDirectories(x, 4)).ToArray();

            // 256 files bottom
            var lvl1Files = lvl1.SelectMany(x => CreateFiles(x, 4)).ToArray();
            var lvl2Files = lvl2.SelectMany(x => CreateFiles(x, 4)).ToArray();
            var lvl3Files = lvl3.SelectMany(x => CreateFiles(x, 4)).ToArray();

            // everything
            var mainContent = new[]
            {
                "**/**/**/*.json",
                "**/**/*.json",
                "**/*.json"
            };
            CreateBower(_tempFolder, mainContent);
            var allFiles = lvl1Files.Concat(lvl2Files).Concat(lvl3Files);
            ExtractPaths().Should().BeEquivalentTo(allFiles);

            // selective
            mainContent = new[]
            {
                "**/**/**/*.json",
                "**/*.json"
            };
            CreateBower(_tempFolder, mainContent);
            allFiles = lvl1Files.Concat(lvl3Files);
            ExtractPaths().Should().BeEquivalentTo(allFiles);
        }
    }
}

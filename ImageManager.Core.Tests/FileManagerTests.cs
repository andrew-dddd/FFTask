using FluentAssertions;
using ImageManager.Core.Models;
using ImageManager.Core.Impl;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

namespace ImageManager.Core.Tests
{
    public class FileManagerTests : IDisposable
    {
        private FileManager _fileManager;

        public FileManagerTests()
        {
            _fileManager = new FileManager();
            Directory.CreateDirectory("./TestGallery");
        }

        [Fact]
        public void FileManager_ShouldCheckIfDirectoryExist()
        {
            // Arrange

            // Act

            // Assert
            _fileManager.DirectoryExists("./TestGallery").Should().BeTrue();
            _fileManager.IsGalleryDirectory("./TestGallery").Should().BeFalse();
        }

        [Fact]
        public void FileManager_ShouldInitializeGallery()
        {
            // Arrange

            // Act
            _fileManager.InitializeGallery("./TestGallery");

            // Assert
            _fileManager.IsGalleryDirectory("./TestGallery").Should().BeTrue();
            var creditsJson = "./TestGallery/__gallery.json";
            File.Exists(creditsJson).Should().BeTrue();
            File.ReadAllText(creditsJson).Should().BeEquivalentTo("{}");
        }

        [Fact]
        public void FileManager_ShouldCopyFile()
        {
            // Arrange
            _fileManager.InitializeGallery("./TestGallery");
            var fileInfo = new GalleryImageInfo("./TestFiles/test.jpg", true, new CreditsEntry
            {
                PhotoUrl = "test url",
                UserName = "test user name",
                UserUrl = "user url",
            });

            // Act
            _fileManager.CopyFileToGallery("./TestGallery", fileInfo);

            // Assert            
            File.Exists("./TestGallery/test.jpg").Should().BeTrue();
        }

        [Fact]
        public void FileManager_ShouldAddFileToCreditsJson_AndReadDirectoryAgain()
        {
            // Arrange
            _fileManager.InitializeGallery("./TestGallery");
            var galleryImagesInfo = new List<GalleryImageInfo>()
            {
                new GalleryImageInfo("./TestFiles/test.jpg", true, new CreditsEntry
                {
                    PhotoUrl = "test url",
                    UserName = "test user name",
                    UserUrl = "user url",
                })
            };

            // Act
            _fileManager.WriteGalleryFile("./TestGallery", galleryImagesInfo);

            // Assert            
            var readFiles = _fileManager.ReadGalleryFiles("./TestGallery");
            readFiles.Should().HaveCount(1);
            readFiles.First().Should().BeEquivalentTo(new KeyValuePair<string, CreditsEntry>("test", new CreditsEntry
            {
                PhotoUrl = "test url",
                UserName = "test user name",
                UserUrl = "user url"
            }));
        }

        /// <summary>
        /// Gallery should be destroyed by removing __gallery.json file, other files should still exist.
        /// </summary>
        [Fact]
        public void FileManager_ShouldDestroyGallery()
        {
            // Arrange
            _fileManager.InitializeGallery("./TestGallery");
            var galleryImagesInfo = new List<GalleryImageInfo>()
            {
                new GalleryImageInfo("./TestFiles/test.jpg", true, new CreditsEntry
                {
                    PhotoUrl = "test url",
                    UserName = "test user name",
                    UserUrl = "user url",
                })
            };

            // Act
            _fileManager.CopyFileToGallery("./TestGallery", galleryImagesInfo.First());
            _fileManager.WriteGalleryFile("./TestGallery", galleryImagesInfo);
            _fileManager.DestroyGallery("./TestGallery");

            // Assert            
            _fileManager.IsGalleryDirectory("./TestGallery").Should().BeFalse();
            File.Exists("./TestGallery/test.jpg").Should().BeTrue();
        }

        [Fact]
        public void FileManager_ShouldRemoveFileAndWriteEmptyCredits()
        {
            // Arrange
            _fileManager.InitializeGallery("./TestGallery");
            var galleryImagesInfo = new List<GalleryImageInfo>()
            {
                new GalleryImageInfo("./TestFiles/test.jpg", true, new CreditsEntry
                {
                    PhotoUrl = "test url",
                    UserName = "test user name",
                    UserUrl = "user url",
                })
            };

            // Act
            _fileManager.CopyFileToGallery("./TestGallery", galleryImagesInfo.First());
            _fileManager.WriteGalleryFile("./TestGallery", galleryImagesInfo);
            _fileManager.RemoveFile("./TestGallery/test.jpg");
            _fileManager.WriteGalleryFile("./TestGallery", new List<GalleryImageInfo>());

            // Assert            
            _fileManager.IsGalleryDirectory("./TestGallery").Should().BeTrue();
            File.Exists("./TestGallery/test.jpg").Should().BeFalse();
            File.ReadAllText("./TestGallery/__gallery.json").Should().Be("{}");
        }

        public void Dispose()
        {
            if (Directory.Exists("./TestGallery"))
            {
                Directory.Delete("./TestGallery", true);
            }
        }
    }
}

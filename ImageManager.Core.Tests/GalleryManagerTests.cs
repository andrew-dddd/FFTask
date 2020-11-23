using FluentAssertions;
using ImageManager.Core.Models;
using ImageManager.Core.Impl;
using NSubstitute;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace ImageManager.Core.Tests
{
    public class GalleryManagerTests
    {
        private readonly GalleryManager _galleryManager;
        private readonly IFileManager _fileManager;

        public GalleryManagerTests()
        {
            _fileManager = Substitute.For<IFileManager>();
            _galleryManager = Substitute.ForPartsOf<GalleryManager>(_fileManager);
            _galleryManager.WhenForAnyArgs(x => x.CheckIfTargetDirExist(default)).DoNotCallBase();
            _galleryManager.WhenForAnyArgs(x => x.CheckIfIsGalleryDirectory(default)).DoNotCallBase();
        }

        [Fact]
        public void WipeGallery_ShouldSucceed()
        {
            // Arrange
            var gallery = new ImageGallery("Test gallery", new List<GalleryImageInfo>
            {
                new GalleryImageInfo("name1", true, new CreditsEntry()),
                new GalleryImageInfo("name2", true, new CreditsEntry()),
            });

            // Act
            _galleryManager.WipeGallery(gallery);

            // Assert
            Received.InOrder(() =>
            {
                _galleryManager.Received(1).CheckIfTargetDirExist("Test gallery");
                _galleryManager.Received(1).CheckIfIsGalleryDirectory("Test gallery");
                _fileManager.Received(1).RemoveFile("name1");
                _fileManager.Received(1).RemoveFile("name2");
                _fileManager.DestroyGallery("Test gallery");
            });
        }

        [Fact]
        public void InitializeGallery_ShouldSucceed()
        {
            // Arrange
            _fileManager.IsGalleryDirectory(Arg.Any<string>()).Returns(false);

            // Act
            _galleryManager.InitializeGallery("test dir");

            // Assert
            Received.InOrder(() =>
            {
                _galleryManager.Received(1).CheckIfTargetDirExist("test dir");
                _fileManager.Received(1).InitializeGallery("test dir");
            });
        }

        [Fact]
        public void InitializeGallery_ShouldFail_DueToAlreadyExistingGallery()
        {
            // Arrange
            _fileManager.IsGalleryDirectory(Arg.Any<string>()).Returns(true);

            // Act
            var ex = Assert.Throws<ImageManagerException>(() => _galleryManager.InitializeGallery("test dir"));

            // Assert
            ex.Message.Should().Be("Already a gallery directory.");
        }

        [Fact]
        public void OpenGallery_ShouldSucceed()
        {
            // Arrange
            _fileManager.DirectoryExists(Arg.Any<string>()).Returns(true);
            _fileManager.IsGalleryDirectory(Arg.Any<string>()).Returns(false);

            _fileManager.ReadGalleryFiles(default).ReturnsForAnyArgs(x =>
            {
                return new Dictionary<string, CreditsEntry>()
                {
                    ["testfile"] = new CreditsEntry
                    {
                        PhotoUrl = "url",
                        UserName = "user name",
                        UserUrl = "user url"
                    }
                };
            });

            // Act
            var imageGallery = _galleryManager.OpenGallery("test dir");

            // Assert
            Received.InOrder(() =>
            {
                _galleryManager.Received(1).CheckIfTargetDirExist("test dir");
                _galleryManager.Received(1).CheckIfIsGalleryDirectory("test dir");
                _fileManager.Received(1).ReadGalleryFiles("test dir");
            });

            imageGallery.FullName.Should().Be("test dir");
            imageGallery.GalleryFiles.Count.Should().Be(1);
            imageGallery.GalleryFiles.First().Should().BeEquivalentTo(new
            {
                Name = "testfile.jpg",
                Exists = false,
                OriginalFileName = "testfile",
                Credits = new
                {
                    PhotoUrl = "url",
                    UserName = "user name",
                    UserUrl = "user url"
                }
            });
        }

        [Fact]
        public void RemoveImage_ShouldSucceed()
        {
            // Arrange
            var gallery = new ImageGallery("test gallery", new List<GalleryImageInfo>
            {
                new GalleryImageInfo("image", true, new CreditsEntry())
            });

            // Act
            _galleryManager.RemoveImage(gallery, 0);

            // Assert
            Received.InOrder(() =>
            {
                _fileManager.RemoveFile("image");
                _fileManager.WriteGalleryFile("test gallery", Arg.Is<List<GalleryImageInfo>>(x => x.Count == 0));
            });
        }

        [Fact]
        public void AddImage_ShouldSucceed()
        {
            // Arrange
            var gallery = new ImageGallery("test gallery", new List<GalleryImageInfo>());
            var newImgInfo = new NewImageInfo
            {
                FullFilePath = "new file.jpg",
                PhotoUrl = "photo url",
                UserName = "user name",
                UserUrl = "user url"
            };

            // Act
            _galleryManager.AddImage(gallery, newImgInfo);

            // Assert
            Received.InOrder(() =>
            {
                _galleryManager.Received(1).CheckIfTargetDirExist("test gallery");
                _galleryManager.Received(1).CheckIfIsGalleryDirectory("test gallery");

                _fileManager.CopyFileToGallery("test gallery", Arg.Is<GalleryImageInfo>(x =>
                    x.Exists == false
                    && x.FullName.EndsWith("new file.jpg")
                    && x.Name == "new file.jpg"
                    && x.OriginalFileName == "new file"
                    && x.Credits.PhotoUrl == "photo url"
                    && x.Credits.UserName == "user name"
                    && x.Credits.UserUrl == "user url"));
                _fileManager.WriteGalleryFile("test gallery", Arg.Is<List<GalleryImageInfo>>(x => x.Count == 1));
            });
        }

        [Fact]
        public void CheckIfTargetDirExist_ShouldThrow_WhenTargetDirDoesNotExist()
        {
            // Arrange
            _galleryManager.WhenForAnyArgs(x => x.CheckIfTargetDirExist(default)).CallBase();
            _fileManager.DirectoryExists(Arg.Any<string>()).Returns(false);

            // Act
            var ex = Assert.Throws<ImageManagerException>(() => _galleryManager.CheckIfTargetDirExist("C:\\test"));
            ex.Message.Should().Be("Directory 'test' does not exist");
        }

        [Fact]
        public void CheckIfIsGalleryDirectory_ShouldThrow_WhenTargetDirDoesNotExist()
        {
            // Arrange
            _galleryManager.WhenForAnyArgs(x => x.CheckIfIsGalleryDirectory(default)).CallBase();
            _fileManager.IsGalleryDirectory(Arg.Any<string>()).Returns(false);

            // Act
            var ex = Assert.Throws<ImageManagerException>(() => _galleryManager.CheckIfIsGalleryDirectory("C:\\test"));
            ex.Message.Should().Be("Directory 'test' is not gallery directory");
        }
    }
}


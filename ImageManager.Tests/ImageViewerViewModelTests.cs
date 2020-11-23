using FluentAssertions;
using ImageManager.Core;
using ImageManager.Core.Models;
using NSubstitute;
using System;
using System.Collections.Generic;
using Xunit;

namespace ImageManager.Tests
{
    public class ImageViewerViewModelTests
    {
        private readonly IGalleryManager _galleryManager;
        private readonly IDirectoryPathProvider _directoryPathProviderMock;
        private readonly IErrorHandler _errorHandlerMock;
        private readonly IConfirmationService _confirmationServiceMock;
        private readonly INewImageDataProvider _newImageDataProvider;
        private readonly ImageViewerViewModel _viewModel;

        public ImageViewerViewModelTests()
        {
            _galleryManager = Substitute.For<IGalleryManager>();
            _directoryPathProviderMock = Substitute.For<IDirectoryPathProvider>();
            _errorHandlerMock = Substitute.For<IErrorHandler>();
            _confirmationServiceMock = Substitute.For<IConfirmationService>();
            _newImageDataProvider = Substitute.For<INewImageDataProvider>();

            _viewModel = Substitute.ForPartsOf<ImageViewerViewModel>(
                _galleryManager,
                _directoryPathProviderMock,
                _errorHandlerMock,
                _confirmationServiceMock,
                _newImageDataProvider);
        }

        [Fact]
        public void OpenGallery_ShouldSucceed()
        {
            // Arrange            
            _galleryManager.OpenGallery(Arg.Any<string>()).Returns(new ImageGallery("Test name", new List<GalleryImageInfo>
            {
                new GalleryImageInfo("fullname\\name.jpg", true, new CreditsEntry
                {
                    PhotoUrl = "url",
                    UserName = "name",
                    UserUrl = "userurl",
                })
            }));

            // Act
            _viewModel.OpenGallery();

            // Assert
            _viewModel.Received(1).LoadGallery(Arg.Any<ImageGallery>());
            _viewModel.CurrentImageIndex.Should().Be(0);
            _viewModel.CurrentImage.Should().BeEquivalentTo(new
            {
                FullName = "fullname\\name.jpg",
                Name = "name.jpg",
                OriginalFileName = "name",
                Exists = true,
            });
            _viewModel.CurrentImageName.Should().Be("name.jpg");
            _viewModel.CurrentCredits.Should().BeEquivalentTo(new
            {
                PhotoUrl = "url",
                UserName = "name",
                UserUrl = "userurl",
            });
        }

        [Fact]
        public void OpenGallery_ShouldShowMessage_ForImageViewerException()
        {
            // Arrange
            _galleryManager.OpenGallery(Arg.Any<string>()).ReturnsForAnyArgs(x => throw new ImageManagerException("test message"));

            // Act
            _viewModel.OpenGallery();

            // Assert
            _errorHandlerMock.Received(1).ShowError(Arg.Is<Exception>(x => x.Message == "test message"));
        }

        [Fact]
        public void OpenDirectory_ShouldShowMessage_ForUnexpectedException()
        {
            // Arrange
            _galleryManager.OpenGallery(Arg.Any<string>()).ReturnsForAnyArgs(x => throw new Exception("test message"));

            // Act
            _viewModel.OpenGallery();

            // Assert
            _errorHandlerMock.Received(1).ShowUnexpectedError(Arg.Is<Exception>(x => x.Message == "test message"));
        }

        [Fact]
        public void InitializeGallery_ShouldSucceed()
        {
            // Arrange
            _directoryPathProviderMock.GetDirectoryPath().Returns("path");
            _viewModel.WhenForAnyArgs(x => x.LoadGallery(default)).DoNotCallBase();

            // Act
            _viewModel.InitializeGallery();

            // Assert
            _galleryManager.Received(1).InitializeGallery("path");
            _galleryManager.Received(1).OpenGallery("path");
            _viewModel.Received(1).LoadGallery(Arg.Any<ImageGallery>());
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void InitializeGallery_ShouldNotSucceed_DueToEmptyPath(string path)
        {
            // Arrange
            _directoryPathProviderMock.GetDirectoryPath().Returns(path);

            // Act
            _viewModel.InitializeGallery();

            // Assert
            _galleryManager.Received(0).InitializeGallery(Arg.Any<string>());
        }

        [Fact]
        public void WipeGallery_ShouldSuceed()
        {
            // Arrange
            _viewModel.GalleryLoaded = true;
            _confirmationServiceMock.Confirm(Arg.Any<string>()).Returns(true);

            // Act
            _viewModel.DestroyGallery();

            // Aassert
            _galleryManager.Received(1).WipeGallery(Arg.Any<ImageGallery>());
            _confirmationServiceMock.Received(1).Confirm("Are you sure you want to wipe gallery? This will remove all images permanently.");

            _viewModel.Should().BeEquivalentTo(new
            {
                GalleryLoaded = false,
                GalleryHasImages = false,
                ImageGallery = (ImageGallery)null,

                CurrentCredits = (CreditsEntry)null,
                CurrentImage = (GalleryImageInfo)null,
                CurrentImageName = (string)null,
                CurrentImageIndex = 0,
            });
        }

        [Fact]
        public void WipeGallery_ShouldNotSucceed_DueToNotLoadedGallery()
        {
            // Arrange
            _viewModel.GalleryLoaded = false;

            // Act
            _viewModel.DestroyGallery();

            // Aassert
            _galleryManager.Received(0).WipeGallery(Arg.Any<ImageGallery>());
            _confirmationServiceMock.Received(0).Confirm(Arg.Any<string>());
            _errorHandlerMock.Received(1).ShowError(Arg.Any<Exception>());
        }

        [Fact]
        public void WipeGallery_ShouldNotSucceed_DueToLackOfConfirmation()
        {
            // Arrange
            _viewModel.GalleryLoaded = true;
            _confirmationServiceMock.Confirm(Arg.Any<string>()).Returns(false);

            // Act
            _viewModel.DestroyGallery();

            // Aassert
            _galleryManager.Received(0).WipeGallery(Arg.Any<ImageGallery>());
            _confirmationServiceMock.Received(1).Confirm("Are you sure you want to wipe gallery? This will remove all images permanently.");
        }

        [Fact]
        public void RemoveImage_ShouldSucceed()
        {
            // Arrange
            _viewModel.GalleryLoaded = true;
            _viewModel.CurrentImageName = "test image";
            _viewModel.ImageGallery = new ImageGallery("test gallery", new List<GalleryImageInfo>());
            _galleryManager.OpenGallery(Arg.Any<string>()).Returns(new ImageGallery("newtestgallery", new List<GalleryImageInfo>
            {
                new GalleryImageInfo("name", true, null)
            }));

            _viewModel.WhenForAnyArgs(x => x.LoadGallery(default)).DoNotCallBase();
            _confirmationServiceMock.Confirm(Arg.Any<string>()).Returns(true);

            // Act
            _viewModel.RemoveImage();

            // Assert
            Received.InOrder(() =>
            {
                _confirmationServiceMock.Received(1).Confirm($"Are you sure you want to remove 'test image'? This operation is permanent.");
                _galleryManager.Received(1).RemoveImage(Arg.Any<ImageGallery>(), Arg.Any<int>());
                _galleryManager.Received(1).OpenGallery(Arg.Any<string>());
                _viewModel.Received(1).LoadGallery(Arg.Any<ImageGallery>());
            });
        }

        [Fact]
        public void RemoveImage_ShouldNotSucceed_DueToLackOfConfirmation()
        {
            // Arrange
            _viewModel.GalleryLoaded = true;
            _confirmationServiceMock.Confirm(Arg.Any<string>()).Returns(false);

            // Act
            _viewModel.RemoveImage();

            // Assert
            _galleryManager.Received(0).RemoveImage(Arg.Any<ImageGallery>(), Arg.Any<int>());
        }

        [Fact]
        public void RemoveImage_ShouldNotSucceed_DueToGalleryNotLoaded()
        {
            // Arrange
            _viewModel.GalleryLoaded = false;

            // Act
            _viewModel.RemoveImage();

            // Assert
            _galleryManager.Received(0).RemoveImage(Arg.Any<ImageGallery>(), Arg.Any<int>());
            _confirmationServiceMock.Received(0).Confirm(Arg.Any<string>());
            _errorHandlerMock.Received(1).ShowError(Arg.Any<Exception>());
        }

        [Fact]
        public void AddImage_ShouldSucceed()
        {
            // Arrange
            _viewModel.GalleryLoaded = true;
            _viewModel.ImageGallery = new ImageGallery();
            var imgInfo = new NewImageInfo();
            _newImageDataProvider.GetImageInfo().Returns(imgInfo);
            _viewModel.WhenForAnyArgs(x => x.LoadGallery(default)).DoNotCallBase();

            // Act
            _viewModel.AddImage();

            // Assert
            Received.InOrder(() =>
            {
                _newImageDataProvider.Received(1).GetImageInfo();
                _galleryManager.Received(1).AddImage(Arg.Any<ImageGallery>(), Arg.Is<NewImageInfo>(x => ReferenceEquals(x, imgInfo)));
                _galleryManager.Received(1).OpenGallery(Arg.Any<string>());
                _viewModel.Received(1).LoadGallery(Arg.Any<ImageGallery>());
            });
        }

        [Fact]
        public void AddImage_ShouldNotSucceed_DueToGalleryNotLoaded()
        {
            // Arrange
            _viewModel.GalleryLoaded = false;

            // Act
            _viewModel.AddImage();

            // Assert
            _newImageDataProvider.Received(0).GetImageInfo();
            _galleryManager.Received(0).AddImage(Arg.Any<ImageGallery>(), Arg.Any<NewImageInfo>());
            _errorHandlerMock.Received(1).ShowError(Arg.Any<Exception>());
        }

        [Fact]
        public void AddImage_ShouldNotSucceed_DueToEmptyResultModel()
        {
            // Arrange
            _viewModel.GalleryLoaded = true;
            _newImageDataProvider.GetImageInfo().Returns((NewImageInfo)null);

            // Act
            _viewModel.AddImage();

            // Assert
            _newImageDataProvider.Received(1).GetImageInfo();
            _galleryManager.Received(0).AddImage(Arg.Any<ImageGallery>(), Arg.Any<NewImageInfo>());
        }
    }
}

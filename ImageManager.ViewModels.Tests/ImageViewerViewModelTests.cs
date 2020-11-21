using FluentAssertions;
using ImageManager.ViewModels.Models;
using ImageManager.ViewModels.Services;
using NSubstitute;
using System;
using System.Collections.Generic;
using Xunit;

namespace ImageManager.ViewModels.Tests
{
    public class ImageViewerViewModelTests
    {
        private readonly IGalleryManager _galleryManager;
        private readonly IDirectoryPathProvider _directoryPathProviderMock;
        private readonly IErrorHandler _errorHandlerMock;
        private readonly IConfirmationService _confirmationServiceMock;
        private readonly INewImageDataProvider _newImageDateProvider;
        private readonly ImageViewerViewModel _viewModel;

        public ImageViewerViewModelTests()
        {
            _galleryManager = Substitute.For<IGalleryManager>();
            _directoryPathProviderMock = Substitute.For<IDirectoryPathProvider>();
            _errorHandlerMock = Substitute.For<IErrorHandler>();
            _confirmationServiceMock = Substitute.For<IConfirmationService>();
            _newImageDateProvider = Substitute.For<INewImageDataProvider>();

            _viewModel = new ImageViewerViewModel(
                _galleryManager,
                _directoryPathProviderMock,
                _errorHandlerMock,
                _confirmationServiceMock,
                _newImageDateProvider);
        }

        [Fact]
        public void OpenDirectory_ShouldSucceed()
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
        public void OpenDirectory_ShouldShowMessage_ForImageViewerException()
        {
            // Arrange
            _galleryManager.OpenGallery(Arg.Any<string>()).ReturnsForAnyArgs(x => throw new ImageManagerException("test message"));

            // Act
            _viewModel.OpenGallery();

            // Assert
            _errorHandlerMock.Received(1).ShowError("test message");
        }

        [Fact]
        public void OpenDirectory_ShouldShowMessage_ForUnexpectedException()
        {
            // Arrange
            _galleryManager.OpenGallery(Arg.Any<string>()).ReturnsForAnyArgs(x => throw new Exception("test message"));

            // Act
            _viewModel.OpenGallery();

            // Assert
            _errorHandlerMock.Received(1).ShowUnexpectedError("test message");
        }

        [Fact]
        public void InitializeGallery_ShouldSucceed()
        {
            // Arrange
            _directoryPathProviderMock.GetDirectoryPath().Returns("path");

            // Act
            _viewModel.InitializeGallery();

            // Assert
            _galleryManager.Received(1).InitializeGallery("path");
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
            _viewModel.WipeGallery();

            // Aassert
            _galleryManager.Received(1).WipeGallery(Arg.Any<ImageGallery>());
            _confirmationServiceMock.Received(1).Confirm("Are you sure you want to wipe gallery? This will remove all images permanently.");
        }

        [Fact]
        public void WipeGallery_ShouldNotSucceed_DueToNotLoadedGallery()
        {
            // Arrange
            _viewModel.GalleryLoaded = false;            

            // Act
            _viewModel.WipeGallery();

            // Aassert
            _galleryManager.Received(0).WipeGallery(Arg.Any<ImageGallery>());
            _confirmationServiceMock.Received(0).Confirm(Arg.Any<string>());
        }

        [Fact]
        public void WipeGallery_ShouldNotSucceed_DueToLackOfConfirmation()
        {
            // Arrange
            _viewModel.GalleryLoaded = true;
            _confirmationServiceMock.Confirm(Arg.Any<string>()).Returns(false);

            // Act
            _viewModel.WipeGallery();

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
            _confirmationServiceMock.Confirm(Arg.Any<string>()).Returns(true);

            // Act
            _viewModel.RemoveImage();

            // Assert
            _galleryManager.Received(1).RemoveFile(Arg.Any<ImageGallery>(), Arg.Any<int>());
            _confirmationServiceMock.Received(1).Confirm($"Are you sure you want to remove 'test image'? This operation is permanent.");
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
            _galleryManager.Received(1).RemoveFile(Arg.Any<ImageGallery>(), Arg.Any<int>());
        }
    }
}

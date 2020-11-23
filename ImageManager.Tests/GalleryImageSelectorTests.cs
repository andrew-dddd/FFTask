using FluentAssertions;
using ImageManager.Core;
using ImageManager.Core.Models;
using NSubstitute;
using System.Collections.Generic;
using Xunit;

namespace ImageManager.Tests
{
    public class GalleryImageSelectorTests
    {
        private readonly IGalleryManager _galleryManager;
        private readonly IDirectoryPathProvider _directoryPathProviderMock;
        private readonly IErrorHandler _errorHandlerMock;
        private readonly IConfirmationService _confirmationServiceMock;
        private readonly INewImageDataProvider _newImageDataProvider;
        private readonly ImageViewerViewModel _viewModel;
        private readonly ImageGallery _testGallery;

        public GalleryImageSelectorTests()
        {
            _galleryManager = Substitute.For<IGalleryManager>();
            _directoryPathProviderMock = Substitute.For<IDirectoryPathProvider>();
            _errorHandlerMock = Substitute.For<IErrorHandler>();
            _confirmationServiceMock = Substitute.For<IConfirmationService>();
            _newImageDataProvider = Substitute.For<INewImageDataProvider>();

            _viewModel = new ImageViewerViewModel(
                _galleryManager,
                _directoryPathProviderMock,
                _errorHandlerMock,
                _confirmationServiceMock,
                _newImageDataProvider);

            _testGallery = new ImageGallery("Test name", new List<GalleryImageInfo>
            {
                new GalleryImageInfo("fullname\\name.jpg", true, new CreditsEntry
                {
                    PhotoUrl = "url",
                    UserName = "username",
                    UserUrl = "userurl",
                }),
                new GalleryImageInfo("fullname\\name1.jpg", true, new CreditsEntry
                {
                    PhotoUrl = "url1",
                    UserName = "username1",
                    UserUrl = "userurl1",
                }),
                new GalleryImageInfo("fullname\\name2.jpg", true, new CreditsEntry
                {
                    PhotoUrl = "url2",
                    UserName = "username2",
                    UserUrl = "userurl2",
                }),
            });

            _galleryManager.OpenGallery(Arg.Any<string>()).Returns(_testGallery);
            _viewModel.OpenGallery();
        }

        [Fact]
        public void ImageSelector_ShouldSelectLastImage()
        {
            // Arrange

            // Act 
            _viewModel.LastImage();

            // Assert
            _viewModel.CurrentImageIndex.Should().Be(2);
            _viewModel.CurrentImageName.Should().Be("name2.jpg");
            _viewModel.CurrentImage.Should().BeSameAs(_testGallery.GalleryFiles[2]);
            _viewModel.CurrentCredits.Should().BeSameAs(_testGallery.GalleryFiles[2].Credits);
        }

        [Fact]
        public void ImageSelector_ShouldSelect_PreviousImage()
        {
            // Arrange
            _viewModel.LastImage();

            // Act 
            _viewModel.PreviousImage();

            // Assert
            _viewModel.CurrentImageIndex.Should().Be(1);
            _viewModel.CurrentImageName.Should().Be("name1.jpg");
            _viewModel.CurrentImage.Should().BeSameAs(_testGallery.GalleryFiles[1]);
            _viewModel.CurrentCredits.Should().BeSameAs(_testGallery.GalleryFiles[1].Credits);
        }

        [Fact]
        public void ImageSelector_ShouldSelect_FirstImage()
        {
            // Arrange
            _viewModel.LastImage();

            // Act 
            _viewModel.FirstImage();

            // Assert
            _viewModel.CurrentImageIndex.Should().Be(0);
            _viewModel.CurrentImageName.Should().Be("name.jpg");
            _viewModel.CurrentImage.Should().BeSameAs(_testGallery.GalleryFiles[0]);
            _viewModel.CurrentCredits.Should().BeSameAs(_testGallery.GalleryFiles[0].Credits);
        }

        [Fact]
        public void ImageSelector_ShouldSelect_NextImage()
        {
            // Arrange

            // Act
            _viewModel.NextImage();

            // Assert
            _viewModel.CurrentImageIndex.Should().Be(1);
            _viewModel.CurrentImageName.Should().Be("name1.jpg");
            _viewModel.CurrentImage.Should().BeSameAs(_testGallery.GalleryFiles[1]);
            _viewModel.CurrentCredits.Should().BeSameAs(_testGallery.GalleryFiles[1].Credits);
        }

        [Fact]
        public void ImageSelector_ShouldGoToFirstImage_WhenLastImageSelected()
        {
            // Arrange
            _viewModel.LastImage();

            // Act
            _viewModel.NextImage();

            // Assert 
            _viewModel.CurrentImageIndex.Should().Be(0);
            _viewModel.CurrentImageName.Should().Be("name.jpg");
            _viewModel.CurrentImage.Should().BeSameAs(_testGallery.GalleryFiles[0]);
            _viewModel.CurrentCredits.Should().BeSameAs(_testGallery.GalleryFiles[0].Credits);
        }

        [Fact]
        public void ImageSelector_ShouldGoToLastImage_WhenPreviousImageSelected()
        {
            // Arrange
            _viewModel.FirstImage();

            // Act 
            _viewModel.PreviousImage();

            // Assert
            _viewModel.CurrentImageIndex.Should().Be(2);
            _viewModel.CurrentImageName.Should().Be("name2.jpg");
            _viewModel.CurrentImage.Should().BeSameAs(_testGallery.GalleryFiles[2]);
            _viewModel.CurrentCredits.Should().BeSameAs(_testGallery.GalleryFiles[2].Credits);
        }
    }
}

using FluentAssertions;
using ImageManager.Core;
using ImageManager.Core.Models;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using System;
using System.IO;
using System.Windows.Media.Imaging;
using Xunit;

namespace ImageManager.Tests
{
    public class IntegrationTests
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IDirectoryPathProvider _directoryPathProviderMock;
        private readonly INewImageDataProvider _newImageDataProviderMock;
        private readonly IErrorHandler _errorHandlerMock;
        private readonly IConfirmationService _confirmationServiceMock;

        public IntegrationTests()
        {
            _directoryPathProviderMock = Substitute.For<IDirectoryPathProvider>();
            _newImageDataProviderMock = Substitute.For<INewImageDataProvider>();            
            _confirmationServiceMock = Substitute.For<IConfirmationService>();

            _errorHandlerMock = Substitute.For<IErrorHandler>();
            _errorHandlerMock.WhenForAnyArgs(x => x.ShowError(default)).Do(x => throw x.ArgAt<Exception>(0));
            _errorHandlerMock.WhenForAnyArgs(x => x.ShowUnexpectedError(default)).Do(x => throw x.ArgAt<Exception>(0));

            ServiceCollection serviceCollection = new ServiceCollection();
            serviceCollection.AddImageManagerCommons();
            serviceCollection.AddImageManagerCore();
            serviceCollection.AddScoped(x => _directoryPathProviderMock);
            serviceCollection.AddScoped(x => _newImageDataProviderMock);
            serviceCollection.AddScoped(x => _errorHandlerMock);
            serviceCollection.AddScoped(x => _confirmationServiceMock);

            _serviceProvider = serviceCollection.BuildServiceProvider();
            if (Directory.Exists(".\\GalleryToBeInitiated"))
            {
                Directory.Delete(".\\GalleryToBeInitiated", true);
            }

            Directory.CreateDirectory(".\\GalleryToBeInitiated");
        }

        /// <summary>
        /// Integration test for opening gallery and viewing images.
        /// Test gallery is hidden under .\\TestGallery folder - see solution explorer.
        /// 
        /// Emulated user interaction:
        /// - Click open gallery
        /// - Click 'Last' button
        /// - Click 'Next' button
        /// - Click 'Previous' button
        /// - Click 'First' button
        /// 
        /// Expected result: 
        /// Gallery is opened, and images are switched as ordered in __gallery.json file.
        /// </summary>
        [UIFact]
        public void Gallery_ShouldBeOpened_IntegrationTest()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                // Arrange
                var mainWindow = scope.ServiceProvider.GetRequiredService<MainWindow>();
                _directoryPathProviderMock.GetDirectoryPath().Returns(".\\TestGallery");

                // Act
                mainWindow.OpenImage_Button_Click(null, null);

                // Assert
                _directoryPathProviderMock.Received(1).GetDirectoryPath();
                mainWindow.ImageControl.Source.Should().BeOfType<BitmapImage>().And.NotBeNull();
                CheckCurrentImageName(mainWindow, "zWZosB8_pZY.jpg");

                // Act - Manipulate images
                mainWindow.Last_Click(null, null);
                CheckCurrentImageName(mainWindow, "KrhdmH4xV3Y.jpg");

                mainWindow.Next_Click(null, null);
                CheckCurrentImageName(mainWindow, "zWZosB8_pZY.jpg");

                mainWindow.Previous_Click(null, null);
                CheckCurrentImageName(mainWindow, "KrhdmH4xV3Y.jpg");

                mainWindow.First_Click(null, null);
                CheckCurrentImageName(mainWindow, "zWZosB8_pZY.jpg");

                mainWindow.Close();
            }
        }

        /// <summary>
        /// Integration test for creating new gallery.
        /// 
        /// Emulated user interaction:
        /// - Click Initialize galler button
        /// - Click to add image
        /// - Click to add another image
        /// 
        /// Expected result:
        /// Complete gallery with 2 images and __gallery.json.
        /// </summary>
        [UIFact]
        public void Gallery_ShouldBeInitialized_AndImagesShouldBeAdded()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                // Arrange
                var mainWindow = scope.ServiceProvider.GetRequiredService<MainWindow>();
                _directoryPathProviderMock.GetDirectoryPath().Returns(".\\GalleryToBeInitiated");
                _newImageDataProviderMock.GetImageInfo()
                    .Returns(
                    new NewImageInfo
                    {
                        FullFilePath = ".\\TestImagesToAdd\\_G0OrcPOpJw.jpg",
                        PhotoUrl = "some url 1",
                        UserName = "some name 1",
                        UserUrl = "some name user url 1"
                    },
                    new NewImageInfo
                    {
                        FullFilePath = ".\\TestImagesToAdd\\1wSBBR0rz4g.jpg",
                        PhotoUrl = "some url 2",
                        UserName = "some name 2",
                        UserUrl = "some name user url 2"
                    });

                // Act
                mainWindow.CreateGallery_Button_Click(null, null);
                mainWindow.AddImage_Button_Click(null, null);
                mainWindow.AddImage_Button_Click(null, null);

                // Assert
                mainWindow.ViewModel.GalleryHasImages.Should().BeTrue();
                mainWindow.ViewModel.ImageGallery.Count.Should().Be(2);
                CheckCurrentImageName(mainWindow, "_G0OrcPOpJw.jpg");

                mainWindow.Close();
            }
        }

        [UIFact]
        public void Gallery_ShouldRemoveImage_ThenShouldBeDestroyed()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                // Initialize gallery first
                var mainWindow = scope.ServiceProvider.GetRequiredService<MainWindow>();
                _directoryPathProviderMock.GetDirectoryPath().Returns(".\\GalleryToBeInitiated");
                _confirmationServiceMock.Confirm(default).ReturnsForAnyArgs(true);
                _newImageDataProviderMock.GetImageInfo()
                    .Returns(
                    new NewImageInfo
                    {
                        FullFilePath = ".\\TestImagesToAdd\\_G0OrcPOpJw.jpg",
                        PhotoUrl = "some url 1",
                        UserName = "some name 1",
                        UserUrl = "some name user url 1"
                    },
                    new NewImageInfo
                    {
                        FullFilePath = ".\\TestImagesToAdd\\1wSBBR0rz4g.jpg",
                        PhotoUrl = "some url 2",
                        UserName = "some name 2",
                        UserUrl = "some name user url 2"
                    });

                mainWindow.CreateGallery_Button_Click(null, null);
                mainWindow.AddImage_Button_Click(null, null);
                mainWindow.AddImage_Button_Click(null, null);

                // Act - remove image
                mainWindow.RemoveImage_Button_Click(null, null);
                mainWindow.RemoveImage_Button_Click(null, null);

                mainWindow.ViewModel.ImageGallery.Count.Should().Be(0);

                // Act destroy gallery
                mainWindow.DestroyGallery_Button_Click(null, null);
                mainWindow.ViewModel.ImageGallery.Should().Be(null);

                mainWindow.Close();
            }
        }

        private static void CheckCurrentImageName(MainWindow mainWindow, string imageName) =>
            (mainWindow.ImageControl.Source as BitmapImage).UriSource.AbsoluteUri.EndsWith(imageName).Should().BeTrue();
    }
}

﻿using FluentValidation;
using Humanizer.Localisation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NextSolution.Core.Entities;
using NextSolution.Core.Exceptions;
using NextSolution.Core.Extensions.FileStorage;
using NextSolution.Core.Models.Medias;
using NextSolution.Core.Repositories;
using NextSolution.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace NextSolution.Core.Services
{
    public class MediaService : IMediaService
    {
        private readonly ILogger<MediaService> _logger;
        private readonly IOptions<MediaServiceOptions> _mediaServiceOptions;
        private readonly IMediaRepository _mediaRepository;
        private readonly IFileStorage _fileStorage;
        private readonly IServiceProvider _validatorProvider;

        public MediaService(ILogger<MediaService> logger, IOptions<MediaServiceOptions> mediaServiceOptions, IMediaRepository mediaRepository, IFileStorage fileStorage, IServiceProvider validatorProvider)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mediaServiceOptions = mediaServiceOptions ?? throw new ArgumentNullException(nameof(mediaServiceOptions));
            _mediaRepository = mediaRepository ?? throw new ArgumentNullException(nameof(mediaRepository));
            _fileStorage = fileStorage ?? throw new ArgumentNullException(nameof(fileStorage));
            _validatorProvider = validatorProvider ?? throw new ArgumentNullException(nameof(validatorProvider));
        }

        public async Task DeleteAsync(DeleteMediaForm form)
        {
            if (form == null) throw new ArgumentNullException(nameof(form));

            var formValidator = _validatorProvider.GetRequiredService<DeleteMediaFormValidator>();
            var formValidationResult = await formValidator.ValidateAsync(form);

            if (!formValidationResult.IsValid)
                throw new BadRequestException(formValidationResult.ToDictionary());

            var media = await _mediaRepository.GetByIdAsync(form.Id);
            if (media == null) return;

            await _mediaRepository.DeleteAsync(media);
            _fileStorage.DeleteAsync(media.Path)
                        .Forget(error => _logger.LogWarning(error, $"Unable to delete '{media.Name}' file."));
        }

        private readonly CancellationToken cancellationToken = default;
        private bool disposed = false;

        public void Dispose()
        {
            if (!disposed)
            {
                disposed = true;
                Dispose(true);
                GC.SuppressFinalize(this);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // myResource.Dispose();
                cancellationToken.ThrowIfCancellationRequested();
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (!disposed)
            {
                disposed = true;
                await DisposeAsync(true);
                GC.SuppressFinalize(this);
            }
        }

        protected ValueTask DisposeAsync(bool disposing)
        {
            if (disposing)
            {
                //  await myResource.DisposeAsync();
                cancellationToken.ThrowIfCancellationRequested();
            }

            return ValueTask.CompletedTask;
        }
    }

    public interface IMediaService : IDisposable, IAsyncDisposable
    {
        Task DeleteAsync(DeleteMediaForm form);
    }


    public class MediaServiceOptions
    {
        public IEnumerable<MediaTypeInfo> Documents { get; set; } = new List<MediaTypeInfo>();

        public IEnumerable<MediaTypeInfo> Images { get; set; } = new List<MediaTypeInfo>();

        public IEnumerable<MediaTypeInfo> Videos { get; set; } = new List<MediaTypeInfo>();

        public IEnumerable<MediaTypeInfo> Audios { get; set; } = new List<MediaTypeInfo>();

        public IEnumerable<MediaTypeInfo> All => new[] { Documents, Images, Videos, Audios }.SelectMany(_ => _).ToArray();

        public bool HasMediaTypeInfo(string fileName, MediaType? mediaType = null)
        {
            var fileExtension = Path.GetExtension(fileName);
            var mediaTypeInfos = mediaType switch
            {
                MediaType.Document => Documents,
                MediaType.Image => Images,
                MediaType.Video => Videos,
                MediaType.Audio => Audios,
                _ => All,
            };
            var result =  mediaTypeInfos.Any(_ => _.FileExtension.Equals(fileExtension, StringComparison.OrdinalIgnoreCase));
            return result;
        }

        public MediaTypeInfo? GetMediaTypeInfo(string fileName, MediaType? mediaType = null)
        {
            var fileExtension = Path.GetExtension(fileName);
            var mediaTypeInfos = mediaType switch
            {
                MediaType.Document => Documents,
                MediaType.Image => Images,
                MediaType.Video => Videos,
                MediaType.Audio => Audios,
                _ => All,
            };
            return mediaTypeInfos.FirstOrDefault(_ => _.FileExtension.Equals(fileExtension, StringComparison.OrdinalIgnoreCase));
        }

        public MediaType GetMediaType(string fileName, MediaType? mediaType = null)
        {
            MediaTypeInfo? mediaTypeInfo = GetMediaTypeInfo(fileName, mediaType);
            return mediaTypeInfo?.MediaType ?? MediaType.Unknown;
        }

        public string GetContentType(string fileName, MediaType? mediaType = null)
        {
            MediaTypeInfo? mediaTypeInfo = GetMediaTypeInfo(fileName, mediaType);
            return mediaTypeInfo?.ContentType ?? MimeTypes.FallbackMimeType;
        }
    }

    public class MediaTypeInfo
    {
        public string ContentType { get; set; } = default!;

        public MediaType MediaType { get; set; }

        public long FileSize { get; set; }

        public string FileExtension { get; set; } = default!;
    }
}
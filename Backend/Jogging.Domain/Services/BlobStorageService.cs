﻿using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jogging.Domain.Services
{
    public class BlobStorageService
    {
        private readonly BlobServiceClient _blobServiceClient;

        public BlobStorageService(BlobServiceClient blobServiceClient)
        {
            _blobServiceClient = blobServiceClient;
        }

        public async Task Upload(IFormFile file)
        {
            var container = _blobServiceClient.GetBlobContainerClient("clubs");

            var uniqueName = $"{Guid.NewGuid()}_{file.FileName}";
            var blobClient = container.GetBlobClient(uniqueName);

            using var stream = file.OpenReadStream();
            await blobClient.UploadAsync(stream);
        }
    }
}

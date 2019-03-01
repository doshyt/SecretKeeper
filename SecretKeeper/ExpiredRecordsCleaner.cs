using SecretKeeper.Models;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SecretKeeper.Engine;

namespace SecretKeeper
{
    internal class ExpiredRecordsCleaner : IHostedService, IDisposable
    {
        private Timer _timer;
        private readonly SecretContext _secretContext;
        private readonly UploadContext _uploadContext;

        public ExpiredRecordsCleaner(SecretContext secretContext, UploadContext uploadContext)
        {
            _secretContext = secretContext;
            _uploadContext = uploadContext;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(Cleanup, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));

            return Task.CompletedTask;
        }

        private void Cleanup(object state)
        {

            foreach (var item in _secretContext.SecretItems)
            {
                if (item.ExpiredBy < DateTime.Now)
                {
                    _secretContext.SecretItems.Remove(item);
                    _secretContext.SaveChanges();
                }
            }

            foreach (var upload in _uploadContext.UploadItems)
            {
                if (upload.ExpiredBy < DateTime.Now)
                {

                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Uploads", upload.Token);
                    _uploadContext.UploadItems.Remove(upload);
                    _uploadContext.SaveChanges();
                    FileOperator.DeleteUploadedFile(path);
                }
            }

            // Clean-up leftover uploads that have no records in the DB
            var allUploadedFiles = Directory.GetFiles(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Uploads"));
            foreach (var uploadedFile in allUploadedFiles)
            {
                if (Path.GetFileName(uploadedFile) == ".placeholder.txt") continue;

                if (!_uploadContext.UploadItems.Any(b => b.Token == Path.GetFileName(uploadedFile)))
                {
                    FileOperator.DeleteUploadedFile(uploadedFile);
                }
            }
        }
    
        public Task StopAsync(CancellationToken cancellationToken)
        {

            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }

    }
}

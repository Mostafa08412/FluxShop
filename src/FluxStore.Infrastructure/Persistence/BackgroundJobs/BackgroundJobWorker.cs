using FluxStore.Application.Auth.ForgetPassword;
using FluxStore.Application.Common.Interfaces;
using Hangfire;

namespace FluxStore.Infrastructure.Persistence.BackgroundJobs
{

    internal class BackgroundJobWorker : IBackgroundJobWorker
    {
        private readonly BackgroundJobBridge _backgroundJobBridge;

        public BackgroundJobWorker(BackgroundJobBridge backgroundJobBridge)
        {
            _backgroundJobBridge = backgroundJobBridge;
        }

        public void SendForgetPasswordEmail(SendForgetPasswordEmail request)
        {
            BackgroundJob.Enqueue<BackgroundJobBridge>(bridge =>
                bridge.SendRequestAsync(request));
        }
    }
}

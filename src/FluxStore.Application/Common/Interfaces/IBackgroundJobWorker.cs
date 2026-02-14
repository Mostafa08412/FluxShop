

using FluxStore.Application.Auth.ForgetPassword;

namespace FluxStore.Application.Common.Interfaces
{
    public interface IBackgroundJobWorker
    {
        void SendForgetPasswordEmail(SendForgetPasswordEmail request);
    }
}

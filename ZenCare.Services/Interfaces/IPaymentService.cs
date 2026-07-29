using ZenCare.Model.Requests;
using ZenCare.Model.Responses;
using ZenCare.Model.SearchObjects;

namespace ZenCare.Services.Interfaces
{
    public interface IPaymentService : ICRUDService<PaymentResponse, PaymentInsertRequest, PaymentUpdateRequest, PaymentSearchObject>
    {
        Task<PaymentIntentResponse> CreatePaymentIntentAsync(int purchaseId, int userId);

        Task<PaymentConfirmResponse> ConfirmPaymentAsync(int purchaseId, int userId);

        Task<PaymentRefundResponse> RefundPaymentAsync(int purchaseId, int userId);
    }
}

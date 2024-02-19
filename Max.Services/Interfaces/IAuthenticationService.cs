
using System.Collections.Generic;
using System.Threading.Tasks;
using Max.Data.DataModel;
using Max.Core.Models;

namespace Max.Services.Interfaces
{
    public interface IAuthenticationService
    {
        Task<AutorizationResponseModel> Authenticate(AuthRequestModel authModel);
        Task<AutorizationResponseModel> AuthenticateEntity(AuthRequestModel authModel);
        Task<ResponseStatusModel> SendMultiFactorToken(MultiFactorRequestModel model);
        Task<ResponseStatusModel> ValidateMultiFactorToken(MultiFactorRequestModel model);
        Task<AuthResponseModel> SendAuthorizationData(int userId, string tenantName);
        Task<AuthResponseModel> SendEntityAuthorizationData(int userId, string tenantName, string ipAddress);
        Task<Accesstoken> AddAccessToken(AccessTokenModel model);
        Staffuser GetStaffUserById(int id);
        Entity GetEntityById(int id);
        Task<TokenResponseModel> RefreshToken(TokenRequestModel model);
        Task<bool> RevokeToken(TokenRequestModel model);
        Task<bool> ValidateToken(TokenRequestModel model);
        Task<SelfPaymentResponseModel> ValidatePaymentUrl(PaymentRequestModel model);
        Task<SelfPaymentReceiptResponseModel> ValidatePaymentReceiptRequest(SelfPaymentReceiptModel model);
        Task<MemberLoginResponseModel> AuthenticateMember(MemberLoginRequestModel memberLoginRequestModel);
        Task<AuthResponseModel> SendAuthorizationDataForMemberPortal(int entityId, string tenantName, string IpAddress);
        Task<TokenResponseModel> RefreshMemberToken(TokenRequestModel model);
        
    }
}
using Duende.IdentityServer.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Identity.Api.Pages.Auth
{
    public class ErrorModel(IIdentityServerInteractionService interactionService) : PageModel
    {
        public string Error { get; private set; } = "unknown_error";
        public string? ErrorDescription { get; private set; }
        public string? RequestId { get; private set; }

        public async Task OnGetAsync(string? errorId, CancellationToken ct)
        {
            var context = await interactionService.GetErrorContextAsync(errorId, ct);

            if (context is null)
                return;

            Error = context.Error;
            ErrorDescription = context.ErrorDescription;
            RequestId = context.RequestId;
        }
    }
}
using Duende.IdentityServer;
using Duende.IdentityServer.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Security.Claims;

namespace Identity.Api.Pages.Auth;

[Authorize(AuthenticationSchemes = IdentityServerConstants.DefaultCookieAuthenticationScheme)]
public sealed class ChangePasswordModel(IUserRepository userRepository,
    IUnitOfWork unitOfWork,
    IPasswordHasher passwordHasher,
    IIdentityServerInteractionService interactionService)
    : PageModel
{
    [BindProperty]
    public InputModel Input { get; set; } = new();

    public string? ErrorMessage { get; private set; }

    public IActionResult OnGet(string? returnUrl)
    {
        if (string.IsNullOrWhiteSpace(returnUrl))
        {
            ErrorMessage = "درخواست تغییر رمز معتبر نیست.";
            return Page();
        }

        if (!interactionService.IsValidReturnUrl(returnUrl))
        {
            ErrorMessage = "آدرس بازگشت معتبر نیست.";
            return Page();
        }

        Input = new InputModel
        {
            ReturnUrl = returnUrl
        };

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(Input.ReturnUrl))
        {
            ErrorMessage = "درخواست تغییر رمز معتبر نیست.";
            return Page();
        }

        if (!interactionService.IsValidReturnUrl(Input.ReturnUrl))
        {
            ErrorMessage = "آدرس بازگشت معتبر نیست.";
            return Page();
        }

        if (!ModelState.IsValid)
            return Page();

        string? subject = User.FindFirstValue("sub");

        if (!Guid.TryParse(subject, out Guid userIdValue))
        {
            await SignOutAsync();
            return Redirect("/auth/login");
        }

        var userId = new UserId(userIdValue);

        var user = await userRepository.GetByIdAsync(userId, cancellationToken);
        if (user is null || !user.IsActive)
        {
            await SignOutAsync();
            return Redirect("/auth/login");
        }

        if (!user.MustChangePassword)
        {
            return Redirect(Input.ReturnUrl);
        }

        string newPasswordHash = passwordHasher.Hash(Input.NewPassword);
        var changePasswordResult = user.ChangePassword(newPasswordHash);
        if (changePasswordResult.IsFailure)
        {
            ErrorMessage = changePasswordResult.Error.Description;
            return Page();
        }

        await unitOfWork.SaveAsync(cancellationToken);

        await RefreshIdentityServerSessionAsync(user);

        return Redirect(Input.ReturnUrl);
    }

    private async Task SignOutAsync()
    {
        await HttpContext.SignOutAsync(IdentityServerConstants.DefaultCookieAuthenticationScheme);
    }

    private async Task RefreshIdentityServerSessionAsync(User user)
    {
        var claims = new List<Claim>
        {
            new("sub", user.Id.Value.ToString()),
            new("name", user.UserName),
            new("username", user.UserName),
            new("national_code", user.UserName),
            new("security_stamp", user.SecurityStamp),
            new("amr", "pwd"),
            new("idp", "local"),
            new("auth_time", DateTimeOffset.UtcNow.ToUnixTimeSeconds()
                    .ToString(CultureInfo.InvariantCulture))
        };

        var identity = new ClaimsIdentity(claims, IdentityServerConstants.DefaultCookieAuthenticationScheme);

        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(IdentityServerConstants.DefaultCookieAuthenticationScheme,
            principal,
            new AuthenticationProperties
            {
                IsPersistent = false,
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8)
            });
    }

    public sealed class InputModel
    {
        [Required]
        public string ReturnUrl { get; set; } = string.Empty;

        [Display(Name = "رمز عبور جدید")]
        [Required(ErrorMessage = "رمز عبور جدید الزامی است.")]
        [StringLength(32, MinimumLength = 8, ErrorMessage = "رمز عبور باید بین ۸ تا ۳۲ کاراکتر باشد.")]
        [RegularExpression(
            @"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{8,32}$",
            ErrorMessage = "رمز عبور باید فقط شامل حروف و اعداد باشد و حداقل یک حرف و یک عدد داشته باشد.")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; } = string.Empty;

        [Display(Name = "تکرار رمز عبور جدید")]
        [Required(ErrorMessage = "تکرار رمز عبور الزامی است.")]
        [DataType(DataType.Password)]
        [Compare(nameof(NewPassword), ErrorMessage = "رمز عبور و تکرار آن یکسان نیستند.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
using Duende.IdentityServer;
using Duende.IdentityServer.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Security.Claims;

namespace Identity.Api.Pages.Auth;

public sealed class LoginModel(IdentityDbContext dbContext,
    IPasswordHasher passwordHasher,
    IIdentityServerInteractionService interactionService) : PageModel
{
    [BindProperty]
    public InputModel Input { get; set; } = new();

    public string? ErrorMessage { get; private set; }

    public async Task<IActionResult> OnGetAsync(string? returnUrl, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(returnUrl))
            return BadRequest("returnUrl is required.");

        if (!interactionService.IsValidReturnUrl(returnUrl))
            return BadRequest("Invalid returnUrl.");

        var authorizationContext = await interactionService.GetAuthorizationContextAsync(returnUrl, ct);

        Input = new InputModel
        {
            ReturnUrl = returnUrl,
            ClientName = authorizationContext?.Client.ClientName
        };

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(Input.ReturnUrl))
            return BadRequest("returnUrl is required.");

        if (!interactionService.IsValidReturnUrl(Input.ReturnUrl))
            return BadRequest("Invalid returnUrl.");

        if (!ModelState.IsValid)
            return Page();

        string userName = Input.UserName;

        var user = await dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.UserName == userName, cancellationToken);
        if (user is null || !user.IsActive)
        {
            ErrorMessage = "نام کاربری یا رمز عبور اشتباه است.";
            return Page();
        }

        bool passwordIsValid = passwordHasher.Verify(Input.Password, user.PasswordHash);

        if (!passwordIsValid || !passwordIsValid)
        {
            ErrorMessage = "نام کاربری یا رمز عبور اشتباه است.";
            return Page();
        }

        var claims = new List<Claim>
        {
            new("sub", user.Id.Value.ToString()),
            new("name", user.UserName),
            new("username", user.UserName),
            new("national_code", user.UserName),
            new("security_stamp", user.SecurityStamp),
            new("amr", "pwd"),
            new("idp", "local"),
            new("auth_time", DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(CultureInfo.InvariantCulture))
        };

        var identity = new ClaimsIdentity(
            claims,
            IdentityServerConstants.DefaultCookieAuthenticationScheme);

        var principal = new ClaimsPrincipal(identity);

        var properties = new AuthenticationProperties
        {
            IsPersistent = Input.RememberMe,
            ExpiresUtc = Input.RememberMe
                ? DateTimeOffset.UtcNow.AddDays(7)
                : DateTimeOffset.UtcNow.AddHours(8)
        };

        await HttpContext.SignInAsync(
            IdentityServerConstants.DefaultCookieAuthenticationScheme,
            principal,
            properties);

        if (user.MustChangePassword)
            return Redirect($"/auth/change-password?returnUrl={Uri.EscapeDataString(Input.ReturnUrl)}");

        return Redirect(Input.ReturnUrl);
    }
}

public sealed class InputModel
{
    [Display(Name = "نام کاربری")]
    [Required(ErrorMessage = "{0} الزامی است.")]
    [MaxLength(50, ErrorMessage = "کاراکترهای وارد شده بیشتر از حد مجاز است.")]
    public string UserName { get; set; } = string.Empty;

    [Display(Name = "رمز عبور")]
    [Required(ErrorMessage = "{0} الزامی است.")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    public bool RememberMe { get; set; }

    public string ReturnUrl { get; set; } = string.Empty;

    public string? ClientName { get; set; }
}
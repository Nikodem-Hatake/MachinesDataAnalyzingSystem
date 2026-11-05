using MachinesDataAnalyzingSystem.Shared.DTO.Accounts;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace MachinesDataAnalyzingSystem.ClientWebApp.Authentication;

public class CustomAuthenticationStateProvider(IJSRuntime JSRuntime) : AuthenticationStateProvider
{
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            var JWTToken = await JSRuntime.InvokeAsync<string>("localStorage.getItem", "JWTToken");
            if(JWTToken is null)
            {
                return new(new());
            }
            return new(new(GetClaimsIdentity(JWTToken)));
        }
        catch(Exception)
        {
            return new(new());
        }
    }

    private ClaimsIdentity GetClaimsIdentity(string JWTToken)
    {
        var handler = new JwtSecurityTokenHandler();
        if(!handler.CanReadToken(JWTToken))
        {
            return new(claims: [], "jwt");
        }
        var claims = handler.ReadJwtToken(JWTToken).Claims;
        return new(claims, "jwt");
    }

    public async Task Logout()
    {
        await JSRuntime.InvokeVoidAsync("localStorage.removeItem", "JWTToken");
        await JSRuntime.InvokeVoidAsync("localStorage.removeItem", "JWTTokenExpirationDate");
        await JSRuntime.InvokeVoidAsync("localStorage.removeItem", "JWTRefreshToken");
        NotifyAuthenticationStateChanged(Task.FromResult<AuthenticationState>(new(new())));
    }

    public async Task SaveAuthenticationResponseData(AuthenticationResponse authenticationResponse)
    {
        await JSRuntime.InvokeVoidAsync("localStorage.setItem", "JWTToken", authenticationResponse.JWTToken);
        await JSRuntime.InvokeVoidAsync("localStorage.setItem", "JWTTokenExpirationDate", 
            authenticationResponse.JWTTokenExpirationDate.ToString());
        await JSRuntime.InvokeVoidAsync("localStorage.setItem", "JWTRefreshToken", authenticationResponse.JWTRefreshToken);
        var identity = GetClaimsIdentity(authenticationResponse.JWTToken);
        var user = new ClaimsPrincipal(identity);
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
    }
}
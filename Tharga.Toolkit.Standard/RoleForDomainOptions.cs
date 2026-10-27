namespace Tharga.Toolkit
{
    /// <summary>
    /// Options for <see cref="ClaimsExtensionsStandard.AddRoleForDomain(System.Security.Claims.ClaimsPrincipal, string, RoleForDomainOptions, string[])"/>.
    /// </summary>
    public class RoleForDomainOptions
    {
        /// <summary>
        /// When <c>true</c>, the <c>preferred_username</c> claim is used as the email if no email claim is present.
        /// Enable this only for identity providers where <c>preferred_username</c> is an address the provider controls,
        /// for example a sign-in name issued by the directory. Default is <c>false</c>.
        /// </summary>
        public bool TrustPreferredUsername { get; set; }
    }
}

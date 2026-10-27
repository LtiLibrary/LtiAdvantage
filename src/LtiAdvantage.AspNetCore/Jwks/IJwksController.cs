using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace LtiAdvantage.AspNetCore.Jwks
{
    /// <summary>JWKS publication endpoint.</summary>
    public interface IJwksController
    {
        /// <summary>Returns the JSON Web Key Set.</summary>
        Task<ActionResult<JwksDocument>> GetJwksAsync();
    }
}

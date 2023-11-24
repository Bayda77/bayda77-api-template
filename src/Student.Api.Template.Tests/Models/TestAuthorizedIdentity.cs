using System.Collections.Generic;
using Nutrien.Shared.Authorize;

namespace Tests.Models
{
    public class TestAuthorizedIdentity: IAuthorizedIdentity
    {
        public TestAuthorizedIdentity(string originalToken)
        {
            OriginalToken = originalToken;
        }

        public string Email { get; set; }
        public string Username { get; set; }
        public string OriginalToken { get; }
        public IEnumerable<string> CognitoGroups { get; set; }
    }
}
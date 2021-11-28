using Microsoft.VisualBasic;

namespace IdentityServer.Models
{
    public class IdentityModel
    {
        public IdentityModel()
        {
            
        }

        public IdentityModel(string taggedUsername, RSAPubKey pubKey)
        {
            TaggedUsername = taggedUsername;
            PublicKey = pubKey;
        }
        
        public string TaggedUsername { get; set; }
        public RSAPubKey PublicKey { get; set; }
    };
}
namespace OnlineStore.Domain.DesignPatterns.Structural.Proxy
{
    // The Proxy controls access to the RealSubject
    public class ProtectionProxy : IResource
    {
        private readonly RealResource _realResource;

        public ProtectionProxy()
        {
            _realResource = new RealResource();
        }

        public string Access(string userRole)
        {
            if (userRole == "Admin")
            {
                return _realResource.Access(userRole);
            }
            else
            {
                return "Acces interzis: Doar administratorii pot accesa această resursă.";
            }
        }
    }
}

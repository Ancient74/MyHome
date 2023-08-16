namespace MyHomeLib
{
    public interface IClientFilter
    {
        bool IsClientAllowed(string ipAddress);
    }

    public class ClientFilterWrapper : IClientFilter
    {
        private MyHomeApiLib.IClientFilter clientFilter;

        public ClientFilterWrapper(MyHomeApiLib.IClientFilter clientFilter)
        {
            this.clientFilter = clientFilter;
        }

        public bool IsClientAllowed(string ipAddress)
        {
            int isAllowed = clientFilter.IsClientAllowed(ipAddress);
            return Convert.ToBoolean(isAllowed);
        }
    }
}

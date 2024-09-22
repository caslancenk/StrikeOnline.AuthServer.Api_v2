
namespace AuthServer.Core.Configuration
{
    public interface Client
    {
        public string Id {  get; set; } 
        public string Secret { get; set; }

        public List<string> Audiences { get; set; }
    }
}

/// ITokenServicedeki         ClientTokenDto CreateTokenByClient(Client client); fonksyonunda parametre olarak kullanılıyor 
/// database için bunları entity olarak client tablosu ve audience tablosu olarak one-to-many olarak ekle ve 
/// ClientTokenDto CreateTokenByClient(Client client); burada dto olarak kullan


# VirtualWallet Backend

## Doel van het project

Dit is het backend gedeelte van het 'VirtualWallet' project.
De bedoeling is om te beleggen met virtueel geld.
Dit project is beperkt tot enkele stocks en cryptocurrencies die men kan aankopen/verkopen met virtueel geld.
Als gebruiker kan je jezelf registreren en vervolgens inloggen. De bedoeling is om virtueel geld op je portefeuille te storten waarmee je aan de slag kan om aandelen te kopen/verkopen.
Het storten van dit virtueel geld kost uiteraard nep en kost geen echt geld.

De koersen worden in hun huidige configuratie via een externe API bijgewerkt in de backend.
Standaard is dit ingesteld op 24u, aangezien het verversen van deze gegevens via de gratis versie van de externe API eerder beperkt is.
Bij een betaalversie zou men kunnen opteren om dit aan te passen naar bijvoorbeeld 5 minuten per refresh om een echte 'stock exchange' ervaring na te bootsen.

Gebruikte valuta in dit project is Amerikaanse dollar (USD).

## Setup instructies

- Clone de repository
- Controleer of je de juiste Dot Net versie ter beschikking hebt (dotnet --version)
- Installeer Nuget packages van het project (dotnet restore)
- Configureer een lokale PostgreSQL database (zie juiste settings in appsettings.json of wijzig naar wens)
- Gratis API key aanvragen via: https://www.alphavantage.co/support/#api-key
- API key in `Services/StockService` vervangen door eigen key
```csharp
        _apiKey = config["ApiKeys:AlphaVantage"];
```


## Structuur

In de solution staan twee projecten.

- VirtualWallet: de backend code
- VirtualWallet.Tests: voornamelijk testen of business rules goed worden toegepast in de service methods

### Controllers

Een niet ingelogde gebruiker mag enkel gegevens ophalen 
die voor hem bedoeld zijn, zoals bijvoorbeeld controle of een username al bestaat of het registreren van een nieuwe gebruiker.
Dit authenticatie schema wordt gebruikt voor de controller functies die dat vereisen:
```csharp
builder.Services.AddAuthentication("CookieAuth")
    .AddCookie("CookieAuth", options =>
    {

        options.Cookie.Name = "VirtualWalletAuth";
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
        options.Cookie.SameSite = SameSiteMode.None;
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        options.LoginPath = "/api/user/login";
        options.AccessDeniedPath = "/api/user/accessdenied";
    });
```

- AuthController: beveiligde cookie met claims samenstellen bij inloggen en laten verwijderen door browser bij uitloggen.
- OrderController: orders ophalen of toevoegen voor huidige gebruiker.
- StockController: ophalen van stock info via externe API en controle hoe lang dit geleden is.
- TransferController: updaten van cash van je virtuele portefeuille (virtuele geld transfers).
- UserController: huidige gebruiker ophalen, checken of username al bestaat en registreren van nieuwe gebruiker.
- WalletController: ophalen van gehele portefeuille.

### Services

Vaak is het nodig om gegevens op te halen adhv de userId.
Daarom is er gekozen om een abstracte klasse te gebruiken om op service niveau gemakkelijk toegang te krijgen tot de userId van de gebruiker:
```csharp
    protected Guid UserId
    {
        get
        {
            var userIdString = _httpContextAccessor?.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
                throw new UnauthorizedAccessException("No valid logged in user found.");
            return userId;
        }
    }
```

De claims die gebruikt worden voor de cookie komen hier goed van pas om via de geïnjecteerde IHttpContextAccessor de userId te achterhalen.
Hierdoor hoeft de userId niet handmatig als parameter door elke methode in de keten te worden doorgegeven, maar is deze context-bewust beschikbaar op de plek waar de logica wordt uitgevoerd.
De get van deze property wordt ook pas uitgevoerd wanneer nodig. Indien het een field zou zijn, gaat men in de constructor van de abstracte klasse al invulling geven.
Dit zorgt ervoor dat de controle echt op het allerlaatste moment gebeurd, net wanneer we de user zijn id nodig hebben.

### Repositories

Er is ook een abstracte klasse voorzien voor de repositories.
Het toevoegen (AddAsync), updaten(UpdateAsync), ophalen (GetByIdAsync en GetAllAsync) van gegevens zijn veelvoorkomende CRUD operaties.
Daarom werden deze adhv een generic type, namelijk de entiteit, alreeds aangemaakt in deze abstracte klasse.

Elke repository geeft de juiste entiteit door aan zijn abstracte parent klasse, bijvoorbeeld bij UserRepository:
```csharp
    public class UserRepository: AbstractBaseRepository<User>, IUserRepository
    {
        public UserRepository(AppDbContext context): base(context)
        { }
```

Op deze manier moet men enkel nog de minder voor de hand liggende CRUD operaties voorzien per repository klasse.


### Unit Of Work

Het kan gebeuren dat er bij een service methode meerdere tabelgegevens gewijzigd moeten worden.
Indien men dit stap per stap doet en telkens nieuwe gegevens opslaat in de database, is dat problematisch.
Stel dat de laatste stap niet lukt, dan zou men alle voorgaande wijzigingen moeten ongedaan maken.
Het is daarom handiger om gebruik te maken van Unit of Work.
Het komt erop neer dat alle database wijzigingen in cache worden bewaart en pas op het einde, nadat alles zonder problemen is gelukt, daadwerkelijk worden opgeslagen in de database.

Dat wil zeggen dat het effectief bewaren van veranderingen (SaveChangesAsync) niet meer op repository niveau gebeurd, maar wordt afgehandeld door UnitOfWork op service niveau.
Aangezien er een abstracte klasse is voorzien voor de services, is UnitOfWork ook geïnjecteerd in deze abstracte klasse.

### Externe API voor koersinformatie

De backend zal via een scheduler zelf na het verstrijken van een bepaald tijdstip (24u) de juiste methods uitvoeren om via de externe API van Alpha Vantage de juiste koersen van stocks ophalen.
De eigen database zal worden geupdated met de nieuwe koersen.

Models:
- ScheduleTimer: de strings 'Key' en 'Value' zorgen ervoor dat je een tijdstip kan opslaan in je database en deze een naam kan geven.
- DailyQuoteAlphaVantage: json gegevens van externe API omzetten in object (enkel wat we echt nodig hebben).
- StockAlphaVantage: json gegevens van externe API omzetten in object (enkel wat we echt nodig hebben).

Services:
- StockUpdateScheduler: geïnjecteerde IServiceProvider gaat om een instelbare tijd (bij deze 20 minuten) methods van services uitvoeren (concreet de stockservice zijn UpdateStockprices method).
- StockService: de method UpdateStockPrices gaat eerst controleren in de database of het al 24u is geleden dat de stocks zijn geupdated in eigen database. Indien dit klopt zal hier de  externe API call worden uitgevoerd en de nieuwe koersgegevens worden geupdated. Uiteindelijk wordt ook het tijdstip van laatste update bijgewerkt in de database.
# ZenCare - Dokumentacija sistema preporuke

## 1. Svrha sistema preporuke

Sistem preporuke u ZenCare aplikaciji pomaze klijentu da lakse pronadje proizvode i wellness usluge koje su relevantne za njegove prethodne aktivnosti. Preporuke se prikazuju u mobilnoj aplikaciji za autentificiranog klijenta, a administrator moze pozvati preporuke za odabranog korisnika preko administrativnih API endpointa.

Cilj nije napredni machine learning model, nego jednostavan, razumljiv i objasnjiv recommender koji koristi stvarne podatke iz baze: kupovine, stavke kupovine, termine, recenzije, proizvode, usluge i njihove kategorije/tipove.

## 2. Opsti pristup

ZenCare koristi hibridni sistem preporuke zasnovan na ponderisanim pravilima:

- content-based signal: podudaranje kategorije/tipa sa prethodnim korisnickim izborima;
- popularity-based signal: ucestalost kupovine proizvoda ili rezervacije usluga;
- rating signal: prosjecna ocjena i broj odobrenih recenzija;
- fallback signal: osnovni bod za svaki dostupan proizvod ili uslugu.

Algoritam se izvrsava u `RecommendationService` i cita podatke direktno iz baze asinhronim EF Core upitima. Nema offline treninga, cache sloja ili collaborative filtering modela.

## 3. Podaci koji se koriste

| Signal/podatak | Izvor | Nacin koristenja |
|---|---|---|
| Prethodno kupljene kategorije proizvoda | `PurchaseItems`, `Purchases`, `Products.ProductCategoryId` | Ako proizvod pripada kategoriji koju je korisnik ranije kupovao, dobija dodatni bod. |
| Prethodno kupljeni tipovi proizvoda | `PurchaseItems`, `Purchases`, `Products.ProductTypeId` | Ako proizvod pripada tipu koji je korisnik ranije kupovao, dobija dodatni bod. |
| Popularnost proizvoda | `PurchaseItems` | Za svaki proizvod se racuna ukupna kupljena kolicina i broj stavki kupovine. |
| Recenzije proizvoda | `Reviews` sa `ProductId` i statusom `Approved` | Racuna se prosjecna ocjena i broj odobrenih recenzija po proizvodu. |
| Prethodno rezervisane kategorije usluga | `Appointments`, `WellnessServices.ServiceCategoryId` | Ako usluga pripada kategoriji koju je korisnik ranije rezervisao, dobija dodatni bod. |
| Popularnost usluga | `Appointments` | Za svaku uslugu se racuna broj termina. |
| Recenzije usluga | `Reviews` sa `AppointmentId`, povezane preko `Appointments.WellnessServiceId`, status `Approved` | Racuna se prosjecna ocjena i broj odobrenih recenzija po usluzi. |
| Dostupnost proizvoda | `Products.Status` | Iskljucuju se proizvodi sa statusima `Inactive`, `OutOfStock` i `Archived`. |
| Dostupnost usluga | `WellnessServices.Status` | Iskljucuju se usluge sa statusima `Inactive` i `Archived`. |
| Evidencija isporucenih preporuka | `RecommendationLogs` | Upisuje se nakon generisanja preporuka; ne koristi se kao ulaz za bodovanje. |

Ne postoji perzistirana historija pretrage, pa pretrazivanje nije signal u trenutnoj implementaciji.

## 4. Algoritam i bodovanje

Implementacija se nalazi u `ZenCare.Services.Services.RecommendationService`.

Konstante:

| Konstanta | Vrijednost | Znacenje |
|---|---:|---|
| `DefaultTake` | 5 | Zadani broj preporuka ako klijent ne posalje validan `take`. |
| `MaxTake` | 50 | Najveci dozvoljeni broj preporuka. |
| `CategoryPreferenceBoost` | 12 | Dodatni bod za podudaranje kategorije proizvoda/usluge. |
| `TypePreferenceBoost` | 8 | Dodatni bod za podudaranje tipa proizvoda. |
| `PopularityWeight` | 1 | Tezina popularnosti. |
| `ReviewRatingWeight` | 2 | Tezina prosjecne ocjene. |
| `ReviewCountWeight` | 0.5 | Tezina broja odobrenih recenzija. |

### Proizvodi

Svaki kandidat pocinje sa:

```text
score = 1
reason = "Recommended as an available product."
```

Zatim se primjenjuju pravila:

```text
ako proizvod ima popularnost:
    score += (ukupna_kolicina + broj_stavki_kupovine) * 1

ako proizvod ima odobrene recenzije:
    score += prosjecna_ocjena * 2 + broj_recenzija * 0.5

ako je prosjecna_ocjena >= 4:
    reason = "Highly rated by users."

ako je kategorija proizvoda ranije kupljena:
    score += 12
    reason = "Recommended because you previously bought products from this category."

ako je tip proizvoda ranije kupljen:
    score += 8
    ako kategorija nije vec podudarena:
        reason = "Recommended because it matches product types you previously purchased."
```

Kandidati se sortiraju po `Score` opadajuce, zatim po `Name` rastuce, i uzima se `take` rezultata.

### Usluge

Svaki kandidat pocinje sa:

```text
score = 1
reason = "Recommended as an available service."
```

Zatim se primjenjuju pravila:

```text
ako usluga ima termine:
    score += broj_termina * 1
    reason = "Popular service based on appointment history."

ako usluga ima odobrene recenzije:
    score += prosjecna_ocjena * 2 + broj_recenzija * 0.5

ako je prosjecna_ocjena >= 4:
    reason = "Highly rated by users."

ako je kategorija usluge ranije rezervisana:
    score += 12
    reason = "Recommended because you previously booked services from this category."
```

Kandidati se sortiraju po `Score` opadajuce, zatim po `Name` rastuce, i uzima se `take` rezultata.

## 5. Personalizacija

Personalizacija se zasniva na historiji konkretnog korisnika:

- za proizvode se gledaju kategorije i tipovi proizvoda koje je korisnik ranije kupio;
- za usluge se gledaju kategorije usluga koje je korisnik ranije rezervisao.

Za klijentske endpoint-e korisnicki identitet se uzima iz JWT claim-a `ClaimTypes.NameIdentifier`, tako da klijent ne salje tudji `UserId`.

## 6. Fallback za korisnike bez historije

Ako korisnik nema historiju kupovina ili termina, sistem i dalje vraca preporuke kada postoje dostupni kandidati:

- svaki dostupan proizvod dobija osnovni `score = 1`;
- svaka dostupna usluga dobija osnovni `score = 1`;
- popularnost i odobrene recenzije mogu dodatno povecati score i bez licne historije korisnika.

Time se izbjegava prazna lista za nove korisnike, osim ako u bazi nema dostupnih proizvoda/usluga koji prolaze status filtere.

## 7. Objasnjive preporuke

Svaka preporuka vraca polje `Reason`. Razlog se postavlja prema najznacajnijem pravilu koje je u kodu posljednje preuzelo objasnjenje.

Primjeri razloga koje trenutna implementacija moze vratiti:

- `Recommended because you previously bought products from this category.`
- `Recommended because you previously booked services from this category.`
- `Popular product based on previous purchases.`
- `Popular service based on appointment history.`
- `Highly rated by users.`
- `Recommended as an available product.`
- `Recommended as an available service.`

Mobilna aplikacija prikazuje razlog na kartici preporuke. Ako bi razlog bio prazan, mobilna aplikacija prikazuje rezervnu poruku `No recommendation reason was provided.`, ali backend trenutno uvijek postavlja razlog za generisane preporuke.

## 8. RecommendationLog / evidencija preporuka

`RecommendationLog` se koristi kao evidencija isporucenih preporuka. Nakon sto `RecommendationService` generise listu preporuka, za svaki rezultat se upisuje zapis sa:

- `UserId`
- `ProductId` ili `WellnessServiceId`
- `Score`
- `Reason`
- `CreatedAt`

Ova tabela ne ucestvuje u trenutnom bodovanju. Ona sluzi za audit/analizu toga sta je sistem preporucio korisniku i sa kojim razlogom. Ako upis logova ne uspije, servis odvaja neuspjele entitete iz EF Core change trackera i ne prekida vracanje preporuka korisniku.

Administrativni CRUD za `RecommendationLog` postoji kroz `RecommendationLogController` i zasticen je rolom `Admin`.

## 9. API endpointi

| Method | Endpoint | Svrha | Autorizacija |
|---|---|---|---|
| GET | `/Recommendation/My/products?take=5` | Vraca personalizovane preporuke proizvoda za trenutno prijavljenog klijenta. | `Client` |
| GET | `/Recommendation/My/services?take=5` | Vraca personalizovane preporuke usluga za trenutno prijavljenog klijenta. | `Client` |
| GET | `/Recommendation/Products/{userId}?take=5` | Vraca preporuke proizvoda za odabranog korisnika. | `Admin` |
| GET | `/Recommendation/Services/{userId}?take=5` | Vraca preporuke usluga za odabranog korisnika. | `Admin` |
| GET | `/RecommendationLog` | Pregled evidencije preporuka. | `Admin` |
| GET | `/RecommendationLog/{id}` | Pregled pojedinacnog log zapisa. | `Admin` |
| POST | `/RecommendationLog` | Rucno kreiranje log zapisa kroz CRUD servis. | `Admin` |
| PUT | `/RecommendationLog/{id}` | Izmjena log zapisa kroz CRUD servis. | `Admin` |
| DELETE | `/RecommendationLog/{id}` | Brisanje log zapisa. | `Admin` |

Parametar `take` se normalizuje:

- ako je `take <= 0`, koristi se `DefaultTake = 5`;
- ako je `take > 50`, koristi se `MaxTake = 50`.

Response za preporuke je lista `RecommendationItemResponse`:

```json
{
  "id": 1,
  "name": "Naziv",
  "type": "Product",
  "score": 13.5,
  "reason": "Recommended because you previously bought products from this category."
}
```

## 10. Mobilna aplikacija

Mobilna aplikacija registruje `RecommendationService` u `main.dart`. `RecommendationProvider` ucitava proizvode i usluge paralelno preko:

- `/Recommendation/My/products`
- `/Recommendation/My/services`

Ekran `RecommendationsScreen` se otvara iz profila preko dugmeta `Recommendations`. Ekran ima dva taba:

- `Products`
- `Services`

Kartica preporuke prikazuje:

- naziv;
- tip (`Product` ili `Service`);
- score;
- reason.

Klik na preporuku proizvoda otvara postojece detalje proizvoda. Klik na preporuku usluge otvara `ServiceDetailsScreen`, koji ucitava detalje preko `GET /Service/{id}`.

Ako API vrati praznu listu, mobilna aplikacija prikazuje poruku `No recommendations available.`.

## 11. Ogranicenja trenutne implementacije

- Model je jednostavan weighted-rule recommender, ne machine learning model.
- Nema collaborative filtering-a.
- Nema offline treninga ili periodickog recalculation job-a.
- Nema perzistirane historije pretrage, pa se search history ne koristi.
- `RecommendationLog` se ne koristi kao ulazni signal za buduce preporuke.
- Status filteri iskljucuju neaktivne/arhivirane proizvode i usluge, ali proizvodi u statusu `Draft` i usluge u statusu `Draft` trenutno nisu iskljuceni jer kod eksplicitno izbacuje samo odredjene statuse.
- Popularnost proizvoda trenutno koristi sve `PurchaseItems`, bez dodatnog filtera po statusu kupovine.
- Popularnost usluga trenutno koristi sve `Appointments`, bez dodatnog filtera po statusu termina.

## 12. Primjer toka preporuke

Primjer za preporuku proizvoda:

1. Klijent je ranije kupio proizvod iz kategorije `Poklon paketi`.
2. `RecommendationService` ucitava njegove prethodno kupljene kategorije i tipove iz `PurchaseItems`.
3. Servis ucitava popularnost proizvoda iz svih `PurchaseItems`.
4. Servis ucitava odobrene recenzije proizvoda iz `Reviews`.
5. Svi proizvodi koji nisu `Inactive`, `OutOfStock` ili `Archived` postaju kandidati.
6. Proizvod iz kategorije `Poklon paketi` dobija osnovni bod i dodatni `CategoryPreferenceBoost = 12`.
7. Ako ima kupovine/recenzije, dodaju se i ti bodovi.
8. Rezultati se sortiraju po score-u i uzima se top `take`.
9. Korisnik u mobilnoj aplikaciji vidi naziv, score i razlog, npr. `Recommended because you previously bought products from this category.`
10. Sistem upisuje preporuku u `RecommendationLogs`.

Primjer za preporuku usluge:

1. Klijent je ranije rezervisao uslugu iz kategorije `Masaže`.
2. Servis pronalazi kategorije usluga iz njegovih `Appointments`.
3. Svaka dostupna usluga iz iste kategorije dobija `CategoryPreferenceBoost = 12`.
4. Dodatno se racuna popularnost po broju termina i rating po odobrenim recenzijama.
5. Mobilna aplikacija prikazuje preporuku i razlog.
6. Klik na preporuku otvara detalje usluge.

## 13. Testiranje sistema preporuke

Preporuceni manualni testovi:

A) Klijent sa historijom:

1. Kreirati klijenta.
2. Napraviti kupovinu proizvoda iz odredjene kategorije/tipa.
3. Pozvati `GET /Recommendation/My/products`.
4. Provjeriti da proizvodi iz iste kategorije/tipa imaju veci score i odgovarajuci reason.

B) Novi klijent bez historije:

1. Registrovati novog klijenta bez kupovina i termina.
2. Pozvati oba `/Recommendation/My/...` endpointa.
3. Provjeriti da se vracaju dostupni proizvodi/usluge ako postoje kandidati u bazi.

C) Popularan ili visoko ocijenjen item:

1. Dodati vise stavki kupovine za proizvod ili vise termina za uslugu.
2. Dodati odobrene recenzije sa ocjenom 4 ili 5.
3. Provjeriti da score raste i da razlog moze biti `Highly rated by users.` ili popularnost.

D) Vidljiv razlog preporuke:

1. Otvoriti mobilni ekran `Recommendations`.
2. Provjeriti da kartica prikazuje `Score` i tekst razloga.

E) Neaktivni proizvodi/usluge:

1. Postaviti proizvod na `Inactive`, `OutOfStock` ili `Archived`.
2. Postaviti uslugu na `Inactive` ili `Archived`.
3. Provjeriti da se takvi zapisi ne vracaju u preporukama.

F) Limit rezultata:

1. Pozvati endpoint sa `take=2`.
2. Provjeriti da se vracaju najvise 2 rezultata.
3. Pozvati endpoint sa vrijednoscu vecom od 50 i provjeriti da se rezultat ogranicava na 50.

G) RecommendationLog:

1. Pozvati endpoint za preporuke.
2. Kao administrator otvoriti `/RecommendationLog`.
3. Provjeriti da su upisani `UserId`, `ProductId` ili `WellnessServiceId`, `Score`, `Reason` i `CreatedAt`.

## 14. Uskladjenost sa RSII zahtjevima

| RSII zahtjev | ZenCare implementacija |
|---|---|
| Koristi stvarne perzistirane podatke | Koriste se `PurchaseItems`, `Purchases`, `Appointments`, `Reviews`, `Products`, `WellnessServices` i kategorije/tipovi. |
| Ne koristi lazne ulaze | Scoring se zasniva na zapisima iz baze podataka. |
| Jednostavan poznat pristup primjeren projektu | Implementiran je hibridni weighted-rule pristup sa content-based, popularity i rating signalima. |
| Svi prikupljeni scoring signali se koriste | U `RecommendationService` se koriste preferencije, popularnost i odobrene recenzije. `RecommendationLog` nije scoring signal nego izlazna evidencija. |
| Objasnjive preporuke | Svaki `RecommendationItemResponse` sadrzi `Reason`, a mobilna aplikacija ga prikazuje. |
| Rezultati su ograniceni | `take` se normalizuje na zadano 5 i maksimalno 50. |
| Personalizacija po korisniku | Klijentski endpointi koriste JWT `NameIdentifier`; admin endpointi eksplicitno primaju `userId`. |
| Dokumentacija u repozitoriju | Ovaj dokument je kreiran kao `recommender-dokumentacija.md`. |
| Uskladjenost sa ZenCare domenom | Preporucuju se proizvodi i wellness usluge, sto odgovara funkcionalnostima aplikacije. |

# ZenCare - Dokumentacija sistema preporuke

## 1. Svrha i pristup

ZenCare koristi objasnjivi hibridni weighted-rule recommender za proizvode i wellness usluge. Sistem kombinuje content-based preferencije konkretnog klijenta, globalnu popularnost, odobrene recenzije i cold-start osnovni bod.

Algoritam se izvrsava u `ZenCare.Services.Services.RecommendationService` nad stvarnim podacima iz SQL baze. Nema offline treninga, collaborative filtering modela niti laznih ulaznih signala.

## 2. Product-Supplier i ProductView podaci

Svaki `Product` ima obavezni `SupplierId` FK prema postojecem `Supplier` entitetu. Kategorija, tip i dobavljac zajedno opisuju content slicnost proizvoda.

Kada autentificirani Client otvori detalje proizvoda, Mobile nakon uspjesnog ucitavanja poziva:

```text
POST /Product/My/{id}/view
```

`UserId` se cita iz JWT `ClaimTypes.NameIdentifier`; klijent ga ne salje. `ProductViews` cuva jedan zapis po kombinaciji `UserId + ProductId`, uz `ViewCount` i `LastViewedAt`. Ponovni pregled povecava broj i osvjezava vrijeme umjesto kreiranja neogranicenih duplikata. Mobile tretira zapisivanje pregleda kao ne-kriticno, pa eventualni problem sa tim pozivom ne blokira prikaz detalja proizvoda.

## 3. Ulazni signali

### Proizvodi

| Signal | Izvor | Upotreba |
|---|---|---|
| Kupljene kategorije | `PurchaseItems -> Purchase -> Product` | Kategorije iz kupovina klijenta koje su `Completed` i `Succeeded`. |
| Kupljeni tipovi | `PurchaseItems -> Purchase -> Product` | Tipovi iz istih validnih kupovina. |
| Kupljeni dobavljaci | `PurchaseItems -> Purchase -> Product.SupplierId` | Dobavljaci iz istih validnih kupovina. |
| Vlastite visoke ocjene | `Reviews` | Samo klijentove `Approved` produkt recenzije sa ocjenom 4 ili 5; izvode se kategorija, tip i dobavljac. |
| Pregledani proizvodi | `ProductViews` | Iz pregledanih proizvoda izvode se kategorija, tip i dobavljac. |
| Globalna popularnost | validni `PurchaseItems` | Ukupna kolicina i broj stavki samo iz `Completed + Succeeded` kupovina. |
| Globalne recenzije | `Approved` produkt recenzije | Prosjecna ocjena i broj recenzija po proizvodu. |
| Dostupnost | `Products` | Kandidat mora biti `Active` i imati `StockQuantity > 0`. |

`Draft`, `Inactive`, `OutOfStock`, `Archived` i proizvodi bez zalihe nisu kandidati.

### Usluge

Postojeci service recommender ostaje zasnovan na:

- kategorijama usluga iz termina konkretnog klijenta;
- globalnom broju termina po usluzi;
- prosjeku i broju odobrenih recenzija;
- dostupnosti prema statusu usluge.

## 4. Product scoring

Svaki aktivan proizvod sa zalihom pocinje sa:

```text
score = 1
reason = "Recommended as an available product."
```

Zatim se sabiraju svi primjenjivi bodovi:

| Pravilo | Bodovi |
|---|---:|
| Globalna popularnost | `(ukupna kolicina + broj stavki) * 1` |
| Globalne odobrene recenzije | `prosjecna ocjena * 2 + broj recenzija * 0.5` |
| Kategorija iz validne kupovine klijenta | `+12` |
| Dobavljac iz validne kupovine klijenta | `+10` |
| Tip iz validne kupovine klijenta | `+8` |
| Kategorija proizvoda koji je klijent ocijenio sa 4 ili 5 | `+10` |
| Dobavljac proizvoda koji je klijent ocijenio sa 4 ili 5 | `+8` |
| Tip proizvoda koji je klijent ocijenio sa 4 ili 5 | `+6` |
| Kategorija pregledanog proizvoda | `+6` |
| Dobavljac pregledanog proizvoda | `+5` |
| Tip pregledanog proizvoda | `+4` |

Kupovina je validan pozitivan signal samo kada je:

```text
Purchase.Status == Completed
Purchase.PaymentStatus == Succeeded
```

Draft, PendingPayment, Paid, Processing, ReadyForPickup, Shipped, Cancelled, Refunded i Failed kupovine ne ulaze u personalne ni globalne purchase signale.

Rezultati se sortiraju po `Score` opadajuce, zatim po `Name` rastuce, a zatim se primjenjuje formalna paginacija. `Page` ima zadanu vrijednost 1, `PageSize` zadanu vrijednost 5 i maksimalnu vrijednost 100. Parametar `Take` ostaje podrzan samo kao backward-compatible alternativa za `PageSize` kada `PageSize` nije poslan.

## 5. Service scoring

Postojeci service scoring ostaje:

| Pravilo | Bodovi |
|---|---:|
| Osnovni bod | `1` |
| Broj termina | `broj termina * 1` |
| Odobrene recenzije | `prosjecna ocjena * 2 + broj recenzija * 0.5` |
| Ranije rezervisana kategorija klijenta | `+12` |

## 6. Objasnjivi Reason

Svaki primijenjeni signal dodaje bodove. `Reason` pripada pojedinacnom primijenjenom signalu sa najvecom vrijednoscu; kod izjednacenja ostaje ranije evaluirani razlog. Moguci produkt razlozi obuhvataju:

- prethodno kupljenu kategoriju, tip ili dobavljaca;
- slicnost proizvodima koje je klijent visoko ocijenio;
- slicnost proizvodima koje je klijent pregledao;
- globalnu popularnost zavrsenih kupovina;
- odobrene produkt recenzije;
- cold-start dostupnost.

Interni iznosi tezina se ne prikazuju u razlogu. Mobile prikazuje `Reason` na kartici preporuke.

## 7. Cold start

Klijent bez kupovine, recenzije ili pregleda i dalje dobija rezultate kada postoje aktivni proizvodi sa zalihom. Svaki kandidat dobija osnovni bod, a globalna popularnost i odobrene recenzije mogu ga dodatno rangirati. Prazna lista je ispravna samo kada nema proizvoda koji zadovoljavaju eligibility filter.

## 8. RecommendationLog

Nakon generisanja, svaki rezultat se evidentira u `RecommendationLogs` sa korisnikom, proizvodom ili uslugom, score-om, razlogom i UTC vremenom. Log je izlazni audit zapis i nije ulazni signal za buduce bodovanje.

## 9. Endpointi

| Metoda | Endpoint | Uloga |
|---|---|---|
| GET | `/Recommendation/My/products?Page=1&PageSize=5` | `Client` |
| GET | `/Recommendation/My/services?Page=1&PageSize=5` | `Client` |
| GET | `/Recommendation/Products/{userId}?Page=1&PageSize=5` | `Admin` |
| GET | `/Recommendation/Services/{userId}?Page=1&PageSize=5` | `Admin` |
| POST | `/Product/My/{id}/view` | `Client` |

Klijentski endpointi identitet uzimaju iz JWT-a. Admin endpointi eksplicitno primaju korisnika za administrativno testiranje preporuka. Svi list endpointi vracaju `PagedResult<RecommendationItemResponse>`; `PageSize` je ogranicen na 100. Legacy `Take` parametar ostaje podrzan radi kompatibilnosti, ali formalni ugovor koristi `Page` i `PageSize`.

## 10. Mobilna aplikacija

`RecommendationsScreen` paralelno ucitava produkt i service preporuke, prikazuje naziv, score i razlog te otvara postojece detalje proizvoda ili usluge. `ProductDetailsScreen` nakon uspjesnog GET detalja salje ne-kriticni view poziv. Supplier se prikazuje u detaljima proizvoda.

## 11. Ogranicenja

- Model je weighted-rule recommender, ne machine learning model.
- Nema collaborative filtering-a niti offline treninga.
- `RecommendationLog` nije signal za ponovno bodovanje.
- Historija tekstualnih pretraga nije signal; proposal-required pregled proizvoda jeste perzistiran signal.
- Service recommender zadrzava postojeci skup signala i statusnu logiku.

## 12. Manualni testovi

1. Novi klijent bez historije dobija aktivne proizvode sa zalihom i objasnjenje cold-start/popularity razloga.
2. Otvaranje detalja proizvoda kreira jedan `ProductView`; ponovno otvaranje povecava `ViewCount` i azurira `LastViewedAt`.
3. Proizvod iste kategorije/tipa/dobavljaca kao pregledani proizvod dobija view boost.
4. Approved ocjena 4 ili 5 daje personalni rating boost slicnim proizvodima; tudja recenzija ostaje samo globalni signal.
5. Samo `Completed + Succeeded` kupovina daje purchase preference i ulazi u globalnu popularnost.
6. Supplier slicnost utice na score i moze dati supplier razlog.
7. Draft, neaktivni, rasprodani i arhivirani proizvodi se ne vracaju.
8. `RecommendationLogs` sadrzi finalni score i razlog.

## 13. RSII uskladjenost

- Svi deklarisani scoring signali citaju se iz stvarno perzistiranih podataka.
- Proposal-required kupovine, visoke ocjene, pregledi, kategorije i dobavljaci stvarno uticu na score.
- Svaka preporuka ima objasnjenje.
- Klijentski identitet dolazi iz validiranog JWT-a.
- Broj rezultata je ogranicen.
- Dokumentacija opisuje iste eligibility uslove, tezine i fallback ponasanje kao implementacija.

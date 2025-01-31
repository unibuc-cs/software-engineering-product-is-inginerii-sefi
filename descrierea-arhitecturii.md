Aplicatia utilizeaza framework-ul ASP.NET Core. Arhitectura ilustreaza design pattern-ul arhitectural Model-View-Controller, ce ofera o delimitare inteligenta intre componentele cruciale ale aplicatiei, oferind o experienta de dezvoltare adedcvata si garantii solide de securitate. 
Astfel, entitatile din baza de date, sunt transpuse in mod automat intr-un model orientat pe obiecte.
Acest model este generat automat prin tehnologia Model Binding, oferind un mecanism de reutilizare extrem de simplu si eficient. De asemenea, in cazul unei schimbari in provider-ul bazei de date, legatura cu noua baza de date se realizeaza automat prin intermediul unei simple schimbari de driver. Astfel, se pot integra si tehnologii diferite precum SQL, NoSQL, Distributed Databases.

Frontend-ul este compilat la nivel de server, permitand integrarea cu bucati de cod C# ce este mai apoi convertit in Java Script. Acest lucru nu ne limiteaza totusi, intrucat am realizat componente specifice precum player-ul si sistemul de like-uri in JS.
Aceasta arhitectura pentru frontend consuma mult mai putine resurse la nivelul utilizatorului, centralizand un procent mare din computatii la nivel de server. 

Cele doua tipuri de utilizatori sunt delimitate cu ajutorul serviciului ASP.NET Core Identity, ce ofera un sistem de roluri, dar si o modalitate de autentificare ce si-a demonstrat eficienta in numeroasele alte proiecte de anvergura.

Backend-ul este realizat cu ajutorul unor Controllere, ce reprezinta actiuni concrete in aplicatie. Spre exemplu, functiile acestora, ilustreaza actini precum adaugarea, modificare, stergerea si afisarea coerenta a unor entitati precum melodiile, playlist-urile, dar chiar si utilizatorii in sine sau interactiunea intre utilizatori (relatia de prietenie, like, comentariu).

Datorita eficientei design-ului MVC, am reusit sa satisfacem toate ideile propuse initial.



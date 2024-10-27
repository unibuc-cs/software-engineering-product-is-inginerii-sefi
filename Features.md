# Product Features

1. Autentificare

**Descriere**
Autentificarea este folosita pentru identificarea individuala a utilizatorilor si este bazata pe un sistem de nume de utilizator si parola. In pasul de creare a unui cont, exista doua posibilitati: cont de ascultator sau cont de artist.
**Constrangeri**
Numele de utilizator trebuie sa fie valid, sa nu se repete cu al altui utilizator, parola trebuie sa respecte niste standarde minime de securitate (minim 8 caractere, cel putin o litera mare, cel putin o cifra, cel putin un caracter special).
**Comentarii**
Pe viitor, daca aplicatia va creste considerabil, se doreste implementarea unui sistem de multi-factor authentication. Alta imbunatatire ar fi verificarea daca parola este gasita intr-o lista de parole comune.

2. Explorare Muzica Noua

**Descriere**
Feature-ul permite utilizatorilor sa exploreze catalogul principal, unde sunt prezentate piese ale artistilor.

**Constrangeri**
Fiind pagina principala de UI, trebuie sa ne asiguram ca se incarca repede si ca este responsive, dinamic si eficient. Piesele artistilor afisate pe catalogul principal nu vor avea automat si continutul piesei (sunetul efectiv) incarcat.

**Comentarii**
In viitor, se poate adauga un algoritm de recomandare personalizat, care sa sugereze muzica in functie de preferintele utilizatorului. Bazat pe User Story-ul 1 si 12.

3. Playlist-uri

**Descriere**
Ascultatorii pot crea si personaliza playlist-uri pentru a-si organiza piesele preferate. Functia permite salvarea si gestionarea playlist-urilor direct in contul personal. De asemenea, utilizatorii imprieteniti isi pot vedea playlist-urile create.

**Constrangeri**
Utilizatorii trebuie sa fie autentificati pentru a crea playlist-uri. Piesele pot fi adaugate dupa crearea unui playlist initial gol. O piesa poate fi adaugata in mai multe playlist-uri.

**Comentarii**
Bazat pe User Story-ul 2 si 4.

4. Sistem de Comentarii

**Descriere**
Functia permite ascultatorilor sa lase comentarii pe piese, facilitand interactiunea dintre utilizatori si oferindu-le posibilitatea de a-si exprima aprecierea sau de a oferi feedback.

**Constrangeri**
Comentariile trebuie moderate pentru a preveni spamul si continutul neadecvat. Fiecare comentariu poate avea o limita de caractere.

**Comentarii**
Ar fi util sa implementam un sistem de reactii rapide, similar cu "like", pentru comentarii, astfel incat utilizatorii sa isi poata exprima rapid aprecierea. Bazat pe User Story-urile 3 si 10.

5. Sistem de cautare

**Descriere**
Utilizatorii pot cauta rapid un artist, o piesa sau un album specific folosind bara de cautare. Aceasta functie permite o navigare eficienta si acces rapid la continutul dorit.

**Constrangeri**
Rezultatele cautarii trebuie sa fie relevante. Ordonarea pieselor sau albumelor va fi dupa data postarii. Bara de cautare trebuie optimizata pentru a accepta diferite variante ale numelor artistilor sau pieselor.

**Comentarii**
Bazat pe User Story-ul 5.

6. Prieteni

**Descriere**
Ascultatorii au posibilitatea de a adauga alti utilizatori ca prieteni pentru a impartasi descoperiri muzicale si a construi o retea de persoane cu interese similare.

**Constrangeri**
Utilizatorii trebuie sa accepte cererile de prietenie pentru a adauga pe cineva in lista de prieteni. O prietenie poate fi anulata ulterior de catre oricare dintre cei doi utilizatori.

**Comentarii**
Ar fi interesant sa adaugam notificari pentru activitatea prietenilor, cum ar fi adaugarea unei noi piese in playlist-ul lor public. Bazat pe User Story-ul 4 si 6.

7. Incarcare Piese pentru Artisti

**Descriere**
Artistii pot incarca gratuit piese noi pe FMI pentru ca acestea sa fie accesibile ascultatorilor. Functia este menita sa incurajeze creatia de continut nou si sa faciliteze interactiunea cu publicul. Piesele unui artist pot fi Single-uri sau organizate in albume.

**Constrangeri**
Un artist nu poate adauga albume goale, un album fiind alcatuit din cel putin doua piese. Un artist nu poate avea doua albume sau piese cu acelasi nume.

**Comentarii**
In viitor, pentru eficienta si ca sa nu se suprasoliciteze severele, putem impune o marime maxima pentru o piesa. Bazat pe User Story-urile 7 si 8.

8. Statistici pentru Artisti

**Descriere**
Artistii pot vizualiza statistici legate de numarul de ascultari, aprecieri si comentarii pentru fiecare piesa si album, pentru a obtine o perspectiva asupra impactului continutului lor.

**Constrangeri**
Pentru a pastra confidentialitatea, statistica detaliata este accesibila doar artistilor.

**Comentarii**
Bazat pe User Story-ul 9.

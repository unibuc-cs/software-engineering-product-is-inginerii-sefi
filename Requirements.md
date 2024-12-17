### Requirements

1. Ca ascultator, vreau sa pot explora gratuit muzica noua din catalogul principal, astfel incat sa pot descoperi artisti underground si sa-mi extind gusturile muzicale.

* Vrem ca ascultatorul sa se bucure gratuit de implementarea unei pagini principale care sa contina:
    * titlu
    * motor de cautare
    * buton hyperlink pentru a ajunge la ea
    * numarul de pagini
    * sistem de navigare prin pagini
    * grid cu preview piese postate de diferiti artisti
    * buton pentru a vedea mai in detaliu ce contine piesa 
 
2. Ca ascultator, vreau sa pot crea playlist-uri cu muzica preferata descoperita pe FMI, astfel incat sa imi organizez piesele pentru diferite stari si activitati.

* Implementarea unei pagini care contine playlist-urile:
    * buton hyperlink pentru a ajunge la ea
    * grid cu toate playlist-urile ascultatorului ordonate alfabetic dupa titlu
    * butoane pentru fiecare playlist de a arata toate piesele incluse in el
    * buton de stergere pentru fiecare playlist daca acesta apartine utilizatorlui conectat
    * numarul de piese pe care fiecare playlist le are
    * numarul de pagini
    * sistem de navigare prin pagini
    * numarul de pagini

* Implementarea butonului de adaugare a unei piese intr-un playlist

* Validari pentru buton (o piesa nu poate fi adaugata de mai multe ori intr-un playlist)

3. Ca ascultator, vreau sa pot lasa comentarii la piesele mele preferate, astfel incat sa imi exprim aprecierea si sa interactionez cu alti fani si cu artistul.

* Implementare formular de lasat comentarii
    * continut
    * utilizator 
    * data postarii
    * buton de stergere
    * buton de reply
* Validari 
    * continutul comentariului trebuie sa fie mai mare strict de 0 caractere si mai mic de 200
    * butonul de stergere va fi vizibil doar utiliztorlui care a postat acel comentariu

4. Ca ascultator, vreau sa pot vizualiza playlist-urile publice ale prietenilor mei, astfel incat sa aflu ce muzica asculta si sa descopar piese noi prin preferintele lor.

* Vreau ca ascultatorul sa aiba posibilitatea de intra pe pagina prietenilor lui printr-un buton hyperlink din meniul principal si acolo sa vada playlist-urile publice ale prietenilor lui.

5. Ca ascultator, vreau sa am optiunea de a cauta direct un artist, o piesa sau un album, pentru a ajunge rapid la continutul dorit.

* Implementarea unui buton de search in catalogul principal care va cauta atat prin numele piselor cat si prin numele atistilor si numele albumelor expuse.


6. Ca ascultator, vreau sa pot adauga alti utilizatori ca prieteni pentru a impartasi descoperirile muzicale si pentru a construi o retea de persoane cu aceleasi interese.

* Implementarea unui buton de cerere de prietenie care va aparea pentru orice ascultator.
* Implementarea sistemului de acceptare a unei cereri de prietenie care va determina imprietenirea celor doi utilizatori corespunzatori. Butonul de accept va aparea intr-o pagina speciala dedicata vizualizarii prietenilor curenti si a cererilor de prietenie care asteapta sa fie confirmate/infirmate.
* Validari:
    - O cerere de prietenie se va putea trimite o singura data.
    - Daca cererea de prietenie este anulata, se poate resolicita prietenia.
    - Daca un user anuleaza prietenia, nu se va trimite o notificare celuilalt user.

7. Ca artist, vreau sa am posibilitatea sa incarc gratuit piese noi pe FMI, pentru ca ascultatorii sa le poata accesa si comenta.

* Implementarea formularului de adaugare a unei piese care va contine campurile:
    - Numele piesei
    - Fisierul mp3/mp4 corespunzator melodiei
* In baza de date se va tine calea corespunzatoare catre fisierul incarcat pe disk.
* Validari:
    - Campurile din formular trebuie sa fie integral completate.

8. Ca artist, vreau sa imi pot organiza piesele in albume sau sa le lansez ca single-uri, astfel incat sa pot crea lansari structurate si sa ofer flexibilitate fanilor mei in ascultare.

* Implementare formular adaugare album care contine campurile:

    - Nume album
    - Nume piesa 1, continut piesa 1, ... , nume piesa n, continut piesa n.

* Va exita un buton care va extinde numarul de piese care pot fi adaugate in album in mod dinamic.
* Cazul de single va fi considerat formularul de adaugare a unei singure piese.

* Validari:
    - Toate campurile cerute trebuie sa fie integral completate. 

9. Ca artist, vreau sa pot vizualiza statisticile legate de numarul de ascultari, aprecieri si comentarii pentru fiecare piesa si album, astfel incat sa pot vedea ce rezoneaza cel mai mult cu publicul meu.

* Implementare pagina de statistici pentru utilizatorii de tipul artist. Aceasta nu va incalca nicio regula de privacy si va prezenta diferite diagrame, tabele, charturi care vor fi calculate in mod dinamic (i.e. nu se vor tine aceste lucruri in baza de date).

10. Ca artist, vreau sa pot raspunde la comentariile ascultatorilor, astfel incat sa creez o legatura mai puternica cu comunitatea si sa primesc feedback direct.

* Implementarea butonului de reply la alte comentarii. Detalii oferite si mai sus.

11. Ca artist, vreau sa pot personaliza profilul meu cu biografia si descrierea muzicii mele, astfel incat ascultatorii sa inteleaga mai bine cine sunt si ce stil abordez.

* Implementare uneui pagini de profil care se va deschide pentru fiecare utilizator artist. Pagina va contine campurile pentru descriere si biografie care nu sunt obligatorii de completat. Exista si validari pentru aceste campuri( intre 10 si 300 de caractere).


12. Ca artist, vreau ca muzica mea sa fie listata automat in catalogul principal pentru descoperirea artistilor underground, astfel incat sa ajung la un public mai larg.

* Implentarea unui view care sa contina toata pisele/albumele care trec de validare, organizate ca in requiremts ul de la user storyul 1.
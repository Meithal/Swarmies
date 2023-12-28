+++
title = 'Compte_rendu'
date = 2023-11-01T17:45:44+01:00
draft = false
+++

Ivo 1 nov
---

Ca ne sert a rien de declarer une cle de map en tant que 
`const` https://stackoverflow.com/questions/50593012/why-does-stdunordered-map-not-work-with-const-stdstring-key

Creer une interface revient a creer une "pure virtual" 
methode et les classes qui en heritent comme final.

Penser a deployer sur github pages https://gohugo.io/hosting-and-deployment/hosting-on-github/

Il est utile de mettre un texte dans le destructeur pour 
detecter les copies couteuses et les duplications dobjets s
dues a un oubli de reference.

11 decembre
---

Crée un projet sur Cmake afin de tout automatiser. Cela
fonctionne toujours, le lib est copiée dans unity et s'exécute.

Ajouté un use case avec criteres d'acceptabilité sur le taiga.

### Bloqueurs

On arrive pas a faire dialoguer XCode avec CmakeLists.txt
quand on ajoute des fichiers dans XCode a un projet ceux
ci ne sont pas ajoutés dans le cmakelists. Ajouter un fichier
doit alors se faire manuellement, mais il ne seront pas rangés
dans les répertoires "Sources" et "Headers". On repasse donc 
sur Clion.

Avec le build system de xcode, l'executable de test final n'est
pas dans le fichier auquel on s'attend et les tests n'arrivent
pas a trouver les bons fichiers d'asset : il faudrait 
mettre une commande de build qui le fasse avec une generator expression.

Bugs Unity dee composant Camera non trouvé et lié au terrain, peut être 
lié qu'on est passé à une nouvelle version d'unity.

lundi 11 decembre (semaine cours)
---

Ecrit un document de game design decrivant les differents
gameplays contenus dans le jeu, au vu du cours de game 
design de la matinée.

12 decembre
---

Deployement de ce projet hugo reussi.

Resolu le prebleme déxecution de test 
backend venant
du fait que le backend est devenu un sous
module.

Reussi a mettre en place un workflow visual 
studio code, probablement plus leger que
d'avoir rider et clion ouverts.

Resolu le bug de script de camera attaché par
erreur au game object Terrain.

### Bloqueurs

Le sass de pico ne build pas.

jeudi 21 decembre
---

On a voulu que le repo soit le seul objet dans le code
a posseder des pointeurs sur sa structure interne, du coup
quand on y ajoute un objet on ne prend que des r-value
refences

Du coup on a essayé de faire

```c
const T& add(const std::string &&name, T && object) {...code}
const T& add(const std::string &&name, T && object) = delete;
const T& add(const std::string &&name, T && object) = delete;

```

Mais a priori c'est pas nécessaire de delete explicitement.

En changeant juste la fonction pour n'accepter que des  &&
on a ce qu'on veut

```c
    struct Foo {
        std::string name;
        ~Foo() {
            std::cout << "destroy foo" << std::endl;
        }
    } a_jeter("non");

    auto repo = Swarmies::TRepository<Foo>();
    auto &f = repo.add(
            "toto", Foo{"toto"} // < r-value, ok, ce qu'on veut
            );
    repo.add("non", a_jeter); // < cette ligne la ne marche plus 
    // car c'est une l-value, c'est ce qu'on veut.
```

On est obligé d'utiliser le constructeur par aggregat,
car Foo("toto") genere une erreur

```
error: no matching conversion for functional-style cast from 'const char[5]' to 'Foo'
```

Pour lui Foo("toto") est un "functional-style cast" ?
cf. https://en.cppreference.com/w/cpp/language/explicit_cast
Dans notre cas precis a un seul argument, c'est equivalent
a un cast de type C.

Selon cette doc, une alternative serait d'écrire
``Foo({"toto"})`` ce qui est plus verbeux que la 
forme qu'on a.

Ensuite dans notre methode `add` de notre `TRepository`
on veut retourner une ref constante, et on ne peut
plus écrire ``map[name] = object`` car l'operateur
`[]` de unordered map a l'effet de bord de creer
une instance de l'objet si il ne la trouve pas
a l'adresse, specifiée, en apellant le constructeur
par copie de l'objet. En déclarant un tel
constructeur, on ne peut plus "aggregate initialiser"
notre objet (`Foo{"toto"}`) a moins d'écrire un
construteur pour. C'est assez fastidieux.

La solution est d'écrire
``map.emplace(name, std::move(object));`` quand on
veut insérer, et `map.at(name)` quand on veut
retourner, ce qui épargne toute construction
par surprise.

### Bloqueur
C'est bizarre que le destructeur de l'objet temporaire
que l'on move dans notre repo soit appellé
avant que l'on teste l'objet en question, avant le 1er assert,
en tout cas douteux.

En declarant nous meme un constructeur par dep pour mitiger
ca ``Foo(Foo&&other): name(other.name) {}``, ca enleve le
constructeur par agregat `Foo{"toto"}`.

Meme en ajoutant un tel constructeur, ca n'enleve pas
le souci de l'appel de destructeur, l'avantage de déclarer
un constructeur c'est que ca interdit la syntaxe

``auto foo2= repo.get("toto");``
et force le client a ecrire 
``auto& foo2= repo.get("toto");``

car ca supprime le constructeur par copie implicite.

Vendredi 22 decembre
---

On a ecrit un document decrivant l'architecture du game state
du jeu. On a aussi modifié notre repo et ajouté un test.


Jeudi 28 decembre
---

Difficultés: creeer un ordonanceur qui contienne tous les repository :
chaque repostory est de type template donc on ne peut pas juste
delarer `std::array<TRepository>`, du coup un tableau de quoi ? D'une
classe de base virtuelle?

Typiquement `std::array<TRepository<Monster|Weapon>>`

Ca me donne l'impression qu'on ne devrait avoir qu'un repository
unique de "GameObjects" et tenir un inventaire séparé de pointeurs
sur des meshes, textures, etc. à l'intérieur de ce repo.

Réorganisé l'arborescence de code de sorte à avoir le code framework
et le code spécifique à Swarmies séparé.

Ai du changer le cmakelist pour copier les assets car ce n'etait
pas implémenté assez solidement.
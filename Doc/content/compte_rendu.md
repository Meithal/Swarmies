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
declarer `std::array<TRepository>`, du coup un tableau de quoi ? D'une
classe de base virtuelle?

Typiquement `std::array<TRepository<Monster|Weapon>>`

Sur discord on m'a suggéré une classe de base virtuelle, un std::tuple,
des pointeurs sur de tels types.

Ca me donne l'impression qu'on ne devrait avoir qu'un repository
unique de "GameObjects" et tenir un inventaire séparé de pointeurs
sur des meshes, textures, etc. à l'intérieur de ce repo.

Réorganisé l'arborescence de code de sorte à avoir le code framework
et le code spécifique à Swarmies séparé.

Ai du changer le cmakelist pour copier les assets car ce n'etait
pas implémenté assez solidement.

Separé tags et relations, laissant l'ordonnanceur se charger 
de faire le liant.

Vendredi 29
---

Progrès dans l'ordonnanceur, abandonne idée d'avoir un
ordonnanceur `constexpr` avec instantiation directe. 
Amélioration usabilité XCode en categorisant
fichiers source selon les dossiers où ils sont.

Dimanche 31
---

Pas mal de reorganisations.

Mardi 2
---

Trouvé comment debuger code natif depuis rider.
https://forum.unity.com/threads/how-to-debug-a-native-c-plugin.428681/

On ne peut ``push_back`` un vecteur de C arrays.
Ils n'acceptent la copie, donc wrap dans une
struct pour que ca marche, sans avoir a faire
a std::array et l'interop C/C#.

Bien avancé dans load de mesh.

Mercredi 3 janvier 2024
---
Quelques soucis pour charger les modeles.obj par le plugin
natif pendant le runtime. Je ne sais pas où se situe la
racine a partir de laquelle ouvrir un fichier.

Piste : https://forum.unity.com/threads/plugins-and-resources-inside-package.730328/

Du coup on utilise le composant filesystem de c++17 pour prober l'environnement.

Ca me fait réaliser le problème d'un plugion natif : il a accès à tout
l'environnement (comme tout autre jeu c++), alors qu'un jeu unity pourrait
être sandboxé et plus facilement distribuable.

A implementé une fonction de comput de vertices pour pouvoir
allouer un tableau de taille fixe dans c sharp.

Jeudi 4 janvier
---
A etabli une fonction qui cree le tableau de vertices a partir
de la taille qu'on connait.

Reste a vraiment y mettre des vertices. Le probleme c'est
que le C# interop ne fonctionne qu'avec du code C, donc
on a du adapter notre vecteur du coté C++ de ne pas
utiliser de std::array.

Vendredi 5 janvier
---
Definit un nom au mesh.
Soiree: etudié l'interop managed/unmanaged de csharp.
https://learn.microsoft.com/en-us/dotnet/framework/interop/consuming-unmanaged-dll-functions
https://learn.microsoft.com/en-us/dotnet/framework/interop/passing-structures

Samedi 6
---
Suivi cours linkedin learning, a decouvert Fsharp pour manipuler megadonnee
Trouvé sur le site cmake https://www.kitware.com/create-dlls-on-windows-without-declspec-using-new-cmake-export-all-feature/
le projet root qui ravaille sur le giga et l'exadonnee https://root.cern.ch//

Dimanche 7
---

Surtout passé du temps à étudier F#, l'ecosystème microsoft et des videos youtube 
(casey muratori) plutot occupantes.

Mardi 9
---
Article interessant sur l'interop
https://learn.microsoft.com/en-us/dotnet/standard/native-interop/best-practices

Mercredi 10
---

Mis la glue swarmies dans le projet swarmies, de sorte la glu
puisse utiliser des choses specifiques a swarmies.

Jeudi 11
---
Reussi a lier des donnees de vertex entre back c++ et le
code unity

Vendredi
---
Lu le scripting api page pour savoir comment creer un mesh dans
unity. Reste a charger triangles et normales il semblerait.
Il n'y a pas de moyen simple dans unity de passer en mode
wireframe sans un shader.

Dimanche
---
Trouvé http://web.cse.ohio-state.edu/~shen.94/581/Site/Lab3_files/Labhelp_Obj_parser.htm
Continue le parseur obj pour l'afficher en jeu.

Samedi 20 janvier
---
cree un iterateur qui sort les indices des vertexes de nos face
depuis notre structure de faces. Appris que dans l'iterateur
end doit retourner l'indice un cran apres la fin.

On ne peut pas iterer a travers les vertices d'un mesh 
et les changer un par un, il faut les charger en un seul 
tenant. Il faut aussi apeller Clear() avant de set les indices.
Par exemple ceci ne fonctionne pas

```c
for (int i = 0; i < vertices.Length / 3; i++)
{
    mesh.vertices[i] = new Vector3(vertices[i * 3 + 0], vertices[i * 3 + 1], vertices[i * 3 + 2]);
}
```
mais cela fonctionne

```c

var vertices_p = new Vector3[vc];
for (int i = 0; i < vertices.Length / 3; i++)
{
    vertices_p[i] = new Vector3(vertices[i * 3 + 0], vertices[i * 3 + 1], vertices[i * 3 + 2]);
}
mesh.Clear();
mesh.vertices = vertices_p;
```

Il faut aussi set les indices d'abord, puis les triangles, chacun
en un seul tenant pour que le moteur vérifie la cohérence
des points donnés.

![Mesh sans les normales](</images/default_material.png>)

Pour les normales unity s'attend qu'à chaque vertex soit associé
une normale car c'est ainsi que les cartes graphiques fonctionnent

https://forum.unity.com/threads/why-are-normals-saved-on-a-per-vertex-basis.742184/#:~:text=To%20do%20triangle%20normal%20interpolation,and%20a%20lot%20more%20math.

Or le format `.obj` compresse les normales en un seul tableau,
c'est à nous de reproduire les vraies vertices à partir des
couplet cooordonées/normales fournies par le format .obj.

Cela est expliqué ici

https://blender.stackexchange.com/questions/63348/export-vertex-normals-into-obj-file

Quand on exporte il faut aussi demander à l'exporteur de trianguler
nos mesh, et toujours travailler en quans (beaucoup plus simple de 
faire des loop cuts ainsi et beaucoup plus maniables).

Pour les collisions, le plus simple serait d'avoir un
mesh de géométrie et un mesh de collision dans notre
.obj. On pourrait utiliser une convention de nommage
pour distinguer les deux au sein du fichier.

L'interet de `Vertex{1, 2}` est que si on a un objet
qui s'initialise avec plus d'un paramètre tels que
```c
struct Vertex {
    int coordinates_ref;
    int normal_ref;
};
```

On doit lui écrire un constructeur si on l'initialise
comme `Vertex(a[i][0], a[i][2])`. Alors qu'en 
l'initialisant `Vertex{a[i][0], a[i][2]}` on n'a
pas besoin de lui écrire un constructeur.

On a reussi a charger correctement un mesh en
creant un map (ordonné) de couplets vertice/normal

Notre iterateur de triangle renvoie une reference
a cette map a partir du hash qu'o obtient a partir
du couplet.

![Mesh avec les normales](</images/with_normals.png>)

Reste a comprendre pourquoi l'objet
est si petit par rapport a l'asset
importé via `.fbx`.

lundi 29 janvier
---
A eu reunion samedi en vocal.

Tous les problemes de camera 
https://blender.stackexchange.com/questions/3502/how-can-i-make-a-camera-the-active-one
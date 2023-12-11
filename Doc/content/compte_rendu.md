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
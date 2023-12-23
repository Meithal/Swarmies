+++
title = 'Architecture'
date = 2023-12-22T09:37:17+01:00
+++

On veut que tous les objets du jeu soient dans un repo.

On veut pouvoir faire des recherches dichotomiques dans 
ce repo sur n'importe quel critere (comme les colonnes avec 
index de SQL).

On veut pouvoir faire des insertions et suppression en O(log(N))
en utilisant des arbres AVL sur les differents criteres du repo
qui nous interesse. Ajouter ou supprimmer un item du repo
modifie le arbres de recherche associés.

La structure primaire est un vecteur 
qui fonctionne comme une queue, ou les insertions se font
cycliquement, a la fin, en modulo si taille < capacité, 
et les supressions ne font que marquer
la valeur comme supprimée. Dans la plupart des cas les 
insertions se font en O(1), et le pire en Omega(N).
Les suppressions sont toujours en 0(1).

Généralement on veut encourager le pooling d'objets en
demandant chaque objet de repo d'implementer une méthode `reset`
qui modifie son état interne de sorte à pouvoir être
réutilisé.

On veut que nos objets contiennent peu d'informations
et qu'un objet complexe se compose via un ECS. De la
sorte nos copies d'objets sont rapides.

On veut que les objets stockés dans le repo soient mutables.

Remarque : il pourrait être intéressant de voire les gains
de performance d'avoir des objets immutables car cela 
permet de nombreuses optimisations. Chaque mutation
creerait un nouvel objet dans le repo, a la maniere de Redux.

On pourrait aussi n'autoriser les mutations que depuis
des reducteurs et marquer les reducteurs comme `friend`
des repos, et seuls eux pourraient modifier le repo.

On veut que les objets retournés depuis le repo soient
des copies de sorte qu'on ne puisse pas modifier
le repo depuis l'exterieur.

Le repo n'admet d'updates que depuis des references 
de r-values (&&) de sorte a limiter le danger de modifier
un objet depuis l'exterieur du repo.

On veut que les relations entre objets soient stockés 
séparément (evite le probleme de la poule et de l'oeuf).

On veut que toutes les relations soit m-n (ManyToMany).
On peut simuler une relation 1-n, et tagant une cardinalité
comme "no_siblings", mais en interne elle sera stockée m-n.

On veut qu'à la suppression d'un objet, 
les relations qui lui sont associées le soient aussi,
et qu'en fonction de la cardinalité de l'objet
supprimé, d'autres le soient aussi, de proche en proche
(par exemple si l'objet est un objet "pere", que d'autres
le sont aussi, et ainsi de suite).

On doit pouvoir tagger les relations pour savoir
comment se comporter en cas de suppression, de mutation.
Comme dans SET_NULL, CASCADE de SQL.

Une relation cyclique pere-pere ne devrait pas
suprimmer l'autre par exemple. Donc le tag "isPere"
attribué par une relation d'apparentage devrait
l'emporter sur un tag "isEnfant" attribué
par une relation de filiation.

Une relation peut simplement demander de heriter
la position,rotation,scale de son parent.

A priori on ne veut pas contrôles que des
relations ne soient pas dupliquées, ou 
qu'il n'y a pas de relation cyclique,
mais on devrait pouvoir être tolérant
à ces cas particuliers. D'une part
on part du principe que le dev sait ce qu'il fait.
Da'utre part ces vérifications
auraient un coût au runtime; mais on peut
imaginer qu'en mode debug on le fasse.

Les suppressions se font dans un ordonanceur
dont quand un pere est supprimé il peut signaler
une demande de suppression de ses fils,
mais ensuite les enfants peuvent demander l'annulation
de la suppression.

C'est l'ordonnanceur qui donne la semantique
aux relations, les infos semantiques sont stockées
séparément des relations.
Les relations sont stockées séparement des objets.

Une fois que l'ordonanceur lance une suppression,
une modification ou tout processus necessitant
une modification de l'état du repo se fait de 
manière atomique.

L'ordonanceur gère tous les repos, et leur relations
et leur mutations.
Il n'est pas question ici d'horizontalité,
on est dans une structure relationelle.
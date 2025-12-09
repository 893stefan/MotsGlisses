Voici le détail de ces classes et de leurs responsabilités:

Classe Dictionnaire

Rôle : Gérer l'ensemble des mots valides du jeu.


Fonctionnalités clés : Elle doit charger le fichier MotsFrancais.txt, permettre le tri des mots (Tri Fusion ou Quick Sort) et surtout vérifier l'existence d'un mot via une recherche dichotomique récursive.


Classe Plateau

Rôle : Représenter la grille de jeu et la manipulation des lettres.

Fonctionnalités clés :

Générer la grille aléatoirement ou à partir d'un fichier CSV (Lettres.txt pour les poids/fréquences).

Recherche_Mot : Vérifier si un mot est formable sur le plateau (adjacence).


Maj_Plateau : Faire "glisser" les lettres vers le bas (comme un Puissance 4) après qu'un mot a été trouvé et retiré.


Sauvegarder l'état du plateau dans un fichier.

Classe Joueur

Rôle : Stocker les informations relatives à un participant.


Fonctionnalités clés : Gérer le nom, le score et la liste des mots trouvés par le joueur. Elle contient des méthodes pour ajouter un mot trouvé ou mettre à jour le score.

Classe Jeu

Rôle : Chef d'orchestre de la partie.

Fonctionnalités clés : Elle fait le lien entre le Dictionnaire, le Plateau et les Joueurs. Elle gère la boucle principale du jeu, le chronomètre (temps par tour et temps global) et l'alternance entre les joueurs.

Programme Principal (Main)

Rôle : Point d'entrée de l'application.

Fonctionnalités clés : Afficher le menu (Jouer, Options, Quitter) et lancer l'instance de la classe Jeu. L'interface doit être en console.

2. Pourquoi cette structure ?
Cette architecture n'est pas arbitraire, elle répond à plusieurs objectifs pédagogiques et techniques précis :

Application de la Programmation Orientée Objet (POO) : Le but explicite du projet est de "mettre en application tous les concepts vus en TD". Cette structure force l'encapsulation : le Plateau ne s'occupe que de la grille, le Dictionnaire que des mots, etc. Cela évite d'avoir tout le code dans un seul fichier géant.

Séparation des Responsabilités (Principe SRP) :

Logique de données vs Logique de jeu : Le Dictionnaire est une base de données passive (il vérifie juste si un mot existe). Le Jeu contient la logique temporelle (tours, chrono). Le Plateau contient la logique spatiale (mouvements, grille).

Cela permet de modifier la façon dont le plateau gère la gravité (le glissement) sans casser la gestion des scores du joueur.

Facilité de Test et de Maintenance : Le sujet impose des Tests Unitaires sur au moins 5 fonctions. En découpant le projet en classes distinctes (ex: tester la recherche dichotomique du dictionnaire indépendamment du reste), il est beaucoup plus facile de valider que chaque brique fonctionne correctement avant de tout assembler.

Performance algorithmique : La structure impose des algorithmes spécifiques pour des raisons de performance. Par exemple, séparer la classe Dictionnaire permet d'implémenter et d'isoler des tris complexes (QuickSort) et des recherches rapides (Dichotomie) nécessaires car le dictionnaire contient beaucoup de mots, ce qui serait lent avec une simple liste non triée.*

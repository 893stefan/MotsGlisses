using System;
using System.Collections.Generic;
using System.IO;

public class Plateau
{
    public char[,] Grille { get; private set; } = new char[0, 0];//Matrice de caractère pour notre Plateau
    public int Lignes { get; private set; }
    public int Colonnes { get; private set; }
    private readonly List<string> motsTrouves = new List<string>();
    private readonly Dictionary<char, int> poidsLettres = new Dictionary<char, int>();

    private static Random r = new Random(); // obligatoire : une seule instance random

    // CONSTRUCTEURS

    // Génération aléatoire du plateau à partir du fichier Lettres.txt
    public Plateau(string fichierLettres, int lignes, int colonnes)
    {
        Lignes = lignes;
        Colonnes = colonnes;
        Grille = new char[Lignes, Colonnes];

        ChargerAleatoire(fichierLettres);//Charge un plateau aléatoire
    }

    // Chargement d’un plateau déjà existant
    public Plateau(string fichierCSV)
    {
        ToRead(fichierCSV);//Notre fonction de lecture du fichier
    }

    // CHARGEMENT ALEATOIRE avec Lettres.txt
    private void ChargerAleatoire(string fichierLettres)
    {
        // Format du fichier : Lettre, Max, Poids

        //Création d'un dictionnaire pour stocker la quantité max de chaque lettres.
        Dictionary<char, int> stock = new Dictionary<char, int>();
        poidsLettres.Clear();//On s'assure d'éviter les doublons

        //File.ReadAllLines lit tout le fichier d'un coup et renvoie un tableau de lignes
        foreach (string ligneBrute in File.ReadAllLines(fichierLettres))
        {
            string ligne = ligneBrute.Trim();//nettoyage des espaces au debut et à la fin
            if (string.IsNullOrWhiteSpace(ligne))//On continue après les lignes null ou espace
                continue;

            // StringSplitOptions.RemoveEmptyEntries évite de créer des cases vides par exemple si on a "A,,10"
            string[] t = ligne.Split(',', StringSplitOptions.RemoveEmptyEntries);//découpage de la ligne avec la virgule comme séparateur
            if (t.Length < 3)// Sécurité : Si on a moins de 3 éléments (Lettre, Max, Poids) la ligne est invalide donc on l'ignore
                continue;

            //nettoyage et récupération lettre max et poids.
            char lettre = t[0].Trim().ToUpper()[0];
            int max = int.Parse(t[1].Trim());
            int poids = int.Parse(t[2].Trim());

            stock[lettre] = max;//on stocke la quantité demandé pour la génération du plateau
            poidsLettres[lettre] = poids;//On garde le poids des lettres pour plus tard, dans notre calcul du score
        }

        List<char> pool = new List<char>();//Le 'sac' qui va contenir toutes les lettres physiquemment disponible

        foreach (var kv in stock)//On parcours chaque entrée dispo
        {
            for (int i = 0; i < kv.Value; i++)//on boucle autant de fois que la valeur 'max' qui est kv.Value
                pool.Add(kv.Key);
        }

        // Mélange
        for (int i = pool.Count - 1; i > 0; i--)
        {
            int j = r.Next(i + 1);//notre élément aléatoire
            (pool[i], pool[j]) = (pool[j], pool[i]);//Echange sans variable temporaire (tuple swap)
        }

        int index = 0;//Notre index pour prendre les lettres dans pool une par une

        //On parcourt la matrice, on place la lettre courante du pool dans la case [i,j] et on incrémente l'index
        for (int i = 0; i < Lignes; i++)
            for (int j = 0; j < Colonnes; j++)
                Grille[i, j] = pool[index++];
    }

    // SAUVEGARDE DU PLATEAU
    public void ToFile(string nom)
    {
        // On ouvre un flux d'écriture (StreamWriter) vers le fichier donné par nom.
        // Le mot-clé using est important car il garantit que le fichier sera correctement fermé et sauvegardé à la fin du bloc même si il  y a un crash
        using StreamWriter sw = new StreamWriter(nom);

        for (int i = 0; i < Lignes; i++)// On boucle sur chaque ligne de la matrice du plateau (de 0 à Lignes-1).
        {
            string ligne = "";// On prépare une chaîne de caractères vide qui va contenir toute la ligne actuelle. Elle est réinitialisée à chaque tour de la boucle i
            for (int j = 0; j < Colonnes; j++)
            {
                ligne += Grille[i, j];
                if (j < Colonnes - 1)
                    ligne += ",";//On ajoute une virgule si c'est pas la dernière colonne
            }
            sw.WriteLine(ligne);//On ecrit dans le fichier et ajoute le saut à la ligne
        }
    }

    // CHARGEMENT D’UN PLATEAU CSV
    public void ToRead(string nom)
    {
        string[] lignes = File.ReadAllLines(nom);//lecture du fichier comme ci-dessus
        Lignes = lignes.Length;
        Colonnes = lignes[0].Split(',').Length;// On compte le nombre de colonnes en découpant la première ligne

        Grille = new char[Lignes, Colonnes];// On recrée la matrice vide avec les nouvelles dimensions adaptées au fichier

        for (int i = 0; i < Lignes; i++)
        {
            string[] t = lignes[i].Split(',');// On découpe la ligne actuelle en un tableau de petits textes
            for (int j = 0; j < Colonnes; j++)
                Grille[i, j] = t[j][0];// On récupère le premier caractère de la chaîne et on le met dans la grille
        }
    }

    // RECHERCHE DE MOT (chemin avec virages)
    public List<(int x, int y)>? Recherche_Mot(string mot)//? implique que ca Peut renvoyer un null
    {
        mot = mot.ToLower().Trim();

        if (mot.Length == 0)// Sécurité : si le mot est vide, on arrête tout de suite
            return null;

        List<(int x, int y)> chemin = new List<(int x, int y)>();// Création de la liste qui va stocker le chemin (les cases utilisées)
        int baseRow = Lignes - 1;// Le départ se fait depuis la "base" (la dernière ligne)

        // On parcourt la base du plateau
        for (int col = 0; col < Colonnes; col++)
        {
            // On veut la première lettre (insensible à la casse)
            if (char.ToLower(Grille[baseRow, col]) == mot[0])
            {
                bool[,] visited = new bool[Lignes, Colonnes];// On prépare une matrice de booléens pour se souvenir des cases déjà visitées(interdiction de repasser deux fois sur un même mot)
                chemin.Clear();

                if (RechRec(mot, 0, baseRow, col, visited, chemin))//Notre appel recursif expliqué ci-dessous
                {
                    // On renvoie une COPIE du chemin trouvé
                    return new List<(int, int)>(chemin);
                }
            }
        }

        return null;
    }

    // Récursif : on peut aller en HAUT, GAUCHE, DROITE
    private bool RechRec(string mot, int index, int x, int y, bool[,] visited, List<(int x, int y)> chemin)
    {
        // Si on a déjà consommé toutes les lettres → succès
        if (index == mot.Length)
            return true;

        // Sortie de plateau
        if (x < 0 || y < 0 || x >= Lignes || y >= Colonnes)
            return false;

        // Case déjà utilisée dans ce mot → on évite les boucles
        if (visited[x, y])
            return false;

        // Lettre différente
        if (char.ToLower(Grille[x, y]) != mot[index])
            return false;

        // On marque la case et on l’ajoute au chemin
        visited[x, y] = true;
        chemin.Add((x, y));

        // Si on vient de placer la dernière lettre : mot trouvé
        if (index == mot.Length - 1)
            return true;

        // Mouvements possibles : haut, gauche, droite + diagonales haut-gauche / haut-droite
        int[,] moves = new int[5, 2] { { -1, 0 }, { 0, -1 }, { 0, 1 }, { -1, -1 }, { -1, 1 } };

        for (int k = 0; k < 5; k++)
        {
            int nx = x + moves[k, 0];
            int ny = y + moves[k, 1];

            if (RechRec(mot, index + 1, nx, ny, visited, chemin))
                return true;
        }

        // On enlève la case du chemin et on la "démarque"
        chemin.RemoveAt(chemin.Count - 1);
        visited[x, y] = false;

        return false;
    }

    // MISE À JOUR DU PLATEAU (PUISSANCE 4/Gravité)
    // coords = liste des (x,y) trouvés

    public void Maj_Plateau(List<(int x, int y)> coords)
    {
        // Parcours colonne par colonne
        for (int col = 0; col < Colonnes; col++)
        {
            // Étape 1 : récupérer les lignes à supprimer dans cette colonne
            // (uniquement avec List<int>, CM6)
            List<int> lignesASuppr = new List<int>();

            for (int i = 0; i < coords.Count; i++)
            {
                if (coords[i].y == col)
                    lignesASuppr.Add(coords[i].x);
            }

            // Si aucune suppression cette colonne alors on passe
            if (lignesASuppr.Count == 0)
                continue;

            // Étape 2 : on construit la nouvelle colonne tassée vers le bas
            List<char> nouvelle = new List<char>();

            // On lit du bas vers le haut comme dans Puissance 4
            for (int ligne = Lignes - 1; ligne >= 0; ligne--)
            {
                bool doitSupprimer = false;

                // On check si cette ligne doit être supprimée
                for (int k = 0; k < lignesASuppr.Count; k++)
                {
                    if (ligne == lignesASuppr[k])
                    {
                        doitSupprimer = true;
                        break;
                    }
                }

                if (!doitSupprimer)
                    nouvelle.Add(Grille[ligne, col]);
            }

            // Étape 3 : on complete avec des cases vides jusqu’à Lignes
            while (nouvelle.Count < Lignes)
                nouvelle.Add(' ');

            // Étape 4 : on réécrit dans la grille (toujours de bas en haut)
            int index = 0;
            for (int ligne = Lignes - 1; ligne >= 0; ligne--)
            {
                Grille[ligne, col] = nouvelle[index];
                index++;
            }
        }
    }

    public void Add_Mot(string mot)
    {
        if (string.IsNullOrWhiteSpace(mot))//Sécurité comme partout
            return;

        string nettoye = mot.Trim().ToLower();//normalisation comme partout

        if (!motsTrouves.Contains(nettoye))//vérif des doublons puis ajoute
            motsTrouves.Add(nettoye);
    }

    public bool Contient(string mot)
    {
        if (string.IsNullOrWhiteSpace(mot))//sécurité
            return false;

        return Recherche_Mot(mot) != null;//si null alors mot pas dans tableau donc on renvoit False sinon True
    }

// TOSTRING
    public override string ToString()
    {
        //Notre affiche
        string s = "";

        for (int i = 0; i < Lignes; i++)
        {
            for (int j = 0; j < Colonnes; j++)
            {
                s += Grille[i, j] + " ";
            }
            s += "\n";
        }
        return s;
    }

    public int ScorePourChemin(List<(int x, int y)> coords)
    {
        //Notre équation: la longueur + bonus des poids des lettres dans le fichier
        int score = 0;
        foreach (var (x, y) in coords)
        {
            char c = char.ToUpper(Grille[x, y]);
            if (poidsLettres.TryGetValue(c, out int p))
                score += p;
        }

        return score;
    }
}

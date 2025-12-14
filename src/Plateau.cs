using System;
using System.Collections.Generic;
using System.IO;

public class Plateau
{
    public char[,] Grille { get; private set; } = new char[0, 0];
    public int Lignes { get; private set; }
    public int Colonnes { get; private set; }
    private readonly List<string> motsTrouves = new List<string>();
    private readonly Dictionary<char, int> poidsLettres = new Dictionary<char, int>();

    private static Random r = new Random(); // obligatoire : une seule instance

    // -------------------------------------------------------
    // CONSTRUCTEURS
    // -------------------------------------------------------

    // Génération aléatoire du plateau à partir du fichier Lettres.txt
    public Plateau(string fichierLettres, int lignes, int colonnes)
    {
        Lignes = lignes;
        Colonnes = colonnes;
        Grille = new char[Lignes, Colonnes];

        ChargerAleatoire(fichierLettres);
    }

    // Chargement d’un plateau déjà existant
    public Plateau(string fichierCSV)
    {
        ToRead(fichierCSV);
    }

    // -------------------------------------------------------
    // CHARGEMENT ALEATOIRE avec Lettres.txt
    // -------------------------------------------------------
    private void ChargerAleatoire(string fichierLettres)
    {
        // Format du fichier : Lettre, Max, Poids (lettre,max,poids)
        Dictionary<char, int> stock = new Dictionary<char, int>();
        poidsLettres.Clear();

        foreach (string ligneBrute in File.ReadAllLines(fichierLettres))
        {
            string ligne = ligneBrute.Trim();
            if (string.IsNullOrWhiteSpace(ligne))
                continue;

            string[] t = ligne.Split(',', StringSplitOptions.RemoveEmptyEntries);
            if (t.Length < 3)
                continue;

            char lettre = t[0].Trim().ToUpper()[0];
            int max = int.Parse(t[1].Trim());
            int poids = int.Parse(t[2].Trim());

            stock[lettre] = max;
            poidsLettres[lettre] = poids;
        }

        List<char> pool = new List<char>();

        foreach (var kv in stock)
        {
            for (int i = 0; i < kv.Value; i++)
                pool.Add(kv.Key);
        }

        // Mélange
        for (int i = pool.Count - 1; i > 0; i--)
        {
            int j = r.Next(i + 1);
            (pool[i], pool[j]) = (pool[j], pool[i]);
        }

        int index = 0;

        for (int i = 0; i < Lignes; i++)
            for (int j = 0; j < Colonnes; j++)
                Grille[i, j] = pool[index++];
    }

    // -------------------------------------------------------
    // SAUVEGARDE DU PLATEAU
    // -------------------------------------------------------
    public void ToFile(string nom)
    {
        using StreamWriter sw = new StreamWriter(nom);

        for (int i = 0; i < Lignes; i++)
        {
            string ligne = "";
            for (int j = 0; j < Colonnes; j++)
            {
                ligne += Grille[i, j];
                if (j < Colonnes - 1)
                    ligne += ",";
            }
            sw.WriteLine(ligne);
        }
    }

    // -------------------------------------------------------
    // CHARGEMENT D’UN PLATEAU CSV
    // -------------------------------------------------------
    public void ToRead(string nom)
    {
        string[] lignes = File.ReadAllLines(nom);
        Lignes = lignes.Length;
        Colonnes = lignes[0].Split(',').Length;

        Grille = new char[Lignes, Colonnes];

        for (int i = 0; i < Lignes; i++)
        {
            string[] t = lignes[i].Split(',');
            for (int j = 0; j < Colonnes; j++)
                Grille[i, j] = t[j][0];
        }
    }

        // -------------------------------------------------------
    // RECHERCHE DE MOT (chemin avec virages)
    // -------------------------------------------------------
    public List<(int x, int y)>? Recherche_Mot(string mot)
    {
        mot = mot.ToLower().Trim();

        if (mot.Length == 0)
            return null;

        List<(int x, int y)> chemin = new List<(int x, int y)>();
        int baseRow = Lignes - 1;

        // On parcourt la base du plateau
        for (int col = 0; col < Colonnes; col++)
        {
            // On veut la première lettre (insensible à la casse)
            if (char.ToLower(Grille[baseRow, col]) == mot[0])
            {
                bool[,] visited = new bool[Lignes, Colonnes];
                chemin.Clear();

                if (RechRec(mot, 0, baseRow, col, visited, chemin))
                {
                    // On renvoie une COPIE du chemin trouvé
                    return new List<(int, int)>(chemin);
                }
            }
        }

        return null;
    }

    // -------------------------------------------------------
    // Récursif : on peut aller en HAUT, GAUCHE, DROITE
    // -------------------------------------------------------
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

        // Mouvements possibles : haut, gauche, droite
        int[,] moves = new int[3, 2] { { -1, 0 }, { 0, -1 }, { 0, 1 } };

        for (int k = 0; k < 3; k++)
        {
            int nx = x + moves[k, 0];
            int ny = y + moves[k, 1];

            if (RechRec(mot, index + 1, nx, ny, visited, chemin))
                return true;
        }

        // Backtracking : on enlève la case du chemin et on la "démarque"
        chemin.RemoveAt(chemin.Count - 1);
        visited[x, y] = false;

        return false;
    }

    // -------------------------------------------------------
    // MISE À JOUR DU PLATEAU (PUISSANCE 4)
    // coords = liste des (x,y) trouvés
    // -------------------------------------------------------

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

            // Si aucune suppression cette colonne → on passe
            if (lignesASuppr.Count == 0)
                continue;

            // Étape 2 : construire la nouvelle colonne tassée vers le bas
            List<char> nouvelle = new List<char>();

            // On lit du bas vers le haut comme dans Puissance 4
            for (int ligne = Lignes - 1; ligne >= 0; ligne--)
            {
                bool doitSupprimer = false;

                // Vérifier si cette ligne doit être supprimée
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

            // Étape 3 : compléter avec des cases vides jusqu’à Lignes
            while (nouvelle.Count < Lignes)
                nouvelle.Add(' ');

            // Étape 4 : réécriture dans la grille (toujours bas → haut)
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
        if (string.IsNullOrWhiteSpace(mot))
            return;

        string nettoye = mot.Trim().ToLower();

        if (!motsTrouves.Contains(nettoye))
            motsTrouves.Add(nettoye);
    }

    public bool Contient(string mot)
    {
        if (string.IsNullOrWhiteSpace(mot))
            return false;

        return Recherche_Mot(mot) != null;
    }

//------------------------------------------------
// TOSTRING
//------------------------------------------------
    public override string ToString()
    {
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

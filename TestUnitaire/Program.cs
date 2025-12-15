using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

// Vérifs minimales et déterministes sur cinq fonctions clés de MotsGlisses.
namespace TestUnitaire;

internal static class Program
{
    private static int _echecs;

    private static void Main()
    {
        string racine = TrouverRacine();
        string cheminDico = Path.Combine(racine, "src", "txt-files", "Mots_Francais.txt");
        string cheminLettres = Path.Combine(racine, "src", "txt-files", "Lettre.txt");

        Console.WriteLine($"Racine: {racine}");

        Test("Dictionnaire.RechDichoRecursif", () => TestDictionnaire(cheminDico));
        Test("Plateau.Recherche_Mot", () => TestRechercheMot());
        Test("Plateau.Maj_Plateau", () => TestMajPlateau());
        Test("Joueur.Add_Mot / Contient", TestJoueur);
        Test("Plateau.ScorePourChemin", () => TestScorePourChemin(cheminLettres));

        Console.WriteLine($"\nTerminé. Échecs: {_echecs}");
        if (_echecs > 0) Environment.Exit(1);
    }

    private static void Test(string nomTest, Action verification)
    {
        try
        {
            verification();
            Console.WriteLine($"[OK]   {nomTest}");
        }
        catch (Exception ex)
        {
            _echecs++;
            Console.WriteLine($"[FAIL] {nomTest} -> {ex.Message}");
        }
    }

    private static void TestDictionnaire(string cheminDico)
    {
        var dico = new Dictionnaire(cheminDico);
        // Prend un mot connu (premier jeton du fichier) et un mot invalide.
        var premiereLigne = File.ReadLines(cheminDico).FirstOrDefault() ?? "";
        var motConnu = premiereLigne.Split(' ', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault()?.ToLower();
        if (string.IsNullOrWhiteSpace(motConnu))
            throw new Exception("Fichier dictionnaire vide");

        Assert(dico.RechDichoRecursif(motConnu), "Mot valide non trouvé");
        Assert(!dico.RechDichoRecursif("mot_inexistant_zzz"), "Mot invalide trouvé");
    }

    private static void TestRechercheMot()
    {
        // Grille 3x3 simple, mot "ghi" posé sur la base.
        string tmp = Path.GetTempFileName();
        File.WriteAllLines(tmp, new[] { "A,B,C", "D,E,F", "G,H,I" });

        var plateau = new Plateau(tmp);
        var chemin = plateau.Recherche_Mot("ghi");
        Assert(chemin != null, "Recherche_Mot n'a pas trouvé 'ghi'");
        Assert(chemin!.Count == 3, "Chemin longueur inattendue");
        File.Delete(tmp);
    }

    private static void TestMajPlateau()
    {
        string tmp = Path.GetTempFileName();
        File.WriteAllLines(tmp, new[] { "A,B,C", "D,E,F", "G,H,I" });
        var plateau = new Plateau(tmp);
        var coords = new List<(int x, int y)> { (1, 1), (2, 1) }; // E et H
        plateau.Maj_Plateau(coords);
        Assert(plateau.Grille[2, 1] == 'B' && plateau.Grille[1, 1] == ' ' && plateau.Grille[0, 1] == ' ',
            "Maj_Plateau n'a pas tassé correctement la colonne 1 (attendu B en bas)");
        File.Delete(tmp);
    }

    private static void TestJoueur()
    {
        var joueur = new Joueur("Test");
        joueur.Add_Mot("Mot");
        joueur.Add_Mot("mot"); // doublon insensible à la casse
        Assert(joueur.Contient("mot"), "Mot devrait être présent");
        Assert(joueur.NbMotsTrouves == 1, "NbMotsTrouves devrait être 1");
    }

    private static void TestScorePourChemin(string cheminLettres)
    {
        var poids = ChargerPoids(cheminLettres);
        var plateau = new Plateau(cheminLettres, 4, 4);
        var coords = new List<(int, int)> { (0, 0), (0, 1), (0, 2) };
        int attendu = coords.Sum(c =>
        {
            char lettre = char.ToUpper(plateau.Grille[c.Item1, c.Item2]);
            return poids.TryGetValue(lettre, out int val) ? val : 0;
        });
        int obtenu = plateau.ScorePourChemin(coords);
        Assert(obtenu == attendu, $"Score attendu {attendu}, obtenu {obtenu}");
    }

    private static Dictionary<char, int> ChargerPoids(string fichierLettres)
    {
        var poids = new Dictionary<char, int>();
        foreach (var ligne in File.ReadLines(fichierLettres))
        {
            var morceaux = ligne.Split(',', StringSplitOptions.RemoveEmptyEntries);
            if (morceaux.Length >= 3)
            {
                char lettre = morceaux[0].Trim().ToUpper()[0];
                poids[lettre] = int.Parse(morceaux[2]);
            }
        }
        return poids;
    }

    private static string TrouverRacine()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir != null && !File.Exists(Path.Combine(dir.FullName, "MotsGlisses.csproj")))
            dir = dir.Parent;
        if (dir == null)
            throw new DirectoryNotFoundException("Racine MotsGlisses introuvable");
        return dir.FullName;
    }

    private static void Assert(bool condition, string message)
    {
        if (!condition) throw new Exception(message);
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

public class Jeu
{
    // -------------------------
    // ATTRIBUTS
    // -------------------------
    private Joueur joueur1;
    private Joueur joueur2;

    private Dictionnaire dico;
    private Plateau plateau;

    private int tourActuel; // 1 = joueur1, 2 = joueur2
    private const int TempsTourSec = 40;

    // -------------------------
    // CONSTRUCTEUR
    // -------------------------
    public Jeu(string fichierDico, string fichierLettres, int lignes, int colonnes)
    {
        dico = new Dictionnaire(fichierDico);
        plateau = new Plateau(fichierLettres, lignes, colonnes);

        Console.Write("Nom joueur 1 : ");
        joueur1 = new Joueur(LireNom("Joueur 1"));

        Console.Write("Nom joueur 2 : ");
        joueur2 = new Joueur(LireNom("Joueur 2"));

        tourActuel = 1;
    }

    // -------------------------
    // OBTENIR LE JOUEUR DU TOUR
    // -------------------------
    private Joueur JoueurActuel()
    {
        if (tourActuel == 1)
            return joueur1;
        return joueur2;
    }

    // -------------------------
    // ALTERNE LES TOURS
    // -------------------------
    private void ChangerTour()
    {
        if (tourActuel == 1) tourActuel = 2;
        else tourActuel = 1;
    }

    // -------------------------
    // LANCEMENT DU JEU
    // -------------------------
    public void Lancer()
    {
        Console.Clear();
        bool continuer = true;

        while (continuer)
        {
            Console.WriteLine("\nPlateau actuel :");
            Console.WriteLine(plateau.ToString());

            Joueur j = JoueurActuel();
            Console.WriteLine($"Tour de : {j.Nom} (score {j.Score})");

            string prompt = "Entrez un mot (ou STOP pour quitter) : ";
            string? entree = LireMotAvecChrono(TempsTourSec, prompt);

            if (entree == null)
            {
                Console.WriteLine("\nTemps écoulé ! Tour perdu.");
                Console.WriteLine("Appuyez sur une touche pour passer au tour suivant...");
                Console.ReadKey(intercept: true);
                Console.Clear();
                ChangerTour();
                continue;
            }

            string mot = entree.Trim().ToLower();

            Console.Clear();

            if (mot == "stop")
                continuer = false;

            else if (!dico.RechDichoRecursif(mot))
            {
                Console.WriteLine("Mot invalide !");
            }
            else
            {
                // mot trouvé → on cherche dans le plateau
                var res = plateau.Recherche_Mot(mot);

                if (res == null)
                {
                    Console.WriteLine("Mot NON présent sur le plateau !");
                }
                else
                {
                    Console.WriteLine("Mot trouvé !");
                    int poids = plateau.ScorePourChemin(res);
                    int points = mot.Length + poids;
                    j.AjouterScore(points);
                    Console.WriteLine($"+{points} points (longueur {mot.Length} + poids lettres {poids})");
                    j.IncrementeMots();

                    plateau.Maj_Plateau(res);
                }
            }

            ChangerTour();
        }

        FinDuJeu();
    }

    // -------------------------
    // AFFICHER LE GAGNANT
    // -------------------------
    private void FinDuJeu()
    {
        Console.WriteLine("\n=== Fin du jeu ===");
        Console.WriteLine(joueur1.ToString());
        Console.WriteLine(joueur2.ToString());

        if (joueur1.Score > joueur2.Score)
            Console.WriteLine($"\nGagnant (meilleur score) : {joueur1.Nom} avec {joueur1.Score} points");
        else if (joueur2.Score > joueur1.Score)
            Console.WriteLine($"\nGagnant (meilleur score) : {joueur2.Nom} avec {joueur2.Score} points");
        else
            Console.WriteLine("\nÉgalité !");
    }

    private string LireNom(string defaut)
    {
        string? lu = Console.ReadLine();
        return string.IsNullOrWhiteSpace(lu) ? defaut : lu;
    }

    private string? LireMotAvecChrono(int secondes, string prompt)
    {
        StringBuilder sb = new StringBuilder();
        DateTime limite = DateTime.UtcNow.AddSeconds(secondes);
        int lastLineLength = 0;

        RedessinerLigne(prompt, sb.ToString(), secondes, ref lastLineLength);

        while (DateTime.UtcNow < limite)
        {
            while (Console.KeyAvailable)
            {
                ConsoleKeyInfo key = Console.ReadKey(intercept: true);

                if (key.Key == ConsoleKey.Enter)
                {
                    Console.WriteLine();
                    return sb.ToString();
                }
                else if (key.Key == ConsoleKey.Backspace)
                {
                    if (sb.Length > 0)
                    {
                        sb.Length--;
                        Console.Write("\b \b");
                    }
                }
                else
                {
                    sb.Append(key.KeyChar);
                }

                RedessinerLigne(prompt, sb.ToString(), Restant(limite), ref lastLineLength);
            }

            Thread.Sleep(50);
            RedessinerLigne(prompt, sb.ToString(), Restant(limite), ref lastLineLength);
        }

        Console.WriteLine();
        return null;
    }

    private int Restant(DateTime limite) =>
        Math.Max(0, (int)Math.Ceiling((limite - DateTime.UtcNow).TotalSeconds));

    private void RedessinerLigne(string prompt, string saisie, int secondesRestantes, ref int lastLineLength)
    {
        string ligne = $"{prompt}{saisie} [{secondesRestantes}s]";
        int padding = Math.Max(0, lastLineLength - ligne.Length);
        Console.Write($"\r{ligne}{new string(' ', padding)}");
        lastLineLength = ligne.Length;
    }
}

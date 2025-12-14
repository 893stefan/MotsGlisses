using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

public class Jeu
{
    private const string Separator = "-----";
    private const int TempsPartieSec = 400;
    private readonly int tempsTourSec;

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
    public Jeu(string fichierDico, string fichierLettres, int lignes, int colonnes, int tempsTourSec, string? fichierPlateauCsv = null)
    {
        dico = new Dictionnaire(fichierDico);
        plateau = fichierPlateauCsv == null ? new Plateau(fichierLettres, lignes, colonnes) : new Plateau(fichierPlateauCsv);
        this.tempsTourSec = tempsTourSec > 0 ? tempsTourSec : 40;

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
        DateTime limitePartie = DateTime.UtcNow.AddSeconds(TempsPartieSec);

        while (continuer && DateTime.UtcNow < limitePartie)
        {
            Joueur j = JoueurActuel();
            Console.Write("Tour de : ");
            AfficherNomCouleur(j);
            Console.WriteLine($" (score {j.Score})");
            Console.WriteLine($" {Separator} ");

            Console.Write(plateau.ToString());
            Console.WriteLine($" {Separator} ");

            string prompt = "Entrez un mot (ou STOP pour quitter) : ";
            string? entree = LireMotAvecChrono(tempsTourSec, prompt);

            if (DateTime.UtcNow >= limitePartie)
                break;

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

            if (DateTime.UtcNow >= limitePartie)
                break;

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
                    plateau.Add_Mot(mot);
                    j.AjouterScore(points);
                    Console.WriteLine($"+{points} points (longueur {mot.Length} + poids lettres {poids})");
                    j.Add_Mot(mot);

                    plateau.Maj_Plateau(res);
                }
            }

            Console.WriteLine($" {Separator} ");
            ChangerTour();
        }

        Console.WriteLine("\nTemps de partie écoulé. Fin de partie.");
        FinDuJeu();
    }

    // -------------------------
    // AFFICHER LE GAGNANT
    // -------------------------
    private void FinDuJeu()
    {
        Console.WriteLine("\n=== Fin du jeu ===");
        AfficherResumeJoueur(joueur1);
        AfficherResumeJoueur(joueur2);

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
        int lastPromptLength = 0;
        int lastTimerLength = 0;
        int baseTop = Console.CursorTop;

        RedessinerLigne(prompt, sb.ToString(), secondes, baseTop, ref lastPromptLength, ref lastTimerLength);

        while (DateTime.UtcNow < limite)
        {
            while (Console.KeyAvailable)
            {
                ConsoleKeyInfo key = Console.ReadKey(intercept: true);

                if (key.Key == ConsoleKey.Enter)
                {
                    Console.SetCursorPosition(0, baseTop + 2);
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

                RedessinerLigne(prompt, sb.ToString(), Restant(limite), baseTop, ref lastPromptLength, ref lastTimerLength);
            }

            Thread.Sleep(50);
            RedessinerLigne(prompt, sb.ToString(), Restant(limite), baseTop, ref lastPromptLength, ref lastTimerLength);
        }

        Console.SetCursorPosition(0, baseTop + 2);
        Console.WriteLine();
        return null;
    }

    private int Restant(DateTime limite) =>
        Math.Max(0, (int)Math.Ceiling((limite - DateTime.UtcNow).TotalSeconds));

    private void AfficherNomCouleur(Joueur joueur)
    {
        ConsoleColor color = joueur == joueur1 ? ConsoleColor.Blue : ConsoleColor.Red;
        Console.ForegroundColor = color;
        Console.Write(joueur.Nom);
        Console.ResetColor();
    }

    private void AfficherResumeJoueur(Joueur joueur)
    {
        Console.Write("Joueur : ");
        AfficherNomCouleur(joueur);
        Console.WriteLine($" | Score : {joueur.Score} | Mots trouvés : {joueur.NbMotsTrouves}");
    }

    private void RedessinerLigne(string prompt, string saisie, int secondesRestantes, int baseTop, ref int lastPromptLength, ref int lastTimerLength)
    {
        Console.SetCursorPosition(0, baseTop);
        string timerLine = $"Temps restant : {secondesRestantes}s";
        int timerPadding = Math.Max(0, lastTimerLength - timerLine.Length);
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write($"{timerLine}{new string(' ', timerPadding)}");
        Console.ResetColor();
        lastTimerLength = timerLine.Length;

        Console.SetCursorPosition(0, baseTop + 1);
        string ligne = $"{prompt}{saisie}";
        int padding = Math.Max(0, lastPromptLength - ligne.Length);
        Console.Write($"{ligne}{new string(' ', padding)}");
        lastPromptLength = ligne.Length;
        Console.SetCursorPosition(ligne.Length, baseTop + 1);
    }
}

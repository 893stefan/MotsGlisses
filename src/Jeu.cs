using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

public class Jeu
{
    private const string Separator = "-----";
    private readonly int tempsTourSec;
    private readonly int tempsPartieSec;


    // ATTRIBUTS

    private Joueur joueur1;
    private Joueur joueur2;

    private Dictionnaire dico;
    private Plateau plateau;

    private int tourActuel; // 1 = joueur1, 2 = joueur2


    // CONSTRUCTEUR

    // renvoie une instance d'un jeu avec une grille d'une certaine dim, une duree par defaut, un dictionnaire fourni, etc..

    public Jeu(string fichierDico, string fichierLettres, int lignes, int colonnes, int tempsTourSec, string? fichierPlateauCsv = null, int tempsPartieSec = 400)
    {
        dico = new Dictionnaire(fichierDico);
        plateau = fichierPlateauCsv == null ? new Plateau(fichierLettres, lignes, colonnes) : new Plateau(fichierPlateauCsv);
        this.tempsTourSec = tempsTourSec > 0 ? tempsTourSec : 40;
        this.tempsPartieSec = tempsPartieSec > 0 ? tempsPartieSec : 400;

        Console.Write("Nom joueur 1 : ");
        joueur1 = new Joueur(LireNom("Joueur 1"));

        Console.Write("Nom joueur 2 : ");
        joueur2 = new Joueur(LireNom("Joueur 2"));

        tourActuel = 1;
    }


    // OBTENIR LE JOUEUR DU TOUR

    private Joueur JoueurActuel()
    {
        if (tourActuel == 1)
            return joueur1;
        return joueur2;
    }


    // ALTERNE LES TOURS

    private void ChangerTour()
    {
        if (tourActuel == 1) tourActuel = 2;
        else tourActuel = 1;
    }


    // LANCEMENT DU JEU

    public void Lancer()
    {
        Console.Clear();
        bool continuer = true;
        DateTime limitePartie = DateTime.UtcNow.AddSeconds(tempsPartieSec);

        while (continuer && DateTime.UtcNow < limitePartie)
        {
            // boucle tant que l'heure actuelle est inférieure a l'h de fin de partie, et tant que continuer est vrai
            Joueur j = JoueurActuel();
            Console.Write("Tour de : ");
            AfficherNomCouleur(j);
            Console.WriteLine($" (score {j.Score})");
            Console.WriteLine($" {Separator} ");

            Console.Write(plateau.ToString());
            Console.WriteLine($" {Separator} ");

            string prompt = "Entrez un mot (ou STOP pour quitter) : ";
            string? entree = LireMotAvecChrono(tempsTourSec, prompt, limitePartie);

            // ci-dessus : affichage du nom en couleur, du score et du plateau, le tout séparés par question d'esthétisme

            if (DateTime.UtcNow >= limitePartie)
                break;

            // check l'h en milieu de boucle pour s'assurer de finir a temps

            if (entree == null)
            {
                // si le joueur n'entre rien, son tour est passé et on attend l'interaction du joueur suivant
                Console.WriteLine("\nTemps écoulé ! Tour perdu.");
                Console.WriteLine("Appuyez sur une touche pour passer au tour suivant...");
                Console.ReadKey(intercept: true);
                Console.Clear();
                ChangerTour();
                continue;
            }

            string mot = entree.Trim().ToLower();

            Console.Clear();
            // pour ne pas encombrer le terminal

            if (DateTime.UtcNow >= limitePartie)
                break;
            // re-check de l'h pour pas dépasser

            if (mot == "stop")
                continuer = false;
            // break la boucle si le joueur insert "stop" et ainsi met fin à la partie en affichant le scoreboard final

            else if (!dico.RechDichoRecursif(mot))
            {
                Console.WriteLine("Mot invalide !");
            }
            else if (j.Contient(mot))
            {
                Console.WriteLine("Mot déjà utilisé par ce joueur !");
            }
            else
            {
                // mot trouvé => on cherche dans le plateau
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


    // AFFICHER LE GAGNANT

    private void FinDuJeu()
    {
        // affichage du scoreboard de fin
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
        // prend l'entrée des noms des users
        string? lu = Console.ReadLine();
        return string.IsNullOrWhiteSpace(lu) ? defaut : lu;
    }

    private string? LireMotAvecChrono(int secondes, string prompt, DateTime limitePartie)
    {
        StringBuilder sb = new StringBuilder();
        // stocke la saisie de l'user
        DateTime limite = DateTime.UtcNow.AddSeconds(secondes);
        // heure de fin du chrono local
        int lastPromptLength = 0;
        int lastTimerLength = 0;
        int baseTop = Console.CursorTop;
        // pour proprement call RedessinerLigne() et éviter les artefacts visuels dans le terminal

        RedessinerLigne(prompt, sb.ToString(), secondes, baseTop, ref lastPromptLength, ref lastTimerLength, limitePartie);

        while (DateTime.UtcNow < limite && DateTime.UtcNow < limitePartie)
        {
            // on boucle tant que le tour n'est pas fini et que la partie n'est pas finie
            while (Console.KeyAvailable)
            {
                // lecture non bloquante de l'entrée de l'user
                ConsoleKeyInfo key = Console.ReadKey(intercept: true);
                // lecture de la touche

                if (key.Key == ConsoleKey.Enter)
                {
                    Console.SetCursorPosition(0, baseTop + 2);
                    Console.WriteLine();
                    return sb.ToString();
                    // recupère l'entrée
                }
                else if (key.Key == ConsoleKey.Backspace)
                {
                    if (sb.Length > 0)
                    {
                        sb.Length--;
                        Console.Write("\b \b");
                    }
                    // permet d'effacer (faute de frappe, etc)
                }
                else
                {
                    sb.Append(key.KeyChar);
                    // sinon on ajoute le char à la saisie
                }

                RedessinerLigne(prompt, sb.ToString(), Restant(limite), baseTop, ref lastPromptLength, ref lastTimerLength, limitePartie);
                // màj de l'affichage
            }

            Thread.Sleep(50);
            // petit délai pour que la boucle n'en demande pas trop à mon pauvre CPU
            RedessinerLigne(prompt, sb.ToString(), Restant(limite), baseTop, ref lastPromptLength, ref lastTimerLength, limitePartie);
        }

        Console.SetCursorPosition(0, baseTop + 2);
        Console.WriteLine();
        return null;
        // fin auto (boucle écoulée) => tour suivant
    }

    private int Restant(DateTime limite) =>
        // renvoie le temps restant en secondes
        Math.Max(0, (int)Math.Ceiling((limite - DateTime.UtcNow).TotalSeconds));

    private void AfficherNomCouleur(Joueur joueur)
    {
        // affiche le nom en couleurs diff pour chaque joueur par souci de lisibilité
        ConsoleColor color = joueur == joueur1 ? ConsoleColor.Blue : ConsoleColor.Red;
        Console.ForegroundColor = color;
        Console.Write(joueur.Nom);
        Console.ResetColor();
    }

    private void AfficherResumeJoueur(Joueur joueur)
    {
        // pour chaque joueur, on affiche le score et la qté de mots trouvés
        Console.Write("Joueur : ");
        AfficherNomCouleur(joueur);
        Console.WriteLine($" | Score : {joueur.Score} | Mots trouvés : {joueur.NbMotsTrouves}");
    }

    private void RedessinerLigne(string prompt, string saisie, int secondesRestantes, int baseTop, ref int lastPromptLength, ref int lastTimerLength, DateTime limitePartie)
    {
        // fonction qui met à jour en temps réel l’affichage du chrono et de la ligne de saisie
        Console.SetCursorPosition(0, baseTop);
        int partieRestante = Restant(limitePartie);
        string timerLine = $"Tour : {secondesRestantes}s | Partie : {partieRestante}s";
        int timerPadding = Math.Max(0, lastTimerLength - timerLine.Length);
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write($"{timerLine}{new string(' ', timerPadding)}");
        Console.ResetColor();
        lastTimerLength = timerLine.Length;

        // on repositionne le curseur au début, calcule le temps restant du tour et de la partie, et réécrit cette ligne en jaune en conservant l'alignement.

        Console.SetCursorPosition(0, baseTop + 1);
        string ligne = $"{prompt}{saisie}";
        int padding = Math.Max(0, lastPromptLength - ligne.Length);
        Console.Write($"{ligne}{new string(' ', padding)}");
        lastPromptLength = ligne.Length;
        Console.SetCursorPosition(ligne.Length, baseTop + 1);
    }
}

using System;
using System.IO;

public class Program
{
    public static void Main(string[] args)
    {
        Console.Clear();
        string baseDir = AppContext.BaseDirectory;
        string fichierDico = Path.Combine(baseDir, "src", "txt-files", "Mots_Francais.txt");
        string fichierLettres = Path.Combine(baseDir, "src", "txt-files", "Lettre.txt");

        int lignes = 10;
        int colonnes = 10;
        int tempsTourSec = 40;

        bool quitter = false;

        while (!quitter)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("=== Jeu des Mots Glissés ===");
            Console.ResetColor();
            Console.WriteLine("1) Nouvelle partie");
            Console.WriteLine("2) Charger un plateau (.csv)");
            Console.WriteLine("3) Paramètres");
            Console.WriteLine("4) Licence");
            Console.WriteLine("5) Quitter");
            Console.Write("Choix : ");
            string? choix = Console.ReadLine();

            switch (choix)
            {
                case "1":
                    LancerJeu(fichierDico, fichierLettres, lignes, colonnes, tempsTourSec, null);
                    break;
                case "2":
                    string? cheminCsv = DemanderCheminCsv();
                    if (cheminCsv != null)
                        LancerJeu(fichierDico, fichierLettres, lignes, colonnes, tempsTourSec, cheminCsv);
                    break;
                case "3":
                    tempsTourSec = AjusterParametres(tempsTourSec);
                    break;
                case "4":
                    AfficherLicence();
                    break;
                case "5":
                    quitter = true;
                    break;
                default:
                    Console.WriteLine("Choix invalide. Appuyez sur une touche pour continuer...");
                    Console.ReadKey(intercept: true);
                    break;
            }
        }

        Console.WriteLine("À bientôt !");
    }

    private static void LancerJeu(string fichierDico, string fichierLettres, int lignes, int colonnes, int tempsTourSec, string? plateauCsv)
    {
        try
        {
            Jeu jeu = new Jeu(fichierDico, fichierLettres, lignes, colonnes, tempsTourSec, plateauCsv);
            jeu.Lancer();
            Console.WriteLine("Merci d'avoir joué !");
            Console.WriteLine("Appuyez sur une touche pour revenir au menu...");
            Console.ReadKey(intercept: true);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur : {ex.Message}");
            Console.WriteLine("Appuyez sur une touche pour revenir au menu...");
            Console.ReadKey(intercept: true);
        }
    }

    private static string? DemanderCheminCsv()
    {
        Console.Write("Chemin du fichier CSV : ");
        string? entree = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(entree))
            return null;

        string chemin = Path.GetFullPath(entree.Trim());
        if (!File.Exists(chemin))
        {
            Console.WriteLine("Fichier introuvable. Appuyez sur une touche pour revenir au menu...");
            Console.ReadKey(intercept: true);
            return null;
        }

        return chemin;
    }

    private static int AjusterParametres(int tempsTourSec)
    {
        Console.WriteLine($"\nDurée du tour actuelle (s) : {tempsTourSec}");
        Console.Write("Nouvelle durée (laisser vide pour conserver) : ");
        tempsTourSec = LireEntierOuDefaut(Console.ReadLine(), tempsTourSec, min: 1);

        Console.WriteLine("\nParamètres mis à jour. Appuyez sur une touche pour revenir au menu...");
        Console.ReadKey(intercept: true);
        return tempsTourSec;
    }

    private static int LireEntierOuDefaut(string? saisie, int valeurActuelle, int min)
    {
        if (string.IsNullOrWhiteSpace(saisie))
            return valeurActuelle;

        if (int.TryParse(saisie.Trim(), out int val) && val >= min)
            return val;

        Console.WriteLine("Entrée invalide, valeur précédente conservée.");
        return valeurActuelle;
    }

    private static void AfficherLicence()
    {
        string cwdPath = Path.Combine(Directory.GetCurrentDirectory(), "LICENSE");
        string baseDirPath = Path.Combine(AppContext.BaseDirectory, "LICENSE");
        string licencePath = File.Exists(cwdPath) ? cwdPath : baseDirPath;

        if (!File.Exists(licencePath))
        {
            Console.WriteLine("Fichier LICENSE introuvable.");
        }
        else
        {
            Console.WriteLine("\n=== LICENCE ===\n");
            Console.WriteLine(File.ReadAllText(licencePath));
        }

        Console.WriteLine("\nAppuyez sur une touche pour revenir au menu...");
        Console.ReadKey(intercept: true);
    }
}

using System;
using System.IO;

public class Program
{
    private const string CsvFolderName = "csv";

    public static void Main(string[] args)
    {
        Console.Clear();
        string baseDir = AppContext.BaseDirectory;
        string fichierDico = Path.Combine(baseDir, "src", "txt-files", "Mots_Francais.txt");
        string fichierLettres = Path.Combine(baseDir, "src", "txt-files", "Lettre.txt");

        int lignes = 10;
        int colonnes = 10;
        int tempsTourSec = 40;
        int tempsPartieSec = 400;

        bool quitter = false;

        while (!quitter)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            AfficherAsciiEsilv();
            Console.WriteLine("=== Jeu des Mots Glissés ===");
            Console.ResetColor();
            Console.WriteLine("1) Nouvelle partie");
            Console.WriteLine("2) Sélectionner une partie (dossier /csv)");
            Console.WriteLine("3) Paramètres");
            Console.WriteLine("4) Licence");
            Console.WriteLine("5) Quitter");
            Console.Write("Choix : ");
            string? choix = Console.ReadLine();

            switch (choix)
            {
                case "1":
                    LancerJeu(fichierDico, fichierLettres, lignes, colonnes, tempsTourSec, tempsPartieSec, null);
                    break;
                case "2":
                    string? cheminCsv = SelectionnerPartieDepuisCsv();
                    if (cheminCsv != null)
                        LancerJeu(fichierDico, fichierLettres, lignes, colonnes, tempsTourSec, tempsPartieSec, cheminCsv);
                    break;
                case "3":
                    (tempsTourSec, tempsPartieSec) = AjusterParametres(tempsTourSec, tempsPartieSec);
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

    private static void LancerJeu(string fichierDico, string fichierLettres, int lignes, int colonnes, int tempsTourSec, int tempsPartieSec, string? plateauCsv)
    {
        try
        {
            Jeu jeu = new Jeu(fichierDico, fichierLettres, lignes, colonnes, tempsTourSec, plateauCsv, tempsPartieSec);
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

    private static string AssurerDossierCsv()
    {
        string dir = Path.Combine(Directory.GetCurrentDirectory(), CsvFolderName);
        Directory.CreateDirectory(dir);
        return dir;
    }

    private static string? SelectionnerPartieDepuisCsv()
    {
        string dir = AssurerDossierCsv();
        string[] fichiers = Directory.GetFiles(dir, "*.csv", SearchOption.TopDirectoryOnly);

        if (fichiers.Length == 0)
        {
            Console.WriteLine($"Aucune sauvegarde trouvée dans {dir}");
            Console.WriteLine("Appuyez sur une touche pour revenir au menu...");
            Console.ReadKey(intercept: true);
            return null;
        }

        Console.WriteLine("\nSauvegardes disponibles :");
        for (int i = 0; i < fichiers.Length; i++)
            Console.WriteLine($"{i + 1}) {Path.GetFileName(fichiers[i])}");

        Console.Write("Choisissez un numéro : ");
        string? saisie = Console.ReadLine();
        if (!int.TryParse(saisie, out int choix) || choix < 1 || choix > fichiers.Length)
        {
            Console.WriteLine("Choix invalide. Appuyez sur une touche pour revenir au menu...");
            Console.ReadKey(intercept: true);
            return null;
        }

        return fichiers[choix - 1];
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

    private static (int tempsTourSec, int tempsPartieSec) AjusterParametres(int tempsTourSec, int tempsPartieSec)
    {
        Console.WriteLine($"\nDurée du tour actuelle (s) : {tempsTourSec}");
        Console.Write("Nouvelle durée (laisser vide pour conserver) : ");
        tempsTourSec = LireEntierOuDefaut(Console.ReadLine(), tempsTourSec, min: 1);

        Console.WriteLine($"\nDurée totale de la partie actuelle (s) : {tempsPartieSec}");
        Console.Write("Nouvelle durée (laisser vide pour conserver) : ");
        tempsPartieSec = LireEntierOuDefaut(Console.ReadLine(), tempsPartieSec, min: 1);

        Console.WriteLine("\nParamètres mis à jour. Appuyez sur une touche pour revenir au menu...");
        Console.ReadKey(intercept: true);
        return (tempsTourSec, tempsPartieSec);
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
        string? licencePath = TrouverLicence();
        if (licencePath == null)
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

    private static string? TrouverLicence()
    {
        string[] bases = { Directory.GetCurrentDirectory(), AppContext.BaseDirectory };

        foreach (string start in bases)
        {
            string dir = Path.GetFullPath(start);
            for (int i = 0; i < 5 && dir != null; i++)
            {
                string candidate = Path.Combine(dir, "LICENSE");
                if (File.Exists(candidate))
                    return candidate;

                DirectoryInfo? parent = Directory.GetParent(dir);
                if (parent == null)
                    break;
                dir = parent.FullName;
            }
        }

        return null;
    }

    private static void AfficherAsciiEsilv()
    {
        string[] lignes =
        {
            " _____   ____    ___   _      __      __",
            "| ____| / ___|  |_ _| | |    \\ \\    / /",
            "|  _|   \\___ \\   | |  | |     \\ \\  / / ",
            "| |___   ___) |  | |  | |___   \\ \\/ /  ",
            "|_____| |____/  |___| |_____|   \\__/   "
        };

        foreach (string ligne in lignes)
            Console.WriteLine(ligne);
        
        Console.WriteLine();
    }
}

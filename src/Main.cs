using System;
using System.IO;

public class Program
{
    private const string CsvFolderName = "csv";

    public static void Main(string[] args)
    {
        Console.Clear();//efface tout
        string baseDir = AppContext.BaseDirectory;
        string fichierDico = Path.Combine(baseDir, "src", "txt-files", "Mots_Francais.txt");
        string fichierLettres = Path.Combine(baseDir, "src", "txt-files", "Lettre.txt");
        //Partir de baseDir a permis la compatibilité avec Linux

        //Initialisation des valeurs par défaut
        int lignes = 10;
        int colonnes = 10;
        int tempsTourSec = 40;
        int tempsPartieSec = 400;

        bool quitter = false;//Quand on la passera à true on s'arrete

        while (!quitter)//Contraire de quitter
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.DarkYellow;//On passe à la couleur jaune
            AfficherAsciiEsilv();//Notre affichage Ascii stylé
            Console.WriteLine("=== Jeu des Mots Glissés ===");
            Console.ResetColor();//On repasse à la couleur par défaut
            Console.WriteLine("1) Nouvelle partie");
            Console.WriteLine("2) Sélectionner une partie (dossier /csv)");
            Console.WriteLine("3) Paramètres");
            Console.WriteLine("4) Licence");
            Console.WriteLine("5) Quitter");
            Console.Write("Choix : ");
            string? choix = Console.ReadLine();//Nullable

            switch (choix)//Notre menu, on appelle les fonctions adéquates à chaque fois (avec les bons paramètres)
            {
                case "1":
                    LancerJeu(fichierDico, fichierLettres, lignes, colonnes, tempsTourSec, tempsPartieSec, null);
                    break;
                case "2":
                    string? cheminCsv = SelectionnerPartieDepuisCsv();//Le ? est là pour montrer que cheminCsv est nullable
                    if (cheminCsv != null)
                        LancerJeu(fichierDico, fichierLettres, lignes, colonnes, tempsTourSec, tempsPartieSec, cheminCsv);//On le lance avec le csv donnée
                    break;
                case "3":
                    (tempsTourSec, tempsPartieSec) = AjusterParametres(tempsTourSec, tempsPartieSec);//On lance la fonction de changement de paramètres
                    break;
                case "4":
                    AfficherLicence();
                    break;
                case "5":
                    quitter = true;//On quitte, arrêt du while
                    break;
                default:
                    //Sécurité
                    Console.WriteLine("Choix invalide. Appuyez sur une touche pour continuer...");
                    Console.ReadKey(intercept: true);
                    break;
            }
        }

        Console.WriteLine("À bientôt !");
    }

    private static void LancerJeu(string fichierDico, string fichierLettres, int lignes, int colonnes, int tempsTourSec, int tempsPartieSec, string? plateauCsv)//plateauCsv est nullable
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
        {   //Pour la sécurité
            Console.WriteLine($"Erreur : {ex.Message}");
            Console.WriteLine("Appuyez sur une touche pour revenir au menu...");
            Console.ReadKey(intercept: true);//Récupère la touche sans l'afficher
        }
    }

    private static string AssurerDossierCsv()
    {   //Créer le dossier si il existe pas: Sécurité
        string dir = Path.Combine(Directory.GetCurrentDirectory(), CsvFolderName);//Compatibilité Linux
        Directory.CreateDirectory(dir);
        return dir;
    }

    private static string? SelectionnerPartieDepuisCsv()
    {
        string dir = AssurerDossierCsv();//On s'assure de son existence
        string[] fichiers = Directory.GetFiles(dir, "*.csv", SearchOption.TopDirectoryOnly);//Lis le dossier avec tout les fichiers en csv(pas les sous dossier)

        if (fichiers.Length == 0)//Cas vide
        {
            Console.WriteLine($"Aucune sauvegarde trouvée dans {dir}");
            Console.WriteLine("Appuyez sur une touche pour revenir au menu...");
            Console.ReadKey(intercept: true);
            return null;
        }

        Console.WriteLine("\nSauvegardes disponibles :");
        for (int i = 0; i < fichiers.Length; i++)//Boucle sur le tableau de fichiers trouvés
            Console.WriteLine($"{i + 1}) {Path.GetFileName(fichiers[i])}");

        Console.Write("Choisissez un numéro : ");
        string? saisie = Console.ReadLine();//Sécurisation, nullable
        if (!int.TryParse(saisie, out int choix) || choix < 1 || choix > fichiers.Length)//Tout les cas de problème
        {
            Console.WriteLine("Choix invalide. Appuyez sur une touche pour revenir au menu...");
            Console.ReadKey(intercept: true);
            return null;
        }

        return fichiers[choix - 1];//Car on commence à 1
    }

    private static (int tempsTourSec, int tempsPartieSec) AjusterParametres(int tempsTourSec, int tempsPartieSec)
    {
        //Permet de modifier les paramètres par défaut
        Console.WriteLine($"\nDurée du tour actuelle (s) : {tempsTourSec}");
        Console.Write("Nouvelle durée (laisser vide pour conserver) : ");
        tempsTourSec = LireEntierOuDefaut(Console.ReadLine(), tempsTourSec, min: 1);//vérification bonne entrée en lisant

        Console.WriteLine($"\nDurée totale de la partie actuelle (s) : {tempsPartieSec}");
        Console.Write("Nouvelle durée (laisser vide pour conserver) : ");
        tempsPartieSec = LireEntierOuDefaut(Console.ReadLine(), tempsPartieSec, min: 1);//Vérification bonne entrée en lisant

        Console.WriteLine("\nParamètres mis à jour. Appuyez sur une touche pour revenir au menu...");
        Console.ReadKey(intercept: true);
        return (tempsTourSec, tempsPartieSec);
    }

    private static int LireEntierOuDefaut(string? saisie, int valeurActuelle, int min)
    {
        //S'assure d'avoir les bonnes entrées
        if (string.IsNullOrWhiteSpace(saisie))
            return valeurActuelle;

        if (int.TryParse(saisie.Trim(), out int val) && val >= min)
            return val;

        Console.WriteLine("Entrée invalide, valeur précédente conservée.");
        return valeurActuelle;
    }

    private static void AfficherLicence()
    {
        //On a trouvé ca rigolo
        string? licencePath = TrouverLicence();
        if (licencePath == null)
        {
            Console.WriteLine("Fichier LICENSE introuvable.");
        }
        else
        {
            Console.WriteLine("\nLICENCE\n");
            Console.WriteLine(File.ReadAllText(licencePath));//Tout notre fichier
        }

        Console.WriteLine("\nAppuyez sur une touche pour revenir au menu...");
        Console.ReadKey(intercept: true);
    }

    private static string? TrouverLicence()
    {
        string[] bases = { Directory.GetCurrentDirectory(), AppContext.BaseDirectory };//Pour la compatibilité Linux

        foreach (string start in bases)
        {
            string dir = Path.GetFullPath(start);
            for (int i = 0; i < 5 && dir != null; i++)//Parcours le dossier pour trouver la licence
            {
                string candidate = Path.Combine(dir, "LICENSE");
                if (File.Exists(candidate))
                    return candidate;

                DirectoryInfo? parent = Directory.GetParent(dir);
                if (parent == null)
                    break;
                dir = parent.FullName;//le dossier
            }
        }

        return null;
    }

    private static void AfficherAsciiEsilv()
    {
        //On trouvait ca aussi esthétique d'afficher ceci dans notre menu
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
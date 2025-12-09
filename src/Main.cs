using System;

public class Program
{
    public static void Main(string[] args)
    {
        // chemins vers les fichiers (en fonction de ta structure réelle)
        string fichierDico = "../txt-files/Mots_Francais.txt";
        string fichierLettres = "../txt-files/Lettre.txt";

        Console.WriteLine("=== Jeu des Mots Glissés ===");

        // création du jeu
        // tu peux ajuster la taille du plateau ici :
        int lignes = 10;
        int colonnes = 10;

        Jeu jeu = new Jeu(fichierDico, fichierLettres, lignes, colonnes);

        // lancement du jeu
        jeu.Lancer();

        Console.WriteLine("Merci d'avoir joué !");
    }
}

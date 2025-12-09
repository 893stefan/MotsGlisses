using System;
using System.Collections.Generic;
using System.IO;

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

    // -------------------------
    // CONSTRUCTEUR
    // -------------------------
    public Jeu(string fichierDico, string fichierLettres, int lignes, int colonnes)
    {
        dico = new Dictionnaire(fichierDico);
        plateau = new Plateau(fichierLettres, lignes, colonnes);

        Console.Write("Nom joueur 1 : ");
        joueur1 = new Joueur(Console.ReadLine());

        Console.Write("Nom joueur 2 : ");
        joueur2 = new Joueur(Console.ReadLine());

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
            Console.WriteLine("Tour de : " + j.Nom);

            Console.Write("Entrez un mot (ou STOP pour quitter) : ");
            string mot = Console.ReadLine().ToLower();

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
                object res = plateau.Recherche_Mot(mot);

                if (res == null)
                {
                    Console.WriteLine("Mot NON présent sur le plateau !");
                }
                else
                {
                    Console.WriteLine("Mot trouvé !");
                    j.AjouterScore(mot.Length);
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
            Console.WriteLine("\nGagnant : " + joueur1.Nom);
        else if (joueur2.Score > joueur1.Score)
            Console.WriteLine("\nGagnant : " + joueur2.Nom);
        else
            Console.WriteLine("\nÉgalité !");
    }
}

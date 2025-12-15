using System;
using System.Collections.Generic;
using System.IO;

public class Joueur
{
    // ATTRIBUTS privés
    private string nom = string.Empty;
    private int score = 0;
    private readonly List<string> motsTrouves = new List<string>();

    // PROPRIÉTÉS
    public string Nom
    {
        get { return nom; }
        set { nom = value; }
    }

    public int Score
    {
        get { return score; }
        set { score = value; }
    }

    public int NbMotsTrouves => motsTrouves.Count; // c'est un return

    // CONSTRUCTEURS
    public Joueur(string nom)
    {
        this.nom = string.IsNullOrWhiteSpace(nom) ? "Joueur" : nom; // Si null ou espace alors on l'appelle "Joueur"
    }

    // MÉTHODES D’INSTANCE
    public void AjouterScore(int valeur)
    {
        score += valeur;
    }

    public void Add_Mot(string mot)
    {
        if (string.IsNullOrWhiteSpace(mot))//si le mot est null ou vide ou fait rien
            return;

        string normalise = mot.Trim().ToLower();//enleve espace et met tout en miniscule
        if (!motsTrouves.Contains(normalise))//si il n'y est pas déjà on l'ajoute
            motsTrouves.Add(normalise);
    }

    public bool Contient(string mot)
    {
        if (string.IsNullOrWhiteSpace(mot))//si pas vide ou null
            return false;

        return motsTrouves.Contains(mot.Trim().ToLower());//on renvoie le booléen
    }

    public override string ToString()
    {
        return "Joueur : " + nom + " | Score : " + score + " | Mots trouvés : " + motsTrouves.Count; //Notre affichage
    }
}

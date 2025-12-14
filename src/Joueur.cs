using System;
using System.Collections.Generic;
using System.IO;

public class Joueur
{
    // -------------------------
    // ATTRIBUTS privés
    // -------------------------
    private string nom = string.Empty;
    private int score = 0;
    private readonly List<string> motsTrouves = new List<string>();

    // -------------------------
    // PROPRIÉTÉS
    // -------------------------
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

    public int NbMotsTrouves => motsTrouves.Count;

    // -------------------------
    // CONSTRUCTEURS
    // -------------------------
    public Joueur() { }

    public Joueur(string nom)
    {
        this.nom = string.IsNullOrWhiteSpace(nom) ? "Joueur" : nom;
    }

    // -------------------------
    // MÉTHODES D’INSTANCE
    // -------------------------
    public void AjouterScore(int valeur)
    {
        score += valeur;
    }

    public void Add_Mot(string mot)
    {
        if (string.IsNullOrWhiteSpace(mot))
            return;

        string normalise = mot.Trim().ToLower();
        if (!motsTrouves.Contains(normalise))
            motsTrouves.Add(normalise);
    }

    public bool Contient(string mot)
    {
        if (string.IsNullOrWhiteSpace(mot))
            return false;

        return motsTrouves.Contains(mot.Trim().ToLower());
    }

    public override string ToString()
    {
        return "Joueur : " + nom + " | Score : " + score + " | Mots trouvés : " + motsTrouves.Count;
    }
}

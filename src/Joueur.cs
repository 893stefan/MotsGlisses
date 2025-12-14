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
    private int nbMotsTrouves = 0;

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

    public int NbMotsTrouves
    {
        get { return nbMotsTrouves; }
        set { nbMotsTrouves = value; }
    }

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

    public void IncrementeMots()
    {
        nbMotsTrouves++;
    }

    public override string ToString()
    {
        return "Joueur : " + nom + " | Score : " + score + " | Mots trouvés : " + nbMotsTrouves;
    }
}

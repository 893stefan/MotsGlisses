using System;
using System.Collections.Generic;
using System.IO;

public class Joueur
{
    // -------------------------
    // ATTRIBUTS privés
    // -------------------------
    private string nom;
    private int score;
    private int nbMotsTrouves;

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
        this.nom = nom;
        this.score = 0;
        this.nbMotsTrouves = 0;
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

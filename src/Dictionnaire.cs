using System;
using System.Collections.Generic;
using System.IO;

public class Dictionnaire
{
    // Tableau de 26 listes → une par lettre A-Z
    private List<string>[] motsParLettre;

    public string Langue { get; private set; } = "Français";

    public Dictionnaire(string cheminFichier)
    {
        motsParLettre = new List<string>[26];
        for (int i = 0; i < 26; i++)
            motsParLettre[i] = new List<string>();

        ChargerFichier(cheminFichier);
        Trier();
    }

    // -------------------------------------------------------
    // CHARGEMENT DU DICTIONNAIRE depuis MotsFrancais.txt
    // -------------------------------------------------------

    private void ChargerFichier(string chemin)
    {
        using (StreamReader sr = new StreamReader(chemin))
        {
            string? ligne;
            int index = 0;

            while ((ligne = sr.ReadLine()) != null && index < 26)
            {
                string[] mots = ligne.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                foreach (string mot in mots)
                    motsParLettre[index].Add(mot.Trim().ToLower());

                index++;
            }
        }
    }

    // -------------------------------------------------------
    // TRI FUSION (MERGE SORT)
    // -------------------------------------------------------

    public void Trier()
    {
        for (int i = 0; i < 26; i++)
            motsParLettre[i] = TriFusion(motsParLettre[i]);
    }

    private List<string> TriFusion(List<string> liste)
    {
        if (liste.Count <= 1)
            return liste;

        int mid = liste.Count / 2;

        return Fusion(TriFusion(liste.GetRange(0, mid)), TriFusion(liste.GetRange(mid, liste.Count - mid)));
    }

    private List<string> Fusion(List<string> g, List<string> d)
    {
        List<string> resultat = new List<string>();
        int i = 0, j = 0;

        while (i < g.Count && j < d.Count)
        {
            if (String.Compare(g[i], d[j], StringComparison.Ordinal) <= 0)
            {
                resultat.Add(g[i]);
                i++;
            }
            else
            {
                resultat.Add(d[j]);
                j++;
            }
        }

        while (i < g.Count) resultat.Add(g[i++]);
        while (j < d.Count) resultat.Add(d[j++]);

        return resultat;
    }

    // -------------------------------------------------------
    // RECHERCHE DICHOTOMIQUE RÉCURSIVE
    // -------------------------------------------------------

    public bool RechDichoRecursif(string mot)
    {
        mot = mot.ToLower();

        if (mot.Length == 0)
            return false;

        int index = mot[0] - 'a';

        if (index < 0 || index > 25)
            return false;

        return DichoRec(motsParLettre[index], mot, 0, motsParLettre[index].Count - 1);
    }

    private bool DichoRec(List<string> liste, string mot, int gauche, int droite)
    {
        if (gauche > droite)
            return false;

        int milieu = (gauche + droite) / 2;
        int cmp = String.Compare(mot, liste[milieu], StringComparison.Ordinal);

        if (cmp == 0)
            return true;
        else if (cmp < 0)
            return DichoRec(liste, mot, gauche, milieu - 1);
        else
            return DichoRec(liste, mot, milieu + 1, droite);
    }

    // -------------------------------------------------------
    // TOSTRING
    // -------------------------------------------------------

    public override string ToString()
    {
        string res = $"Dictionnaire ({Langue}) - Nombre de mots par lettre :\n";

        for (int i = 0; i < 26; i++)
        {
            char lettre = (char)('A' + i);
            res += $"{lettre} : {motsParLettre[i].Count} mots\n";
        }

        return res;
    }
}

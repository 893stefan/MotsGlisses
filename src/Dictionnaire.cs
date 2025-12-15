using System;
using System.Collections.Generic;
using System.IO;

public class Dictionnaire
{
    // Tableau de 26 listes, une par lettre
    private List<string>[] motsParLettre;

    //Constructeur
    public Dictionnaire(string cheminFichier)
    {
        motsParLettre = new List<string>[26];//Tableau de taille 26
        for (int i = 0; i < 26; i++)
            motsParLettre[i] = new List<string>();//Liste vide dans chacune (on veut  pas que ce soit null)

        ChargerFichier(cheminFichier);//Ajoute les mots mais dans le désordre
        Trier();//Trie tout
    }

    // Chargement du dictionnaire depuis MotsFrancais.txt

    private void ChargerFichier(string chemin)
    {
        using (StreamReader sr = new StreamReader(chemin))//Le using est important pour fermer proprement le fichier, si le programme plante ca le libère
        {
            string? ligne;//nullable
            int index = 0;

            while ((ligne = sr.ReadLine()) != null && index < 26)//Chaque ligne du dictionnaire correspond aux mots qui commence par une lettre de l'alphabet
            {
                string[] mots = ligne.Split(' ', StringSplitOptions.RemoveEmptyEntries);//Prend la ligne entière et la coupe à chaque espace

                foreach (string mot in mots)
                    motsParLettre[index].Add(mot.Trim().ToLower());//Ajoute le mot sans espace et en minuscule

                index++;
            }
        }
    }

    // TRI FUSION
    public void Trier()
    {
        //Lance le tri pour chacune des lettres
        for (int i = 0; i < 26; i++)
            motsParLettre[i] = TriFusion(motsParLettre[i]);
    }

    private List<string> TriFusion(List<string> liste)
    {
        //Condition d'arrêt
        if (liste.Count <= 1)
            return liste;

        int mid = liste.Count / 2;//Divise

        //Appel récursif
        return Fusion(TriFusion(liste.GetRange(0, mid)), TriFusion(liste.GetRange(mid, liste.Count - mid)));//Fusion recolle les morceaux triés
    }

    private List<string> Fusion(List<string> g, List<string> d)
    {
        List<string> resultat = new List<string>();
        int i = 0, j = 0;//i parcourt à gauche, j à droite

        //boucle principale et comparaison
        while (i < g.Count && j < d.Count)
        {
            if (String.Compare(g[i], d[j], StringComparison.Ordinal) <= 0)//On compare mot de gauche et de droite, Compare retourne -1 si g[i] est devant d[j] dans l'alphabet
            {
                resultat.Add(g[i]);//On prend le mot de gauche
                i++;
            }
            else
            {
                resultat.Add(d[j]);//On prend le mot de droite
                j++;
            }
        }

        //Nettoyage: On ajoute tout ce qu'il reste
        while (i < g.Count) resultat.Add(g[i++]);
        while (j < d.Count) resultat.Add(d[j++]);

        return resultat;
    }

    // RECHERCHE DICHOTOMIQUE RÉCURSIVE
    public bool RechDichoRecursif(string mot)
    {
        mot = mot.ToLower();//Tout en minuscule

        if (mot.Length == 0)//vérifie que pas vide
            return false;

        int index = mot[0] - 'a';//Calcul de l'indice avec les codes ASCII ('c' - 'a' = 2)
        //C'est top car on élimine directement tout le reste du dictionnaire, ca fait gagner beaucoup de complexité
        //On élimine instantanément environ 96% du dictionnaire

        //On check que ca commence bien par une lettre (entre A et Z)
        if (index < 0 || index > 25)
            return false;

        return DichoRec(motsParLettre[index], mot, 0, motsParLettre[index].Count - 1);//Notre recherche directement pour la bonne lettre
    }

    private bool DichoRec(List<string> liste, string mot, int gauche, int droite)
    {
        //Condition d'arrêt
        if (gauche > droite)
            return false;

        int milieu = (gauche + droite) / 2; //Notre milieu
        int cmp = String.Compare(mot, liste[milieu], StringComparison.Ordinal);//renvoie 0 si égal, <0 si avant, >0 si après

        if (cmp == 0)
            return true;//0 si égal donc TROUVE
        else if (cmp < 0)
            return DichoRec(liste, mot, gauche, milieu - 1);//Trop loin, on prend à gauche
        else
            return DichoRec(liste, mot, milieu + 1, droite);//Trop tot, on prend à droite
    }

    // TOSTRING
    public override string ToString()
    {
        //Notre Affichage
        string res = "Dictionnaire (Français) - Nombre de mots par lettre :\n";
        for (int i = 0; i < 26; i++)
        {
            char lettre = (char)('A' + i);//Astuce ASCII, 'A'=65, 'A'+1=66='B' etc.
            res += $"{lettre} : {motsParLettre[i].Count} mots\n";//Pour chaque lettres on affiche le nombre de mots correspondants
        }

        return res;
    }
}
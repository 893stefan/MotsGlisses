using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

namespace TestUnitaire;

public class PlateauTests
{
    public void Recherche_Mot_trouve_un_mot_sur_grille_csv()
    {
        string tmp = Path.GetTempFileName();
        File.WriteAllLines(tmp, new[] { "A,B,C", "D,E,F", "G,H,I" });

        var plateau = new Plateau(tmp);
        var chemin = plateau.Recherche_Mot("ghi");

        Assert.NotNull(chemin);
        Assert.Equal(3, chemin!.Count);

        File.Delete(tmp);
    }

    public void Maj_Plateau_tasse_correctement_une_colonne()
    {
        string tmp = Path.GetTempFileName();
        File.WriteAllLines(tmp, new[] { "A,B,C", "D,E,F", "G,H,I" });
        var plateau = new Plateau(tmp);

        var coords = new List<(int x, int y)> { (1, 1), (2, 1) }; // E et H
        plateau.Maj_Plateau(coords);

        Assert.Equal('B', plateau.Grille[2, 1]);
        Assert.Equal(' ', plateau.Grille[1, 1]);
        Assert.Equal(' ', plateau.Grille[0, 1]);

        File.Delete(tmp);
    }

    public void ScorePourChemin_calcul_depuis_poids_lettres()
    {
        string racine = TestHelper.RacineSolution();
        string cheminLettres = Path.Combine(racine, "src", "txt-files", "Lettre.txt");
        var plateau = new Plateau(cheminLettres, 4, 4);

        var coords = new List<(int, int)> { (0, 0), (0, 1), (0, 2) };
        int attendu = coords.Sum(c =>
        {
            char lettre = char.ToUpper(plateau.Grille[c.Item1, c.Item2]);
            return LirePoids(cheminLettres, lettre);
        });

        Assert.Equal(attendu, plateau.ScorePourChemin(coords));
    }

    private static int LirePoids(string fichier, char lettre)
    {
        foreach (var ligne in File.ReadLines(fichier))
        {
            var parts = ligne.Split(',', System.StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length >= 3 && parts[0].Trim().ToUpper()[0] == lettre)
                return int.Parse(parts[2]);
        }
        return 0;
    }
}

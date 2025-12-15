using System.IO;
using System.Linq;
using Xunit;

namespace TestUnitaire;

public class DictionnaireTests
{
    public void RechDichoRecursif_trouve_mot_connu_et_rejette_inconnu()
    {
        string racine = TestHelper.RacineSolution();
        string cheminDico = Path.Combine(racine, "src", "txt-files", "Mots_Francais.txt");

        var dico = new Dictionnaire(cheminDico);
        var premierMot = (File.ReadLines(cheminDico).FirstOrDefault() ?? "")
            .Split(' ', System.StringSplitOptions.RemoveEmptyEntries)
            .FirstOrDefault() ?? "abc";

        Assert.True(dico.RechDichoRecursif(premierMot.ToLower()));
        Assert.False(dico.RechDichoRecursif("mot_inexistant_zzz"));
    }
}

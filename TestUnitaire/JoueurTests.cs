using Xunit;

namespace TestUnitaire;

public class JoueurTests
{
    public void Add_Mot_evite_les_doublons_insensibles_a_la_casse()
    {
        var joueur = new Joueur("Test");
        joueur.Add_Mot("Mot");
        joueur.Add_Mot("mot");

        Assert.True(joueur.Contient("mot"));
        Assert.Equal(1, joueur.NbMotsTrouves);
    }
}

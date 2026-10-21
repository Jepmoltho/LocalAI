namespace LocalAI.Embeddings;

/// <summary>
/// A small hard-coded catalogue to experiment with. Later this comes from the mybarmenu database.
/// </summary>
public static class CocktailCatalog
{
    public static List<Cocktail> GetAll()
    {
        List<Cocktail> cocktails = new List<Cocktail>();

        cocktails.Add(new Cocktail
        {
            Name = "Negroni",
            Description = "Gin, Campari and sweet vermouth stirred over ice. Bitter, herbal and bold, with an orange peel garnish."
        });

        cocktails.Add(new Cocktail
        {
            Name = "Mezcal Negroni",
            Description = "Mezcal, Campari and sweet vermouth. A smoky, bitter twist on the classic Negroni."
        });

        cocktails.Add(new Cocktail
        {
            Name = "Mojito",
            Description = "White rum, fresh mint, lime, sugar and soda water. Light, refreshing and perfect on a hot summer day."
        });

        cocktails.Add(new Cocktail
        {
            Name = "Espresso Martini",
            Description = "Vodka, coffee liqueur and fresh espresso shaken hard. Rich, sweet and energising, with a creamy foam."
        });

        cocktails.Add(new Cocktail
        {
            Name = "Piña Colada",
            Description = "Rum, pineapple juice and coconut cream blended with ice. Sweet, creamy and tropical."
        });

        cocktails.Add(new Cocktail
        {
            Name = "Penicillin",
            Description = "Blended Scotch, lemon, honey-ginger syrup and a float of peaty Islay whisky. Smoky, spicy and warming."
        });

        cocktails.Add(new Cocktail
        {
            Name = "Aperol Spritz",
            Description = "Aperol, prosecco and a splash of soda. Low in alcohol, bittersweet and bubbly, ideal for an afternoon aperitivo."
        });

        cocktails.Add(new Cocktail
        {
            Name = "Old Fashioned",
            Description = "Bourbon, a sugar cube and Angostura bitters stirred with an orange twist. Strong, spirit-forward and not sweet."
        });

        cocktails.Add(new Cocktail
        {
            Name = "Last Word",
            Description = "Gin, green Chartreuse, maraschino liqueur and lime in equal parts. Sharp, herbal and complex."
        });

        cocktails.Add(new Cocktail
        {
            Name = "Virgin Mojito",
            Description = "Fresh mint, lime, sugar and soda water with no alcohol. Refreshing and suitable for anyone."
        });

        return cocktails;
    }
}

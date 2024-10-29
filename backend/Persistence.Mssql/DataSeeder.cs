namespace ActivityGameBackend.Persistence.Mssql;

public static class DataSeeder
{
    public static async Task SeedWordsAsync(ApplicationDbContext context)
    {
        if (!context.Words.Any())
        {
            var words = new List<WordEntity>
            {
                 new()
                 {
                    Value = "bokszkesztyű", Method = MethodType.Drawing,
                },
                new()
                {
                    Value = "rendőrautó", Method = MethodType.Drawing,
                },
                new()
                {
                    Value = "méhkas", Method = MethodType.Drawing,
                },
                new()
                {
                    Value = "szamárlap", Method = MethodType.Drawing,
                },
                new()
                {
                    Value = "játszótér", Method = MethodType.Drawing,
                },
                new()
                {
                    Value = "erdőhatár", Method = MethodType.Drawing,
                },
                new()
                {
                    Value = "fészekvirágzatúak", Method = MethodType.Drawing,
                },
                new()
                {
                    Value = "sörhab", Method = MethodType.Drawing,
                },
                new()
                {
                    Value = "tettenérés", Method = MethodType.Description,
                },
                new()
                {
                    Value = "lángész", Method = MethodType.Description,
                },
                new()
                {
                    Value = "dugattyúhenger", Method = MethodType.Description,
                },
                new()
                {
                    Value = "korallzátony", Method = MethodType.Description,
                },
                new()
                {
                    Value = "hadkötelezettség", Method = MethodType.Description,
                },
                new()
                {
                    Value = "pártkönyv", Method = MethodType.Description,
                },
                new()
                {
                    Value = "zuhanyrózsa", Method = MethodType.Description,
                },
                new()
                {
                    Value = "lánc: faház", Method = MethodType.Description,
                },
                new()
                {
                    Value = "karének", Method = MethodType.Description,
                },
                new()
                {
                    Value = "talajtorna", Method = MethodType.Mimic,
                },
                new()
                {
                    Value = "darázsfészek", Method = MethodType.Mimic,
                },
                new()
                {
                    Value = "zászlórúd", Method = MethodType.Mimic,
                },
                new()
                {
                    Value = "gyalupad", Method = MethodType.Mimic,
                },
                new()
                {
                    Value = "körtefa", Method = MethodType.Mimic,
                },
                new()
                {
                    Value = "mászókötél", Method = MethodType.Mimic,
                },
                new()
                {
                    Value = "levélszekrény", Method = MethodType.Mimic,
                },
                new()
                {
                    Value = "teveszőr", Method = MethodType.Mimic,
                },
                new()
                {
                    Value = "Airport", Method = MethodType.Description,
                },
                new()
                {
                    Value = "Football", Method = MethodType.Mimic,
                },
            };

            context.Words.AddRange(words);
            await context.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}

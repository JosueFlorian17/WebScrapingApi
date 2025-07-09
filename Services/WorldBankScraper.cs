using HtmlAgilityPack;

namespace WebScrapingApi.Services
{
    public class WorldBankScraper
    {
        private readonly string url = "https://projects.worldbank.org/en/projects-operations/procurement/debarred-firms";

        public async Task<(int count, List<Dictionary<string, string>> results)> SearchEntity(string entity)
        {
            var web = new HtmlWeb();
            var doc = await web.LoadFromWebAsync(url);

            var results = new List<Dictionary<string, string>>();

            // Encuentra todas las tablas
            var allTables = doc.DocumentNode.SelectNodes("//table");
            if (allTables == null) return (0, results);

            HtmlNode targetTable = null;

            // Busca la tabla que contiene el texto esperado en su header
            foreach (var table in allTables)
            {
                var headers = table.SelectNodes(".//tr[1]/td");
                if (headers != null && headers.Count == 4 &&
                    headers[0].InnerText.Contains("Name of Firm") &&
                    headers[1].InnerText.Contains("Date of Imposition") &&
                    headers[2].InnerText.Contains("Sanction Imposed") &&
                    headers[3].InnerText.Contains("Grounds"))
                {
                    targetTable = table;
                    break;
                }
            }

            if (targetTable == null)
                return (0, results);

            var rows = targetTable.SelectNodes(".//tr");
            if (rows == null || rows.Count <= 1)
                return (0, results);

            foreach (var row in rows.Skip(1))
            {
                var cols = row.SelectNodes(".//td");
                if (cols == null || cols.Count != 4)
                    continue;

                // Combina y limpia texto para búsqueda
                var rowText = string.Join(" ", cols.Select(c => c.InnerText.Trim().ToLower()));

                if (rowText.Contains(entity.ToLower().Trim()))
                {
                    results.Add(new Dictionary<string, string>
                    {
                        ["NameAndAddress"] = cols[0].InnerText.Trim(),
                        ["SanctionDate"] = cols[1].InnerText.Trim(),
                        ["SanctionType"] = cols[2].InnerText.Trim(),
                        ["Grounds"] = cols[3].InnerText.Trim()
                    });
                }
            }

            return (results.Count, results);
        }
    }
}

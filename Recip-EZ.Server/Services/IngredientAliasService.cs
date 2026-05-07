using Microsoft.EntityFrameworkCore;
using Recip_EZ.Server.Data;
using Recip_EZ.Server.Models;
using System.Text.RegularExpressions;

namespace Recip_EZ.Server.Services
{
    public class IngredientAliasService
    {
        #region Fields

        private readonly RecipEzDbContext _context;

        private static readonly HashSet<string> IgnoredAliasWords = new(StringComparer.OrdinalIgnoreCase)
        {
            "a",
            "an",
            "and",
            "boneless",
            "cooked",
            "crushed",
            "diced",
            "dried",
            "extra",
            "fresh",
            "fried",
            "grated",
            "ground",
            "large",
            "medium",
            "minced",
            "of",
            "optional",
            "or",
            "raw",
            "roasted",
            "shredded",
            "skinless",
            "sliced",
            "small",
            "taste",
            "to",
            "virgin",
            "low fat",
            "no fat"
        };

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Constructor for the service so that the DbContext can be used in the methods of this service.
        /// </summary>
        /// <param name="context">The database context to be used by the service</param>
        public IngredientAliasService(RecipEzDbContext context)
        {
            _context = context;
        }

        #endregion

        #region CRUD Methods

        /// <summary>
        /// Populates the alias table using the shared alias-generation rules. Safe to run multiple times.
        /// </summary>
        public void AddToAliases()
        {
            var ingredients = _context.Ingredients
                .AsNoTracking()
                .Where(i => !string.IsNullOrWhiteSpace(i.Name))
                .ToList();

            var existingAliases = _context.IngredientAliases
                .AsNoTracking()
                .ToList()
                .GroupBy(alias => alias.IngredientId)
                .ToDictionary(
                    group => group.Key,
                    group => group
                        .Select(alias => alias.AliasName.Trim().ToLowerInvariant())
                        .ToHashSet(StringComparer.OrdinalIgnoreCase));

            List<IngredientAlias> aliasesToAdd = new();

            foreach (var ingredient in ingredients)
            {
                if (ingredient.Name == null)
                {
                    continue;
                }

                var knownAliases = existingAliases.GetValueOrDefault(
                    ingredient.IngredientId,
                    new HashSet<string>(StringComparer.OrdinalIgnoreCase));

                foreach (var alias in GetAliasesForIngredientName(ingredient.Name))
                {
                    if (knownAliases.Contains(alias))
                    {
                        continue;
                    }

                    aliasesToAdd.Add(new IngredientAlias
                    {
                        IngredientId = ingredient.IngredientId,
                        AliasName = alias
                    });

                    knownAliases.Add(alias);
                }

                existingAliases[ingredient.IngredientId] = knownAliases;
            }

            if (aliasesToAdd.Count == 0)
            {
                return;
            }

            _context.IngredientAliases.AddRange(aliasesToAdd);
            _context.SaveChanges();
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Creates a set of consistent aliases for an ingredient phrase so inventory items and recipes can be matched
        /// through the same normalization pipeline.
        /// </summary>
        /// <param name="name">Ingredient phrase to normalize into aliases.</param>
        /// <returns>Distinct, normalized aliases ordered by insertion.</returns>
        public HashSet<string> GetAliasesForIngredientName(string? name)
        {
            var aliases = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            if (string.IsNullOrWhiteSpace(name))
            {
                return aliases;
            }

            var cleanedPhrase = CleanPhrase(name);
            AddAlias(aliases, cleanedPhrase);

            var normalizedWords = GetNormalizedWords(name);

            if(normalizedWords.Count == 0)
            {
                return aliases;
            }

            var singularPhrase = string.Join(' ', normalizedWords);
            AddAlias(aliases, singularPhrase);

            var pluralWords = normalizedWords.Select(PluralizeWord).ToList();
            var pluralPhrase = string.Join(' ', pluralWords);
            AddAlias(aliases, pluralPhrase);

            var lastWord = normalizedWords.Last();
            AddAlias(aliases, lastWord);
            AddAlias(aliases, PluralizeWord(lastWord));

            if (normalizedWords.Count >= 2)
            {
                var lastTwoSingular = string.Join(' ', normalizedWords.TakeLast(2));
                var lastTwoPlural = string.Join(' ', normalizedWords.TakeLast(2).Select((word, index) =>
                    index == 1 ? PluralizeWord(word) : word));

                AddAlias(aliases, lastTwoSingular);
                AddAlias(aliases, lastTwoPlural);
            }

            if (normalizedWords.Count >= 3)
            {
                AddAlias(aliases, string.Join(' ', normalizedWords.TakeLast(3)));
            }

            return aliases;
        }

        /// <summary>
        /// Produces the main normalized phrase for an ingredient.
        /// </summary>
        /// <param name="name">Ingredient phrase to normalize.</param>
        /// <returns>Normalized ingredient phrase.</returns>
        public string NormalizeIngredient(string? name)
        {
            return string.Join(' ', GetNormalizedWords(name));
        }

        private List<string> GetNormalizedWords(string? name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return new List<string>();
            }

            var cleanedPhrase = CleanPhrase(name);

            return cleanedPhrase
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Where(word => !IgnoredAliasWords.Contains(word))
                .Select(SingularizeWord)
                .Where(word => !string.IsNullOrWhiteSpace(word))
                .ToList();
        }

        private string CleanPhrase(string name)
        {
            var cleanedPhrase = name.Trim().ToLowerInvariant();
            cleanedPhrase = cleanedPhrase.Replace("&", " and ");
            cleanedPhrase = Regex.Replace(cleanedPhrase, @"[\d]", " ");
            cleanedPhrase = Regex.Replace(cleanedPhrase, @"[^\w\s]", " ");
            cleanedPhrase = Regex.Replace(cleanedPhrase, @"\s+", " ").Trim();
            return cleanedPhrase;
        }

        private void AddAlias(HashSet<string> aliases, string? alias)
        {
            if (string.IsNullOrWhiteSpace(alias))
            {
                return;
            }

            aliases.Add(alias.Trim().ToLowerInvariant());
        }

        private string SingularizeWord(string word)
        {
            if (string.IsNullOrWhiteSpace(word) || word.Length <= 2)
            {
                return word;
            }

            if (word.EndsWith("ies", StringComparison.OrdinalIgnoreCase) && word.Length > 3)
            {
                return word[..^3] + "y";
            }

            if (word.EndsWith("oes", StringComparison.OrdinalIgnoreCase) && word.Length > 3)
            {
                return word[..^2];
            }

            if (word.EndsWith("es", StringComparison.OrdinalIgnoreCase) &&
                word.Length > 3 &&
                !word.EndsWith("ses", StringComparison.OrdinalIgnoreCase))
            {
                return word[..^2];
            }

            if (word.EndsWith("s", StringComparison.OrdinalIgnoreCase) &&
                word.Length > 3 &&
                !word.EndsWith("ss", StringComparison.OrdinalIgnoreCase))
            {
                return word[..^1];
            }

            return word;
        }

        private string PluralizeWord(string word)
        {
            if (string.IsNullOrWhiteSpace(word) || word.Length <= 2)
            {
                return word;
            }

            if (word.EndsWith("y", StringComparison.OrdinalIgnoreCase) &&
                word.Length > 1 &&
                !"aeiou".Contains(char.ToLowerInvariant(word[^2])))
            {
                return word[..^1] + "ies";
            }

            if (word.EndsWith("o", StringComparison.OrdinalIgnoreCase))
            {
                return word + "es";
            }

            if (word.EndsWith("s", StringComparison.OrdinalIgnoreCase) ||
                word.EndsWith("x", StringComparison.OrdinalIgnoreCase) ||
                word.EndsWith("z", StringComparison.OrdinalIgnoreCase) ||
                word.EndsWith("ch", StringComparison.OrdinalIgnoreCase) ||
                word.EndsWith("sh", StringComparison.OrdinalIgnoreCase))
            {
                return word + "es";
            }

            return word + "s";
        }
        #endregion


    }
}

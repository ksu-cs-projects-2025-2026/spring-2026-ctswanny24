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
            "low",
            "no",
            "fat",
            "free",
            "reduced"
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
        /// Populates the alias table based on the INGREDIENTS db. DIFFERENT FROM THE RECIPE.
        /// </summary>
        public void AddToAliases()
        {
            //Grabs the list of ingredients that aren't null
            var ingredients = _context.Ingredients
                .AsNoTracking()
                .Where(i => !string.IsNullOrWhiteSpace(i.Name))
                .ToList();

            //Grabs the current list of Aliases and groups them into dictionaries based on ingredientId
            //I.e. parmesean cheese and its aliases are stored in dictionary at ingredient id 15
            //and the HashSet of aliases is contained in that dictionary there
            var existingAliases = _context.IngredientAliases
                .AsNoTracking()
                .ToList()
                .GroupBy(alias => alias.IngredientId)
                .ToDictionary(
                    group => group.Key,
                    group => group
                        .Select(alias => alias.AliasName.Trim().ToLowerInvariant())
                        .ToHashSet(StringComparer.OrdinalIgnoreCase));

            //Creates a list of aliases that will need to be added during this logic
            List<IngredientAlias> aliasesToAdd = new();

            //Iterate through EACH ingredient.
            foreach (var ingredient in ingredients)
            {
                //Checks for nulls
                if (ingredient.Name == null)
                {
                    continue;
                }

                //Grabs the hash set of the aliases for the current ingredient.
                var knownAliases = existingAliases.GetValueOrDefault(
                    ingredient.IngredientId,
                    new HashSet<string>(StringComparer.OrdinalIgnoreCase));

                //Now, a list of aliases for the ingredient is conducted.
                //This foreach will go through every one that is generated to see if there is a new one.
                foreach (var alias in GetAliasesForIngredientName(ingredient.Name))
                {
                    //If it's already known, continue
                    if (knownAliases.Contains(alias))
                    {
                        continue;
                    }

                    //Else, add a new alias
                    aliasesToAdd.Add(new IngredientAlias
                    {
                        IngredientId = ingredient.IngredientId,
                        AliasName = alias
                    });

                    //Add new alias to the known aliases
                    knownAliases.Add(alias);
                }

                //Now, include the new knownAliases for this ingredient into the dictionary
                existingAliases[ingredient.IngredientId] = knownAliases;
            }

            //If no new aliases were created, return
            if (aliasesToAdd.Count == 0)
            {
                return;
            }

            //Else, add the new aliases to the db.
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
            //Create a new hash set for all the aliases
            var aliases = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            //Check to make sure no null or white space is being passed in
            if (string.IsNullOrWhiteSpace(name))
            {
                return aliases;
            }

            //Clean the ingredient phrase.
            var cleanedPhrase = CleanPhrase(name);

            //Add the cleaned phrase into the aliases
            AddAlias(aliases, cleanedPhrase);

            //Now normalize the word
            var normalizedWords = GetNormalizedWords(cleanedPhrase);

            if(normalizedWords.Count == 0)
            {
                return aliases;
            }

            var singularPhrase = string.Join(' ', normalizedWords);
            AddAlias(aliases, singularPhrase);

            var pluralWords = normalizedWords.Select(PluralizeWord).ToList();
            var pluralPhrase = string.Join(' ', pluralWords);
            AddAlias(aliases, pluralPhrase);

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

        /// <summary>
        /// Returns a list of normalized words extracted from the specified name, excluding ignored words and applying
        /// singularization.
        /// </summary>
        /// <remarks>Normalization includes cleaning the phrase, splitting into words, removing ignored
        /// words, and converting words to their singular form.</remarks>
        /// <param name="name">The input string from which to extract and normalize words. Can be null or whitespace.</param>
        /// <returns>A list of normalized words derived from the input. Returns an empty list if the input is null or contains
        /// only whitespace.</returns>
        private List<string> GetNormalizedWords(string? name)
        {
            //Ensure the name is not null
            if (string.IsNullOrWhiteSpace(name))
            {
                return new List<string>();
            }

            //var cleanedPhrase = CleanPhrase(name);

            //Split the name into multiple strings
            var cleanedWords = name.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            //Create a final list to return
            List<string> finalNormalized = new List<string>();

            //Go through each cleaned word
            foreach(var word in cleanedWords)
            {
                //Check if it is somehow null or whitespace
                if (string.IsNullOrWhiteSpace(word))
                {
                    continue;
                }

                //Singularize the word
                var singularWord = SingularizeWord(word);

                //If either the normal word or singular word is NOT an ignored word (from the list above)
                //It will be added to the list of final normalized words
                if (!IgnoredAliasWords.Contains(word) || !IgnoredAliasWords.Contains(singularWord))
                {
                    //finalNormalized.Add(word);
                    finalNormalized.Add(singularWord);
                }
            }

            //Return the final list.
            return finalNormalized;
        }

        /// <summary>
        /// Cleans the phrase to scrub out all digits, extra symbols, and errant spaces
        /// </summary>
        /// <param name="name">Ingredient Name</param>
        /// <returns>Cleaned ingredient name</returns>
        private string CleanPhrase(string name)
        {
            //Trim the phrase and make it all lowercase.
            var cleanedPhrase = name.Trim().ToLowerInvariant();

            //Replace certain symbols with their alternatives
            cleanedPhrase = cleanedPhrase.Replace("&", " and ");

            //If there's a digit, replace it with a space
            cleanedPhrase = Regex.Replace(cleanedPhrase, @"[\d]", " ");

            //All punctuation is replaced with a space
            cleanedPhrase = Regex.Replace(cleanedPhrase, @"[^\w\s]", " ");

            //Normalizes all errant spaces created by the previous logic
            cleanedPhrase = Regex.Replace(cleanedPhrase, @"\s+", " ").Trim();

            //Return a cleaned phrase with no digits or punctuation.
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

        /// <summary>
        /// Singularizes a word that may or may not be plural
        /// </summary>
        /// <param name="word">Word to be singularized</param>
        /// <returns>singularized word</returns>
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

        /// <summary>
        /// Pluralizes a word that may or may not be plural
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
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

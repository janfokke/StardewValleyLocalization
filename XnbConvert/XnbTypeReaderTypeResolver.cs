using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using XnbConvert.Exceptions;
using XnbConvert.Readers;

namespace XnbConvert
{
    public static class XnbTypeReaderTypeResolver
    {
        private static readonly Dictionary<string, Type> TypesFromTypeNames = new Dictionary<string, Type>
        {
            {"System.Collections.Generic.Dictionary", typeof(Dictionary<,>)},
            {"Microsoft.Xna.Framework.Content.DictionaryReader", typeof(DictionaryTypeReader<,>)},
            {"Microsoft.Xna.Framework.Content.StringReader", typeof(StringTypeReader)},
            {"Microsoft.Xna.Framework.Content.Int32Reader", typeof(IntTypeReader)},

            {"System.Int32", typeof(int)},
            {"System.String", typeof(string)},
            {"System.Collections.Generic.List", typeof(List<>)}
        };

        private static readonly Dictionary<Type, Func<Type, Type>> ReaderTypeFromTargetTypes =
            new Dictionary<Type, Func<Type, Type>>()
            {
                {
                    typeof(Dictionary<,>), type =>
                    {
                        Type[] genericArguments = type.GetGenericArguments();
                        return typeof(DictionaryTypeReader<,>).MakeGenericType(genericArguments);
                    }
                },
                {
                    typeof(string), _ => typeof(StringTypeReader)
                },
                {
                    typeof(int), _ => typeof(IntTypeReader) 
                }
            };


        private static int GetGenericCountFromTypeName(string fullname)
        {   
            string[] split = fullname.Split(',','`');
            if (split.Length <= 1) 
                return 0;
            
            if (split[1].StartsWith(" "))
            {
                return 0;
            }
                
            var numberResolver = new Regex(@"[0-9]+");
            Match match = numberResolver.Match(split[1]);
            return match.Success ? int.Parse(match.Value) : 0;
        }

        private static string GetSimplifiedName(string fullName)
        {
            return fullName.Split('`', ',')[0];
        }

        public static Type ResolveFromTargetType<TTargetType>()
        {
            return ResolveFromTargetType(typeof(TTargetType));
        }
        
        public static Type ResolveFromTargetType(Type targetType)
        {
            Type targetTypeTmp = targetType.IsGenericType ? targetType.GetGenericTypeDefinition() : targetType;

            if (!ReaderTypeFromTargetTypes.TryGetValue(targetTypeTmp, out Func<Type, Type> factoryFunc))
                throw new NotSupportedException($"{targetType.Name} is not supported");
            
            return factoryFunc(targetType);
        }

        public static Type ResolveFromName(string fullName)
        {
            if (string.IsNullOrEmpty(fullName) || !fullName.StartsWith("Microsoft.Xna.Framework.Content"))
            {
                throw new InvalidXnbTypeReaderNameException(fullName);
            }

            return _resolveFromName(fullName);
        }
        
        private static Type _resolveFromName(string fullName)
        {
            string simplifiedName = GetSimplifiedName(fullName);
            if (!TypesFromTypeNames.TryGetValue(simplifiedName, out Type mainType))
            {
                throw new XnbException($"{simplifiedName} is not supported");
            }
            
            int genericCount = GetGenericCountFromTypeName(fullName);
            
            if(genericCount == 0)
                return mainType; 
            
            string genericTypesString = GetStringsBetweenMostOuterBrackets(fullName).First();
            var innerTypes = new Type[genericCount];
            List<string> innerTypeFullNames = GetStringsBetweenMostOuterBrackets(genericTypesString);
            for (var index = 0; index < innerTypeFullNames.Count; index++)
            {
                string innerType = innerTypeFullNames[index];
                innerTypes[index] = _resolveFromName(innerType);
            }
            return mainType.MakeGenericType(innerTypes.ToArray());
        }

        private static List<string> GetStringsBetweenMostOuterBrackets(string value)
        {
            var regex = new Regex(@"(?:[\[])(?:[^\[\]]|(?<Open>[\[])|(?<-Open>[\]]))*\]");
            Match genericMatch = regex.Match(value);
            var matches = new List<string>();
            while (true)
            {
                string matchWithBrackets = genericMatch.Value;
                matches.Add(RemoveFirstAndLastChar(matchWithBrackets));   
                genericMatch = genericMatch.NextMatch();
                if(!genericMatch.Success)
                    break;
            }
            return matches;
        }

        private static string RemoveFirstAndLastChar(string matchWithBrackets)
        {
            return matchWithBrackets.Substring(1, matchWithBrackets.Length - 2);
        }
    }
}
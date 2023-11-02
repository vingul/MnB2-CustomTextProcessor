using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using TaleWorlds.Library;
using TaleWorlds.Localization.TextProcessor;
using System.IO;

namespace CustomTextProcessor
{
    public class UkrainianTextProcessor : LanguageSpecificTextProcessor
	{
		public UkrainianTextProcessor()
        {
			ReadRulesData();
        }
		private Dictionary<string, Dictionary<string, Dictionary<string, SuffixGroup>>> m_affixRules = null;
		private void ReadRulesData()
        {
			using (MemoryStream resourceStream = new MemoryStream(AffixResource.rules))
			{
				if (resourceStream != null)
				{
					System.IO.BinaryReader reader = new System.IO.BinaryReader(resourceStream);
					
					int affixCount = reader.ReadInt32();
					m_affixRules = new Dictionary<string, Dictionary<string, Dictionary<string, SuffixGroup>>>();
					for (int i = 0; i < affixCount; i++)
					{
						string rule = reader.ReadString();
						int reCount = reader.ReadInt32();
						Dictionary<string, Dictionary<string, SuffixGroup>> ruleDict = new Dictionary<string, Dictionary<string, SuffixGroup>>();
						ruleDict["m:v_naz"] = new Dictionary<string, SuffixGroup>();
						ruleDict["m:v_rod"] = new Dictionary<string, SuffixGroup>();
						ruleDict["m:v_dav"] = new Dictionary<string, SuffixGroup>();
						ruleDict["m:v_zna"] = new Dictionary<string, SuffixGroup>();
						ruleDict["m:v_oru"] = new Dictionary<string, SuffixGroup>();
						ruleDict["m:v_mis"] = new Dictionary<string, SuffixGroup>();
						ruleDict["m:v_kly"] = new Dictionary<string, SuffixGroup>();
						
						ruleDict["f:v_naz"] = new Dictionary<string, SuffixGroup>();
						ruleDict["f:v_rod"] = new Dictionary<string, SuffixGroup>();
						ruleDict["f:v_dav"] = new Dictionary<string, SuffixGroup>();
						ruleDict["f:v_zna"] = new Dictionary<string, SuffixGroup>();
						ruleDict["f:v_oru"] = new Dictionary<string, SuffixGroup>();
						ruleDict["f:v_mis"] = new Dictionary<string, SuffixGroup>();
						ruleDict["f:v_kly"] = new Dictionary<string, SuffixGroup>();
						
						ruleDict["n:v_naz"] = new Dictionary<string, SuffixGroup>();
						ruleDict["n:v_rod"] = new Dictionary<string, SuffixGroup>();
						ruleDict["n:v_dav"] = new Dictionary<string, SuffixGroup>();
						ruleDict["n:v_zna"] = new Dictionary<string, SuffixGroup>();
						ruleDict["n:v_oru"] = new Dictionary<string, SuffixGroup>();
						ruleDict["n:v_mis"] = new Dictionary<string, SuffixGroup>();
						ruleDict["n:v_kly"] = new Dictionary<string, SuffixGroup>();
						
						ruleDict["p:v_naz"] = new Dictionary<string, SuffixGroup>();
						ruleDict["p:v_rod"] = new Dictionary<string, SuffixGroup>();
						ruleDict["p:v_dav"] = new Dictionary<string, SuffixGroup>();
						ruleDict["p:v_zna"] = new Dictionary<string, SuffixGroup>();
						ruleDict["p:v_oru"] = new Dictionary<string, SuffixGroup>();
						ruleDict["p:v_mis"] = new Dictionary<string, SuffixGroup>();
						ruleDict["p:v_kly"] = new Dictionary<string, SuffixGroup>();

						for (int j = 0; j < reCount; j++)
						{
							string re = reader.ReadString();
							bool has_re_neg = false;
							string re_neg = "";
							has_re_neg = re.Contains(" -");
							if (has_re_neg)
                            {
								string[] splits = re.Split(new[] { " -" }, StringSplitOptions.None);
								re = splits[0].Substring(0, splits[0].Length - 1);
								re_neg = splits[1];
							}
							int endCount = reader.ReadInt32();
							for (int k = 0; k < endCount; k++)
							{
								string fromm = reader.ReadString();
								string to = reader.ReadString();
								Suffix suf = new Suffix(fromm, to, "");
								int len = reader.ReadInt32(); // Кількість варіантів тегів (для різних родів)
								for (int l = 0; l < len; l++)
								{
									int v_len = reader.ReadInt32(); // Кількість відмінків для поточного роду
									if (v_len > 0)
									{
										string rid = reader.ReadString();
										for (int m = 0; m < v_len; m++)
										{
											string vidm = $"{rid}:{reader.ReadString()}";
											if (ruleDict.ContainsKey(vidm))
											{
												SuffixGroup sufGroup;
												if (!ruleDict[vidm].TryGetValue(re, out sufGroup))
                                                {
													sufGroup = has_re_neg ? new SuffixGroup(re, re_neg) : new SuffixGroup(re);
													ruleDict[vidm][re] = sufGroup;
												}
												sufGroup.AppendAffix(suf);
											}
										}
									}
								}
							}
						}
						m_affixRules[rule] = ruleDict;
					}
				}
				else
				{
					Debug.Print("Could not find resource file!");
				}
			}
		}
		private void SetMasculine()
        {
			_curGenderOrPlural = WordGenderOrPluralEnum.Masculine;
        }
		private void SetFeminine()
        {
			_curGenderOrPlural = WordGenderOrPluralEnum.Feminine;
        }
		private void SetNeuter()
        {
			_curGenderOrPlural = WordGenderOrPluralEnum.Neuter;
        }
		private void SetPlural()
        {
			_curGenderOrPlural = WordGenderOrPluralEnum.Plural;
        }
		public override CultureInfo CultureInfoForLanguage
		{
			get
			{
				return CultureInfo;
			}
		}
		public override void ClearTemporaryData()
		{
#if DEBUG
			Debug.Print("ClearTemporaryData();"); // Виводить повідомлення в лог гри "c:\ProgramData\Mount and Blade II Bannerlord\logs\rgl_log_#####.txt
#endif

			LinkList.Clear();
			WordGroups.Clear();
			WordGroupsNoTags.Clear();
			_doesComeFromWordGroup = false;

			_curGenderOrPlural = WordGenderOrPluralEnum.NoValue;
		}
		
		private static List<(string wordGroup, int firstMarkerPost)> WordGroups
		{
			get
			{
				if (_wordGroups == null)
				{
					_wordGroups = new List<(string wordGroup, int firstMarkerPost)>();
				}
				return _wordGroups;
			}
		}

		private static List<string> WordGroupsNoTags
		{
			get
			{
				if (_wordGroupsNoTags == null)
				{
					_wordGroupsNoTags = new List<string>();
				}
				return _wordGroupsNoTags;
			}
		}

		private static List<string> LinkList
		{
			get
			{
				if (_linkList == null)
				{
					_linkList = new List<string>();
				}
				return _linkList;
			}
		}

		private string LinkTag
		{
			get
			{
				return ".link";
			}
		}

		private int LinkTagLength
		{
			get
			{
				return 7;
			}
		}

		private string LinkStarter
		{
			get
			{
				return "<a style=\"Link.";
			}
		}

		private string LinkEnding
		{
			get
			{
				return "</b></a>";
			}
		}

		private int LinkEndingLength
		{
			get
			{
				return 8;
			}
		}

		private static char GetLastCharacter(StringBuilder outputString)
		{
			if (outputString.Length <= 0)
			{
				return '*';
			}
			return outputString[outputString.Length - 1];
		}

		private static string GetEnding(StringBuilder outputString, int numChars)
		{
			numChars = MathF.Min(numChars, outputString.Length);
			return outputString.ToString(outputString.Length - numChars, numChars);
		}
		public override void ProcessToken(string sourceText, ref int cursorPos, string token, StringBuilder outputString)
		{
#if DEBUG
			Debug.Print($"Start ProcessToken({sourceText}, {cursorPos}, {token}, {outputString})");
#endif

			bool flag = false;
			if (token == LinkTag)
			{
				LinkList.Add(sourceText.Substring(LinkTagLength));
			}
			else if (sourceText.Contains(LinkStarter))
			{
				flag = IsLink(sourceText, token.Length + 2, cursorPos);
			}
			if (flag)
			{
				cursorPos -= LinkEndingLength;
				outputString.Remove(outputString.Length - LinkEndingLength, LinkEndingLength);
			}
			int num2;
			if (token.EndsWith("Creator"))
			{
				outputString.Append("{" + token.Replace("Creator", "") + "}");
			}
			else if (GenderOrPluralTokens.HasToken(token))
			{
				if (token == ".m")
					SetMasculine();
				else if (token == ".f")
					SetFeminine();
				else if (token == ".n")
					SetNeuter();
				else
					SetPlural();
			}
			else if (WordCaseTokens.HasToken(token) && (!_doesComeFromWordGroup || (_doesComeFromWordGroup && _curGenderOrPlural == WordGenderOrPluralEnum.NoValue)) && IsWordGroup(token.Length, sourceText, cursorPos, out num2))
			{
				if (num2 >= 0)
				{
					_curGenderOrPlural = WordGenderOrPluralEnum.NoValue;

					AddSuffixWordGroup(token, num2, outputString);
				}
			}
			else if (token.StartsWith(".R_")) // Токен з правилами
            {
				string rule = token.Substring(3);
				bool hasVidm = rule.Length >= 7;
				string vidm = "";
				if (hasVidm)
				{
					vidm = rule.Substring(rule.Length - 5);
					hasVidm = WordCaseTokens.HasToken("."+vidm);
				}
				if (hasVidm)
				{
					rule = rule.Substring(0, rule.Length - 6);
				}
				else
					vidm = "v_naz";
				if (hasVidm || (_curGenderOrPlural != WordGenderOrPluralEnum.Masculine && _curGenderOrPlural != WordGenderOrPluralEnum.NoValue)) 
                {
					string[] s = rule.Split('_');
					List<string> rules = new List<string>();
					if (s.Length > 0)
						rules.Add(s[0]);
					for (int i = 1; i < s.Length; i++)
						rules.Add($"{s[0]}_{s[i]}");
					string fullCase;
					switch(_curGenderOrPlural)
                    {
						default://case WordGenderOrPluralEnum.Masculine:
							fullCase = "m:";
							break;
						case WordGenderOrPluralEnum.Feminine:
							fullCase = "f:";
							break;
						case WordGenderOrPluralEnum.Neuter:
							fullCase = "n:";
							break;
						case WordGenderOrPluralEnum.Plural:
							fullCase = "p:";
							break;
                    }
					fullCase = fullCase + vidm;

					int wordStart = sourceText.Remove(cursorPos - token.Length - 2).LastIndexOf('}') + 1;
					int wordLength = cursorPos - token.Length - 2 - wordStart;
					string word = sourceText.Substring(wordStart, wordLength);

					bool ruleFound = false;
					foreach (var r in rules)
                    {
						if (m_affixRules.ContainsKey(r))
                        {
							var allSuffixes = m_affixRules[r][fullCase];
							foreach (var sufPair in allSuffixes)
                            {
								if (sufPair.Value.Matches(word))
                                {
									var sufList = sufPair.Value.GetAffixes();
									if (sufList.Count > 0)
                                    {
										word = sufList[0].Apply(word);
										ruleFound = true;
										break;
                                    }
                                }
                            }
                        }
						if (ruleFound)
							break;
                    }
					outputString.Remove(outputString.Length - wordLength, wordLength);
					outputString.Append(word);
                }

				_curGenderOrPlural = WordGenderOrPluralEnum.NoValue;
				if (!_doesComeFromWordGroup)
					WordGroupProcessor(sourceText, cursorPos);
			}
			if (flag)
			{
				cursorPos += LinkEndingLength;
				outputString.Append(LinkEnding);
			}
#if DEBUG
			Debug.Print($"UkrainianTextProcessor.ProcessToken({sourceText}, {cursorPos}, {token}, {outputString}).");
#endif
		}

		private void AddSuffixWordGroup(string token, int wordGroupIndex, StringBuilder outputString)
		{
#if DEBUG
			Debug.Print($"AddSuffixWordGroup({token}, {wordGroupIndex})");
#endif
			bool flag = char.IsUpper(outputString[outputString.Length - WordGroupsNoTags[wordGroupIndex].Length]);
			string text = WordGroups[wordGroupIndex].wordGroup;
			outputString.Remove(outputString.Length - WordGroupsNoTags[wordGroupIndex].Length, WordGroupsNoTags[wordGroupIndex].Length);
			int tokenStart = text.IndexOf("{.R_");
			token = "_" + token.Substring(1);
			while (tokenStart != -1)
            {
				int tokenEnd = text.IndexOf('}', tokenStart + 4);
				if (tokenEnd != -1)
                {
					text = text.Insert(tokenEnd, token);
                }
				tokenStart = text.IndexOf("{.R_", tokenEnd + token.Length + 1);
            }
			_doesComeFromWordGroup = true;
			string text2 = base.Process(text);
			_doesComeFromWordGroup = false;
			if (flag && char.IsLower(text2[0]))
			{
				outputString.Append(char.ToUpperInvariant(text2[0]));
				outputString.Append(text2.Substring(1));
				return;
			}
			if (!flag && char.IsUpper(text2[0]))
			{
				outputString.Append(char.ToLowerInvariant(text2[0]));
				outputString.Append(text2.Substring(1));
				return;
			}
			outputString.Append(text2);
		}

		private bool IsWordGroup(int tokenLength, string sourceText, int curPos, out int wordGroupIndex)
		{
			int num = 0;
			while (num < WordGroupsNoTags.Count && curPos - tokenLength - 2 - WordGroupsNoTags[num].Length >= 0)
			{
				if (sourceText.Substring(curPos - tokenLength - 2 - WordGroupsNoTags[num].Length, WordGroupsNoTags[num].Length).Equals(WordGroupsNoTags[num]))
				{
					wordGroupIndex = num;
					return true;
				}
				num++;
			}
			wordGroupIndex = -1;
			return false;
		}

		private bool IsRecordedWithPreviousTag(string sourceText, int cursorPos)
		{
			foreach (var wg in WordGroups)
				if (wg.wordGroup == sourceText && wg.firstMarkerPost != cursorPos)
					return true;
			return false;
		}

		private void WordGroupProcessor(string sourceText, int cursorPos)
		{
#if DEBUG
			Debug.Print($"WordGroupProcessor({sourceText}, {cursorPos}); Count={WordGroups.Count}");
#endif
			if (!IsRecordedWithPreviousTag(sourceText, cursorPos))
			{
				WordGroups.Add(new ValueTuple<string, int>(sourceText, cursorPos));
				_doesComeFromWordGroup = true;
				string s = base.Process(sourceText);
				WordGroupsNoTags.Add(s);
#if DEBUG
				Debug.Print($"  WordGroupProcessor: WordGroupsNoTags.Add({s});");
#endif
				_doesComeFromWordGroup = false;
			}
		}

		private bool IsLink(string sourceText, int tokenLength, int cursorPos)
		{
			string text = sourceText.Remove(cursorPos - tokenLength);
			for (int i = 0; i < LinkList.Count; i++)
			{
				if (sourceText.Length >= LinkList[i].Length && text.EndsWith(LinkList[i]))
				{
					LinkList.RemoveAt(i);
					return true;
				}
			}
			return false;
		}

		private static readonly CultureInfo CultureInfo = new CultureInfo("uk-UA");

		[ThreadStatic]
		private static WordGenderOrPluralEnum _curGenderOrPlural = WordGenderOrPluralEnum.NoValue;

		[ThreadStatic]
		private static List<(string wordGroup, int firstMarkerPost)> _wordGroups = new List<(string wordGroup, int firstMarkerPost)>();

		[ThreadStatic]
		private static List<string> _wordGroupsNoTags = new List<string>();

		[ThreadStatic]
		private static List<string> _linkList = new List<string>();

		[ThreadStatic]
		private static bool _doesComeFromWordGroup = false;

		enum WordGenderOrPluralEnum
		{
			NoValue,
			Masculine,
			Feminine,
			Neuter,
			Plural
		}

		private static class GenderOrPluralTokens
		{
			public static bool HasToken(string token)
            {
				return Tokens.Contains(token);
            }

			public static HashSet<string> Tokens = new HashSet<string>(new string[] 
			{
				".m", 
				".f",
				".n",
				".p"
			});
		}
		private static class WordCaseTokens
        {
			public static bool HasToken(string token)
            {
				return Tokens.Contains(token);
            }
			public static HashSet<string> Tokens = new HashSet<string>(new string[]
			{
				".v_naz",
				".v_rod",
				".v_dav",
				".v_zna",
				".v_oru",
				".v_mis",
				".v_kly"
			});
        }
	}
}

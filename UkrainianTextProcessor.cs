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
		private Dictionary<string, Dictionary<string, (List<SuffixGroup> list, Dictionary<string, SuffixGroup> dic)>> m_affixRules = null;
		private void ReadRulesData()
		{
			using (MemoryStream resourceStream = new MemoryStream(AffixResource.rules))
			{
				if (resourceStream != null)
				{
					System.IO.BinaryReader reader = new System.IO.BinaryReader(resourceStream);

					int affixCount = reader.ReadInt32();
					m_affixRules = new Dictionary<string, Dictionary<string, (List<SuffixGroup>, Dictionary<string, SuffixGroup>)>>();
					for (int i = 0; i < affixCount; i++)
					{
						string rule = reader.ReadString();
						int reCount = reader.ReadInt32();
						Dictionary<string, (List<SuffixGroup> list, Dictionary<string, SuffixGroup> dict)> ruleDict = new Dictionary<string, (List<SuffixGroup>, Dictionary<string, SuffixGroup>)>();
						ruleDict["m:v_naz"] = (new List<SuffixGroup>(), new Dictionary<string, SuffixGroup>());
						ruleDict["m:v_rod"] = (new List<SuffixGroup>(), new Dictionary<string, SuffixGroup>());
						ruleDict["m:v_dav"] = (new List<SuffixGroup>(), new Dictionary<string, SuffixGroup>());
						ruleDict["m:v_zna"] = (new List<SuffixGroup>(), new Dictionary<string, SuffixGroup>());
						ruleDict["m:v_oru"] = (new List<SuffixGroup>(), new Dictionary<string, SuffixGroup>());
						ruleDict["m:v_mis"] = (new List<SuffixGroup>(), new Dictionary<string, SuffixGroup>());
						ruleDict["m:v_kly"] = (new List<SuffixGroup>(), new Dictionary<string, SuffixGroup>());

						ruleDict["f:v_naz"] = (new List<SuffixGroup>(), new Dictionary<string, SuffixGroup>());
						ruleDict["f:v_rod"] = (new List<SuffixGroup>(), new Dictionary<string, SuffixGroup>());
						ruleDict["f:v_dav"] = (new List<SuffixGroup>(), new Dictionary<string, SuffixGroup>());
						ruleDict["f:v_zna"] = (new List<SuffixGroup>(), new Dictionary<string, SuffixGroup>());
						ruleDict["f:v_oru"] = (new List<SuffixGroup>(), new Dictionary<string, SuffixGroup>());
						ruleDict["f:v_mis"] = (new List<SuffixGroup>(), new Dictionary<string, SuffixGroup>());
						ruleDict["f:v_kly"] = (new List<SuffixGroup>(), new Dictionary<string, SuffixGroup>());

						ruleDict["n:v_naz"] = (new List<SuffixGroup>(), new Dictionary<string, SuffixGroup>());
						ruleDict["n:v_rod"] = (new List<SuffixGroup>(), new Dictionary<string, SuffixGroup>());
						ruleDict["n:v_dav"] = (new List<SuffixGroup>(), new Dictionary<string, SuffixGroup>());
						ruleDict["n:v_zna"] = (new List<SuffixGroup>(), new Dictionary<string, SuffixGroup>());
						ruleDict["n:v_oru"] = (new List<SuffixGroup>(), new Dictionary<string, SuffixGroup>());
						ruleDict["n:v_mis"] = (new List<SuffixGroup>(), new Dictionary<string, SuffixGroup>());
						ruleDict["n:v_kly"] = (new List<SuffixGroup>(), new Dictionary<string, SuffixGroup>());

						ruleDict["p:v_naz"] = (new List<SuffixGroup>(), new Dictionary<string, SuffixGroup>());
						ruleDict["p:v_rod"] = (new List<SuffixGroup>(), new Dictionary<string, SuffixGroup>());
						ruleDict["p:v_dav"] = (new List<SuffixGroup>(), new Dictionary<string, SuffixGroup>());
						ruleDict["p:v_zna"] = (new List<SuffixGroup>(), new Dictionary<string, SuffixGroup>());
						ruleDict["p:v_oru"] = (new List<SuffixGroup>(), new Dictionary<string, SuffixGroup>());
						ruleDict["p:v_mis"] = (new List<SuffixGroup>(), new Dictionary<string, SuffixGroup>());
						ruleDict["p:v_kly"] = (new List<SuffixGroup>(), new Dictionary<string, SuffixGroup>());

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
												if (!ruleDict[vidm].dict.TryGetValue(re, out sufGroup))
												{
													sufGroup = has_re_neg ? new SuffixGroup(re, re_neg) : new SuffixGroup(re);
													ruleDict[vidm].dict[re] = sufGroup;
													if (v_len == 1)
													{
														ruleDict[vidm].list.Insert(0, sufGroup);
													}
													else
														ruleDict[vidm].list.Add(sufGroup);
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
			_needUpdateSourceText = false;

			_curGenderOrPlural = WordGenderOrPluralEnum.NoValue;

			_caseStartPosition = -1;
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

		private string CaseStartTag
		{
			get
			{
				return ".S";
			}
		}

		private string CopyRuleStartTag
        {
			get
            {
				return ".CS";
            }
        }
		private string CopyRuleEndTag
        {
			get
            {
				return ".CE";
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
			Debug.Print($"{_debugSpace}>> START [ProcessToken({sourceText}, {cursorPos}, {token}, {outputString})]");
#endif
			if (token == CaseStartTag)
			{
				if (_processingLevel == 0)
					_caseStartTagsCount++;
				_caseStartPosition = outputString.Length;
				return;
			}
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
			int num, pos;
			if (token.Length > 7 && token.EndsWith("Creator"))
			{
				// Removing "Creator" from the end of the token
				outputString.Append("{" + token.Remove(token.Length - 7, 7) + "}");
			}
			else if (token.Length == 3 && token == CopyRuleEndTag)
			{
				if (_processingLevel == 0)
					_copyRuleEndTagsCount++;
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
			else if (WordCaseTokens.HasToken(token) && IsWordGroup(token.Length, outputString, cursorPos, out num, out pos))
			{
				if (_processingLevel == 0)
					_caseTagsCount++;
				if (num >= 0)
				{
					_curGenderOrPlural = WordGenderOrPluralEnum.NoValue;

#if DEBUG
					_debugSpace += "  ";
#endif
					AddSuffixWordGroup(token, num, pos, outputString);
#if DEBUG
					_debugSpace = _debugSpace.Remove(0, 2);
#endif

					_caseStartPosition = -1;
				}
			}
			else if (token.StartsWith(".R_")) // Токен з правилами
			{
				string rule = token.Substring(3);
				bool hasVidm = false;
				string vidm = "";
				if (rule.Length >= 7)
				{
					vidm = rule.Substring(rule.Length - 5);
					hasVidm = WordCaseTokens.HasToken("." + vidm);
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
					{
						// First search in sub-rules
						for (int i = s.Length-1; i > 0; i--)
							rules.Add($"{s[0]}_{s[i]}");
						rules.Add(s[0]);
					}
					string fullCase;
					switch (_curGenderOrPlural)
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
							foreach (var sufPair in allSuffixes.list)
							{
								if (sufPair.Matches(word))
								{
									var sufList = sufPair.GetAffixes();
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
					if (!ruleFound && _curGenderOrPlural == WordGenderOrPluralEnum.Plural)
                    {
						// Правило не знайшли, але все ще необхідно змінити слово, оскільки воно в множині
						fullCase = "p:v_naz";
						foreach (var r in rules)
						{
							if (m_affixRules.ContainsKey(r))
							{
								var allSuffixes = m_affixRules[r][fullCase];
								foreach (var sufPair in allSuffixes.list)
								{
									if (sufPair.Matches(word))
									{
										var sufList = sufPair.GetAffixes();
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
					}
					outputString.Remove(outputString.Length - wordLength, wordLength);
					outputString.Append(word);
				}

				_curGenderOrPlural = WordGenderOrPluralEnum.NoValue;
				if (!_doesComeFromWordGroup)
				{
#if DEBUG
					_debugSpace += "  ";
#endif
					string outString = WordGroupProcessor(sourceText, cursorPos);
#if DEBUG
					_debugSpace = _debugSpace.Remove(0, 2);
#endif
					outputString.Clear();
					outputString.Append(outString);
					cursorPos = sourceText.Length;
					return;
				}
			}
			else if (WordCaseTokens.HasToken(token) && _processingLevel == 0)
			{
				_caseTagsCount++;
			}
		
			if (flag)
			{
				cursorPos += LinkEndingLength;
				outputString.Append(LinkEnding);
			}
#if DEBUG
			Debug.Print($"{_debugSpace}>> END [ProcessToken({sourceText}, {cursorPos}, {token}, {outputString})]");
#endif
		}

		private void AddSuffixWordGroup(string token, int wordGroupIndex, int wordGroupPos, StringBuilder outputString)
		{
#if DEBUG
			Debug.Print($"{_debugSpace}>> START AddSuffixWordGroup({token}, {wordGroupIndex}, {wordGroupPos}, {outputString.ToString()})");
#endif
			string crrWordGroup = WordGroupsNoTags[wordGroupIndex];
			int wordGroupLength = crrWordGroup.Length;
			bool flag = char.IsUpper(outputString[wordGroupPos]);//outputString.Length - wordGroupLength]);

			string text = WordGroups[wordGroupIndex].wordGroup;
			outputString.Remove(wordGroupPos, wordGroupLength);//outputString.Length - wordGroupLength, wordGroupLength);
			int updIdx = -1;
			if (_needUpdateSourceText)
			{
				updIdx = _updatedSourceText.LastIndexOf(crrWordGroup);
#if DEBUG
				Debug.Print($"{_debugSpace}>>   {_updatedSourceText} - {crrWordGroup} - [{updIdx}]");
#endif
				if (updIdx != -1)
					_updatedSourceText = _updatedSourceText.Remove(updIdx, crrWordGroup.Length);
			}
			int tokenStart = text.IndexOf("{.R_");
			string tokenRule = "_" + token.Substring(1);
			while (tokenStart != -1)
			{
				int tokenEnd = text.IndexOf('}', tokenStart + 4);
				if (tokenEnd != -1)
				{
					text = text.Insert(tokenEnd, tokenRule);
				}
				tokenStart = text.IndexOf("{.R_", tokenEnd + tokenRule.Length + 1);
			}

			bool origComeFromWordGroup = _doesComeFromWordGroup;
			int origCaseStartPosition = _caseStartPosition;

			_caseStartPosition = -1;
			_doesComeFromWordGroup = true;
#if DEBUG
			_debugSpace += "  ";
#endif
			_processingLevel++;
			string text2 = base.Process(text);
			_processingLevel--;
#if DEBUG
			_debugSpace = _debugSpace.Remove(0, 2);
#endif

			_doesComeFromWordGroup = origComeFromWordGroup;
			_caseStartPosition = origCaseStartPosition;

			if (updIdx != -1)
			{
				_updatedSourceText = _updatedSourceText.Insert(updIdx, text2);
				string tokenStr = "{" + token + "}";
				int tokenIdx = _updatedSourceText.LastIndexOf(tokenStr);
				while (tokenIdx != -1)
                {
					_updatedSourceText = _updatedSourceText.Remove(tokenIdx);
					if (_processingLevel == 0)
						_caseTagsCount--;
					tokenIdx = _updatedSourceText.LastIndexOf(tokenStr, tokenIdx);
				}

#if DEBUG
				Debug.Print($"{_debugSpace}>>   {_updatedSourceText} - {text2}");
#endif
			}

			if (flag && char.IsLower(text2[0]))
			{
				/*outputString.Append(char.ToUpperInvariant(text2[0]));
				outputString.Append(text2.Substring(1));*/
				outputString.Insert(wordGroupPos, char.ToUpperInvariant(text2[0]));
				outputString.Insert(wordGroupPos + 1, text2.Substring(1));
				WordGroups.RemoveAt(wordGroupIndex);
				WordGroupsNoTags.RemoveAt(wordGroupIndex);
#if DEBUG
				Debug.Print($"{_debugSpace}>> END AddSuffixWordGroup({token}, {wordGroupIndex}, {wordGroupPos}, {outputString.ToString()})");
#endif
				return;
			}
			if (!flag && char.IsUpper(text2[0]))
			{
				/*outputString.Append(char.ToLowerInvariant(text2[0]));
				outputString.Append(text2.Substring(1));*/
				outputString.Insert(wordGroupPos, char.ToLowerInvariant(text2[0]));
				outputString.Insert(wordGroupPos + 1, text2.Substring(1));
				WordGroups.RemoveAt(wordGroupIndex);
				WordGroupsNoTags.RemoveAt(wordGroupIndex);
#if DEBUG
				Debug.Print($"{_debugSpace}>> END AddSuffixWordGroup({token}, {wordGroupIndex}, {wordGroupPos}, {outputString.ToString()})");
#endif
				return;
			}
			//outputString.Append(text2);
			outputString.Insert(wordGroupPos, text2);
			WordGroups.RemoveAt(wordGroupIndex);
			WordGroupsNoTags.RemoveAt(wordGroupIndex);
#if DEBUG
			Debug.Print($"{_debugSpace}>> END AddSuffixWordGroup({token}, {wordGroupIndex}, {wordGroupPos}, {outputString.ToString()})");
#endif
		}

		private bool IsWordGroup(int tokenLength, StringBuilder outputText, int curPos, out int wordGroupIndex, out int wordGroupPos)
		{
			string outputString = outputText.ToString();
#if DEBUG
			Debug.Print($"{_debugSpace}  IsWordGroup({tokenLength}, {outputString}, {curPos})");
			Debug.Print($"{_debugSpace}    WordGroupsNoTags.Count = {WordGroupsNoTags.Count}, ({string.Join(",", WordGroupsNoTags)})");
#endif

			int num = 0;
			while (num < WordGroupsNoTags.Count && outputString.Length - WordGroupsNoTags[num].Length >= 0)
			{
				string crrWorkGroup = WordGroupsNoTags[num];
				if (outputString.Substring(outputString.Length - crrWorkGroup.Length, crrWorkGroup.Length).Equals(crrWorkGroup, StringComparison.OrdinalIgnoreCase))
				{
					wordGroupIndex = num;
					wordGroupPos = outputString.Length - crrWorkGroup.Length;
					return true;
				}
				num++;
			}
			// If we do not found any word group
			// Then looking for the word group closest to the end
			int maxPos = 0;
			num = 0;
			wordGroupIndex = -1;
			wordGroupPos = -1;
			foreach (var crrWorkGroup in WordGroupsNoTags)
			{
				int crrPos = outputString.LastIndexOf(crrWorkGroup, outputString.Length, outputString.Length - (_caseStartPosition < 0 ? 0 : _caseStartPosition));
				if (crrPos > maxPos)
				{
					maxPos = crrPos;
					wordGroupPos = maxPos;
					wordGroupIndex = num;
				}
				num++;
			}
			return wordGroupIndex != -1;
		}

		private bool IsRecordedWithPreviousTag(string sourceText, int cursorPos, out string processedText)
		{
			int idx = 0;
			foreach (var wg in WordGroups)
			{
				if (wg.wordGroup == sourceText && wg.firstMarkerPost != cursorPos)
				{
					processedText = WordGroupsNoTags[idx];
					return true;
				}
				idx++;
			}
			processedText = "";
			return false;
		}

		private string WordGroupProcessor(string sourceText, int cursorPos)
		{
#if DEBUG
			Debug.Print($"{_debugSpace}>>START WordGroupProcessor({sourceText}, {cursorPos}); Count={WordGroups.Count}");
#endif
			string res;
			if (!IsRecordedWithPreviousTag(sourceText, cursorPos, out res))
			{
				_needUpdateSourceText = true;
				_updatedSourceText = sourceText;
				_doesComeFromWordGroup = true;
#if DEBUG
				_debugSpace += "  ";
#endif
				_copyRuleEndTagsCount = 0;
				_caseTagsCount = 0;
				_caseStartTagsCount = 0;
				_processingLevel = 0;
				res = base.Process(sourceText);
#if DEBUG
				_debugSpace = _debugSpace.Remove(0, 2);
#endif
				_needUpdateSourceText = false;

				// Processing copy rules tags
				if (_copyRuleEndTagsCount > 0)
				{
					int crEndIdx = _updatedSourceText.IndexOf("{" + CopyRuleEndTag + "}");
					int crStartIdx = -1;
#if DEBUG
					Debug.Print($"{_debugSpace}  Has CopyRuleTag == true, crEndIdx = {crEndIdx}, _updatedSourceText = \"{_updatedSourceText}\"");
#endif
					var wdToDelSet = new HashSet<int>();
					int crrCopyRuleEndTagsCount = _copyRuleEndTagsCount;
					int minIdx = 0;
					while (crrCopyRuleEndTagsCount > 0 && crEndIdx != -1)
					{
						crStartIdx = _updatedSourceText.LastIndexOf("{" + CopyRuleStartTag + "}", crEndIdx);
						if (crStartIdx != -1)
							crStartIdx += 5; // Skip Tag
						else
							minIdx = 0;
						string wordS = _updatedSourceText.Substring(crStartIdx, crEndIdx - crStartIdx);
#if DEBUG
						Debug.Print($"{_debugSpace}  crStartIdx = {crStartIdx}, wordS = {wordS}");
#endif
						int wordIdx = 0;
						bool found = false;
						foreach (var crrWordNoTags in WordGroupsNoTags)
                        {
							if (crrWordNoTags.Equals(wordS))
							{
								found = true;
								break;
							}
							wordIdx++;
                        }
#if DEBUG
						Debug.Print($"{_debugSpace}  found = {found}, wordIdx = {wordIdx}");
#endif
						if (found)
                        {
							crStartIdx -= 5;
							_updatedSourceText = _updatedSourceText.Remove(crStartIdx, wordS.Length + 10);
							_updatedSourceText = _updatedSourceText.Insert(crStartIdx, WordGroups[wordIdx].wordGroup);
#if DEBUG
							Debug.Print($"{_debugSpace}  _updatedSourceText = \"{_updatedSourceText}\"");
#endif
							wdToDelSet.Add(wordIdx);
                        }

						crrCopyRuleEndTagsCount--;
						if (crrCopyRuleEndTagsCount > 0)
						{
							crEndIdx += 5;
							minIdx = crEndIdx;
							crEndIdx = _updatedSourceText.IndexOf("{" + CopyRuleEndTag + "}", crEndIdx);
						}
					}

					if (_copyRuleEndTagsCount == 1 || wdToDelSet.Count == 1)
                    {
						int wordIdx = wdToDelSet.First();
						WordGroups.RemoveAt(wordIdx);
						WordGroupsNoTags.RemoveAt(wordIdx);
					}
					else
                    {
						// Getting indices sorted from biggest to smallest
						var sortedIdxs = wdToDelSet.OrderByDescending(x => x).ToList();
						foreach (var wordIdx in sortedIdxs)
                        {
							WordGroups.RemoveAt(wordIdx);
							WordGroupsNoTags.RemoveAt(wordIdx);
						}
					}
				}
				// Cleaning from other tags
				if (_caseTagsCount > 0 || _caseStartTagsCount > 0)
				{
#if DEBUG
					Debug.Print($"{_debugSpace}  Cleaning tags: \"{_updatedSourceText}\"");
					Debug.Print($"{_debugSpace}  _caseTagsCount = {_caseTagsCount} && _caseStartTagsCount = {_caseStartTagsCount}");
#endif
					var tagEndIdx = _updatedSourceText.LastIndexOf('}');
					while (tagEndIdx > 0 && (_caseTagsCount > 0 || _caseStartTagsCount > 0))
					{
						var tagStartIdx = _updatedSourceText.LastIndexOf('{', tagEndIdx);
						if (tagStartIdx == -1)
							break;
						var tag = _updatedSourceText.Substring(tagStartIdx + 1, tagEndIdx - tagStartIdx - 1);
#if DEBUG
						Debug.Print($"{_debugSpace}  Found Tag: tagStartIdx = {tagStartIdx}, tagEndIdx = {tagEndIdx}, tag = {tag}");
#endif
						if (WordCaseTokens.HasToken(tag))
                        {
							_updatedSourceText = _updatedSourceText.Remove(tagStartIdx, tagEndIdx - tagStartIdx + 1);
							_caseTagsCount--;
						}
						else if (tag == CaseStartTag)
                        {
							_updatedSourceText = _updatedSourceText.Remove(tagStartIdx, tagEndIdx - tagStartIdx + 1);
							_caseStartTagsCount--;
						}
						if (_caseTagsCount > 0 || _caseStartTagsCount > 0)
							tagEndIdx = _updatedSourceText.LastIndexOf('}', tagStartIdx - 1);
						else
							tagEndIdx = 0;
					}
				}

				WordGroups.Add(new ValueTuple<string, int>(_updatedSourceText, cursorPos));
				WordGroupsNoTags.Add(res);
#if DEBUG
				Debug.Print($"{_debugSpace}  WordGroupProcessor: WordGroupsNoTags.Add({_updatedSourceText} - {res});");
#endif
				_doesComeFromWordGroup = false;
			}
			else
			{
#if DEBUG
				Debug.Print($"{_debugSpace}  WordGroupProcessor: HERE!!! IsRecordedWithPreviousTag({sourceText}, {cursorPos}) == true");
#endif
			}

#if DEBUG
			Debug.Print($"{_debugSpace}>>END WordGroupProcessor({sourceText}, {cursorPos}); Count={WordGroups.Count}");
#endif
			return res;
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

#if DEBUG
		private string _debugSpace = "";
#endif

		private static readonly CultureInfo CultureInfo = new CultureInfo("uk-UA");

		[ThreadStatic]
		private static int _caseStartPosition = -1;

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

		[ThreadStatic]
		private static bool _needUpdateSourceText = false;

		[ThreadStatic]
		private static string _updatedSourceText = "";

		[ThreadStatic]
		private static int _processingLevel = 0;

		[ThreadStatic]
		private static int _copyRuleEndTagsCount = 0;

		[ThreadStatic]
		private static int _caseStartTagsCount = 0;

		[ThreadStatic]
		private static int _caseTagsCount = 0;

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
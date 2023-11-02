using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace CustomTextProcessor
{
    public class SuffixGroup
    {
        private readonly string match;
        private readonly string neg_match;
        private readonly List<Suffix> affixes = new List<Suffix>();
        private readonly Regex match_ends_re;
        private readonly Regex neg_match_ends_re;
        private int counter;

        public SuffixGroup(string match_)
        {
            match = match_;
            neg_match = null;
            match_ends_re = new Regex(match_ + "$");
            neg_match_ends_re = null;
            counter = 0;
        }

        public SuffixGroup(string match_, string neg_match_)
        {
            match = match_;
            neg_match = neg_match_;
            match_ends_re = new Regex(match_ + "$");
            neg_match_ends_re = neg_match_ != null ? new Regex(neg_match_ + "$") : null;
            counter = 0;
        }

        public bool Matches(string word)
        {
            return match_ends_re.IsMatch(word) && (neg_match == null || !neg_match_ends_re.IsMatch(word));
        }

        public void AppendAffix(Suffix affixObj)
        {
            affixes.Add(affixObj);
        }

        public List<Suffix> GetAffixes()
        {
            return affixes;
        }

    }
}
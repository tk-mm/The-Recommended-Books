using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Linq.Mapping;

namespace WebInfo
{
    [Table(Name = "NegaposiTable")]
    class Negaposi
    {
        public Negaposi() { }

        public Negaposi(string word)
        {
            this.word = word;
            this.point = 0;
        }

        public override string ToString()
        {
            return word + point;
        }

        [Column(Name = "word", DbType = "TEXT")]
        public string word { get; set; }

        [Column(Name = "read", DbType = "TEXT")]
        public string read { get; set; }

        [Column(Name = "wordclass", DbType = "TEXT")]
        public string wordclass { get; set; }

        [Column(Name = "point", DbType = "NUMERIC")]
        public double point { get; set; }

        public override bool Equals(object obj)
        {
            try
            {
                return this.word == ((Negaposi)obj).word;
            }
            catch (NullReferenceException)
            {
                return false;
            }
            catch (InvalidCastException)
            {
                return false;
            }
        }

        public static bool operator ==(Negaposi x, Negaposi y)
        {
            return x.Equals(y);
        }

        public static bool operator !=(Negaposi x, Negaposi y)
        {
            return !x.Equals(y);
        }

        public static bool operator ==(Negaposi x, string y)
        {
            try
            {
                return x.word == y;
            }
            catch (NullReferenceException)
            {
                return false;
            }
        }

        public static bool operator !=(Negaposi x, string y)
        {
            return !(x == y);
        }

        public override int GetHashCode()
        {
            return this.word.GetHashCode();
        }
    }
}

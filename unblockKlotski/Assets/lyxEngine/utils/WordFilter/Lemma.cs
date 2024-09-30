
using sw.util;
namespace sw.game.wordfilter
{
	public class Lemma
	{
		public int prop;
		public int len;
		public int poff;
        public Lemma Clone()
        {
            return this.MemberwiseClone() as Lemma;
        }
        public void CopyTo(Lemma other)
        {
            other.prop = prop;
            other.len = len;
            other.poff = poff;
        }
        public void readData(ByteArray data)
		{
			int _startPos = data.position;
			int _len = data.readInt();
			prop = data.readInt();
			len = data.readInt();
			poff = data.readInt();
			//if(_len != data.position - _startPos){throw new Error("invalid data");}
		}
	}
}

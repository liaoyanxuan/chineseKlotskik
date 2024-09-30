
using sw.util;
namespace sw.game.wordfilter
{

	public class dm_entry_t
	{
		public int value;
		public int lemma_pos;
		public int suffix_pos;

		public void readData(ByteArray data)
		{
			int _startPos = data.position;
			int _len  = data.readInt();
			value = data.readInt();
			lemma_pos = data.readInt();
			suffix_pos = data.readInt();
			//if(_len != data.position - _startPos){throw new Error("invalid data");}
		}
	}
}

 
namespace sw.game.wordfilter
{
	public interface IWordFilter
	{
		string replace(string str);
		bool contain(string str);
		void loadCfg(WordFilterCfg cfg);
		bool replaceToRed(string str,out string s);
		void LoadDict(string[] arr);

	}
}

using sw.util;
using System.Collections.Generic;
 
namespace sw.game.wordfilter
{
	public class WordFilterCfg
	{
		public List<dm_entry_t> dentry; //  存放树的每个节点
		public List<sufentry> seinfo;
		public int sebufsize;
		public List<Lemma> lmlist; // 存放完成匹配对应的某模式
		public int entrance;
		public  WordFilterCfg()
		{
		}

		public void readData(ByteArray data)
		{
			dentry = new List<dm_entry_t>();
			int len_dentry = data.readInt();
			for(int i = 0 ;i < len_dentry;i ++)
			{
				dm_entry_t dm_entry_tCfg;
				dm_entry_tCfg = new dm_entry_t();
				dm_entry_tCfg.readData(data);
				dentry.Add(dm_entry_tCfg);
			}
			
			seinfo = new List<sufentry>();
			int len_seinfo = data.readInt();
			for(int i = 0 ;i < len_seinfo;i ++)
			{
				sufentry sufentryCfg;
				sufentryCfg = new sufentry();
				sufentryCfg.readData(data);
				seinfo.Add(sufentryCfg);
			}
			
			sebufsize = data.readInt();
			
			lmlist = new List<Lemma>();
			int len_lmlist = data.readInt();
			for(int i = 0 ;i < len_lmlist;i ++)
			{
				Lemma LemmaCfg;
				LemmaCfg = new Lemma();
				LemmaCfg.readData(data);
				lmlist.Add(LemmaCfg);
			}
			
			entrance = data.readInt();
		}
	}
}

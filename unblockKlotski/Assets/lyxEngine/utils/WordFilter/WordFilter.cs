
using System.Collections.Generic;
using System.Collections;
 
using sw.util;
using System;
namespace sw.game.wordfilter{
	public class WordFilter :IWordFilter
	{
		public  WordFilter()
		{
		}		
		private const int  DM_DENTRY_NULL = -1;
		private const int DM_DENTRY_FIRST = -2;
		private const int DM_SUFENTRY_NULL = -1;
		private const int DM_LEMMA_NULL = -1;
		private const int DM_COMMON_NULL = -1;
		private const int DM_OUT_FMM = 1; 
		private const int DM_OUT_ALL = 2;
		private const int DM_OUT_CONTAIN = 3;
		private const string DEFAULT_REPLACEMENT="*";
		private const int  DM_DEFAULT_SEBUFSIZE = 1024000;
		public WordFilterCfg cfg;
		private  void Init()
		{
			cfg = new WordFilterCfg();
			cfg.dentry = new List<dm_entry_t>();
			cfg.seinfo = new List<sufentry>();
			cfg.sebufsize = DM_DEFAULT_SEBUFSIZE;
			cfg.lmlist = new List<Lemma>();
			
			cfg.entrance = 0;
			sufentry s = InitSufentry(1, DM_DENTRY_FIRST);
			cfg.seinfo.Add(s);
		}
		
		public  void LoadDict(string[] arr)
		{
			Init();
			int cnt = 0;
			foreach(string  w in arr)
			{
				int prop = 0; //字典中所有term属性均设置为0，如有需要可以修改选择从字典加载
				cnt++;
				if(cnt %1000 == 0)
				{					//LoggerHelper.Debug("cnt:"+cnt);				
}

				if (AddLemma(w, prop) < 0)
				{
					return;
				}
			}
		}
        List<Lemma> lemmaPool = new List<Lemma>();
        Lemma GetLemma()
        {
            if(lemmaPool.Count == 0)
            {
                return new Lemma();
            }
            Lemma l = lemmaPool[lemmaPool.Count - 1];
            lemmaPool.RemoveAt(lemmaPool.Count - 1);
            return l;
        }
        void DisposeLemma(Lemma l)
        {
            lemmaPool.Add(l);
        }
		/* (non-Javadoc)
		* @see com.wanpu.common.service.impl.WordSearch#Search(java.lang.String, int)
		*/
		public  List<Lemma>  Search(string inbuf,int  opertype)
		{
			List<Lemma> dm_r = new List<Lemma>();
			int bpos = 0;
			int pos = 0;
			int nextpos = 0;
			int nde = 0;
			int lde = DM_DENTRY_FIRST;
			int lmpos = DM_LEMMA_NULL;
			int slemma = DM_LEMMA_NULL;
			
			if (opertype == DM_OUT_FMM)
			{
				while (pos < inbuf.Length)
				{
					bpos = pos;
					nextpos = pos + 1;
					while (pos < inbuf.Length)
					{
						nde = SeekEntry(lde,(int)(inbuf[pos]));
						if (nde == DM_DENTRY_NULL)
						{
							break;
						}
						lmpos = cfg.dentry[nde].lemma_pos;
						if (lmpos != DM_LEMMA_NULL)
						{
							slemma = lmpos;
							nextpos = pos + 1;
						}
						
						lde = nde;
						pos++;
					}
					
					if (slemma != DM_LEMMA_NULL)
					{
						cfg.lmlist[slemma].poff = bpos;
                        Lemma l = GetLemma();
                        cfg.lmlist[slemma].CopyTo(l);
						dm_r.Add(l);
					}
					
					lde = DM_DENTRY_FIRST;
					slemma = DM_LEMMA_NULL;
					pos = nextpos;
				}
			}
			else if (opertype == DM_OUT_ALL || opertype == DM_OUT_CONTAIN)
			{
				while (pos < inbuf.Length)
				{
					bpos = pos;
					nextpos = pos + 1;
					
					while (pos < inbuf.Length)
					{
						nde = SeekEntry(lde, (int)(inbuf[pos]));
						if (nde == DM_DENTRY_NULL)
						{
							break;
						}
						
						lmpos = cfg.dentry[nde].lemma_pos;
						if (lmpos != DM_LEMMA_NULL)
						{
							
							
							cfg.lmlist[lmpos].poff = bpos;
							dm_r.Add(cfg.lmlist[lmpos]);
							if(opertype == DM_OUT_CONTAIN)
								return dm_r;
						}
						lde = nde;
						pos++;
					}
					
					lde = DM_DENTRY_FIRST;
					pos = nextpos;
				}
			}
			
			return dm_r;
		}
		
		private  int SeekEntry(int lde,int  value)
		{
			int sufpos;
			int nde;
			int hsize;
			int hpos;
			
			if (lde == DM_DENTRY_FIRST)
			{
				sufpos = cfg.entrance;
			}
			else
			{
				sufpos = cfg.dentry[lde].suffix_pos;
			}
			if (sufpos == DM_SUFENTRY_NULL)
			{
				return DM_DENTRY_NULL;
			}
			hsize = cfg.seinfo[sufpos].hashsize;
			hpos = value % hsize;
			if (((nde = cfg.seinfo[sufpos].hashList[hpos]) == DM_DENTRY_NULL)
				|| (cfg.dentry[nde].value != value))
			{
				return DM_DENTRY_NULL;
			}
			else
			{
				return nde;
			}
		}
		
		private  int AddLemma(string strTerm,int  prop)
		{
			if(strTerm.Length==0)
				return 0;
			int curpos = 0;
			int last_depos = DM_DENTRY_FIRST;
			int cur_depos = DM_COMMON_NULL;
			int value = 0;
			int lmpos = DM_COMMON_NULL;
			
			//检查lemma是否已经在字典中存在
			if ((lmpos = SeekLemma(strTerm)) == DM_LEMMA_NULL)
			{
				//新lemma，插入到字典中
				while (curpos < strTerm.Length)
				{
					value = (int)(strTerm[curpos]);
					curpos++;
					
					cur_depos = InsertDentry(last_depos, value, cur_depos);
					last_depos = cur_depos;
				}
				
				cfg.dentry[cur_depos].lemma_pos = cfg.lmlist.Count;
				
				Lemma lm = new Lemma();
				lm.prop = prop;
				lm.len = strTerm.Length;
				
				cfg.lmlist.Add(lm);
				
				return 1;
			}
			return 0;
		}
		
		private  int InsertDentry(int lastpos,int  value,int  curpos)
		{
			int tmpdepos;
			int curdepos;
			int sufpos;
			int hsize;
			int hpos;
			
			if (lastpos != DM_DENTRY_FIRST)
			{
				sufpos = cfg.dentry[lastpos].suffix_pos;
			}
			else
			{
				sufpos = cfg.entrance;
			}
			
			if (sufpos == DM_SUFENTRY_NULL)
			{
				if (cfg.seinfo.Count > cfg.sebufsize)
				{
					if (!ResizeInfo())
					{
						return -1;
						
					}
				}
				
				cfg.dentry[lastpos].suffix_pos = cfg.seinfo.Count;
				sufentry s = InitSufentry(1, lastpos);
				cfg.seinfo.Add(s);
				sufpos = cfg.dentry[lastpos].suffix_pos;
			}
			
			
			hsize = cfg.seinfo[sufpos].hashsize;
			hpos = value % hsize;
			tmpdepos = cfg.seinfo[sufpos].hashList[hpos];
			if ((tmpdepos != DM_DENTRY_NULL) && (cfg.dentry[tmpdepos].value == value))
			{
				curpos = tmpdepos;
				return curpos;
			}
			else
			{ 
				dm_entry_t det = new dm_entry_t();
				det.value = value;
				det.lemma_pos = DM_LEMMA_NULL;
				det.suffix_pos = DM_SUFENTRY_NULL;
				
				curdepos = cfg.dentry.Count;
				cfg.dentry.Add(det);
				
				if (tmpdepos == DM_DENTRY_NULL)
				{
					cfg.seinfo[sufpos].hashList[hpos]=curdepos;
					curpos = curdepos;
					return curpos;
				}
				else
				{
					int newhash;
					for (newhash = hsize + 1; ; newhash++) 	
					{
						int conflict = 0;
						if (cfg.seinfo.Count > cfg.sebufsize)
						{
							if (!ResizeInfo())
							{
								return -1;
								
							}
						}
						
						if (lastpos != DM_DENTRY_FIRST)
						{
							sufpos = cfg.dentry[lastpos].suffix_pos;
						}
						else
						{
							sufpos = cfg.entrance;
						}
						
						sufentry s = InitSufentry(newhash, lastpos);
						for (int  i = 0; i < hsize; i++)
						{
							int others;
							others = cfg.seinfo[sufpos].hashList[i];
							if (others != DM_DENTRY_NULL)
							{
								int tmphpos;
								tmphpos = cfg.dentry[others].value % newhash;
								if (s.hashList[tmphpos]  == DM_DENTRY_NULL)
								{
									s.hashList[tmphpos]=others;
								}
								else
								{
									conflict = 1;
									break;
								}
							}
						}
						if (conflict == 0)
						{
							int tmphpos;
							tmphpos = cfg.dentry[curdepos].value % newhash;
							if (s.hashList[tmphpos] == DM_DENTRY_NULL)
							{
								s.hashList[tmphpos] =curdepos;
							}
							else
							{
								conflict = 1;
							}
						}
						if (conflict == 0)
						{
							if (lastpos != DM_DENTRY_FIRST)
							{
								cfg.dentry[lastpos].suffix_pos = cfg.seinfo.Count;
							}
							else
							{
								cfg.entrance = cfg.seinfo.Count;
							}
							cfg.seinfo.Add(s);
							curpos = curdepos;
							return curpos;
						}
					}
				}
			}
			return -1;
		}
		
		private  bool ResizeInfo()
		{
			int nextentry = 0;
			int newpos = 0;
			int curde;
			int hsize;
			
			while (nextentry < cfg.seinfo.Count)
			{
				curde = cfg.seinfo[nextentry].backsepos;
				hsize = cfg.seinfo[nextentry].hashsize;
				
				if ((curde != DM_DENTRY_FIRST) && (curde >= cfg.dentry.Count))
				{
					return false;
				}
				if (((curde == DM_DENTRY_FIRST) && (cfg.entrance != nextentry))
					|| ((curde != DM_DENTRY_FIRST) && (cfg.dentry[curde].suffix_pos != nextentry)))
				{
					nextentry++;
				}
				else
				{
					if (nextentry != newpos)
					{
						cfg.seinfo[newpos]=cfg.seinfo[nextentry];
						if (curde != DM_DENTRY_FIRST)
						{
							cfg.dentry[curde].suffix_pos = newpos;
						}
						else
						{
							cfg.entrance = newpos;
						}
					}
					nextentry ++;
					newpos ++;
				}
			}
			int i = cfg.seinfo.Count-1;
			while (i>=newpos) {
				cfg.seinfo.RemoveAt(i);
				i--;
			}
			//cfg.seinfo.splice(newpos,cfg.seinfo.Count-newpos);
			if (cfg.seinfo.Count > cfg.sebufsize / 2)
			{
				cfg.sebufsize *= 2;
			}
			return true;
		}
		
		public  string replace(string str)
		{
            
            if(readComplete == false)
            {
                if (this.data == null)
                    return str;
                WordFilterCfg wordFilterCfg = new WordFilterCfg();
                wordFilterCfg.readData(this.data);
                this.data = null;
                this.loadCfg(wordFilterCfg);
            }
			if(cfg == null)
				return str;
			List<Lemma> re = this.Search(str, DM_OUT_FMM);
			string sb="";
			int pos = 0; 
			foreach( Lemma l in  re){
                //LoggerHelper.Debug("pos:" + pos + ",off:" + l.poff+",str:"+str);
				sb += str.Substring(pos,l.poff-pos);
				for(int  i=0;i<l.len;i++){
					sb +=  DEFAULT_REPLACEMENT;
//					sb.setCharAt(l.poff+i, DEFAULT_REPLACEMENT);
					
				}
				pos = l.poff+l.len;
                DisposeLemma(l);
			}
			sb += str.Substring(pos);
         
			return sb;
			 
		}
		/**
		 * 
		 * @param str
		 * @return  s内容 屏蔽的字符是红色  bo是否有屏蔽字
		 * 
		 */		
		public  bool replaceToRed(string str,out string s)
		{
			s = "";
			if(cfg == null)
				return true;
			List<Lemma> re = this.Search(str, DM_OUT_FMM);
			string sb="";
			int pos = 0; 
			bool bo=false;
			foreach( Lemma l in  re){
				sb += str.Substring(pos,l.poff);
				for(int  i=0;i<l.len;i++){
					sb += "<font color='#ff0000'>" +str.Substring(l.poff+i,(l.poff+(i+1)))+"</font>";
					bo=true;
					//					sb.setCharAt(l.poff+i, DEFAULT_REPLACEMENT);
					
				}
				pos = l.poff+l.len;
			}
			sb += str.Substring(pos);
			s = sb;
			return bo;
		}
		
		public  bool contain(string str)
		{
			if(cfg == null)
				return false;
			List<Lemma> re  = this.Search(str, DM_OUT_CONTAIN);
			return re.Count>0;
		}
		
		private  int SeekLemma(string term)
		{
			int value = 0;
			int curpos = 0;
			int sufpos = 0;
			int hsize = 0;
			int hpos = 0;
			int nde = 0;
			sufpos = cfg.entrance;
			while (curpos < term.Length)
			{
				if (sufpos == DM_SUFENTRY_NULL)
				{
					return DM_LEMMA_NULL;
				}
				
				value = (int)(term[curpos]);
				curpos++;
				
				hsize = cfg.seinfo[sufpos].hashsize;
				hpos = value % hsize;
				nde = cfg.seinfo[sufpos].hashList[hpos];
				if ((nde == DM_DENTRY_NULL) || (cfg.dentry[nde].value != value))
				{
					return DM_LEMMA_NULL;
				}
				sufpos = cfg.dentry[nde].suffix_pos;
			}
			
			return cfg.dentry[nde].lemma_pos;
			
		}
		
		private  sufentry InitSufentry(int hashsize,int  backsepos)
		{
			sufentry s = new sufentry();
			s.hashsize = hashsize;
			s.backsepos = backsepos;
			s.hashList = new List<int>();
			for (int  i = 0; i < hashsize; i++)
			{
				s.hashList.Add(DM_DENTRY_NULL);
			}
			
			return s;
		}
		
		public  void loadCfg(WordFilterCfg cfg)
		{
			this.cfg = cfg;
            readComplete = true;
		}

        ByteArray data;
        public bool loadComplete = false;
        public bool readComplete = false;
        public void loadData(ByteArray data)
        {
            this.data = data;
            loadComplete = true;
        }



        private static WordFilter _instance;
        public static WordFilter Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new WordFilter();
                return _instance;
            }
        }
        public static void Dispose()
        {
            _instance = null;
        }
	}
}

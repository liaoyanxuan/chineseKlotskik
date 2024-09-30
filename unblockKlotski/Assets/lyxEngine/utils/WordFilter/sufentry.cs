using sw.util;
using System.Collections;
using System.Collections.Generic;
 
namespace sw.game.wordfilter
{
	//存放每个节点的子树的指针列表（即后继块）
	public class sufentry
	{
		public int hashsize; //该后继块的hash表大小
		public int backsepos; //指向其属主dentry节点
		public List<int> hashList; //存放子树指针的hash表

        public void readData(ByteArray data)
		{
			int _startPos = data.position;
			int _len = data.readInt();
			hashsize = data.readInt();
			backsepos = data.readInt();
			hashList=new List<int>();
			int sz_hashList = data.readInt();
			for(int i = 0 ;i < sz_hashList ;i ++)
			{
				hashList.Add(data.readInt());
			}

			//if(_len != data.position - _startPos){throw new Error("invalid data");}
		}
	}
}

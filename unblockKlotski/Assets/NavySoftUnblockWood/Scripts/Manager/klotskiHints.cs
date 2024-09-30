using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class klotskiHints
{
   //运行时提示
    public static List<HintInfor> getHintList(char[] startBoard, Dictionary<int, int> blockValueBlockIndexDic)
    {
   
        klotskiSolution findAnswer = new klotskiSolution();
        string boardstr = new string(startBoard);
        findAnswer.init(boardstr, klotskiSolution.MOVE_MODE.RIGHT_ANGLE_TURN);
        FindResult findResult = findAnswer.find();
        Debug.Log("findResult.elapsedTime:" + findResult.elapsedTime);
        Debug.Log("findResult.exploreCount:" + findResult.exploreCount);
        Debug.Log("findResult.boardList.Count:" + findResult.boardList.Count);

        int maxMove = findResult.boardList.Count - 1;

        /////startBoard = klotskiPuzzle.key2Board(findResult.boardList[0]);
        DebugEx.Log("getHintList-->startBoard:\n" + showBoard(startBoard));

      
        List<HintInfor> blockHintInfors = new List<HintInfor>();

        for (var i = 1; i <= maxMove; i++)
        {
            //只与空格子有关系，其他格子是的blockvalue不影响
            SolveMoveInfo moveInfo = klotskiPuzzle.getMoveInfo(klotskiPuzzle.key2Board(findResult.boardList[i - 1]), klotskiPuzzle.key2Board(findResult.boardList[i]));

            int srtPos = moveInfo.srcX + moveInfo.srcY * klotskiShare.G_BOARD_X;
            int destPos = moveInfo.dstX + moveInfo.dstY * klotskiShare.G_BOARD_X;

            char srcBlock = startBoard[srtPos];
            char destBlock = startBoard[destPos];


            Debug.Log("begin-->" + i + ":\n" + showBoard(startBoard));

            Debug.Log("srcBlock:" + srcBlock + " descBlock:" + destBlock + "-->moveInfo:" + moveInfo);


            int blockvalue = (int)(srcBlock - klotskiShare.G_VOID_CHAR);
            int blockStyle = klotskiShare.gBlockBelongTo[blockvalue];//代表的形状


            HintInfor blockHintInfor = new HintInfor();
            blockHintInfor.blockIndex = blockValueBlockIndexDic[blockvalue];
            blockHintInfor.numberMoveCol = moveInfo.dstX - moveInfo.srcX;
            blockHintInfor.numberMoveRow = -(moveInfo.dstY - moveInfo.srcY);
            blockHintInfors.Add(blockHintInfor);

            switch (blockStyle)
            {
                case 0: //empty block
                    break;
                case 1: // 1X1 block
                    startBoard[srtPos] = '@'; //空格
                    startBoard[destPos] = srcBlock;
                    break;
                case 2: // 2X1 block
                    startBoard[srtPos] = '@';
                    startBoard[srtPos + 1] = '@';
                    startBoard[destPos] = srcBlock;
                    startBoard[destPos + 1] = srcBlock;
                    break;
                case 3: // 1X2 block
                    startBoard[srtPos] = '@';
                    startBoard[srtPos + klotskiShare.G_BOARD_X] = '@';
                    startBoard[destPos] = srcBlock;
                    startBoard[destPos + klotskiShare.G_BOARD_X] = srcBlock;
                    break;
                case 4: // 2X2 block
                    startBoard[srtPos] = '@';
                    startBoard[srtPos + 1] = '@';
                    startBoard[srtPos + klotskiShare.G_BOARD_X] = '@';
                    startBoard[srtPos + 1 + klotskiShare.G_BOARD_X] = '@';

                    startBoard[destPos] = srcBlock;
                    startBoard[destPos + 1] = srcBlock;
                    startBoard[destPos + klotskiShare.G_BOARD_X] = srcBlock;
                    startBoard[destPos + 1 + klotskiShare.G_BOARD_X] = srcBlock;

                    break;
                default:
                    Debug.LogError("key2Board(): design error !");
                    break;
            }

            Debug.Log("end-->" + i + ":\n" + showBoard(startBoard));
        }

        //function hints(); --klotski.puzzle.js

        string jsonHitblocks = JsonHelper.ToJson<HintInfor>(blockHintInfors.ToArray());
        Debug.Log("jsonHitblocks:" + jsonHitblocks);
        return blockHintInfors;



    }

    private static string showBoard(char[] charBoard)
    {
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < charBoard.Length; i++)
        {
            if (i != 0 && i % 4 == 0)
            {
                sb.Append("\n");
            }

            sb.Append(charBoard[i]);

        }
        return sb.ToString();
    }
}

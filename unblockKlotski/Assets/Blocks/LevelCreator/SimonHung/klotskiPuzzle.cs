using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class klotskiPuzzle 
{


    //-------------------------------------
    // from source board to target board
    // to get the move info 
    //-------------------------------------
    //只与空格子有关系，其他格子是的blockvalue不影响
    public static SolveMoveInfo getMoveInfo(char[] srcBoard, char[] dstBoard)
    {
        int srcPos=-1, dstPos=-1;
    

        char srcStyle='?',dstStyle= '?';

        for (var i = 0; i < srcBoard.Length && (dstPos == -1 || srcPos == -1); i++)
        {
            if (srcBoard[i] != dstBoard[i])
            {
                if (srcBoard[i] == '@')
                {
                    //move block to here
                    if (dstPos == -1)
                    { //first time
                        dstPos = i;
                        dstStyle = dstBoard[i];
                    }
                }
                else if (dstBoard[i] == '@')
                {
                    // move block out here 
                    /*
                    if(dstBoard[i] != ' ') {
                        debug("Error2: wrong board (" + i + ") !");
                        break;
                    }*/
                    if (srcPos == -1)
                    { //first time
                        srcPos = i;
                        srcStyle = srcBoard[i];
                    }
                }
            }
        }
        int srcX = srcPos % klotskiShare.G_BOARD_X, srcY = (srcPos - srcX) / klotskiShare.G_BOARD_X;
        int dstX = dstPos % klotskiShare.G_BOARD_X, dstY = (dstPos - dstX) / klotskiShare.G_BOARD_X;

        //find the left-up position
        while (srcX > 0 && srcBoard[srcX - 1 + srcY * klotskiShare.G_BOARD_X] == srcStyle) srcX--;
        while (srcY > 0 && srcBoard[srcX + (srcY - 1) * klotskiShare.G_BOARD_X] == srcStyle) srcY--;

        //find the left-up position
        while (dstX > 0 && dstBoard[dstX - 1 + dstY * klotskiShare.G_BOARD_X] == dstStyle) dstX--;
        while (dstY > 0 && dstBoard[dstX + (dstY - 1) * klotskiShare.G_BOARD_X] == dstStyle) dstY--;

        return new SolveMoveInfo(srcX, srcY, dstX, dstY);
       // return { startX: srcX, startY: srcY, endX: dstX, endY: dstY }
    }



    //-------------------------------------------------
    // convert integer board key to board value 
    // with different char value of same block style
    // for draw board 
    //-------------------------------------------------
    public static char[] key2Board(long curKey)
    {
        int blockIndex;
        char[] board = new char[20];

        /* 初始化数组 n 中的元素 */
        for (int i = 0; i < 20; i++)
        {
            board[i] = '?';
        }

        //0   1    2    3    4
        char[] blockValue = { '@', 'N', 'B', 'H', 'A' };
        int primeBlockPos = (int)(curKey & 0x0F); //position of prime minister block (曹操), 4 bits

        //set prime minister block
        board[primeBlockPos] = blockValue[4];
        board[primeBlockPos + klotskiShare.G_BOARD_X] = blockValue[4];
        board[primeBlockPos + 1] = blockValue[4];
        board[primeBlockPos + 1 + klotskiShare.G_BOARD_X] = blockValue[4];
        //curKey = Math.floor(curKey / 16); //shift >> 4 bits
        curKey >>= 4;

        int[] blockIndexs = new int[20];
        for (int bb = 0; bb < 20; bb++)
        {
            blockIndexs[bb] =0;
        }


        for (int i = 19; i>= 0; i--)
        {
            if (board[i] == blockValue[4])
            {
                blockIndexs[i] = 4;
               
            }
            else
            {
                blockIndex = (int)(curKey & 0x03); //2 bits
                blockIndexs[i] = blockIndex;
                curKey >>= 2; //shift >> 2 bits, now the value <= 32 bits can use bitwise operator
            }
        }

        for (var curPos = 0; curPos<= 19; curPos++)
        {
            if (board[curPos] == blockValue[4]) continue;
            blockIndex = blockIndexs[curPos]; 
            if (board[curPos] != '?') continue;
            switch (blockIndex)
            {
                case 0: //empty block
                    board[curPos] = blockValue[0];
                    break;
                case 1: // 1X1 block
                    board[curPos] = blockValue[1];
                    blockValue[1] = (char)(blockValue[1] + 1); //ascii + 1
                    break;
                case 2: // 2X1 block
                    board[curPos] = blockValue[2];
                    board[curPos + 1] = blockValue[2];
                    blockValue[2] = (char)(blockValue[2] + 1); //ascii + 1
                    break;
                case 3: // 1X2 block
                    board[curPos] = blockValue[3];
                    board[curPos + klotskiShare.G_BOARD_X] = blockValue[3];
                    blockValue[3] = (char)(blockValue[3] + 1); //ascii + 1
                    break;
                case 4: // 2X2 block
                    break;
                default:
                    Debug.LogError("key2Board(): design error !");
                    goto loop;
            }
        }
    loop:
        return board;
    }

    //-------------------------------------------------
    // convert integer board key to board value 
    // with different char value of same block style
    // for draw board 
    //-------------------------------------------------
    public static char[] key2Boardbackup(long curKey)
    {
        int blockIndex;
        char[] board = new char[20];

        /* 初始化数组 n 中的元素 */
        for (int i = 0; i < 20; i++)
        {
            board[i] = '?';
        }

        //0   1    2    3    4
        char[] blockValue = { '@', 'N', 'B', 'H', 'A' };
        int primeBlockPos = (int)(curKey & 0x0F); //position of prime minister block (曹操), 4 bits

        //set prime minister block
        board[primeBlockPos] = blockValue[4];
        board[primeBlockPos + klotskiShare.G_BOARD_X] = blockValue[4];
        board[primeBlockPos + 1] = blockValue[4];
        board[primeBlockPos + 1 + klotskiShare.G_BOARD_X] = blockValue[4];
        //curKey = Math.floor(curKey / 16); //shift >> 4 bits
        curKey >>= 4;


        for (var curPos = (klotskiShare.G_BOARD_Y * klotskiShare.G_BOARD_X) - 1; curPos >= 0; curPos--)
        {
            if (board[curPos] == blockValue[4]) continue;

            blockIndex = (int)(curKey & 0x03); //2 bits
            curKey >>= 2; //shift >> 2 bits, now the value <= 32 bits can use bitwise operator

            if (board[curPos] != '?') continue;

            switch (blockIndex)
            {
                case 0: //empty block
                    board[curPos] = blockValue[0];
                    break;
                case 1: // 1X1 block
                    board[curPos] = blockValue[1];
                    blockValue[1] =(char)(blockValue[1] + 1); //ascii + 1
                    break;
                case 2: // 2X1 block
                    board[curPos] = blockValue[2];
                    board[curPos - 1] = blockValue[2];
                    blockValue[2] = (char)(blockValue[2] + 1); //ascii + 1
                    break;
                case 3: // 1X2 block
                    board[curPos] = blockValue[3];
                    board[curPos - klotskiShare.G_BOARD_X] = blockValue[3];
                    blockValue[3] = (char)(blockValue[3] + 1); //ascii + 1
                    break;
                case 4: // 2X2 block	
                default:
                    Debug.LogError("key2Board(): design error !");
                    goto loop;
            }
        }
    loop:
        return board;
    }
}




public class SolveMoveInfo
{
    public int srcX;
    public int srcY;
    public int dstX;
    public int dstY;

    public SolveMoveInfo(int psrcX, int psrcY, int pdstX, int pdstY)
    {
         srcX= psrcX;
         srcY= psrcY;
         dstX= pdstX;
         dstY= pdstY;
    }

    public override string ToString()
    {
        return "srcX:" + srcX + " srxY:" + srcY + " dstX:" + dstX + " dstY:" + dstY;
    }
}

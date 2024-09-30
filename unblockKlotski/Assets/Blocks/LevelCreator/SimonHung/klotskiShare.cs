using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class klotskiShare
{
    //=======================================
    // Klotski share variable & function
    //=======================================

    //---------------------------------------------------------------------------------------
    //  0----------> X        
    //  |"HAAI"
    //  |"HAAI"
    //  |"JBBK"
    //  |"JNOK"
    //  |"P@@Q"
    //  V
    //  Y
    //
    // normal block = A..[
    // empty  block = @
    //---------------------------------------------------------------------------------------
    //Block Shapes:
    //    @, AA BB CC DD EE FF GG  H  I  J  K  L  M  N  O  P  Q  R  S  T  U  V  W  X  Y  Z  [
    //       AA                    H  I  J  K  L  M
    //---------------------------------------------------------------------------------------

    public static int G_BOARD_X = 4;
    public static int G_BOARD_Y = 5;
    public static int G_BOARD_SIZE = G_BOARD_X * G_BOARD_Y; //board size

    public static char G_VOID_CHAR = '?';
    public static char G_EMPTY_CHAR = '@';

    public static int G_EMPTY_BLOCK = 1; //gBlockBelongTo index 1 ('@')
    public static int G_GOAL_BLOCK = 2; //gBlockBelongTo index 2 ('A')
    public static int G_GOAL_STYLE = 4; //index of gBlockStyle for goal block

    //convert char to index of block style
    //ASCII char   :                       ?, @, A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q, R, S, T, U, V, W, X, Y, Z, [
    public static int[] gBlockBelongTo ={ -1, 0, 4, 2, 2, 2, 2, 2, 2, 3, 3, 3, 3, 3, 3, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };  //形状索引
    public static int[,] gBlockStyle ={ { 1, 1 },{ 1, 1 },{ 2, 1 },{ 1, 2 },{ 2, 2 } }; //block style:[x size, y size]   形状
    public static int[,] gGoalPos ={ { 1, 4 }, { 2, 4 } }; //goal position: [x,y]

    //                                         0   1     2    3    4
    public static char[] gBlockStartStyle = { '@', 'N', 'B', 'H', 'A' };

    //--------------------------------------------------------------------
    // board string convert to board array with gBlockBelongTo index value
    //--------------------------------------------------------------------
    public static int[] gEasyBoard(string boardString)
    {
        char[] boardArray = boardString.ToCharArray();
        int[] newBoardArray = new int[20];

        for (var i = 0; i < boardArray.Length; i++)
        {
            newBoardArray[i] = (int)(boardArray[i] - G_VOID_CHAR);
        }
        return newBoardArray;
    }

    //---------------------------------------------------------
    // transfer the board to 64 bits int
    // one char convert to 2 bits
    //
    // javascript: They are 64-bit floating point values, 
    //             the largest exact integral value is 2 ^ 53
    //             but bitwise/shifts only operate on int32
    //
    // add support key for left-right mirror, 09/02/2017
    //---------------------------------------------------------
    public static long gBoard2Key(int[] board, bool mirror= false)
    {
        long boardKey = 0;
        int primeBlockPos = -1;
        int invBase = 0;
        int blockValue;

        if (mirror) invBase = -(G_BOARD_X + 1); //key for mirror board

        for (var i = 0; i < board.Length; i++)
        {
            //---------------------------------------------------------------------
            // Javascript only support 53 bits integer	(64 bits floating)
            // for save space, one cell use 2 bits 
            // and only keep the position of prime minister block (曹操)
            //---------------------------------------------------------------------
            // maxmum length = (4 * 5 - 4) * 2 + 4  
            //               = 32 + 4 = 36 bits
            //
            // 4 * 5 : max cell
            // - 4   : prime minister block size
            // * 2   : one cell use 2 bits 
            // + 4   : prime minister block position use 4 bits
            //---------------------------------------------------------------------
            // if (!(i % G_BOARD_X))
            if ((i % G_BOARD_X)==0)
            {
                invBase += G_BOARD_X * 2; //key for mirror board
            }

            if ((blockValue = board[mirror ? invBase - i : i]) == G_GOAL_BLOCK)
            {
                //skip prime minister block (曹操), only keep position  
                if (primeBlockPos < 0) primeBlockPos = i;
                continue;
            }
            boardKey = (boardKey << 2) + gBlockBelongTo[blockValue];  //bitwise/shifts must <= 32 bits)
        }
        boardKey = (boardKey * 16) + primeBlockPos; //shift 4 bits (0x00-0x0E) 
        return boardKey;
    }

    //----------------------------------------------------
    // convert row major 2 diamonion to 1 diamonion index
    //----------------------------------------------------
    public static int gRowMajorToIndex(int x, int y)
    {
        return x + G_BOARD_X * y;
    }

    //---------------------------------------
    // board verify 
    //
    // return emptyCount for maximum deep check
    //---------------------------------------
    public static CheckResult gBlockCheck(string boardString)
    {
        char[] tmpBoard = boardString.ToCharArray(); //string to array
        Dictionary<int,int> indexCheck = new Dictionary<int, int>();
        int rc = 0;
        int emptyCount = 0;

        //(1) board size check
        if (tmpBoard.Length != G_BOARD_SIZE)
        {
            Debug.LogError("Wrong board size !");
            return new CheckResult (1,0);
        }

        //(2) check block style and don't duplicate
        int blockValue;
        int blockIndex;
        int sizeX, sizeY;
        int x = 0, y = 0;
   
        for (y = 0; y < G_BOARD_Y; y++)
        {
            for ( x = 0; x < G_BOARD_X; x++)
            {
                if ((blockValue = tmpBoard[gRowMajorToIndex(x, y)]) == '0') continue; //already verified

                blockIndex = blockValue - G_VOID_CHAR;
               
                sizeX = gBlockStyle[gBlockBelongTo[blockIndex],0]; //block size X
                sizeY = gBlockStyle[gBlockBelongTo[blockIndex],1]; //block size Y

                for (int blockY = 0; blockY < sizeY; blockY++)
                {
                    if (blockY + y >= G_BOARD_Y) { rc = 1; goto loop; }

                    for (int blockX = 0; blockX < sizeX; blockX++)
                    {
                        if (blockX + x >= G_BOARD_X) { rc = 1; goto loop; }

                        int index = gRowMajorToIndex(blockX + x, blockY + y);
                        if (tmpBoard[index] != blockValue) { rc = 1; goto loop; }
                        tmpBoard[index] = '0'; //verified 
                    }
                }
                if (blockValue == G_EMPTY_CHAR)
                {
                    ++emptyCount;
                }
                else if (indexCheck.ContainsKey(blockIndex))
                {
                    rc = 2;
                    goto loop; //duplicate
                }
                indexCheck[blockIndex] = 1;
            }
        }
    loop:
        if (rc == 1) Debug.LogError("Error: wrong block at [" + x + "," + y + "]");
        if (rc == 2) Debug.LogError("Error: block duplicate at [" + x + "," + y + "]");
        if (emptyCount > 2) Debug.LogError("Warning: too many empty block!");
        return new CheckResult(rc, emptyCount);
    }

}

public class CheckResult
{
    public int rc = 0;
    public int emptyCount = 0;
    public CheckResult(int prc, int pEmptyCount)
    {
        rc = prc;
        emptyCount = pEmptyCount;
    }
}

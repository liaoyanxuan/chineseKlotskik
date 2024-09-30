//=======================================================================================================
// Klotski solver (華容道) - BFS (Breadth-first search)
//
// 09/02/2017 - Don't calculate left-right mirror state 
//              reference: https://github.com/jeantimex/Klotski
//
// 09/01/2017 - Fixed hash-key duplicate calculation by saving hash-key to queue 
//
// 08/21/2013 - Add support 3 option (default = RIGHT_ANGLE_TURN)
//
// 05/20/2013 - Include "klitski.share.js" for share variable & function
//              and move easyBoard(), board2Key(), blockCheck(), rowMajorToIndex() 
//              to "klitski.share.js" as share function
// 
// 05/02/2013 - Add support more than 2 empty block for "發芽網"  
//
// 04/23/2013 - For improve efficiency
//              (1) embedded the function inBound(), getBlockValue() and isBlock() to code directly
//              (2) Add easyBoard() to convert board char to gBlockBelongTo index
//
// 04/20/2013 - Created by Simon Hung
//              
//=======================================================================================================  

//----------------
// class function
//----------------
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class klotskiSolution
{
    //-----------
    // define
    //-----------
    public enum MOVE_MODE
    {
        RIGHT_ANGLE_TURN=1,
        STRAIGHT=2,
        ONE_CELL_ONLY=3
    }; //08/21/2013


    bool SKIP_MIRROR_STATE = true; // 09/02/2017
                                   //-----------
                                   // variable	
                                   //-----------

    MOVE_MODE moveMode = MOVE_MODE.RIGHT_ANGLE_TURN;
    Queue<BoardObj> Q; //queue for breadth first search (external)
    Dictionary<long, long> H; //hash maps for current state to parent state & collision detection (external)

    int[] initBoard;
    int exploreCount;
    int emptyCount, wrongBoard;


    public klotskiSolution()
    {
       
    }

    bool reachGoal(int[] curBoard)
    {
        int index = 0;
        try
        {
            int arrayLength = klotskiShare.gGoalPos.GetLength(0);
            for (var i = 0; i < arrayLength; i++)
            {
                index = klotskiShare.gGoalPos[i, 0] + klotskiShare.gGoalPos[i, 1] * klotskiShare.G_BOARD_X;
                if (curBoard[index] != klotskiShare.G_GOAL_BLOCK)
                    return false;
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
            throw e;
        }

        return true;
    }

    //------------------------------------------------------------	
    //add new state to queue and hashmap if does not exist before 
    //------------------------------------------------------------
    int statePropose(BoardObj boardObj, long parentKey)
    {
        long curMirrorKey;
       
        if (H.ContainsKey(boardObj.key) == false)
        {
            H[boardObj.key] = parentKey;

            if (SKIP_MIRROR_STATE)
            { //don't calculate left-right mirror state
                curMirrorKey = klotskiShare.gBoard2Key(boardObj.board, true);
                H[curMirrorKey]=parentKey;
            }
            //no any state same as current, add it
            int[] newboardArr = new int[20];
            boardObj.board.CopyTo(newboardArr, 0);

            BoardObj newBoardObj = new BoardObj(newboardArr, boardObj.key);

            //Q.add({ board: boardObj.board.slice(0), key: boardObj.key});
            Q.Enqueue(newBoardObj);

            return 1; //add new state
        }

        return 0; //state already exist
    }

    //------------------------------------------------------
    // how many spaces or block there are (origin excluded)
    //------------------------------------------------------

    int countLengthX(int[] board, int posX, int posY, int directionX, int block)
    {
        int step = -1;

        do
        {
            step++;
            posX += directionX;
        } while (posX >= 0 && posX < klotskiShare.G_BOARD_X && board[posX + posY * klotskiShare.G_BOARD_X] == block);

        return step;
    }

    int countLengthY(int[] board, int posX, int posY, int directionY, int block)
    {
        int step = -1;

        do
        {
            step++;
            posY += directionY;
        } while (posY >= 0 && posY < klotskiShare.G_BOARD_Y && board[posX + posY * klotskiShare.G_BOARD_X] == block);

        return step;
    }

    //---------------------------------------------------
    // slide empty-cell up or down by directionY (-1 or 1)
    //
    // directionY: 1: empty down (block up), -1: empty up (block down)
    //
    // return how many new state created
    // 
    //---------------------------------------------------
    int slideVertical(BoardObj boardObj, long parentKey, int emptyX, int emptyY, int directionY, int maxMove)
    {
        int blockX, blockY; //block position (x,y)
        int blockValue; //block value
       // int styleIndex; //index of block style
        int blockSizeX, blockSizeY; //block style
        int[] curBoard = boardObj.board;

        //Find the block
        blockX = emptyX;
        blockY = emptyY + directionY;
        if (blockY < 0 || blockY >= klotskiShare.G_BOARD_Y) return 0; //out of range

        if ((blockValue = curBoard[blockX + blockY * klotskiShare.G_BOARD_X]) <= klotskiShare.G_EMPTY_BLOCK) return 0; //empty
        blockSizeX = klotskiShare.gBlockStyle[klotskiShare.gBlockBelongTo[blockValue],0]; //block size X
        blockSizeY = klotskiShare.gBlockStyle[klotskiShare.gBlockBelongTo[blockValue],1]; //block size Y

        //Begin vertical move ------------------

        //----------------------------------------------------------------	
        // block slide up|down: must block size X can slide to empty space
        //
        //   min-X <---- empty space ----> max-X
        // 
        //           +--------------+
        //           |              | 
        //           +--------------+ 
        //     min-X <---- block ---> max-X
        // 
        //   minBlockX must >= minSpaceX && maxBlockX must <= maxSpaceX
        //-----------------------------------------------------------------  

        //--------------------------------------------------------------------------
        // find the block min-X and max-X
        // minimum block position X = current block position X - count of left block
        //--------------------------------------------------------------------------
        var minBlockX = blockX - countLengthX(curBoard, blockX, blockY, -1, blockValue);
        var maxBlockX = minBlockX + blockSizeX - 1;

        var stateCount = 0;
        var boardCopied = 0;
        int[] childBoard= new int[20];
        BoardObj childObj;
        long curKey =0;

        do
        {
            //--------------------------------------------------------------------------
            // calculate the space min-X and max-X of next position
            //--------------------------------------------------------------------------

            //minimum space position X = current space position X - count of left space
            var minSpaceX = emptyX - countLengthX(curBoard, emptyX, emptyY, -1, klotskiShare.G_EMPTY_BLOCK);

            //maximum space position X = current space position X + count of right space
            var maxSpaceX = emptyX + countLengthX(curBoard, emptyX, emptyY, +1, klotskiShare.G_EMPTY_BLOCK);

            //block left-right (X) range must less or equal to left-right (X) space size
            if (minBlockX < minSpaceX || maxBlockX > maxSpaceX) return stateCount;

            if (boardCopied==0)
            {
                //first time, copy board array & set curKey
                //childBoard = curBoard.slice(0);
                curBoard.CopyTo(childBoard, 0);

                curKey = boardObj.key;
                boardCopied = 1;
            }

            //slide empty-block up|down
            for (var x = minBlockX; x <= maxBlockX; x++)
            {
                childBoard[x + emptyY * klotskiShare.G_BOARD_X] = blockValue;
                childBoard[x + (emptyY + blockSizeY * directionY) * klotskiShare.G_BOARD_X] = klotskiShare.G_EMPTY_BLOCK;
            }

            childObj = new BoardObj(childBoard, klotskiShare.gBoard2Key(childBoard));
           // childObj = { board: childBoard, key: gBoard2Key(childBoard)};
            if (parentKey != 0)
            {
                //-----------------------------------------------
                // parentKey != 0 means move more than one step
                //-----------------------------------------------
                stateCount += statePropose(childObj, parentKey);
            }
            else
            {
                stateCount += statePropose(childObj, curKey);
                parentKey = curKey;
            }

            {
                //---------------------------------------------------------------------------------
                // only for one size block:
                // for more than one step and move to different direction (vertical to horizontal)
                //
                //
                //   +---+---+      +---+---+
                //   | E   E |      | E | B |
                //   +---+---+  ==> +   +---+ 
                //   | B |          | E | 
                //   +---+          +---+
                //---------------------------------------------------------------------------------
                if (moveMode == MOVE_MODE.RIGHT_ANGLE_TURN && maxMove < emptyCount)
                {
                    if (minSpaceX < minBlockX && minBlockX == emptyX)
                    {
                        //slide block left
                        stateCount += slideHorizontal(childObj, parentKey, emptyX - 1, emptyY, +1, maxMove + 1);
                    }
                    if (maxSpaceX > maxBlockX && maxBlockX == emptyX)
                    {
                        //slide block right
                        stateCount += slideHorizontal(childObj, parentKey, emptyX + 1, emptyY, -1, maxMove + 1);
                    }
                }
            }

            emptyY -= directionY;

        } while (moveMode != MOVE_MODE.ONE_CELL_ONLY && emptyY >= 0 && emptyY < klotskiShare.G_BOARD_Y && childBoard[emptyX + emptyY * klotskiShare.G_BOARD_X] == klotskiShare.G_EMPTY_BLOCK);

        return stateCount;
    }

    //---------------------------------------------------
    // slide empty-cell left or right by directionX (-1 or 1)
    //
    // directionX: 1: empty right (block left), -1: empty right (block left)
    //
    // return how many new state created
    //---------------------------------------------------
    int slideHorizontal(BoardObj boardObj, long parentKey, int emptyX, int emptyY, int directionX, int maxMove)
    {
        int blockX, blockY; //block position (x,y)
        int blockValue; //block value
       // int styleIndex; //index of block style
        int blockSizeX, blockSizeY; //block style
        int[] curBoard = boardObj.board;

        //Find the block
        blockX = emptyX + directionX;
        if (blockX < 0 || blockX >= klotskiShare.G_BOARD_X) return 0; //out of range

        blockY = emptyY;

        if ((blockValue = curBoard[blockX + blockY * klotskiShare.G_BOARD_X]) <= klotskiShare.G_EMPTY_BLOCK) return 0; //empty
        blockSizeX = klotskiShare.gBlockStyle[klotskiShare.gBlockBelongTo[blockValue],0]; //block size X
        blockSizeY = klotskiShare.gBlockStyle[klotskiShare.gBlockBelongTo[blockValue],1]; //block size Y

        //Begin horizontal move ------------------

        //--------------------------------------------------------------------	
        // block slide left|right: must block size Y can slide to empty space
        //
        //   min-X <---- empty space ----> max-X
        // 
        //    --+-- min-Y
        //      |          +---+   --+-- min-Y
        //      |          |   |     |
        //  empty space    |   |   block
        //      |          |   |     | 
        //      |          +---+   --+-- max-Y
        //    --+-- max-Y 
        // 
        //   minBlockY must >= minSpaceY && maxBlockY must <= maxSpaceY
        //---------------------------------------------------------------------  

        //--------------------------------------------------------------------------
        // find the block min-Y and max-Y
        // minimum block position Y = current block position Y - count of up block
        //--------------------------------------------------------------------------
        var minBlockY = blockY - countLengthY(curBoard, blockX, blockY, -1, blockValue);
        var maxBlockY = minBlockY + blockSizeY - 1;

        var stateCount = 0;
        var boardCopied = 0;
        int[] childBoard= new int[20];
        BoardObj childObj;
        long curKey =0;

        do
        {
            //--------------------------------------------------------------------------
            // calculate the space min-X and max-X of next position
            //--------------------------------------------------------------------------

            //minimum space position Y = current space position Y - count of up space
            var minSpaceY = emptyY - countLengthY(curBoard, emptyX, emptyY, -1, klotskiShare.G_EMPTY_BLOCK);

            //maximum space position X = current space position X + count of right space
            var maxSpaceY = emptyY + countLengthY(curBoard, emptyX, emptyY, +1, klotskiShare.G_EMPTY_BLOCK);

            //block up-down (Y) range must less or equal to up-down (Y) space size
            if (minBlockY < minSpaceY || maxBlockY > maxSpaceY) return stateCount;

            if (boardCopied==0)
            {
                //first time, copy board array & set curKey
             //   childBoard = curBoard.slice(0);
                curBoard.CopyTo(childBoard,0);
                curKey = boardObj.key;
                boardCopied = 1;
            }

            //slide empty-block left|right
            for (var y = minBlockY; y <= maxBlockY; y++)
            {
                childBoard[emptyX + y * klotskiShare.G_BOARD_X] = blockValue;
                childBoard[(emptyX + blockSizeX * directionX) + y * klotskiShare.G_BOARD_X] = klotskiShare.G_EMPTY_BLOCK;
            }

           // childObj = { board: childBoard, key: gBoard2Key(childBoard)};
            childObj = new BoardObj(childBoard, klotskiShare.gBoard2Key(childBoard));
            if (parentKey != 0)
            {
                //-----------------------------------------------
                // parentKey != 0 means move more than one step
                //-----------------------------------------------
                stateCount += statePropose(childObj, parentKey);
            }
            else
            {
                stateCount += statePropose(childObj, curKey);
                parentKey = curKey;
            }

            {
                //---------------------------------------------------------------------------------
                // only for one size block:
                // for more than one step and move to different direction (horizontal to vertical)
                //
                //
                //       +---+          +---+
                //       | E |          | B |
                //   +---+   +  ==> +---+---+ 
                //   | B | E |      | E   E | 
                //   +---+---+      +---+---+
                //---------------------------------------------------------------------------------
                if (moveMode == MOVE_MODE.RIGHT_ANGLE_TURN && maxMove < emptyCount)
                {
                    if (minSpaceY < minBlockY && minBlockY == emptyY)
                    {
                        //slide block up (empty down)
                        stateCount += slideVertical(childObj, parentKey, emptyX, emptyY - 1, +1, maxMove + 1);
                    }
                    if (maxSpaceY > maxBlockY && maxBlockY == emptyY)
                    {
                        //slide block down (empty up)
                        stateCount += slideVertical(childObj, parentKey, emptyX, emptyY + 1, -1, maxMove + 1);
                    }
                }
            }
            emptyX -= directionX;

        } while (moveMode != MOVE_MODE.ONE_CELL_ONLY && emptyX >= 0 && emptyX < klotskiShare.G_BOARD_X && childBoard[emptyX + emptyY * klotskiShare.G_BOARD_X] == klotskiShare.G_EMPTY_BLOCK);

        return stateCount;
    }

    //--------------------------------------------------------
    // Using recursion to trace the steps from button to top 
    // then put the key value to array 
    //--------------------------------------------------------
    void getAnswerList(long curKey, List<long> boardList)
    {
        long parentKey = H[curKey]; //{ key: curKey, value: parentKey }

        if (parentKey!=0) getAnswerList(parentKey, boardList);
        boardList.Add(curKey);
    }

    //---------------------------------------------
    // for all empty block to find the next state
    //---------------------------------------------
    public int explore(BoardObj boardObj)
    {
        int stateCount = 0; //how many new state created
        int eCount = 0;   //empty count

    
        for (int emptyX = 0; emptyX < klotskiShare.G_BOARD_X; emptyX++)
        {
            for (int emptyY = 0; emptyY < klotskiShare.G_BOARD_Y; emptyY++)
            {

                if (boardObj.board[emptyX + emptyY * klotskiShare.G_BOARD_X] != klotskiShare.G_EMPTY_BLOCK) continue;
                eCount++;

                //slide empty up ==> block down
                stateCount += slideVertical(boardObj, 0, emptyX, emptyY, -1, 0); //block at Y-1

                //slide empty down ==> block up
                stateCount += slideVertical(boardObj, 0, emptyX, emptyY, +1, 0); //block at Y+1

                //slide empty left ==> block right
                stateCount += slideHorizontal(boardObj, 0, emptyX, emptyY, -1, 0); //block at X-1

                //slide empty right ==> block left  
                stateCount += slideHorizontal(boardObj, 0, emptyX, emptyY, +1, 0); //block at X+1 
                if (eCount >= emptyCount)  //2个宫格都尝试过了
                {
                    goto breakLoop; 
                }
            }
        }
        breakLoop:
        return stateCount;
    }

    	//---------------------------------
	// public function : initial value 
	//---------------------------------
	public void init(string boardString, MOVE_MODE boardMoveMode)
    {
        //boardPrint(boardString.split("")); //debug

        CheckResult result = klotskiShare.gBlockCheck(boardString);
        if (result.rc>0) { wrongBoard = 1; return; }

        emptyCount = result.emptyCount;
        wrongBoard = 0;

        
        switch (boardMoveMode)
        {
            case MOVE_MODE.ONE_CELL_ONLY:
                moveMode = MOVE_MODE.ONE_CELL_ONLY;
                break;
            case MOVE_MODE.STRAIGHT:
                moveMode = MOVE_MODE.STRAIGHT;
                break;
            default:
            case MOVE_MODE.RIGHT_ANGLE_TURN:
                moveMode = MOVE_MODE.RIGHT_ANGLE_TURN;
                break;
        };
        

        Q = new Queue<BoardObj>(); //queue for breadth first search
        H = new Dictionary<long, long>(); //hash maps for current state to parent state & collision detection

        initBoard = klotskiShare.gEasyBoard(boardString);
    }

    //-----------------------------------
	// public function : find the answer 
	//-----------------------------------
	public FindResult find()
    {
        int startTime, endTime;
        List<long> boardList = null;

        if (wrongBoard!=0)
        {
           // return { exploreCount: 0, elapsedTime: 0, boardList: null};
            return new FindResult(0, 0, null);
        }
        startTime = DateTime.Now.Millisecond;

        //Put the initial state to BFS queue & hash map
        BoardObj intiBoardObj = new BoardObj(initBoard, klotskiShare.gBoard2Key(initBoard));
        statePropose(intiBoardObj, 0);
        exploreCount = 1; //initial state

        while (Q.Count>0)
        {
            var boardObj = Q.Dequeue();

            if (reachGoal(boardObj.board))
            {
                boardList = new List<long>();
                getAnswerList(boardObj.key, boardList);
                break; //find a solution
            }
            exploreCount += explore(boardObj); //find next board state
        }
        endTime = DateTime.Now.Millisecond;

        Q =null;
         H=null;

        return new FindResult(exploreCount, (endTime - startTime) / 1000, boardList);
       
    }
}


public class BoardObj
{
    public int[] board;
    public long key;

    public BoardObj(int[] pBoard, long pkey)
    {
        board = pBoard;
        key = pkey;
    }
}

public class FindResult
{
    public int exploreCount = 0;
    public int elapsedTime = 0;
    public List<long> boardList = null;

    public FindResult(int pExploreCount, int pElapsedTime, List<long> pBoardList)
    {
        exploreCount = pExploreCount;
        elapsedTime = pElapsedTime;
        boardList = pBoardList;
    }

}

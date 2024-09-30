using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static BizzyBeeGames.Blocks.LevelCreatorWindow;

namespace blocksolutions
{


    public class State
    {
        public List<List<SGridCell>> board;
        public Dictionary<int, List<SGridCell>> planks;    //1-R:R代表红木的数字
        public int from;  //记录上一个状态（来自哪个状态）
        public int index;
        public Action action=new Action();   //记录从父级是如何变化到目前状态的，第几个木块，如何移动； 
        public long hashValue;
      

        public State cloneSta()
        {
            State cloneState = new State();

            cloneState.board = new List<List<SGridCell>>();

            foreach (List<SGridCell> row in board)
            {
                List<SGridCell> cloneRow = new List<SGridCell>();

                foreach (SGridCell gridCell in row)
                {
                    cloneRow.Add(gridCell.cloneCell());
                }

                cloneState.board.Add(cloneRow);
            }

            ///*********************************************************///

            cloneState.planks = new Dictionary<int, List<SGridCell>>();

            foreach (KeyValuePair<int, List<SGridCell>> kv in planks)
            {
                List<SGridCell> cloneList = new List<SGridCell>();

                foreach (SGridCell gridCell in kv.Value)
                {
                    cloneList.Add(gridCell.cloneCell());
                }

                cloneState.planks[kv.Key] = cloneList;
            }

            ///*********************************************************///
            cloneState.from = from;
            cloneState.index = index;
            cloneState.action = action.cloneAction();


            return cloneState;
        }
    }

    public class Action
    {
        public byte No=0; 
        public sbyte moveRow = 0;
        public sbyte moveCol = 0;

        public Action cloneAction()
        {
            Action action = new Action();
            action.No = No;
            action.moveRow = moveRow;
            action.moveCol = moveCol;
            return action;
        }
    }

    public class BlockSolution
    {
        const int NR = 5;
        const int NC = 4;

        const long M = (long)1e9 + 7;

 
        List<State> states;  //状态列表，用于确定index
        Dictionary<long, List<State>> visited;  //集合set，已经访问过(集合（set）是一个无序的不重复元素序列)
        Queue<State> Q;  //队列
        public int redIndex = 0;   //红木块index


        List<State> result;


        //克隆State；
        State cloneStateFun(State ss)
        {
            State cState = ss.cloneSta();
            return cState;
        }

        //获得哈希值
        /**
        *Hash，一般翻译做散列、杂凑，或音译为哈希，是把任意长度的输入（又叫做预映射pre-image）通过散列算法变换成固定长度的输出，该输出就是散列值。
        这种转换是一种压缩映射，也就是，散列值的空间通常远小于输入的空间，不同的输入可能会散列成相同的输出，所以不可能从散列值来确定唯一的输入值
        */

        long getHash(State newState)
        {
            int x = 0;
            long  resultHash = 0L;
            for (int i = 0; i < NR; i++)
            {
                for (int j = 0; j < NC; j++)
                {
                    resultHash = M * resultHash + newState.board[i][j].shapeIndex;
                }
            }
            return resultHash;
        }

        void enQueue(State newState, int from)
        {
            long mask = getHash(newState);
            if (visited.ContainsKey(mask))   //哈希冲突，先粗放比较
            {
                List<State> visitedStates = visited[mask];
                foreach (State stateInList in visitedStates)
                {
                    if (stateInList != null)
                    {
                        if (isStateEqual(newState, stateInList)) //精细比较
                        {
                            return;  //发现是已有的State，直接返回；
                        }
                    }  
                }
            }
            else
            {
                visited[mask] = new List<State>();
            }


            visited[mask].Add(newState);

            newState.from = from;
            newState.index = states.Count;

            states.Add(newState);
            Q.Enqueue(newState);
        }

        bool isStateEqual(State state1,State state2)
        {
            for (int i = 0; i < NR; i++)
            {
                for (int j = 0; j < NC; j++)
                {
                    if (state1.board[i][j].shapeIndex != state2.board[i][j].shapeIndex)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        bool checkWin(State ff)
        {
            
            int minRow = getMinRowGridCell(ff.planks[redIndex]).row-1;

            int minCol = getMinColGridCell(ff.planks[redIndex]).col;
            int maxCol = getMaxColGridCell(ff.planks[redIndex]).col;

            if (minCol == 1 && maxCol == 2)
            {
                if (minRow == -1)
                {
                    return true;
                }

                while (minRow >=0 && ff.board[minRow][1].shapeIndex == 0 && ff.board[minRow][2].shapeIndex == 0)
                {
                    minRow--;
                }

                if (minRow == -1)
                {
                    return true;
                }

            }

            return false;
        }

        List<State> win(int index, int d = 0)
        {
            if (index == -1)
            {
                return null;
            }
            win(states[index].from, d + 1);

            result.Add(states[index]);
                  
            return result;
        }



        //广度搜索穷举法--》注意排除重复状态！！！   (2) 直線移動視為一步
        public List<State> startSearch(State oo,int red)
        {
            redIndex = red;
            result = new List<State>();
            states = new List<State>();
            visited=new Dictionary<long, List<State>> ();  //集合set，已经访问过(集合（set）是一个无序的不重复元素序列)
            Q= new Queue<State>();  //队列

            oo.from = -1;     //来自状态(初始状态)
            oo.index = 0;     //当前状态的index
            ////////////////////////////////////////////////////////////////////////////////////
           
            states.Add(oo);   //保存所有状态
            ///////////////////////////////////////////////////////////////////////////////////
            Q.Enqueue(oo);   //初始状态

            //////////////////////////////////////////////////////////////////////////////////
            visited.Clear();
            long stateKey = getHash(oo);   //计算出一个哈希值，然后放到哈希表里；visited，用于快速判断是否访问过，是否有重复的状态
            oo.hashValue = stateKey;
            if (visited.ContainsKey(stateKey) == false)
            {
                visited[stateKey] = new List<State>();
            }
            visited[stateKey].Add(oo);

            //////////////////////////////////////////////////////////////////////////////////

            while (Q.Count > 0)
            {
                State ff = Q.Dequeue();  //弹出第一个

                if (states.Count >= 25000000)
                {   //超过2千5百万个状态
                    Debug.LogError("states.Count >= 25000000");
                    return null;
                }

                if (checkWin(ff))
                {   //检查当前状态是否胜利
                    /////// oh yeah!!!
                    win(ff.index); // result
                    return result;
                }

                //广度优先遍历，使用队列
                //每一块木头，移动一个位置形成一个新状态；（遍历每一块木头，一块木头当前所有可行位置状态），这些新状态都是ff的子状态
                //状态的Hash值一样表示相同的状态，不进入队列
                for (byte i = 1; i <= redIndex; i++)
                {   //逐个木块尝试，最后尝试红木 （每一个木块移动一次形成一个新状态），这些状态都是ff的子状态        
                        moveLeftAndRight(ff, i);
                        moveUpAndDown(ff, i);
                }
            }

            return null;
        }

        //左右走
        private void moveLeftAndRight(State ff,byte i)
        {
            SGridCell firstCol;
            SGridCell lastCol;

            SGridCell firstRow;
            SGridCell lastRow;

            firstCol = getMinColGridCell(ff.planks[i]);  //此处要克隆
            lastCol = getMaxColGridCell(ff.planks[i]);  //此处要克隆

            firstRow = getMinRowGridCell(ff.planks[i]);  //此处要克隆
            lastRow = getMaxRowGridCell(ff.planks[i]);  //此处要克隆

            int rr = firstRow.row;   //行
            int rr2 = lastRow.row;

            int cc = lastCol.col;     //列

            while (cc < NC - 1)
            {
                cc++;   //往右移动x格 
                if (ff.board[rr][cc].shapeIndex == 0 && ff.board[rr2][cc].shapeIndex == 0)
                {
                    State newState = cloneStateFun(ff);  //深度克隆状态

                    moveCol(i, newState, newState.planks[i], (sbyte)(cc - lastCol.col));


                    //盘面属性
                    for (int ty = firstCol.col; ty <= lastCol.col; ty++)
                    {
                        newState.board[firstRow.row][ty].shapeIndex = 0;
                        newState.board[lastRow.row][ty].shapeIndex = 0;
                    }

                    for (int ty = firstCol.col + cc - lastCol.col; ty <= cc; ty++)
                    {
                        newState.board[firstRow.row][ty].shapeIndex = i;
                        newState.board[lastRow.row][ty].shapeIndex = i;
                    }


                    enQueue(newState, ff.index);
                }
                else
                {
                    break;
                }
            }

            cc = firstCol.col;   //行,// left（往左移动）
                                // left（往左移动）
            while (cc > 0)
            {
                cc--;
                if (ff.board[rr][cc].shapeIndex == 0 && ff.board[rr2][cc].shapeIndex == 0)
                {
                    State newState = cloneStateFun(ff);  //深度克隆状态

                    moveCol(i, newState, newState.planks[i], (sbyte)(cc - firstCol.col));

                    //盘面属性
                    for (int ty = firstCol.col; ty <= lastCol.col; ty++)
                    {
                        newState.board[firstRow.row][ty].shapeIndex = 0;
                        newState.board[lastRow.row][ty].shapeIndex = 0;
                    }
                       
                    for (int ty = cc; ty <= lastCol.col - (firstCol.col - cc); ty++)
                    {
                        newState.board[firstRow.row][ty].shapeIndex = i;
                        newState.board[lastRow.row][ty].shapeIndex = i;
                    }
                       

                    enQueue(newState, ff.index);
                }
                else
                {
                    break;
                } 
            }
            
        }

        //上下走
        private void moveUpAndDown(State ff, byte i)
        {
            SGridCell firstCol;
            SGridCell lastCol;

            SGridCell firstRow;
            SGridCell lastRow;

            firstCol = getMinColGridCell(ff.planks[i]);  //此处要克隆
            lastCol = getMaxColGridCell(ff.planks[i]);  //此处要克隆

            firstRow = getMinRowGridCell(ff.planks[i]);  //此处要克隆
            lastRow = getMaxRowGridCell(ff.planks[i]);  //此处要克隆


            int rr = lastRow.row;
            int rr2 = firstRow.row;

            int cc1 = firstCol.col;
            int cc2 = lastCol.col;
            //up,向上走
            while (rr < NR - 1)
            {
                rr++;
                if (ff.board[rr][cc1].shapeIndex == 0 && ff.board[rr][cc2].shapeIndex == 0)
                {
                    State newState = cloneStateFun(ff);  //深度克隆状态

                    moveRow(i, newState, newState.planks[i], (sbyte)(rr - lastRow.row));

                    for (int tx = firstRow.row; tx <= lastRow.row; tx++)
                    {
                        newState.board[tx][firstCol.col].shapeIndex = 0;
                        newState.board[tx][lastCol.col].shapeIndex = 0;
                    }
                       
                    for (int tx = firstRow.row + (rr - lastRow.row); tx <= rr; tx++)
                    {
                        newState.board[tx][firstCol.col].shapeIndex = i;
                        newState.board[tx][lastCol.col].shapeIndex = i;
                    }
                    
                    enQueue(newState, ff.index);
                }
                else
                {
                    break;
                }    
            }

            rr = firstRow.row;
            rr2 = lastRow.row;
            // down
            while (rr > 0)
            {
                rr--;
                if (ff.board[rr][cc1].shapeIndex == 0 && ff.board[rr][cc2].shapeIndex == 0)
                {
                    State newState = cloneStateFun(ff);  //深度克隆状态

                    moveRow(i, newState, newState.planks[i], (sbyte)(rr - firstRow.row));

                    for (int tx = firstRow.row; tx <= lastRow.row; tx++)
                    {
                        newState.board[tx][firstCol.col].shapeIndex = 0;
                        newState.board[tx][lastCol.col].shapeIndex = 0;
                    }
                      
                    for (int tx = rr; tx <= lastRow.row - (firstRow.row - rr); tx++)
                    {
                        newState.board[tx][firstCol.col].shapeIndex = i;
                        newState.board[tx][lastCol.col].shapeIndex = i;
                    }
                       

                    enQueue(newState, ff.index);
                }
                else
                {
                    break;
                }
                  
            }
        }


        public void moveCol(byte moveNo,State state,List<SGridCell> gridCells,sbyte moveCol)
        {
            foreach (SGridCell gridCell in gridCells)
            {
                gridCell.col = (byte)(gridCell.col + moveCol);
            }

            state.action.No = moveNo;
            state.action.moveCol = moveCol;
            state.action.moveRow = 0;
        }


        public void moveRow(byte moveNo, State state, List<SGridCell> gridCells, sbyte moveRow)
        {
            foreach (SGridCell gridCell in gridCells)
            {
                gridCell.row = (byte)(gridCell.row + moveRow);
            }

            state.action.No = moveNo;
            state.action.moveRow = moveRow;
            state.action.moveCol = 0;
        }

        public SGridCell getMinRowGridCell(List<SGridCell> gridCells)
        {
            SGridCell gridCell = gridCells[0];
            if (gridCells.Count >= 2)
            {
                if (gridCells[1].row < gridCell.row)
                {
                    gridCell = gridCells[1];
                }
            }

            if (gridCells.Count == 4)
            {
                if (gridCells[2].row < gridCell.row)
                {
                    gridCell = gridCells[2];
                }

                if (gridCells[3].row < gridCell.row)
                {
                    gridCell = gridCells[3];
                }
            }

            return gridCell.cloneCell();
        }


        public SGridCell getMaxRowGridCell(List<SGridCell> gridCells)
        {
            SGridCell gridCell = gridCells[0];
            if (gridCells.Count >= 2)
            {
                if (gridCells[1].row > gridCell.row)
                {
                    gridCell = gridCells[1];
                }
            }

            if (gridCells.Count == 4)
            {
                if (gridCells[2].row > gridCell.row)
                {
                    gridCell = gridCells[2];
                }

                if (gridCells[3].row > gridCell.row)
                {
                    gridCell = gridCells[3];
                }
            }

            return gridCell.cloneCell();
        }

        public SGridCell getMinColGridCell(List<SGridCell> gridCells)
        {
            SGridCell gridCell = gridCells[0];

            if (gridCells.Count >= 2)
            {
                if (gridCells[1].col < gridCell.col)
                {
                    gridCell = gridCells[1];
                }
            }

            if (gridCells.Count==4)
            {
                if (gridCells[2].col < gridCell.col)
                {
                    gridCell = gridCells[2];
                }

                if (gridCells[3].col < gridCell.col)
                {
                    gridCell = gridCells[3];
                }
            }

            return gridCell.cloneCell();
        }

        public SGridCell getMaxColGridCell(List<SGridCell> gridCells)
        {
            SGridCell gridCell = gridCells[0];

            if (gridCells.Count >=2)
            {
                if (gridCells[1].col > gridCell.col)
                {
                    gridCell = gridCells[1];
                }
            }

            if (gridCells.Count == 4)
            {
                if (gridCells[2].col >gridCell.col)
                {
                    gridCell = gridCells[2];
                }

                if (gridCells[3].col > gridCell.col)
                {
                    gridCell = gridCells[3];
                }
            }

            return gridCell.cloneCell();
        }

    }

}

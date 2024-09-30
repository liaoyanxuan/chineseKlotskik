using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BizzyBeeGames.Blocks
{
	public class AutoGenerationWorker : Worker
	{
		#region Classes

		private class Data
		{
			public List<List<CellData>> gridCells;
		}

		private class CellData
		{
			public int	x;  //行
			public int	y;  //列
			public bool	isBlank;  //空白
			public bool	isBlock; //阻挡
			public int	shapeIndex;  //shape索引

			public int regionMarker;

			public bool IsEmpty { get { return !isBlock && !isBlank && shapeIndex == -1; } }
		}

		private class Region
		{
			public List<CellData> cells;
		}

		private class Grow
		{
			public CellData	fromCell;
			public int		xDir;
			public int		yDir;
		}

		#endregion // Classes

		#region Member Variables

		private LevelData.LevelType	levelType;
		private bool				rotateHexagon;
		private int					xCells;
		private int					yCells;
		private List<List<int>>		cellTypes;
		private int					numShapes;
		private int					minShapeSize;  //最小的shap尺寸（有几个基本形状组成）
        private int					maxShapeSize; //最大的shap尺寸（有几个基本形状组成）

        private Data				data;
		private int					currentRegionMarker;

		private System.Random 		random;

		#endregion // Member Variables

		#region Public Methods
		
		/// <summary>
		/// Fills the board with shapes
		/// </summary>
		public AutoGenerationWorker(LevelData.LevelType levelType, bool rotateHexagon, int xCells, int yCells, List<List<int>> cellTypes, int numShapes, int minShapeSize, int maxShapeSize)
		{
			this.levelType		= levelType;
			this.rotateHexagon	= rotateHexagon;
			this.xCells			= xCells;
			this.yCells			= yCells;
			this.cellTypes		= cellTypes;
			this.numShapes		= numShapes;
			this.minShapeSize	= minShapeSize;
			this.maxShapeSize	= maxShapeSize;

			random = new System.Random();
		}

		public AutoGenerationWorker(){}

		/// <summary>
		/// Returns the generated grid of cells, 0 is blank, 1 is block, and > 1 is the shape id
		/// </summary>
		public List<List<int>> GetGrid()
		{
			List<List<int>> grid = new List<List<int>>();

			for (int y = 0; y < yCells; y++)
			{
				grid.Add(new List<int>());

				for (int x = 0; x < xCells; x++)
				{
					CellData cell = data.gridCells[y][x];

					if (cell.isBlank)
					{
						grid[y].Add(0);
					}
					else if (cell.isBlock)
					{
						grid[y].Add(1);
					}
					else
					{
						grid[y].Add(cell.shapeIndex + 2);
					}
				}
			}

			return grid;
		}
		
		#endregion // Public Methods

		#region Protected Methods
		
		protected override void Begin()
		{

		}

		protected override void DoWork()
		{
			// Create and initialize the data object
			CreateData();

            // Get all the starting blank regions
            //可能存在多个区域 Region 的情况，区域之间不连通
            List<Region> startingRegions = GetRegions();

			// First check if it is not possible to fill the regions given the number of shapes and the min/max shape size
			if (!CheckCanFillRemainingRegions(startingRegions, 0, 0))
			{
				// Adjust the min/max shape size values
				if (startingRegions.Count > 1)
				{
					// If there are more than 1 region then it would be to complicated to get the percise min/max values so just set it to 1 and max integer
					// The algo will take a little longer in some cases but that's the price we pay for complicated boards
					minShapeSize = 1;
					maxShapeSize = int.MaxValue;
				}
				else   //只有一个Region（区域）
				{
					int		cellCount		= startingRegions[0].cells.Count;
					float	cellsPerShape	= (float)cellCount / (float)numShapes;   //每一个shape占据多少cells

                    //最小shapeSize的上限，肯定不会比这个更大（比这个更大可以分配给其它）
                    int maximumMinShapeSize = Mathf.Max(1, Mathf.FloorToInt(cellsPerShape));

					minShapeSize = Mathf.Clamp(minShapeSize, 1, maximumMinShapeSize);

                    //最大ShapeSize的下限，（最大能到多大）不会比平均分配更小
                    int minimumMaxShapeSize = Mathf.CeilToInt(cellsPerShape);

                    //其他的都采用最小的，剩下的就是最大的
					int maximumMaxShapeSize = cellCount - minShapeSize * (numShapes - 1);

					maxShapeSize = Mathf.Clamp(maxShapeSize, minimumMaxShapeSize, maximumMaxShapeSize);
				}
			}

			// Fill the board with shapes
			if (FillRegions(startingRegions, 0, 0))
			{
				// Now that we have a valid board, randomize the shapes by growing the shapes in random directions
				RandomizeBoard();
			}
			else
			{
				error = "Could not fill board with shapes.";
			}

			Stop();
		}
		
		#endregion // Protected Methods

		#region Private Methods

		private void CreateData()
		{
			data			= new Data();
			data.gridCells	= new List<List<CellData>>();

			for (int y = 0; y < yCells; y++)
			{
				data.gridCells.Add(new List<CellData>());

				for (int x = 0; x < xCells; x++)
				{
					CellData cellData = new CellData();

					cellData.x			= x;
					cellData.y			= y;
					cellData.isBlank	= cellTypes != null && cellTypes[y][x] == 0;
					cellData.isBlock	= cellTypes != null && cellTypes[y][x] == 1;
					cellData.shapeIndex	= -1;

					data.gridCells[y].Add(cellData);
				}
			}
		}

		private List<Region> GetRegions()
		{
			currentRegionMarker++;

			List<Region> regions = new List<Region>();
			
			for (int y = 0; y < yCells; y++)
			{
				for (int x = 0; x < xCells; x++)
				{
					CellData cellData = data.gridCells[y][x];

					// If the cell is blank and has not been added to a region yet create another region starting from it
					if (cellData.IsEmpty && cellData.regionMarker != currentRegionMarker)
					{
						regions.Add(GetRegion(cellData));
					}
				}
			}

			return regions;
		}

        //cells 代表的区域
        private List<Region> GetRegions(List<CellData> cells, bool onlyEmptyCells, int shapeIndex)
		{
			currentRegionMarker++;

			List<Region> regions = new List<Region>();

			for (int i = 0; i < cells.Count; i++)
			{
				CellData cellData = cells[i];

				// If the cell is blank and has not been added to a region yet create another region starting from it
				if (IsCellInRegion(cellData, onlyEmptyCells, shapeIndex) && cellData.regionMarker != currentRegionMarker)
				{
					regions.Add(GetRegion(cellData, onlyEmptyCells, shapeIndex));
				}
			}

			return regions;
		}

        //与startCell连通的cell所组成的区域
        private Region GetRegion(CellData startCell, bool onlyEmptyCells = true, int shapeIndex = -1)
		{
			Region region = new Region();
			region.cells = new List<CellData>();

			Queue<CellData> checkCells = new Queue<CellData>();

			checkCells.Enqueue(startCell);

			while (checkCells.Count > 0)
			{
				CellData cell = checkCells.Dequeue();

				if (cell.regionMarker != currentRegionMarker)
				{
					// Add the cell to the list of region cells
					region.cells.Add(cell);

					// Set the region marker so it's not processed/added again
					cell.regionMarker = currentRegionMarker;

					// Get all the blank neighours 
					List<CellData> neighbours = GetNeighbours(cell, onlyEmptyCells, shapeIndex);

					// Queue each neighbour to be checked and added to the list of region cells
					for (int i = 0; i < neighbours.Count; i++)
					{
						checkCells.Enqueue(neighbours[i]);
					}
				}
			}

			return region;
		}

		private List<CellData> GetNeighbours(CellData cell, bool onlyEmptyCells, int shapeIndex = -1)
		{
			switch (levelType)
			{
				case LevelData.LevelType.Square:
				{
					return GetValidSquareNeighbours(cell, onlyEmptyCells, shapeIndex);
				}
				case LevelData.LevelType.Triangle:
				{
					return GetValidTriangleNeighbours(cell, onlyEmptyCells, shapeIndex);
				}
				case LevelData.LevelType.Hexagon:
				{
					return GetValidHexagonNeighbours(cell, onlyEmptyCells, shapeIndex);
				}
			}

			return null;
		}

        /// <summary>
        /// 方形 --> 邻居块：左右上下
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="onlyEmptyCells"></param>
        /// <param name="shapeIndex"></param>
        /// <returns></returns>
		private List<CellData> GetValidSquareNeighbours(CellData cell, bool onlyEmptyCells, int shapeIndex)
		{
			List<CellData> neighbours = new List<CellData>();

			AddIfValid(cell.x - 1, cell.y, neighbours, onlyEmptyCells, shapeIndex);
			AddIfValid(cell.x + 1, cell.y, neighbours, onlyEmptyCells, shapeIndex);
			AddIfValid(cell.x, cell.y - 1, neighbours, onlyEmptyCells, shapeIndex);
			AddIfValid(cell.x, cell.y + 1, neighbours, onlyEmptyCells, shapeIndex);

			return neighbours;
		}

		private List<CellData> GetValidTriangleNeighbours(CellData cell, bool onlyEmptyCells, int shapeIndex)
		{
			List<CellData> neighbours = new List<CellData>();

			bool upsideDown = (cell.x + cell.y) % 2 == 1;

			AddIfValid(cell.x - 1, cell.y, neighbours, onlyEmptyCells, shapeIndex);
			AddIfValid(cell.x + 1, cell.y, neighbours, onlyEmptyCells, shapeIndex);
			AddIfValid(cell.x, cell.y + (upsideDown ? -1 : 1), neighbours, onlyEmptyCells, shapeIndex);

			return neighbours;
		}

		private List<CellData> GetValidHexagonNeighbours(CellData cell, bool onlyEmptyCells, int shapeIndex)
		{
			List<CellData> neighbours = new List<CellData>();

			if (rotateHexagon)
			{
				if (cell.x % 2 == 0)
				{
					AddIfValid(cell.x - 1, cell.y - 1, neighbours, onlyEmptyCells, shapeIndex);
					AddIfValid(cell.x + 0, cell.y - 1, neighbours, onlyEmptyCells, shapeIndex);
					AddIfValid(cell.x + 1, cell.y - 1, neighbours, onlyEmptyCells, shapeIndex);
					AddIfValid(cell.x + 1, cell.y + 0, neighbours, onlyEmptyCells, shapeIndex);
					AddIfValid(cell.x + 0, cell.y + 1, neighbours, onlyEmptyCells, shapeIndex);
					AddIfValid(cell.x - 1, cell.y + 0, neighbours, onlyEmptyCells, shapeIndex);
				}
				else
				{
					AddIfValid(cell.x - 1, cell.y + 0, neighbours, onlyEmptyCells, shapeIndex);
					AddIfValid(cell.x + 0, cell.y - 1, neighbours, onlyEmptyCells, shapeIndex);
					AddIfValid(cell.x + 1, cell.y + 0, neighbours, onlyEmptyCells, shapeIndex);
					AddIfValid(cell.x + 1, cell.y + 1, neighbours, onlyEmptyCells, shapeIndex);
					AddIfValid(cell.x + 0, cell.y + 1, neighbours, onlyEmptyCells, shapeIndex);
					AddIfValid(cell.x - 1, cell.y + 1, neighbours, onlyEmptyCells, shapeIndex);
				}
			}
			else
			{
				if (cell.y % 2 == 0)
				{
					AddIfValid(cell.x - 1, cell.y, neighbours, onlyEmptyCells, shapeIndex);
					AddIfValid(cell.x - 1, cell.y - 1, neighbours, onlyEmptyCells, shapeIndex);
					AddIfValid(cell.x, cell.y - 1, neighbours, onlyEmptyCells, shapeIndex);
					AddIfValid(cell.x + 1, cell.y, neighbours, onlyEmptyCells, shapeIndex);
					AddIfValid(cell.x, cell.y + 1, neighbours, onlyEmptyCells, shapeIndex);
					AddIfValid(cell.x - 1, cell.y + 1, neighbours, onlyEmptyCells, shapeIndex);
				}
				else
				{
					AddIfValid(cell.x - 1, cell.y, neighbours, onlyEmptyCells, shapeIndex);
					AddIfValid(cell.x, cell.y - 1, neighbours, onlyEmptyCells, shapeIndex);
					AddIfValid(cell.x + 1, cell.y - 1, neighbours, onlyEmptyCells, shapeIndex);
					AddIfValid(cell.x + 1, cell.y, neighbours, onlyEmptyCells, shapeIndex);
					AddIfValid(cell.x + 1, cell.y + 1, neighbours, onlyEmptyCells, shapeIndex);
					AddIfValid(cell.x, cell.y + 1, neighbours, onlyEmptyCells, shapeIndex);
				}
			}

			return neighbours;
		}

		/// <summary>
		/// Adds the cell at x, y if x/y is a valid coordinate and the cell is a blank cell
		/// </summary>
		private void AddIfValid(int x, int y, List<CellData> cells, bool onlyEmptyCells, int shapeIndex)
		{
			if (x >= 0 && y >= 0 && x < xCells && y < yCells)
			{
				CellData cellData = data.gridCells[y][x];

				if (IsCellInRegion(cellData, onlyEmptyCells, shapeIndex))
				{
					cells.Add(cellData);
				}
			}
		}

		private bool IsCellInRegion(CellData cellData, bool onlyEmptyCells, int shapeIndex)
		{
			if (onlyEmptyCells)
			{
				if (cellData.IsEmpty)
				{
					return true;
				}
			}
			else if (!cellData.isBlank && !cellData.isBlock)
			{
				if (shapeIndex == -1 || shapeIndex == cellData.shapeIndex)
				{
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Fills all the given regions with shapes. Returns false if there was no possible way to fit the remaining shapes in the given regions
		/// </summary>
		private bool FillRegions(List<Region> regions, int regionIndex, int shapeIndex)
		{
			if (Stopping) return false;

			if (regionIndex >= regions.Count)
			{
				// Check if there are still shapes that need to be placed on the board, if so then this is an invalid board
				if (shapeIndex < numShapes)
				{
					return false;
				}

				// All regions have been filled, the board is not filled and complete
				return true;
			}

			// Check if we can even fill the remaining regions with the number of remaining shapes
			if (!CheckCanFillRemainingRegions(regions, regionIndex, shapeIndex))
			{
				return false;
			}

			// Get the next region to fill with shapes
			Region region = regions[regionIndex];

			// Try and fill the region starting from each of the blank cells in the region
			for (int i = 0; i < region.cells.Count; i++)
			{
				if (Stopping) return false;

				CellData cell = region.cells[i];

				if (SpreadShape(regions, regionIndex, shapeIndex, region, cell, new List<CellData>(), new List<CellData>(), new HashSet<string>()))
				{
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Does some quick checks to see if there is no possible way to fit the remaining shapes in the remaining regions
        /// 快速检查剩余的shape是否能填充剩余的区域
		/// </summary>
		private bool CheckCanFillRemainingRegions(List<Region> regions, int regionIndex, int shapeIndex)
		{
			if (Stopping) return false;

            //剩余区域数量
			int numRemainingRegions = regions.Count - regionIndex;
            //剩余shape数量
			int numRemainingShapes	= numShapes - shapeIndex;

            //剩余区域比剩余shape数量多
			if (numRemainingRegions > numRemainingShapes)
			{
				// If the number of remaining shapes is less than the number of remaining regions then we can't fill the remaining regions
				return false;
			}

			// Get the maximum number of shapes that can fit in all regions
			int maxShapesCanFit = 0;
			int minShapesCanFit = 0;

			for (int i = regionIndex; i < regions.Count; i++)
			{
				if (Stopping) return false;

				Region region = regions[i];
				int numCellsInRegion = region.cells.Count;

				// Check if the region is to small to fit a single block
                //区域中格子的数量比 最小shape尺寸还要小
				if (numCellsInRegion < minShapeSize)
				{
					return false;
				}

				// Increate the minimum number of shapes needed to fill all regions
                //最少需要多少个shape
				minShapesCanFit += GetMinNumShapesToFill(numCellsInRegion);

				// Check if the minimum number of shpaes needed to fill all regions is now greater than the number of shapes left then we can't
				// possibly fill the remaining regions with the remaining shapes
                //至少需要的shape大于剩余可用的shape；
				if (minShapesCanFit > numRemainingShapes)
				{
					return false;
				}

				// Get the maximum number of shapes that can fit in this region
                //最多可以使用多少个shape，假设都是用minShapeSize
				int maxShapes = Mathf.FloorToInt(numCellsInRegion / minShapeSize);

				if (maxShapes == 0)
				{
					// This region is to small to fit a shape
					return false;
				}

				maxShapesCanFit += maxShapes;
			}

            //最多可使用的shape 小于 剩余可用的shape
			if (maxShapesCanFit < numRemainingShapes)
			{
				// If the maximum number of shapes is less than the remaining number of shapes then there is no way to fit the remaining shapes in the remaining regions
				return false;
			}

			return true;
		}

        /// <summary>
        /// Gets the minimum number of shapes needed ot fill the region
        /// val:numCellsInRegion
        /// </summary>
        private int GetMinNumShapesToFill(int val)
		{
			if (val < minShapeSize)
			{
				return 0;
			}

			for (int i = maxShapeSize; i >= minShapeSize; i--)
			{
				if (i > val)
				{
					continue;
				}

				if (i == val)
				{
					return 1;  //范围在 [maxShapeSize,minShapeSize]
                }

                //val > i的情况，即region中的cell比 maxShapeSize 还要大
                //递归，i占用了1个shape， 继续求解剩下的 val-i 会占据多少个shape
                int result = GetMinNumShapesToFill(val - i);

				if (result > 0)
				{
					return result + 1;
				}
			}

			return 0;
		}
        /// <summary>
        /// 扩展Shape,将一个cell放到一个shape里面
        /// </summary>
        /// <param name="regions"></param>
        /// <param name="regionIndex"></param>
        /// <param name="shapeIndex"></param>
        /// <param name="region"></param>
        /// <param name="cell"></param>
        /// <param name="shape"></param>
        /// <param name="allNeighbours"></param>
        /// <param name="triedShapes"></param>
        /// <returns></returns>
		private bool SpreadShape(List<Region> regions, int regionIndex, int shapeIndex, Region region, CellData cell, List<CellData> shape, List<CellData> allNeighbours, HashSet<string> triedShapes)
		{
			if (Stopping) return false;

			cell.shapeIndex = shapeIndex;

			string shapeHash = InsertSorted(cell, shape);

            //triedShapes:尝试的shape， shapeHash是Shape包括的Cell的坐标；
            if (triedShapes.Contains(shapeHash))
			{
				shape.Remove(cell);
				cell.shapeIndex = -1;

				return false;
			}

			triedShapes.Add(shapeHash);

			// Check if the shape is at it's maximum size
			if (shape.Count == maxShapeSize)  //达到最大size
			{
				if (ShapePlaced(regions, regionIndex, shapeIndex, region))
				{
					return true;
				}
			}
			// Else the shape is not at maximum size so try and spread it one more cell
			else
			{
				// Get the cells blank neighbours
				List<CellData> neighbours = GetNeighbours(cell, true);

				int allNeighboursCount = allNeighbours.Count;

				for (int i = 0; i < neighbours.Count; i++)
				{
					CellData neighbour = neighbours[i];

					if (!allNeighbours.Contains(neighbour))
					{
						allNeighbours.Add(neighbour);
					}
				}

				// Try and spread to each neighbour
				for (int i = 0; i < allNeighbours.Count; i++)
				{
					if (Stopping) return false;

					CellData neighbourToTry = allNeighbours[i];

					allNeighbours.RemoveAt(i);

					if (SpreadShape(regions, regionIndex, shapeIndex, region, neighbourToTry, shape, allNeighbours, triedShapes))
					{
						return true;
					}

					allNeighbours.Insert(i, neighbourToTry);
				}

				allNeighbours.RemoveRange(allNeighboursCount, allNeighbours.Count - allNeighboursCount);

				// If we get here, spearding the shape to each of this cells neighbours caused the board to be unsolvable
				// Check if the shape is larger than the min required size, if so try and fill the remaining regions now
				if (shape.Count >= minShapeSize && ShapePlaced(regions, regionIndex, shapeIndex, region))
				{
					return true;
				}
			}

			// Remove the cell from the shape
			shape.Remove(cell);
			cell.shapeIndex = -1;

			return false;
		}

        /// <summary>
        /// 在shape中，对cell进行插入（按x,y排序）
        /// </summary>
        /// <param name="cellToInsert"></param>
        /// <param name="shape"></param>
        /// <returns></returns>
		private string InsertSorted(CellData cellToInsert, List<CellData> shape)
		{
			string hash = "";
			bool inserted = false;

			for (int i = 0; i < shape.Count; i++)
			{
				CellData cell = shape[i];

				if (!inserted)
				{
					if (cellToInsert.x < cell.x)
					{
						shape.Insert(i, cellToInsert);
						inserted = true;
					}
					else if (cellToInsert.x == cell.x)
					{
						if (cellToInsert.y < cell.y)
						{
							shape.Insert(i, cellToInsert);
							inserted = true;
						}
					}
				}

				// Re-assign it incase cellToInsert was just inserted
				cell	= shape[i];
				hash	+= string.Format("{0}_{1}_", cell.x, cell.y);
			}

			if (!inserted)
			{
				shape.Add(cellToInsert);
				hash	+= string.Format("{0}_{1}_", cellToInsert.x, cellToInsert.y);
			}

			return hash;
		}

        /// <summary>
        /// 下一个区域或下一个shape
        /// </summary>
        /// <param name="regions"></param>
        /// <param name="regionIndex"></param>
        /// <param name="shapeIndex"></param>
        /// <param name="region"></param>
        /// <returns></returns>
		private bool ShapePlaced(List<Region> regions, int regionIndex, int shapeIndex, Region region)
		{
			if (Stopping) return false;

			// Get the new list of regions from the region we just added a shape to
			List<Region> newRegions = GetRegions(region.cells, true, -1);

			int newRegionIndex = regionIndex + 1;

			// Is newRegions is empty then there are no more blank cells in region
			if (newRegions.Count > 0)
			{
				// Add the remaining regions
				for (int i = newRegionIndex; i < regions.Count; i++)
				{
					newRegions.Add(regions[i]);
				}

				// We will now start from the beginning of the regions list again
				newRegionIndex = 0;
			}
			else
			{
				// Use the current list of regions
				newRegions = regions;
			}

			// Try and fill the remaining regions
			if (FillRegions(newRegions, newRegionIndex, shapeIndex + 1))
			{
				return true;
			}

			return false;
		}

		/// <summary>
		/// Randomizes the board by growing the shapes in random directions while maintaining a valid board based on the min/max shape sizes
		/// </summary>
		private void RandomizeBoard()
		{
			// Randomly grow shapes 1000 times
          
			for (int i = 0; i < 1000; i++)
			{   //1000次的随机占领方案，每一次都在上一次的基础上，每一次都独立，随机
				// Get all the possible grow directions for all shapes
				List<Grow> possibleGrows = GetPossibleGrows();

				if (possibleGrows.Count == 0)
				{
					return;
				}
                //随机选取一个占领方案
				// Pick a random shape/direction to grow
				Grow grow = possibleGrows[random.Next(0, possibleGrows.Count)];

				CellData fromCell	= grow.fromCell;
				CellData toCell		= data.gridCells[fromCell.y + grow.yDir][fromCell.x + grow.xDir];

				toCell.shapeIndex = fromCell.shapeIndex;
			}
		}

		/// <summary>
		/// Gets all cells that can grow in any direction
		/// </summary>
		private List<Grow> GetPossibleGrows()
		{
			List<Grow> possibleGrows = new List<Grow>();
            //获得shapes数组
			List<List<CellData>> shapes = GetShapeCells();

			for (int i = 0; i < shapes.Count; i++)
			{
				List<CellData> shapeCells = shapes[i];

				if (shapeCells.Count == maxShapeSize)
				{
					// This shape can't grow, it is already at the maximum shape size
					continue;
				}

				for (int j = 0; j < shapeCells.Count; j++)
				{
					CellData		cellData	= shapeCells[j];

					List<CellData>	neighbours	= GetNeighbours(cellData, false);

					for (int k = 0; k < neighbours.Count; k++)
					{
						CellData neighbour = neighbours[k];
                        //是否可以占领邻居
						if (cellData.shapeIndex != neighbour.shapeIndex && CanGrow(cellData, neighbour))
						{
							Grow grow		= new Grow();
							grow.fromCell	= cellData;
							grow.xDir		= neighbour.x - cellData.x;
							grow.yDir		= neighbour.y - cellData.y;
                            //占领的可能性方案，每个方案间是独立的
                            possibleGrows.Add(grow); 
						}
					}
				}
			}

			return possibleGrows;
		}

		private List<List<CellData>> GetShapeCells()
		{
			List<List<CellData>> shapes = new List<List<CellData>>();

			for (int i = 0; i < numShapes; i++)
			{
				shapes.Add(new List<CellData>());
			}

			for (int y = 0; y < yCells; y++)
			{
				for (int x = 0; x < xCells; x++)
				{
					CellData cellData = data.gridCells[y][x];

					if (cellData.isBlank || cellData.isBlock)
					{
						continue;
					}

					shapes[cellData.shapeIndex].Add(cellData);
				}
			}

			return shapes;
		}

		private bool CanGrow(CellData fromCell, CellData toCell)
		{
			int toCellShapeIndex = toCell.shapeIndex;

			// Set teh to cells shapeIndex so it's not included in the count we are about to do
			toCell.shapeIndex = fromCell.shapeIndex;

			// Get all the shapes neighbours that belong to the toCells shape then get the region for the shape
            //交换后获得regions，验证是否可行
			List<CellData>	neighbours	= GetNeighbours(toCell, false, toCellShapeIndex);

			List<Region>	regions		= GetRegions(neighbours, false, toCellShapeIndex);

			// Set the shape index back
			toCell.shapeIndex = toCellShapeIndex;

			// Check if growing to this cell causes the shape to split or the shape is now smaller than the min shape size
            //移动后是否分割成多个区域，是否区域中的格子数小于最小尺寸
			if (regions.Count != 1 || regions[0].cells.Count < minShapeSize)
			{
				return false;  //不可移动
			}

			return true;  //可移动，移动后仍然在一个区域
		}

		#endregion // Private Methods

		#region Debug Methods

		private string PrintCells(string header)
		{
			string print = header;

			for (int y = 0; y < yCells; y++)
			{
				print += "\n";

				for (int x = 0; x < xCells; x++)
				{
					CellData cell = data.gridCells[y][x];

					if (cell.isBlock)
					{
						print += "#";
					}
					else if (cell.isBlank)
					{
						print += "_";
					}
					else if (cell.shapeIndex == -1)
					{
						print += "E";
					}
					else
					{
						print += cell.shapeIndex.ToString();
					}
				}
			}

			return print;
		}

		#endregion // Debug Methods
	}
}

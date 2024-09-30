using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BizzyBeeGames.Blocks
{
	public class LevelData
    {
		#region Classes

		public class Shape
		{
			public int				index;
			public RectInt			bounds;  ////shape的包围盒bounds 尺寸
            public List<CellPos>	cellPositions;  //cell坐标,所在列行
           

			/// <summary>
			/// Gets the CellPosition to use when positioning the shape on the grid
			/// </summary>
			public CellPos	Anchor			{ get { return cellPositions[0]; } }

			/// <summary>
			/// For triangle levels, returns true if the tile at the Anchor cell position is upside down
			/// </summary>
			public bool		IsAnchorFlipped	{ get { return (Anchor.x + Anchor.y) % 2 == 1; } }
		}

		#endregion // Classes

		#region Enums

        public enum LevelType
        {
            Square,
            Triangle,
            Hexagon
        }

		public enum CellType
		{
			Blank,
			Block,
			Normal
		}

		public enum HexagonOrientation
		{
			Vertical,
			Horizontal
		}

        #endregion

		#region Member Variables

		private TextAsset				levelFile;
		private string					levelFileText;
		private bool					isLevelFileParsed;

		// Values parsed from level file
		private string					timestamp;
		private LevelType				levelType;
		private HexagonOrientation		hexagonOrientation;
		private int						yCells;
		private int						xCells;
		private List<List<CellType>>	gridCellTypes;
		private List<Shape>				shapes;

		#endregion

		#region Properties

		public string				Id				{ get; private set; }
		public string				PackId			{ get; private set; }
		public int					LevelIndex		{ get; private set; }

		public string				Timestamp		{ get { if (!isLevelFileParsed) ParseLevelFile(); return timestamp; } }
		public LevelType			Type			{ get { if (!isLevelFileParsed) ParseLevelFile(); return levelType; } }
		public bool					IsVertHexagons	{ get { if (!isLevelFileParsed) ParseLevelFile(); return hexagonOrientation == HexagonOrientation.Vertical; } }
		public int					YCells			{ get { if (!isLevelFileParsed) ParseLevelFile(); return yCells; } }
		public int					XCells			{ get { if (!isLevelFileParsed) ParseLevelFile(); return xCells; } }
		public List<List<CellType>>	GridCellTypes	{ get { if (!isLevelFileParsed) ParseLevelFile(); return gridCellTypes; } }
		public List<Shape>			Shapes			{ get { if (!isLevelFileParsed) ParseLevelFile(); return shapes; } }


        private string[] achieveColors = null;


        private Color[] aColor = null;

        public Color[] AColor
        {
            get
            {
                if (achieveColors == null)
                {
                    return null;
                }
                else
                {
                    if (aColor != null)
                    {
                        return aColor;
                    }

                    aColor = new Color[achieveColors.Length];

                    for (int i = 0; i < achieveColors.Length; i ++)
                    {
                        string colorStr = achieveColors[i];
                        Color achieveColor = Color.clear;

                        if (colorStr == "-1")
                        {
                            achieveColor = Color.clear;
                        }
                        else
                        {
                            ColorUtility.TryParseHtmlString(colorStr, out achieveColor);
                        }


                        aColor[i]=achieveColor;
                    }

                    return aColor;
                }
            }
        }


        public string LevelFileText
		{
			get
			{
				if (string.IsNullOrEmpty(levelFileText) && levelFile != null)
				{
					levelFileText	= levelFile.text;
					levelFile		= null;
				}

				return levelFileText;
			}
		}

		#endregion

		#region Constructor

		public LevelData(TextAsset levelFile, string packId, int levelIndex)
		{
			this.levelFile	= levelFile;
			PackId			= packId;
			LevelIndex		= levelIndex;
			Id				= string.Format("{0}_{1}", packId, levelIndex);
		}


        public bool isNewGuideLevel()
        {
            if (LevelIndex == 0 && PackId.IndexOf("beginner") >= 0)
            {
                return true;
            }
            return false;
        }

		#endregion

		#region Private Methods

		/// <summary>
		/// Parse the json in the level file
		/// </summary>
		public void ParseLevelFile()
		{
			if (isLevelFileParsed) return;

			string levelFileContentOriginal	= LevelFileText;

            string[] levelFileContentLines= levelFileContentOriginal.Split('\n');


            string levelFileContents = levelFileContentLines[0];
            achieveColors = null;

            if (levelFileContentLines.Length == 2)
            {
                string colorContents = levelFileContentLines[1];
                achieveColors = colorContents.Split(',');
            }
            

            string[]	items				= levelFileContents.Split(',');

			int itemIndex = 0;

			// First item is the timestamp for when the level file was generated
			timestamp = items[itemIndex++];

			// Next item is the level type
			levelType = (LevelType)int.Parse(items[itemIndex++]);

			// If the level type is Hexagon the the next value will determine the orentation of the hexagons 
			hexagonOrientation = (bool)bool.Parse(items[itemIndex++]) ? HexagonOrientation.Horizontal : HexagonOrientation.Vertical;

			// Next two items are the yCells, and xCells
			yCells = int.Parse(items[itemIndex++]);
			xCells = int.Parse(items[itemIndex++]);

			gridCellTypes = new List<List<CellType>>();

            //一个颜色（int）代表一类
            Dictionary<int, List<object>> shapeDatas = new Dictionary<int, List<object>>();
            

            // Rest of the items are the grid cell types and where the shapes are placed on the grid
            // Value of 0 means its a blank cell, 1 means its a block, > 1 are the shapes
            for (int y = 0; y < yCells; y++)
			{
				gridCellTypes.Add(new List<CellType>());

				for (int x = 0; x < xCells; x++)
				{

					int value = int.Parse(items[itemIndex++]);

					if (value == 0)
					{
						gridCellTypes[y].Add(CellType.Blank);
					}
					else if (value == 1)
					{
						gridCellTypes[y].Add(CellType.Block);
					}
					else
					{
						gridCellTypes[y].Add(CellType.Normal);

						List<object> shapeData = null;

						if (!shapeDatas.ContainsKey(value))
						{
							shapeData = new List<object>();
							shapeData.Add(int.MaxValue);
							shapeData.Add(int.MinValue);
							shapeData.Add(int.MaxValue);
							shapeData.Add(int.MinValue);

							shapeDatas[value] = shapeData;
						}
						else
						{
							shapeData = shapeDatas[value];
						}

						// Update the bounds of the shape
						shapeData[0] = System.Math.Min(Convert.ToInt32(shapeData[0]), x);	// Set left
						shapeData[1] = System.Math.Max(Convert.ToInt32(shapeData[1]), x);	// Set right
						shapeData[2] = System.Math.Min(Convert.ToInt32(shapeData[2]), y);	// Set top
						shapeData[3] = System.Math.Max(Convert.ToInt32(shapeData[3]), y);	// Set bottom

                       
						// Add the cell x/y
						shapeData.Add(x);
						shapeData.Add(y);

                        Color achieveColor = Color.clear;
                        if (achieveColors != null)
                        {
                            string colorstr = achieveColors[itemIndex-6];
                            //ColorUtility.TryParseHtmlString(colorstr, out achieveColor);
                            shapeData.Add(colorstr);
                        }
                    }
				}
			}

			// Create the shapes list
			shapes = new List<Shape>();
			int index = 0;

			foreach (KeyValuePair<int, List<object>> pair in shapeDatas)
			{
				List<object> shapeData = pair.Value;

				int left	= Convert.ToInt32(shapeData[0]);
				int right	= Convert.ToInt32(shapeData[1]);
				int top		= Convert.ToInt32(shapeData[2]);
				int bottom	= Convert.ToInt32(shapeData[3]);

				Shape shape = new Shape();

				shape.index			= index++;
				shape.bounds		= new RectInt(left, top, right - left + 1, bottom - top + 1);
				shape.cellPositions	= new List<CellPos>();

                if (achieveColors == null)
                {
                    for (int i = 4; i < shapeData.Count; i += 2)
                    {
                        shape.cellPositions.Add(new CellPos(Convert.ToInt32(shapeData[i]), Convert.ToInt32(shapeData[i + 1])));

                    }
                }
                else
                {
                    for (int i = 4; i < shapeData.Count; i += 3)
                    {
                        CellPos cellPos = new CellPos(Convert.ToInt32(shapeData[i]), Convert.ToInt32(shapeData[i + 1]));
                        shape.cellPositions.Add(cellPos);

                        string colorStr = Convert.ToString(shapeData[i + 2]);
                        Color achieveColor = Color.clear;

                        if (colorStr == "-1")
                        {
                            achieveColor = Color.clear;
                        }
                        else
                        {
                            ColorUtility.TryParseHtmlString(colorStr, out achieveColor);
                        }


                        cellPos.achieveColor=achieveColor;
                    }
                }
				

				shapes.Add(shape);
			}

			isLevelFileParsed = true;
		}

		#endregion
	}
}

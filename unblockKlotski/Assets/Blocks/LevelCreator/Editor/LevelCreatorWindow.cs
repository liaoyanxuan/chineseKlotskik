using System.Collections.Generic;

using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using static BizzyBeeGames.Blocks.LevelData;
using blocksolutions;
using System.Text;
using System;
using System.Reflection;

namespace BizzyBeeGames.Blocks
{
	public class LevelCreatorWindow : EditorWindow
	{
       
        public List<TextAsset> levelFiles;

        #region Classes

        public class GridCell
		{
			public int		row;   //所在行
            public int		col;   //所在列
            public Image	imageField;    //形状image: 矩形，三角，六角
            public int		shapeIndex;   //第几个形状块(每个形状快对应一种颜色)
            public Color shapeColor=Color.clear;    //shapeColor
            public Color acheveColor = Color.clear;   //成就Color


            public GridCell cloneCell()
            {
                GridCell newGridCell = new GridCell();

                newGridCell.row = this.row;
                newGridCell.col = this.col;
                newGridCell.shapeIndex = this.shapeIndex;

                return newGridCell;

            }
		}


        public class SGridCell
        {
            public byte row;   //所在行
            public byte col;   //所在列
            public byte shapeIndex;   //第几个形状块(每个形状快对应一种颜色)

            public SGridCell cloneCell()
            {
                SGridCell newGridCell = new SGridCell();

                newGridCell.row = this.row;
                newGridCell.col = this.col;
                newGridCell.shapeIndex = this.shapeIndex;
                return newGridCell;
            }
        }

        #endregion // Classes

        #region Enums

        #endregion // Enums

        #region Member Variables

        private const int	MaxCells			= 15;	// Maximum number of cells that can be in a row/column
		private const float MaxCellSize 		= 50;

		private const float ShapeBlockSize		= 25;
		private const float ShapeBlockSpacing	= 3;

		
		private const int	EmptyCellShapeIndex = 0;  //还原，初始块

        private Texture squareTexture;
		private Texture triangleTexture;
		private Texture hexagonTexture;

		private List<List<GridCell>>	grid;
		private GridCell				hoveredGridCell;
		private bool					isDragging;
		private List<VisualElement>		shapeBlocks;
		private List<Color>				shapeColors;
		private int						selectedShapeIndex;
	

		private bool	isGeneratingGrid;
		private bool	isBatchGenerating;
		private int		numLevelsLeftToGenerate;

        private  int RED_INDEX = 0;

		#endregion // Member Variables

		#region Properties

		// Containers
		private VisualElement	GridContainerElement	{ get { return rootVisualElement.Q("gridContainer") as VisualElement; } }
		private VisualElement	ShapesContainerElement	{ get { return rootVisualElement.Q("shapesContainer") as VisualElement; } }

		// Settings fields
		private EnumField		LevelTypeField			{ get { return rootVisualElement.Q("levelType") as EnumField; } }
		private Toggle			RotateHexagonField		{ get { return rootVisualElement.Q("rotateHexagon") as Toggle; } }
		private IntegerField	XCellsField				{ get { return rootVisualElement.Q("xCells") as IntegerField; } }
		private IntegerField	YCellsField				{ get { return rootVisualElement.Q("yCells") as IntegerField; } }
		private IntegerField	NumShapesField			{ get { return rootVisualElement.Q("numShapes") as IntegerField; } }

        private ColorField colorField                   { get { return rootVisualElement.Q("colorfield") as ColorField; } }
        private Toggle colorToggle                      { get { return rootVisualElement.Q("colorToggle") as Toggle; } }

        private LevelData.LevelType LevelType
		{
			get { return LevelCreatorData.Instance.levelType; }
			set { LevelCreatorData.Instance.levelType = value; }
		}

		private bool RotateHexagon
		{
			get { return LevelCreatorData.Instance.rotateHexagon; }
			set { LevelCreatorData.Instance.rotateHexagon = value; }
		}

		private int XCells
		{
			get { return LevelCreatorData.Instance.xCells; }
			set { LevelCreatorData.Instance.xCells = value; }
		}

		private int YCells
		{
			get { return LevelCreatorData.Instance.yCells; }
			set { LevelCreatorData.Instance.yCells = value; }
		}

		private int NumShapes
		{
			get { return LevelCreatorData.Instance.numShapes; }
			set { LevelCreatorData.Instance.numShapes = value; }
		}

		// Export fields
		private TextField		FilenameField			{ get { return rootVisualElement.Q("filename") as TextField; } }
		private ObjectField		OutputFolderField		{ get { return rootVisualElement.Q("outputFolder") as ObjectField; } }

		private string Filename
		{
			get { return FilenameField.value; }
			set { FilenameField.value = value; }
		}

		private string OutputFolderAssetPath
		{
			get { return LevelCreatorData.Instance.outputFolderAssetPath; }
			set { LevelCreatorData.Instance.outputFolderAssetPath = value; }
		}


        // Import fields
        private TextField InPutFilenameField { get { return rootVisualElement.Q("importfilename") as TextField; } }
        private ObjectField InputFolderField { get { return rootVisualElement.Q("inputFolder") as ObjectField; } }

        private string InPutFilename
        {
            get { return InPutFilenameField.value; }
            set { InPutFilenameField.value = value; }
        }

        private string InputFolderAssetPath
        {
            get { return LevelCreatorData.Instance.intputFolderAssetPath; }
            set { LevelCreatorData.Instance.intputFolderAssetPath = value; }
        }

        // Auto Generation fields
        private IntegerField	MinShapeSizeField	{ get { return rootVisualElement.Q("minShapeSize") as IntegerField; } }
		private IntegerField	MaxShapeSizeField	{ get { return rootVisualElement.Q("maxShapeSize") as IntegerField; } }
		private IntegerField	NumberOfLevelsField	{ get { return rootVisualElement.Q("numLevels") as IntegerField; } }

		private int MinShapeSize
		{
			get { return LevelCreatorData.Instance.minShapeSize; }
			set { LevelCreatorData.Instance.minShapeSize = value; }
		}

		private int MaxShapeSize
		{
			get { return LevelCreatorData.Instance.maxShapeSize; }
			set { LevelCreatorData.Instance.maxShapeSize = value; }
		}

		private int NumberOfLevels
		{
			get { return LevelCreatorData.Instance.numLevels; }
			set { LevelCreatorData.Instance.numLevels = value; }
		}

		// Textures
		private Texture SquareTexture	{ get { return (squareTexture == null ? squareTexture = AssetDatabase.LoadAssetAtPath<Texture>(LevelCreatorPaths.SquareTexturePath) : squareTexture); } }
		private Texture TriangleTexture	{ get { return (triangleTexture == null ? triangleTexture = AssetDatabase.LoadAssetAtPath<Texture>(LevelCreatorPaths.TriangleTexturePath) : triangleTexture); } }
		private Texture HexagonTexture	{ get { return (hexagonTexture == null ? hexagonTexture = AssetDatabase.LoadAssetAtPath<Texture>(LevelCreatorPaths.HexagonTexturePath) : hexagonTexture); } }

		#endregion // Properties

		#region Unity Methods

		[MenuItem("Tools/Bizzy Bee Games/Level Creator Window")]
		public static void ShowWindow()
		{
			LevelCreatorWindow wnd = GetWindow<LevelCreatorWindow>();
			wnd.titleContent = new GUIContent("Level Creator");
		}

		private void OnEnable()
		{
			grid				= new List<List<GridCell>>();
			shapeColors			= new List<Color>();
			shapeBlocks			= new List<VisualElement>();

			SetupWindowUI();
		}

		

		#endregion // Unity Methods

		#region Private Methods
		
		private void SetupWindowUI()
		{
			rootVisualElement.Clear();

			selectedShapeIndex = 0;

			// Get a reference to the UXML and USS files
			VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(LevelCreatorPaths.UXMLFilePath);
			StyleSheet      styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(LevelCreatorPaths.USSFilePath);

			if (visualTree == null || styleSheet == null)
			{
				Debug.LogError("The .uxml and/or .uss file could not be found for the Level Creator Window. If you have changed the location of these files please update the UXMLFilePath and USSFilePath inside LevelCreatorPaths.cs");
				
				return;
			}

			// Setup the base UI of the window
			visualTree.CloneTree(rootVisualElement);
			rootVisualElement.styleSheets.Add(styleSheet);

			// Setup the enums
			LevelTypeField.Init(LevelCreatorData.Instance.levelType);

			// Set the accepted type of the output folder object field to Object, we will check if it is actually a folder when the user drags something into the field
			OutputFolderField.objectType = typeof(UnityEngine.Object);

            InputFolderField.objectType = typeof(UnityEngine.Object);

            // Register a GeometryChangedEvent on the grid-container VisualElement so when it re-sizes we can update the size of the cells
            // This will be called once right away when the window first opens and sizes itself
            rootVisualElement.RegisterCallback<GeometryChangedEvent>((evt) => { ResizeGrid(); });
			rootVisualElement.RegisterCallback<GeometryChangedEvent>((evt) => { ResizeShapeBlocks(); });

			// Register value changed events
			LevelTypeField.RegisterCallback<ChangeEvent<System.Enum>>(LevelTypeChanged);
			RotateHexagonField.RegisterCallback<ChangeEvent<bool>>(RotateHexagonChanged);
			XCellsField.RegisterCallback<ChangeEvent<int>>(XYCellsChanged);
			YCellsField.RegisterCallback<ChangeEvent<int>>(XYCellsChanged);
			NumShapesField.RegisterCallback<ChangeEvent<int>>(NumShapesChanged);
			OutputFolderField.RegisterCallback<ChangeEvent<UnityEngine.Object>>((evt) => { UpdateOutputPaths(); });
			FilenameField.RegisterCallback<ChangeEvent<string>>((evt) => { UpdateOutputPaths(); });
			MinShapeSizeField.RegisterCallback<ChangeEvent<int>>(MinMaxCellsChanged);
			MaxShapeSizeField.RegisterCallback<ChangeEvent<int>>(MinMaxCellsChanged);


            colorToggle.RegisterCallback<ChangeEvent<bool>>(ColorToggleChanged);

            // Setup button click listeners
            (rootVisualElement.Q("clearShapesButton") as Button).clickable.clicked	+= ClearShapes;
			(rootVisualElement.Q("resetGridButton") as Button).clickable.clicked	+= ResetGrid;
			
			(rootVisualElement.Q("export") as Button).clickable.clicked				+= ExportClicked2;
            (rootVisualElement.Q("import") as Button).clickable.clicked += ImportClicked;
          

			// Bind the UI elements to the LevelCreatorData ScriptableObject fields
			XCellsField.bindingPath			= "xCells";
			YCellsField.bindingPath			= "yCells";
			NumShapesField.bindingPath		= "numShapes";
			FilenameField.bindingPath		= "filename";
			MinShapeSizeField.bindingPath	= "minShapeSize";
			MaxShapeSizeField.bindingPath	= "maxShapeSize";
			NumberOfLevelsField.bindingPath	= "numLevels";

			MinShapeSizeField.isDelayed = true;
			MaxShapeSizeField.isDelayed = true;

			// Bind the root element to the instance of LevelCreatorData
			rootVisualElement.Bind(new SerializedObject(LevelCreatorData.Instance));

			// Set the reference to the output folder
			if (!string.IsNullOrEmpty(OutputFolderAssetPath))
			{
				UnityEngine.Object outputFolder = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(OutputFolderAssetPath);

				// Check if the folder still exists
				if (outputFolder == null)
				{
					OutputFolderAssetPath = null;
				}

				OutputFolderField.value = outputFolder;
			}

			// Generate the inital list of shape colors
			GenerateShapeColors();

			// Build the initial grid cells matrix
			RebuildGridCells();

			UpdateOutputPaths();

			RotateHexagonField.value = RotateHexagon;
			RotateHexagonField.style.display = (LevelType == LevelData.LevelType.Hexagon) ? DisplayStyle.Flex : DisplayStyle.None;


           

        }

		private void LevelTypeChanged(ChangeEvent<System.Enum> evt)
		{
			// There is a bug with binding enums, need to manually set it when it changes
			LevelType = (LevelData.LevelType)LevelTypeField.value;

			// If Hexagon level type was selected show the Rotate Hexagon field
			RotateHexagonField.style.display = (LevelType == LevelData.LevelType.Hexagon) ? DisplayStyle.Flex : DisplayStyle.None;

			RebuildGridCells();
			ResizeGrid();
		}

		private void RotateHexagonChanged(ChangeEvent<bool> evt)
		{
			RotateHexagon = RotateHexagonField.value;

			ResizeGrid();
		}

		private void XYCellsChanged(ChangeEvent<int> evt)
		{
			XCellsField.value	= Mathf.Clamp(XCellsField.value, 1, MaxCells);
			YCellsField.value	= Mathf.Clamp(YCellsField.value, 1, MaxCells);
			XCells				= XCellsField.value;
			YCells				= YCellsField.value;

			RebuildGridCells();
			ResizeGrid();
		}

		private void NumShapesChanged(ChangeEvent<int> evt)
		{
			NumShapesField.value	= Mathf.Clamp(NumShapesField.value, 1, int.MaxValue);
			NumShapes				= NumShapesField.value;

			selectedShapeIndex = Mathf.Clamp(selectedShapeIndex, 0, NumShapes);

			ValidateMinMaxShapeSize();
			GenerateShapeColors();
			UpdateGridCellColors();
			ResizeShapeBlocks();
		}

		private void MinMaxCellsChanged(ChangeEvent<int> evt)
		{
			ValidateMinMaxShapeSize();
		}

		private void ValidateMinMaxShapeSize()
		{
			int cellCount = CountCells();

			// Cell count is invalid, there are not enough empty cells to fit all shapes
			if (cellCount < NumShapes)
			{
				MinShapeSizeField.value = 1;
				MaxShapeSizeField.value = 1;

				return;
			}

			float cellsPerShape = (float)cellCount / (float)NumShapes;

			int maximumMinShapeSize = Mathf.Max(1, Mathf.FloorToInt(cellsPerShape));

			MinShapeSizeField.value		= Mathf.Clamp(MinShapeSizeField.value, 1, maximumMinShapeSize);
			MinShapeSize				= MinShapeSizeField.value;

			int minimumMaxShapeSize = Mathf.CeilToInt(cellsPerShape);
			int maximumMaxShapeSize = cellCount - MinShapeSize * (NumShapes - 1);

			MaxShapeSizeField.value		= Mathf.Clamp(MaxShapeSizeField.value, minimumMaxShapeSize, maximumMaxShapeSize);
			MaxShapeSize				= MaxShapeSizeField.value;
		}

		private int CountCells()
		{
			int xCells = XCells;
			int yCells = YCells;

			int count = 0;

			for (int y = 0; y < yCells; y++)
			{
				for (int x = 0; x < xCells; x++)
				{
					GridCell gridCell = grid[y][x];

					if (gridCell.shapeIndex >= EmptyCellShapeIndex)
					{
						count++;
					}
				}
			}

			return count;
		}

		private void UpdateOutputPaths()
		{
			if (OutputFolderField.value != null)
			{
				// Get the asset path of the object
				string assetPath = AssetDatabase.GetAssetPath(OutputFolderField.value);

				// Check if it is a directory
				bool isDir = ((System.IO.File.GetAttributes(assetPath) & System.IO.FileAttributes.Directory) == System.IO.FileAttributes.Directory);

				// Show the error message if it's not a directory
				rootVisualElement.Q("outputFolderErrorContainer").style.display = isDir ? DisplayStyle.None : DisplayStyle.Flex;

				if (isDir)
				{
					OutputFolderAssetPath = assetPath;
				}
			}
			else
			{
				OutputFolderAssetPath = null;
			}

			(rootVisualElement.Q("outputFolderPath") as TextElement).text = "Output path: " + GetOutputFileAssetPath(GetOutputFolderAssetPath(),string.Empty);
		}

		private void ClearShapes()
		{
			int xCells = XCells;
			int yCells = YCells;

			for (int y = 0; y < yCells; y++)
			{
				for (int x = 0; x < xCells; x++)
				{
					GridCell gridCell = grid[y][x];

					// Set any shape index that is above the empty shape index (IE all the colors) back to the empty shape index
					if (gridCell.shapeIndex > EmptyCellShapeIndex)
					{
						gridCell.shapeIndex = EmptyCellShapeIndex;
					}
				}
			}

			UpdateGridCellColors();
		}

		private void ResetGrid()
		{
			RebuildGridCells();
			ResizeGrid();
		}

		private void RebuildGridCells()
		{
			VisualElement gridContainer = GridContainerElement;

			gridContainer.Clear();
			grid.Clear();

			int					xCells		= XCells;
			int					yCells		= YCells;
			LevelData.LevelType	levelType	= LevelType;

			// Get the texture to use for the Image field
			Texture cellTexture = GetCellTexture(levelType);

			// Create a container that all the cells will be added to
			VisualElement gridCellContainer = new VisualElement();

			gridCellContainer.name = "gridCellContainer";

			gridCellContainer.RegisterCallback<MouseMoveEvent>(OnMouseMoved);
			gridCellContainer.RegisterCallback<MouseLeaveEvent>(OnMouseLeave);
			gridCellContainer.RegisterCallback<MouseDownEvent>(OnMouseDown);
			gridCellContainer.RegisterCallback<MouseUpEvent>(OnMouseUp);

			// Add the cell container to the grid container, the cell container will be centered because of grid-containers uss styling
			gridContainer.Add(gridCellContainer);

			for (int y = 0; y < yCells; y++)
			{
				grid.Add(new List<GridCell>());

				for (int x = 0; x < xCells; x++)
				{
					Image cellImage = new Image();

					cellImage.AddToClassList("grid-cell");

					cellImage.image = cellTexture;

					gridCellContainer.Add(cellImage);

					GridCell gridCell = new GridCell();

					gridCell.row		= (yCells -1) - y;
					gridCell.col		= x;
					gridCell.imageField	= cellImage;
					gridCell.shapeIndex = EmptyCellShapeIndex; // Default to the white color

					grid[y].Add(gridCell);
				}
			}

			ValidateMinMaxShapeSize();
		}

		private void ResizeGrid()
		{
			VisualElement gridContainer = GridContainerElement;

			int					xCells		= XCells;
			int					yCells		= YCells;
			LevelData.LevelType	levelType	= LevelType;

			// Get the max cell size for each cell
			float maxCellSize = GetMaxCellSize(gridContainer, levelType, xCells);

			// Get the cells width/height based on the type of level
			float cellWidth, cellHeight;
			GetCellSize(levelType, maxCellSize, out cellWidth, out cellHeight);

			// Set the width / height of the cell container to exactly what we need
			VisualElement gridCellContainer = gridContainer.Q("gridCellContainer");
			SetCellContainerSize(gridCellContainer, levelType, xCells, yCells, cellWidth, cellHeight);

			for (int y = 0; y < yCells; y++)
			{
				
				for (int x = 0; x < xCells; x++)
				{
					GridCell gridCell = grid[y][x];

					Image cellImage = gridCell.imageField;

					// Set the width/height of the cell
					if (levelType == LevelData.LevelType.Hexagon && RotateHexagon)
					{
						cellImage.style.width	= cellHeight;
						cellImage.style.height	= cellWidth;
					}
					else
					{
						cellImage.style.width	= cellWidth;
						cellImage.style.height	= cellHeight;
					}

					// Position the cell in the container
					PositionCell(cellImage, levelType, x, y, cellWidth, cellHeight);
				}
			}
		}

		/// <summary>
		/// Gets the max width a cell can be on the grid
		/// </summary>
		private float GetMaxCellSize(VisualElement gridContainer, LevelData.LevelType levelType, int xCells)
		{
			float maxCellWidth = 0;

			switch (levelType)
			{
				case LevelData.LevelType.Square:
				{
					maxCellWidth = gridContainer.localBound.width / xCells;
					break;
				}
				case LevelData.LevelType.Triangle:
				{
					maxCellWidth = gridContainer.localBound.width / ((xCells + 1f) / 2f);
					break;
				}
				case LevelData.LevelType.Hexagon:
				{
					if (RotateHexagon)
					{
						float partWidth = gridContainer.localBound.width / (xCells + 0.25f);

						maxCellWidth = (4f/3f) * partWidth;
					}
					else
					{
						maxCellWidth = gridContainer.localBound.width / (xCells + 0.5f);
					}
					break;
				}
			}

			return Mathf.Min(MaxCellSize, maxCellWidth);
		}

		/// <summary>
		/// Gets a cells with/height on the grid
		/// </summary>
		private void GetCellSize(LevelData.LevelType levelType, float maxCellSize, out float cellWidth, out float cellHeight)
		{
			cellWidth	= 1;
			cellHeight	= 1;

			switch (levelType)
			{
				case LevelData.LevelType.Square:
				{
					cellWidth	= maxCellSize;
					cellHeight	= maxCellSize;
					break;
				}
				case LevelData.LevelType.Triangle:
				{
					cellWidth	= maxCellSize;
					cellHeight	= maxCellSize * ((float)TriangleTexture.height / (float)TriangleTexture.width);
					break;
				}
				case LevelData.LevelType.Hexagon:
				{
					if (RotateHexagon)
					{
						cellWidth	= maxCellSize;
						cellHeight	= maxCellSize * (Mathf.Sqrt(3) / 2f);
					}
					else
					{
						cellWidth	= maxCellSize;
						cellHeight	= maxCellSize / (Mathf.Sqrt(3) / 2f);
					}
					break;
				}
			}
		}

		/// <summary>
		/// Gets the Texture associated with the level type
		/// </summary>
		private Texture GetCellTexture(LevelData.LevelType levelType)
		{
			switch (levelType)
			{
				case LevelData.LevelType.Square:
				{
					return SquareTexture;
				}
				case LevelData.LevelType.Triangle:
				{
					return TriangleTexture;
				}
				case LevelData.LevelType.Hexagon:
				{
					return HexagonTexture;
				}
			}

			return null;
		}

		/// <summary>
		/// Sets the grid cell containers widht/height to be exactly what it needs to be to fit all cells on the grid
		/// </summary>
		private void SetCellContainerSize(VisualElement gridCellContainer, LevelData.LevelType levelType, int xCells, int yCells, float cellWidth, float cellHeight)
		{
			switch (levelType)
			{
				case LevelData.LevelType.Square:
				{
					gridCellContainer.style.width	= xCells * cellWidth;
					gridCellContainer.style.height	= yCells * cellHeight;
					break;
				}
				case LevelData.LevelType.Triangle:
				{
					gridCellContainer.style.width	= Mathf.Ceil(xCells / 2f) * cellWidth + (xCells % 2 == 0 ? cellWidth / 2f : 0);
					gridCellContainer.style.height	= yCells * cellHeight;
					break;
				}
				case LevelData.LevelType.Hexagon:
				{
					if (RotateHexagon)
					{
						float width = 0;

						width += Mathf.Ceil(xCells / 2f) * cellWidth;
						width += Mathf.Floor(xCells / 2f) * (cellWidth / 2f);
						width += (xCells % 2 == 0 ? cellWidth / 4f : 0);

						gridCellContainer.style.width	= width;
						gridCellContainer.style.height	= yCells * cellHeight + cellHeight / 2f;
					}
					else
					{
						float height = 0;

						height += Mathf.Ceil(yCells / 2f) * cellHeight;
						height += Mathf.Floor(yCells / 2f) * (cellHeight / 2f);
						height += (yCells % 2 == 0 ? cellHeight / 4f : 0);

						gridCellContainer.style.width	= xCells * cellWidth;
						gridCellContainer.style.height	= height;
					}
					break;
				}
			}
		}

		/// <summary>
		/// Positions the cell on the grid based on the level type
		/// </summary>
		private void PositionCell(VisualElement cell, LevelData.LevelType levelType, int x, int y, float cellWidth, float cellHeight)
		{
			switch (levelType)
			{
				case LevelData.LevelType.Square:
				{
					float xPos = x * cellWidth;
					float yPos = y * cellHeight;

					cell.transform.position = new Vector3(xPos, yPos, 0f);
					break;
				}
				case LevelData.LevelType.Triangle:
				{
					bool upsideDown = (x + y) % 2 != 0;

					float xPos = x * (cellWidth / 2f);
					float yPos = y * cellHeight;

					if (upsideDown)
					{
						// If it's an upside down triangle then rotate it by 180 degrees
						Quaternion rotation = new Quaternion();
						rotation.eulerAngles = new Vector3(0, 0, 180);
						cell.transform.rotation = rotation;

						// Need to re-position because the pivot point is in the top left corner
						xPos += cellWidth;
						yPos += cellHeight;
					}

					cell.transform.position = new Vector3(xPos, yPos, 0f);
					break;
				}
				case LevelData.LevelType.Hexagon:
				{
					if (RotateHexagon)
					{
						float xPos = x * ((3f / 4f) * cellWidth);
						float yPos = y * cellHeight;
						
						if (x % 2 != 0)
						{
							yPos += cellHeight / 2f;
						}

						// // Make sure the rotation is set to 90
						Quaternion rotation = new Quaternion();
						rotation.eulerAngles = new Vector3(0, 0, 90);
						cell.transform.rotation = rotation;

						xPos += cellWidth;

						cell.transform.position = new Vector3(xPos, yPos, 0f);
					}
					else
					{
						float xPos = x * cellWidth - cellWidth / 4f;
						float yPos = y * ((3f / 4f) * cellHeight);
						
						if (y % 2 != 0)
						{
							xPos += cellWidth / 2f;
						}

						// // Make sure the rotation is set to 0
						Quaternion rotation = new Quaternion();
						rotation.eulerAngles = new Vector3(0, 0, 0);
						cell.transform.rotation = rotation;

						cell.transform.position = new Vector3(xPos, yPos, 0f);
					}
					break;
				}
			}
		}

		/// <summary>
		/// Invoked when the mouse hovers over the grid cell container
		/// </summary>
		private void OnMouseMoved(MouseMoveEvent evt)
		{
            if (colorToggle.value)
            {
                return;
            }

			GridCell gridCell = GetClosestCell(evt.localMousePosition);

			// Only update if the hover target has changed
			if (hoveredGridCell != gridCell)
			{
				if (hoveredGridCell != null)
				{
					SetHover(hoveredGridCell, false);
				}

				SetHover(gridCell, true);

				hoveredGridCell = gridCell;
			}

			if (isDragging)
			{
				SetCellToSelectedShape(gridCell);
			}
		}

		/// <summary>
		/// Invoked when the mouse moves out of the grid cell container
		/// </summary>
		private void OnMouseLeave(MouseLeaveEvent evt)
		{
            if (colorToggle.value)
            {
                return;
            }

            if (hoveredGridCell != null)
			{
				SetHover(hoveredGridCell, false);
			}

			hoveredGridCell	= null;
			isDragging		= false;
		}

		/// <summary>
		/// Invoked when the mouse clicks on the grid cell container
		/// </summary>
		private void OnMouseDown(MouseDownEvent evt)
		{
			SetCellToSelectedShape(GetClosestCell(evt.localMousePosition));

			isDragging = true;
		}

		/// <summary>
		/// Invoked when the mouse clicks on the grid cell container
		/// </summary>
		private void OnMouseUp(MouseUpEvent evt)
		{
			isDragging = false;
		}


        private void ColorToggleChanged(ChangeEvent<bool> evt)
        {

            int xCells = XCells;
            int yCells = YCells;

            for (int y = 0; y < yCells; y++)
            {
                for (int x = 0; x < xCells; x++)
                {
                    GridCell gridCell = grid[y][x];

                    if (colorToggle.value)
                    {
                        if (gridCell.acheveColor != Color.clear)
                        {
                            gridCell.imageField.tintColor = gridCell.acheveColor;
                        }
                       
                    }
                    else
                    {
                        gridCell.imageField.tintColor = gridCell.shapeColor;
                    }
                }
            }
           
        }

        /// <summary>
        /// Sets the cell to the currently selected shape
        /// 设置网格所在shape
        /// </summary>
        private void SetCellToSelectedShape(GridCell gridCell)
		{
            //涂颜色模式
            if (colorToggle.value)
            {
                Color colorPicker = colorField.value;
                Debug.Log("colorPicker:" + ColorUtility.ToHtmlStringRGBA(colorPicker)+"-->"+ gridCell.row+":"+ gridCell.col);
                if (gridCell.acheveColor != colorPicker)
                {
                    gridCell.acheveColor = colorPicker;
                    gridCell.imageField.tintColor = colorPicker;
                }
                else
                {
                    gridCell.acheveColor = Color.clear;
                    gridCell.imageField.tintColor = gridCell.shapeColor;
                }
                
            }
            else
            {
                if (gridCell.shapeIndex != selectedShapeIndex)
                {

                    gridCell.imageField.tintColor = GetShapeColor(selectedShapeIndex);

                    gridCell.shapeColor = gridCell.imageField.tintColor;

                    gridCell.shapeIndex = selectedShapeIndex;

                    ValidateMinMaxShapeSize();
                }
            }
		}

		/// <summary>
		/// Gets the closest grid cell to the given mouse position
		/// </summary>
		private GridCell GetClosestCell(Vector2 mousePosition)
		{
			int					xCells		= XCells;
			int					yCells		= YCells;
			LevelData.LevelType	levelType	= LevelType;

			int		targetX		= 0;
			int		targetY		= 0;
			float	minDistance	= float.MaxValue;

			for (int y = 0; y < yCells; y++)
			{
				for (int x = 0; x < xCells; x++)
				{
					Image	gridCell	= grid[y][x].imageField;
					Vector2	cellMiddle	= gridCell.transform.position;

					cellMiddle.x += gridCell.style.width.value.value / 2f;
					cellMiddle.y += gridCell.style.height.value.value / 2f;

					if (levelType == LevelData.LevelType.Triangle && (x + y) % 2 == 1)
					{
						cellMiddle.x -= gridCell.style.width.value.value ;
						cellMiddle.y -= gridCell.style.height.value.value ;
					}
					else if (levelType == LevelData.LevelType.Hexagon && RotateHexagon)
					{
						cellMiddle.x -= gridCell.style.width.value.value;
					}

					float distance = Vector2.Distance(cellMiddle, mousePosition);

					if (distance < minDistance)
					{
						targetX		= x;
						targetY		= y;
						minDistance = distance;
					}
				}
			}

			return grid[targetY][targetX];
		}

		private void SetHover(GridCell gridCell, bool isHovered)
		{
			if (isHovered)
			{
				Color hoverColor = GetShapeColor(selectedShapeIndex);
				
				if (selectedShapeIndex > 0)
				{
					hoverColor.a = 0.7f;
				}

				gridCell.imageField.tintColor = hoverColor;
			}
			else
			{
				gridCell.imageField.tintColor = GetShapeColor(gridCell.shapeIndex);
			}
		}

		private Color GetShapeColor(int shapeIndex)
		{
			Color color = Color.white;

			
			color = shapeColors[shapeIndex];
			

			return color;
		}

		/// <summary>
		/// Updates the number of color shapes used when creating manual levels
		/// </summary>
		private void ResizeShapeBlocks()
		{
			// Get the shpaes container and clear it
			VisualElement shapesContainer = ShapesContainerElement;

			shapesContainer.Clear();
			shapeBlocks.Clear();

			// Get the number of rows / cols we need to display all the shapes
			int numColors	= shapeColors.Count;
			int numCols		= Mathf.FloorToInt((shapesContainer.localBound.width + ShapeBlockSpacing) / (ShapeBlockSize + ShapeBlockSpacing));
			int numRows		= Mathf.CeilToInt((float)numColors / (float)numCols);

			int index = 0;

			for (int r = 0; r < numRows && index < numColors; r++)
			{
				// Add a row to the the shapes container
				VisualElement shapesRow = new VisualElement();

				shapesRow.AddToClassList("shapes-container-row");

				if (r > 0)
				{
					shapesRow.style.marginTop = ShapeBlockSpacing;
				}

				shapesContainer.Add(shapesRow);

				for (int c = 0; c < numCols && index < numColors; c++, index++)
				{
					VisualElement shapeBlock = new VisualElement();

					shapeBlock.style.width				= ShapeBlockSize;
					shapeBlock.style.height				= ShapeBlockSize;
					shapeBlock.style.backgroundColor	= GetShapeColor(index);

					// Give the block a border of size 2, we will set the border color to clear/white if the block is selected or not
					shapeBlock.style.borderBottomWidth	= 2f;
					shapeBlock.style.borderTopWidth		= 2f;
					shapeBlock.style.borderLeftWidth	= 2f;
					shapeBlock.style.borderRightWidth	= 2f;

					if (c > 0)
					{
						shapeBlock.style.marginLeft = ShapeBlockSpacing;
					}

					shapeBlock.RegisterCallback<MouseDownEvent>(OnShapeClicked);

					// Add the element to the row
					shapesRow.Add(shapeBlock);

					// Add the element to the list of active shape blocks
					shapeBlocks.Add(shapeBlock);
				}
			}

			if (shapeBlocks.Count > 0)
			{
				SetShapeBlockSelected(shapeBlocks[selectedShapeIndex], true);
			}
		}

		/// <summary>
		/// Creates a unique color for each shape
		/// </summary>
		private void GenerateShapeColors()
		{

            List<Color> fixColors = new List<Color>();

            //默认主题
            Color BACK_GROUND_COLOR = new Color(0.96f, 0.96f, 0.96f, 1.00f); //#f5f5f5;  //背景颜色
            Color NORMAL_COLOR = new Color(0.96f, 0.96f, 0.96f, 1.00f); //#f5f5f5;  //普通格子背景颜色                                                                     // public static Color CANDIDATE_COLOR = new Color(0.54f, 0.54f, 0.54f, 1.00f); //#8a8a8a;  //��ѡ����ɫ, ��ɫ
            Color CANDIDATE_COLOR = new Color(0f, 0f, 0f, 1.00f); //#8a8a8a;  //候选数颜色
            Color CELL_VALUE_COLOR = new Color(0.28f, 0.42f, 0.72f, 1f);//#476ab7;easybrain蓝 填入数字的颜色
            Color SELECT_CELL_COLOR = new Color(0.74f, 0.86f, 1.00f, 1f);//#bcdcff; easybrain选中格子颜色；（高亮）
            Color SELECTED_BUDDY_COLOR = new Color(0.89f, 0.91f, 0.93f, 1f); //#e2e7ed; easybrain可见格子区颜色；
            Color SAME_VALUE_GRAY = new Color(0.49f, 0.66f, 0.85f, 1.00f); //#7ea8da;  easybrain相同值灰，相同数字格子的颜色
            Color GRID_4_BGCOLOR = new Color(0.68f, 0.78f, 0.90f, 1.00f); //#ADC7E5; 4格背景色 (选中--变暗)


            //绿色主题
            Color GREEN_BACK_GROUND_COLOR = new Color(0.97f, 0.98f, 0.95f, 1.00f); //#f7f9f3;  //背景颜色
            Color GREEN_NORMAL_COLOR = new Color(0.97f, 0.98f, 0.95f, 1.00f); //#f7f9f3;  //普通格子背景颜色                                                                        
            Color GREEN_CELL_VALUE_COLOR = new Color(0.13f, 0.48f, 0.48f, 1.00f);//#217A7A; 开心数独——绿 填入数字的颜色
            Color GREEN_SELECT_CELL_COLOR = new Color(0.81f, 0.86f, 0.63f, 1.00f);//#cfdca1;开心数独——选中格子颜色；
            Color GREEN_SELECTED_BUDDY_COLOR = new Color(0.94f, 0.95f, 0.87f, 1f); //#f0f3dd;开心数独——可见格子区颜色；
            Color GREEN_SAME_VALUE_GRAY = new Color(0.42f, 0.57f, 0.56f, 1.00f); //#6a928f;  开心数独——相同值灰，相同数字格子的颜色
            Color GREEN_GRID_4_BGCOLOR = new Color(0.85f, 0.90f, 0.77f, 1.00f); //#DAE5C5FF;CFDFB4 4格背景色


            //棕色主题（非黑色）

            Color BROWN_BACK_GROUND_COLOR = new Color(0.94f, 0.92f, 0.86f, 1.00f); //#f0eadc;  /背景颜色（两个一样）
            Color BROWN_NORMAL_COLOR = new Color(0.94f, 0.92f, 0.86f, 1.00f); //#f0eadc;  //普通格子背景颜色
            Color BROWN_CELL_VALUE_COLOR = new Color(0.39f, 0.25f, 0.24f, 1.00f);//#63403D; 开心数独 填入数字的颜色 (99,64,61)
            Color BROWN_SELECT_CELL_COLOR = new Color(0.58f, 0.40f, 0.34f, 1.00f);//#946656; 开心数独——选中格子颜色；
            Color BROWN_SELECTED_BUDDY_COLOR = new Color(0.90f, 0.86f, 0.78f, 1f); //#e5dcc6; 开心数独——可见格子区颜色；
            Color BROWN_SAME_VALUE_GRAY = new Color(0.46f, 0.27f, 0.22f, 1.00f); //#764538; 开心数独——相同值灰，相同数字格子的颜色
            Color BROWN_GRID_4_BGCOLOR = new Color(0.71f, 0.58f, 0.53f, 1.00f); //#B69388; 4格背景色

            Color PINK_FIX_VALUE_COLOR = new Color(0f, 0f, 0f, 1.00f);//固定数的颜色
            Color PINK_CANDIDATE_COLOR = new Color(0f, 0f, 0f, 1.00f); //#8a8a8a;  //候选数颜色（固定）
            Color PINK_BACK_GROUND_COLOR = new Color(1.00f, 0.93f, 0.96f, 1.00f); //#ffedf5;  //背景颜色（两个一样-最浅）
            Color PINK_NORMAL_COLOR = new Color(1.00f, 0.93f, 0.96f, 1.00f); //#ffedf5;  //普通格子背景颜色两个一样-最浅）
            Color PINK_GRID_4_BGCOLOR = new Color(0.95f, 0.64f, 0.78f, 1.00f); //#fdcbe6;4格背景色（次浅)
            Color PINK_SELECTED_BUDDY_COLOR = new Color(0.99f, 0.80f, 0.90f, 1f);   //#f2a4c6; 开心数独——可见格子区颜色；（浅亮）
            Color PINK_SELECT_CELL_COLOR = new Color(1.00f, 0.63f, 0.69f, 1.00f);//#ffa0b1; 开心数独——选中格子颜色；（深亮）
            Color PINK_SAME_VALUE_GRAY = new Color(0.90f, 0.70f, 0.61f, 1.00f); //#e6b39b; 开心数独——相同值灰，相同数字格子的颜色(最深)
            Color PINK_CELL_VALUE_COLOR = new Color(0.65f, 0.43f, 0.45f, 1.00f);//#7c686a; 开心数独 填入数字的颜色(次深)


            Color ORANGE_COLOR = new Color(0.99f, 0.66f, 0.32f, 1.00f);
            Color GOOGLE_RED = new Color(0.87f, 0.31f, 0.26f, 1.00f);
            Color PURPLE = new Color(0.53f, 0.37f, 0.77f, 1.00f);
            Color PURE_YELLOW = new Color(1f, 1f,0f, 1.00f);


            fixColors.Add(Color.white);
            fixColors.Add(CELL_VALUE_COLOR);
            fixColors.Add(SELECT_CELL_COLOR);
        
            fixColors.Add(GREEN_CELL_VALUE_COLOR);
            fixColors.Add(GREEN_SELECT_CELL_COLOR);

            fixColors.Add(BROWN_CELL_VALUE_COLOR);
            fixColors.Add(BROWN_SELECT_CELL_COLOR);

            fixColors.Add(ORANGE_COLOR);
            fixColors.Add(PINK_SELECT_CELL_COLOR);

            fixColors.Add(GOOGLE_RED);
            fixColors.Add(PURPLE);
            fixColors.Add(PURE_YELLOW);

            fixColors.Add(Color.blue);
            fixColors.Add(Color.green);
            fixColors.Add(Color.cyan);
            fixColors.Add(Color.red);


            shapeColors.Clear();

			int		numShapes	= NumShapes;
			float	step		= 1f / numShapes;

			for (int i = 0; i < numShapes + EmptyCellShapeIndex + 1; i++)
			{
				
					//shapeColors.Add(Color.HSVToRGB((i - (EmptyCellShapeIndex + 1)) * step, 2, 2));
                    shapeColors.Add(fixColors[i]);
			}
		}

		/// <summary>
		/// Re-sets the tintColor on all GridCell imageFiles
		/// </summary>
		private void UpdateGridCellColors()
		{
			int xCells = XCells;
			int yCells = YCells;

			for (int y = 0; y < yCells; y++)
			{

				for (int x = 0; x < xCells; x++)
				{
					GridCell	gridCell	= grid[y][x];
					Image		cellImage	= gridCell.imageField;

					if (gridCell.shapeIndex >= shapeColors.Count)
					{
						// Set it back to white
						gridCell.shapeIndex = EmptyCellShapeIndex;
					}

					cellImage.tintColor = GetShapeColor(gridCell.shapeIndex);
				}
			}
		}

        /// <summary>
        ///选择色块(shape)
        /// </summary>
        /// <param name="evt"></param>
		private void OnShapeClicked(MouseDownEvent evt)
		{
			VisualElement shapeBlock = evt.target as VisualElement;

			int selectedIndex = shapeBlocks.IndexOf(shapeBlock);

			if (selectedIndex != selectedShapeIndex)
			{
				SetShapeBlockSelected(shapeBlocks[selectedShapeIndex], false);
				SetShapeBlockSelected(shapeBlock, true);

				selectedShapeIndex = selectedIndex;
			}
		}

		private void SetShapeBlockSelected(VisualElement shapeBlock, bool isSelected)
		{
			shapeBlock.style.borderBottomColor	= isSelected ? Color.black : Color.clear;
			shapeBlock.style.borderTopColor		= isSelected ? Color.black : Color.clear;
			shapeBlock.style.borderLeftColor	= isSelected ? Color.black : Color.clear;
			shapeBlock.style.borderRightColor	= isSelected ? Color.black : Color.clear;
		}

        List<List<SGridCell>> newGridCell;


        [MenuItem("Tools/Bizzy Bee Games/ExportKlotskiSolution")]
        public static void exportKlotskiSolution()
        {
           // ExportClicked2();
        }

        private static Dictionary<int, int> blockValueBlockIndexDic;


		private  void ExportClicked2()
		{
			blockValueBlockIndexDic = new Dictionary<int, int>();
			klotskiSolution findAnswer = new klotskiSolution();
		

			string filename = Filename.Trim();

			BindingFlags flag = BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public;
			FieldInfo f_key = typeof(klotskiBoardFayaa19).GetField("obj"+ filename, flag);
			object objtarget = f_key.GetValue(null);

			//object objtarget = klotskiBoardFayaa.obj2;

			string boardstr = objtarget.GetType().GetProperty("board").GetValue(objtarget, null).ToString();
			string blockName= objtarget.GetType().GetProperty("name").GetValue(objtarget, null).ToString();
			int mini = Convert.ToInt32(objtarget.GetType().GetProperty("mini").GetValue(objtarget, null));
            string level = Convert.ToInt32(objtarget.GetType().GetProperty("level").GetValue(objtarget, null)).ToString();
            string url_name = objtarget.GetType().GetProperty("url_name").GetValue(objtarget, null).ToString();



            findAnswer.init(boardstr, klotskiSolution.MOVE_MODE.RIGHT_ANGLE_TURN);
			FindResult findResult = findAnswer.find();
			Debug.Log("findResult.elapsedTime:" + findResult.elapsedTime);
			Debug.Log("findResult.exploreCount:" + findResult.exploreCount);
			Debug.Log("findResult.boardList.Count:" + findResult.boardList.Count);

			int maxMove = findResult.boardList.Count - 1;

			char[] startBoard = klotskiPuzzle.key2Board(findResult.boardList[0]);
			Debug.Log("startBoard:\n" + showBoard(startBoard));

			createInitBlockinfo(startBoard, blockName,mini, filename, url_name);

		}

		private static void ExportClicked2backup()
        {
            blockValueBlockIndexDic = new Dictionary<int, int>();
            klotskiSolution findAnswer = new klotskiSolution();
            //"橫刀立馬";
            string boardstr =
                "HAAI" +
                "HAAI" +
                "JBBK" +
                "JNOK" +
                "P@@Q";

            findAnswer.init(boardstr, klotskiSolution.MOVE_MODE.RIGHT_ANGLE_TURN);
            FindResult findResult=findAnswer.find();
            Debug.Log("findResult.elapsedTime:" + findResult.elapsedTime);
            Debug.Log("findResult.exploreCount:" + findResult.exploreCount);
            Debug.Log("findResult.boardList.Count:" + findResult.boardList.Count);

            int maxMove = findResult.boardList.Count-1;

            char[] startBoard = klotskiPuzzle.key2Board(findResult.boardList[0]);
            Debug.Log("startBoard:\n" + showBoard(startBoard));

            createInitBlockinfo(startBoard,"",0, "", "");

            List<BlockHintInfo> blockHintInfors = new List<BlockHintInfo>();

            for (var i = 1; i <= maxMove; i++)
            {
                SolveMoveInfo moveInfo = klotskiPuzzle.getMoveInfo(klotskiPuzzle.key2Board(findResult.boardList[i - 1]), klotskiPuzzle.key2Board(findResult.boardList[i]));

                int srtPos = moveInfo.srcX + moveInfo.srcY * klotskiShare.G_BOARD_X;
                int destPos= moveInfo.dstX + moveInfo.dstY * klotskiShare.G_BOARD_X;

                char srcBlock = startBoard[srtPos];
                char destBlock=startBoard[destPos];


                Debug.Log("begin-->" +i+ ":\n" + showBoard(startBoard));

                Debug.Log("srcBlock:" + srcBlock + " descBlock:" + destBlock+ "-->moveInfo:"+ moveInfo);


                int blockvalue = (int)(srcBlock - klotskiShare.G_VOID_CHAR);
                int blockStyle = klotskiShare.gBlockBelongTo[blockvalue];//代表的形状


                BlockHintInfo blockHintInfor = new BlockHintInfo();
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
                        startBoard[srtPos+1] = '@';
                        startBoard[destPos] = srcBlock;
                        startBoard[destPos+1] = srcBlock;
                        break;
                    case 3: // 1X2 block
                        startBoard[srtPos] = '@';
                        startBoard[srtPos+ klotskiShare.G_BOARD_X] = '@';
                        startBoard[destPos] = srcBlock;
                        startBoard[destPos + klotskiShare.G_BOARD_X] = srcBlock;
                        break;
                    case 4: // 2X2 block
                        startBoard[srtPos] = '@';
                        startBoard[srtPos + 1] = '@';
                        startBoard[srtPos + klotskiShare.G_BOARD_X] = '@';
                        startBoard[srtPos +1+ klotskiShare.G_BOARD_X] = '@';

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

            string jsonHitblocks = JsonHelper.ToJson<BlockHintInfo>(blockHintInfors.ToArray());
            Debug.Log("jsonHitblocks:"+jsonHitblocks);

            string outputFolderAssetPath = "Assets/Blocks/outpueLevelFiles";
            string outputFileAssetPath = GetOutputFileAssetPath(outputFolderAssetPath, "Level_Hint");
            string outputFileFullPath = Application.dataPath + outputFileAssetPath.Remove(0, "Assets".Length);

            System.IO.File.WriteAllText(outputFileFullPath, jsonHitblocks);

        }

        private  static void createInitBlockinfo(char[] startBoard,string blockname,int mini,string level,string url_name)
        {

            List<BlockInfor> blockInfors = new List<BlockInfor>();
            Dictionary<int,bool> checkedDone= new Dictionary<int, bool>();

            BlockInfor speicalblockInfo = null;
            int blockStyle = 0;
            //从上到下，从左到右
            for (var i = 0; i < 20; i++)
            {
                char srcBlock = startBoard[i];
                int blockvalue = (int)(srcBlock - klotskiShare.G_VOID_CHAR);

                if (checkedDone.ContainsKey(blockvalue))
                {
                    continue;//已经记录该blockvalue
                }

                blockStyle = klotskiShare.gBlockBelongTo[blockvalue];//代表的形状

                int srcPosX = i % klotskiShare.G_BOARD_X;
                int srcPosY = (i - srcPosX) / klotskiShare.G_BOARD_X;
                srcPosY = 4 - srcPosY;   //y坐标反转（从上到下----》从下到上）

                BlockInfor blockInfo=null;

               
                switch (blockStyle)
                {
                    case 0: //empty block
                        break;
                    case 1: // 1X1 block
                        blockInfo = new BlockInfor();
                        blockInfo.x = srcPosX;
                        blockInfo.y = srcPosY;
                        blockInfo.width = 1;
                        blockInfo.height = 1;
                        break;
                    case 2: // 2X1 block
                        blockInfo = new BlockInfor();
                        blockInfo.x = srcPosX;
                        blockInfo.y = srcPosY;
                        blockInfo.width = 2;
                        blockInfo.height = 1;
                        break;
                    case 3: // 1X2 block
                        blockInfo = new BlockInfor();
                        blockInfo.x = srcPosX;
                        blockInfo.y = srcPosY-1;
                        blockInfo.width = 1;
                        blockInfo.height = 2;
                        break;
                    case 4: // 2X2 block
                        speicalblockInfo = new BlockInfor();
                        speicalblockInfo.x = srcPosX;
                        speicalblockInfo.y = srcPosY-1;
                        speicalblockInfo.width = 2;
                        speicalblockInfo.height = 2;
                        break;
                    default:
                        Debug.LogError("key2Board(): design error !");
                        break;
                }

                checkedDone[blockvalue] = true;    //已经记录

                if (blockInfo != null)
                {
                    blockInfors.Add(blockInfo);
                    blockInfo.charName = srcBlock;
                    blockInfo.blockStyle = blockStyle;
                    blockValueBlockIndexDic[blockvalue] = blockInfors.Count;
                }
                
            }

            speicalblockInfo.charName = 'A';   //曹操
            speicalblockInfo.blockStyle = 4;   //曹操
            speicalblockInfo.blockname = blockname;
            speicalblockInfo.mini = mini;


            blockInfors.Insert(0, speicalblockInfo);
            blockValueBlockIndexDic[2] = 0;  //曹操块blockvalue为2；


            string jsonblocks = JsonHelper.ToJson<BlockInfor>(blockInfors.ToArray());
            Debug.Log(jsonblocks);

            string outputFolderAssetPath = "Assets/Blocks/outpueLevelFiles";// GetOutputFolderAssetPath();
            string outputFileAssetPath = GetOutputFileAssetPath(outputFolderAssetPath, url_name+"_"+level + "_" + blockname +"_"+mini);
            string outputFileFullPath = Application.dataPath + outputFileAssetPath.Remove(0, "Assets".Length);

            System.IO.File.WriteAllText(outputFileFullPath, jsonblocks);


            //******************************solution*********************************************//

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


        private void ExportClicked()
		{
            //二维数组

            Dictionary<int, List<SGridCell>> unblockMebGrids= new Dictionary<int, List<SGridCell>>();
            RED_INDEX = 0;


            newGridCell = new List<List<SGridCell>>();
            for (int yy = 0; yy < YCells; yy++)
            {
                newGridCell.Add(new List<SGridCell>());
                for (int xx = 0; xx < XCells; xx++)
                {
                    newGridCell[yy].Add(new SGridCell());
                }
            }


            // Convert the grid to the proper format for exporting
            for (int y = 0; y < YCells; y++)
			{
               
                for (int x = 0; x < XCells; x++)
				{
					GridCell gridCell = grid[y][x];

                    SGridCell sgridCell = newGridCell[gridCell.row][gridCell.col];

                    sgridCell.row= (byte)gridCell.row;
                    sgridCell.col = (byte)gridCell.col;
                    sgridCell.shapeIndex = (byte)gridCell.shapeIndex;


                    RED_INDEX = Mathf.Max(RED_INDEX, sgridCell.shapeIndex);

                    if (sgridCell.shapeIndex > 0)
                    {
                        if (unblockMebGrids.ContainsKey(sgridCell.shapeIndex) == false)
                        {
                            unblockMebGrids[sgridCell.shapeIndex] = new List<SGridCell>();
                        }
                        unblockMebGrids[sgridCell.shapeIndex].Add(sgridCell);
                    }
                  
                }
			}


		////	Export(gridCellValues);

            parseUnblockMe(unblockMebGrids);


         ////   AssetDatabase.Refresh();
		}



        private void parseUnblockMe(Dictionary<int, List<SGridCell>> unblockMebGrids)
        {

            List<BlockInforPre> blockInforsPre = new List<BlockInforPre>();
            BlockInforPre blockInfoPre = null;

            //  JsonHelper
            foreach (KeyValuePair<int, List<SGridCell>> kv in unblockMebGrids)
            {

                //BlockInfor
                blockInfoPre = new BlockInforPre();
                blockInfoPre.index = kv.Key;

                List<SGridCell> gridcells = kv.Value;

                if (gridcells.Count<1)
                {
                    Debug.LogError("Block is less than 1");
                    return;
                }

                if (gridcells.Count > 4)
                {
                    Debug.LogError("Block is greater than 4");
                    return;
                }

                if (gridcells.Count == 2)
                {
                    if (gridcells[0].row == gridcells[1].row)  //行
                    {
                        blockInfoPre.width = 2;
                        blockInfoPre.height = 1;

                        blockInfoPre.y = gridcells[0].row;
                        blockInfoPre.x = Mathf.Min(gridcells[0].col, gridcells[1].col);
                    }
                    else if (gridcells[0].col == gridcells[1].col)  //列
                    {
                        blockInfoPre.width = 1;
                        blockInfoPre.height = 2;

                        blockInfoPre.x = gridcells[0].col;
                        blockInfoPre.y = Mathf.Min(gridcells[0].row, gridcells[1].row);

                    }
                    else
                    {
                        Debug.LogError("2 blocks cross");
                        return;
                    }
                }
                else if (gridcells.Count == 4)
                {

                    blockInfoPre.width = 2;
                    blockInfoPre.height = 2;

                    int[] rows = {gridcells[0].row, gridcells[1].row, gridcells[2].row, gridcells[3].row};
                    blockInfoPre.y = Mathf.Min(rows);

                    int[] cols = { gridcells[0].col, gridcells[1].col, gridcells[2].col, gridcells[3].col};
                    blockInfoPre.x = Mathf.Min(cols);

                }
                else if (gridcells.Count == 1)
                {
                    blockInfoPre.width = 1;
                    blockInfoPre.height = 1;
                    blockInfoPre.x = gridcells[0].col;
                    blockInfoPre.y = gridcells[0].row;
                }

                blockInforsPre.Add(blockInfoPre);

            }

            blockInforsPre.Sort((x, y) => (x.index-y.index));


            //输出布局
            List<BlockInfor> blockInfors = new List<BlockInfor>();
            BlockInfor blockInfo = null;

            foreach (BlockInforPre bInfoPre in blockInforsPre)
            {
                if (bInfoPre.index == RED_INDEX)   //?红色
                {
                    blockInfo = new BlockInfor();
                    blockInfo.x = bInfoPre.x;
                    blockInfo.y = bInfoPre.y;
                    blockInfo.width = bInfoPre.width;
                    blockInfo.height = bInfoPre.height;
                    blockInfo.blockname= InPutFilename;
                    blockInfors.Add(blockInfo);

                    break;
                }
            }

            foreach (BlockInforPre bInfoPre in blockInforsPre)
            {
                if (bInfoPre.index != RED_INDEX)  //非红色
                {
                    blockInfo = new BlockInfor();
                    blockInfo.x = bInfoPre.x;
                    blockInfo.y = bInfoPre.y;
                    blockInfo.width = bInfoPre.width;
                    blockInfo.height = bInfoPre.height;

                    blockInfors.Add(blockInfo);
                }
            }


            //固定
            blockInfo = new BlockInfor();
            blockInfo.x = 6;
            blockInfo.y = 3;
            blockInfo.width = 0;
            blockInfo.height = 0;
    

            blockInfors.Add(blockInfo);

            string jsonblocks= JsonHelper.ToJson<BlockInfor>(blockInfors.ToArray());
            Debug.Log(jsonblocks);

            string outputFolderAssetPath = GetOutputFolderAssetPath();
            string outputFileAssetPath = GetOutputFileAssetPath(outputFolderAssetPath, "Level");
            string outputFileFullPath = Application.dataPath + outputFileAssetPath.Remove(0, "Assets".Length);

            System.IO.File.WriteAllText(outputFileFullPath, jsonblocks);

         
            ///////////////////////////////////////////////////////
            BlockSolution blocksolution = new BlockSolution();
            State oo = new State();
            oo.board = newGridCell;
            oo.planks = unblockMebGrids;


            List<State> resultList = blocksolution.startSearch(oo, RED_INDEX);
            if (resultList != null)
            {
                List<BlockHintInfo> blockHintInfors = new List<BlockHintInfo>();
                BlockHintInfo blockHintInfo = null;
                for (int resultIndex = 1; resultIndex < resultList.Count; resultIndex++)
                {
                    State resultState = resultList[resultIndex];

                    blockHintInfo = new BlockHintInfo();
                    blockHintInfo.blockIndex = resultState.action.No;

                    if (blockHintInfo.blockIndex == RED_INDEX)
                    {
                        blockHintInfo.blockIndex = 0;   //红色块特殊处理,blockIndex为0
                    }

                    blockHintInfo.numberMoveRow = resultState.action.moveRow;
                    blockHintInfo.numberMoveCol = resultState.action.moveCol;
                    blockHintInfors.Add(blockHintInfo);
                }

                string jsonHitblocks = JsonHelper.ToJson<BlockHintInfo>(blockHintInfors.ToArray());
                Debug.Log(jsonHitblocks);

                outputFileAssetPath = GetOutputFileAssetPath(outputFolderAssetPath, "Level_Hint");
                outputFileFullPath = Application.dataPath + outputFileAssetPath.Remove(0, "Assets".Length);

                System.IO.File.WriteAllText(outputFileFullPath, jsonHitblocks);
            }
            else
            {
                Debug.Log("Hint is null");
            }
            
        }


        private LevelData inputLevelData;
        private void ImportClicked()
        {
            string outputFolderAssetPath = GetInputFolderAssetPath();
            string outputFileAssetPath = GetInputFileAssetPath(outputFolderAssetPath);
            string outputFileFullPath = Application.dataPath + outputFileAssetPath.Remove(0, "Assets".Length);

            string inputText=System.IO.File.ReadAllText(outputFileFullPath);

            Debug.Log("inputText:"+ inputText);
            TextAsset textAsset = new TextAsset(inputText);


            inputLevelData = new LevelData(textAsset, "input", 1);
            inputLevelData.ParseLevelFile();

            ImpuptResetGrid();

        }

        private void ImpuptResetGrid()
        {
            //设置列数和行数
            XCellsField.value = inputLevelData.XCells;
            YCellsField.value = inputLevelData.YCells;

            XCells = XCellsField.value;
            YCells = YCellsField.value;

            //设置类型
            // There is a bug with binding enums, need to manually set it when it changes
            LevelTypeField.value = inputLevelData.Type;
            LevelType = (LevelData.LevelType)LevelTypeField.value;


            // If Hexagon level type was selected show the Rotate Hexagon field
            RotateHexagonField.style.display = (LevelType == LevelData.LevelType.Hexagon) ? DisplayStyle.Flex : DisplayStyle.None;


            InputRebuildGridCells();
            ResizeGrid();
        }

        private void InputRebuildGridCells()
        {
            //解析inputText
            string[] itemsOrign = inputLevelData.LevelFileText.Split('\n');

            string[] items= itemsOrign[0].Split(',');

            string[] colorItems = null;
            if (itemsOrign.Length == 2)
            {
                colorItems=itemsOrign[1].Split(',');
            }


            int itemIndex = 0;
            // First item is the timestamp for when the level file was generated
            string timestamp = items[itemIndex++];
            // Next item is the level type
            LevelType levelType = (LevelType)int.Parse(items[itemIndex++]);

            // If the level type is Hexagon the the next value will determine the orentation of the hexagons 
            HexagonOrientation hexagonOrientation = (bool)bool.Parse(items[itemIndex++]) ? HexagonOrientation.Horizontal : HexagonOrientation.Vertical;

            // Next two items are the yCells, and xCells
            int yCells = int.Parse(items[itemIndex++]);
            int xCells = int.Parse(items[itemIndex++]);

            VisualElement gridContainer = GridContainerElement;

            gridContainer.Clear();
            grid.Clear();


            // Get the texture to use for the Image field
            Texture cellTexture = GetCellTexture(levelType);

            // Create a container that all the cells will be added to
            VisualElement gridCellContainer = new VisualElement();

            gridCellContainer.name = "gridCellContainer";

            gridCellContainer.RegisterCallback<MouseMoveEvent>(OnMouseMoved);
            gridCellContainer.RegisterCallback<MouseLeaveEvent>(OnMouseLeave);
            gridCellContainer.RegisterCallback<MouseDownEvent>(OnMouseDown);
            gridCellContainer.RegisterCallback<MouseUpEvent>(OnMouseUp);

            // Add the cell container to the grid container, the cell container will be centered because of grid-containers uss styling
            gridContainer.Add(gridCellContainer);

            for (int y = 0; y < yCells; y++)
            {
                grid.Add(new List<GridCell>());

                for (int x = 0; x < xCells; x++)
                {
                    Image cellImage = new Image();

                    cellImage.AddToClassList("grid-cell");

                    cellImage.image = cellTexture;

                    gridCellContainer.Add(cellImage);


                    GridCell gridCell = new GridCell();

                    gridCell.row = (yCells - 1)-y;  //从下往上
                    gridCell.col = x;
                    gridCell.imageField = cellImage;


                    int value = int.Parse(items[itemIndex++]);
                   
                    gridCell.shapeIndex = value; 
                    gridCell.imageField.tintColor = GetShapeColor(value);
                    gridCell.shapeColor = GetShapeColor(value);

                    if (colorItems != null)
                    {
                        string colorStr = colorItems[itemIndex - 6];

                        Color achieveColor = Color.clear;
                        if (colorStr == "-1")
                        {
                            achieveColor = Color.clear;
                        }
                        else
                        {
                            ColorUtility.TryParseHtmlString(colorStr, out achieveColor);
                        }
                      

                        gridCell.acheveColor = achieveColor;
                    }

                    grid[y].Add(gridCell);
                }
            }

            ValidateMinMaxShapeSize();
        }



        /// <summary>
        /// Gets the output folder asset path
        /// </summary>
        private string GetInputFolderAssetPath()
        {
            if (OutputFolderField.value != null)
            {
                string assetPath = AssetDatabase.GetAssetPath(InputFolderField.value);
                bool isDir = ((System.IO.File.GetAttributes(assetPath) & System.IO.FileAttributes.Directory) == System.IO.FileAttributes.Directory);

                if (isDir)
                {
                    return assetPath;
                }
            }

            return "Assets";
        }

        /// <summary>
        /// Gets the output file asset path
        /// </summary>
        private string GetInputFileAssetPath(string folderAssetPath)
        {
            string filename = string.IsNullOrEmpty(InPutFilename) ? "level" : InPutFilename;

            string path = folderAssetPath + "/" + filename + ".txt";

            return path;
        }

        /// <summary>
        /// Exports the level text file
        /// </summary>
        private void Export(List<List<int>> gridCellValues)
		{
			int yCells = gridCellValues.Count;
			int xCells = gridCellValues[0].Count;

			ulong timestamp = (ulong)Utilities.SystemTimeInMilliseconds;

			string contents = string.Format("{0},{1},{2},{3},{4}", timestamp, (int)LevelType, RotateHexagon, yCells, xCells);

			for (int y = 0; y < yCells; y++)
			{
				for (int x = 0; x < xCells; x++)
				{
					contents += "," + gridCellValues[y][x];     //gridCellValues[y].Add(gridCell.shapeIndex);
                }
			}



			string outputFolderAssetPath	= GetOutputFolderAssetPath();
			string outputFileAssetPath		= GetOutputFileAssetPath(outputFolderAssetPath,string.Empty);
			string outputFileFullPath		= Application.dataPath + outputFileAssetPath.Remove(0, "Assets".Length);

			System.IO.File.WriteAllText(outputFileFullPath, contents);
		}

		/// <summary>
		/// Gets the output folder asset path
		/// </summary>
		private string GetOutputFolderAssetPath()
		{
			if (OutputFolderField.value != null)
			{
				string	assetPath	= AssetDatabase.GetAssetPath(OutputFolderField.value);
				bool	isDir		= ((System.IO.File.GetAttributes(assetPath) & System.IO.FileAttributes.Directory) == System.IO.FileAttributes.Directory);

				if (isDir)
				{
					return assetPath;
				}
			}

			return "Assets";
		}

		/// <summary>
		/// Gets the output file asset path
		/// </summary>
		private static string GetOutputFileAssetPath(string folderAssetPath, string filename)
		{
			//string filename = string.IsNullOrEmpty(Filename) ? "level" : Filename;

			string path = folderAssetPath + "/" + filename + ".json";

			return AssetDatabase.GenerateUniqueAssetPath(path);
		}


        





        #endregion // Private Methods
    }
}

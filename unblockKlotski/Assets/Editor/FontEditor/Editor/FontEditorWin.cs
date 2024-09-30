using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using Tools;
namespace AzhaoFontEditor
{
    public class FontEditorWin : EditorWindow
    {
        private int selectTitle = 0;
        private string[] titles = new string[] { "文件", "设置" };
        private List<FontTextureData> fontTextures;
        private Vector2 texScrollPos = new Vector2();
        private int[] sizeArr = new int[] { 64, 128, 256, 512, 1024 };
        private string[] sizeStrArr = new string[] { "64", "128", "256", "512", "1024" };
        private int maxWidth = 1;
        private int maxHeight = 1;
        private string fontName = "";
        private List<Texture2D> atlas;
        private Texture2D curAtlas;
        private string fontPath = "";
        private int spacing = 0;
        private int fontSpace = 0;
        private int curX = 0;
        private int curY = 0;
        private int MaxY = 0;
        private int curPage = 0;
        void OnGUI()
        {
            selectTitle = GUILayout.SelectionGrid(selectTitle, titles, 5);
            //文件
            if (selectTitle == 0)
            {
                if (GUILayout.Button("加载旧字体"))
                {
                    LoadOldFont();
                }
                if (GUILayout.Button("加载图片"))
                {
                    AddTexture();
                }
                if (fontTextures != null)
                {
                    texScrollPos = EditorGUILayout.BeginScrollView(texScrollPos, GUILayout.Width(300), GUILayout.Height(400));
                    for (int i = 0; i < fontTextures.Count; i++)
                    {
                        EditorGUILayout.BeginHorizontal();
                        GUILayout.Box(fontTextures[i].tex, GUILayout.Width(20), GUILayout.Height(20));
                        fontTextures[i].text = GUILayout.TextField(fontTextures[i].text);
                        if (GUILayout.Button("删除"))
                        {
                            DelTexture(fontTextures[i].tex);
                        }

                        EditorGUILayout.EndHorizontal();
                    }
                    EditorGUILayout.EndScrollView();
                }
            }
            //设置
            else if (selectTitle == 1)
            {
                GUILayout.Label("请输入字体名称");
                ////允许ctrl+v粘贴内容
                //// && Event.current.modifiers == EventModifiers.Control && Event.current.keyCode==KeyCode.V
                //if (Event.current.commandName == "Paste")
                //{

                //    TextEditor p = new TextEditor();
                //    p.OnFocus();
                //    p.Paste();
                //    string text = p.text;
                //    if (!string.IsNullOrEmpty(text))
                //        fontName = GUILayout.TextField(text);
                //}
                //else
                //    fontName = GUILayout.TextField(fontName);
                fontName = EditorGUILayout.TextField(fontName);
                GUILayout.Label("图集设置");
                GUILayout.Label("最大宽度");
                maxWidth = EditorGUILayout.IntPopup(maxWidth, sizeStrArr, sizeArr);
                GUILayout.Label("最大高度");
                maxHeight = EditorGUILayout.IntPopup(maxHeight, sizeStrArr, sizeArr);
                GUILayout.Label("图集间距");
                spacing = EditorGUILayout.IntField(spacing);
                GUILayout.Label("字体间距");
                fontSpace = EditorGUILayout.IntField(fontSpace);
                if (GUILayout.Button("保存"))
                {
                    SaveFont();
                }
            }
        }

        public void InitData()
        {
            maxWidth = 256;
            maxHeight = 256;
        }

        private void LoadOldFont()
        {
            Object[] objs = Selection.GetFiltered(typeof(TextAsset), SelectionMode.DeepAssets);
            bool isLoad = false;
            FontAtlasData fad = null;
            for (int i = 0; i < objs.Length; i++)
            {
                TextAsset ta = (TextAsset)objs[i];
                fad = AzhaoJson.Json.ToObject<FontAtlasData>(ta.text);
                if (fad != null && !string.IsNullOrEmpty(fad.name))
                {
                    isLoad = true;
                    break;
                }
            }
            if (isLoad == false)
            {
                EditorUtility.DisplayDialog("提示", "没有适合的文件", "确定");
                return;
            }

            fontName = fad.name;
            maxWidth = fad.maxWidth;
            maxHeight = fad.maxHeight;
            spacing = fad.spacing;
            fontSpace = fad.fontSpace;
            LoadOldTextures(fad);

        }

        private void LoadOldTextures(FontAtlasData fad)
        {
            bool isError = false;
            string errorPath = "";
            for (int i = 0; i < fad.characters.Count; i++)
            {
                Texture2D tex = AssetDatabase.LoadAssetAtPath<Texture2D>(fad.characters[i].path);
                if (tex == null)
                {
                    isError = true;
                    errorPath = fad.characters[i].path;
                    break;
                }
                fad.characters[i].tex = tex;
            }
            if (isError == false)
            {
                fontTextures = fad.characters;
            }
            else
            {
                EditorUtility.DisplayDialog("提示", "找不到原始资源：" + errorPath, "确定");
            }

        }

        private void AddTexture()
        {
            if (fontTextures == null)
            {
                fontTextures = new List<FontTextureData>();
            }
            LoadTexture();
        }

        private void LoadTexture()
        {
            Object[] objxs = Selection.GetFiltered(typeof(Object), SelectionMode.Assets);
            if(objxs.Length>0)
            {
                fontName = objxs[0].name;
            }
            Object[] objs = Selection.GetFiltered(typeof(Texture2D), SelectionMode.DeepAssets);
            if (objs != null && objs.Length > 0)
            {
                for (int i = 0; i < objs.Length; i++)
                {
                    string path = AssetDatabase.GetAssetPath(objs[i]);
                    if (!string.IsNullOrEmpty(path))
                    {
                        TextureImporter ti = (TextureImporter)AssetImporter.GetAtPath(path);
                        if (ti != null)
                        {
                            ti.textureFormat = TextureImporterFormat.AutomaticTruecolor;
                            Texture2D tex = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
                            Add2TextureLibs(tex);
                        }
                    }
                    //Add2TextureLibs((Texture2D)objs[i]);
                }
            }
            else
            {
                EditorUtility.DisplayDialog("提示", "请先选中有美术字图片的文件夹", "确定");
            }
        }

        private void Add2TextureLibs(Texture2D tex)
        {
            if (tex == null)
            {
                return;
            }
            if (isTextureExist(tex))
            {
                return;
            }
            fontTextures.Add(GetTextureData(tex));
        }

        private FontTextureData GetTextureData(Texture2D tex)
        {
            FontTextureData ftd = new FontTextureData();
            ftd.path = AssetDatabase.GetAssetPath(tex);
            ftd.tex = tex;
            ftd.width = tex.width;
            ftd.height = tex.height;
            string texName = tex.name.ToLower();
            if(texName.IndexOf("space")>=0)
            {
                ftd.text = " ";
            }
            else
            {
                ftd.text = tex.name;
            }           

            return ftd;
        }

        private bool isTextureExist(Texture2D tex)
        {
            bool isExist = false;
            for (int i = 0; i < fontTextures.Count; i++)
            {
                if (fontTextures[i].tex == tex)
                {
                    isExist = true;
                    break;
                }
            }
            return isExist;
        }

        private void DelTexture(Texture2D tex)
        {
            int ind = -1;
            for (int i = 0; i < fontTextures.Count; i++)
            {
                if (fontTextures[i].tex == tex)
                {
                    ind = i;
                    break;
                }
            }
            if (ind >= 0)
            {
                fontTextures.RemoveAt(ind);
            }
        }

        private void SaveFont()
        {
            if (fontTextures == null || fontTextures.Count == 0)
            {
                EditorUtility.DisplayDialog("提示", "请先选择美术字图片", "确定");
                return;
            }
            if (ChectSavePath())
            {
                if (EditorUtility.DisplayDialog("提示", "字体文件已存在，是否覆盖？", "确定", "取消") == false)
                {
                    return;
                }

            }
            FileManager.CheckDirection(fontPath);
            string r = CreateKeyCode();
            if (!string.IsNullOrEmpty(r))
            {
                EditorUtility.DisplayDialog("提示", "ASCII编码转换错误:" + r, "确定");
                return;
            }
            ChangeTexture();
            r = CreateAtlas();
            if (!string.IsNullOrEmpty(r))
            {
                EditorUtility.DisplayDialog("提示", "图集转换错误：" + r, "确定");
                return;
            }
            SaveFile();
            CreateFont();
            EditorUtility.DisplayDialog("提示", "创建字体成功，路径：" + fontPath, "确定");
            return;
        }

        private bool ChectSavePath()
        {
            fontPath = Application.dataPath + "/UINew/artFont/" + fontName + "/";
            if (FileManager.IsDirectoryExists(fontPath))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private string CreateKeyCode()
        {
            List<int> checkList = new List<int>();
            if (fontTextures == null)
            {
                return "没有美术字图片";
            }
            string errorCode = "";
            for (int i = 0; i < fontTextures.Count; i++)
            {
                if (string.IsNullOrEmpty(fontTextures[i].text))
                {
                    errorCode = "有些美术字没有输入对应的文字";
                    break;
                }
                try
                {
                    int[] codes = StringTools.String2Unicodes(fontTextures[i].text);
                    for (int j = 0; j < codes.Length; j++)
                    {
                        if (checkList.IndexOf(codes[j]) < 0)
                        {
                            checkList.Add(codes[j]);
                        }
                        else
                        {
                            errorCode = "有些美术字输入的对应文字重复";
                            break;
                        }
                    }
                    if (string.IsNullOrEmpty(errorCode))
                    {
                        fontTextures[i].codeId = codes;
                    }
                    else
                    {
                        break;
                    }


                }
                catch (System.Exception e)
                {
                    errorCode = e.Message;
                    break;
                }

            }

            return errorCode;
        }

        private void ChangeTexture()
        {
            for (int i = 0; i < fontTextures.Count; i++)
            {
                Texture2D tex = fontTextures[i].tex;
                string path = AssetDatabase.GetAssetPath(tex);
                int ind = path.IndexOf("Assets");
                if (ind >= 0)
                {
                    path = path.Substring(ind, path.Length - ind);
                }
                TextureImporter ti = (TextureImporter)AssetImporter.GetAtPath(path);
                if (ti != null)
                {
                    ti.textureType = TextureImporterType.Default;
                    ti.npotScale = TextureImporterNPOTScale.None;
                    ti.isReadable = true;
                    ti.SaveAndReimport();
                }
            }
            AssetDatabase.SaveAssets();

        }

        private string CreateAtlas()
        {
            InitAtlasData();
            string errorCode = "";
            for (int i = 0; i < fontTextures.Count; i++)
            {
                if (CheckCanWriteAtlas(fontTextures[i].tex) == false)
                {
                    errorCode = "设置的图集宽高不足以容纳所有字体，请调大一点";
                    break;
                    //AddAtlasPage();
                }

                Write2Atlas(fontTextures[i]);
            }
            return errorCode;
        }

        private Texture2D CreateEmptyTexture(int w, int h)
        {
            Texture2D tex = new Texture2D(w, h);
            Color32[] cols = new Color32[w * h];
            int ind = 0;
            for (int i = 0; i < h; i++)
            {
                for (int j = 0; j < w; j++)
                {
                    cols[ind] = new Color32(0, 0, 0, 0);
                    ind++;
                }
            }
            tex.SetPixels32(cols);
            tex.Apply();
            return tex;
        }

        private void InitAtlasData()
        {
            curX = 0;
            curY = 0;
            MaxY = 0;
            curPage = 0;
            atlas = new List<Texture2D>();
            curAtlas = CreateEmptyTexture(maxWidth, maxHeight);
            atlas.Add(curAtlas);
        }

        private void AddAtlasPage()
        {
            curX = 0;
            curY = 0;
            MaxY = 0;
            curPage++;
            curAtlas = CreateEmptyTexture(maxWidth, maxHeight);
            atlas.Add(curAtlas);
        }


        //private void Write2Atlas(FontTextureData data)
        //{
        //    Texture2D tex = data.tex;
        //    data.x = curX + spacing;
        //    data.y = curY + spacing;
        //    data.width = tex.width;
        //    data.height = tex.height;
        //    data.page = curPage;
        //    data.offsetY = -1 * tex.height;
        //    int tex_top = tex.height;
        //    int tex_bottom = 0;
        //    int tex_left = tex.width;
        //    int tex_right = 0;
        //    int width_min = 0;
        //    int height_min = 0;
        //    for (int i = 0; i < tex.height; i++)
        //    {
        //        for (int j = 0; j < tex.width; j++)
        //        {
        //            Color c = tex.GetPixel(j, tex.height - i);
        //            if (c.a > 0)
        //            {
        //                tex_top = i < tex_top ? i : tex_top;
        //                tex_bottom = i > tex_bottom ? i : tex_bottom;
        //                tex_left = j < tex_left ? j : tex_left;
        //                tex_right = j > tex_right ? j : tex_right;
        //            }
        //        }
        //    }

        //    //这里为了让图片显示更美观，top和bottom各多加1个像素点
        //    tex_top -= 1;
        //    tex_bottom += 1;
        //    tex_left -= 1;
        //    tex_right += 1;
        //    width_min = tex_right - tex_left;
        //    height_min = tex_bottom - tex_top;

        //    data.width = width_min;
        //    data.height = height_min;
        //    //data.offsetY = -1 * height_min;

        //    int set_height = 0;
        //    int set_width = 0;
        //    for (int i = tex_left; i < tex_right; i++)
        //    {
        //        set_width = curX + spacing + i - tex_left;
        //        for (int j = tex_top; j < tex_bottom; j++)
        //        {
        //            set_height = curY + spacing + j - tex_top;
        //            Color c = tex.GetPixel(i, tex.height - j);
        //            curAtlas.SetPixel(set_width, maxHeight - set_height, c);
        //        }
        //    }
        //    curX = curX + spacing + width_min;
        //    MaxY = curY + spacing + height_min;
        //    curAtlas.Apply();
        //}

        #region 昭爷写的，注释掉
        private void Write2Atlas(FontTextureData data)
        {
            Texture2D tex = data.tex;
            data.x = curX + spacing;
            data.y = curY + spacing;
            data.width = tex.width;
            data.height = tex.height;
            data.page = curPage;
            data.offsetY = -1 * tex.height;
            List<Color32> cols = new List<Color32>();
            for (int i = 0; i < tex.height; i++)
            {
                for (int j = 0; j < tex.width; j++)
                {
                    cols.Add(tex.GetPixel(j, tex.height - i));
                }
            }
            int ind = 0;
            for (int i = curY + spacing; i < curY + spacing + tex.height; i++)
            {
                for (int j = curX + spacing; j < curX + spacing + tex.width; j++)
                {
                    curAtlas.SetPixel(j, maxHeight - i, cols[ind]);
                    ind++;
                }
            }
            //curAtlas.SetPixels32(curX + spacing, curY+spacing, tex.width, tex.height, cols);
            curX = curX + spacing + tex.width;
            MaxY = curY + spacing + tex.height;
            curAtlas.Apply();
        }
        #endregion

        private bool CheckCanWriteAtlas(Texture2D tex)
        {

            if (curY + spacing + tex.height > maxHeight)
            {
                return false;
            }
            if (curX + spacing + tex.width > maxWidth)
            {
                curX = 0;
                curY = MaxY;
                return CheckCanWriteAtlas(tex);
            }
            return true;
        }

        private void SaveFile()
        {
            FontAtlasData data = new FontAtlasData();
            data.name = fontName;
            data.maxWidth = maxWidth;
            data.maxHeight = maxHeight;
            data.spacing = spacing;
            data.fontSpace = fontSpace;

            List<string> texPaths = new List<string>();
            for (int i = 0; i < curPage + 1; i++)
            {
                texPaths.Add(fontName + i);
            }
            data.textures = texPaths;
            data.characters = fontTextures;
            string jsonStr = AzhaoJson.Json.ToJson(data);
            string configPath = fontPath + fontName + "config.txt";
            FileManager.SaveFile(configPath, jsonStr, true);
            for (int i = 0; i < atlas.Count; i++)
            {
                string pngPath = fontPath + fontName + i + ".png";
                byte[] bs = atlas[i].EncodeToPNG();
                FileManager.SaveBytes(pngPath, bs);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                TextureImporter ti = (TextureImporter)AssetImporter.GetAtPath("Assets/Resources/Font/" + fontName + "/" + fontName + i + ".png");
                if (ti != null)
                {
                    ti.textureFormat = TextureImporterFormat.AutomaticTruecolor;
                }
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }


        private void CreateFont()
        {
            string path = (string)fontPath.Clone();
            int ind = path.IndexOf("Assets");
            if (ind >= 0)
            {
                path = path.Substring(ind, path.Length - ind);
            }
            bool isCreateFont = false;
            string fontAssetPath = path + fontName + ".fontsettings";

            //Font font = new Font();//AssetDatabase.LoadAssetAtPath<Font>(path + fontName + ".fontsettings");
            ////if(font == null)
            ////{
            ////    font = new Font();
            ////    AssetDatabase.CreateAsset(font, path + fontName + ".fontsettings");
            ////}

            Font font = AssetDatabase.LoadAssetAtPath<Font>(fontAssetPath);
            if(font==null)
            {
                font = new Font();
                isCreateFont = true;
            }

            int characterNum = 0;
            for (int i = 0; i < fontTextures.Count; i++)
            {
                characterNum += fontTextures[i].codeId.Length;
            }

            CharacterInfo[] cs = new CharacterInfo[characterNum];
            int csInd = 0;
            int lineHeight = 0;
            for (int i = 0; i < fontTextures.Count; i++)
            {
                FontTextureData ftd = fontTextures[i];
                for (int j = 0; j < ftd.codeId.Length; j++)
                {
                    CharacterInfo ci = GetCharacterInfo(ftd, ftd.codeId[j]);
                    cs[csInd] = ci;
                    if (ftd.height > lineHeight)
                    {
                        lineHeight = ftd.height;
                    }
                    csInd++;
                }

            }
            font.characterInfo = cs;
            font.name = fontName;

            Material mat = AssetDatabase.LoadAssetAtPath<Material>(path + fontName + "mat.mat");
            if (mat == null)
            {
                mat = new Material(Shader.Find("UI/Default"));
                AssetDatabase.CreateAsset(mat, path + fontName + "mat.mat");
            }


            Texture tex = AssetDatabase.LoadAssetAtPath<Texture>(path + fontName + "0.png");
            mat.SetTexture("_MainTex", tex);
            font.material = mat;
            if(isCreateFont==true)
            {
                AssetDatabase.CreateAsset(font, path + fontName + ".fontsettings");
            }        
            else
            {
                EditorUtility.SetDirty(font);
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private CharacterInfo GetCharacterInfo(FontTextureData ftd, int index)
        {
            CharacterInfo ci = new CharacterInfo();
            ci.index = index;
            ci.width = ftd.width;
            ci.vert = new Rect(ftd.offsetX, ftd.offsetY, ftd.width, ftd.height);
            float uvx = (float)ftd.x / (float)maxWidth;
            float uvy = 1 - (float)(ftd.y) / (float)maxHeight;
            float uvw = (float)ftd.width / (float)maxWidth;
            float uvh = -1 * (float)ftd.height / (float)maxHeight;
            ci.uv = new Rect(uvx, uvy, uvw, uvh);
            ci.advance = ftd.width + fontSpace;
            return ci;
        }
    }



    public class FontEditor
    {
        [MenuItem("Tools/UI/美术字体编辑器", false, 40002)]
        [MenuItem("Assets/美术字体编辑器", false, 40002)]
        static void OpenEditor()
        {
            FontEditorWin win = (FontEditorWin)EditorWindow.GetWindow(typeof(FontEditorWin));
            win.titleContent = new GUIContent("美术字体编辑器");
            win.InitData();

        }
    }
}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AzhaoFontEditor
{
    public class FontAtlasData
    {
        public string name;
        public int maxWidth;
        public int maxHeight;
        public int spacing;
        public int fontSpace;
        public List<string> textures;
        public List<FontTextureData> characters;
    }
}

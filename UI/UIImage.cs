using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.UI
{
	public class UIImage : UIElement
	{
		public Texture2D texture = null;
		
		public UIImage(Texture2D tex, int width = -1, int height = -1) : base()
		{
			texture = tex;
			this.Width.Set(width == -1 ? tex.Width : width, 0f);
			this.Height.Set(height == -1 ? tex.Height : height, 0f);
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			CalculatedStyle dimensions = base.GetDimensions();
			Color color = Color.White;
            //base.IsMouseHovering ? Color.White : Color.Silver; //uncomment this to produce it getting brighter if hovered over.
			int width = (int)dimensions.Width, height = (int)dimensions.Height;
			spriteBatch.Draw(texture, new Rectangle((int)dimensions.X, (int)dimensions.Y, width, height), new Rectangle(0, 0, width, height), color);
		}		
	}
}
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.Yharon
{
	public class YScreenShaderData : ScreenShaderData
	{
		private int YIndex;

		public YScreenShaderData(string passName)
			: base(passName)
		{
		}

		private void UpdateYIndex()
		{
			int YType = ModLoader.GetMod("CalamityMod").NPCType("Yharon");
			if (YIndex >= 0 && Main.npc[YIndex].active && Main.npc[YIndex].type == YType)
			{
				return;
			}
			YIndex = -1;
			for (int i = 0; i < Main.npc.Length; i++)
			{
				if (Main.npc[i].active && Main.npc[i].type == YType)
				{
					YIndex = i;
					break;
				}
			}
		}

		public override void Apply()
		{
			UpdateYIndex();
			if (YIndex != -1)
			{
				UseTargetPosition(Main.npc[YIndex].Center);
			}
			base.Apply();
		}
	}
}
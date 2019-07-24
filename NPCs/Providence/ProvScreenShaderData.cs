using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.Providence
{
	public class ProvScreenShaderData : ScreenShaderData
	{
		private int ProvIndex;

		public ProvScreenShaderData(string passName)
			: base(passName)
		{
		}

		private void UpdatePIndex()
		{
			int ProvType = ModLoader.GetMod("CalamityMod").NPCType("Providence");
			if (ProvIndex >= 0 && Main.npc[ProvIndex].active && Main.npc[ProvIndex].type == ProvType)
			{
				return;
			}
			ProvIndex = -1;
			for (int i = 0; i < Main.npc.Length; i++)
			{
				if (Main.npc[i].active && Main.npc[i].type == ProvType)
				{
					ProvIndex = i;
					break;
				}
			}
		}

		public override void Apply()
		{
			UpdatePIndex();
			if (ProvIndex != -1)
			{
				UseTargetPosition(Main.npc[ProvIndex].Center);
			}
			base.Apply();
		}
	}
}
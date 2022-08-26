using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Gores.Trees
{
	public class AstralLeaf : ModGore
	{
		public override void SetStaticDefaults()
		{			
			GoreID.Sets.SpecialAI[Type] = 3;
		}
	}
}
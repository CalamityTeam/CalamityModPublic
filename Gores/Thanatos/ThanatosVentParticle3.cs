using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Gores
{
	public class ThanatosVentParticle3 : ModGore
	{
		public override void OnSpawn(Gore gore)
		{
			gore.sticky = false;
		}

		public override bool Update(Gore gore)
		{
			if (Main.netMode == NetmodeID.Server || !gore.active)
				return false;

			gore.velocity *= 0.98f;
			gore.scale -= 0.007f;
			if (gore.scale < 0.1f)
			{
				gore.scale = 0.1f;
				gore.alpha = 255;
			}

			gore.position += gore.velocity;

			if (gore.alpha >= 255)
				gore.active = false;

			return false;
		}
	}
}
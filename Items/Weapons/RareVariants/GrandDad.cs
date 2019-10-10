using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.NPCs;

namespace CalamityMod.Items.Weapons.RareVariants
{
    public class GrandDad : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Grand Dad");
			Tooltip.SetDefault("Lowers enemy defense to 0 when they are struck\n" +
						"Yeets enemies across space and time\n" +
						"7");
		}

		public override void SetDefaults()
		{
			item.width = 124;
			item.damage = 777;
			item.melee = true;
			item.useAnimation = 25;
			item.useStyle = 1;
			item.useTime = 25;
			item.useTurn = true;
			item.knockBack = 77f;
			item.UseSound = SoundID.Item1;
			item.autoReuse = true;
			item.height = 124;
			item.value = Item.buyPrice(1, 0, 0, 0);
			item.rare = 10;
			item.Calamity().postMoonLordRarity = 22;
		}

		public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
		{
			if (CalamityGlobalNPC.ShouldAffectNPC(target))
			{
				target.knockBackResist = 7f;
				target.defense = 0;
			}
		}
	}
}

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Bumblebirb
{
    public class GildedProboscis : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Gilded Proboscis");
            Tooltip.SetDefault("Ignores immunity frames\n" +
				"Heals the player on enemy hits");
        }

        public override void SetDefaults()
        {
            item.width = 66;
            item.damage = 160;
            item.melee = true;
            item.noMelee = true;
            item.useTurn = true;
            item.noUseGraphic = true;
            item.useAnimation = 19;
            item.useStyle = 5;
            item.useTime = 19;
            item.knockBack = 8.75f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 66;
            item.value = Item.buyPrice(1, 20, 0, 0);
            item.rare = 10;
            item.shoot = mod.ProjectileType("GildedProboscis");
            item.shootSpeed = 13f;
			item.GetGlobalItem<CalamityGlobalItem>(mod).postMoonLordRarity = 12;
		}

        public override bool CanUseItem(Player player)
        {
            for (int i = 0; i < 1000; ++i)
            {
                if (Main.projectile[i].active && Main.projectile[i].owner == Main.myPlayer && Main.projectile[i].type == item.shoot)
                {
                    return false;
                }
            }
            return true;
        }
    }
}

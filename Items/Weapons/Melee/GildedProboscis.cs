using CalamityMod.Projectiles.Melee.Spears;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class GildedProboscis : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Gilded Proboscis");
            Tooltip.SetDefault("Ignores immunity frames\n" +
                "Heals the player on hit");
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
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.useTime = 19;
            item.knockBack = 8.75f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 66;
            item.value = Item.buyPrice(1, 20, 0, 0);
            item.rare = 10;
            item.shoot = ModContent.ProjectileType<GildedProboscisProj>();
            item.shootSpeed = 13f;
            item.Calamity().customRarity = CalamityRarity.Turquoise;
        }

		public override bool CanUseItem(Player player) => player.ownedProjectileCounts[item.shoot] <= 0;
	}
}

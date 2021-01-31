using CalamityMod.Projectiles.Melee.Spears;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
	public class InsidiousImpaler : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Insidious Impaler");
            Tooltip.SetDefault("Fires a harpoon that sticks to enemies and explodes");
        }

        public override void SetDefaults()
        {
            item.width = 66;
            item.damage = 210;
            item.melee = true;
            item.noMelee = true;
            item.useTurn = true;
            item.noUseGraphic = true;
            item.useAnimation = 20;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.useTime = 20;
            item.knockBack = 8f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 70;
            item.value = Item.buyPrice(1, 40, 0, 0);
            item.Calamity().postMoonLordRarity = 13;
			item.rare = 10;
            item.shoot = ModContent.ProjectileType<InsidiousImpalerProj>();
            item.shootSpeed = 5f;
        }

		public override bool CanUseItem(Player player) => player.ownedProjectileCounts[item.shoot] <= 0;
	}
}

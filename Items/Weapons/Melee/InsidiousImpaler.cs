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
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 66;
            Item.damage = 210;
            Item.DamageType = DamageClass.Melee;
            Item.noMelee = true;
            Item.useTurn = true;
            Item.noUseGraphic = true;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useTime = 20;
            Item.knockBack = 8f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.height = 70;
            Item.shoot = ModContent.ProjectileType<InsidiousImpalerProj>();
            Item.shootSpeed = 5f;

            Item.value = CalamityGlobalItem.Rarity13BuyPrice;
            Item.Calamity().customRarity = CalamityRarity.PureGreen;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] <= 0;
    }
}

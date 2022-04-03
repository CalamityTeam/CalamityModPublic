using CalamityMod.Projectiles.Rogue;
using Terraria;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class FinalDawn : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Final Dawn");
            Tooltip.SetDefault("We shall ride into the sunrise once more\n" +
                "Attack enemies with a giant scythe swing to replenish stealth\n" +
                "Press up and attack to throw the scythe \n" +
                "Stealth strikes perform a horizontal swing that leaves a lingering fire aura\n" +
                "Stealth strikes performed while pressing up fling yourself at the enemy and slice through them, causing homing fireballs to emerge");
        }

        public override void SafeSetDefaults()
        {
            Item.damage = 1500;
            Item.Calamity().rogue = true;
            Item.width = 78;
            Item.height = 66;
            Item.noMelee = true;
            Item.useTime = Item.useAnimation = 15;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 4;

            Item.value = CalamityGlobalItem.Rarity15BuyPrice;
            Item.Calamity().customRarity = CalamityRarity.Violet;
            Item.Calamity().donorItem = true;

            Item.autoReuse = false;
            Item.shoot = ProjectileType<FinalDawnProjectile>();
            Item.shootSpeed = 1f;
            Item.useTurn = false;
            Item.channel = true;
            Item.noUseGraphic = true;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] +
            player.ownedProjectileCounts[ProjectileType<FinalDawnFireSlash>()] +
            player.ownedProjectileCounts[ProjectileType<FinalDawnHorizontalSlash>()] +
            player.ownedProjectileCounts[ProjectileType<FinalDawnThrow>()] +
            player.ownedProjectileCounts[ProjectileType<FinalDawnThrow2>()] <= 0;
    }
}

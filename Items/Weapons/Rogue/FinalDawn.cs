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
            item.damage = 1500;
            item.Calamity().rogue = true;
            item.width = 78;
            item.height = 66;
            item.noMelee = true;
            item.useTime = item.useAnimation = 15;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.knockBack = 4;

            item.value = CalamityGlobalItem.Rarity15BuyPrice;
            item.Calamity().customRarity = CalamityRarity.Violet;
            item.Calamity().donorItem = true;

            item.autoReuse = false;
            item.shoot = ProjectileType<FinalDawnProjectile>();
            item.shootSpeed = 1f;
            item.useTurn = false;
            item.channel = true;
            item.noUseGraphic = true;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[item.shoot] +
            player.ownedProjectileCounts[ProjectileType<FinalDawnFireSlash>()] +
            player.ownedProjectileCounts[ProjectileType<FinalDawnHorizontalSlash>()] +
            player.ownedProjectileCounts[ProjectileType<FinalDawnThrow>()] +
            player.ownedProjectileCounts[ProjectileType<FinalDawnThrow2>()] <= 0;
    }
}

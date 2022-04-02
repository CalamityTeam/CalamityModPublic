using CalamityMod.Projectiles.Magic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class GloriousEnd : ModItem
    {
        public static int PlayerExplosionDmgMin = 50;
        public static int PlayerExplosionDmgMax = 60;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Glorious End");
            Tooltip.SetDefault("Casts a meteor star for the player to ride in the direction of the cursor\n" +
            "The meteor star explodes after hitting an enemy, crashing into a wall, or after 6 seconds\n" +
            "This explosion hurts both enemies and the player\n" +
            "Releasing the cursor before the star explodes will cause it to explode prematurely for less damage\n" +
            "Mounts are disabled while this weapon is in use");
        }

        public override void SetDefaults()
        {
            item.damage = 120;
            item.knockBack = 10f;
            item.useTime = item.useAnimation = 30;
            item.mana = 20;
            item.magic = true;
            item.channel = true;
            item.shootSpeed = 7f;
            item.shoot = ModContent.ProjectileType<MeteorStar>();

            item.width = 30;
            item.height = 58;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.noMelee = true;
            item.UseSound = SoundID.Item9;
            item.value = CalamityGlobalItem.Rarity4BuyPrice;
            item.rare = ItemRarityID.LightRed;
            item.Calamity().donorItem = true;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[item.shoot] <= 0;
    }
}

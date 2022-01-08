using CalamityMod.Projectiles.Melee;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class BladecrestOathsword : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bladecrest Oathsword");
            Tooltip.SetDefault("Fires bursts of demonic blades that exponentially decelerate and explode\n" +
                "Sword of an ancient demon lord");
        }

        public override void SetDefaults()
        {
            item.width = 56;
            item.height = 56;
            item.damage = 25;
            item.melee = true;
            item.useAnimation = 21;
            item.useTime = 21;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.knockBack = 4f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.noUseGraphic = true;
            item.useTurn = true;
            item.channel = true;
            item.value = Item.buyPrice(0, 4, 0, 0);
            item.rare = ItemRarityID.Orange;
            item.shootSpeed = 6f;
        }

        public override bool CanUseItem(Player player)
        {
            int bladeProjID = ModContent.ProjectileType<BladecrestOathswordProj>();
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                if (Main.projectile[i].type != bladeProjID || !Main.projectile[i].active || Main.projectile[i].owner != player.whoAmI)
                    continue;

                return Main.projectile[i].ModProjectile<BladecrestOathswordProj>().PostSwingRepositionDelay <= 0f;
            }

            return base.CanUseItem(player);
        }

        public override bool? CanHitNPC(Player player, NPC target) => false;

        public override bool CanHitPvp(Player player, Player target) => false;
    }
}

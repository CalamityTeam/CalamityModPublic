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
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 56;
            Item.height = 56;
            Item.damage = 25;
            Item.DamageType = DamageClass.Melee;
            Item.useAnimation = 21;
            Item.useTime = 21;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 4f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.noUseGraphic = true;
            Item.useTurn = true;
            Item.channel = true;
            Item.value = CalamityGlobalItem.Rarity3BuyPrice;
            Item.rare = ItemRarityID.Orange;
            Item.shootSpeed = 6f;
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

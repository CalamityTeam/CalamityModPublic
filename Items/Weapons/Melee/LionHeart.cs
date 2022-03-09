using CalamityMod.Cooldowns;
using CalamityMod.Projectiles.Ranged;
using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class LionHeart : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lion Heart");
            Tooltip.SetDefault("Summons an energy explosion on enemy hits\n" +
            "Right click to summon an energy shell for a few seconds that halves all damage sources\n" +
            "This has a 45 second cooldown");
        }

        public override void SetDefaults()
        {
            item.width = 60;
            item.height = 62;

            item.damage = 323;
            item.knockBack = 5.5f;
            item.scale = 1.5f;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useAnimation = 15;
            item.useTime = 15;
            item.shootSpeed = 0f;

            item.melee = true;
            item.useTurn = true;
            item.autoReuse = true;
            item.UseSound = SoundID.Item1;

            item.value = CalamityGlobalItem.Rarity13BuyPrice;
            item.rare = ItemRarityID.Red;
            item.Calamity().customRarity = CalamityRarity.PureGreen;
            item.Calamity().donorItem = true;
        }

        public override bool AltFunctionUse(Player player) => true;

        public override bool CanUseItem(Player player)
        {
            item.shoot = player.altFunctionUse == 2 ? ModContent.ProjectileType<EnergyShell>() : ProjectileID.None;
            return base.CanUseItem(player);
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.altFunctionUse == 2 && !player.HasCooldown(LionHeartShield.ID) && player.ownedProjectileCounts[ModContent.ProjectileType<EnergyShell>()] <= 0)
                Projectile.NewProjectile(position.X, position.Y, speedX, speedY, ModContent.ProjectileType<EnergyShell>(), 0, 0f, player.whoAmI);
            return false;
        }

        public override bool? CanHitNPC(Player player, NPC target)
        {
            if (player.altFunctionUse == 2)
            {
                return false;
            }
            return null;
        }

        public override bool CanHitPvp(Player player, Player target) => player.altFunctionUse != 2;

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            if (crit)
                damage /= 2;

            int explosion = Projectile.NewProjectile(target.Center, Vector2.Zero, ModContent.ProjectileType<PlanarRipperExplosion>(), damage, knockback, player.whoAmI);
            if (explosion.WithinBounds(Main.maxProjectiles))
                Main.projectile[explosion].Calamity().forceMelee = true;
        }

        public override void OnHitPvp(Player player, Player target, int damage, bool crit)
        {
            if (crit)
                damage /= 2;

            int explosion = Projectile.NewProjectile(target.Center, Vector2.Zero, ModContent.ProjectileType<PlanarRipperExplosion>(), damage, item.knockBack, player.whoAmI);
            if (explosion.WithinBounds(Main.maxProjectiles))
                Main.projectile[explosion].Calamity().forceMelee = true;
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(3))
                Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 132);
        }
    }
}

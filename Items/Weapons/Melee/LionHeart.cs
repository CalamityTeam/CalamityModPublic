using Terraria.DataStructures;
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
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 60;
            Item.height = 62;

            Item.damage = 323;
            Item.knockBack = 5.5f;
            Item.scale = 1.5f;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useAnimation = 15;
            Item.useTime = 15;
            Item.shootSpeed = 0f;

            Item.DamageType = DamageClass.Melee;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item1;

            Item.value = CalamityGlobalItem.Rarity13BuyPrice;
            Item.rare = ItemRarityID.Red;
            Item.Calamity().customRarity = CalamityRarity.PureGreen;
            Item.Calamity().donorItem = true;
        }

        public override bool AltFunctionUse(Player player) => true;

        public override bool CanUseItem(Player player)
        {
            Item.shoot = player.altFunctionUse == 2 ? ModContent.ProjectileType<EnergyShell>() : ProjectileID.None;
            return base.CanUseItem(player);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2 && !player.HasCooldown(LionHeartShield.ID) && player.ownedProjectileCounts[ModContent.ProjectileType<EnergyShell>()] <= 0)
                Projectile.NewProjectile(source, position.X, position.Y, velocity.X, velocity.Y, ModContent.ProjectileType<EnergyShell>(), 0, 0f, player.whoAmI);
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
            var source = player.GetSource_ItemUse(Item);
            if (crit)
                damage /= 2;

            int explosion = Projectile.NewProjectile(source, target.Center, Vector2.Zero, ModContent.ProjectileType<PlanarRipperExplosion>(), damage, knockback, player.whoAmI);
            if (explosion.WithinBounds(Main.maxProjectiles))
                Main.projectile[explosion].DamageType = DamageClass.Melee;
        }

        public override void OnHitPvp(Player player, Player target, int damage, bool crit)
        {
            var source = player.GetSource_ItemUse(Item);
            if (crit)
                damage /= 2;

            int explosion = Projectile.NewProjectile(source, target.Center, Vector2.Zero, ModContent.ProjectileType<PlanarRipperExplosion>(), damage, Item.knockBack, player.whoAmI);
            if (explosion.WithinBounds(Main.maxProjectiles))
                Main.projectile[explosion].DamageType = DamageClass.Melee;
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(3))
                Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 132);
        }
    }
}

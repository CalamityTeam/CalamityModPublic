using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Tools
{
    public class InfernaCutter : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Inferna Cutter");
            Tooltip.SetDefault("Critical hits with the blade cause small explosions\n" +
                "Generates a number of small sparks when swung");
        }

        public override void SetDefaults()
        {
            Item.damage = 110;
            Item.knockBack = 7f;
            Item.useTime = 8;
            Item.useAnimation = 12;
            Item.axe = 135 / 5;
            Item.tileBoost += 1;

            Item.DamageType = DamageClass.Melee;
            Item.width = 80;
            Item.height = 66;
            Item.useTurn = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.value = Item.buyPrice(0, 36, 0, 0);
            Item.rare = ItemRarityID.Pink;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
        }

        // Terraria seems to really dislike high crit values in SetDefaults
        public override void ModifyWeaponCrit(Player player, ref float crit) => crit += 10;

        public override void UseItemHitbox(Player player, ref Rectangle hitbox, ref bool noHitbox)
        {
            hitbox = CalamityUtils.FixSwingHitbox(54, 54);
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<AxeofPurity>().
                AddIngredient(ItemID.SoulofFright, 8).
                AddIngredient<EssenceofChaos>(3).
                AddTile(TileID.MythrilAnvil).
                Register();
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (player.whoAmI == Main.myPlayer)
            {
                if (player.itemAnimation == (int)(player.itemAnimationMax * 0.1) ||
                    player.itemAnimation == (int)(player.itemAnimationMax * 0.3) ||
                    player.itemAnimation == (int)(player.itemAnimationMax * 0.5) ||
                    player.itemAnimation == (int)(player.itemAnimationMax * 0.7) ||
                    player.itemAnimation == (int)(player.itemAnimationMax * 0.9))
                {
                    float num339 = 0f;
                    float num340 = 0f;
                    float num341 = 0f;
                    float num342 = 0f;
                    if (player.itemAnimation == (int)(player.itemAnimationMax * 0.9))
                    {
                        num339 = -7f;
                    }
                    if (player.itemAnimation == (int)(player.itemAnimationMax * 0.7))
                    {
                        num339 = -6f;
                        num340 = 2f;
                    }
                    if (player.itemAnimation == (int)(player.itemAnimationMax * 0.5))
                    {
                        num339 = -4f;
                        num340 = 4f;
                    }
                    if (player.itemAnimation == (int)(player.itemAnimationMax * 0.3))
                    {
                        num339 = -2f;
                        num340 = 6f;
                    }
                    if (player.itemAnimation == (int)(player.itemAnimationMax * 0.1))
                    {
                        num340 = 7f;
                    }
                    if (player.itemAnimation == (int)(player.itemAnimationMax * 0.7))
                    {
                        num342 = 26f;
                    }
                    if (player.itemAnimation == (int)(player.itemAnimationMax * 0.3))
                    {
                        num342 -= 4f;
                        num341 -= 20f;
                    }
                    if (player.itemAnimation == (int)(player.itemAnimationMax * 0.1))
                    {
                        num341 += 6f;
                    }
                    if (player.direction == -1)
                    {
                        if (player.itemAnimation == (int)(player.itemAnimationMax * 0.9))
                        {
                            num342 -= 8f;
                        }
                        if (player.itemAnimation == (int)(player.itemAnimationMax * 0.7))
                        {
                            num342 -= 6f;
                        }
                    }
                    num339 *= 1.5f;
                    num340 *= 1.5f;
                    num342 *= (float)player.direction;
                    num341 *= player.gravDir;
                    var source = player.GetSource_ItemUse(Item);
                    int damage = (int)player.GetTotalDamage<MeleeDamageClass>().ApplyTo(Item.damage * 0.2f);
                    int spark = Projectile.NewProjectile(source, (float)(hitbox.X + hitbox.Width / 2) + num342, (float)(hitbox.Y + hitbox.Height / 2) + num341, (float)player.direction * num340, num339 * player.gravDir, ProjectileID.Spark, damage, 0f, player.whoAmI);
                    if (spark.WithinBounds(Main.maxProjectiles))
                        Main.projectile[spark].DamageType = DamageClass.Melee;
                }
            }
            if (Main.rand.NextBool(4))
            {
                int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 6);
            }
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            if (crit)
            {
                damage /= 2;
                var source = player.GetSource_ItemUse(Item);
                int boom = Projectile.NewProjectile(source, target.Center, Vector2.Zero, ModContent.ProjectileType<FuckYou>(), damage, knockback, player.whoAmI, 0f, 0.85f + Main.rand.NextFloat() * 1.15f);
                if (boom.WithinBounds(Main.maxProjectiles))
                    Main.projectile[boom].DamageType = DamageClass.Melee;
            }
            target.AddBuff(BuffID.OnFire, 300);
            target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 150);
        }
    }
}

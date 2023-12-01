using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Tools
{
    public class InfernaCutter : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Tools";
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
            Item.value = CalamityGlobalItem.Rarity5BuyPrice;
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
                AddIngredient<EssenceofHavoc>(3).
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
                    float sparkYVel = 0f;
                    float sparkXVel = 0f;
                    float sparkYSpawn = 0f;
                    float sparkXSpawn = 0f;
                    if (player.itemAnimation == (int)(player.itemAnimationMax * 0.9))
                    {
                        sparkYVel = -7f;
                    }
                    if (player.itemAnimation == (int)(player.itemAnimationMax * 0.7))
                    {
                        sparkYVel = -6f;
                        sparkXVel = 2f;
                    }
                    if (player.itemAnimation == (int)(player.itemAnimationMax * 0.5))
                    {
                        sparkYVel = -4f;
                        sparkXVel = 4f;
                    }
                    if (player.itemAnimation == (int)(player.itemAnimationMax * 0.3))
                    {
                        sparkYVel = -2f;
                        sparkXVel = 6f;
                    }
                    if (player.itemAnimation == (int)(player.itemAnimationMax * 0.1))
                    {
                        sparkXVel = 7f;
                    }
                    if (player.itemAnimation == (int)(player.itemAnimationMax * 0.7))
                    {
                        sparkXSpawn = 26f;
                    }
                    if (player.itemAnimation == (int)(player.itemAnimationMax * 0.3))
                    {
                        sparkXSpawn -= 4f;
                        sparkYSpawn -= 20f;
                    }
                    if (player.itemAnimation == (int)(player.itemAnimationMax * 0.1))
                    {
                        sparkYSpawn += 6f;
                    }
                    if (player.direction == -1)
                    {
                        if (player.itemAnimation == (int)(player.itemAnimationMax * 0.9))
                        {
                            sparkXSpawn -= 8f;
                        }
                        if (player.itemAnimation == (int)(player.itemAnimationMax * 0.7))
                        {
                            sparkXSpawn -= 6f;
                        }
                    }
                    sparkYVel *= 1.5f;
                    sparkXVel *= 1.5f;
                    sparkXSpawn *= (float)player.direction;
                    sparkYSpawn *= player.gravDir;
                    var source = player.GetSource_ItemUse(Item);
                    int damage = (int)player.GetTotalDamage<MeleeDamageClass>().ApplyTo(Item.damage * 0.2f);
                    int spark = Projectile.NewProjectile(source, (float)(hitbox.X + hitbox.Width / 2) + sparkXSpawn, (float)(hitbox.Y + hitbox.Height / 2) + sparkYSpawn, (float)player.direction * sparkXVel, sparkYVel * player.gravDir, ProjectileID.Spark, damage, 0f, player.whoAmI);
                    if (spark.WithinBounds(Main.maxProjectiles))
                        Main.projectile[spark].DamageType = DamageClass.Melee;
                }
            }
            if (Main.rand.NextBool(4))
            {
                int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 6);
            }
        }

        public override void ModifyHitNPC(Player player, NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.CritDamage *= 0.5f;
        }
        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (hit.Crit)
            {
                var source = player.GetSource_ItemUse(Item);
                int boom = Projectile.NewProjectile(source, target.Center, Vector2.Zero, ModContent.ProjectileType<FuckYou>(), Item.damage, Item.knockBack, player.whoAmI, 0f, 0.85f + Main.rand.NextFloat() * 1.15f);
                if (boom.WithinBounds(Main.maxProjectiles))
                    Main.projectile[boom].DamageType = DamageClass.Melee;
            }
            target.AddBuff(BuffID.OnFire3, 300);
        }
    }
}

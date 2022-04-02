using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class SoulHarvester : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Soul Harvester");
            Tooltip.SetDefault("Shoots a soul scythe\n" +
                "Enemies explode when on low health, spreading the plague");
        }

        public override void SetDefaults()
        {
            item.width = 62;
            item.damage = 98;
            item.melee = true;
            item.useAnimation = 20;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTime = 20;
            item.useTurn = true;
            item.knockBack = 7.5f;
            item.UseSound = SoundID.Item71;
            item.autoReuse = true;
            item.height = 64;
            item.value = Item.buyPrice(0, 80, 0, 0);
            item.rare = ItemRarityID.Yellow;
            item.shoot = ModContent.ProjectileType<SoulScythe>();
            item.shootSpeed = 18f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.DeathSickle);
            recipe.AddIngredient(ModContent.ItemType<PlagueCellCluster>(), 10);
            recipe.AddIngredient(ItemID.CursedFlame, 20);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(3))
            {
                int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 75);
            }
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<Plague>(), 240);
            target.AddBuff(BuffID.CursedInferno, 120);
            if (target.life <= (target.lifeMax * 0.15f))
            {
                Main.PlaySound(SoundID.Item14, target.position);
                player.ApplyDamageToNPC(target, (int)(item.damage * (player.allDamage + player.meleeDamage - 1f)), 0f, 0, false);
                for (int num621 = 0; num621 < 10; num621++)
                {
                    int num622 = Dust.NewDust(new Vector2(target.position.X, target.position.Y), target.width, target.height, 89, 0f, 0f, 100, default, 2f);
                    Main.dust[num622].velocity *= 3f;
                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[num622].scale = 0.5f;
                        Main.dust[num622].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int num623 = 0; num623 < 20; num623++)
                {
                    int num624 = Dust.NewDust(new Vector2(target.position.X, target.position.Y), target.width, target.height, 89, 0f, 0f, 100, default, 3f);
                    Main.dust[num624].noGravity = true;
                    Main.dust[num624].velocity *= 5f;
                    num624 = Dust.NewDust(new Vector2(target.position.X, target.position.Y), target.width, target.height, 89, 0f, 0f, 100, default, 2f);
                    Main.dust[num624].velocity *= 2f;
                }
            }
        }

        public override void OnHitPvp(Player player, Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<Plague>(), 240);
            target.AddBuff(BuffID.CursedInferno, 120);
            if (target.statLife <= (target.statLifeMax * 0.15f))
            {
                Main.PlaySound(SoundID.Item14, target.position);
                for (int num621 = 0; num621 < 10; num621++)
                {
                    int num622 = Dust.NewDust(new Vector2(target.position.X, target.position.Y), target.width, target.height, 89, 0f, 0f, 100, default, 2f);
                    Main.dust[num622].velocity *= 3f;
                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[num622].scale = 0.5f;
                        Main.dust[num622].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int num623 = 0; num623 < 20; num623++)
                {
                    int num624 = Dust.NewDust(new Vector2(target.position.X, target.position.Y), target.width, target.height, 89, 0f, 0f, 100, default, 3f);
                    Main.dust[num624].noGravity = true;
                    Main.dust[num624].velocity *= 5f;
                    num624 = Dust.NewDust(new Vector2(target.position.X, target.position.Y), target.width, target.height, 89, 0f, 0f, 100, default, 2f);
                    Main.dust[num624].velocity *= 2f;
                }
            }
        }
    }
}

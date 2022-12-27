using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Items.Weapons.Melee
{
    public class SoulHarvester : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Soul Harvester");
            Tooltip.SetDefault("Shoots a soul scythe\n" +
                "Enemies explode when on low health, spreading the plague");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 62;
            Item.damage = 98;
            Item.DamageType = DamageClass.Melee;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 20;
            Item.useTurn = true;
            Item.knockBack = 7.5f;
            Item.UseSound = SoundID.Item71;
            Item.autoReuse = true;
            Item.height = 64;
            Item.value = CalamityGlobalItem.Rarity8BuyPrice;
            Item.rare = ItemRarityID.Yellow;
            Item.shoot = ModContent.ProjectileType<SoulScythe>();
            Item.shootSpeed = 18f;
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
                SoundEngine.PlaySound(SoundID.Item14, target.position);
                int onHitDamage = player.CalcIntDamage<MeleeDamageClass>(Item.damage);
                player.ApplyDamageToNPC(target, onHitDamage, 0f, 0, false);
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
                SoundEngine.PlaySound(SoundID.Item14, target.position);
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

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.DeathSickle).
                AddIngredient<PlagueCellCanister>(10).
                AddIngredient(ItemID.CursedFlame, 20).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}

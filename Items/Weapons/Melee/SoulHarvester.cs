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
    public class SoulHarvester : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";
        public override void SetDefaults()
        {
            Item.width = 62;
            Item.damage = 85;
            Item.DamageType = DamageClass.Melee;
            Item.useAnimation = Item.useTime = 22;
            Item.useStyle = ItemUseStyleID.Swing;
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

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<Plague>(), 240);
            target.AddBuff(BuffID.CursedInferno, 120);
            if (target.life <= (target.lifeMax * 0.15f))
            {
                SoundEngine.PlaySound(SoundID.Item14, target.Center);
                int onHitDamage = player.CalcIntDamage<MeleeDamageClass>(Item.damage);
                player.ApplyDamageToNPC(target, onHitDamage, 0f, 0, false);
                for (int i = 0; i < 10; i++)
                {
                    int plagueDust = Dust.NewDust(new Vector2(target.position.X, target.position.Y), target.width, target.height, 89, 0f, 0f, 100, default, 2f);
                    Main.dust[plagueDust].velocity *= 3f;
                    if (Main.rand.NextBool())
                    {
                        Main.dust[plagueDust].scale = 0.5f;
                        Main.dust[plagueDust].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int j = 0; j < 20; j++)
                {
                    int plagueDust2 = Dust.NewDust(new Vector2(target.position.X, target.position.Y), target.width, target.height, 89, 0f, 0f, 100, default, 3f);
                    Main.dust[plagueDust2].noGravity = true;
                    Main.dust[plagueDust2].velocity *= 5f;
                    plagueDust2 = Dust.NewDust(new Vector2(target.position.X, target.position.Y), target.width, target.height, 89, 0f, 0f, 100, default, 2f);
                    Main.dust[plagueDust2].velocity *= 2f;
                }
            }
        }

        public override void OnHitPvp(Player player, Player target, Player.HurtInfo hurtInfo)
        {
            target.AddBuff(ModContent.BuffType<Plague>(), 240);
            target.AddBuff(BuffID.CursedInferno, 120);
            if (target.statLife <= (target.statLifeMax * 0.15f))
            {
                SoundEngine.PlaySound(SoundID.Item14, target.Center);
                for (int i = 0; i < 10; i++)
                {
                    int plagueDust = Dust.NewDust(new Vector2(target.position.X, target.position.Y), target.width, target.height, 89, 0f, 0f, 100, default, 2f);
                    Main.dust[plagueDust].velocity *= 3f;
                    if (Main.rand.NextBool())
                    {
                        Main.dust[plagueDust].scale = 0.5f;
                        Main.dust[plagueDust].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int j = 0; j < 20; j++)
                {
                    int plagueDust2 = Dust.NewDust(new Vector2(target.position.X, target.position.Y), target.width, target.height, 89, 0f, 0f, 100, default, 3f);
                    Main.dust[plagueDust2].noGravity = true;
                    Main.dust[plagueDust2].velocity *= 5f;
                    plagueDust2 = Dust.NewDust(new Vector2(target.position.X, target.position.Y), target.width, target.height, 89, 0f, 0f, 100, default, 2f);
                    Main.dust[plagueDust2].velocity *= 2f;
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

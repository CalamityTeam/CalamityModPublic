using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Melee;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    [LegacyName("SoulEdge")]
    public class VoidEdge : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";

        public static readonly SoundStyle ProjectileDeathSound = SoundID.Item100 with { Volume = 0.5f };

        internal const int TotalProjectilesPerSwing = 3;

        internal const int ProjectileSpreadOutTime = 20;

        internal const float ShootSpeed = 10f;

        internal const float SmallSoulStatMultiplier = 0.8f;

        internal const float MediumSoulStatMultiplier = 0.9f;

        public override void SetDefaults()
        {
            Item.width = 88;
            Item.height = 88;
            Item.damage = 210;
            Item.DamageType = DamageClass.Melee;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 24;
            Item.useAnimation = 24;
            Item.useTurn = true;
            Item.knockBack = 7f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<GhastlySoulLarge>();
            Item.shootSpeed = ShootSpeed;

            Item.value = CalamityGlobalItem.RarityPureGreenBuyPrice;
            Item.rare = ModContent.RarityType<PureGreen>();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int numShots = TotalProjectilesPerSwing;
            for (int i = 0; i < numShots; i++)
            {
                velocity = velocity.RotatedByRandom(0.35) * Main.rand.NextFloat(0.9f, 1.1f);
                float ai1 = MathHelper.Lerp(0.75f, 1.25f, Main.rand.NextFloat());
                switch (i)
                {
                    default:
                    case 0:
                        break;

                    case 1:
                        type = ModContent.ProjectileType<GhastlySoulMedium>();
                        knockback *= MediumSoulStatMultiplier;
                        break;

                    case 2:
                        type = ModContent.ProjectileType<GhastlySoulSmall>();
                        knockback *= SmallSoulStatMultiplier;
                        break;
                }
                int proj = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, 0f, ai1);
                Main.projectile[proj].netUpdate = true;
            }
            return false;
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool())
            {
                int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, DustID.RainbowTorch, 0f, 0f, 0, Color.Plum, Main.rand.NextFloat(0.65f, 1.2f));
                Main.dust[dust].noGravity = true;
            }
        }
        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            SoundStyle sound = new("CalamityMod/Sounds/Item/PhantomSpirit");
            SoundEngine.PlaySound(sound with { Volume = 0.65f, PitchVariance = 0.3f, Pitch = -0.5f }, target.Center);
            for (int i = 0; i <= 30; i++)
            {
                Dust dust = Dust.NewDustPerfect(target.Center, DustID.RainbowTorch, new Vector2(0, -18).RotatedByRandom(MathHelper.ToRadians(15f)) * Main.rand.NextFloat(0.1f, 1.9f));
                dust.noGravity = true;
                dust.scale = Main.rand.NextFloat(0.7f, 1.6f);
                Dust dust2 = Dust.NewDustPerfect(target.Center, DustID.RainbowTorch, new Vector2(0, -7).RotatedByRandom(MathHelper.ToRadians(35f)) * Main.rand.NextFloat(0.1f, 1.9f));
                dust2.noGravity = true;
                dust2.scale = Main.rand.NextFloat(0.7f, 1.6f);
            }

            float ai1 = MathHelper.Lerp(0.75f, 1.25f, Main.rand.NextFloat());
            int soulDamage = (int)(damageDone / 3);
            Vector2 velocity = new Vector2(0f, -14f).RotatedByRandom(0.65f) * Main.rand.NextFloat(0.9f, 1.1f);
            Projectile.NewProjectile(player.GetSource_ItemUse(Item), target.Center + new Vector2(0, 1300f), velocity.RotatedByRandom(0.4f) * Main.rand.NextFloat(0.9f, 1.1f), ModContent.ProjectileType<GhastlySoulLarge>(), soulDamage, 0f, player.whoAmI, 0f, ai1);
            Projectile.NewProjectile(player.GetSource_ItemUse(Item), target.Center + new Vector2(0, 1300f), velocity.RotatedByRandom(0.4f) * Main.rand.NextFloat(0.9f, 1.1f), ModContent.ProjectileType<GhastlySoulMedium>(), soulDamage, 0f, player.whoAmI, 0f, ai1);
            Projectile.NewProjectile(player.GetSource_ItemUse(Item), target.Center + new Vector2(0, 1300f), velocity.RotatedByRandom(0.4f) * Main.rand.NextFloat(0.9f, 1.1f), ModContent.ProjectileType<GhastlySoulSmall>(), soulDamage, 0f, player.whoAmI, 0f, ai1);
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<HellfireFlamberge>().
                AddIngredient<Necroplasm>(15).
                AddIngredient<RuinousSoul>(3).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}

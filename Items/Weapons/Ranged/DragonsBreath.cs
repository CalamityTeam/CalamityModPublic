using Terraria.DataStructures;
using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class DragonsBreath : ModItem
    {
        public const int BetweenShotsPause = 15;
        public const int PelletsPerShot = 6;
        public const float FullAutoFireRateMult = 1.25f;
        public const float FullAutoDamageMult = 0.8f;
        // note this is extremely low because it's per pellet
        public const float Spread = 0.018f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dragon's Breath");
            Tooltip.SetDefault("Left click fires in two-shot bursts\n" +
                "The first shot is a spread of 6 normal bullets\n" +
                "The second shot is a tight spread of 6 Dragon's Breath rounds\n" +
                "Right click fires full auto and mixes the bullets randomly, but does 20% less damage\n" +
                "This weapon has no randomness to its spread pattern\n" +
                "66% chance to not consume ammo");
            SacrificeTotal = 1;
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 198;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 124;
            Item.height = 78;

            Item.useTime = 9;
            Item.useAnimation = 18;
            Item.reuseDelay = BetweenShotsPause;
            Item.autoReuse = true;
            Item.useStyle = ItemUseStyleID.Shoot;

            Item.noMelee = true;
            Item.knockBack = 6.5f;
            // item.UseSound = SoundID.Item38;
            Item.shoot = ModContent.ProjectileType<DragonsBreathRound>();
            Item.shootSpeed = 12f;
            Item.useAmmo = AmmoID.Bullet;
            Item.Calamity().canFirePointBlankShots = true;

            Item.Calamity().customRarity = CalamityRarity.Violet;
            Item.value = CalamityGlobalItem.Rarity15BuyPrice;
        }
        public override bool CanConsumeAmmo(Item ammo, Player player) => Main.rand.NextFloat() > 0.66f;

        public override Vector2? HoldoutOffset() => new Vector2(-5, 5);

        public override bool AltFunctionUse(Player player) => true;

        public override bool CanUseItem(Player player)
        {
            Item.reuseDelay = player.altFunctionUse == 2 ? 0 : BetweenShotsPause;
            return base.CanUseItem(player);
        }

        public override float UseSpeedMultiplier(Player player) => player.altFunctionUse == 2 ? FullAutoFireRateMult : 1f;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            SoundEngine.PlaySound(SoundID.Item38, position);
            bool doDust = false;
            Vector2 vel = velocity;
            int[] bulletIDs = new int[PelletsPerShot];
            float spreadFactor = 1f;

            // Right click full auto: Randomly intermix three regular bullets and three Dragon's Breath Rounds in a wider spread
            if (player.altFunctionUse == 2)
            {
                damage = (int)(damage * FullAutoDamageMult);

                for (int i = 0; i < PelletsPerShot; ++i)
                    bulletIDs[i] = type;
                int dragonsBreathAdded = 0;
                while (dragonsBreathAdded < PelletsPerShot / 2)
                {
                    int i = Main.rand.Next(PelletsPerShot);
                    if (bulletIDs[i] == Item.shoot)
                        continue;
                    bulletIDs[i] = Item.shoot;
                    ++dragonsBreathAdded;
                }

                spreadFactor = 1.52f;
            }
            // Left click first shot: Six regular bullets, low spread
            else if (player.itemAnimation == player.itemAnimationMax - 1)
            {
                for (int i = 0; i < PelletsPerShot; ++i)
                    bulletIDs[i] = type;
            }
            // Left click second shot: Six Dragon's Breath Rounds, very low spread. Extra sound and dust blast.
            else
            {
                SoundEngine.PlaySound(SoundID.Item74, position);
                for (int i = 0; i < PelletsPerShot; ++i)
                    bulletIDs[i] = Item.shoot;

                spreadFactor = 0.5f;
                doDust = true;
            }

            // Actually fire the chosen bullets.
            float angleOffset = Spread * -0.5f * (PelletsPerShot - 1) * spreadFactor;
            for (int i = 0; i < PelletsPerShot; ++i)
            {
                Vector2 rotatedVel = vel.RotatedBy(angleOffset);
                Projectile.NewProjectile(source, position, rotatedVel, bulletIDs[i], damage, knockback, player.whoAmI);
                angleOffset += Spread * spreadFactor;

                if (doDust)
                    SpawnDragonsBreathDust(position, rotatedVel);
            }
            return false;
        }

        private void SpawnDragonsBreathDust(Vector2 pos, Vector2 velocity)
        {
            pos += velocity.SafeNormalize(Vector2.Zero) * Item.width * Item.scale * 0.71f;
            for (int i = 0; i < 30; ++i)
            {
                // Pick a random type of smoke (there's a little fire mixed in)
                int dustID;
                switch (Main.rand.Next(6))
                {
                    case 0:
                        dustID = 262;
                        break;
                    case 1:
                    case 2:
                        dustID = 54;
                        break;
                    default:
                        dustID = 53;
                        break;
                }

                // Choose a random speed and angle to belch out the smoke
                float dustSpeed = Main.rand.NextFloat(3.0f, 13.0f);
                float angleRandom = 0.06f;
                Vector2 dustVel = new Vector2(dustSpeed, 0.0f).RotatedBy(velocity.ToRotation());
                dustVel = dustVel.RotatedBy(-angleRandom);
                dustVel = dustVel.RotatedByRandom(2.0f * angleRandom);

                // Sometimes make smoke fly upward instead of outward.
                if (Main.rand.NextBool(4))
                    dustVel = Vector2.Lerp(dustVel, -Vector2.UnitY * dustVel.Length(), Main.rand.NextFloat(0.6f, 0.85f)) * 0.6f;

                // Pick a size for the smoke particle
                float scale = Main.rand.NextFloat(0.5f, 1.6f);

                // Actually spawn the smoke
                int idx = Dust.NewDust(pos, 1, 1, dustID, dustVel.X, dustVel.Y, 0, default, scale);
                Main.dust[idx].noGravity = true;
                Main.dust[idx].position = pos;
            }
        }
    }
}

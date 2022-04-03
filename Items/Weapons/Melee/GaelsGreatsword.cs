using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace CalamityMod.Items.Weapons.Melee
{
    public class GaelsGreatsword : ModItem
    {
        //Help, they're forcing me to slave away at Calamity until I die! - Dominic

        // Weapon attribute constants
        public static readonly int BaseDamage = 780;
        public static readonly float GiantSkullDamageMultiplier = 1.5f;

        // Weapon projectile attribute constants
        public static readonly int SearchDistance = 1450;
        public static readonly int ImmunityFrames = 10;
        public static readonly int SkullsplosionCooldownSeconds = 30;

        // Skull ring attribute constants
        public static readonly float SkullsplosionDamageMultiplier = 1.5f;

        // Rage gain attribute constant
        public static readonly float RagePerSecond = 0.03f; // 3% rage per second, consistent with what it was prior to rage rework

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Gael's Greatsword");
            Tooltip.SetDefault("Hand it over, that thing. Your dark soul.\n" +
                "First swing fires homing skulls\n" +
                "Second swing fires a giant, powerful skull\n" +
                "Third swing has no projectiles\n" +
                "Constantly generates rage when in use\n" +
                "Swings leave behind exploding blood trails when below 50% health\n" +
                "Replaces Rage Mode with an enormous barrage of skulls");
        }
        //NOTE: GetWeaponDamage is in the CalamityPlayer file
        public override void SetDefaults()
        {
            Item.width = 88;
            Item.height = 84;
            Item.damage = BaseDamage;
            Item.DamageType = DamageClass.Melee;
            Item.useAnimation = Item.useTime = 12;
            Item.useTurn = true;
            Item.knockBack = 9;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;

            Item.value = CalamityGlobalItem.Rarity15BuyPrice;
            Item.Calamity().customRarity = CalamityRarity.Violet;
            Item.Calamity().devItem = true;

            Item.shoot = ModContent.ProjectileType<GaelSkull>();
            Item.shootSpeed = 15f;
            Item.useStyle = ItemUseStyleID.Swing;
        }

        public override Vector2? HoldoutOffset() => new Vector2(12, 12);

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (CalamityUtils.CountProjectiles(ModContent.ProjectileType<LightningThing>()) < 3 &&
                player.statLife <= player.statLifeMax2 * 0.5f &&
                Main.myPlayer == player.whoAmI)
            {
                Point origin = (player.Center + Main.rand.Next(-300, 301) * Vector2.UnitX).ToTileCoordinates();
                if (WorldUtils.Find(origin, Searches.Chain(new Searches.Down(400), new GenCondition[]
                {
                    new Conditions.IsSolid()
                }), out Point spawnPosition))
                {
                    Projectile.NewProjectile(spawnPosition.ToWorldCoordinates(8f, 0f), Vector2.Zero, ModContent.ProjectileType<LightningThing>(), 0, 0f, player.whoAmI);
                }
            }
            if (player.itemAnimation == (int)(player.itemAnimationMax * 0.5))
            {
                player.Calamity().gaelSwipes++;
                if (player.statLife <= player.statLifeMax2 * 0.5f)
                {
                    for (int i = 0; i < 170; i++)
                    {
                        float r = (float)Math.Sqrt(Main.rand.NextDouble());
                        float t = Main.rand.NextFloat() * MathHelper.TwoPi;
                        Vector2 dustSpawn = t.ToRotationVector2() * r * Item.Size;
                        if (dustSpawn.X > Item.width / 2)
                        {
                            Dust.NewDustPerfect(player.MountedCenter + dustSpawn.RotatedBy(player.itemRotation) * player.direction, 218, Vector2.Zero).noGravity = true;
                        }
                        else
                        {
                            //Don't waste this version of "i" just because we failed. Decrease so that we can try again.
                            i--;
                            continue;
                        }
                        if (Main.rand.NextBool(100))
                        {
                            Projectile.NewProjectile(player.MountedCenter + dustSpawn.RotatedBy(player.itemRotation) * player.direction,
                                                     Vector2.Zero,
                                                     ModContent.ProjectileType<GaelExplosion>(),
                                                     (int)(Item.damage * player.MeleeDamage()),
                                                     0f,
                                                     player.whoAmI);
                        }
                    }
                }
            }
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.altFunctionUse == 2)
            {
                // Check CalamityPlayer.cs
                return false;
            }
            switch (player.Calamity().gaelSwipes % 3)
            {
                //Two small, quick skulls
                case 0:
                    int numProj = 2;
                    float rotation = MathHelper.ToRadians(10f);
                    for (int i = 0; i < numProj; i++)
                    {
                        Vector2 perturbedSpeed = new Vector2(speedX, speedY).RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numProj - 1)));
                        Projectile.NewProjectile(position, perturbedSpeed, type, damage, knockBack, player.whoAmI);
                    }
                    break;
                //Giant, slow, fading skull
                case 1:
                    int largeSkullDmg = (int)(damage * 1.5f);
                    int projectileIndex = Projectile.NewProjectile(position, new Vector2(speedX,speedY) * 0.5f, type, largeSkullDmg, knockBack, player.whoAmI, ai1:1f);
                    Main.projectile[projectileIndex].scale = 1.75f;
                    break;
            }
            return false;
        }
    }
}

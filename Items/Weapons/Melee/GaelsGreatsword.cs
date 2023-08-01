using CalamityMod.Projectiles.Melee;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace CalamityMod.Items.Weapons.Melee
{
    public class GaelsGreatsword : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";
        // Weapon attribute constants
        public static readonly int BaseDamage = 650;
        public static readonly float GiantSkullDamageMultiplier = 1.5f;

        // Weapon projectile attribute constants
        public static readonly int SearchDistance = 1450;
        public static readonly int ImmunityFrames = 10;
        public static readonly int SkullsplosionCooldownSeconds = 30;

        // Skull ring attribute constants
        public static readonly float SkullsplosionDamageMultiplier = 1.5f;
        internal static string SkullsplosionEntitySourceContext => "GaelsGreatswordRageSkullsplosion";

        // Rage gain attribute constant
        public static readonly float RagePerSecond = 0.025f; // 2.5% rage per second

        //NOTE: GetWeaponDamage is in the CalamityPlayer file
        public override void SetDefaults()
        {
            Item.width = 112;
            Item.height = 102;
            Item.damage = BaseDamage;
            Item.DamageType = DamageClass.Melee;
            Item.useAnimation = Item.useTime = 12;
            Item.useTurn = true;
            Item.knockBack = 9;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;

            Item.value = CalamityGlobalItem.Rarity15BuyPrice;
            Item.rare = ModContent.RarityType<Violet>();
            Item.Calamity().devItem = true;

            Item.shoot = ModContent.ProjectileType<GaelSkull>();
            Item.shootSpeed = 15f;
            Item.useStyle = ItemUseStyleID.Swing;
        }

        public override Vector2? HoldoutOffset() => new Vector2(12, 12);

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            var source = player.GetSource_ItemUse(Item);
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
                    Projectile.NewProjectile(source, spawnPosition.ToWorldCoordinates(8f, 0f), Vector2.Zero, ModContent.ProjectileType<LightningThing>(), 0, 0f, player.whoAmI);
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
                            int damage = (int)player.GetTotalDamage<MeleeDamageClass>().ApplyTo(Item.damage);
                            Projectile.NewProjectile(source, player.MountedCenter + dustSpawn.RotatedBy(player.itemRotation) * player.direction,
                                                     Vector2.Zero,
                                                     ModContent.ProjectileType<GaelExplosion>(),
                                                     damage,
                                                     0f,
                                                     player.whoAmI);
                        }
                    }
                }
            }
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
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
                        Vector2 perturbedSpeed = velocity.RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (float)(numProj - 1)));
                        Projectile.NewProjectile(source, position, perturbedSpeed, type, damage, knockback, player.whoAmI);
                    }
                    break;
                //Giant, slow, fading skull
                case 1:
                    int largeSkullDmg = (int)(damage * 1.5f);
                    int projectileIndex = Projectile.NewProjectile(source, position, velocity * 0.5f, type, largeSkullDmg, knockback, player.whoAmI, ai1:1f);
                    Main.projectile[projectileIndex].scale = 1.75f;
                    break;
            }
            return false;
        }
    }
}

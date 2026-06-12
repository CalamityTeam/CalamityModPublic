using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Melee;
using CalamityMod.Rarities;
using CalamityMod.Systems.Collections;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    [LegacyName("TrueTyrantYharimsUltisword")]
    public class DefiledGreatsword : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";

        public const int TotalProjectiles = 3;

        public const float ProjectileFullyVisibleDuration = 40f;

        public const float ProjectileFullyVisibleDurationIncreasePerAdditionalProjectile = 8f;

        public const float ShootSpeed = 16f;
        public override void SetStaticDefaults()
        {
            CalamityItemSets.ExtraDebuffTooltip_Enemy[Type] = [BuffID.OnFire3, BuffID.Venom];
        }

        public override void SetDefaults()
        {
            Item.width = 102;
            Item.height = 102;
            Item.damage = 119;
            Item.DamageType = DamageClass.Melee;
            Item.useAnimation = Item.useTime = 28;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTurn = true;
            Item.knockBack = 9f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.shootsEveryUse = true;
            Item.value = CalamityGlobalItem.RarityTurquoiseBuyPrice;
            Item.rare = ModContent.RarityType<Turquoise>();
            Item.shoot = ModContent.ProjectileType<BlazingPhantomBlade>();
            Item.shootSpeed = ShootSpeed;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            float adjustedItemScale = player.GetAdjustedItemScale(Item);
            for (int i = 0; i < TotalProjectiles; i++)
            {
                float ai1 = ProjectileFullyVisibleDuration + i * ProjectileFullyVisibleDurationIncreasePerAdditionalProjectile;
                float velocityMultiplier = 1f - i / (float)TotalProjectiles;
                Projectile.NewProjectile(source, player.MountedCenter, velocity * velocityMultiplier, type, (int)(damage * 0.75), knockback * 0.5f, player.whoAmI, (float)player.direction * player.gravDir, ai1, adjustedItemScale);
            }

            NetMessage.SendData(MessageID.PlayerControls, -1, -1, null, player.whoAmI);

            return false;
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(3))
            {
                int dustType = DustID.Venom;
                switch (Main.rand.Next(5))
                {
                    default:
                    case 0:
                    case 1:
                        break;

                    case 2:
                        switch (Main.rand.Next(3))
                        {
                            default:
                            case 0:
                                dustType = DustID.RedTorch;
                                break;

                            case 1:
                                dustType = DustID.YellowTorch;
                                break;

                            case 2:
                                dustType = DustID.GreenTorch;
                                break;
                        }
                        break;

                    case 3:
                        switch (Main.rand.Next(3))
                        {
                            default:
                            case 0:
                                dustType = DustID.CrimsonTorch;
                                break;

                            case 1:
                                dustType = DustID.IchorTorch;
                                break;

                            case 2:
                                dustType = DustID.CursedTorch;
                                break;
                        }
                        break;

                    case 4:
                        dustType = DustID.GreenFairy;
                        break;
                }
                int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, dustType, 0f, 0f, 100, default, Main.rand.NextFloat(1.8f, 2.4f));
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity *= 0f;
                if (dustType == DustID.Venom)
                    Main.dust[dust].fadeIn = 1.5f;
            }
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Venom, 360);
            target.AddBuff(BuffID.OnFire3, 360);
        }

        public override void OnHitPvp(Player player, Player target, Player.HurtInfo hurtInfo)
        {
            target.AddBuff(BuffID.Venom, 360);
            target.AddBuff(BuffID.OnFire3, 360);
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<BlightedCleaver>().
                AddIngredient<CoreofCalamity>().
                AddIngredient<UelibloomBar>(15).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}

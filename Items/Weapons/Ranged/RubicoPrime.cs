using System;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class RubicoPrime : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Ranged";
        public static readonly SoundStyle UseSound = new("CalamityMod/Sounds/Item/TankCannon") { PitchVariance = 0.5f };
        public static readonly SoundStyle ReloadSound = new("CalamityMod/Sounds/Item/RubicoReload") { Volume = 1.1f, PitchVariance = 0.1f };
        public static int BaseReuseDelay = 25;
        public static int ShotsToReset = 5;

        public int ShotAmount = 0;
        public override void SetDefaults()
        {
            Item.width = 82;
            Item.height = 28;
            Item.damage = 450;
            Item.DamageType = DamageClass.Ranged;
            Item.crit = 38;
            Item.knockBack = 10f;
            Item.useTime = 50;
            Item.useAnimation = 50;
            Item.reuseDelay = BaseReuseDelay;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.shoot = ModContent.ProjectileType<ImpactRound>();
            Item.shootSpeed = 12f;
            Item.useAmmo = AmmoID.Bullet;
            Item.value = CalamityGlobalItem.RarityYellowBuyPrice;
            Item.rare = ItemRarityID.Yellow;
            Item.Calamity().donorItem = true;
        }

        public override Vector2? HoldoutOffset() => new Vector2(-6, 10);

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            ShotAmount++;
            if (ShotAmount <= ShotsToReset)
            {
                SoundEngine.PlaySound(UseSound, player.Center);
                Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<ImpactRound>(), damage, knockback, player.whoAmI);
                player.velocity += velocity.SafeNormalize(Vector2.UnitX) * -6f;
            }
            else
            {
                if (ShotAmount >= ShotsToReset)
                {
                    ShotAmount = 0;
                    Item.reuseDelay = BaseReuseDelay;
                    Projectile.NewProjectile(source, player.Center, new Vector2(5 * -player.direction, -5), ModContent.ProjectileType<RubicoPrimeMag>(), 1, knockback, player.whoAmI);
                    SoundEngine.PlaySound(ReloadSound, player.Center);
                }
            }
            return false;
        }


        public override void HoldItem(Player player) => player.Calamity().mouseWorldListener = true;

        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
            player.ChangeDir(Math.Sign((player.Calamity().mouseWorld - player.Center).X));
            float itemRotation = player.compositeFrontArm.rotation + MathHelper.PiOver2 * player.gravDir;

            Vector2 itemPosition = player.MountedCenter + itemRotation.ToRotationVector2() * 35f;
            Vector2 itemSize = new Vector2(Item.width, Item.height);
            Vector2 itemOrigin = new Vector2(-5, 6);

            CalamityUtils.CleanHoldStyle(player, itemRotation, itemPosition, itemSize, itemOrigin);
            base.UseStyle(player, heldItemFrame);
        }

        // Recoil + Not having the gun aim downwards
        public override void UseItemFrame(Player player)
        {
            player.ChangeDir(Math.Sign((player.Calamity().mouseWorld - player.Center).X));

            float animProgress = 1 - player.itemTime / (float)player.itemTimeMax;
            float rotation = (player.Center - player.Calamity().mouseWorld).ToRotation() * player.gravDir + MathHelper.PiOver2;
            if (animProgress < 0.5)
                rotation += (player.altFunctionUse == 2 ? -1f : -0.45f) * (float)Math.Pow((0.5f - animProgress) / 0.5f, 2) * player.direction;
            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, rotation); //must be here otherwise it will vibrate


            //Reloads the gun 
            if (animProgress > 0.5f)
            {
                float backArmRotation = rotation + 0.52f * player.direction;

                Player.CompositeArmStretchAmount stretch = ((float)Math.Sin(MathHelper.Pi * (animProgress - 0.5f) / 0.36f)).ToStretchAmount();
                player.SetCompositeArmBack(true, stretch, backArmRotation);
            }

        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<NitroExpressRifle>().
                AddIngredient(ItemID.IllegalGunParts).
                AddIngredient<ScoriaBar>(8).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}

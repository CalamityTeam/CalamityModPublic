using System.Collections.Generic;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.NPCs.DevourerofGods;
using CalamityMod.Projectiles.Typeless;
using CalamityMod.Rarities;
using CalamityMod.Systems.Collections;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    [LegacyName("Deathwind")]
    public class ThreadOfEradication : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Ranged";

        public override void SetStaticDefaults()
        {
            CalamityItemSets.ShowScalingCritDamageTooltip[Type] = true;
            CalamityItemSets.ExtraDebuffTooltip_Enemy[Type] = [ModContent.BuffType<GodSlayerInferno>()];
        }
        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 82;
            Item.damage = 1500;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = Item.useAnimation = 45;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 5f;
            Item.value = CalamityGlobalItem.RarityDarkBlueBuyPrice;
            Item.rare = ModContent.RarityType<CosmicPurple>();
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<FriendlyLaserWallBeam>();
            Item.shootSpeed = 20f;
            Item.useAmmo = AmmoID.Arrow;
            Item.channel = true;
        }

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            Item.DrawItemGlowmaskSingleFrame(spriteBatch, rotation, ModContent.Request<Texture2D>("CalamityMod/Items/Weapons/Ranged/ThreadOfEradicationGlow").Value);
        }
        private int storedDMG = 1;
        private float storedKB = 1;
        private int storedDir = 1;
        private float storedRot = -5;
        public override void UseItemFrame(Player player)
        {
            if (player.direction == -1)
                player.itemRotation -= MathHelper.Pi;
            if (storedRot == -5)
                storedRot = player.itemRotation;
            bool flip = false;
            if (storedDir != player.direction)
                flip = true;
            if (player.itemTime < 60)
            {
                player.itemRotation = player.DirectionTo(player.Calamity().mouseWorld).ToRotation();
                if (player.itemTime == 1)
                {
                    if (player.channel)
                    {
                        player.itemTime = 180;
                        player.itemAnimation = 180;
                    }
                    else
                    {

                        if (Main.LocalPlayer.Distance(player.Center) < 1600)
                            Main.LocalPlayer.SetScreenshake(2f);
                        if (Main.myPlayer == player.whoAmI)
                        {
                            int p = Projectile.NewProjectile(player.GetSource_ItemUse(Item), player.Center + Vector2.UnitX.RotatedBy(player.itemRotation) * 2016, Vector2.UnitX.RotatedBy(player.itemRotation), ModContent.ProjectileType<FriendlyLaserWallBeam>(), storedDMG, storedKB, player.whoAmI, -1, 1);
                            if (Main.projectile.IndexInRange(p))
                                Main.projectile[p].DamageType = DamageClass.Ranged;
                        }
                    }
                }
            }
            else
            {
                player.itemRotation += MathHelper.Clamp(MathHelper.WrapAngle(player.DirectionTo(player.Calamity().mouseWorld).ToRotation() - player.itemRotation), -0.045f, 0.045f);
                if (player.itemTime == 60)
                {

                    if (Main.LocalPlayer.Distance(player.Center) < 1600)
                        Main.LocalPlayer.SetScreenshake(5f);
                    if (Main.myPlayer == player.whoAmI)
                    {
                        int p = Projectile.NewProjectile(player.GetSource_ItemUse(Item), player.Center + Vector2.UnitX.RotatedBy(player.itemRotation) * 2016, Vector2.UnitX.RotatedBy(player.itemRotation), ModContent.ProjectileType<FriendlyLaserWallBeam>(), storedDMG*4, storedKB, player.whoAmI,-0.25f,1);
                        if (Main.projectile.IndexInRange(p))
                        {
                            Main.projectile[p].DamageType = DamageClass.Ranged;
                            Main.projectile[p].scale = 4;
                        }
                    }
                    player.itemTime = 0;
                    player.itemAnimation = 0;
                }
            }
            if (player.dashDelay != -1)
            player.direction = player.itemRotation.ToRotationVector2().X.DirectionalSign();
            storedDir = player.direction;
            storedRot = player.itemRotation;
            if (player.direction == -1)
                player.itemRotation += MathHelper.Pi;
            if (flip)
            {
                player.itemRotation -= MathHelper.Pi;
                player.itemRotation *= -1;
            }
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            storedDMG = damage;
            storedKB = knockback;
            return false;
        }
    }
}

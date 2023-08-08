using CalamityMod.CalPlayer;
using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using System.Linq;
using static CalamityMod.CalamityUtils;

namespace CalamityMod.Items.Accessories
{
    [AutoloadEquip(EquipType.Back)]
    //Its not like its a renamed version of the bayonet, but i put this here more as a way to "refund" the item, so it doesnt end up rotting as an unloaded item.
    [LegacyName("MarniteBayonet")]
    public class MarniteRepulsionShield : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 30;
            Item.rare = ItemRarityID.Blue;
            Item.value = CalamityGlobalItem.Rarity1BuyPrice;
            Item.accessory = true;
            Item.defense = 2;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            MarniteRepulsionShieldPlayer modPlayer = player.GetModPlayer<MarniteRepulsionShieldPlayer>();
            modPlayer.shieldEquipped = true;

            if (player.whoAmI == Main.myPlayer)
            {
                int baseDamage = 5;
                var source = player.GetSource_Accessory(Item);
                if (player.ownedProjectileCounts[ModContent.ProjectileType<MarniteRepulsionHitbox>()] < 1)
                {
                    var hitbox = Projectile.NewProjectileDirect(source, player.Center, Vector2.Zero, ModContent.ProjectileType<MarniteRepulsionHitbox>(), baseDamage, 10f, Main.myPlayer);
                    hitbox.originalDamage = baseDamage;
                }
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddRecipeGroup("AnyGoldBar", 5).
                AddIngredient(ItemID.Granite, 15).
                AddIngredient(ItemID.Marble, 15).
                AddTile(TileID.Anvils).
                Register();

        }
    }

    public class MarniteRepulsionShieldPlayer : ModPlayer
    {
        public bool shieldEquipped = false;

        public override void ResetEffects()
        {
            shieldEquipped = false;
        }

        public override void UpdateDead()
        {
            shieldEquipped = false;
        }
    }

    public class MarniteRepulsionHitbox : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Misc";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public Player Owner => Main.player[Projectile.owner];


        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 80;
            Projectile.friendly = true;
            Projectile.DamageType = TrueMeleeNoSpeedDamageClass.Instance;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 700;
            Projectile.tileCollide = false;
            Projectile.netImportant = true;
        }

        public override void AI()
        {
            if (Owner.active && Owner.GetModPlayer<MarniteRepulsionShieldPlayer>().shieldEquipped)
            {
                Projectile.Center = Owner.Center + Vector2.UnitX * Owner.direction * -20f;
                if (Owner.mount.Active)
                {
                    Projectile.Center += -Vector2.UnitY * Owner.mount.PlayerOffset;
                }

                if (Main.rand.NextBool(6))
                {
                    Vector2 dustOrigin = Owner.MountedCenter;
                    Vector2 dustDirection = (Vector2.UnitX * -1 * Owner.direction).RotatedByRandom(MathHelper.PiOver2 * 0.93f);
                    dustOrigin += dustDirection * 14f;
                    float spikeSpeed = Main.rand.NextFloat(1f, 3f);
                    Vector2 dustVelocity = dustDirection * spikeSpeed + Owner.velocity;
                    Vector2 dustOriginOffset = dustDirection * 4f;

                    for (int i = 0; i < 5; i++)
                    {
                        Dust dust = Dust.NewDustPerfect(dustOrigin, 229, dustVelocity, 120, Scale: Main.rand.NextFloat(0.6f, 1f));
                        dust.noGravity = true;
                        dustOrigin += dustOriginOffset;
                    }
                }
            }

            else
                Projectile.active = false;
        }

        public override bool? CanHitNPC(NPC target)
        { 
            //Only enemies that are behind the player (shouldn't happen but just in case
            if (Math.Sign((Owner.Center - target.Center).X) != Owner.direction)
                return false;

            //Don't hit friendly enemies and such (it would look really off)
            if (target.CountsAsACritter || target.friendly || !target.chaseable)
                return false;

            return base.CanHitNPC(target);
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.HitDirectionOverride = Math.Sign(-Owner.direction);
        }

        public override bool? CanCutTiles() => false;
    }
}

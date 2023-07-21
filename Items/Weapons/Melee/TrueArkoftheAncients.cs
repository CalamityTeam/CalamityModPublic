using Terraria.DataStructures;
using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace CalamityMod.Items.Weapons.Melee
{
    // TODO -- CANNOT RENAME THIS to ArkoftheAncients without corrupting existing items
    public class TrueArkoftheAncients : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";
        public float Combo = 1f;
        public float Charge = 0f;
        public static float chargeDamageMultiplier = 1.45f; //Extra damage from charge
        public static float beamDamageMultiplier = 0.8f; //Damage multiplier for the charged shots (remember it applies ontop of the charge damage multiplied
        public static float glassStarDamageMultiplier = 0.4f; //Damage multiplier for the glass stars it shoots (Shoots 3x glass stars when not charged, shoots 2x glass stars + one beam when charged)

        public static float blastDamageMultiplier = 0.5f; //Damage multiplier applied ontop of the charge damage multiplier mutliplied by the amount of charges consumed. So if you consume 5 charges, the blast will get multiplied by 5 times the damage multiplier
        public static float blastFalloffSpeed = 0.1f; //How much the blast damage falls off as you hit more and more targets
        public static float blastFalloffStrenght = 0.75f; //Value between 0 and 1 that determines how much falloff increases affect the damage : Closer to 0 = damage falls off less intensely, closer to 1 : damage falls off way harder

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (tooltips == null)
                return;

            Player player = Main.player[Main.myPlayer];
            if (player is null)
                return;

            var tooltip = tooltips.FirstOrDefault(x => x.Text.Contains("[PARRY]") && x.Mod == "Terraria");
            if (tooltip != null)
            {
                tooltip.Text = Lang.SupportGlyphs(this.GetLocalizedValue("ParryInfo"));
                tooltip.OverrideColor = Color.CornflowerBlue;
            }
        }

        public override void SetDefaults()
        {
            Item.width = Item.height = 72;
            Item.damage = 188;
            Item.DamageType = DamageClass.MeleeNoSpeed;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.useAnimation = 25;
            Item.useTime = 25;
            Item.useTurn = true;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 6.5f;
            Item.UseSound = null;
            Item.autoReuse = true;
            Item.value = CalamityGlobalItem.Rarity8BuyPrice;
            Item.rare = ItemRarityID.Yellow;
            Item.shoot = ProjectileID.PurificationPowder;
            Item.shootSpeed = 12f;
        }
        public override bool AltFunctionUse(Player player) => true;

        public override bool CanUseItem(Player player)
        {
            return !Main.projectile.Any(n => n.active && n.owner == player.whoAmI && n.type == ProjectileType<TrueArkoftheAncientsSwungBlade>());
        }

        public override void HoldItem(Player player)
        {
            player.Calamity().mouseWorldListener = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                if (Charge > 0 && player.controlUp)
                {
                    float angle = player.SafeDirectionTo(Main.MouseWorld, Vector2.UnitY).ToRotation();
                    Vector2 laserVector = player.SafeDirectionTo(Main.MouseWorld, Vector2.UnitY) * 600;
                    Projectile.NewProjectile(source, player.Center + angle.ToRotationVector2() * 90f, laserVector, ProjectileType<TrueAncientBlast>(), (int)(damage * Charge * chargeDamageMultiplier * blastDamageMultiplier), 0, player.whoAmI);

                    if (Main.LocalPlayer.Calamity().GeneralScreenShakePower < 3)
                        Main.LocalPlayer.Calamity().GeneralScreenShakePower = 3;

                    Charge = 0;
                }

                else if (!Main.projectile.Any(n => n.active && n.owner == player.whoAmI && (n.type == ProjectileType<ArkoftheAncientsParryHoldout>() || n.type == ProjectileType<TrueArkoftheAncientsParryHoldout>() || n.type == ProjectileType<ArkoftheElementsParryHoldout>() || n.type == ProjectileType<ArkoftheCosmosParryHoldout>())))
                    Projectile.NewProjectile(source, player.Center, velocity, ProjectileType<TrueArkoftheAncientsParryHoldout>(), damage, 0, player.whoAmI, 0, 0);

                return false;
            }

            //Failsafe
            if (Combo != -1 && Combo != 1)
                Combo = 1;

            if (Charge > 0)
                damage = (int)(chargeDamageMultiplier * damage);
            Projectile.NewProjectile(source, player.Center, velocity, ProjectileType<TrueArkoftheAncientsSwungBlade>(), damage, knockback, player.whoAmI, Combo, Charge);
            Combo *= -1f;

            //Shoot an extra star projectile every upwards swing
            if (Combo == -1f)
            {
                //Only shoot the center star if theres no charge
                if (Charge == 0)
                    Projectile.NewProjectile(source, player.Center + Utils.SafeNormalize(velocity, Vector2.Zero) * 20, velocity, ProjectileType<AncientStar>(), (int)(damage * glassStarDamageMultiplier), knockback, player.whoAmI);

                Vector2 Shift = Utils.SafeNormalize(velocity.RotatedBy(MathHelper.PiOver2), Vector2.Zero) * 30;

                Projectile.NewProjectile(source, player.Center + Shift, velocity.RotatedBy(MathHelper.PiOver4 * 0.3f) , ProjectileType<AncientStar>(), (int)(damage * glassStarDamageMultiplier), knockback, player.whoAmI, Charge > 0 ? 1 : 0);
                Projectile.NewProjectile(source, player.Center - Shift, velocity.RotatedBy(-MathHelper.PiOver4 * 0.3f), ProjectileType<AncientStar>(), (int)(damage * glassStarDamageMultiplier), knockback, player.whoAmI, Charge > 0 ? 1 : 0);
            }


            Charge--;
            if (Charge < 0)
                Charge = 0;

            return false;
        }

        public override ModItem Clone(Item item)
        {
            var clone = base.Clone(item);

            if (clone is TrueArkoftheAncients a && item.ModItem is TrueArkoftheAncients a2)
                a.Charge = a2.Charge;

            return clone;
        }

        public override void NetSend(BinaryWriter writer)
        {
            writer.Write(Charge);
        }

        public override void NetReceive(BinaryReader reader)
        {
            Charge = reader.ReadSingle();
        }

        public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            if (Charge <= 0)
                return;

            var barBG = Request<Texture2D>("CalamityMod/UI/MiscTextures/GenericBarBack").Value;
            var barFG = Request<Texture2D>("CalamityMod/UI/MiscTextures/GenericBarFront").Value;

            float barScale = 1.5f;

            Vector2 drawPos = position + Vector2.UnitY * ( frame.Height - 2f ) * scale + Vector2.UnitX * (frame.Width - barBG.Width * barScale) * scale * 0.5f;
            Rectangle frameCrop = new Rectangle(0, 0, (int)(Charge / 10f * barFG.Width), barFG.Height);
            Color color = Main.hslToRgb((Main.GlobalTimeWrappedHourly * 0.6f) % 1, 1, 0.85f + (float)Math.Sin(Main.GlobalTimeWrappedHourly * 3f) * 0.1f);

            spriteBatch.Draw(barBG, drawPos, null, color, 0f, origin, scale * barScale, 0f, 0f);
            spriteBatch.Draw(barFG, drawPos, frameCrop, color * 0.8f, 0f, origin, scale * barScale, 0f, 0f);
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<FracturedArk>().
                AddIngredient(ItemID.Starfury).
                AddIngredient(ItemID.BrokenHeroSword).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}

using Terraria.DataStructures;
using CalamityMod.Items.Materials;
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
    public class ArkoftheElements : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";
        public float Combo = 0f;
        public float Charge = 0f;

        public const float ComboLength = 4f; //How many regular swings before the long throw happens
        public static float snapDamageMultiplier = 1.2f; //Extra damage from making the scissors snap
        public static float chargeDamageMultiplier = 1.2f; //Extra damage from charge

        public static float needleDamageMultiplier = 0.8f; //Damage multiplier for non-homing needle projectile
        public static float glassStarDamageMultiplier = 0.2f; //Damage multiplier for the homing glass stars (4 glass stars per shot)

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

            var comboTooltip = tooltips.FirstOrDefault(x => x.Text.Contains("[COMBO]") && x.Mod == "Terraria");
            if (comboTooltip != null)
            {
                comboTooltip.Text = Lang.SupportGlyphs(this.GetLocalizedValue("ComboInfo"));
                comboTooltip.OverrideColor = Color.Crimson;
            }
            var parryTooltip = tooltips.FirstOrDefault(x => x.Text.Contains("[PARRY]") && x.Mod == "Terraria");
            if (parryTooltip != null)
            {
                parryTooltip.Text = Lang.SupportGlyphs(this.GetLocalizedValue("ParryInfo"));
                parryTooltip.OverrideColor = Color.Orange;
            }
            var blastTooltip = tooltips.FirstOrDefault(x => x.Text.Contains("[BLAST]") && x.Mod == "Terraria");
            if (blastTooltip != null)
            {
                blastTooltip.Text = Lang.SupportGlyphs(this.GetLocalizedValue("BlastInfo"));
                blastTooltip.OverrideColor = Color.Gold;
            }
        }

        public override void SetDefaults()
        {
            Item.width = 112;
            Item.height = 172;
            Item.damage = 560;
            Item.DamageType = DamageClass.MeleeNoSpeed;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.useAnimation = 20;
            Item.useTime = 20;
            Item.useTurn = true;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 8.5f;
            Item.UseSound = null;
            Item.autoReuse = true;
            Item.value = CalamityGlobalItem.Rarity11BuyPrice;
            Item.rare = ItemRarityID.Purple;
            Item.shoot = ProjectileID.PurificationPowder;
            Item.shootSpeed = 16f;
        }

        // Terraria seems to really dislike high crit values in SetDefaults
        public override void ModifyWeaponCrit(Player player, ref float crit) => crit += 10;

        public override bool AltFunctionUse(Player player) => true;

        public override void HoldItem(Player player)
        {
            player.Calamity().mouseWorldListener = true;

            if (CanUseItem(player) && Combo != 4)
                Item.channel = false;

            if (Combo == 4)
                Item.channel = true;
        }

        public override bool CanUseItem(Player player)
        {
            return !Main.projectile.Any(n => n.active && n.owner == player.whoAmI && n.type == ProjectileType<ArkoftheElementsSwungBlade>());
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                if (Charge > 0 && player.controlUp)
                {
                    float angle = velocity.ToRotation();
                    Projectile.NewProjectile(source, player.Center + angle.ToRotationVector2() * 90f, velocity, ProjectileType<ArkoftheElementsSnapBlast>(), (int)(damage * Charge * chargeDamageMultiplier * blastDamageMultiplier), 0, player.whoAmI);

                    if (Main.LocalPlayer.Calamity().GeneralScreenShakePower < 3)
                        Main.LocalPlayer.Calamity().GeneralScreenShakePower = 3;

                    Charge = 0;
                }

                else if (!Main.projectile.Any(n => n.active && n.owner == player.whoAmI && (n.type == ProjectileType<ArkoftheAncientsParryHoldout>() || n.type == ProjectileType<TrueArkoftheAncientsParryHoldout>() || n.type == ProjectileType<ArkoftheElementsParryHoldout>() || n.type == ProjectileType<ArkoftheCosmosParryHoldout>())))
                    Projectile.NewProjectile(source, player.Center, velocity, ProjectileType<ArkoftheElementsParryHoldout>(), damage, 0, player.whoAmI, 0, 0);

                return false;
            }

            if (Charge > 0)
                damage = (int)(chargeDamageMultiplier * damage);
            float scissorState = Combo == ComboLength ? 2 : Combo % 2;

            Projectile.NewProjectile(source, player.Center, velocity, ProjectileType<ArkoftheElementsSwungBlade>(), damage, knockback, player.whoAmI, scissorState, Charge);

            Combo += 1;
            if (Combo > ComboLength)
                Combo = 0;




            //Shoot projectiles every upwards swing
            if (scissorState == 1f)
            {
                float empoweredNeedles = Charge > 0 ? 1f : 0f;
                Projectile.NewProjectile(source, player.Center + Utils.SafeNormalize(velocity, Vector2.Zero) * 20, velocity * 2.8f, ProjectileType<SolarNeedle>(), (int)(damage * needleDamageMultiplier), knockback, player.whoAmI, empoweredNeedles);


                Vector2 Shift = Utils.SafeNormalize(velocity.RotatedBy(MathHelper.PiOver2), Vector2.Zero) * 20;

                Projectile.NewProjectile(source, player.Center + Shift, velocity.RotatedBy(MathHelper.PiOver4 * 0.3f), ProjectileType<ElementalGlassStar>(), (int)(damage * glassStarDamageMultiplier), knockback, player.whoAmI);
                Projectile.NewProjectile(source, player.Center + Shift * 1.2f, velocity.RotatedBy(MathHelper.PiOver4 * 0.4f) * 0.8f, ProjectileType<ElementalGlassStar>(), (int)(damage * glassStarDamageMultiplier), knockback, player.whoAmI);


                Projectile.NewProjectile(source, player.Center - Shift, velocity.RotatedBy(-MathHelper.PiOver4 * 0.3f), ProjectileType<ElementalGlassStar>(), (int)(damage * glassStarDamageMultiplier), knockback, player.whoAmI);
                Projectile.NewProjectile(source, player.Center - Shift * 1.2f, velocity.RotatedBy(-MathHelper.PiOver4 * 0.4f) * 0.8f, ProjectileType<ElementalGlassStar>(), (int)(damage * glassStarDamageMultiplier), knockback, player.whoAmI);
            }

            Charge--;
            if (Charge < 0)
                Charge = 0;

            return false;
        }

        public override ModItem Clone(Item item)
        {
            var clone = base.Clone(item);

            if (clone is ArkoftheElements a && item.ModItem is ArkoftheElements a2)
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

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            float extraScale = 0.3f;
            Texture2D frontTexture = Request<Texture2D>("CalamityMod/Items/Weapons/Melee/ArkoftheElements").Value;
            Texture2D backTexture = Request<Texture2D>("CalamityMod/Items/Weapons/Melee/ArkoftheElementsBack").Value;

            float tweakedScale = scale * (1 + extraScale); //Make the scale bigger to avoid crunching of the item
            Vector2 offset = (frontTexture.Size() * extraScale / 2f) * scale;

            float backLayerOpacity = (Charge > 0) ? 1f : (float)Math.Sin(Main.GlobalTimeWrappedHourly * 0.9f) * 0.2f + 0.3f;

            spriteBatch.Draw(backTexture, position - offset, null, drawColor * backLayerOpacity, 0f, origin, tweakedScale, SpriteEffects.None, 0f); //Make the back scissor slightly transparent if the ark isnt charged
            spriteBatch.Draw(frontTexture, position - offset, null, drawColor, 0f, origin, tweakedScale, SpriteEffects.None, 0f);

            return false;
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Texture2D frontTexture = Request<Texture2D>("CalamityMod/Items/Weapons/Melee/ArkoftheElements").Value;
            Texture2D backTexture = Request<Texture2D>("CalamityMod/Items/Weapons/Melee/ArkoftheElementsBack").Value;

            spriteBatch.Draw(backTexture, Item.Center - Main.screenPosition, null, lightColor, rotation, Item.Size * 0.5f, scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(frontTexture, Item.Center - Main.screenPosition, null, lightColor, rotation, Item.Size * 0.5f, scale, SpriteEffects.None, 0f);
            return false;
        }

        public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            if (Charge <= 0)
                return;

            var barBG = Request<Texture2D>("CalamityMod/UI/MiscTextures/GenericBarBack").Value;
            var barFG = Request<Texture2D>("CalamityMod/UI/MiscTextures/GenericBarFront").Value;

            float barScale = 3.5f;

            Vector2 drawPos = position + Vector2.UnitY * (frame.Height - 10) * scale + Vector2.UnitX * (frame.Width - barBG.Width * barScale) * scale * 0.5f;
            Rectangle frameCrop = new Rectangle(0, 0, (int)(Charge / 10f * barFG.Width), barFG.Height);
            Color color = Main.hslToRgb(((float)Math.Sin(Main.GlobalTimeWrappedHourly * 0.6f) * 0.5f + 0.5f) * 0.15f, 1, 0.85f + (float)Math.Sin(Main.GlobalTimeWrappedHourly * 3f) * 0.1f);

            spriteBatch.Draw(barBG, drawPos, null, color, 0f, origin, scale * barScale, 0f, 0f);
            spriteBatch.Draw(barFG, drawPos, frameCrop, color * 0.8f, 0f, origin, scale * barScale, 0f, 0f);
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<TrueArkoftheAncients>().
                AddIngredient(ItemID.LunarBar, 5).
                AddIngredient<LifeAlloy>(5).
                AddIngredient<GalacticaSingularity>(5).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}

using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Melee;
using CalamityMod.Rarities;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace CalamityMod.Items.Weapons.Melee
{
    public class ArkoftheCosmos : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";
        public float Combo = 0f;
        public float Charge = 0f;

        public static float NeedleDamageMultiplier = 0.7f; //Damage on the non-homing needle projectile
        public static float MaxThrowReach = 760;
        public static float snapDamageMultiplier = 1.2f; //Extra damage from making the scissors snap

        public static float chargeDamageMultiplier = 1.4f; //Extra damage from charge
        public static float chainDamageMultiplier = 0.1f;

        public static int DashIframes = 10;
        public static float SlashBoltsDamageMultiplier = 0.2f;
        public static float SnapBoltsDamageMultiplier = 0.1f;

        public static float blastDamageMultiplier = 0.5f; //Damage multiplier applied ontop of the charge damage multiplier mutliplied by the amount of charges consumed. So if you consume 5 charges, the blast will get multiplied by 5 times the damage multiplier
        public static float blastFalloffSpeed = 0.1f; //How much the blast damage falls off as you hit more and more targets
        public static float blastFalloffStrenght = 0.75f; //Value between 0 and 1 that determines how much falloff increases affect the damage : Closer to 0 = damage falls off less intensely, closer to 1 : damage falls off way harder

        public static float SwirlBoltAmount = 6f; //The amount of cosmic bolts produced during hte swirl attack
        public static float SwirlBoltDamageMultiplier = 0.7f; //This is the damage multiplier for ALL THE BOLTS: Aka, said damage multiplier is divided by the amount of bolts in a swirl and the full damage multiplier is gotten if you hit all the bolts

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
                comboTooltip.OverrideColor = Color.Lerp(Color.Gold, Color.Goldenrod, 0.5f + (float)Math.Sin(Main.GlobalTimeWrappedHourly) * 0.5f);
            }

            var parryTooltip = tooltips.FirstOrDefault(x => x.Text.Contains("[PARRY]") && x.Mod == "Terraria");
            if (parryTooltip != null)
            {
                parryTooltip.Text = Lang.SupportGlyphs(this.GetLocalizedValue("ParryInfo"));
                parryTooltip.OverrideColor = Color.Lerp(Color.Cyan, Color.DeepSkyBlue, 0.5f + (float)Math.Sin(Main.GlobalTimeWrappedHourly) * 0.75f);
            }

            var blastTooltip = tooltips.FirstOrDefault(x => x.Text.Contains("[BLAST]") && x.Mod == "Terraria");
            if (blastTooltip != null)
            {
                blastTooltip.Text = Lang.SupportGlyphs(this.GetLocalizedValue("BlastInfo"));
                blastTooltip.OverrideColor = Color.Lerp(Color.HotPink, Color.Crimson, 0.5f + (float)Math.Sin(Main.GlobalTimeWrappedHourly) * 0.625f);
            }
        }

        public override void SetDefaults()
        {
            Item.width = Item.height = 136;
            Item.damage = 1700;
            Item.DamageType = DamageClass.MeleeNoSpeed;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.useAnimation = 15;
            Item.useTime = 15;
            Item.useTurn = true;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 9.5f;
            Item.UseSound = null;
            Item.autoReuse = true;
            Item.value = CalamityGlobalItem.Rarity15BuyPrice;
            Item.shoot = ProjectileID.PurificationPowder;
            Item.shootSpeed = 28f;
            Item.rare = ModContent.RarityType<Violet>();
        }

        // Terraria seems to really dislike high crit values in SetDefaults
        public override void ModifyWeaponCrit(Player player, ref float crit) => crit += 15;

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
            return !Main.projectile.Any(n => n.active && n.owner == player.whoAmI && n.type == ProjectileType<ArkoftheCosmosSwungBlade>());
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                if (Charge > 0 && player.controlUp)
                {
                    float angle = velocity.ToRotation();
                    Projectile.NewProjectile(source, player.Center + angle.ToRotationVector2() * 90f, velocity, ProjectileType<ArkoftheCosmosBlast>(), (int)(damage * Charge * chargeDamageMultiplier * blastDamageMultiplier), 0, player.whoAmI, Charge);

                    if (Main.LocalPlayer.Calamity().GeneralScreenShakePower < 3)
                        Main.LocalPlayer.Calamity().GeneralScreenShakePower = 3;

                    Charge = 0;
                }

                else if (!Main.projectile.Any(n => n.active && n.owner == player.whoAmI && (n.type == ProjectileType<ArkoftheAncientsParryHoldout>() || n.type == ProjectileType<TrueArkoftheAncientsParryHoldout>() || n.type == ProjectileType<ArkoftheElementsParryHoldout>() || n.type == ProjectileType<ArkoftheCosmosParryHoldout>())))
                    Projectile.NewProjectile(source, player.Center, velocity, ProjectileType<ArkoftheCosmosParryHoldout>(), damage, 0, player.whoAmI, 0, 0);

                return false;
            }

            if (Charge > 0)
                damage = (int)(chargeDamageMultiplier * damage);

            float scissorState = Combo == 4 ? 2 : Combo % 2;

            Projectile.NewProjectile(source, player.Center, velocity, ProjectileType<ArkoftheCosmosSwungBlade>(), damage, knockback, player.whoAmI, scissorState, Charge);


            //Shoot projectiles
            if (scissorState != 2)
            {
                Projectile.NewProjectile(source, player.Center + Utils.SafeNormalize(velocity, Vector2.Zero) * 20, velocity * 1.4f, ProjectileType<RendingNeedle>(), (int)(damage * NeedleDamageMultiplier), knockback, player.whoAmI);
            }

            Combo += 1;
            if (Combo > 4)
                Combo = 0;

            Charge--;
            if (Charge < 0)
                Charge = 0;

            return false;
        }

        public override ModItem Clone(Item item)
        {
            var clone = base.Clone(item);

            if (clone is ArkoftheCosmos a && item.ModItem is ArkoftheCosmos a2)
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
            Texture2D handleTexture = Request<Texture2D>("CalamityMod/Items/Weapons/Melee/ArkoftheCosmosHandle").Value;
            Texture2D bladeTexture = Request<Texture2D>("CalamityMod/Items/Weapons/Melee/ArkoftheCosmosGlow").Value;

            float bladeOpacity = (Charge > 0) ? 1f : MathHelper.Clamp((float)Math.Sin(Main.GlobalTimeWrappedHourly % MathHelper.Pi) * 2f, 0, 1) * 0.7f + 0.3f;

            spriteBatch.Draw(handleTexture, position, null, drawColor, 0f, origin, scale, SpriteEffects.None, 0f); //Make the back scissor slightly transparent if the ark isnt charged
            spriteBatch.Draw(bladeTexture, position, null, drawColor * bladeOpacity, 0f, origin, scale, SpriteEffects.None, 0f);

            return false;
        }

        public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            if (Charge <= 0)
                return;

            var barBG = Request<Texture2D>("CalamityMod/UI/MiscTextures/GenericBarBack").Value;
            var barFG = Request<Texture2D>("CalamityMod/UI/MiscTextures/GenericBarFront").Value;

            float barScale = 3f;
            Vector2 barOrigin = barBG.Size() * 0.5f;
            float yOffset = 50f;
            Vector2 drawPos = position + Vector2.UnitY * scale * (frame.Height - yOffset);
            Rectangle frameCrop = new Rectangle(0, 0, (int)(Charge / 10f * barFG.Width), barFG.Height);
            Color color = Main.hslToRgb((Main.GlobalTimeWrappedHourly * 0.6f) % 1, 1, 0.75f + (float)Math.Sin(Main.GlobalTimeWrappedHourly * 3f) * 0.1f);

            spriteBatch.Draw(barBG, drawPos, null, color, 0f, barOrigin, scale * barScale, 0f, 0f);
            spriteBatch.Draw(barFG, drawPos, frameCrop, color * 0.8f, 0f, barOrigin, scale * barScale, 0f, 0f);
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<FourSeasonsGalaxia>().
                AddIngredient<ArkoftheElements>().
                AddIngredient<AuricBar>(5).
                AddTile<CosmicAnvil>().
                Register();
        }
    }
}

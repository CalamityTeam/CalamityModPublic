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
    public class ArkoftheElements : ModItem
    {
        public float Combo = 0f;
        public float Charge = 0f;
        public override bool CloneNewInstances => true;

        public const float ComboLenght = 4f; //How many regular swings before the long throw happens
        public static float snapDamageMultiplier = 1.2f; //Extra damage from making the scissors snap
        public static float chargeDamageMultiplier = 1.2f; //Extra damage from charge

        public static float needleDamageMultiplier = 0.8f; //Damage multiplier for non-homing needle projectile
        public static float glassStarDamageMultiplier = 0.2f; //Damage multiplier for the homing glass stars (4 glass stars per shot)

        public static float blastDamageMultiplier = 0.5f; //Damage multiplier applied ontop of the charge damage multiplier mutliplied by the amount of charges consumed. So if you consume 5 charges, the blast will get multiplied by 5 times the damage multiplier
        public static float blastFalloffSpeed = 0.1f; //How much the blast damage falls off as you hit more and more targets
        public static float blastFalloffStrenght = 0.75f; //Value between 0 and 1 that determines how much falloff increases affect the damage : Closer to 0 = damage falls off less intensely, closer to 1 : damage falls off way harder


        const string ComboTooltip = "Performs a combo of swings, throwing the blade out every 5 swings\n" +
                "Releasing the mouse while the blade is out will throw the second half towards it, making the scissors snap\n" +
                "Snapping the scissors together increase their damage and empower your next two swings";

        const string ParryTooltip = "Using RMB will snip out the scissor blades in front of you\n" +
                "Hitting an enemy with it will parry them, granting you a small window of invulnerability\n" +
                "You can also parry projectiles and temporarily make them deal 200 less damage\n" +
                "Parrying will empower the next 10 swings of the sword, letting you use both blades at once";

        const string BlastTooltip = "Using RMB and pressing up while the Ark is empowered will throw the blades in front of you to provoke a Big Rip in spacetime, using up all your charges in the process";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ark of the Elements");
            Tooltip.SetDefault("This line gets set in ModifyTooltips\n" +
                "This line also gets set in ModifyTooltips\n" +
                "This line also gets set in ModifyTooltips\n" +
                "A heavenly pair of blades infused with the essence of Terraria, powerful enough to cut through the fabric of reality");
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (tooltips == null)
                return;

            Player player = Main.player[Main.myPlayer];
            if (player is null)
                return;

            var comboTooltip = tooltips.FirstOrDefault(x => x.Name == "Tooltip0" && x.Mod == "Terraria");
            comboTooltip.text = ComboTooltip;
            comboTooltip.overrideColor = Color.Crimson;

            var parryTooltip = tooltips.FirstOrDefault(x => x.Name == "Tooltip1" && x.Mod == "Terraria");
            parryTooltip.text = ParryTooltip;
            parryTooltip.overrideColor = Color.Orange;

            var blastTooltip = tooltips.FirstOrDefault(x => x.Name == "Tooltip2" && x.Mod == "Terraria");
            blastTooltip.text = BlastTooltip;
            blastTooltip.overrideColor = Color.Gold;
        }

        public override void SetDefaults()
        {
            Item.width = 112;
            Item.height = 172;
            Item.damage = 600;
            Item.DamageType = DamageClass.Melee;
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
        public override void GetWeaponCrit(Player player, ref int crit) => crit += 10;

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

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.altFunctionUse == 2)
            {
                if (Charge > 0 && player.controlUp)
                {
                    float angle = new Vector2(speedX, speedY).ToRotation();
                    Projectile.NewProjectile(player.Center + angle.ToRotationVector2() * 90f, new Vector2(speedX, speedY), ProjectileType<ArkoftheElementsSnapBlast>(), (int)(damage * Charge * chargeDamageMultiplier * blastDamageMultiplier), 0, player.whoAmI);

                    if (Main.LocalPlayer.Calamity().GeneralScreenShakePower < 3)
                        Main.LocalPlayer.Calamity().GeneralScreenShakePower = 3;

                    Charge = 0;
                }

                else if (!Main.projectile.Any(n => n.active && n.owner == player.whoAmI && (n.type == ProjectileType<ArkoftheAncientsParryHoldout>() || n.type == ProjectileType<TrueArkoftheAncientsParryHoldout>() || n.type == ProjectileType<ArkoftheElementsParryHoldout>() || n.type == ProjectileType<ArkoftheCosmosParryHoldout>())))
                    Projectile.NewProjectile(player.Center, new Vector2(speedX, speedY), ProjectileType<ArkoftheElementsParryHoldout>(), damage, 0, player.whoAmI, 0, 0);

                return false;
            }

            if (Charge > 0)
                damage = (int)(chargeDamageMultiplier * damage);
            float scissorState = Combo == ComboLenght ? 2 : Combo % 2;

            Projectile.NewProjectile(player.Center, new Vector2(speedX, speedY), ProjectileType<ArkoftheElementsSwungBlade>(), damage, knockBack, player.whoAmI, scissorState, Charge);

            Combo += 1;
            if (Combo > ComboLenght)
                Combo = 0;




            //Shoot projectiles every upwards swing
            if (scissorState == 1f)
            {
                Vector2 throwVector = new Vector2(speedX, speedY);
                float empoweredNeedles = Charge > 0 ? 1f : 0f;
                Projectile.NewProjectile(player.Center + Utils.SafeNormalize(throwVector, Vector2.Zero) * 20, new Vector2(speedX, speedY) * 2.8f, ProjectileType<SolarNeedle>(), (int)(damage * needleDamageMultiplier), knockBack, player.whoAmI, empoweredNeedles);


                Vector2 Shift = Utils.SafeNormalize(new Vector2(speedX, speedY).RotatedBy(MathHelper.PiOver2), Vector2.Zero) * 20;

                Projectile.NewProjectile(player.Center + Shift, throwVector.RotatedBy(MathHelper.PiOver4 * 0.3f), ProjectileType<ElementalGlassStar>(), (int)(damage * glassStarDamageMultiplier), knockBack, player.whoAmI);
                Projectile.NewProjectile(player.Center + Shift * 1.2f, throwVector.RotatedBy(MathHelper.PiOver4 * 0.4f) * 0.8f, ProjectileType<ElementalGlassStar>(), (int)(damage * glassStarDamageMultiplier), knockBack, player.whoAmI);


                Projectile.NewProjectile(player.Center - Shift, throwVector.RotatedBy(-MathHelper.PiOver4 * 0.3f), ProjectileType<ElementalGlassStar>(), (int)(damage * glassStarDamageMultiplier), knockBack, player.whoAmI);
                Projectile.NewProjectile(player.Center - Shift * 1.2f, throwVector.RotatedBy(-MathHelper.PiOver4 * 0.4f) * 0.8f, ProjectileType<ElementalGlassStar>(), (int)(damage * glassStarDamageMultiplier), knockBack, player.whoAmI);
            }

            Charge--;
            if (Charge < 0)
                Charge = 0;

            return false;
        }

        public override ModItem Clone(Item item)
        {
            var clone = base.Clone(item);

            (clone as ArkoftheElements).Charge = (item.modItem as ArkoftheElements).Charge;

            return clone;
        }
        public override ModItem Clone()
        {
            var clone = base.Clone();

            (clone as ArkoftheElements).Charge = Charge;

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

        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ItemType<TrueArkoftheAncients>()).AddIngredient(ItemType<GalacticaSingularity>(), 5).AddIngredient(ItemType<CoreofCalamity>(), 5).AddIngredient(ItemType<BarofLife>(), 5).AddIngredient(ItemID.LunarBar, 5).AddTile(TileID.LunarCraftingStation).Register();
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            float extraScale = 0.3f;
            Texture2D frontTexture = GetTexture("CalamityMod/Items/Weapons/Melee/ArkoftheElements");
            Texture2D backTexture = GetTexture("CalamityMod/Items/Weapons/Melee/ArkoftheElementsBack");

            float tweakedScale = scale * (1 + extraScale); //Make the scale bigger to avoid crunching of the item
            Vector2 offset = (frontTexture.Size() * extraScale / 2f) * scale;

            float backLayerOpacity = (Charge > 0) ? 1f : (float)Math.Sin(Main.GlobalTimeWrappedHourly * 0.9f) * 0.2f + 0.3f;

            spriteBatch.Draw(backTexture, position - offset, null, drawColor * backLayerOpacity, 0f, origin, tweakedScale, SpriteEffects.None, 0f); //Make the back scissor slightly transparent if the ark isnt charged
            spriteBatch.Draw(frontTexture, position - offset, null, drawColor, 0f, origin, tweakedScale, SpriteEffects.None, 0f);

            return false;
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Texture2D frontTexture = GetTexture("CalamityMod/Items/Weapons/Melee/ArkoftheElements");
            Texture2D backTexture = GetTexture("CalamityMod/Items/Weapons/Melee/ArkoftheElementsBack");

            spriteBatch.Draw(backTexture, Item.Center - Main.screenPosition, null, lightColor, rotation, Item.Size * 0.5f, scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(frontTexture, Item.Center - Main.screenPosition, null, lightColor, rotation, Item.Size * 0.5f, scale, SpriteEffects.None, 0f);
            return false;
        }

        public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            if (Charge <= 0)
                return;

            var barBG = GetTexture("CalamityMod/ExtraTextures/GenericBarBack");
            var barFG = GetTexture("CalamityMod/ExtraTextures/GenericBarFront");

            float barScale = 3.5f;

            Vector2 drawPos = position + Vector2.UnitY * (frame.Height - 10) * scale + Vector2.UnitX * (frame.Width - barBG.Width * barScale) * scale * 0.5f;
            Rectangle frameCrop = new Rectangle(0, 0, (int)(Charge / 10f * barFG.Width), barFG.Height);
            Color color = Main.hslToRgb(((float)Math.Sin(Main.GlobalTimeWrappedHourly * 0.6f) * 0.5f + 0.5f) * 0.15f, 1, 0.85f + (float)Math.Sin(Main.GlobalTimeWrappedHourly * 3f) * 0.1f);

            spriteBatch.Draw(barBG, drawPos, null, color, 0f, origin, scale * barScale, 0f, 0f);
            spriteBatch.Draw(barFG, drawPos, frameCrop, color * 0.8f, 0f, origin, scale * barScale, 0f, 0f);
        }
    }
}

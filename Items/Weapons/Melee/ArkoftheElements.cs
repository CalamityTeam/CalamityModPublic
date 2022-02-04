using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
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

        public const float ComboLenght = 4f; //How many regular swings before the long throw happens

        const string ParryTooltip = "Using RMB will snip out the scissor blades in front of you. Hitting an enemy with it will parry them, granting you a small window of invulnerability\n" +
                "You can also parry projectiles and temporarily make them deal 200 less damage\n" +
                "Parrying will empower the next 10 swings of the sword, letting you use both blades at once\n" +
                "Using RMB and pressing up while the Ark is charged will release all the charges in a powerful burst of energy";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ark of the Elements");
            Tooltip.SetDefault("This line gets set in ModifyTooltips\n" + 
                "A heavenly pair of blades infused with the essence of Terraria");
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            var tooltip = tooltips.FirstOrDefault(x => x.Name == "Tooltip0" && x.mod == "Terraria");
            tooltip.text = ParryTooltip;
            tooltip.overrideColor = Color.Coral;
        }

        public override void SetDefaults()
        {
            item.width = 84;
            item.damage = 115;
            item.melee = true;
            item.noUseGraphic = true;
            item.noMelee = true;
            item.useAnimation = 20;
            item.useTime = 20;
            item.useTurn = true;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.knockBack = 8.5f;
            item.UseSound = null;
            item.autoReuse = true;
            item.height = 84;
			item.value = CalamityGlobalItem.Rarity11BuyPrice;
			item.rare = ItemRarityID.Purple;
			item.shoot = ProjectileID.PurificationPowder;
            item.shootSpeed = 16f;
        }

		// Terraria seems to really dislike high crit values in SetDefaults
		public override void GetWeaponCrit(Player player, ref int crit) => crit += 10;

        public override bool AltFunctionUse(Player player) => true;

        public override void HoldItem(Player player)
        {
            if (CanUseItem(player) && Combo != 4)
                item.channel = false;

            if (Combo == 4)
                item.channel = true;
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
                    Projectile.NewProjectile(player.Center + angle.ToRotationVector2() * 90f, Vector2.Zero, ProjectileType<TrueAncientBlast>(), (int)(damage * Charge * 1.8f), 0, player.whoAmI, angle, 600);

                    if (Main.LocalPlayer.Calamity().GeneralScreenShakePower < 3)
                        Main.LocalPlayer.Calamity().GeneralScreenShakePower = 3;

                    Charge = 0;
                }

                else if (!Main.projectile.Any(n => n.active && n.owner == player.whoAmI && (n.type == ProjectileType<ArkoftheAncientsParryHoldout>() || n.type == ProjectileType<TrueArkoftheAncientsParryHoldout>() || n.type == ProjectileType<ArkoftheElementsParryHoldout>())))
                    Projectile.NewProjectile(player.Center, new Vector2(speedX, speedY), ProjectileType<ArkoftheElementsParryHoldout>(), damage, 0, player.whoAmI, 0, 0);

                return false;
            }

            if (Charge > 0)
                damage *= 2;
            float scissorState = Combo == ComboLenght ? 2 : Combo % 2;

            Projectile.NewProjectile(player.Center, new Vector2(speedX, speedY), ProjectileType<ArkoftheElementsSwungBlade>(), damage, knockBack, player.whoAmI, scissorState, Charge);
            
            Combo += 1;
            if (Combo > ComboLenght)
                Combo = 0;

            //Shoot an extra star projectile every upwards swing
            if (Combo == -1f)
            {
                //Only shoot the center star if theres no charge
                if (Charge == 0)
                    Projectile.NewProjectile(player.Center + Vector2.Normalize(new Vector2(speedX, speedY)) * 20, new Vector2(speedX, speedY), ProjectileType<AncientStar>(), (int)(damage * 0.2f), knockBack, player.whoAmI);

                Vector2 Shift = Vector2.Normalize(new Vector2(speedX, speedY).RotatedBy(MathHelper.PiOver2)) * 30;

                Projectile.NewProjectile(player.Center + Shift, new Vector2(speedX, speedY).RotatedBy(MathHelper.PiOver4 * 0.3f), ProjectileType<AncientStar>(), (int)(damage * 0.2f), knockBack, player.whoAmI, Charge > 0 ? 1 : 0);
                Projectile.NewProjectile(player.Center - Shift, new Vector2(speedX, speedY).RotatedBy(-MathHelper.PiOver4 * 0.3f), ProjectileType<AncientStar>(), (int)(damage * 0.2f), knockBack, player.whoAmI, Charge > 0 ? 1 : 0);
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

        public override void NetRecieve(BinaryReader reader)
        {
            Charge = reader.ReadInt32();
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemType<TrueArkoftheAncients>());
            recipe.AddIngredient(ItemType<GalacticaSingularity>(), 5);
            recipe.AddIngredient(ItemType<CoreofCalamity>(), 5);
            recipe.AddIngredient(ItemType<BarofLife>(), 5);
            recipe.AddIngredient(ItemID.LunarBar, 5);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            if (Charge <= 0)
                return;

            var barBG = GetTexture("CalamityMod/ExtraTextures/GenericBarBack");
            var barFG = GetTexture("CalamityMod/ExtraTextures/GenericBarFront");

            Vector2 drawPos = position + Vector2.UnitY * frame.Height * scale + Vector2.UnitX * (frame.Width - barBG.Width) * scale * 0.5f;
            Rectangle frameCrop = new Rectangle(0, 0, (int)(Charge / 10f * barFG.Width), barFG.Height);
            Color color = Main.hslToRgb(((float)Math.Sin(Main.GlobalTime * 0.6f) * 0.5f + 0.5f) * 0.15f, 1, 0.85f + (float)Math.Sin(Main.GlobalTime * 3f) * 0.1f);

            spriteBatch.Draw(barBG, drawPos, null, color, 0f, origin, scale, 0f, 0f);
            spriteBatch.Draw(barFG, drawPos, frameCrop, color * 0.8f, 0f, origin, scale, 0f, 0f);
        }
    }
}

using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Melee;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Tools
{
    public class CrystylCrusher : ModItem
    {
        private static int PickPower = 1000;
        private static float LaserSpeed = 14f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Crystyl Crusher");
            Tooltip.SetDefault("Gotta dig faster, gotta go deeper\n" +
                "Right click to swing normally");
            Item.staff[item.type] = true;
        }

        public override void SetDefaults()
        {
            item.damage = 400;
            item.knockBack = 9f;
            item.useTime = 2;
            item.useAnimation = 2;
            item.pick = PickPower;
            // tile boost intentionally missing, usually 50

            item.melee = true;
            item.noMelee = true;
            item.channel = true;
            item.width = 70;
            item.height = 70;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.shootSpeed = LaserSpeed;
            item.UseSound = mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/CrystylCharge");
            item.shoot = ModContent.ProjectileType<CrystylCrusherRay>();

            item.value = CalamityGlobalItem.Rarity16BuyPrice;
            item.Calamity().customRarity = CalamityRarity.HotPink;
            item.Calamity().devItem = true;
        }

        public override Vector2? HoldoutOrigin()
        {
            if (item.useStyle == ItemUseStyleID.SwingThrow)
                return null;
            return new Vector2(10, 10);
        }

        // Terraria seems to really dislike high crit values in SetDefaults
        public override void GetWeaponCrit(Player player, ref int crit) => crit += 25;

        public override bool AltFunctionUse(Player player) => true;

        public override void HoldItem(Player player)
        {
            if (Main.myPlayer == player.whoAmI)
                player.Calamity().rightClickListener = true;

            if (player.Calamity().mouseRight && player.ownedProjectileCounts[ModContent.ProjectileType<CrystylCrusherRay>()] <= 0)
            {
                item.shoot = ProjectileID.None;
                item.shootSpeed = 0f;
                item.tileBoost = 50;
                item.UseSound = SoundID.Item1;
                item.useStyle = ItemUseStyleID.SwingThrow;
                item.useTurn = true;
                item.autoReuse = true;
                item.noMelee = false;
            }
            else
            {
                item.shoot = ModContent.ProjectileType<CrystylCrusherRay>();
                item.shootSpeed = LaserSpeed;
                item.tileBoost = -6;
                item.UseSound = null;
                item.useStyle = ItemUseStyleID.HoldingOut;
                item.useTurn = false;
                item.autoReuse = false;
                item.noMelee = true;
            }
        }

        public override void ModifyTooltips(List<TooltipLine> list)
        {
            if (item.useStyle == ItemUseStyleID.HoldingOut)
            {
                foreach (TooltipLine line2 in list)
                {
                    if (line2.mod == "Terraria" && line2.Name == "TileBoost")
                    {
                        line2.text = "";
                    }
                }
            }
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (player.altFunctionUse == 2)
                return;

            if (Main.rand.NextBool(3))
            {
                int dustType = Utils.SelectRandom(Main.rand, new int[]
                {
                    173,
                    57,
                    58
                });
                int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, dustType);
            }
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddRecipeGroup("LunarPickaxe");
            recipe.AddIngredient(ModContent.ItemType<BlossomPickaxe>());
            recipe.AddIngredient(ModContent.ItemType<ShadowspecBar>(), 5);
            recipe.AddTile(ModContent.TileType<DraedonsForge>());
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}

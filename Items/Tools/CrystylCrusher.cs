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
            Item.staff[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 400;
            Item.knockBack = 9f;
            Item.useTime = 2;
            Item.useAnimation = 2;
            Item.pick = PickPower;
            // tile boost intentionally missing, usually 50

            Item.DamageType = DamageClass.Melee;
            Item.noMelee = true;
            Item.channel = true;
            Item.width = 70;
            Item.height = 70;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.shootSpeed = LaserSpeed;
            Item.UseSound = Mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/CrystylCharge");
            Item.shoot = ModContent.ProjectileType<CrystylCrusherRay>();

            Item.value = CalamityGlobalItem.Rarity16BuyPrice;
            Item.Calamity().customRarity = CalamityRarity.HotPink;
            Item.Calamity().devItem = true;
        }

        public override Vector2? HoldoutOrigin()
        {
            if (Item.useStyle == ItemUseStyleID.Swing)
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
                Item.shoot = ProjectileID.None;
                Item.shootSpeed = 0f;
                Item.tileBoost = 50;
                Item.UseSound = SoundID.Item1;
                Item.useStyle = ItemUseStyleID.Swing;
                Item.useTurn = true;
                Item.autoReuse = true;
                Item.noMelee = false;
            }
            else
            {
                Item.shoot = ModContent.ProjectileType<CrystylCrusherRay>();
                Item.shootSpeed = LaserSpeed;
                Item.tileBoost = -6;
                Item.UseSound = null;
                Item.useStyle = ItemUseStyleID.Shoot;
                Item.useTurn = false;
                Item.autoReuse = false;
                Item.noMelee = true;
            }
        }

        public override void ModifyTooltips(List<TooltipLine> list)
        {
            if (Item.useStyle == ItemUseStyleID.Shoot)
            {
                foreach (TooltipLine line2 in list)
                {
                    if (line2.Mod == "Terraria" && line2.Name == "TileBoost")
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
            CreateRecipe(1).AddRecipeGroup("LunarPickaxe").AddIngredient(ModContent.ItemType<BlossomPickaxe>()).AddIngredient(ModContent.ItemType<ShadowspecBar>(), 5).AddTile(ModContent.TileType<DraedonsForge>()).Register();
        }
    }
}

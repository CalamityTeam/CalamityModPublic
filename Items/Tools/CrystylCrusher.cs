using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Melee;
using CalamityMod.Rarities;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Tools
{
    public class CrystylCrusher : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Tools";
        private static int PickPower = 1000;
        private static float LaserSpeed = 14f;

        public static readonly SoundStyle ChargeSound = new("CalamityMod/Sounds/Item/CrystylCharge");

        public override void SetStaticDefaults()
        {
            Item.staff[Item.type] = true;
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 400;
            Item.knockBack = 9f;
            Item.useTime = 1;
            Item.useAnimation = 10;
            Item.pick = PickPower;
            Item.tileBoost = 50;

            Item.DamageType = DamageClass.Melee;
            Item.channel = true;
            Item.width = 70;
            Item.height = 70;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.shootSpeed = LaserSpeed;
            Item.UseSound = SoundID.Item1;
            Item.shoot = ModContent.ProjectileType<CrystylCrusherRay>();

            Item.value = CalamityGlobalItem.Rarity16BuyPrice;
            Item.rare = ModContent.RarityType<HotPink>();
            Item.Calamity().devItem = true;
        }

        public override Vector2? HoldoutOrigin()
        {
            if (Item.useStyle == ItemUseStyleID.Swing)
                return null;
            return new Vector2(10, 10);
        }

        // Terraria seems to really dislike high crit values in SetDefaults
        public override void ModifyWeaponCrit(Player player, ref float crit) => crit += 25;

        public override bool AltFunctionUse(Player player) => true;

        public override void HoldItem(Player player)
        {
            if (Main.myPlayer == player.whoAmI)
                player.Calamity().rightClickListener = true;

            if (player.Calamity().mouseRight && CanUseItem(player) && player.whoAmI == Main.myPlayer && !Main.mapFullscreen && !Main.blockMouse)
            {
                //Don't shoot out a visual blade if you already have one out
                if (Main.projectile.Any(n => n.active && n.type == ModContent.ProjectileType<CrystylCrusherRay>() && n.owner == player.whoAmI))
                    return;

                int damage = (int)player.GetTotalDamage<MeleeDamageClass>().ApplyTo(Item.damage);
                float kb = player.GetTotalKnockback<MeleeDamageClass>().ApplyTo(Item.knockBack);
                Projectile.NewProjectile(Item.GetSource_FromThis(), player.Center, Vector2.Zero, ModContent.ProjectileType<CrystylCrusherRay>(), damage, kb, player.whoAmI);
                Item.shoot = ModContent.ProjectileType<CrystylCrusherRay>();
                Item.tileBoost = int.MinValue;
                Item.autoReuse = false;
            }
            else if (player.ownedProjectileCounts[ModContent.ProjectileType<CrystylCrusherRay>()] <= 0)
            {
                Item.shoot = ProjectileID.None;
                Item.tileBoost = 50;
                Item.autoReuse = true;
            }
        }

        public override bool? UseItem(Player player)
        {
            Item.noMelee = player.altFunctionUse == 2;
            return base.UseItem(player);
        }

        public override void UseAnimation(Player player)
        {
            if (player.altFunctionUse == 2f)
            {
                Item.useStyle = ItemUseStyleID.Shoot;
                Item.UseSound = null;
                Item.useTurn = false;
            }
            else
            {
                Item.useStyle = ItemUseStyleID.Swing;
                Item.UseSound = SoundID.Item1;
                Item.useTurn = true;
            }
        }

        public override void ModifyTooltips(List<TooltipLine> list)
        {
            if (Item.useStyle == ItemUseStyleID.Shoot)
            {
                TooltipLine line = list.FirstOrDefault(x => x.Mod == "Terraria" && x.Name == "TileBoost");

                if (line != null)
                    line.Text = string.Empty;
            }
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (player.altFunctionUse == 1)
                return;

            if (Main.rand.NextBool(3))
            {
                int dustType = Utils.SelectRandom(Main.rand, new int[]
                {
                    173,
                    57,
                    58
                });
                Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, dustType);
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddRecipeGroup("LunarPickaxe").
                AddIngredient<BlossomPickaxe>().
                AddIngredient<ShadowspecBar>(5).
                AddTile<DraedonsForge>().
                Register();
        }
    }
}

using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Rogue;
using CalamityMod.Rarities;
using CalamityMod.Systems.Collections;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class Penumbra : RogueWeapon
    {
        public static float ShootSpeed = 9f;
        public override void SetStaticDefaults()
        {
            CalamityItemSets.ExtraDebuffTooltip_Enemy[Type] = [ModContent.BuffType<Daybroken>(), ModContent.BuffType<Nightwither>()];
        }
        public override void SetDefaults()
        {
            Item.width = 46;
            Item.height = 32;
            Item.autoReuse = true;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item103;
            Item.value = CalamityGlobalItem.RarityDarkBlueBuyPrice;

            Item.damage = 725;
            Item.crit = 16;
            Item.useTime = Item.useAnimation = 35;
            Item.knockBack = 8f;
            Item.shoot = ModContent.ProjectileType<PenumbraBomb>();
            Item.shootSpeed = 9f;

            Item.rare = ModContent.RarityType<CosmicPurple>();
            Item.DamageType = RogueDamageClass.Instance;
        }

        public override float StealthDamageMultiplier => 0.9f;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.Calamity().StealthStrikeAvailable())
            {
                Vector2 realPlayerPos = player.RotatedRelativePoint(player.MountedCenter, true);
                float mouseXDist = Main.screenPosition.X + Main.mouseX - realPlayerPos.X;
                float mouseYDist = Main.screenPosition.Y + (player.gravDir == -1 ? Main.screenHeight - Main.mouseY : Main.mouseY) - realPlayerPos.Y;
                if ((float.IsNaN(mouseXDist) && float.IsNaN(mouseYDist)) || (mouseXDist == 0f && mouseYDist == 0f))
                {
                    mouseXDist = player.direction;
                    mouseYDist = 0f;
                }
                realPlayerPos += new Vector2(mouseXDist, mouseYDist);
                int proj = Projectile.NewProjectile(source, realPlayerPos, -Vector2.UnitY * 0.25f, ModContent.ProjectileType<PenumbraBomb>(), damage, knockback, player.whoAmI);
                if (proj.WithinBounds(Main.maxProjectiles))
                {
                    Main.projectile[proj].Calamity().stealthStrike = true;
                    Main.projectile[proj].timeLeft = 450;
                } 
                return false;
            }
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<CosmiliteBar>(8).
                AddIngredient<NightmareFuel>(20).
                AddIngredient<RuinousSoul>(6).
                AddTile<CosmicAnvil>().
                Register();
        }
    }
}

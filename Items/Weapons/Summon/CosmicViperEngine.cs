using Terraria.DataStructures;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Ores;
using CalamityMod.Projectiles.Summon;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Summon
{
    public class CosmicViperEngine : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cosmic Viper Engine");
            Tooltip.SetDefault("Summons a cosmic gunship to shoot down your foes");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 138;
            Item.mana = 10;
            Item.width = 46;
            Item.height = 28;
            Item.useTime = Item.useAnimation = 9;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.noMelee = true;
            Item.knockBack = 6f;
            Item.UseSound = SoundID.Item15; //phaseblade sound effect
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<CosmicViperSummon>();
            Item.shootSpeed = 10f;
            Item.DamageType = DamageClass.Summon;

            Item.value = CalamityGlobalItem.Rarity14BuyPrice;
            Item.Calamity().customRarity = CalamityRarity.DarkBlue;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse != 2)
            {
                int i = Main.myPlayer;
                float num72 = Item.shootSpeed;
                player.itemTime = Item.useTime;
                Vector2 vector2 = player.RotatedRelativePoint(player.MountedCenter, true);
                float num78 = (float)Main.mouseX + Main.screenPosition.X - vector2.X;
                float num79 = (float)Main.mouseY + Main.screenPosition.Y - vector2.Y;
                if (player.gravDir == -1f)
                {
                    num79 = Main.screenPosition.Y + (float)Main.screenHeight - (float)Main.mouseY - vector2.Y;
                }
                float num80 = (float)Math.Sqrt((double)(num78 * num78 + num79 * num79));
                if ((float.IsNaN(num78) && float.IsNaN(num79)) || (num78 == 0f && num79 == 0f))
                {
                    num78 = (float)player.direction;
                    num79 = 0f;
                    num80 = num72;
                }
                else
                {
                    num80 = num72 / num80;
                }
                num78 *= num80;
                num79 *= num80;
                vector2.X = (float)Main.mouseX + Main.screenPosition.X;
                vector2.Y = (float)Main.mouseY + Main.screenPosition.Y;
                Vector2 spinningpoint = new Vector2(num78, num79);
                spinningpoint = spinningpoint.RotatedBy(MathHelper.PiOver2, default);
                int p = Projectile.NewProjectile(source, vector2.X + spinningpoint.X, vector2.Y + spinningpoint.Y, spinningpoint.X, spinningpoint.Y, type, damage, knockback, i, 0f, 1f);
                if (Main.projectile.IndexInRange(p))
                    Main.projectile[p].originalDamage = Item.damage;
            }
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<TacticalPlagueEngine>().
                AddIngredient<ExodiumClusterOre>(20).
                AddIngredient<CosmiliteBar>(10).
                AddTile<CosmicAnvil>().
                Register();
        }
    }
}

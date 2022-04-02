using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Projectiles.Rogue;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class LunarKunai : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lunar Kunai");
            Tooltip.SetDefault("Throws out a set of three kunai that ignore gravity and slightly home in on enemies\n"
                              +"After traveling enough distance, the kunai supercharge with lunar energy, homing in far more aggressively and exploding on impact\n"
                              +"Stealth strikes instantly throw eight supercharged Kunai");
        }

        public override void SafeSetDefaults()
        {
            item.width = 38;
            item.height = 38;
            item.damage = 120;
            item.maxStack = 999;
            item.consumable = true;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.useAnimation = 10;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTime = 10;
            item.knockBack = 2f;
            item.UseSound = SoundID.Item39;
            item.autoReuse = true;
            item.value = Item.sellPrice(copper: 24);
            item.rare = ItemRarityID.Red;
            item.shoot = ModContent.ProjectileType<LunarKunaiProj>();
            item.shootSpeed = 22f;
            item.Calamity().rogue = true;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            float num72 = item.shootSpeed;
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
            if (player.Calamity().StealthStrikeAvailable())
            {
                for (int i = 0; i < 8; i++)
                {
                    float num148 = num78;
                    float num149 = num79;
                    float num150 = 0.05f * (float)i;
                    num148 += (float)Main.rand.Next(-35, 36) * num150;
                    num149 += (float)Main.rand.Next(-35, 36) * num150;
                    num80 = (float)Math.Sqrt((double)(num148 * num148 + num149 * num149));
                    num80 = num72 / num80;
                    num148 *= num80;
                    num149 *= num80;
                    float x4 = vector2.X;
                    float y4 = vector2.Y;
                    int stealth = Projectile.NewProjectile(x4, y4, num148, num149, ModContent.ProjectileType<LunarKunaiProj>(), damage, knockBack, player.whoAmI, 0f, 0f);
                    if (stealth.WithinBounds(Main.maxProjectiles))
                        Main.projectile[stealth].Calamity().stealthStrike = true;
                }
            }
            else
            {
                for (int i = 0; i < 3; i++)
                {
                    float num148 = num78;
                    float num149 = num79;
                    float num150 = 0.05f * (float)i;
                    num148 += (float)Main.rand.Next(-35, 36) * num150;
                    num149 += (float)Main.rand.Next(-35, 36) * num150;
                    num80 = (float)Math.Sqrt((double)(num148 * num148 + num149 * num149));
                    num80 = num72 / num80;
                    num148 *= num80;
                    num149 *= num80;
                    float x4 = vector2.X;
                    float y4 = vector2.Y;
                    Projectile.NewProjectile(x4, y4, num148, num149, ModContent.ProjectileType<LunarKunaiProj>(), damage, knockBack, player.whoAmI, 0f, 0f);
                }
            }
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.LunarBar, 5);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this, 333);
            recipe.AddRecipe();
        }
    }
}

using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class TheMutilator : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Mutilator");
            Tooltip.SetDefault("Striking an enemy below 20% life will trigger a bloodsplosion\n" +
                "Bloodsplosions cause hearts to drop that can be picked up to heal you");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 90;
            Item.height = 90;
            Item.scale = 1.5f;
            Item.damage = 483;
            Item.DamageType = DamageClass.Melee;
            Item.useAnimation = 18;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 18;
            Item.useTurn = true;
            Item.knockBack = 8f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.shootSpeed = 10f;

            Item.value = CalamityGlobalItem.Rarity12BuyPrice;
            Item.rare = ModContent.RarityType<Turquoise>();
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            if (target.life <= (target.lifeMax * 0.2f) && target.canGhostHeal)
            {
                if (!CalamityPlayer.areThereAnyDamnBosses || Main.rand.NextBool(2))
                {
                    int heartDrop = CalamityPlayer.areThereAnyDamnBosses ? 1 : Main.rand.Next(1, 3);
                    for (int i = 0; i < heartDrop; i++)
                    {
                        Item.NewItem(player.GetSource_OnHit(target), (int)target.position.X, (int)target.position.Y, target.width, target.height, 58, 1, false, 0, false, false);
                    }
                }
                SoundEngine.PlaySound(SoundID.Item14, target.Center);
                target.position.X += target.width / 2;
                target.position.Y += target.height / 2;
                target.position.X -= target.width / 2;
                target.position.Y -= target.height / 2;
                for (int num621 = 0; num621 < 30; num621++)
                {
                    int num622 = Dust.NewDust(new Vector2(target.position.X, target.position.Y), target.width, target.height, 5, 0f, 0f, 100, default, 2f);
                    Main.dust[num622].velocity *= 3f;
                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[num622].scale = 0.5f;
                        Main.dust[num622].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int num623 = 0; num623 < 50; num623++)
                {
                    int num624 = Dust.NewDust(new Vector2(target.position.X, target.position.Y), target.width, target.height, 5, 0f, 0f, 100, default, 3f);
                    Main.dust[num624].noGravity = true;
                    Main.dust[num624].velocity *= 5f;
                    num624 = Dust.NewDust(new Vector2(target.position.X, target.position.Y), target.width, target.height, 5, 0f, 0f, 100, default, 2f);
                    Main.dust[num624].velocity *= 2f;
                }
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<BloodstoneCore>(5).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}

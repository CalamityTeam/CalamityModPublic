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
    public class TheMutilator : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";
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

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (target.life <= (target.lifeMax * 0.2f) && target.canGhostHeal)
            {
                if (!CalamityPlayer.areThereAnyDamnBosses || Main.rand.NextBool())
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
                for (int i = 0; i < 30; i++)
                {
                    int bloodDust = Dust.NewDust(new Vector2(target.position.X, target.position.Y), target.width, target.height, 5, 0f, 0f, 100, default, 2f);
                    Main.dust[bloodDust].velocity *= 3f;
                    if (Main.rand.NextBool())
                    {
                        Main.dust[bloodDust].scale = 0.5f;
                        Main.dust[bloodDust].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int j = 0; j < 50; j++)
                {
                    int bloodDust2 = Dust.NewDust(new Vector2(target.position.X, target.position.Y), target.width, target.height, 5, 0f, 0f, 100, default, 3f);
                    Main.dust[bloodDust2].noGravity = true;
                    Main.dust[bloodDust2].velocity *= 5f;
                    bloodDust2 = Dust.NewDust(new Vector2(target.position.X, target.position.Y), target.width, target.height, 5, 0f, 0f, 100, default, 2f);
                    Main.dust[bloodDust2].velocity *= 2f;
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

using CalamityMod.Buffs.Pets;
using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Head)]
    public class ReaverHeadgear : ModItem
    {
        //Exploration and Mining Helm
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Reaver Headgear");
            Tooltip.SetDefault("10% increased pick speed and 20% increased block/wall placement speed\n" +
                "Temporary immunity to lava and can move freely through liquids");
        }

        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 22;
            Item.value = Item.buyPrice(0, 30, 0, 0);
            Item.rare = ItemRarityID.Lime;
            Item.defense = 7; //40
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<ReaverScaleMail>() && legs.type == ModContent.ItemType<ReaverCuisses>();
        }

        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawShadowSubtle = true;
            player.armorEffectDrawOutlines = true;
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "Causes nearby treasure to sparkle\n" +
                "Increased item grab range and block placement range\n" +
                "Mining tiles restores breath while underwater\n" +
                "Summons a reaver orb to light up the area around you\n" +
                "Reduces enemy aggression, even in the abyss\n" +
                "Provides a small amount of light in the abyss";
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.reaverExplore = true;
            modPlayer.wearingRogueArmor = true;
            player.blockRange += 2;
            player.aggro -= 200;

            if (player.whoAmI == Main.myPlayer)
            {
                if (player.FindBuffIndex(ModContent.BuffType<ReaverOrbBuff>()) == -1)
                {
                    player.AddBuff(ModContent.BuffType<ReaverOrbBuff>(), 3600, true);
                }
                if (player.ownedProjectileCounts[ModContent.ProjectileType<ReaverOrb>()] < 1)
                {
                    Projectile.NewProjectile(player.Center, Vector2.Zero, ModContent.ProjectileType<ReaverOrb>(), 0, 0f, player.whoAmI);
                }
            }
            if (player.miscCounter % 10 == 0)
            {
                int searchDist = 17;
                int x = (int)player.Center.X / 16;
                int y = (int)player.Center.Y / 16;
                for (int i = x - searchDist; i <= x + searchDist; ++i)
                {
                    for (int j = y - searchDist; j <= y + searchDist; ++j)
                    {
                        if (Main.rand.NextBool(4) && (new Vector2((float)(x - i), (float)(y - j)).Length() < (float)searchDist && i > 0 && (i < Main.maxTilesX - 1 && j > 0) && (j < Main.maxTilesY - 1 && Main.tile[i, j] != null && Main.tile[i, j].active())))
                        {
                            bool shouldSparkle = false;
                            //Check for the money piles
                            if (Main.tile[i, j].TileType == TileID.SmallPiles && Main.tile[i, j].TileFrameY == 18)
                            {
                                if (Main.tile[i, j].TileFrameX >= 576 && Main.tile[i, j].TileFrameX <= 882)
                                    shouldSparkle = true;
                            }
                            else if (Main.tile[i, j].TileType == TileID.LargePiles && Main.tile[i, j].TileFrameX >= 864 && Main.tile[i, j].TileFrameX <= 1170)
                                shouldSparkle = true;

                            if (shouldSparkle || Main.tileSpelunker[Main.tile[i, j].TileType] || Main.tileAlch[Main.tile[i, j].TileType] && Main.tile[i, j].TileType != TileID.ImmatureHerbs)
                            {
                                int sparkle = Dust.NewDust(new Vector2((float)(i * 16), (float)(j * 16)), 16, 16, 204, 0f, 0f, 150, new Color(), 0.3f);
                                Dust dust = Main.dust[sparkle];
                                dust.fadeIn = 0.75f;
                                dust.velocity = dust.velocity * 0.1f;
                                dust.noLight = true;
                            }
                        }
                    }
                }
            }
        }

        public override void UpdateEquip(Player player)
        {
            player.ignoreWater = true;
            player.pickSpeed -= 0.1f;
            player.tileSpeed += 0.2f;
            player.wallSpeed += 0.2f;
            player.lavaMax += 420;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ModContent.ItemType<DraedonBar>(), 6).AddIngredient(ItemID.JungleSpores, 4).AddIngredient(ModContent.ItemType<EssenceofCinder>()).AddTile(TileID.MythrilAnvil).Register();
        }
    }
}

using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories.Wings
{
    [AutoloadEquip(EquipType.Wings)]
    public class HadarianWings : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories.Wings";
        public override void SetStaticDefaults()
        {
            ArmorIDs.Wing.Sets.Stats[Item.wingSlot] = new WingStats(90, 9f, 1.75f);
        }

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 36;
            Item.value = CalamityGlobalItem.Rarity7BuyPrice;
            Item.rare = ItemRarityID.Lime;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (player.controlJump && player.wingTime > 0f && player.jump == 0)
            {
                bool hovering = player.TryingToHoverDown && !player.merman;
                if (hovering)
                {
                    player.velocity.Y *= 0.2f;
                    if (player.velocity.Y > -2f && player.velocity.Y < 2f)
                    {
                        // I can't get the player to have zero y velocity (setting it to 0 doesn't work and I tried a lot of numbers)
                        player.velocity.Y = 0.105f;
                    }
                    player.wingTime += 0.75f;
                }

                if (player.velocity.Y != 0f && !hideVisual)
                {
                    float xOffset = 4f;
                    if (player.direction == 1)
                    {
                        xOffset = -40f;
                    }
                    if (!hovering || Main.rand.NextBool(3))
                    {
                        int idx = Dust.NewDust(new Vector2(player.Center.X + xOffset, player.Center.Y - 15f), 30, 30, ModContent.DustType<AstralOrange>(), 0f, 0f, 100, default, 1.75f);
                        Main.dust[idx].noGravity = true;
                        Main.dust[idx].velocity *= 0.3f;
                        if (Main.rand.NextBool(10))
                        {
                            Main.dust[idx].fadeIn = 2f;
                        }
                        Main.dust[idx].shader = GameShaders.Armor.GetSecondaryShader(player.cWings, player);
                    }
                }
            }
            player.noFallDmg = true;
        }

        public override void VerticalWingSpeeds(Player player, ref float ascentWhenFalling, ref float ascentWhenRising, ref float maxCanAscendMultiplier, ref float maxAscentMultiplier, ref float constantAscend)
        {
            ascentWhenFalling = 0.8f;
            ascentWhenRising = 0.155f;
            maxCanAscendMultiplier = 1.05f;
            maxAscentMultiplier = 2.55f;
            constantAscend = 0.13f;
        }
    }
}

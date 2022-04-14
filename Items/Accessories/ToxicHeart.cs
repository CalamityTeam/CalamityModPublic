using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Projectiles.Typeless;
using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Accessories
{
    public class ToxicHeart : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            DisplayName.SetDefault("Toxic Heart");
            Tooltip.SetDefault("Summons a plague aura around you that damages nearby enemies\n" +
        "Makes you immune to the Plague debuff ");
        }

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 32;
            Item.value = CalamityGlobalItem.Rarity8BuyPrice;
            Item.rare = ItemRarityID.Yellow;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            player.buffImmune[ModContent.BuffType<Plague>()] = true;
            int plagueCounter = 0;
            Lighting.AddLight((int)(player.Center.X / 16f), (int)(player.Center.Y / 16f), 0.05f, 1f, 0.1f);
            int num = ModContent.BuffType<Plague>();
            float num2 = 300f;
            bool flag = plagueCounter % 60 == 0;
            int num3 = (int)(50 * player.AverageDamage());
            int random = Main.rand.Next(10);
            var source = player.GetProjectileSource_Accessory(Item);
            if (player.whoAmI == Main.myPlayer)
            {
                if (random == 0)
                {
                    for (int l = 0; l < Main.maxNPCs; l++)
                    {
                        NPC nPC = Main.npc[l];
                        if (nPC.active && !nPC.friendly && nPC.damage > 0 && !nPC.dontTakeDamage && !nPC.buffImmune[num] && Vector2.Distance(player.Center, nPC.Center) <= num2)
                        {
                            if (nPC.FindBuffIndex(num) == -1)
                            {
                                nPC.AddBuff(num, 120, false);
                            }
                            if (flag)
                            {
                                if (player.whoAmI == Main.myPlayer)
                                {
                                    Projectile p = Projectile.NewProjectileDirect(source, nPC.Center, Vector2.Zero, ModContent.ProjectileType<DirectStrike>(), num3, 0f, player.whoAmI, l);
                                }
                            }
                        }
                    }
                }
            }
            plagueCounter++;
            if (plagueCounter >= 180)
            {
            }
        }
    }
}

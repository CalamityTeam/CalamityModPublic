using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Projectiles.Typeless;
using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Accessories
{
    public class ToxicHeart : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
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
            modPlayer.toxicHeart = true;
            player.buffImmune[ModContent.BuffType<Plague>()] = true;
            int plagueCounter = 0;
            Lighting.AddLight((int)(player.Center.X / 16f), (int)(player.Center.Y / 16f), 0.05f, 1f, 0.1f);
            int plagueDebuff = ModContent.BuffType<Plague>();
            bool shouldInflictPlague = plagueCounter % 60 == 0;
            int auraDamage = (int)player.GetBestClassDamage().ApplyTo(50);
            int random = Main.rand.Next(10);
            var source = player.GetSource_Accessory(Item);
            if (player.whoAmI == Main.myPlayer)
            {
                if (random == 0)
                {
                    for (int l = 0; l < Main.maxNPCs; l++)
                    {
                        NPC nPC = Main.npc[l];
                        if (nPC.IsAnEnemy() && !nPC.dontTakeDamage && !nPC.buffImmune[plagueDebuff] && Vector2.Distance(player.Center, nPC.Center) <= 300f)
                        {
                            if (nPC.FindBuffIndex(plagueDebuff) == -1)
                            {
                                nPC.AddBuff(plagueDebuff, 120, false);
                            }
                            if (shouldInflictPlague)
                            {
                                if (player.whoAmI == Main.myPlayer)
                                {
                                    Projectile.NewProjectileDirect(source, nPC.Center, Vector2.Zero, ModContent.ProjectileType<DirectStrike>(), auraDamage, 0f, player.whoAmI, l);
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
